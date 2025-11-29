using System.ComponentModel.DataAnnotations;

namespace TFU.Common.Models
{
	public class ResetPasswordModel
	{
		/// <summary>
		/// mật khẩu cũ
		/// </summary>
		[Required(ErrorMessage = Constants.Required)]
		public string PasswordOld { get; set; }

		/// <summary>
		/// mật khẩu mới
		/// </summary>
		[Required(ErrorMessage = Constants.Required)]
		[Display(Name = "Mật khẩu"), StringLength(100, ErrorMessage = Constants.PasswordStringLengthError, MinimumLength = 6)]
		[RegularExpression(Constants.REGEX_PASSWORD, ErrorMessage = Constants.PasswordInvalidFormat)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		/// <summary>
		/// nhập lại mật khẩu mới
		/// </summary>
		[DataType(DataType.Password)]
		[Required(ErrorMessage = Constants.Required)]
		[Compare("Password", ErrorMessage = Constants.ConfirmPasswordError)]
		[Display(Name = "Nhập lại mật khẩu"), StringLength(100, ErrorMessage = Constants.MaxlengthError, MinimumLength = 6)]
		public string ConfirmPassword { get; set; }

		public string Code { get; set; }
	}
}
