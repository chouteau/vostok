using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static System.Configuration.ConfigurationManager;

namespace Vostok
{
	public class GlobalConfiguration
	{
		private static Lazy<Settings> m_LazySettings = new Lazy<Settings>(() =>
		{
			var result = new Settings();
			var defaultFolder = System.IO.Path.GetDirectoryName(typeof(GlobalConfiguration).Assembly.Location);
			result.RootFolder = AppSettings["RootFolder"] ?? defaultFolder;
			if (string.IsNullOrWhiteSpace(result.RootFolder))
			{
				result.RootFolder = defaultFolder;
			}
			result.Pattern = AppSettings["Pattern"];
			result.Login = AppSettings["Login"];
			result.Password = AppSettings["Password"];
			result.Ftp = AppSettings["Ftp"];
			if (!result.Ftp.EndsWith("/"))
			{
				result.Ftp += "/";
			}
			result.KeepAlive = Convert.ToBoolean(AppSettings["KeepAlive"] ?? "false");
			result.UseBinary = Convert.ToBoolean(AppSettings["UseBinary"] ?? "false");
			result.UsePassive = Convert.ToBoolean(AppSettings["UsePassive"] ?? "false");
			string bufferSizeSetting = AppSettings["BufferSize"] ?? result.BufferSize.ToString();
			int bufferSize = 4096;
			if (int.TryParse(bufferSizeSetting, out bufferSize))
			{
				result.BufferSize = bufferSize;
			}
			string startHourSetting = AppSettings["StartHour"];
			int startHour = 5;
			if (int.TryParse(startHourSetting, out startHour))
			{
				result.StartHour = startHour;
			}
			return result;
		});

		public static Settings Settings
		{
			get
			{
				return m_LazySettings.Value;
			}
		}
	}
}
