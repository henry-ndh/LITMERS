using Microsoft.AspNetCore.Identity;

namespace TFU.Models.IdentityModels
{
	public class RoleClaimDTO : IdentityRoleClaim<long>
	{
		public virtual RoleDTO Role { get; set; }
	}

	public class UserClaimDTO : IdentityUserClaim<long>
	{
		public virtual UserDTO User { get; set; }
	}

	public class UserLoginDTO : IdentityUserLogin<long>
	{
		public virtual UserDTO User { get; set; }
	}

	public class UserRoleDTO : IdentityUserRole<long>
	{
		public virtual UserDTO User { get; set; }
		public virtual RoleDTO Role { get; set; }
	}

	public class UserTokenDTO : IdentityUserToken<long>
	{
		public virtual UserDTO User { get; set; }
	}
}
