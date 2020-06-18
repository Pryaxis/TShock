using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using static TShockAPI.GetDataHandlers;

namespace TShockAPI.Handlers
{
	/// <summary>
	/// 
	/// </summary>
	public class RequestTileEntityInteractionHandler : IPacketHandler<RequestTileEntityInteractionEventArgs>
	{
		public void OnReceive(object sender, RequestTileEntityInteractionEventArgs args)
		{
			if (args.TileEntityID != -1)
			{
				TileEntity tileEntity;
				if (TileEntity.ByID.TryGetValue(args.TileEntityID, out tileEntity))
				{
					if (tileEntity is TEHatRack && !args.Player.HasBuildPermission(tileEntity.Position.X, tileEntity.Position.Y, false))
					{
						args.Player.SendErrorMessage("You do not have permission to modify a Hat Rack in a protected area!");
						args.Handled = true;
						return;
					}
					else if (tileEntity is TEDisplayDoll && !args.Player.HasBuildPermission(tileEntity.Position.X, tileEntity.Position.Y, false))
					{
						args.Player.SendErrorMessage("You do not have permission to modify a Mannequin in a protected area!");
						args.Handled = true;
						return;
					}
					else if (!args.Player.HasBuildPermission(tileEntity.Position.X, tileEntity.Position.Y, false))
					{
						args.Player.SendErrorMessage("You do not have permission to modify a TileEntity in a protected area!");
						TShock.Log.ConsoleDebug($"RequestTileEntityInteractionHandler: Rejected packet due to lack of building permissions! - From {args.Player.Name} | Position X:{tileEntity.Position.X} Y:{tileEntity.Position.Y}, TileEntity type: {tileEntity.type}, Tile type: {Main.tile[tileEntity.Position.X, tileEntity.Position.Y].type}");
						args.Handled = true;
						return;
					}
				}
			}
		}
	}
}
