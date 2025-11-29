using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using TFU.Models.IdentityModels;

namespace TFU.BLL.Interfaces
{
	public interface IIdentityBizLogic
	{
		Task<UserDTO> GetByEmailAsync(string email);
		Task<long> AddUserAsync(UserDTO dto, string password);
		Task<bool> UpdateAsync(UserDTO dto);
		Task<UserDTO> GetByIdAsync(long id);
		Task<bool> CheckPasswordAsync(UserDTO dto, string password);
		/// <summary>
		/// lấy thông tin user theo điện thoại
		/// </summary>
		/// <param name="phoneNumber">số điện thoại</param>
		/// <returns></returns>
		Task<UserDTO> GetByPhoneAsync(string phoneNumber);
		/// <summary>
		/// xác thực email
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="code"></param>
		/// <returns></returns>
		Task<bool> ConfirmEmailAsync(string userId, string code);
		/// <summary>
		/// thay đổi mật khẩu
		/// </summary>
		/// <param name="userId">id user cần thay đổi mật khẩu</param>
		/// <param name="passwordNew">mật khẩu mới</param>
		/// <returns></returns>
		Task<bool> ChangePassword(string userId, string passwordNew);
		/// <summary>
		/// thêm role cho người dùng theo tên role
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="roleName"></param>
		/// <returns></returns>
		Task<bool> AddRoleByNameAsync(string userId, string roleName);
		/// <summary>
		/// generate email confirm token
		/// </summary>
		/// <param name="user"></param>
		/// <returns></returns>
		Task<string> GenerateEmailConfirmationTokenAsync(UserDTO user);
		Task<string> GeneratePasswordResetTokenAsync(UserDTO user);
		Task<bool> ResetPasswordAsync(string userId, string token, string newPassword);
		Task<bool> HasPasswordAsync(UserDTO user);
		Task<IdentityResult> AddPasswordAsync(UserDTO dto, string password);
		/// <summary>
		/// Check User có ở trong Roles ko.
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="RoleName"></param>
		/// <returns></returns>
		Task<bool> CreateUpdateRoleAsync(string roleName, bool isAdmin);
		Task<bool> IsUserInRoles(long userId, string RoleName);
		/// <summary>
		/// xóa quyền của user
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		Task<bool> DeleteRoleByUser(long userId);
		/// <summary>
		/// DeleteListRole
		/// </summary>
		/// <param name="ids"></param>
		/// <returns></returns>
		Task<bool> DeleteListRole(long[] ids);

		/// <summary>
		/// Verified user can be accessed to that function/action
		/// </summary>
		Task<bool> VerifyPermission(long userId, string function, string action);

		/// <summary>
		/// Get list roles by userId
		/// </summary>
		/// <param name="userId">The user identified.</param>
		/// <returns></returns>
		Task<string[]> GetRolesAsync(long userId);

		/// <summary>
		/// lấy toàn bộ role quản trị
		/// </summary>
		/// <returns></returns>
		Task<List<RoleDTO>> GetRolesAdmin();
		Task<string> GenerateJwtToken(UserDTO user, bool isRemember, bool isAdmin, bool isCreator = false, bool isArtist = false);
	}
}
