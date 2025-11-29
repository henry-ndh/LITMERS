using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System.Threading.Tasks;
using TFU.MessagePlatform.Zalo;

namespace TFU.MessagePlatform.Implements
{
   public class ZaloBizLogic : IZaloBizLogic
   {
      private readonly IConfiguration _configuration;
      private readonly ILogger<ZaloBizLogic> _logger;
      public static string ZaloBaseAPI { get; set; }
      public ZaloBizLogic(IConfiguration configuration, ILogger<ZaloBizLogic> logger)
      {
         _configuration = configuration;
         _logger = logger;
      }

      #region Login Flow
      public async Task<string> ExecuteCallBackUrl()
      {
         string authorizeUrl = _configuration.GetValue<string>("Zalo:AuthorizeUrl");
         string appId = _configuration.GetValue<string>("Zalo:AppID");
         string callbackUrl = _configuration.GetValue<string>("Zalo:CallbackUrl");

         RestClient client = new RestClient(authorizeUrl);
         RestRequest request = new RestRequest("/permission");
         request.AddQueryParameter("app_id", appId);
         request.AddQueryParameter("redirect_uri", callbackUrl);
         var response = await client.ExecuteGetAsync(request);
         return response.ResponseUri.ToString();
      }

      public async Task<ZaloTokenResponse> GenerateAccessToken(string code)
      {
         string authorizeUrl = _configuration.GetValue<string>("Zalo:AuthorizeUrl");
         string appId = _configuration.GetValue<string>("Zalo:AppID");
         string appSecret = _configuration.GetValue<string>("Zalo:AppSecret");
         RestClient client = new RestClient(authorizeUrl);
         RestRequest request = new RestRequest("/access_token");
         request.AddQueryParameter("app_id", appId);
         request.AddQueryParameter("app_secret", appSecret);
         request.AddQueryParameter("code", code);

         var response = await client.ExecuteAsync(request);
         if (response.IsSuccessful)
            return JsonConvert.DeserializeObject<ZaloTokenResponse>(response.Content);
         return null;
      }

      public async Task<ZaloUserInfoModel> GetZaloUserInfo(string token)
      {
         var api = "/me";
         var data = new { fields = "id,birthday,name,gender,picture" };

         var response = await CallZalo_API<ZaloUserInfoModel>(token, api, data, Method.Get);
         return response;
      }
      #endregion

      #region Send Zalo as Backend
      public async Task<bool> SendZalo(PostMessageModel model)
      {
         var webhookToken = _configuration.GetValue<string>("Zalo:Token");
         var dingResponse = await CallZalo_API<ZaloResponse<MessageResponse>>(webhookToken, "/oa/message", model, Method.Post);
         if (dingResponse.Success)
            return true;
         _logger.LogError(dingResponse.message);
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
      private static async Task<T> CallZalo_API<T>(string token, string api, object data = null, Method method = Method.Get)
      {
         var client = new RestClient(ZaloBaseAPI);
         var request = new RestRequest(api, method);
         //Add condition
         request.AddQueryParameter("access_token", token);
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
