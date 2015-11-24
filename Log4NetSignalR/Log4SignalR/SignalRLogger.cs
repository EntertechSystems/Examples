using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.Core;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Owin;

namespace Log4SignalR
{
	public static class SignalRLogger
	{
		static IDisposable _webapp;
		public static void StartUp()
		{
			if (_webapp != null)
				return;

			_webapp = WebApplication.Start<Startup>(SignalRAppender.GetUrl());
			
			Console.WriteLine("Server running at {0}",SignalRAppender.GetUrl());

			
		}

		public static void Stop()
		{
			if (_webapp != null)
				_webapp.Dispose();
		}

		public class Startup
		{
			public void Configuration(IAppBuilder app)
			{
				var config = new HubConfiguration { EnableCrossDomain = true };
				
				app.MapHubs(config);
			}
		}

		internal static void Send(string logString, log4net.Core.Level level)
		{
			var hubContext = GlobalHost.ConnectionManager.GetHubContext<Log4SignalRHub>();

			var clients = Log4SignalR.Log4SignalRHub.ClientMap.ToList(); //Copy

			clients.ForEach(data =>
			{
				if (Level.Compare(level, data.Loglevel) >= 0)
				{
					hubContext.Clients.Client(data.ConnectionId).Log(new Entry { Level = level.Name, Message = logString });
				}
			});
		}

		public static void SetLogLevel()
		{
			var clients = Log4SignalR.Log4SignalRHub.ClientMap.ToList();
			var minimumLevel = clients.Min(data => data.Loglevel);
			
			SignalRAppender.SetMinimumRootLevel(minimumLevel);
		}
	}

	public class Entry
	{
		public string Message { get; set; }

		public string Level { get; set; }
	}
}
