using System.Windows;
using System.Windows.Input;
using Declarant.Task.Messeges;
using Declarant.Task.Models;
using Declarant.Task.Providers;
using Declarant.Task.Views;
using DevExpress.Mvvm;
using EasyProg.WPF.MVVM.Services;
using EasyProg.WPF.MVVM.ViewModels;

namespace Declarant.Task.ViewModels
{
	public class MainViewModel : CrudListViewModelBase<EventItem, int>
	{
		public MainViewModel() : base(new CrudListConfig(Properties.Resources.ttlEventItems, typeof(EventItemEditorView).Name))
		{
			ExitCommand = new DelegateCommand(OnExit);
			this.Resolve<IMessenger>()
				.Register<TimeMessage>(this, OnTime);
		}

		private void OnTime(TimeMessage message)
		{
			GetService<IDispatcherService>()
				.BeginInvoke(() =>
				{
					GetService<IMessageBoxService>()
						.Show(message.EventItem.Description, DisplayTitle, MessageBoxButton.OK, MessageBoxImage.Information);
				});
		}

		protected override string GetDeleteMessage(EventItem item)
		{
			return string.Format(Properties.Resources.msgDeleteEventItem, item.Name);
		}

		protected override int GetId(EventItem item)
		{
			return item.Id;
		}

		public ICommand ExitCommand { get; }

		void OnExit()
		{
			GetService<ICurrentWindowService>().Close();
		}

		protected override async System.Threading.Tasks.Task OnDeleteAsync()
		{
			var item = SelectedItem;
			await base.OnDeleteAsync();
			this.Resolve<IShedulerProvider>().RemoveEvent(item.Id);
		}
	}
}
