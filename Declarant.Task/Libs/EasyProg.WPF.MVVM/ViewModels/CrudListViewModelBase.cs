using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Autofac;
using DevExpress.Mvvm;
using EasyProg.BusinessLogic.Interfaces;
using EasyProg.Models.Interfaces;
using EasyProg.WPF.MVVM.Messages;
using EasyProg.WPF.MVVM.Services;

namespace EasyProg.WPF.MVVM.ViewModels
{
	public abstract  class CrudListViewModelBase<TModel, TManager, TItem, TKey> : ViewModelBase
		where TModel : class, IIdentity<TKey>
		where TItem : class
		where TManager : class, ICrudManager<TModel, TItem, TKey>
	{
		private readonly CrudListConfig _config;

		protected CrudListViewModelBase(CrudListConfig config)
		{
			_config = config;
			DisplayTitle = config.DefaultTitle;
			DeleteCommand = new AsyncCommand(OnDeleteAsync, OnCanDelete);
			RefreshCommand = new AsyncCommand(OnRefreshAsync);
			DetailsCommand = new AsyncCommand(OnDetails, OnCanDetails);
			AddCommand = new AsyncCommand(OnAdd, OnCanAdd);
			OnLoadedCommand = new AsyncCommand(OnLoadedAsync);
			ItemsSource = new ObservableCollection<TItem>();
		}

		public ICommand OnLoadedCommand { get; private set; }

		protected virtual async Task OnLoadedAsync()
		{
			ItemsSource = await GetItemsSource();

		}

		public ObservableCollection<TItem> ItemsSource
		{
			get { return GetProperty(() => ItemsSource); }
			set { SetProperty(() => ItemsSource, value); }
		}

		public RefreshDataMessage RefreshMessage
		{
			get { return GetProperty(() => RefreshMessage); }
			set { SetProperty(() => RefreshMessage, value); }
		}

		protected virtual async Task<ObservableCollection<TItem>> GetItemsSource()
		{
			var waitService = GetService<ISplashScreenService>();
			try
			{
				waitService?.ShowSplashScreen();
				using (var scope = this.BeginRequest())
				{
					var items = await Task.Run(() => GetQueryable(scope).ToList());
					return new ObservableCollection<TItem>(items);
				}
			}
			catch (Exception ex)
			{
				waitService?.HideSplashScreen();
				this.ShowError(DisplayTitle, ex);
				return new ObservableCollection<TItem>();
			}
			finally
			{
				waitService?.HideSplashScreen();
			}
		}

		protected virtual IQueryable<TItem> GetQueryable(ILifetimeScope scope)
		{
			return scope.Resolve<TManager>().Query;
		}

		public TItem SelectedItem
		{
			get { return GetProperty(() => SelectedItem); }
			set { SetProperty(() => SelectedItem, value); }
		}

		public string DisplayTitle
		{
			get { return GetProperty(() => DisplayTitle); }
			set { SetProperty(() => DisplayTitle, value); }
		}

		public ICommand DeleteCommand { get; private set; }

		protected virtual bool OnCanDelete()
		{
			return SelectedItem != null;
		}

		protected abstract string GetDeleteMessage(TItem item);

		protected abstract TKey GetId(TItem item);

		protected virtual async Task OnDeleteAsync()
		{
			var messageBox = GetService<IMessageBoxService>();
			var item = SelectedItem;
			if (messageBox.Show(GetDeleteMessage(item), DisplayTitle, MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
			{
				return;
			}
			var waitService = GetService<ISplashScreenService>();
			try
			{
				waitService.ShowSplashScreen();
				using (var scope = this.BeginRequest())
				{
					var result = await IoCExtensions.Resolve<TManager>(scope).DeleteAsync(GetId(item));
					if (result.HasError)
					{
						this.ShowError(DisplayTitle, result);
						return;
					}
				}
				ItemsSource.Remove(item);
			}
			catch (Exception e)
			{
				waitService.HideSplashScreen();
				this.ShowError(DisplayTitle, e);
			}
			finally
			{
				waitService.HideSplashScreen();
			}
		}

		public ICommand RefreshCommand { get; set; }

		protected virtual async Task OnRefreshAsync()
		{
			ItemsSource = await GetItemsSource();
		}

		public ICommand DetailsCommand { get; private set; }

		protected virtual bool OnCanDetails()
		{
			return SelectedItem != null;
		}

		protected virtual object CreateDetailsParameter(TItem item)
		{
			return new ModelEditorParameter<TKey> {Id = GetId(item)};
		}

		protected virtual Task OnDetails()
		{
			var windowService = GetService<IWindowService>();
			windowService.Title = _config.DetailsTitle;
			windowService
				.Show(_config.DetailsEditorViewName, CreateDetailsParameter(SelectedItem), this);
			return OnRefreshAsync();
		}

		public ICommand AddCommand { get; private set; }

		protected virtual bool OnCanAdd()
		{
			return true;
		}

		protected virtual Task OnAdd()
		{
			var windowService = GetService<IWindowService>();
			windowService.Title = _config.AddTitle;
			windowService
				.Show(_config.AddEditorViewName, CreateNewParameter(), this);
			return OnRefreshAsync();
		}

		private object CreateNewParameter()
		{
			return new ModelEditorParameter<TKey> {IsNew = true};
		}
	}

	public abstract class CrudListViewModelBase<TModel, TManager, TKey> : CrudListViewModelBase<TModel, TManager, TModel, TKey> 
		where TModel : class, IIdentity<TKey> 
		where TManager : class, ICrudManager<TModel, TKey> 
	{
		protected CrudListViewModelBase(CrudListConfig config) 
			: base(config)
		{
		}
	}

	public abstract  class CrudListViewModelBase<TModel, TKey> : CrudListViewModelBase<TModel, ICrudManager<TModel, TKey>, TKey>
		where TModel : class, IIdentity<TKey> 
	{
		protected CrudListViewModelBase(CrudListConfig config)
			: base(config)
		{
		}
	}
}
