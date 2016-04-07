using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vostok
{
	public class GlobalConfiguration
	{
		private static Lazy<Settings> m_LazySettings = new Lazy<Settings>(() =>
		{
			var result = new Settings();
			var defaultFolder = System.IO.Path.GetDirectoryName(typeof(GlobalConfiguration).Assembly.Location);
			result.RootFolder = System.Configuration.ConfigurationManager.AppSettings["RootFolder"] ?? defaultFolder;
			if (string.IsNullOrWhiteSpace(result.RootFolder))
			{
				result.RootFolder = defaultFolder;
			}
			result.Pattern = System.Configuration.ConfigurationManager.AppSettings["Pattern"];
			result.Login = System.Configuration.ConfigurationManager.AppSettings["Login"];
			result.Password = System.Configuration.ConfigurationManager.AppSettings["Password"];
			result.Ftp = System.Configuration.ConfigurationManager.AppSettings["Ftp"];
			if (!result.Ftp.EndsWith("/"))
			{
				result.Ftp += "/";
			}
			result.KeepAlive = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["KeepAlive"] ?? "false");
			result.UseBinary = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UseBinary"] ?? "false");
			result.UsePassive = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["UsePassive"] ?? "false");
			string bufferSizeSetting = System.Configuration.ConfigurationManager.AppSettings["BufferSize"] ?? result.BufferSize.ToString();
			int bufferSize = 0;
			if (int.TryParse(bufferSizeSetting, out bufferSize))
			{
				result.BufferSize = bufferSize;
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
