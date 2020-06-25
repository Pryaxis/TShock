using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TShockAPI.GetDataHandlers;

namespace TShockAPI.Handlers
{
	/// <summary>
	/// Handles the TileEntityDisplayDollItemSync packets and checks for permissions.
	/// </summary>
	public class DisplayDollItemSyncHandler : IPacketHandler<DisplayDollItemSyncEventArgs>
	{
		public void OnReceive(object sender, DisplayDollItemSyncEventArgs args)
		{
			/// If the player has no building permissions means that they couldn't even see the content of the doll in the first place.
			/// Thus, they would not be able to modify its content. This means that a hacker attempted to send this packet directly, or through raw bytes to tamper with the DisplayDoll. This is why I do not bother with making sure the player gets their item back.
			if (!args.Player.HasBuildPermission(args.DisplayDollEntity.Position.X, args.DisplayDollEntity.Position.Y, false))
			{
				args.Player.SendErrorMessage("You do not have permission to modify a Mannequin in a protected area!");
				args.Handled = true;
				return;
			}
		}
	}
}
