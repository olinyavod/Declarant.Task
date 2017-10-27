using System;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm;
using EasyProg.Utils.Errors;
using EasyProg.WPF.MVVM.Services;

namespace EasyProg.WPF.MVVM.ViewModels
{
	public class ErrorViewModel : ViewModelBase
	{
		public string DisplayTitle
		{
			get { return GetProperty(() => DisplayTitle); }
			set { SetProperty(() => DisplayTitle, value); }
		}

		public string ErrorMessage
		{
			get { return GetProperty(() => ErrorMessage); }
			set { SetProperty(() => ErrorMessage, value); }
		}

		public string ErrorDetails
		{
			get { return GetProperty(() => ErrorDetails); }
			set { SetProperty(() => ErrorDetails, value); }
		}

		public string ApplicationName
		{
			get { return GetProperty(() => ApplicationName); }
			set { SetProperty(() => ApplicationName, value); }
		}

		protected override void OnParameterChanged(object parameter)
		{
			base.OnParameterChanged(parameter);
			var p = parameter as ErrorParameter;
			if (p != null)
			{
				ErrorDetails = p.ErrorDetails;
				ErrorMessage = p.ErrorMessage;
				ApplicationName = p.ApplicationName;
				DisplayTitle = p.Title;
			}
		}

		public bool IsBusy
		{
			get { return GetProperty(() => IsBusy); }
			set { SetProperty(() => IsBusy, value); }
		}

		public ICommand SendReportCommand => new AsyncCommand(OnSendReport, OnCanSendReport);

		bool OnCanSendReport()
		{
			return !IsBusy;
		}

		private async Task OnSendReport()
		{
			try
			{
				IsBusy = true;
				await this.Resolve<IErrorSender>()
					.SendReportAsync($"Error: {ApplicationName}", new StringBuilder()
						.AppendFormat("Application: {0}", ApplicationName)
						.AppendLine()
						.AppendFormat("Display Title: {0}", DisplayTitle)
						.AppendLine()
						.AppendFormat("Error message: {0}", ErrorMessage)
						.AppendLine()
						.AppendLine("Details:")
						.AppendLine(ErrorDetails)
						.ToString());
				IsBusy = false;
				GetService<ICurrentWindowService>()
					.Close();
			}
			catch (Exception ex)
			{
				IsBusy = false;
				GetService<IMessageBoxService>()
					.Show(ex.Message, DisplayTitle, MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				IsBusy = false;
			}
		}

		public ICommand CloseCommand => new DelegateCommand(OnClose, OnCanClose);

		private bool OnCanClose()
		{
			return !IsBusy;
		}

		private void OnClose()
		{
			GetService<ICurrentWindowService>()
				.Close();
		}

		public ICommand ClosingCommand => new DelegateCommand<CancelEventArgs>(OnClosing);

		private void OnClosing(CancelEventArgs e)
		{
			e.Cancel = IsBusy;
		}
	}
}