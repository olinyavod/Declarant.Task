using System;
using System.Threading.Tasks;
using Declarant.Task.Models;
using Declarant.Task.Properties;
using Declarant.Task.Providers;
using EasyProg.WPF.MVVM.Services;
using EasyProg.WPF.MVVM.ViewModels;

namespace Declarant.Task.ViewModels
{
	public class EventItemEditorViewModel : ModelEditorViewModelBase<EventItem, int>
	{
		protected override string GetClosingMessage()
		{
			return Properties.Resources.msgClosingChangesEditor;
		}

		public string Name
		{
			get { return GetProperty(() => Name); }
			set { SetProperty(() => Name, value); }
		}

		public DateTime StartTime
		{
			get { return GetProperty(() => StartTime); }
			set { SetProperty(() => StartTime, value); }
		}

		public DateTime? EndTime
		{
			get { return GetProperty(() => EndTime); }
			set { SetProperty(() => EndTime, value); }
		}

		public string Description
		{
			get { return GetProperty(() => Description); }
			set { SetProperty(() => Description, value); }
		}

		public string SaveTitle
		{
			get { return GetProperty(() => SaveTitle); }
			set { SetProperty(() => SaveTitle, value); }
		}

		protected override void OnModelChanged(EventItem oldValue, EventItem newValue)
		{
			base.OnModelChanged(oldValue, newValue);
			DisplayTitle = IsNew
				? Resources.tttAddNewEvent
				: string.Format(Resources.ttlEditEvent, newValue.Name);
			SaveTitle = IsNew ? Resources.cmdAdd : Resources.cmdSave;
		}

		protected override async Task<bool> SaveChangesAsync()
		{
			var r = await base.SaveChangesAsync();
			if (r)
			{
				this.Resolve<IShedulerProvider>().SetEvent(Model);
			}
			return r;
		}

		protected override async System.Threading.Tasks.Task OnLoaded()
		{
			await base.OnLoaded();
			if (IsNew)
				StartTime = DateTime.Now;

		}
	}
}
