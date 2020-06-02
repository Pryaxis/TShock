using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TShockAPI.Handlers
{
	/// <summary>
	/// Handles emoji packets and checks for validity and permissions
	/// </summary>
	public class EmojiHandler
	{
		public void OnReceiveEmoji(object sender, GetDataHandlers.EmojiEventArgs args)
		{
			if (args.PlayerIndex != args.Player.Index)
			{
				TShock.Log.ConsoleError($"EmojiHandler: Emoji packet rejected for ID spoofing. Expected {args.Player.Index}, received {args.PlayerIndex} from {args.Player.Name}.");
				args.Handled = true;
				return;
			}

			if (!args.Player.HasPermission(Permissions.sendemoji))
			{
				args.Player.SendErrorMessage("You do not have permission to send emotes!");
				args.Handled = true;
				return;
			}
		}
	}
}
