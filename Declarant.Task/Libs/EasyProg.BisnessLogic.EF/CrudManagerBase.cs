using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EasyProg.BusinessLogic.Interfaces;
using EasyProg.Models.Interfaces;
using FluentValidation;

namespace EasyProg.BusinessLogic.EF
{
	public abstract class CrudManagerBase<TModel, TItem, TKey> : ICrudManager<TModel, TItem, TKey>
		where TModel : class, IIdentity<TKey> 
		where TItem : class
	{
		private readonly IValidator<TModel> _validator;
		private readonly IMapper _mapper;
		private readonly DbContext _context;

		protected CrudManagerBase(
			IValidator<TModel> validator, 
			IMapper mapper,
			DbContext context)
		{
			_validator = validator;
			_mapper = mapper;
			_context = context;
		}

		protected abstract IQueryable<TItem> GetQuery();
		
		public virtual ModifyStatus<TModel> Add(TModel model)
		{
			try
			{
				if (model == null)
					return new ModifyStatus<TModel> {HasError = true, ErrorMessage = "Model is null"};
				var validationResult = _validator.Validate(model).ToValidStatus<TModel>();
				if (!validationResult.IsValid)
					return validationResult;
				AddCore(model);
				_context.SaveChanges();
				return new ModifyStatus<TModel>
				{
					Result = model
				};
			}
			catch (Exception ex)
			{
				return ex.ToStatus<ModifyStatus<TModel>>();
			}
		}

		protected virtual void AddCore(TModel model)
		{
			_context.Set<TModel>().Add(model);
		}

		public virtual async Task<ModifyStatus<TModel>> AddAsync(TModel model)
		{
			try
			{
				if (model == null)
					return new ModifyStatus<TModel> { HasError = true, ErrorMessage = "Model is null" };

				var validationResult = (await _validator.ValidateAsync(model)).ToValidStatus<TModel>();
				if (!validationResult.IsValid)
					return validationResult;
				AddCore(model);
				await _context.SaveChangesAsync();
				return new ModifyStatus<TModel>
				{
					Result = model
				};
			}
			catch (Exception ex)
			{
				return ex.ToStatus<ModifyStatus<TModel>>();
			}
		}

		protected virtual void UpdateCore(TModel oldModel, TModel model)
		{
			_mapper.Map(model, oldModel);
		}

		public virtual ModifyStatus<TModel> Update(TModel model)
		{
			try
			{
				if (model == null)
					return new ModifyStatus<TModel> { HasError = true, ErrorMessage = "Model is null" };

				var validationResult = _validator.Validate(model).ToValidStatus<TModel>();
				if (!validationResult.IsValid)
				{
					return validationResult;
				}
				var oldModel = _context.Set<TModel>().Find(model.Id);
				if(oldModel == null) return new ModifyStatus<TModel>
				{
					HasError = true
				};
				UpdateCore(oldModel, model);
				_context.SaveChanges();
				return new ModifyStatus<TModel>
				{
					Result = model
				};
			}
			catch (Exception e)
			{
				return e.ToStatus<ModifyStatus<TModel>>();
			}
		}

		public virtual async Task<ModifyStatus<TModel>> UpdateAsync(TModel model)
		{
			try
			{
				if (model == null)
					return new ModifyStatus<TModel> { HasError = true, ErrorMessage = "Model is null" };

				var validationResult = (await _validator.ValidateAsync(model)).ToValidStatus<TModel>();
				if (!validationResult.IsValid)
				{
					return validationResult;
				}
				var oldModel = await _context.Set<TModel>().FindAsync(model.Id);
				if (oldModel == null) return new ModifyStatus<TModel>
				{
					HasError = true
				};
				UpdateCore(oldModel, model);
				await _context.SaveChangesAsync();
				return new ModifyStatus<TModel>
				{
					Result = model
				};
			}
			catch (Exception ex)
			{
				return ex.ToStatus<ModifyStatus<TModel>>();
			}
		}

		public virtual ExecuteStatus Delete(TKey id)
		{
			try
			{
				var item = _context.Set<TModel>().Find(id);
				_context.Set<TModel>().Remove(item);
				_context.SaveChanges();
				return new ExecuteStatus();
			}
			catch (Exception e)
			{
				return e.ToStatus<ExecuteStatus>();
			}
		}

		public virtual async Task<ExecuteStatus> DeleteAsync(TKey id)
		{
			try
			{
				var item = await _context.Set<TModel>().FindAsync(id);
				_context.Set<TModel>().Remove(item);
				await _context.SaveChangesAsync();
				return new ExecuteStatus();
			}
			catch (Exception e)
			{
				return e.ToStatus<ExecuteStatus>();
			}
		}

		protected virtual TModel GetCore(TKey id)
		{
			return _context.Set<TModel>().Find(id);
		}

		public ExecuteStatus<TModel> Get(TKey id)
		{
			try
			{
				var item = GetCore(id);
				return new ExecuteStatus<TModel>
				{
					Result = item
				};
			}
			catch (Exception e)
			{
				return e.ToStatus<ExecuteStatus<TModel>>();
			}
		}

		public async Task<ExecuteStatus<TModel>> GetAsync(TKey id)
		{
			try
			{
				var item = await Task.Run(() => GetCore(id));
				return new ExecuteStatus<TModel>
				{
					Result = item
				};
			}
			catch (Exception e)
			{
				return e.ToStatus<ExecuteStatus<TModel>>();
			}
		}

		public IQueryable<TItem> Query => GetQuery();
		
	}

	public class CrudManagerBase<TModel, TKey> : CrudManagerBase<TModel, TModel, TKey>, ICrudManager<TModel, TKey>
		where TModel : class, IIdentity<TKey>
	{
		private readonly DbContext _context;

		public CrudManagerBase(IValidator<TModel> validator, IMapper mapper, DbContext context)
			: base(validator, mapper, context)
		{
			_context = context;
		}

		protected override IQueryable<TModel> GetQuery()
		{
			return _context.Set<TModel>();
		}
	}
}