using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TShockAPI
{
	public class HandlerList : HandlerList<EventArgs>
	{
	}
	public class HandlerList<T> where T : EventArgs
	{
		protected class HandlerObject
		{
			public EventHandler<T> Handler { get; set; }
			public HandlerPriority Priority { get; set; }
			public bool GetHandled { get; set; }
		}

		protected object HandlerLock = new object();
		protected List<HandlerObject> Handlers { get; set; }
		public HandlerList()
		{
			Handlers = new List<HandlerObject>();
		}

		/// <summary>
		/// Register a handler
		/// </summary>
		/// <param name="handler">Delegate to be called</param>
		/// <param name="priority">Priority of the delegate</param>
		/// <param name="gethandled">Should the handler receive a call even if it has been handled</param>
		public void Register(EventHandler<T> handler, HandlerPriority priority = HandlerPriority.Normal, bool gethandled = false)
		{
			lock (HandlerLock)
			{
				Handlers.Add(new HandlerObject { Handler = handler, Priority = priority, GetHandled = gethandled });
				Handlers = Handlers.OrderBy(h => (int)h.Priority).ToList();
			}
		}

		public void UnRegister(EventHandler<T> handler)
		{
			lock (HandlerLock)
			{
				Handlers.RemoveAll(h => h.Handler.Equals(handler));
			}
		}

		public void Invoke(object sender, T e)
		{
			List<HandlerObject> handlers;
			lock (HandlerLock)
			{
				//Copy the list for invoking as to not keep it locked during the invokes
				handlers = new List<HandlerObject>(Handlers);
			}

			var hargs = e as HandledEventArgs;
			for (int i = 0; i < handlers.Count; i++)
			{
				if (hargs == null || !hargs.Handled || (hargs.Handled && handlers[i].GetHandled))
				{
					handlers[i].Handler(sender, e);
				}
			}
		}
	}

	public enum HandlerPriority
	{
		Highest = 1,
		High = 2,
		Normal = 3,
		Low = 4,
		Lowest = 5,
	}
}
