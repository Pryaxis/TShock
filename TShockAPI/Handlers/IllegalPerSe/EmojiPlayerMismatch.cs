using static TShockAPI.GetDataHandlers;

namespace TShockAPI.Handlers.IllegalPerSe
{
	/// <summary>
	/// Rejects emoji packets with mismatched identifiers
	/// </summary>
	public class EmojiPlayerMismatch : IPacketHandler<EmojiEventArgs>
	{
		/// <summary>
		/// Invoked on emoji send. Rejects packets that are impossible.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void OnReceive(object sender, EmojiEventArgs args)
		{
			if (args.PlayerIndex != args.Player.Index)
			{
				TShock.Log.ConsoleError($"IllegalPerSe: Emoji packet rejected for ID spoofing. Expected {args.Player.Index}, received {args.PlayerIndex} from {args.Player.Name}.");
				args.Handled = true;
				return;
			}
		}
	}
}
