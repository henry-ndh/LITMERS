

using App.BLL.Implement;
using App.BLL.Interface;
using App.DAL.Interface;
using App.DAL.Implement;
using App.DAL.DataBase;
using System.ComponentModel;

namespace Main.API.DependencyConfig
{
    public class DependencyConfig
    {
        public static void Register(IServiceCollection services)
        {
            //General
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            services.AddHttpClient(); // For AI Service
            
            ////BLL
            services.AddScoped<IIdentityBiz, IdentityBiz>();
            services.AddScoped<ITeamBiz, TeamBiz>();
            services.AddScoped<IProjectBiz, ProjectBiz>();
            services.AddScoped<IIssueBiz, IssueBiz>();
            services.AddScoped<ICommentBiz, CommentBiz>();
            services.AddScoped<INotificationBiz, NotificationBiz>();
            services.AddScoped<IAIService, AIService>();



            // DAL
            services.AddScoped<IIdentityRepository, IdentityRepository>();
            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IIssueRepository, IssueRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();


            services.AddScoped<IPasswordHasher, PasswordHasher>();
            // Mail
            services.AddScoped<IEmailService, EmailService>();

            //JWT


            //worker
            //services.AddHostedService<OrderWoker>();


            // DB
            services.AddDbContext<BaseDBContext>();

        }
    }
}
