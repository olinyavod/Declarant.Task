using System.Windows;
using System.Windows.Input;
using Declarant.Task.Models;
using Declarant.Task.Views;
using DevExpress.Mvvm;
using EasyProg.WPF.MVVM.ViewModels;

namespace Declarant.Task.ViewModels
{
	public class MainViewModel : CrudListViewModelBase<EventItem, int>
	{
		public MainViewModel() : base(new CrudListConfig(Properties.Resources.ttlEventItems, typeof(EventItemEditorView).Name))
		{
			ExitCommand = new DelegateCommand(OnExit);
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

		protected override System.Threading.Tasks.Task OnDeleteAsync()
		{
			return base.OnDeleteAsync();
		}
	}
}
