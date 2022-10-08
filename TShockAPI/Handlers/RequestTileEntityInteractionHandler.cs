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
			if (args.TileEntity is TEHatRack && !args.Player.HasBuildPermissionForTileObject(args.TileEntity.Position.X, args.TileEntity.Position.Y, TEHatRack.entityTileWidth, TEHatRack.entityTileHeight, false))
			{
				args.Player.SendErrorMessage("你没有权限修改保护区域内的衣帽架!");
				args.Handled = true;
				return;
			}
			else if (args.TileEntity is TEDisplayDoll && !args.Player.HasBuildPermissionForTileObject(args.TileEntity.Position.X, args.TileEntity.Position.Y, TEDisplayDoll.entityTileWidth, TEDisplayDoll.entityTileHeight, false))
			{
				args.Player.SendErrorMessage("你没有权限修改保护区域内的人体模型!");
				args.Handled = true;
				return;
			}
			else if (!args.Player.HasBuildPermission(args.TileEntity.Position.X, args.TileEntity.Position.Y, false))
			{
				args.Player.SendErrorMessage("你没有权限修改保护区域内的图格!");
				TShock.Log.ConsoleDebug($"RequestTileEntityInteractionHandler: Rejected packet due to lack of building permissions! - From {args.Player.Name} | Position X:{args.TileEntity.Position.X} Y:{args.TileEntity.Position.Y}, TileEntity type: {args.TileEntity.type}, Tile type: {Main.tile[args.TileEntity.Position.X, args.TileEntity.Position.Y].type}");
				args.Handled = true;
				return;
			}
		}
	}
}
