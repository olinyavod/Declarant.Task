using System;
using System.Threading.Tasks;

namespace EasyProg.Utils.Errors
{
	public interface IErrorSender : IDisposable
	{
		Task SendErrorAsync(Exception ex);

		void SendError(Exception ex);

		void SendReport(string title, string message);

		Task SendReportAsync(string title, string message);

		void Cancel();
	}
}