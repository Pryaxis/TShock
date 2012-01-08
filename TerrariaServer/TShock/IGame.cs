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

		/// <summary>
		/// Sends message to specified player
		/// </summary>
		/// <param name="ply"></param>
		/// <param name="msg"></param>
		void SendMessage(IPlayer ply, string msg, Color color = default(Color));

		/// <summary>
		/// Sends message to specified player
		/// </summary>
		void SendMessage(IPlayer ply, string msg);
	}
}
