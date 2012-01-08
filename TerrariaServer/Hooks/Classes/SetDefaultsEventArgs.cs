using System.ComponentModel;

namespace TerrariaServer.Hooks.Classes
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
