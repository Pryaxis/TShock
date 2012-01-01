using System;
using System.ComponentModel;
namespace Hooks
{
	public class SetDefaultsEventArgs<T, F> : HandledEventArgs
	{
		public T Object
		{
			get;
			set;
		}
		public F Info
		{
			get;
			set;
		}
	}
}
