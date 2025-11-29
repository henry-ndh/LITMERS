using App.DAL.DataBase;
using App.DAL.Interface;
using App.Entity.DTO.Request;
using App.Entity.DTO.Request.User;
using App.Entity.DTO.Response;
using App.Entity.Models;
using App.Entity.Models.Enums;
using App.Entity.Models.Wapper;
using Base.Common;
using Base.Common.Extension;
using BCrypt.Net;
using EventZ.API.MiddleWare;
using Google.Authenticator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Transactions;
using static QRCoder.PayloadGenerator;

namespace App.DAL.Implement
{
    public class IdentityRepository : AppBaseRepository, IIdentityRepository
    {
        private readonly PasswordHasher<UserModel> _passwordHasher;
        private readonly BaseDBContext _dbContext;
        private readonly IConfiguration _configuration;
        private static readonly Random _random = new Random();
        private readonly IServiceProvider _serviceProvider;
        private readonly IPasswordHasher _hasher;
        private readonly IMemoryCache _cache;
        private IEmailService EmailService =>
        _serviceProvider.GetRequiredService<IEmailService>();
        private readonly Queue<(string to, string subject, string template, Dictionary<string, string> placeholders)> _emailQueue = new();

        public IdentityRepository(BaseDBContext dbContext, 
                                    IConfiguration configuration, 
                                    IServiceProvider serviceProvider, 
                                    IPasswordHasher hasher,
                                    IMemoryCache cache) : base(dbContext)
        {
            _dbContext = dbContext;
            _passwordHasher = new PasswordHasher<UserModel>();
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _hasher = hasher;
            _cache = cache;
        }

        public async Task<UserTokenResponse> Login(UserLoginRequestDTO dto)
        {
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
            if (existingUser == null)
            {
                throw new Exception(Constants.UserNotFound);
            }
            if (existingUser.IsActive == false)
            {
                throw new Exception(Constants.UserBanned);
            }
            var token = GenerateToken(existingUser);

            return new UserTokenResponse
            {
                AccessToken = token,
            };
        }



        public async Task<UserModel> CreateUser(UserModel model)
        {
            var existingUser = await _dbContext.Users.Where(x => x.Email == model.Email).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                throw new Exception(Constants.UserExisted);
            }
            else
            {
                var hashedPassword = HashPassword(model, model.Password);

                var newUser = new UserModel
                {
                    Name = model.Name,
                    Email = model.Email,
                    Password = hashedPassword,
                    Avatar = null,
                    Role = model.Role,
                    CreatedAt = DateTime.Now,
                };
                await _dbContext.Users.AddAsync(newUser);
                await _dbContext.SaveChangesAsync();

                var placeholders = new Dictionary<string, string>
{
                    { "Name", model.Email },
                    { "UserName", model.Email },
                    { "Password", model.Password }
                };
                QueueEmail(newUser.Email, "Chào mừng", "register.html", placeholders);
                await ProcessEmailQueueAsync();
                return newUser;
            }

        }

        private void QueueEmail(string to, string subject, string template, Dictionary<string, string> placeholders)
        {
            _emailQueue.Enqueue((to, subject, template, placeholders));
        }

        public async Task ProcessEmailQueueAsync()
        {
            while (_emailQueue.Count > 0)
            {
                var email = _emailQueue.Dequeue();
                try
                {
                    await EmailService.SendEmailAsync(email.to, email.subject, email.template, email.placeholders);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Gửi email thất bại đến {email.to}: {ex.Message}");
                }
            }
        }

        public async Task<UserModel> GetUsersByEmail(string email)
        {
            return await _dbContext.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
        }

        public async Task<UserModel> GetUsersById(int id)
        {
            return await _dbContext.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public string HashPassword(UserModel user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        public bool VerifyPassword(UserModel user, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return result == PasswordVerificationResult.Success;
        }

        public bool CheckPassword(string providePass, string hashPass)
        {
            return BCrypt.Net.BCrypt.Verify(providePass, hashPass);
        }

        public string GenerateToken(UserModel userModel)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userModel.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{userModel.Name}"),
                new Claim(ClaimTypes.Email, userModel.Email),
                new Claim(Constants.ROLE, userModel.Role.ToString()),
                new Claim(Constants.CLAIM_EMAIL, userModel.Email),
                new Claim(Constants.CLAIM_ID, userModel.Id.ToString()),
            };
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<bool> DeleteById(int id)
        {
            var existing = await _dbContext.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (existing == null)
            {
                throw new Exception($"[DeleteById] user not found with id {id}");
            }
            _dbContext.Users.Remove(existing);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<string> DeleteAccount(int userId)
        {
            // FR-007: Account Deletion with soft delete
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                return "User not found";
            }

            // Check for owned teams
            var ownedTeams = await _dbContext.Set<TeamModel>()
                .Where(t => t.OwnerId == userId && t.DeletedAt == null)
                .ToListAsync();

            if (ownedTeams.Any())
            {
                return "Please delete owned teams or transfer ownership first";
            }

            // Soft delete: set IsActive = false
            user.IsActive = false;
            user.UpdatedAt = Utils.GetCurrentVNTime();
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return "Account deleted successfully";
        }

        public async Task<bool> ChangeIsActive(bool status, int id)
        {
            var existingUser = await _dbContext.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (existingUser == null)
            {
                throw new Exception($"[ChangeIsActive] user not found with id {id}");
            }
            existingUser.IsActive = status;
            _dbContext.Users.Update(existingUser);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<UserTokenResponse> LoginGoogleAuthenticator(TwoFactorAuthRequest dto)
        {
            var email = "sysadmin@gmail.com";
            var secretKey = "M7GHR74X2YD6Z3FR";
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            bool isValid = tfa.ValidateTwoFactorPIN(secretKey, dto.OTPCode);
            if (isValid)
            {
                var existingUser = await GetUsersByEmail(email);
                if (existingUser == null)
                {
                    throw new Exception(Constants.UserNotFound);
                }
                var token = GenerateToken(existingUser);
                return new UserTokenResponse
                {
                    AccessToken = token,
                };
            }
            else
            {
                throw new Exception("OTP code is incorrect");
            }
        }

        public async Task<long> Register(RegisterDTO request)
        {
            await BeginTransaction();
            try
            {
                var register = new UserModel
                {

                    Email = request.Email,
                    Name = request.Name,
                    CreatedAt = Utils.GetCurrentVNTime(),
                    IsActive = true,
                    Password = _hasher.HashPassword(request.Password)
                };
                await _dbContext.Users.AddAsync(register);
                await _dbContext.SaveChangesAsync();
                
                await CommitTransaction();
                var domain = _configuration["BackendURL"];
                var verifyEmailToken = Utils.GenerateVerifyEmailToken(_configuration, request.Email);
                var link = $"{domain}/api/Auth/verify-email?token={verifyEmailToken}";
                var placeholders = new Dictionary<string, string>
                {
                    { "VERIFY_LINK", link }
                };

                await EmailService.SendWorker(register.Email,
                    "Xác thực tài khoản",
                    "email-verify.html",
                placeholders);

                return register.Id;
            }
            catch (Exception ex) {
                await RollbackTransaction();
                throw new Exception("Error: " + ex.ToString());
            }
        }

        public async Task<bool> CheckEmailAlready(string email)
        {
            var checkEmail = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()));
            if (checkEmail != null)
            {
                return true;
            }

            return false;
        }

        public async Task<string> VerifyEmailTokenAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var email = jwtToken.Subject;

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                    return "User Not Found";

                user.UpdatedAt = DateTime.UtcNow;

                await _dbContext.SaveChangesAsync();

                return "Verified";
            }
            catch
            {
                return "Token is invalid or expired";
            }
        }

        public async Task<string> ChangePassword(int userID, ChangePasswordDTO request)
        {
            try
            {
                var getUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userID);
                if (getUser == null) return "User Not Found";

                // FR-006: Disable password change for Google OAuth users (users without password)
                if (string.IsNullOrEmpty(getUser.Password))
                {
                    return "This account was registered via Google and does not have a password. Password change is not available for Google OAuth accounts.";
                }

                var isOldPasswordCorrect = _hasher.VerifyPassword(request.OldPassword, getUser.Password);
                if (!isOldPasswordCorrect)
                    return "Old password incorrect";

                getUser.Password = _hasher.HashPassword(request.NewPassword);
                _dbContext.Update(getUser);
                await _dbContext.SaveChangesAsync();

                return "Change password success";
            }
            catch (Exception ex)
            {
                throw new Exception($"Change password failed: {ex.Message}");
            }
        }


        public async Task<LoginResponse> Login(LoginDTO request)
        {
            var checkLogin = await _dbContext.Users
                .Include(ur => ur.Email)
                .FirstOrDefaultAsync(x =>
                    (x.Email.ToLower().Equals(request.EmailOrUserName.ToLower()) 
                    || x.Email.ToLower().Equals(request.EmailOrUserName.ToLower()))
                );

            if (checkLogin != null)
            {
                if (BCrypt.Net.BCrypt.Verify(request.Password, checkLogin.Password))
                {
                    if (checkLogin != null)
                    {
                        var domain = _configuration["ClientURL"];
                        var verifyEmailToken = Utils.GenerateVerifyEmailToken(_configuration, checkLogin.Email);
                        var link = $"{domain}/Auth/verify-email?token={verifyEmailToken}";
                        var placeholders = new Dictionary<string, string>
                        {
                            { "VERIFY_LINK", link }
                        };

                        var success = await EmailService.SendEmailAsync(
                            checkLogin.Email,
                            "Xác thực tài khoản",
                            "email-verify.html",
                            placeholders
                       );

                        return new LoginResponse
                        {  
                            Email = checkLogin.Email,
                            AccessToken = null,
                            RefeshToken = null,
                            Message = "Email not verified"
                        };
                    }
                    var accessToken = JWTHandler.GenerateJWT(checkLogin, _configuration);
                    var refreshToken = Utils.GenerateRefreshToken();

                    await _dbContext.SaveChangesAsync();

                    return new LoginResponse
                    {
                        Email = checkLogin.Email,
                        AccessToken = accessToken,
                        RefeshToken = refreshToken,
                    };
                }
                else
                {
                    return null;
                }
            }

            return null;
        }
        
      

        public async Task<GetUserProfile> GetProfileByUser(int userID)
        {

            var getUserProfile = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userID);

            if (getUserProfile == null)
            {
                return null;
            }
            var userProfile = new GetUserProfile
            {
                Id = getUserProfile.Id,
                Email = getUserProfile.Email,
                Avatar = getUserProfile.Avatar,
                IsActive = getUserProfile.IsActive,
                Gender = getUserProfile.Gender,
                Name = getUserProfile.Name,
            };


            return userProfile;
        }

        public async Task<GetUserProfile> UpdateProfile(int userID, UpdateProfileRequestDTO dto)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userID);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // FR-005: Update name and/or avatar
            if (!string.IsNullOrWhiteSpace(dto.Name))
            {
                if (dto.Name.Length < 1 || dto.Name.Length > 50)
                {
                    throw new Exception("Name must be between 1 and 50 characters");
                }
                user.Name = dto.Name;
            }

            if (dto.Avatar != null) // Allow setting to empty string
            {
                if (dto.Avatar.Length > 500)
                {
                    throw new Exception("Avatar URL must be less than 500 characters");
                }
                user.Avatar = dto.Avatar;
            }

            user.UpdatedAt = Utils.GetCurrentVNTime();
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return new GetUserProfile
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Avatar = user.Avatar,
                IsActive = user.IsActive,
                Gender = user.Gender,
            };
        }

        public async Task<string> SendMailResetPassword(string email)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.IsActive);

            if (user == null)
            {
                // Don't reveal if email exists or not for security
                return "If the email exists, a password reset link has been sent.";
            }

            // Check if user has password (not Google-only account)
            if (string.IsNullOrEmpty(user.Password))
            {
                return "This account uses Google authentication. Please use Google sign-in.";
            }

            // Generate unique token
            var token = Guid.NewGuid().ToString();
            var expiresAt = Utils.GetCurrentVNTime().AddHours(1); // Token expires in 1 hour (FR-003 requirement)

            // Invalidate old tokens for this user
            var oldTokens = await _dbContext.Set<PasswordResetTokenModel>()
                .Where(prt => prt.UserId == user.Id && prt.UsedAt == null && prt.ExpiresAt > Utils.GetCurrentVNTime())
                .ToListAsync();

            foreach (var oldToken in oldTokens)
            {
                oldToken.UsedAt = Utils.GetCurrentVNTime();
            }
            if (oldTokens.Any())
            {
                _dbContext.Set<PasswordResetTokenModel>().UpdateRange(oldTokens);
            }

            // Create new reset token
            var resetToken = new PasswordResetTokenModel
            {
                UserId = user.Id,
                Token = token,
                ExpiresAt = expiresAt,
                CreatedAt = Utils.GetCurrentVNTime()
            };

            await _dbContext.Set<PasswordResetTokenModel>().AddAsync(resetToken);
            await _dbContext.SaveChangesAsync();

            // Send email with reset link
            var clientURL = _configuration["ClientURL"] ?? "http://localhost:3000";
            var resetLink = $"{clientURL}/reset-password?token={token}";

            var placeholders = new Dictionary<string, string>
            {
                { "RESET_LINK", resetLink },
                { "EMAIL", user.Email },
                { "USER_NAME", user.Name ?? "User" }
            };

            var success = await EmailService.SendEmailAsync(
                user.Email,
                "Reset your password",
                "email-reset-password.html",
                placeholders
            );

            return success
                ? "Password reset email sent successfully."
                : "Failed to send password reset email.";
        }

        public async Task<string> ResetPassword(string token, string newPassword)
        {
            try
            {
                // Find reset token
                var resetToken = await _dbContext.Set<PasswordResetTokenModel>()
                    .Include(prt => prt.User)
                    .Where(prt => prt.Token == token && prt.UsedAt == null)
                    .FirstOrDefaultAsync();

                if (resetToken == null)
                {
                    return "Invalid or expired token";
                }

                // Check if token is expired
                if (resetToken.ExpiresAt < Utils.GetCurrentVNTime())
                {
                    return "Token has expired. Please request a new password reset link.";
                }

                // Check if token is already used
                if (resetToken.UsedAt != null)
                {
                    return "This token has already been used. Please request a new password reset link.";
                }

                // Validate new password
                if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                {
                    return "Password must be at least 6 characters long";
                }

                // Update user password
                var user = resetToken.User;
                if (user == null)
                {
                    return "User not found";
                }

                user.Password = _hasher.HashPassword(newPassword);
                _dbContext.Users.Update(user);

                // Mark token as used
                resetToken.UsedAt = Utils.GetCurrentVNTime();
                _dbContext.Set<PasswordResetTokenModel>().Update(resetToken);

                await _dbContext.SaveChangesAsync();

                return "Password has been reset successfully";
            }
            catch (Exception ex)
            {
                return $"An error occurred during password reset: {ex.Message}";
            }
        }

        public async Task<UserModel> GetInfo(long userId)
        {
            return await _dbContext.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
        }

        public async Task<PagedResult<UserModel>> GetUserByPaging(PagingModel model)
        {
            var query = _dbContext.Users.AsQueryable();
            if (!string.IsNullOrEmpty(model.Keyword))
            {
                query = query.Where(x => x.Email.Contains(model.Keyword));
            };
            return await query.ToPagedResultAsync(model.PageNumber, model.PageSize);
        }

        public async Task<UserTokenResponse> LoginEvent(UserLoginRequestDTO dto)
        {
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);
            if (existingUser == null)
            {
                throw new Exception(Constants.UserNotFound);
            }
            if (existingUser.IsActive == false)
            {
                throw new Exception(Constants.UserBanned);
            }

            if (!CheckPassword(dto.Password, existingUser.Password))
            {
                throw new Exception(Constants.PasswordIsInCorrect);
            }
            var token = GenerateToken(existingUser);

            return new UserTokenResponse
            {
                AccessToken = token,
            };
        }

        public async Task<string> CreateUserGoogle(UserLoginGoogleDTO dto)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email == dto.Email && x.GoogleId != string.Empty);

            if (user != null)
            {
                user.GoogleId = dto.GoogleId;
                user.Avatar = user.Avatar ?? dto.Avatar;
                user.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                var ( firstName, lastName )= Utils.SplitName(dto.Name);
                user = new UserModel
                {
                    Email = dto.Email,
                    Password = null,
                    Avatar = dto.Avatar,
                    Role = UserRole.USER,
                    UserOrigin = UserOrigin.Google,
                    GoogleId = dto.GoogleId,
                    CreatedAt = Utils.GetCurrentVNTime(),
                };
                await _dbContext.Users.AddAsync(user);
            }

            await _dbContext.SaveChangesAsync();
            var token = GenerateToken(user);
            return token;
        }

    }
}
