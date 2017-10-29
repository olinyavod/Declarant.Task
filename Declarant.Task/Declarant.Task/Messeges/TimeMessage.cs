using Declarant.Task.Models;

namespace Declarant.Task.Messeges
{
	public class TimeMessage
	{
		public EventItem EventItem { get; }

		public TimeMessage(EventItem eventItem)
		{
			EventItem = eventItem;
		}
	}
}