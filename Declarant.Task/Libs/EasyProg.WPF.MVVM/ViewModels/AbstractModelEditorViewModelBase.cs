using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Autofac;
using DevExpress.Mvvm;
using EasyProg.BusinessLogic.Interfaces;
using EasyProg.WPF.MVVM.Services;

namespace EasyProg.WPF.MVVM.ViewModels
{
	public abstract class AbstractModelEditorViewModelBase<TModel> : ChangeableViewModelBase<TModel>
		where TModel : class
	{
		protected AbstractModelEditorViewModelBase()
		{
			CancelCommand = new DelegateCommand(OnCancel, OnCanCancel);
			SaveCommand = new AsyncCommand(OnSaveAsync, OnCanSave);
		}

		protected abstract Task<ModifyStatus<TModel>> AddAsync(ILifetimeScope scope, TModel model);

		protected abstract Task<ModifyStatus<TModel>> UpdateAsync(ILifetimeScope scope, TModel model);
		
		protected abstract Task<ExecuteStatus<TModel>> GetAsync(ILifetimeScope scope);


		public string DisplayTitle
		{
			get { return GetProperty(() => DisplayTitle); }
			set { SetProperty(() => DisplayTitle, value); }
		}

		public ICommand ClosingCommand => new AsyncCommand<CancelEventArgs>(OnClosing);

		protected abstract string GetClosingMessage();


		protected virtual async Task OnClosing(CancelEventArgs e)
		{
			if (IsChanged)
			{
				e.Cancel = true;
				switch (
					GetService<IMessageBoxService>()
						.Show(GetClosingMessage(), DisplayTitle, MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
				{
					case MessageBoxResult.Yes:
						if (await SaveChangesAsync())
						{
							e.Cancel = false;
							GetService<ICurrentWindowService>().Close();

						}
						break;
					case MessageBoxResult.No:
						e.Cancel = false;
						break;
				}
			}
		}

		public ICommand OnLoadedCommand => new AsyncCommand(OnLoaded);

		protected virtual async Task OnLoaded()
		{
			var waitService = GetService<ISplashScreenService>();
			try
			{
				waitService?.ShowSplashScreen();
				using (var scope = this.BeginRequest())
				{
					if (IsNew)
					{
						Model = CreateNewModel();
					}
					else
					{
						var getResult = await GetAsync(scope);
						if (getResult.HasError)
						{
							this.ShowError(DisplayTitle, getResult);
							return;
						}
						Model = getResult.Result;
					}
				}
			}
			catch (Exception ex)
			{
				waitService?.HideSplashScreen();
				this.ShowError(DisplayTitle, ex);
			}
			finally
			{
				waitService?.HideSplashScreen();
			}
		}

		protected virtual async Task<bool> SaveChangesAsync()
		{
			var waitService = GetService<ISplashScreenService>();
			try
			{
				waitService?.ShowSplashScreen();
				TModel model = CreateNewModel();
				using (var scope = this.BeginRequest())
				{
					CommitEdit(model);
					ModifyStatus<TModel> result;
					if (IsNew)
					{
						result = await AddAsync(scope, model);
					}
					else
					{
						result = await UpdateAsync(scope, model);
					}
					if (result.HasError)
					{
						waitService?.HideSplashScreen();
						this.ShowError(DisplayTitle, result);
						return false;
					}
					if (!result.IsValid)
					{
						ClearValidationErrors();
						foreach (var propety in result.FailPropeties)
						{
							HasErrors = true;
							AddValidationError(propety.PropertyName, propety.ErrorMessage);
							RaiseErrorsChanged(propety.PropertyName);
						}
						
						return false;
					}
					Model = result.Result;
				}
				
				IsNew = false;
				IsChanged = false;
				return true;
			}
			catch (Exception ex)
			{
				waitService?.HideSplashScreen();
				this.ShowError(DisplayTitle, ex);
				return false;
			}
			finally
			{
				waitService?.HideSplashScreen();
			}
		}

		public ICommand SaveCommand { get; private set; }

		protected virtual bool OnCanSave()
		{
			return IsChanged && !HasErrors;
		}

		protected virtual async Task OnSaveAsync()
		{
			if (await SaveChangesAsync())
			{
				GetService<ICurrentWindowService>().Close();
			}
		}

		public ICommand CancelCommand { get; private set; }

		protected virtual bool OnCanCancel()
		{
			return true;
		}

		protected virtual void OnCancel()
		{
			CancelEdit(Model);
			GetService<ICurrentWindowService>().Close();
		}
	}
}