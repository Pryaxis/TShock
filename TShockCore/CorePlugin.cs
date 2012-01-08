using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShock;

namespace TShockCore
{
	/// <summary>
	/// Base TShockPlugin except with the Core class added.
	/// </summary>
	public abstract class CorePlugin : Plugin
	{
		ICore Core {get;set;}
		public override void SetInterfaces(IEnumerable<object> interfaces)
		{
			Core = GetInterface<ICore>(interfaces);
			base.SetInterfaces(interfaces);
		}
	}
}
