using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShock
{
	public interface IPlayer
	{
		/// <summary>
		/// Meant to only be used by TServer.
		/// </summary>
		int Id { get; }

		/// <summary>
		/// The character name of the player.
		/// </summary>
		string Name { get; set; }

        /// <summary>
        /// The IP of the player.
        /// </summary>
        string IP { get; set; }


		/// <summary>
		/// Attempts to damage the player.
		/// </summary>
		/// <param name="amt">int - how much damage to give</param>
		void Damage(int amount);

		/// <summary>
		/// Sends message to specified player
		/// </summary>
		/// <param name="msg">string - text to send</param>
		/// <param name="color">Color - color for the text</param>
		void SendMessage(string msg, Color color = default(Color));
	}
}
