using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace EasyProg.Utils.SqlServer
{
	public class ProcessSqlLocalDbFinder : ILocalDbFinder
	{
		ProcessStartInfo CreateStartInfo()
		{
			return new ProcessStartInfo
			{
				Arguments = "i",
				FileName = "sqllocaldb",
				RedirectStandardOutput = true,
				UseShellExecute = false,
				CreateNoWindow = true

			};
		}

		public IEnumerable<string> FindInstances()
		{
			try
			{
				var process = Process.Start(CreateStartInfo());
				if (process == null) return Enumerable.Empty<string>();
				var list = new List<string>();
				while (!process.StandardOutput.EndOfStream)
				{
					list.Add($@"(localdb)\{process.StandardOutput.ReadLine()}");
				}
				return list;
			}
			catch (Exception)
			{
				return Enumerable.Empty<string>();
			}
		}

		public async Task<IEnumerable<string>> FindInstancesAsync()
		{
			try
			{
				var process = Process.Start(CreateStartInfo());
				if (process == null) return Enumerable.Empty<string>();
				var list = new List<string>();
				while (!process.StandardOutput.EndOfStream)
				{
					list.Add(await process.StandardOutput.ReadLineAsync());
				}
				return list;
			}
			catch (Exception)
			{
				return Enumerable.Empty<string>();
			}
		}
	}
}