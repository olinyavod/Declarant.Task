using System.ComponentModel;
using System.Threading.Tasks;
using Autofac;
using EasyProg.BusinessLogic.Interfaces;
using EasyProg.Models.Interfaces;

namespace EasyProg.WPF.MVVM.ViewModels
{
	public abstract class ModelEditorViewModelBase<TModel, TItem, TKey, TManager> : AbstractModelEditorViewModelBase<TModel>, INotifyDataErrorInfo
		where TModel : class, IIdentity<TKey>
		where TManager : class, ICrudManager<TModel, TItem, TKey>
		where TItem : class, IIdentity<TKey>
	{
		public TKey Id { get; set; }

		protected override void OnParameterChanged(object parameter)
		{
			base.OnParameterChanged(parameter);
			var p = Parameter as ModelEditorParameter<TKey>;
			if (p == null) return;
			Id = p.Id;
			IsNew = p.IsNew;

		}
		protected override Task<ExecuteStatus<TModel>> GetAsync(ILifetimeScope scope)
		{
			return scope.Resolve<TManager>().GetAsync(Id);
		}

		protected override Task<ModifyStatus<TModel>> AddAsync(ILifetimeScope scope, TModel model)
		{
			return scope.Resolve<TManager>().AddAsync(model);
		}

		protected override Task<ModifyStatus<TModel>> UpdateAsync(ILifetimeScope scope, TModel model)
		{
			return scope.Resolve<TManager>().UpdateAsync(model);
		}

	}

	public abstract class ModelEditorViewModelBase<TModel, TItem, TManager> : ModelEditorViewModelBase<TModel, TItem, int, TManager> 
		where TModel : class, IIdentity<int>
		where TManager : class, ICrudManager<TModel, TItem, int>
		where TItem : class, IIdentity<int>
	{
	}

	public abstract class ModelEditorViewModelBase<TModel, TKey> : ModelEditorViewModelBase<TModel, TModel, TKey, ICrudManager<TModel, TKey>>
		where TModel : class, IIdentity<TKey>
	{
		
	}
}