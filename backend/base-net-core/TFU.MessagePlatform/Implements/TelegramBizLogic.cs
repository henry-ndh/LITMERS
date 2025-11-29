using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System.Threading.Tasks;
using TFU.MessagePlatform.Interface;
using TFU.MessagePlatform.Model.Telegram;

namespace TFU.MessagePlatform.Implements
{
   public class TelegramBizLogic : ITelegramBizLogic
   {
      private readonly IConfiguration _configuration;
      private readonly ILogger<TelegramBizLogic> _logger;
      public TelegramBizLogic(IConfiguration configuration, ILogger<TelegramBizLogic> logger)
      {
         _configuration = configuration;
         _logger = logger;
      }

      #region Send Telegram as Backend
      public async Task<bool> SendTelegram(TelegramPostModel model, string webhookToken = null)
      {
         var token = string.IsNullOrEmpty(webhookToken) ? _configuration.GetValue<string>("Telegram:Token") : webhookToken;
         var dingResponse = await CallTelegram_API<TelegramResponse>(token, "/sendMessage", model, Method.Post);
         if (dingResponse.Success)
            return true;
         _logger.LogError("SendTelegram: {0}", JsonConvert.SerializeObject(dingResponse.result));
         return false;
      }
      #endregion

      #region Base
      /// <summary>
      /// Call To Zalo
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="api"></param>
      /// <param name="data"></param>
      /// <param name="method"></param>
      private async Task<T> CallTelegram_API<T>(string token, string api, object data = null, Method method = Method.Get)
      {
         var TelegramBaseAPI = _configuration.GetValue<string>("Telegram:BaseAPI");
         var client = new RestClient($"{TelegramBaseAPI}/bot{token}");
         var request = new RestRequest(api, method);
         if (data != null)
         {
            switch (method)
            {
               case Method.Get:
                  foreach (var element in data.GetType().GetProperties())
                     request.AddQueryParameter(element.Name, element.GetValue(data, null).ToString());
                  break;
               case Method.Post:
                  request.AddJsonBody(data);
                  break;
               default:
                  break;
            }
         }
         //Execute
         var response = await client.ExecuteAsync(request);
         if (response != null)
         {
            var content = response.Content;
            var obj = JsonConvert.DeserializeObject<T>(content);
            return obj;
         }
         return default;
      }
      #endregion
   }
}
