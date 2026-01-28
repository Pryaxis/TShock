using System;
using System.IO;
using System.IO.Streams;
using Terraria;
using Terraria.GameContent.NetModules;
using Terraria.ID;
using Terraria.Net;

namespace TShockAPI.Handlers.NetModules
{
	/// <summary>
	/// Handles creative unlock requests
	/// </summary>
	public class CreativeUnlocksHandler : INetModuleHandler
	{
		/// <summary>
		/// This field used to always be 0 in 1.4.4.9. Starting 1.4.5, this field now contains the ID of the player researching/sacrificing the item.
		/// </summary>
		[Obsolete($"Use {nameof(PlayerId)} instead. This field used to always be 0 in 1.4.4.9. Starting 1.4.5, this field now contains the ID of the player researching/sacrificing the item.")]
		public byte UnknownField
		{
			get => PlayerId;
			set => PlayerId = value;
		}
		/// <summary>
		/// ID of the player researching/sacrificing the item.
		/// </summary>
		public byte PlayerId { get; set; }
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
			PlayerId = data.ReadInt8();
			ItemId = data.ReadUInt16();
			Amount = data.ReadUInt16();
		}

		/// <summary>
		/// Determines if the unlock is valid and the player has permission to perform the unlock.
		/// Syncs unlock status if the packet is accepted
		/// </summary>
		/// <param name="player"></param>
		/// <param name="rejectPacket"></param>
		public void HandlePacket(TSPlayer player, out bool rejectPacket)
		{
			if (Main.GameMode != GameModeID.Creative)
			{
				TShock.Log.ConsoleDebug(
					GetString($"NetModuleHandler received attempt to unlock sacrifice while not in journey mode from {player.Name}")
				);

				rejectPacket = true;
				return;
			}

			if (!player.HasPermission(Permissions.journey_contributeresearch))
			{
				player.SendErrorMessage(GetString("You do not have permission to contribute research."));
				rejectPacket = true;
				return;
			}

#if TRUE
			// NOTE: this is a temporary solution to get TShock to build
			/* Given that the NetCreativeUnlocksModule has been removed in 1.4.5.0,
			 * TShock can no longer directly set the research progress of items. Therefore,
			 * the following codepath does not function at all.
			 */
			/* NetCreativeUnlocksPlayerReportModule can be used in place of NetCreativeUnlocksModule,
			 * however, it is plagued with two issues.
			 * 1. The client will only accept the change if it is in a team (not white), and
			 *    the player with PlayerId is in the same team as them
			 * 2. The vanilla handling of NetCreativeUnlocksPlayerReportModule does not broadcast the
			 *    packet back to the sender, and the sender does not update their research progress
			 *    if SSC is on unless it receives a response from the server. Even if no. 1 were not true,
			 *    this bug causes researching items to break when SSC is on.
			 */
			rejectPacket = true;
			return;
#else
			var totalSacrificed = TShock.ResearchDatastore.SacrificeItem(ItemId, Amount, player);

			var response = NetCreativeUnlocksModule.SerializeItemSacrifice(ItemId, totalSacrificed);
			NetManager.Instance.Broadcast(response);

			rejectPacket = false;
#endif
		}
	}
}
