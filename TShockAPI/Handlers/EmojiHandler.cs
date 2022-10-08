﻿using static TShockAPI.GetDataHandlers;

namespace TShockAPI.Handlers
{
	/// <summary>
	/// Handles emoji packets and checks for permissions
	/// </summary>
	public class EmojiHandler : IPacketHandler<EmojiEventArgs>
	{
		/// <summary>
		/// Invoked when an emoji is sent in chat. Rejects the emoji packet if the player does not have emoji permissions
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void OnReceive(object sender, EmojiEventArgs args)
		{
			if (!args.Player.HasPermission(Permissions.sendemoji))
			{
				args.Player.SendErrorMessage("你没有权限发送Emoji!");
				args.Handled = true;
				return;
			}
		}
	}
}
