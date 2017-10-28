using System.Data.Entity;
using AutoMapper;
using Declarant.Task.Models;
using EasyProg.BusinessLogic.EF;
using FluentValidation;

namespace Declarant.Task.Managers
{
	public class EventItemsManager: CrudManagerBase<EventItem, int>
	{
		public EventItemsManager(IValidator<EventItem> validator, IMapper mapper, DbContext context) : base(validator, mapper, context)
		{
		}
	}
}
