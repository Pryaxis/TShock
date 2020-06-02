using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using static TShockAPI.GetDataHandlers;

namespace TShockAPI.Handlers
{
	class SyncTilePickingHandler : IPacketHandler<SyncTilePickingEventArgs>
	{
		/// <summary>
		/// Invoked when player damages a tile.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void OnReceive(object sender, SyncTilePickingEventArgs args)
		{
			if (args.PlayerIndex != args.Player.Index)
			{
				TShock.Log.ConsoleDebug($"SyncTilePickingHandler: SyncTilePicking packet rejected for ID spoofing. Expected {args.Player.Index}, received {args.PlayerIndex} from {args.Player.Name}.");
				args.Handled = true;
				return;
			}

			if (args.TileX > Main.maxTilesX || args.TileX < 0
			   || args.TileY > Main.maxTilesY || args.TileY < 0)
			{
				TShock.Log.ConsoleDebug($"SyncTilePickingHandler: X and Y position is out of world bounds! - From {args.Player.Name}");
				args.Handled = true;
				return;
			}
		}
	}
}
