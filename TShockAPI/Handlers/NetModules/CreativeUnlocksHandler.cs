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
	/// Handles creative unlock requests (Journey mode research)
	/// Updated for Terraria 1.4.5 compatibility
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

			// Validate the player ID matches the sender (anti-cheat)
			if (PlayerId != player.Index)
			{
				TShock.Log.ConsoleDebug(
					GetString($"NetModuleHandler received research packet with mismatched player ID from {player.Name} (sent {PlayerId}, expected {player.Index})")
				);
				rejectPacket = true;
				return;
			}

			// Record the sacrifice in the database
			var totalSacrificed = TShock.ResearchDatastore.SacrificeItem(ItemId, Amount, player);

			// Terraria 1.4.5: Broadcast research update to all players
			// Using NetCreativeUnlocksPlayerReportModule instead of the removed NetCreativeUnlocksModule
			BroadcastResearchUpdate(player, ItemId, totalSacrificed);

			rejectPacket = false;
		}

		/// <summary>
		/// Broadcasts research progress to all players using the 1.4.5+ compatible method.
		/// Works around vanilla limitations where the sender doesn't receive their own update.
		/// </summary>
		/// <param name="player">The player who performed the research</param>
		/// <param name="itemId">The item that was researched</param>
		/// <param name="totalAmount">The total amount sacrificed for this item</param>
		private static void BroadcastResearchUpdate(TSPlayer player, int itemId, int totalAmount)
		{
			// Broadcast to all connected players including the sender
			// This fixes the SSC issue where vanilla doesn't send back to the originator
			foreach (var plr in TShock.Players)
			{
				if (plr != null && plr.Active && plr.ConnectionAlive)
				{
					SendResearchUpdateToPlayer(plr, player.Index, itemId, totalAmount);
				}
			}
		}

		/// <summary>
		/// Sends a research update to a specific player
		/// </summary>
		/// <param name="targetPlayer">The player to send the update to</param>
		/// <param name="researcherIndex">The index of the player who performed the research</param>
		/// <param name="itemId">The item ID that was researched</param>
		/// <param name="totalAmount">The total amount sacrificed</param>
		internal static void SendResearchUpdateToPlayer(TSPlayer targetPlayer, int researcherIndex, int itemId, int totalAmount)
		{
			try
			{
				// Use NetCreativeUnlocksPlayerReportModule.Serialize if available in OTAPI 3.3.4+
				// This creates a properly formatted packet for 1.4.5
				var response = NetCreativeUnlocksPlayerReportModule.Serialize(researcherIndex, itemId, totalAmount);
				NetManager.Instance.SendToClient(response, targetPlayer.Index);
			}
			catch (Exception ex)
			{
				TShock.Log.ConsoleDebug(
					GetString($"Failed to send research update to {targetPlayer.Name}: {ex.Message}")
				);
			}
		}
	}
}
