namespace TFU.Common.Models
{
	public class GoogleUserModel
	{
		public string Id { get; set; }
		public string FamilyName { get; set; }
		public string GivenName { get; set; }
		public string ImageUrl { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
	}

	public class GoogleTokenModel
	{
		public string Access_token { get; set; }
		public int Expires_in { get; set; }
		public string Refresh_token { get; set; }
		public string Scope { get; set; }
		public string Token_type { get; set; }
		public string Id_token { get; set; }
	}
	public class GoogleResponseModel
	{
		public string Azp { get; set; }
		public string Aud { get; set; }
		public string Sub { get; set; }
		public string Email { get; set; }
		public string Picture { get; set; }
		public string Given_name { get; set; }
		public string Name { get; set; }
		public bool Email_verified { get; set; }
	}




	public class GoogleResponseRootobject
	{
		public string iss { get; set; }
		public string azp { get; set; }
		public string aud { get; set; }
		public string sub { get; set; }
		public string email { get; set; }
		public string email_verified { get; set; }
		public string at_hash { get; set; }
		public string name { get; set; }
		public string picture { get; set; }
		public string given_name { get; set; }
		public string family_name { get; set; }
		public string locale { get; set; }
		public string iat { get; set; }
		public string exp { get; set; }
		public string alg { get; set; }
		public string kid { get; set; }
		public string typ { get; set; }
	}



}
