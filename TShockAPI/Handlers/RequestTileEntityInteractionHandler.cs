using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
					if (tileEntity is TEDisplayDoll)
					{
						if (!args.Player.HasBuildPermission(tileEntity.Position.X, tileEntity.Position.Y, false))
						{
							args.Player.SendErrorMessage("You have no permission to modify a Mannequin in a protected area!");
							args.Handled = true;
							return;
						}
					}
				}
			}
		}
	}
}
