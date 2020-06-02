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
		/// Invoked when player damages a tile. Rejects the packet if its out of world bounds.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void OnReceive(object sender, SyncTilePickingEventArgs args)
		{
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
