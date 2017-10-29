using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Autofac;
using Declarant.Task.Managers;
using Declarant.Task.Messeges;
using Declarant.Task.Models;
using DevExpress.Mvvm;
using EasyProg.WPF.MVVM.Services;

namespace Declarant.Task.Providers
{
	class ShedulerProvider : IShedulerProvider, IDisposable
	{
		private readonly IDictionary<int, Timer> _timers;
		private readonly IMessenger _messenger;

		public ShedulerProvider(IMessenger messenger)
		{
			_messenger = messenger;
			_timers = new Dictionary<int, Timer>();
		}

		public void Start()
		{
			using (var r = this.BeginRequest())
			{
				var now = DateTime.Now;
				foreach (var item in r.Resolve<EventItemsManager>().Query.Where(i => i.StartTime > now).ToArray())
				{
					SetEvent(item);
				}
			}
		}

		TimeSpan GetEventTime(EventItem item)
		{
			var r = item.StartTime - DateTime.Now;
			if (r < TimeSpan.FromMinutes(30))
				return r;
			return r - TimeSpan.FromMinutes(30);
		}

		public void SetEvent(EventItem item)
		{
			var time = GetEventTime(item);
			if (_timers.TryGetValue(item.Id, out var t))
			{
				t.Change(time, TimeSpan.FromMinutes(1));
			}
			else
			{
				_timers.Add(item.Id, new Timer(OnTime, item.Id, time, TimeSpan.FromMinutes(1)));
			}
		}

		private void OnTime(object state)
		{
			var eventId = (int) state;
			EventItem item;
			using (var r = this.BeginRequest())
			{
				item = this.Resolve<EventItemsManager>().Get(eventId).Result;
			}
			_messenger.Send(new TimeMessage(item));
		}

		public void RemoveEvent(int eventId)
		{
			if (_timers.TryGetValue(eventId, out var t))
			{
				t.Dispose();
				_timers.Remove(eventId);
			}
		}

		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (var timer in _timers.ToArray())
				{
					timer.Value.Dispose();
					_timers.Remove(timer.Key);
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}
	}
}