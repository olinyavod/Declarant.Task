using Declarant.Task.Models;

namespace Declarant.Task.Providers
{
	public interface IShedulerProvider
	{
		void Start();

		void SetEvent(EventItem item);

		void RemoveEvent(int eventId);
	}
}
