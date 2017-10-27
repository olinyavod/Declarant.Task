using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using AutoMapper;
using DevExpress.Mvvm;
using EasyProg.WPF.MVVM.Services;

namespace EasyProg.WPF.MVVM.ViewModels
{
	public abstract class ChangeableViewModelBase<TModel> : ViewModelBase, INotifyDataErrorInfo 
		where TModel : class
	{
		private IDictionary<string, ICollection<string>> _validationErrors;

		protected ChangeableViewModelBase()
		{
			_validationErrors = new Dictionary<string, ICollection<string>>();
			PropertyChanged += OnPropertyChanged;
		}

		public virtual event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		public TModel Model
		{
			get { return GetProperty(() => Model); }
			set { SetProperty(() => Model, value, old => OnModelChanged(old, value)); }
		}

		public bool HasErrors
		{
			get { return GetProperty(() => HasErrors); }
			set { SetProperty(() => HasErrors, value, old => OnHasErrorsChanged(old, value)); }
		}

		public bool IsChanged
		{
			get { return GetProperty(() => IsChanged); }
			set { SetProperty(() => IsChanged, value, old => OnIsChangedOnChanged(old, value)); }
		}

		public bool IsNew
		{
			get { return GetProperty(() => IsNew); }
			set { SetProperty(() => IsNew, value, oldValue => OnIsNewChanged(oldValue, value)); }
		}

		protected void RaiseErrorsChanged(string propertyName)
		{
			if (ErrorsChanged != null)
				ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
		}

		protected virtual TModel CreateNewModel()
		{
			return Activator.CreateInstance<TModel>();
		}

		protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_validationErrors.ContainsKey(e.PropertyName))
			{
				_validationErrors.Remove(e.PropertyName);
				HasErrors = _validationErrors.Count > 0;
				RaiseErrorsChanged(e.PropertyName);
			}
			var property = Model?.GetType().GetProperty(e.PropertyName);
			if (property != null)
				IsChanged = true;
		}

		public IEnumerable GetErrors(string propertyName)
		{
			return _validationErrors.ContainsKey(propertyName)
				? _validationErrors[propertyName]
				: new string[0];
		}

		protected virtual void OnModelChanged(TModel oldValue, TModel newValue)
		{
			CancelEdit(newValue);
			IsChanged = false;
		}

		protected virtual void CancelEdit(TModel model)
		{
			this.Resolve<IMapper>()
				.Map(model, this);
			
		}

		protected virtual void OnHasErrorsChanged(bool oldValue, bool newValue)
		{

		}

		protected virtual void OnIsChangedOnChanged(bool oldValue, bool newValue)
		{

		}

		protected virtual void OnIsNewChanged(bool oldValue, bool newValue)
		{

		}

		protected virtual void CommitEdit(TModel model)
		{
			this.Resolve<IMapper>().Map(this, model);
		}

		protected void ClearValidationErrors()
		{
			_validationErrors.Clear();
		}

		protected void AddValidationError(string propertyName, string errors)
		{
			if (_validationErrors.ContainsKey(propertyName))
				_validationErrors[propertyName].Add(errors);
			else
				_validationErrors.Add(propertyName, new List<string> {errors});
		}

		protected void RaiseAllErrorsChanged()
		{
			foreach (var p in _validationErrors)
				RaiseErrorsChanged(p.Key);
		}
	}
}