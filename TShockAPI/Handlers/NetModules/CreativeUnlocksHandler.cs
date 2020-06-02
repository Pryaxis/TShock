using System.IO;
using System.IO.Streams;
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
			if (UnknownField != 0)
			{
				TShock.Log.ConsoleDebug(
					"CreativeUnlocksNetModuleHandler received non-vanilla unlock request. Random field value: {0} from {1}",
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
