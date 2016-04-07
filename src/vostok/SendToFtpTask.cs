using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TasksOnTime;

namespace Vostok
{
	public class SendToFtpTask : TasksOnTime.ITask
	{
		public void Execute(ExecutionContext context)
		{
			if (context.IsCancelRequested)
			{
				return;
			}

			if (System.DateTime.Now.Hour < GlobalConfiguration.Settings.StartHour)
			{
				return;
			}

			System.Diagnostics.Trace.WriteLine("SendToFtpTask started");

			var lastBackupFile = System.IO.Path.Combine(GlobalConfiguration.Settings.RootFolder, "lastbackup.txt");
			var lastBackupfileInfo = new System.IO.FileInfo(lastBackupFile);
			if (lastBackupfileInfo.Exists
				&& lastBackupfileInfo.LastWriteTime > DateTime.Today)
			{
				System.Diagnostics.Trace.WriteLine("Backup already executed");
				return;
			}
			
			var fileList = from file in System.IO.Directory.GetFiles(GlobalConfiguration.Settings.RootFolder, GlobalConfiguration.Settings.Pattern, System.IO.SearchOption.AllDirectories)
						   let fileInfo = new System.IO.FileInfo(file)
						   where fileInfo.LastWriteTime > DateTime.Today
						   orderby fileInfo.LastWriteTime descending
						   select file;

			if (fileList.Count()  == 0)
			{
				System.Diagnostics.Trace.WriteLine($"No file to send in folder {GlobalConfiguration.Settings.RootFolder} with pattern {GlobalConfiguration.Settings.Pattern}");
				return;
			}

			var hasException = false;
			foreach (var lastFile in fileList)
			{
				if (context.IsCancelRequested)
				{
					break;
				}

				try
				{
					System.Diagnostics.Trace.WriteLine($"Try to upload {lastFile} to ftp");
					UploadFileToFtp(lastFile, context);
					System.IO.File.AppendAllText(lastBackupFile, System.IO.Path.GetFileName(lastFile) + System.Environment.NewLine);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Trace.WriteLine($"Fail to upload {lastFile}");
					System.Diagnostics.Trace.TraceError(ex.ToString());
					hasException = true;
				}
			}

			if (context.IsCancelRequested)
			{
				return;
			}

			if (!hasException)
			{
				SynchronizeExistingFileList(context);
			}
		}

		private void UploadFileToFtp(string fileName, ExecutionContext context)
		{
			var localFileName = System.IO.Path.GetFileName(fileName);
			var uri = $"{GlobalConfiguration.Settings.Ftp}{localFileName}";
			var ftp = (FtpWebRequest)WebRequest.Create(uri);
			ftp.Credentials = new System.Net.NetworkCredential(GlobalConfiguration.Settings.Login, GlobalConfiguration.Settings.Password);
			ftp.KeepAlive = GlobalConfiguration.Settings.KeepAlive;
			ftp.UseBinary = GlobalConfiguration.Settings.UseBinary;
			ftp.UsePassive = GlobalConfiguration.Settings.UsePassive;
			ftp.Method = System.Net.WebRequestMethods.Ftp.UploadFile;

			int bufferSize = GlobalConfiguration.Settings.BufferSize;
			var pos = 0;
			using (var stream = System.IO.File.OpenRead(fileName))
			{
				long contentLenght = stream.Length;

				if (System.Environment.UserInteractive)
				{
					Console.WriteLine($"Upload in progress for : {fileName}");
					Console.WriteLine($"Size : {contentLenght / 1000000.0:f2} Mb");
				}

				var buffer = new byte[bufferSize];
				var output = ftp.GetRequestStream();
				long outputSize = 0;
				long oldProgress = -1;
				var watch = new System.Diagnostics.Stopwatch();
				watch.Start();
				while ((pos = stream.Read(buffer, 0, bufferSize)) > 0)
				{
					if (context.IsCancelRequested)
					{
						break;
					}

					output.Write(buffer, 0, pos);
					outputSize += buffer.Length;
					if (System.Environment.UserInteractive)
					{
						var percent = outputSize / (contentLenght * 1.0);
						long progress = Convert.ToInt64(Math.Round(percent * 10000, 0));
						if (oldProgress != progress)
						{
							var speedBySecond = (outputSize / (watch.ElapsedMilliseconds / 1000.0)) / 1000;
							Console.SetCursorPosition(0, Console.CursorTop);
							Console.Write($"Upload in progress : {outputSize / 1000000.0:f2}Mb ({percent:p}) - {speedBySecond:F0}Ko/s       ");
						}
						oldProgress = progress;
					}
				}
				watch.Stop();
				output.Close();
				System.Diagnostics.Trace.WriteLine($"Uploaded with success in {watch.ElapsedMilliseconds / 1000}s with speed average {(outputSize / (watch.ElapsedMilliseconds / 1000.0)) / 1000:f2} Ko/s");
			}
		}

		private void SynchronizeExistingFileList(ExecutionContext context)
		{
			var localFileList = from file in System.IO.Directory.GetFiles(GlobalConfiguration.Settings.RootFolder, GlobalConfiguration.Settings.Pattern, System.IO.SearchOption.AllDirectories)
								let fileName = System.IO.Path.GetFileName(file)
								select fileName;

			var ftpFileList = GetFtpFileList();

			var fileToDeleteList = ftpFileList.Except(localFileList);

			System.IO.File.WriteAllLines(System.IO.Path.Combine(GlobalConfiguration.Settings.RootFolder, "delete.txt"), fileToDeleteList);

			foreach (var fileName in fileToDeleteList)
			{
				if (context.IsCancelRequested)
				{
					break;
				}

				try
				{
					DeleteFtpFile(fileName);
				}
				catch (Exception ex)
				{
					System.Diagnostics.Trace.WriteLine($"Fail to delete {fileName}");
					System.Diagnostics.Trace.TraceError(ex.ToString());
				}
			}
		}

		private List<string> GetFtpFileList()
		{
			var uri = $"{GlobalConfiguration.Settings.Ftp}";
			var ftp = (FtpWebRequest)WebRequest.Create(uri);
			ftp.Credentials = new System.Net.NetworkCredential(GlobalConfiguration.Settings.Login, GlobalConfiguration.Settings.Password);
			ftp.KeepAlive = GlobalConfiguration.Settings.KeepAlive;
			ftp.UseBinary = GlobalConfiguration.Settings.UseBinary;
			ftp.UsePassive = GlobalConfiguration.Settings.UsePassive;
			ftp.Method = System.Net.WebRequestMethods.Ftp.ListDirectory;

			var result = new List<string>();
			string content = null;
			using (var response = ftp.GetResponse())
			{
				using (var responseStream = response.GetResponseStream())
				{
					using (var reader = new System.IO.StreamReader(responseStream))
					{
						content = reader.ReadToEnd();
						reader.Close();
					}
					responseStream.Close();
				}
				response.Close();
			}

			if (content != null)
			{
				var lines = content.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
				foreach (var line in lines)
				{
					var endWith = GlobalConfiguration.Settings.Pattern.Replace("*", "");
					if (line.EndsWith(endWith))
					{
						result.Add(line);
					}
				}
			}

			return result;
		}

		private void DeleteFtpFile(string fileName)
		{
			var localFileName = System.IO.Path.GetFileName(fileName);
			var uri = $"{GlobalConfiguration.Settings.Ftp}{localFileName}";
			var ftp = (FtpWebRequest)WebRequest.Create(uri);
			ftp.Credentials = new System.Net.NetworkCredential(GlobalConfiguration.Settings.Login, GlobalConfiguration.Settings.Password);
			ftp.KeepAlive = GlobalConfiguration.Settings.KeepAlive;
			ftp.UseBinary = GlobalConfiguration.Settings.UseBinary;
			ftp.UsePassive = GlobalConfiguration.Settings.UsePassive;
			ftp.Method = System.Net.WebRequestMethods.Ftp.DeleteFile;

			var response = ftp.GetResponse();
			response.Close();
		}

	}
}
