using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShock.Hooks;

namespace TShock.Hooks
{
	/// <summary>
	/// Hooks interface
	/// </summary>
	public interface IHooks
	{
		IPlayerHooks PlayerHooks { get; set; }
	}
}
