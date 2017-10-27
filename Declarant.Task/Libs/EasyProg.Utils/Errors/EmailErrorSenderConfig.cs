namespace EasyProg.Utils.Errors
{
	public class EmailErrorSenderConfig
	{
		public string SmtpServer { get; set; }

		public int SmtpPort { get; set; }

		public string Email { get; set; }

		public string EmailPassword { get; set; }

		public bool UseSsl { get; set; }

		public string AppName { get; set; }

		public string SenderEmail { get; set; }
	}
}