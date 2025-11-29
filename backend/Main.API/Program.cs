using App.DAL.DataBase;
using App.Entity.Models.Enums;
using Main.Mappings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Security.Claims;
using System.Text;

namespace Main.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Serilog.Debugging.SelfLog.Enable(msg =>
            {
                File.AppendAllText("serilog-selflog.txt", msg);
            });

            var builder = WebApplication.CreateBuilder(args);

            // === Serilog ===
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
            builder.Host.UseSerilog();

            // === VNPAY ===
            var vnPaySection = builder.Configuration.GetSection("VNPayConfig");

            // === DbContext ===
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<BaseDBContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                    b => 
                    {
                        b.MigrationsAssembly("Main.API");
                        // Performance: Increase command timeout to 60 seconds
                        b.CommandTimeout(60);
                        // Performance: Enable query splitting for better performance with multiple Includes
                        b.EnableStringComparisonTranslations();
                    });
                // Performance: Enable sensitive data logging only in development
                #if DEBUG
                options.EnableSensitiveDataLogging();
                #endif
            });

            // === Dependency Injection ===
            //DependencyConfig.Register(builder.Services);
            DependencyConfig.DependencyConfig.Register(builder.Services);

            // === AutoMapper ===
            builder.Services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);

            // === CORS ===
            var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy
                        .WithOrigins(allowedOrigins ?? new string[] { })
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            // === SMTP ===
            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SMTP"));

            // === Swagger + JWT Support ===
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "MainAPI", Version = "v1" });

                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Nhập token theo định dạng: Bearer {token}",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            // === Authentication Configuration ===
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!)
                    )
                };
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
            })
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                var googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");

                options.ClientId = googleAuthNSection["ClientId"];
                options.ClientSecret = googleAuthNSection["ClientSecret"];
                options.CallbackPath = "/signin-google";

                options.Scope.Add("profile");
                options.ClaimActions.MapJsonKey("picture", "picture", "url");
                options.SaveTokens = true;

                options.Events.OnCreatingTicket = ctx =>
                {
                    var picture = ctx.User.GetProperty("picture").GetString();
                    ctx.Identity?.AddClaim(new Claim("picture", picture ?? ""));
                    return Task.CompletedTask;
                };
            });

            // === MVC + Swagger ===
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            app.UseSerilogRequestLogging();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            // === Test logging to Telegram ===
            app.MapGet("/test-telegram-error", () =>
            {
                Log.Error("Đây là lỗi thử nghiệm gửi về Telegram");
                throw new Exception("Đây là Exception test gửi về Telegram");
            });

            app.MapControllers();

            app.Run();
        }
    }
}
