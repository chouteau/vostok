using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vostok
{
	public class Settings
	{
		public Settings()
		{
			Pattern = "*.bak";
			BufferSize = 4096;
			StartHour = 5;
		}

		public string RootFolder { get; set; }
		public string Pattern { get; set; }
		public string Ftp { get; set; }
		public string Login { get; set; }
		public string Password { get; set; }
		public bool UseBinary { get; set; }
		public bool UsePassive { get; set; }
		public bool KeepAlive { get; set; }
		public int BufferSize { get; set; }

		public int StartHour { get; set; }

		public override string ToString()
		{
			var result = new StringBuilder();
			result.AppendLine($"RootFolder : {RootFolder}");
			result.AppendLine($"Pattern : {Pattern}");
			result.AppendLine($"Ftp : {Ftp}");
			result.AppendLine($"Loging : {Login}");
			result.AppendLine($"Password : {Password}");
			result.AppendLine($"UseBinary : {UseBinary}");
			result.AppendLine($"UsePassive : {UsePassive}");
			result.AppendLine($"KeepAlive : {KeepAlive}");
			result.AppendLine($"BufferSize : {BufferSize}");
			return result.ToString();
		}

		public string Validate()
		{
			var result = new StringBuilder();
			if (string.IsNullOrWhiteSpace(RootFolder))
			{
				result.AppendLine("RootFolder parameter must be configured");
			}
			else if (!System.IO.Directory.Exists(RootFolder))
			{
				result.AppendLine(string.Format("RootFolder parameter {0} not exists", RootFolder));
			}
			if (string.IsNullOrWhiteSpace(Pattern))
			{
				result.AppendLine("Pattern parameter must be *.ext");
			}
			if (string.IsNullOrWhiteSpace(Ftp))
			{
				result.AppendLine("Ftp parameter must be configured");
			}
			else if (!Ftp.StartsWith("ftp://"))
			{
				result.AppendLine("Ftp parameter must start with ftp://");
			}
			if (string.IsNullOrWhiteSpace(Login))
			{
				result.AppendLine("Login parameter must be configured");
			}
			if (string.IsNullOrWhiteSpace(Password))
			{
				result.AppendLine("Password parameter must be configured");
			}
			if (BufferSize <= 0)
			{
				result.AppendLine("BufferSize paramter must be greater than zero");
			}
			return result.ToString();
		}
	}
}
