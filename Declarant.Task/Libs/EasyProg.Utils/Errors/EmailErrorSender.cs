using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EasyProg.Utils.Errors
{
	public class EmailErrorSender : IErrorSender
	{
		private readonly EmailErrorSenderConfig _config;
		private readonly SmtpClient _client;
		public EmailErrorSender(EmailErrorSenderConfig config)
		{
			_config = config;
			_client = new SmtpClient(_config.SmtpServer)
				{
					EnableSsl = _config.UseSsl,
					DeliveryFormat = SmtpDeliveryFormat.International,
					UseDefaultCredentials = false,
					Credentials = new NetworkCredential(_config.SenderEmail, _config.EmailPassword)
				};
		}

		public async Task SendErrorAsync(Exception ex)
		{
			await _client.SendMailAsync(_config.Email, _config.Email, CreateTitle(), CreateBody(ex));
		}

		public void SendError(Exception ex)
		{
			_client.Send(_config.Email, _config.Email, CreateTitle(), CreateBody(ex));
		}

		public void SendReport(string title, string message)
		{
			_client.Send(_config.Email, _config.Email, title, message);
		}

		public Task SendReportAsync(string title, string message)
		{
			return _client.SendMailAsync(_config.Email, _config.Email, title, message);
		}

		public void Cancel()
		{
			_client.SendAsyncCancel();
		}

		string CreateTitle()
		{
			return string.Format("Ошибка в {0}", _config.AppName);
		}

		string CreateBody(Exception ex)
		{
			return string.Format(@"Произошла небоработанная ошибка в прложении {0} в {1}.

Сообщение ошибки: {2}.
ПОдробное описание ошибки:
{3}.", _config.AppName, DateTime.Now, ex.Message, ex);
		}

		public void Dispose()
		{
			_client.Dispose();
		}
	}
}
