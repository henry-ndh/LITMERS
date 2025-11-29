using System.ComponentModel.DataAnnotations;

namespace TFU.Common.Models
{
   /// <summary>
   /// Login model
   /// </summary>
   public class LoginModel
   {
	  /// <summary>
	  /// The email.
	  /// </summary>
	  public string Email => Username;
	  [EmailAddress(ErrorMessage = "Constants.EmailAddressFormatError")]
	  public string Username { get; set; }
	  public string Password { get; set; }
	  public bool IsRemember { get; set; }
	  public long RUUID { get; set; }
	  public string Redirect { get; set; }
   }
}
