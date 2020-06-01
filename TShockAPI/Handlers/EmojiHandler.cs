using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TShockAPI.Handlers
{
	/// <summary>
	/// Handles an exploit and checks for permissions.
	/// </summary>
	public class EmojiHandler
	{
		public void OnEmoji(object sender, GetDataHandlers.EmojiEventArgs args)
		{
			if (args.PlayerIndex != args.Player.Index)
			{
				TShock.Log.ConsoleError($"EmojiHandler: Packet is spoofing to be player ID {args.PlayerIndex}! - From [{args.Player.Index}]{args.Player.Name}");
				args.Handled = true;
				return;
			}

			if (!args.Player.HasPermission(Permissions.sendemoji))
			{
				args.Player.SendErrorMessage("You have no permission to send emotes!");
				args.Handled = true;
				return;
			}
		}
	}
}
