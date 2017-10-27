using System.Linq;
using System.Threading.Tasks;
using EasyProg.Models.Interfaces;

namespace EasyProg.BusinessLogic.Interfaces
{
	public interface ICrudManager<TModel, TItem, TKey>
		where TModel : class, IIdentity<TKey>
		where TItem : class
    {
	    ModifyStatus<TModel> Add(TModel model);

		Task<ModifyStatus<TModel>> AddAsync(TModel model);

	    ModifyStatus<TModel> Update(TModel model);

		Task<ModifyStatus<TModel>> UpdateAsync(TModel model);

	    ExecuteStatus Delete(TKey id);

	    Task<ExecuteStatus> DeleteAsync(TKey id);

	    ExecuteStatus<TModel> Get(TKey id);

	    Task<ExecuteStatus<TModel>> GetAsync(TKey id);

		IQueryable<TItem> Query { get; }
    }

	public interface ICrudManager<TModel, TKey> : ICrudManager<TModel, TModel, TKey> 
		where TModel : class, IIdentity<TKey>
	{
		
	}
}
