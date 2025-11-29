using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TFU.Models.IdentityModels
{
	public class RoleDTO : IdentityRole<long>
	{
		public virtual ICollection<UserRoleDTO> UserRoles { get; set; }
		public virtual ICollection<RoleClaimDTO> RoleClaims { get; set; }
		public bool IsAdmin { get; set; }
	}
}
