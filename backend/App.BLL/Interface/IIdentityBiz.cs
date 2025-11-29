using App.Entity.DTO.Request;
using App.Entity.DTO.Request.User;
using App.Entity.DTO.Response;
using App.Entity.Models;
using App.Entity.Models.Wapper;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.BLL.Interface
{
    public interface IIdentityBiz
    {
        Task<UserTokenResponse> Login(UserLoginRequestDTO dto);
        Task<UserModel> CreateUser(UserModel model);

        Task<UserModel> GetUsersByEmail(string email);
        Task<bool> DeleteById(int id);

        Task<bool> CheckEmailAlready(string email);

        Task<bool> ChangeIsActive(bool status, int id);

        Task<UserTokenResponse> LoginGoogleAuthenticator(TwoFactorAuthRequest dto);

        Task<string> ChangePassword(int userID, ChangePasswordDTO request);
        Task<GetUserProfile> GetProfileByUser(int userID);
        Task<GetUserProfile> UpdateProfile(int userID, UpdateProfileRequestDTO dto);

        Task<string> SendMailResetPassword(string email);

        Task<string> ResetPassword(string token, string password);

        Task<string> VerifyEmailTokenAsync(string token);
        Task<UserModel> GetInfo(long userId);
        Task<PagedResult<UserModel>> GetUserByPaging(PagingModel model);


        Task<UserTokenResponse> LoginEvent(UserLoginRequestDTO dto);
        Task<string> CreateUserGoogle(UserLoginGoogleDTO dto);
        Task<string> DeleteAccount(int userId);

    }
}
