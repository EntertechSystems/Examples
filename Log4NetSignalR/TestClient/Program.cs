using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace TestClient
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Write("Enter Log Level: ");
			var logLevel = Console.ReadLine();

			
			var hubConnection = new HubConnection("http://localhost:8080/");
			IHubProxy log4NetHub = hubConnection.CreateHubProxy("Log4SignalRHub");
			log4NetHub.On<Entry>("Log", entry => Console.WriteLine("Level:{0},  Message:{1}",entry.Level, entry.Message));
			hubConnection.Start().Wait();
			log4NetHub.Invoke("SetLogLevel", logLevel).Wait();
			

			Console.WriteLine("Client läuft");
			Console.ReadLine();
		}
	}

	public class Entry
	{
		public string Message { get; set; }

		public string Level { get; set; }
	}
}
