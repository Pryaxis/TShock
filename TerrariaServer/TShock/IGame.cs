using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShock
{
	public interface IGame
	{
		/// <summary>
		/// List of players
		/// </summary>
		IList<IPlayer> Players { get; }
	}
}
