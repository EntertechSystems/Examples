using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Filter;
using Microsoft.AspNet.SignalR;

namespace Log4SignalR
{
	public class SignalRAppender : AppenderSkeleton
	{
		private Level standardRootLoggerLevel;

		private Level minimumLevel;

		private string _url;

		public string Url
		{
			get { return _url; }
			set { _url = value; }
		}

		private static SignalRAppender _instance;

		private static log4net.Repository.Hierarchy.Hierarchy _hierarchy = ((log4net.Repository.Hierarchy.Hierarchy)LogManager.GetLoggerRepository());

		public SignalRAppender()
		{
			if(_instance != null)
				throw new Exception("Only one instance of SignalRAppender allowed!");

			_instance = this;
			standardRootLoggerLevel = _hierarchy.Root.Level;

			Console.WriteLine("Root Logger Level: {0}", standardRootLoggerLevel);			
				
		}

		public override void ActivateOptions()
		{
			//ClearFilters();
			//Threshold = null;
			var filter = this.FilterHead;
			IFilter previosFilter = null;
			IFilter firstFilter = null;

			while (filter != null)
			{
				if (!(filter is LevelRangeFilter))
				{
					if (firstFilter == null)
						firstFilter = filter;

					if (previosFilter != null)
						previosFilter.Next = filter;

					previosFilter = filter;										
				}
				filter = filter.Next;
			}
			this.ClearFilters();

			if(firstFilter != null)
				this.AddFilter(firstFilter);

			SignalRLogger.StartUp();

			base.ActivateOptions();
		} 

		public static void SetMinimumRootLevel(Level level)
		{
			_instance.SetMinimum(level);
		}		

		private void SetMinimum(Level level)
		{
			minimumLevel = level;

			if (level == null)
				level = standardRootLoggerLevel;

			var currentLevel = _hierarchy.Root.Level;

			_hierarchy.Root.Level = standardRootLoggerLevel < level ? standardRootLoggerLevel : level;

			if (currentLevel != _hierarchy.Root.Level)
			{
				_hierarchy.RaiseConfigurationChanged(new EventArgs());

				Console.WriteLine("LogLevel set to: {0}", _hierarchy.Root.Level);
			}
		}

		protected override void Append(log4net.Core.LoggingEvent loggingEvent)
		{
			if (minimumLevel == null || minimumLevel > loggingEvent.Level)
				return;

			var logString = RenderLoggingEvent(loggingEvent);

			SignalRLogger.Send(logString, loggingEvent.Level);
		}

		internal static string GetUrl()
		{
			return _instance.Url;
		}
	}
}
