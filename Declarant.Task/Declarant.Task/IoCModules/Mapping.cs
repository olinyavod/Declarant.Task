using AutoMapper;
using Declarant.Task.Models;
using Declarant.Task.ViewModels;

namespace Declarant.Task.IoCModules
{
	public class Mapping : Profile
	{
		public Mapping()
		{
			CreateMap<EventItem, EventItem>()
				.ReverseMap();

			CreateMap<EventItemEditorViewModel, EventItem>()
				.ReverseMap();
		}
	}
}