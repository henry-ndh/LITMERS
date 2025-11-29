using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace TFU.Helper
{
	public class RedisHelper
	{
		public const string HOST = "118.69.83.25";
		public const string HOST_DEV = "127.0.0.1";
		public const int PORT = 6379;
		public const string PASSWORD = "l0tus&fif0";
		public const string HOT_PRODUCT_DAILY = "HOT_PRODUCT_DAILY";
		public const string HOT_PRODUCT_WEEKLY = "HOT_PRODUCT_WEEKLY";
		public const string HOT_PRODUCT_MONTHLY = "HOT_PRODUCT_MONTHLY";
		public const string HOT_PRODUCT_YEARLY = "HOT_PRODUCT_YEARLY";
		public const string LETS_ENHANCE_CREDENTIALS = "LETS_ENHANCE_CREDENTIALS";
		public const string WAITING_TRANSFORM = "WAITING_TRANSFORM";
		private ConnectionMultiplexer _redis;
		private IDatabase _redisDb;
		private string _scope;

		public ILogger<RedisHelper> _logger { get; }

		public RedisHelper(IHostingEnvironment env, ILogger<RedisHelper> logger)
		{
			bool isDevelopment;
			if (env.IsDevelopment())
				isDevelopment = true;
			else
				isDevelopment = false;


			string scope = "Production";
			string host = isDevelopment ? HOST_DEV : HOST;
			var options = ConfigurationOptions.Parse($"{host}:{PORT}"); // host1:port1, host2:port2, ...
			if (!isDevelopment)
				options.Password = PASSWORD;
			_redis = ConnectionMultiplexer.Connect(options);
			_redisDb = _redis.GetDatabase();
			_scope = scope;
			_logger = logger;

			Console.WriteLine($"RedisHelper: {isDevelopment}");
			_logger.LogInformation($"RedisHelper: {isDevelopment}");
		}

		/// <summary>
		/// add or update value of the key
		/// </summary>
		/// <param name="key">The Key</param>
		/// <param name="value">The Json value</param>
		/// <returns></returns>
		public async Task<bool> AddOrUpdateAsync(string key, string value, TimeSpan timeExpiried)
		{
			return await _redisDb.StringSetAsync($"{_scope}/{key}", value, timeExpiried);
		}

		/// <summary>
		/// add or update value of the key
		/// </summary>
		/// <param name="key">The Key</param>
		/// <param name="value">The Json value</param>
		/// <returns></returns>
		public bool AddOrUpdate(string key, string value, TimeSpan timeExpiried)
		{
			return _redisDb.StringSet($"{_scope}/{key}", value, timeExpiried);
		}

		/// <summary>
		/// get value of the key
		/// </summary>
		public async Task<string> GetAsync(string key)
		{
			return await _redisDb.StringGetAsync($"{_scope}/{key}");
		}

		/// <summary>
		/// get value of the key
		/// </summary>
		public string Get(string key)
		{
			return _redisDb.StringGet($"{_scope}/{key}");
		}

		/// <summary>
		/// Remove key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public async Task<bool> RemoveAsync(string key)
		{
			return await _redisDb.KeyDeleteAsync($"{_scope}/{key}");
		}

		/// <summary>
		/// key existed or not.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public async Task<bool> ExistedAsync(string key)
		{
			return await _redisDb.KeyExistsAsync(key);
		}

		/// <summary>
		/// key existed or not.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool Existed(string key)
		{
			return _redisDb.KeyExists(key);
		}

		public void Publisher(string channel, string message)
		{
			_redis.GetSubscriber().Publish(channel, message);
		}

		public void Subscriber(string channel, Action<RedisChannel, RedisValue> action)
		{
			_redis.GetSubscriber().Subscribe(channel, action);
		}
	}
}
