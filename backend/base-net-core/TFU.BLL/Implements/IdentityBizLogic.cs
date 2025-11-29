using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using TFU.BLL.Interfaces;
using TFU.DAL.Interfaces;
using TFU.Models.IdentityModels;

namespace TFU.BLL.Implements
{
	public class IdentityBizLogic : IIdentityBizLogic
	{
		IIdentityRepository _identityRepository;
		public IdentityBizLogic(IIdentityRepository identityRepository)
		{
			_identityRepository = identityRepository;
		}
		public async Task<long> AddUserAsync(UserDTO dto, string password)
		{
			return await _identityRepository.AddUserAsync(dto, password);
		}
		public async Task<bool> UpdateAsync(UserDTO dto)
		{
			return await _identityRepository.UpdateAsync(dto);
		}
		public async Task<string> GenerateJwtToken(UserDTO user, bool isRemember, bool isAdmin, bool isCreator = false, bool isArtist = false)
		{
			return await _identityRepository.GenerateJwtToken(user, isRemember, isAdmin, isCreator, isArtist);
		}
		public async Task<UserDTO> GetByEmailAsync(string email)
		{
			return await _identityRepository.GetByEmailAsync(email);
		}
		public async Task<UserDTO> GetByIdAsync(long id)
		{
			return await _identityRepository.GetByIdAsync(id);
		}
		public async Task<UserDTO> GetByExternalIdAsync(string id)
		{
			return await _identityRepository.GetByExternalIdAsync(id);
		}
		public async Task<bool> CheckPasswordAsync(UserDTO dto, string password)
		{
			return await _identityRepository.CheckPasswordAsync(dto, password);
		}
		public async Task<bool> HasPasswordAsync(UserDTO dto)
		{
			return await _identityRepository.HasPasswordAsync(dto);
		}
		public async Task<IdentityResult> AddPasswordAsync(UserDTO dto, string password)
		{
			return await _identityRepository.AddPasswordAsync(dto, password);
		}
		public async Task<UserDTO> GetByPhoneAsync(string phoneNumber)
		{
			return await _identityRepository.GetByPhoneAsync(phoneNumber);
		}
		public async Task<bool> ConfirmEmailAsync(string userId, string token)
		{
			return await _identityRepository.ConfirmEmailAsync(userId, token);
		}
		public async Task<bool> ChangePassword(string userId, string passwordNew)
		{
			return await _identityRepository.ChangePassword(userId, passwordNew);
		}
		public async Task<bool> AddRoleByNameAsync(string userId, string roleName)
		{
			return await _identityRepository.AddRoleByNameAsync(userId, roleName);
		}
		public async Task<string> GenerateEmailConfirmationTokenAsync(UserDTO user)
		{
			return await _identityRepository.GenerateEmailConfirmationTokenAsync(user);
		}
		public async Task<string> GeneratePasswordResetTokenAsync(UserDTO user)
		{
			return await _identityRepository.GeneratePasswordResetTokenAsync(user);
		}
		public async Task<bool> ResetPasswordAsync(string userId, string token, string newPassword)
		{
			return await _identityRepository.ResetPasswordAsync(userId, token, newPassword);
		}

		public async Task<bool> CreateUpdateRoleAsync(string roleName, bool isAdmin)
		{
			return await _identityRepository.CreateUpdateRoleAsync(roleName, isAdmin);
		}

		public async Task<bool> IsUserInRoles(long userId, string RoleName)
		{
			var user = await _identityRepository.GetByIdAsync(userId);
			if (user == null)
				return false;
			var roles = RoleName.Split(",");
			for (int i = 0; i < roles.Length; i++)
			{
				var role = roles[i];
				bool isInRole = await _identityRepository.IsUserInRole(user, role);
				if (isInRole)
					return true;
			}
			return false;
		}
		public async Task<bool> DeleteRoleByUser(long userId)
		{
			return await _identityRepository.DeleteRoleByUser(userId);
		}
		public async Task<bool> DeleteListRole(long[] ids)
		{
			return await _identityRepository.DeleteListRole(ids);
		}
		public async Task<string[]> GetRolesAsync(long id)
		{
			return await _identityRepository.GetRolesAsync(id);
		}
		public async Task<bool> VerifyPermission(long userId, string function, string action)
		{
			return await _identityRepository.VerifyPermission(userId, $"{function}.{action}");
		}
		public async Task<List<RoleDTO>> GetRolesAdmin()
		{
			return await _identityRepository.GetRolesAdmin();
		}


	}
}
