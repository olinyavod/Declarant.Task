using System;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI;
using EasyProg.BusinessLogic.Interfaces;
using EasyProg.WPF.MVVM.Services;
using EasyProg.WPF.MVVM.Views;

namespace EasyProg.WPF.MVVM.ViewModels
{
	public static  class ViewModelExtensions
	{
		public static IMessageBoxService GetMessageBoxService(this object viewModel)
		{
			var services = viewModel as ISupportServices;
			if (services != null)
				services.ServiceContainer.GetService<IMessageBoxService>();
			return null;
		}

		public static TStatus ShowError<TStatus>(this object viewModel, string title, TStatus status)
			where TStatus : ExecuteStatus
		{
			if (status.HasError)
			{
				ShowErrorWindow(viewModel, new ErrorParameter
				{
					ApplicationName = AppDomain.CurrentDomain.FriendlyName,
					ErrorDetails = status.ErrorMessage,
					Title = title,
					ErrorMessage = status.ErrorMessage
				});
			}
			return status;
		}

		static void ShowErrorWindow(object parentViewModel, ErrorParameter parameter)
		{
			var errorWindow = parentViewModel.Resolve<Window>(CommonViewNames.ErrorWindow);
			ViewHelper.InitializeView(errorWindow, new ErrorViewModel(), parameter, parentViewModel);
			errorWindow.ShowDialog();
		}

		public static void ShowError(this object viewModel, string title, Exception ex)
		{
			ShowErrorWindow(viewModel, new ErrorParameter
			{
				Title = title,
				ApplicationName = AppDomain.CurrentDomain.FriendlyName,
				ErrorDetails = ex.ToString(),
				ErrorMessage = ex.Message
			});
		}
	}
}
