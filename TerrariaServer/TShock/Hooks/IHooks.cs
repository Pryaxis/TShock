using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShock.Hooks;
using TShock.Hooks.Player;

namespace TShock.Hooks
{
	/// <summary>
	/// Hooks interface
	/// </summary>
	public interface IHooks
	{
		PlayerHooks PlayerHooks { get; }
	}
}
