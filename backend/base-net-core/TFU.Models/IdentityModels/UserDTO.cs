using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TFU.Models.IdentityModels
{
	public class UserDTO : IdentityUser<long>
	{
		public virtual ICollection<UserClaimDTO> Claims { get; set; }
		public virtual ICollection<UserLoginDTO> Logins { get; set; }
		public virtual ICollection<UserTokenDTO> Tokens { get; set; }
		public virtual ICollection<UserRoleDTO> UserRoles { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Avatar { get; set; }
		public string FacebookUserId { get; set; }
		public string GoogleUserId { get; set; }
		public string TiktokUserId { get; set; }
		public string ZaloUserId { get; set; }
	}
}
