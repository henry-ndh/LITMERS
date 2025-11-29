using Microsoft.Extensions.Logging;
using System;

namespace TFU.Logger
{
	public class TFULogger : ILogger
	{
		private readonly string _name;
		private readonly bool _logAll;
		private TFULoggerContext _dbContext;

		public TFULogger(string name, string sqliteConnectionString, bool logAll)
		{
			_name = name;
			_dbContext = new TFULoggerContext(sqliteConnectionString);
			_dbContext.Database.EnsureCreated();
			_logAll = logAll;
		}

		public IDisposable BeginScope<TState>(TState state) => null;

		public bool IsEnabled(LogLevel logLevel) => throw new NotImplementedException();

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			if (_logAll || _name.Contains("Controllers"))
			{
				var loggerModel = new TFULoggerModel
				{
					LogLevel = logLevel,
					EventId = eventId.Id,
					CategoryName = _name,
					Msg = formatter(state, exception),
					Time = DateTime.Now
				};

				_dbContext.Logger.AddAsync(loggerModel);
				_dbContext.SaveChangesAsync();
			}
		}
	}
}
