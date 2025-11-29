using System.Threading.Tasks;
using TFU.MessagePlatform.Model.Telegram;

namespace TFU.MessagePlatform.Interface
{
	public interface ITelegramBizLogic
	{
		Task<bool> SendTelegram(TelegramPostModel model, string webhookToken = null);
	}
}