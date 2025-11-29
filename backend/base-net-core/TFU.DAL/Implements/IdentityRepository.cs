using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TFU.Common;
using TFU.DAL.Interfaces;
using TFU.EntityFramework;
using TFU.Models.IdentityModels;

namespace TFU.DAL.Implements
{
	public class IdentityRepository : BaseRepository, IIdentityRepository
	{
		private readonly UserManager<UserDTO> _userManager;
		private readonly RoleManager<RoleDTO> _roleManager;
		private readonly ILogger<IdentityRepository> _logger;
		private readonly IConfiguration _configuration;
		public IdentityRepository(IConfiguration config, UserManager<UserDTO> userManager, RoleManager<RoleDTO> roleManager, TFUDbContext dbContext, ILogger<IdentityRepository> logger) : base(config, dbContext)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			this._logger = logger;
			_configuration = config;
		}

		public async Task<long> AddUserAsync(UserDTO dto, string password)
		{
			IdentityResult result;
			if (string.IsNullOrEmpty(password))
			{
				result = await _userManager.CreateAsync(dto);
			}
			else
			{
				result = await _userManager.CreateAsync(dto, password);
			}
			if (result.Succeeded)
				return dto.Id;
			return -1;
		}
		public async Task<bool> UpdateAsync(UserDTO dto)
		{
			var result = await _userManager.UpdateAsync(dto);
			return result.Succeeded;
		}

		public async Task<string> GenerateJwtToken(UserDTO user, bool isRemember, bool isAdmin, bool isCreator = false, bool isArtist = false)
		{
			var claims = new[] {
				new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
				new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
				new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
				new Claim(Constants.USERNAME, user.FirstName + " " +  user.LastName ?? string.Empty),
				new Claim(Constants.CLAIM_EMAIL, user.Email ?? String.Empty),
				new Claim(Constants.POLICY_VERIFY_EMAIL, user.EmailConfirmed.ToString()),
				new Claim(Constants.CLAIM_ID, user.Id.ToString()),
				new Claim(Constants.AVATAR, user.Avatar ?? "https://static.vecteezy.com/system/resources/previews/009/734/564/non_2x/default-avatar-profile-icon-of-social-media-user-vector.jpg"),
				new Claim(Constants.IS_ADMIN, isAdmin.ToString()),
				new Claim(Constants.IS_CREATOR, isCreator.ToString()),
				new Claim(Constants.IS_ARTIST, isArtist.ToString())

			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				 _configuration["Jwt:Issuer"],
				 _configuration["Jwt:Issuer"],
				 claims,
				 expires: isRemember ? DateTime.Now.AddDays(28) : DateTime.Now.AddDays(28),
				 signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public async Task<UserDTO> GetByEmailAsync(string email)
		{
			if (email.IndexOf('@') >= 0)
			{
				// Find by email:
				var result = await _userManager.FindByEmailAsync(email);
				if (result == null)
					result = await _dbContext.Users.FirstOrDefaultAsync(x => x.Email.Replace(".", "") == email.Replace(".", ""));
				return result;
			}
			// Find by userName
			return await _userManager.FindByNameAsync(email);
		}

		public async Task<UserDTO> GetByIdAsync(long id)
		{
			return await _userManager.FindByIdAsync(id.ToString());
		}

		public async Task<UserDTO> GetByExternalIdAsync(string id)
		{
			return await _userManager.FindByIdAsync(id.ToString());
		}

		public async Task<bool> CheckPasswordAsync(UserDTO user, string password)
		{
			return await _userManager.CheckPasswordAsync(user, password);
		}

		public async Task<bool> HasPasswordAsync(UserDTO user)
		{
			return await _userManager.HasPasswordAsync(user);
		}

		public async Task<IdentityResult> AddPasswordAsync(UserDTO dto, string password)
		{
			return await _userManager.AddPasswordAsync(dto, password);
		}

		/// <summary>
		/// lấy thông tin user theo điện thoại
		/// </summary>
		/// <param name="phoneNumber">số điện thoại</param>
		/// <returns></returns>
		public async Task<UserDTO> GetByPhoneAsync(string phoneNumber)
		{
			var tempPhone = phoneNumber.StartsWith(Constants.PhoneNumberVietNam) ? phoneNumber.Substring(3) : phoneNumber;
			if (!tempPhone.StartsWith("0"))
				tempPhone = $"0{tempPhone}";
			else
				tempPhone = tempPhone.Substring(1);
			tempPhone = Constants.PhoneNumberVietNam + tempPhone;
			return await _userManager.Users.Where(x => x.PhoneNumber.Equals(phoneNumber) || x.PhoneNumber.Equals(tempPhone)).AsNoTracking().FirstOrDefaultAsync();
		}

		/// <summary>
		/// xác thực email
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="code"></param>
		/// <returns></returns>
		public async Task<bool> ConfirmEmailAsync(string userId, string token)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user != null)
			{
				var decodeToken = HttpUtility.UrlDecode(token);
				var result = await _userManager.ConfirmEmailAsync(user, decodeToken);
				if (result.Succeeded)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// thay đổi mật khẩu
		/// </summary>
		/// <param name="userId">id user cần thay đổi mật khẩu</param>
		/// <param name="passwordNew">mật khẩu mới</param>
		/// <returns></returns>
		public async Task<bool> ChangePassword(string userId, string passwordNew)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user != null)
			{
				//remove password
				var remove = await _userManager.RemovePasswordAsync(user);
				if (remove.Succeeded)
				{
					var result = await _userManager.AddPasswordAsync(user, passwordNew);
					return result.Succeeded;
				}
			}
			return false;
		}

		/// <summary>
		/// thêm role cho người dùng theo tên role
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public async Task<bool> AddRoleByNameAsync(string userId, string roleName)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user != null)
			{
				var role = await _roleManager.FindByNameAsync(roleName);
				if (role != null)
				{
					var result = await _userManager.AddToRoleAsync(user, roleName);
					if (result.Succeeded)
						return true;
				}
			}
			return false;
		}
		/// <summary>
		/// generate email confirm token
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		public async Task<string> GenerateEmailConfirmationTokenAsync(UserDTO user)
		{
			return await _userManager.GenerateEmailConfirmationTokenAsync(user);
		}

		public async Task<string> GeneratePasswordResetTokenAsync(UserDTO user)
		{
			return await _userManager.GeneratePasswordResetTokenAsync(user);
		}

		public async Task<bool> ResetPasswordAsync(string userId, string token, string newPassword)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user != null)
			{
				var decodeToken = HttpUtility.UrlDecode(token);
				var result = await _userManager.ResetPasswordAsync(user, decodeToken, newPassword);

				if (!user.EmailConfirmed && result.Succeeded)
				{
					user.EmailConfirmed = true;
					await _userManager.UpdateAsync(user);
				}

				return result.Succeeded;
			}
			return false;
		}

		public async Task<bool> CreateUpdateRoleAsync(string roleName, bool isAdmin)
		{
			var roleExists = await _roleManager.RoleExistsAsync(roleName);

			if (!roleExists)
			{
				var newRole = new RoleDTO
				{
					Name = roleName,
					IsAdmin = isAdmin
				};
				var result = await _roleManager.CreateAsync(newRole);
				if (result.Succeeded)
				{
					return true;
				}
			}
			else
			{
				var role = await _roleManager.FindByNameAsync(roleName);
				if (role != null)
				{
					role.IsAdmin = isAdmin;
					var result = await _roleManager.UpdateAsync(role);
					if (result.Succeeded)
					{
						return true;
					}
				}
			}
			return false;

		}

		public async Task<bool> IsUserInRole(UserDTO user, string role)
		{
			return await _userManager.IsInRoleAsync(user, role);
		}
		/// <summary>
		/// xóa quyền của user
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		public async Task<bool> DeleteRoleByUser(long userId)
		{
			var userRoles = _dbContext.UserRoles.Where(a => a.UserId == userId);
			_dbContext.RemoveRange(userRoles);
			var result = await _dbContext.SaveChangesAsync();
			return result > 0;
		}

		/// <summary>
		/// DeleteListRole
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
		public async Task<bool> DeleteListRole(long[] ids)
		{
			if (ids != null && ids.Length > 0)
			{
				foreach (var id in ids)
				{
					var role = await _roleManager.FindByIdAsync(id.ToString());
					if (role != null)
					{
						// delete RoleClaims
						var claims = await _roleManager.GetClaimsAsync(role);
						if (claims != null && claims.Count > 0)
							foreach (var claim in claims)
								await _roleManager.RemoveClaimAsync(role, claim);

						// delete UserRoles
						var userRoles = _dbContext.UserRoles.Where(a => a.RoleId == id);
						if (userRoles != null)
							_dbContext.RemoveRange(userRoles);

						// delete Role
						await _roleManager.DeleteAsync(role);
					}
				}
				return true;
			}
			return false;
		}

		public async Task<string[]> GetRolesAsync(long userId)
		{
			try
			{
				var user = await _userManager.FindByIdAsync(userId.ToString());
				if (user == null)
					return null;
				var roles = await _userManager.GetRolesAsync(user);
				return roles.ToArray();
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<bool> VerifyPermission(long userId, string claim)
		{
			try
			{
				var roles = _dbContext.UserRoles.Where(m => m.UserId == userId).Select(m => m.RoleId);
				var claims = await _dbContext.RoleClaims.Where(m => roles.Contains(m.RoleId)).Select(m => m.ClaimValue).ToArrayAsync();
				return claims.Contains(claim);
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// lấy toàn bộ role quản trị
		/// </summary>
		/// <returns></returns>
		public async Task<List<RoleDTO>> GetRolesAdmin()
		{
			try
			{
				var roles = await _dbContext.Roles.Where(x => x.IsAdmin).ToListAsync();
				if (roles != null && roles.Any())
				{
					foreach (var role in roles)
						role.RoleClaims = await _dbContext.RoleClaims.Where(x => x.RoleId == role.Id).ToListAsync();
					return roles;
				}
				return null;
			}
			catch (Exception)
			{
				throw;
			}
		}


	}
}
