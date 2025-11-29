using App.Entity.Models.DingTalk;
using System.Threading.Tasks;

namespace TFU.MessagePlatform.Implements
{
	public interface IDingBizLogic
	{
		Task<bool> SendDingChatHook(string webhookToken, DingChatSendModel model);
	}
}