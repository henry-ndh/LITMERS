using App.BLL.Interface;
using App.DAL.Implement;
using App.DAL.Interface;
using App.Entity.DTO.Request;
using App.Entity.DTO.Request.User;
using App.Entity.DTO.Response;
using App.Entity.Models;
using App.Entity.Models.Wapper;
using Base.API;
using Base.Common;
using Base.Common.Attributes;
using Base.Common.Extension;
using EventZ.API.MiddleWare;
using Google.Authenticator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Main.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : BaseAPIController
    {
        private readonly IIdentityBiz _identityBiz;
        private readonly ILogger<AuthController> _logger;
        private readonly IIdentityRepository _identityRepository;
        private readonly IConfiguration _configuration;
        public AuthController(IIdentityBiz identityBiz, ILogger<AuthController> logger, IIdentityRepository identityRepository, IConfiguration configuration)
        {
            this._identityBiz = identityBiz;
            _logger = logger;
            _identityRepository = identityRepository;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginRequestDTO dto)
        {
            try
            {
                var result = await _identityBiz.Login(dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[AuthController]", ex.Message );
                return SaveError(ex.Message);
            }
        }


        [HttpPost("login-sysadmin")]
        public async Task<IActionResult> LoginGoogleAuthenticator(TwoFactorAuthRequest dto)
        {
            try
            {
                var result = await _identityBiz.LoginGoogleAuthenticator(dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[LoginGoogleAuthenticator] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("get-info")]
        public async Task<IActionResult> GetInfo()
        {
            try
            {
                var result = await _identityBiz.GetInfo(UserId);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[GetInfo] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                bool emailExitsting = await _identityBiz.CheckEmailAlready(request.Email);

                if (emailExitsting)
                {
                    throw new Exception("Email already exists");
                }

                var result = await _identityRepository.Register(request);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Register] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }


        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            try
            {
                var result = await _identityBiz.VerifyEmailTokenAsync(token);

                if (result.Contains("Verified"))
                    return Redirect(_configuration["ClientURL"]!);
                else
                    return SaveError(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[VerifyEmail] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }


        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO request)
        {
            try
            {
                int userID = JWTHandler.GetUserIdFromHttpContext(HttpContext);
                if (request == null)
                {
                    return BadRequest();
                }

                var response = await _identityBiz.ChangePassword(UserId, request);
                if (response == "Old password incorrect")
                {
                    return BadRequest(response);
                }

                else if (response == "User Not Found")
                {
                    return NotFound(response);
                }

                else
                {
                    return Success(response);
                }
            }
            catch (Exception ex)
            {

                _logger.LogError("[Change passwrod] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }



        [HttpGet("test-telegram-error")]
        public IActionResult TestTelegramError()
        {
            _logger.LogError("Hello tôi là bug rất vui được gặp.");
            return Ok("Đã gửi lỗi qua Telegram.");
        }

        [HttpGet("get-chat-bot-id")]
        public async Task<IActionResult> GetChatBotID()
        {

            string botToken = _configuration["Serilog:WriteTo:2:Args:token"]!;
            string url = $"https://api.telegram.org/bot{botToken}/getUpdates";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var jsonResponse = System.Text.Json.JsonSerializer.Deserialize<TelegramResponse>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (jsonResponse?.Ok == true && jsonResponse.Result?.Any() == true)
                    {
                        var chatId = jsonResponse.Result.First().Message.Chat.Id;
                        return Ok(new { ChatId = chatId });
                    }

                    return Ok(new { Message = "Không tìm thấy cập nhật nào. Hãy nhắn tin cho bot trước." });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { Error = ex.Message });
                }
            }   
        }


        [HttpGet("get-profile-by-user")]
        [Authorize]
        public async Task<IActionResult> GetProfileByUser()
        {
            try
            {

                int userID = JWTHandler.GetUserIdFromHttpContext(HttpContext);
                var response = await _identityBiz.GetProfileByUser(userID);

                if (response == null) return NotFound(new { Status = false, Message = "User not found", StatusCode = 404 });

                return Success("Get user profile is success",response);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Get profile by user] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }

        }

        /// <summary>
        /// Update user profile (name and/or avatar) - FR-005
        /// </summary>
        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _identityBiz.UpdateProfile(UserId, dto);
                return Success("Profile updated successfully", response);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Update profile] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Delete user account (soft delete) - FR-007
        /// </summary>
        [HttpDelete("delete-account")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount()
        {
            try
            {
                var result = await _identityBiz.DeleteAccount(UserId);
                
                if (result == "Please delete owned teams or transfer ownership first")
                {
                    return Conflict(result);
                }

                if (result == "User not found")
                {
                    return NotFound(result);
                }

                return Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Delete account] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Send password reset email
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ResetPasswordModel request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return BadRequest("Email is required");
                }

                var result = await _identityBiz.SendMailResetPassword(request.Email);

                // Always return success message for security (don't reveal if email exists)
                return Success("If the email exists, a password reset link has been sent to your email.");
            }
            catch (Exception ex)
            {
                _logger.LogError("[Forgot Password] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Send password reset email (legacy endpoint)
        /// </summary>
        [HttpPost("send-mail-reset-password")]
        public async Task<IActionResult> SendMailResetPassword([FromForm] string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return BadRequest("Email is required");
                }

                var result = await _identityBiz.SendMailResetPassword(email);

                // Always return success message for security
                return Success("If the email exists, a password reset link has been sent to your email.");
            }
            catch (Exception ex)
            {
                _logger.LogError("[Send Mail Reset Password] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        /// <summary>
        /// Reset password using token from email
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Token))
                {
                    return BadRequest("Token is required");
                }

                if (string.IsNullOrWhiteSpace(request.NewPassword))
                {
                    return BadRequest("New password is required");
                }

                if (request.NewPassword.Length < 6)
                {
                    return BadRequest("Password must be at least 6 characters long");
                }

                var result = await _identityBiz.ResetPassword(request.Token, request.NewPassword);

                if (result == "Password has been reset successfully")
                {
                    return Success(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Reset Password] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        [HttpGet("get-user-by-paging")]
        [HAuthorize(RoleConstant.ADMIN)]
        public async Task<IActionResult> GetUserByPaging([FromQuery] PagingModel model)
        {
            try
            {
                var result = await _identityBiz.GetUserByPaging(model);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Get User By Paging] {0} {1}", ex.Message, ex.StackTrace);
                return SaveError(ex.Message);
            }
        }

        [HttpPost("e-login")]
        public async Task<IActionResult> LoginEvent([FromBody] UserLoginRequestDTO dto)
        {
            try
            {
                var result = await _identityBiz.LoginEvent(dto);
                return SaveSuccess(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("[Login Event] {0} {1}", ex.Message, ex.StackTrace);
                return Error(ex.Message);
            }
        }

        [HttpGet("login-google")]
        public IActionResult LoginWithGoogle()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Auth");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal?.Claims;

            var userId = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var avatar = claims?.FirstOrDefault(c => c.Type == "picture")?.Value;

            var UserLoginGoogleDTO = new UserLoginGoogleDTO
            {
                GoogleId = userId,
                Email = email,
                Name = name,
                Avatar = avatar
            };

            var token = await _identityBiz.CreateUserGoogle(UserLoginGoogleDTO);
            var clientURL = _configuration["ClientURL"]!;
            return Redirect($"{clientURL}/api/auth/google-callback?token={token}");
        }
    }
}

