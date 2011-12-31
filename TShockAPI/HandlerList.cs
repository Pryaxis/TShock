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
		public class HandlerItem
		{
			public EventHandler<T> Handler { get; set; }
			public HandlerPriority Priority { get; set; }
			public bool GetHandled { get; set; }
		}

		protected object HandlerLock = new object();
		protected List<HandlerItem> Handlers { get; set; }
		public HandlerList()
		{
			Handlers = new List<HandlerItem>();
		}

		/// <summary>
		/// Register a handler
		/// </summary>
		/// <param name="handler">Delegate to be called</param>
		/// <param name="priority">Priority of the delegate</param>
		/// <param name="gethandled">Should the handler receive a call even if it has been handled</param>
		public void Register(EventHandler<T> handler, HandlerPriority priority = HandlerPriority.Normal, bool gethandled = false)
		{
			Register(Create(handler, priority, gethandled));
		}

		public void Register(HandlerItem obj)
		{
			lock (HandlerLock)
			{
				Handlers.Add(obj);
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
			List<HandlerItem> handlers;
			lock (HandlerLock)
			{
				//Copy the list for invoking as to not keep it locked during the invokes
				handlers = new List<HandlerItem>(Handlers);
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

		public static HandlerItem Create(EventHandler<T> handler, HandlerPriority priority = HandlerPriority.Normal, bool gethandled = false)
		{
			return new HandlerItem { Handler = handler, Priority = priority, GetHandled = gethandled };
		}
		public static HandlerList<T> operator +(HandlerList<T> hand, HandlerItem obj)
		{
			if (hand == null)
				hand = new HandlerList<T>();

			hand.Register(obj);
			return hand;
		}
		public static HandlerList<T> operator +(HandlerList<T> hand, EventHandler<T> handler)
		{
			if (hand == null)
				hand = new HandlerList<T>();

			hand.Register(Create(handler));
			return hand;
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
