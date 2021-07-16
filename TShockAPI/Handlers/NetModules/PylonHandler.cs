using System.IO;
using System.IO.Streams;
using Terraria.GameContent;
using static Terraria.GameContent.NetModules.NetTeleportPylonModule;

namespace TShockAPI.Handlers.NetModules
{
	/// <summary>
	/// Handles a pylon net module
	/// </summary>
	public class PylonHandler : INetModuleHandler
	{
		/// <summary>
		/// Event occurring
		/// </summary>
		public SubPacketType PylonEventType { get; set; }
		/// <summary>
		/// Tile X coordinate of the pylon
		/// </summary>
		public short TileX { get; set; }
		/// <summary>
		/// Tile Y coordinate of the pylon
		/// </summary>
		public short TileY { get; set; }
		/// <summary>
		/// Type of Pylon
		/// </summary>
		public TeleportPylonType PylonType { get; set; }

		/// <summary>
		/// Reads the pylon data from the net module
		/// </summary>
		/// <param name="data"></param>
		public void Deserialize(MemoryStream data)
		{
			PylonEventType = (SubPacketType)data.ReadInt8();
			TileX = data.ReadInt16();
			TileY = data.ReadInt16();
			PylonType = (TeleportPylonType)data.ReadInt8();
		}

		/// <summary>
		/// Rejects a pylon teleport request if the player does not have permission
		/// </summary>
		/// <param name="player"></param>
		/// <param name="rejectPacket"></param>
		public void HandlePacket(TSPlayer player, out bool rejectPacket)
		{
			if (PylonEventType == SubPacketType.PlayerRequestsTeleport)
			{
				if (!player.HasPermission(Permissions.pylon))
				{
					rejectPacket = true;
					player.SendErrorMessage("You do not have permission to teleport using pylons.");
					return;
				}
			}

			rejectPacket = false;
		}
	}
}
