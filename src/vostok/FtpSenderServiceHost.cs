using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using TasksOnTime;
using TasksOnTime.Scheduling;


namespace Vostok
{
	/**
<?xml version="1.0" encoding="utf-8"?>
<configuration>
		<appSettings>
			<add key="RootFolder" value="****" /> <!- Optionnal (current folder by default) -->
			<add key="Pattern" value="*.bak" />
			<add key="Ftp" value="ftp://ftpaddress" />
			<add key="Login" value="****" />
			<add key="Password" value="***" />
			<add key="UseBinary" value="true" />
			<add key="UsePassive" value="true" />
			<add key="KeepAlive" value="true" />
			<add key="BufferSize" value="4096" />
		</appSettings>
</configuration> 
	**/
	public class FtpSenderServiceHost 
	{
		public void Initialize()
		{
			var brokenRules = GlobalConfiguration.Settings.Validate();
			if (!string.IsNullOrWhiteSpace(brokenRules))
			{
				System.Diagnostics.Trace.TraceError(brokenRules);
				throw new Exception(brokenRules);
			}

			var task = TasksOnTime.Scheduling.Scheduler.CreateScheduledTask<SendToFtpTask>("SendToFtpTask")
							.EveryHour()
							.AllowMultipleInstance(false);

			TasksOnTime.GlobalConfiguration.Settings.ScheduledTaskDisabledByDefault = false;
			TasksOnTime.Scheduling.Scheduler.Add(task);
		}

		public void Start()
		{
			TasksOnTime.Scheduling.Scheduler.Start();
			System.Diagnostics.Trace.WriteLine("Vostok Started");
		}

		public void Stop()
		{
			TasksOnTime.Scheduling.Scheduler.Stop();
			System.Diagnostics.Trace.WriteLine("Vostok Stopped");
		}
	}

}
