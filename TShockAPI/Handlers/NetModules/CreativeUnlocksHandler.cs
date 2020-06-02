using System.IO;
using System.IO.Streams;
using Terraria;
using Terraria.GameContent.NetModules;
using Terraria.Net;

namespace TShockAPI.Handlers.NetModules
{
	/// <summary>
	/// Handles creative unlock requests
	/// </summary>
	public class CreativeUnlocksHandler : INetModuleHandler
	{
		/// <summary>
		/// An unknown field. If this does not have a value of '0' the packet should be rejected.
		/// </summary>
		public byte UnknownField { get; set; }
		/// <summary>
		/// ID of the item being sacrificed
		/// </summary>
		public ushort ItemId { get; set; }
		/// <summary>
		/// Stack size of the item being sacrificed
		/// </summary>
		public ushort Amount { get; set; }

		/// <summary>
		/// Reads the unlock data from the stream
		/// </summary>
		/// <param name="data"></param>
		public void Deserialize(MemoryStream data)
		{
			// For whatever reason Terraria writes '0' to the stream at the beginning of this packet.
			// If this value is not 0 then its been crafted by a non-vanilla client.
			// We don't actually know why the 0 is written, so we're just going to call this UnknownField for now
			UnknownField = data.ReadInt8();
			if (UnknownField == 0)
			{
				ItemId = data.ReadUInt16();
				Amount = data.ReadUInt16();
			}
		}

		/// <summary>
		/// Determines if the unlock is valid and the player has permission to perform the unlock.
		/// Syncs unlock status if the packet is accepted
		/// </summary>
		/// <param name="player"></param>
		/// <param name="rejectPacket"></param>
		public void HandlePacket(TSPlayer player, out bool rejectPacket)
		{
			if (!Main.GameModeInfo.IsJourneyMode)
			{
				TShock.Log.ConsoleDebug(
					"NetModuleHandler received attempt to unlock sacrifice while not in journey mode from",
					player.Name
				);

				rejectPacket = true;
				return;
			}

			if (UnknownField != 0)
			{
				TShock.Log.ConsoleDebug(
					"CreativeUnlocksHandler received non-vanilla unlock request. Random field value: {0} but should be 0 from {1}",
					UnknownField,
					player.Name
				);

				rejectPacket = true;
				return;
			}

			if (!player.HasPermission(Permissions.journey_contributeresearch))
			{
				player.SendErrorMessage("You do not have permission to contribute research.");
				rejectPacket = true;
				return;
			}

			var totalSacrificed = TShock.ResearchDatastore.SacrificeItem(ItemId, Amount, player);

			var response = NetCreativeUnlocksModule.SerializeItemSacrifice(ItemId, totalSacrificed);
			NetManager.Instance.Broadcast(response);

			rejectPacket = false;
		}
	}
}
