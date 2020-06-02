using static TShockAPI.GetDataHandlers;

namespace TShockAPI.Handlers
{
	/// <summary>
	/// Handles emoji packets and checks for validity and permissions
	/// </summary>
	public class EmojiHandler : IPacketHandler<EmojiEventArgs>
	{
		/// <summary>
		/// Invoked when an emoji is sent in chat. Rejects the emoji packet if the player is spoofing IDs or does not have emoji permissions
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void OnReceive(object sender, EmojiEventArgs args)
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
