using App.Entity.Models.DingTalk;
using App.Utility;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace TFU.MessagePlatform.Implements
{
   public class DingBizLogic : IDingBizLogic
   {
      #region DingTalk
      public static string DingAgentId { get; set; }
      public static string DingAppKey { get; set; }
      public static string DingAppSecret { get; set; }
      public static string DingBaseAPI { get; set; }
      public static string CustomKeyWords { get; set; }
      #endregion

      public DingBizLogic()
      {
      }
      /// <summary>
      /// Send text bang Robot Webhook duoc config tay
      /// </summary>
      /// <param name="webhookToken"></param>
      /// <param name="model"></param>
      /// <returns></returns>
      public async Task<bool> SendDingChatHook(string webhookToken, DingChatSendModel model)
      {
         if (model != null)
         {
            model.text.content = $"[{CustomKeyWords} - {DateTime.Now:HH:mm dd/MM/yy}]\n{model.text.content}";
            var dingResponse = await CallDingTalk_API<string>(webhookToken, SystemConstants.DingRobotSendChat, model, Method.Post);
            if (dingResponse != null && dingResponse.Success)
               return true;
         }
         return false;
      }

      /// <summary>
      /// Call To Dingtalk API
      /// </summary>
      /// <typeparam name="T"></typeparam>
      /// <param name="api"></param>
      /// <param name="data"></param>
      /// <param name="method"></param>
      /// <returns></returns>
      public static async Task<DingTalkResponse<T>> CallDingTalk_API<T>(string token, string api, object data = null, Method method = Method.Get)
      {

         var client = new RestClient(DingBaseAPI);
         var request = new RestRequest(api);
         request.Method = method;

         //Add condition
         request.AddQueryParameter("access_token", token);
         if (data != null)
            request.AddJsonBody(data);

         //Execute
         var response = await client.ExecuteAsync(request);
         if (response != null)
         {
            var content = response.Content;
            var obj = JsonConvert.DeserializeObject<DingTalkResponse<T>>(content);
            return obj;
         }

         return null;
      }
   }
}
