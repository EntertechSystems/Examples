using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net.Core;
using Microsoft.AspNet.SignalR;

namespace Log4SignalR
{
	public class Log4SignalRHub : Hub
	{
		public static List<Client> ClientMap = new List<Client>();

		public void SetLogLevel(string level)
		{

			var levelStrongTyped = LogLevelMap.GetLogLevel(level);		

			var client = ClientMap.FirstOrDefault(data => data.ConnectionId == this.Context.ConnectionId);

			if (client == null)
			{
				client = new Client { ConnectionId = this.Context.ConnectionId };
				ClientMap.Add(client);
			}

			client.Loglevel = levelStrongTyped;

			SignalRLogger.SetLogLevel();
		}

		public override Task OnDisconnected()
		{
			ClientMap.RemoveAll(data => data.ConnectionId == Context.ConnectionId);
			SignalRLogger.SetLogLevel();

			return base.OnDisconnected();
		}
	}

	public class Client
	{
		   public Level Loglevel {get;set;}

		   public string ConnectionId {get;set;}
	}

	public static class LogLevelMap
	{
		static LevelMap levelMap = new LevelMap();

		static LogLevelMap()
		{
			foreach (FieldInfo fieldInfo in typeof(Level).GetFields(BindingFlags.Public | BindingFlags.Static))
			{
				if (fieldInfo.FieldType == typeof(Level))
				{
					levelMap.Add((Level)fieldInfo.GetValue(null));
				}
			}
		}

		public static Level GetLogLevel(string logLevel)
		{
			if (string.IsNullOrWhiteSpace(logLevel))
			{
				return null;
			}
			else
			{
				return levelMap[logLevel];
			}
		}
	}
}
