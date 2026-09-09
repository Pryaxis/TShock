using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TShockAPI.GetDataHandlers;

namespace TShockAPI.Handlers
{
	/// <summary>
	/// Handles the TileEntityDisplayDollPoseSync packets and checks for permissions.
	/// </summary>
	public class DisplayDollPoseSyncHandler : IPacketHandler<DisplayDollPoseSyncEventArgs>
	{
		public void OnReceive(object sender, DisplayDollPoseSyncEventArgs args)
		{
			if (!args.Player.HasBuildPermission(args.DisplayDollEntity.Position.X, args.DisplayDollEntity.Position.Y, false))
			{
				args.Player.SendErrorMessage(GetString("You do not have permission to modify a Mannequin in a protected area!"));
				// Note - itemIndex is unused, so it remains 0 here.
				args.Player.SendData(PacketTypes.TileEntityDisplayDollItemSync, "", 255, args.TileEntityID, 0, (int)DisplayDollInventoryID.Pose);
				args.Handled = true;
				return;
			}
		}
	}
}
