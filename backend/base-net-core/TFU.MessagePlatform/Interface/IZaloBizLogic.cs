using System.Threading.Tasks;
using TFU.MessagePlatform.Zalo;

namespace TFU.MessagePlatform.Implements
{
	public interface IZaloBizLogic
	{
		Task<string> ExecuteCallBackUrl();
		Task<ZaloTokenResponse> GenerateAccessToken(string code);
		Task<ZaloUserInfoModel> GetZaloUserInfo(string token);
		Task<bool> SendZalo(PostMessageModel model);
	}
}