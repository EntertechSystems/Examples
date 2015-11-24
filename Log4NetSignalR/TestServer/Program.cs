using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace TestServer
{
	class Program
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		static Timer _timer;

		static void Main(string[] args)
		{
			
			
			_timer = new Timer(_timer_Elapsed,null,1000,1000);

			Console.WriteLine("Server läuft..");
			Console.ReadLine();
		}

		static void _timer_Elapsed(object sender)
		{
			log.Debug("Debug mopped");
			log.Info("Info Mopped");
			log.Error("Error Mopped");
			log.Fatal("Fatal mopped");
		}
	}
}
