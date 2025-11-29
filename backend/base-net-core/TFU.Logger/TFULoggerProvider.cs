using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace TFU.Logger
{
	public class TFULoggerProvider : ILoggerProvider
	{
		private string _connectString;
		private bool _logAll;
		private TFULogger logger;
		private readonly ConcurrentDictionary<string, TFULogger> _loggers = new ConcurrentDictionary<string, TFULogger>();
		public TFULoggerProvider(string sqliteConnectionString, bool LogAll)
		{
			_connectString = sqliteConnectionString;
			_logAll = LogAll;
		}

		public ILogger CreateLogger(string categoryName)
		{
			return _loggers.GetOrAdd(categoryName, name => new TFULogger(name, _connectString, _logAll));
		}

		public void Dispose()
		{
			_loggers.Clear();
		}
	}
}
