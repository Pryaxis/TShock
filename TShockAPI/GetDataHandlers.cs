/*
TShock, a server mod for Terraria
Copyright (C) 2011-2019 Pryaxis & TShock Contributors

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Streams;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using TShockAPI.DB;
using TShockAPI.Net;
using Terraria;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.GameContent.Tile_Entities;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using TShockAPI.Localization;
using TShockAPI.Models;
using TShockAPI.Models.PlayerUpdate;
using TShockAPI.Models.Projectiles;
using Terraria.Net;
using Terraria.GameContent.NetModules;

namespace TShockAPI
{
	public delegate bool GetDataHandlerDelegate(GetDataHandlerArgs args);

	public class GetDataHandlerArgs : EventArgs
	{
		public TSPlayer Player { get; private set; }
		public MemoryStream Data { get; private set; }

		public Player TPlayer
		{
			get { return Player.TPlayer; }
		}

		public GetDataHandlerArgs(TSPlayer player, MemoryStream data)
		{
			Player = player;
			Data = data;
		}
	}

	/// <summary>
	/// A custom HandledEventArgs that contains TShock's TSPlayer for the triggering uesr and the Terraria MP data stream.
	/// Differentiated by GetDataHandlerArgs because it can be handled and responds to being handled.
	/// </summary>
	public class GetDataHandledEventArgs : HandledEventArgs
	{
		/// <summary>The TSPlayer that triggered the event.</summary>
		public TSPlayer Player { get; set; }

		/// <summary>The raw MP packet data associated with the event.</summary>
		public MemoryStream Data { get; set; }
	}

	public static class GetDataHandlers
	{
		private static Dictionary<PacketTypes, GetDataHandlerDelegate> GetDataHandlerDelegates;

		public static void InitGetDataHandler()
		{
			GetDataHandlerDelegates = new Dictionary<PacketTypes, GetDataHandlerDelegate>
				{
					{ PacketTypes.PlayerInfo, HandlePlayerInfo },
					{ PacketTypes.PlayerSlot, HandlePlayerSlot },
					{ PacketTypes.ContinueConnecting2, HandleConnecting },
					{ PacketTypes.TileGetSection, HandleGetSection },
					{ PacketTypes.PlayerSpawn, HandleSpawn },
					{ PacketTypes.PlayerUpdate, HandlePlayerUpdate },
					{ PacketTypes.PlayerHp, HandlePlayerHp },
					{ PacketTypes.Tile, HandleTile },
					{ PacketTypes.DoorUse, HandleDoorUse },
					{ PacketTypes.TileSendSquare, HandleSendTileRect },
					{ PacketTypes.ItemDrop, HandleItemDrop },
					{ PacketTypes.ItemOwner, HandleItemOwner },
					{ PacketTypes.ProjectileNew, HandleProjectileNew },
					{ PacketTypes.NpcStrike, HandleNpcStrike },
					{ PacketTypes.ProjectileDestroy, HandleProjectileKill },
					{ PacketTypes.TogglePvp, HandleTogglePvp },
					{ PacketTypes.ChestGetContents, HandleChestOpen },
					{ PacketTypes.ChestItem, HandleChestItem },
					{ PacketTypes.ChestOpen, HandleChestActive },
					{ PacketTypes.PlaceChest, HandlePlaceChest },
					{ PacketTypes.Zones, HandlePlayerZone },
					{ PacketTypes.PasswordSend, HandlePassword },
					{ PacketTypes.NpcTalk, HandleNpcTalk },
					{ PacketTypes.PlayerAnimation, HandlePlayerAnimation },
					{ PacketTypes.PlayerMana, HandlePlayerMana },
					{ PacketTypes.PlayerTeam, HandlePlayerTeam },
					{ PacketTypes.SignRead, HandleSignRead },
					{ PacketTypes.SignNew, HandleSign },
					{ PacketTypes.LiquidSet, HandleLiquidSet },
					{ PacketTypes.PlayerBuff, HandlePlayerBuffList },
					{ PacketTypes.NpcSpecial, HandleSpecial },
					{ PacketTypes.NpcAddBuff, HandleNPCAddBuff },
					{ PacketTypes.PlayerAddBuff, HandlePlayerAddBuff },
					{ PacketTypes.UpdateNPCHome, HandleUpdateNPCHome },
					{ PacketTypes.SpawnBossorInvasion, HandleSpawnBoss },
					{ PacketTypes.PaintTile, HandlePaintTile },
					{ PacketTypes.PaintWall, HandlePaintWall },
					{ PacketTypes.Teleport, HandleTeleport },
					{ PacketTypes.PlayerHealOther, HandleHealOther },
					{ PacketTypes.CatchNPC, HandleCatchNpc },
					{ PacketTypes.ReleaseNPC, HandleReleaseNpc },
					{ PacketTypes.TeleportationPotion, HandleTeleportationPotion },
					{ PacketTypes.CompleteAnglerQuest, HandleCompleteAnglerQuest },
					{ PacketTypes.NumberOfAnglerQuestsCompleted, HandleNumberOfAnglerQuestsCompleted },
					{ PacketTypes.PlaceObject, HandlePlaceObject },
					{ PacketTypes.LoadNetModule, HandleLoadNetModule },
					{ PacketTypes.PlaceTileEntity, HandlePlaceTileEntity },
					{ PacketTypes.PlaceItemFrame, HandlePlaceItemFrame },
					{ PacketTypes.UpdateItemDrop, HandleItemDrop },
					{ PacketTypes.SyncExtraValue, HandleSyncExtraValue },
					{ PacketTypes.KillPortal, HandleKillPortal },
					{ PacketTypes.PlayerTeleportPortal, HandlePlayerPortalTeleport },
					{ PacketTypes.NpcTeleportPortal, HandleNpcTeleportPortal },
					{ PacketTypes.GemLockToggle, HandleGemLockToggle },
					{ PacketTypes.MassWireOperation, HandleMassWireOperation },
					{ PacketTypes.ToggleParty, HandleToggleParty },
					{ PacketTypes.CrystalInvasionStart, HandleOldOnesArmy },
					{ PacketTypes.PlayerHurtV2, HandlePlayerDamageV2 },
					{ PacketTypes.PlayerDeathV2, HandlePlayerKillMeV2 },
					{ PacketTypes.Emoji, HandleEmoji },
					{ PacketTypes.TileEntityDisplayDollItemSync, HandleTileEntityDisplayDollItemSync },
					{ PacketTypes.RequestTileEntityInteraction, HandleRequestTileEntityInteraction },
					{ PacketTypes.SyncTilePicking, HandleSyncTilePicking },
					{ PacketTypes.SyncRevengeMarker, HandleSyncRevengeMarker },
					{ PacketTypes.LandGolfBallInCup, HandleLandGolfBallInCup },
					{ PacketTypes.FishOutNPC, HandleFishOutNPC },
					{ PacketTypes.FoodPlatterTryPlacing, HandleFoodPlatterTryPlacing },
					{ PacketTypes.SyncCavernMonsterType, HandleSyncCavernMonsterType },
					{ PacketTypes.SyncLoadout, HandleSyncLoadout }
				};
		}

		public static bool HandlerGetData(PacketTypes type, TSPlayer player, MemoryStream data)
		{
			GetDataHandlerDelegate handler;
			if (GetDataHandlerDelegates.TryGetValue(type, out handler))
			{
				try
				{
					return handler(new GetDataHandlerArgs(player, data));
				}
				catch (Exception ex)
				{
					TShock.Log.Error(ex.ToString());
					return true;
				}
			}
			return false;
		}

		#region Events

		public class PlayerInfoEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The Terraria playerID of the player
			/// </summary>
			public byte PlayerId { get; set; }
			/// <summary>
			/// Hair color
			/// </summary>
			public byte Hair { get; set; }
			/// <summary>
			/// Clothing style. 0-3 are for male characters, and 4-7 are for female characters.
			/// </summary>
			public int Style { get; set; }
			/// <summary>
			/// Character difficulty
			/// </summary>
			public byte Difficulty { get; set; }
			/// <summary>
			/// Player/character name
			/// </summary>
			public string Name { get; set; }
		}
		/// <summary>
		/// PlayerInfo - called at a PlayerInfo event
		/// If this is cancelled, the server will kick the player. If this should be changed in the future, let someone know.
		/// </summary>
		public static HandlerList<PlayerInfoEventArgs> PlayerInfo = new HandlerList<PlayerInfoEventArgs>();
		private static bool OnPlayerInfo(TSPlayer player, MemoryStream data, byte _plrid, byte _hair, int _style, byte _difficulty, string _name)
		{
			if (PlayerInfo == null)
				return false;

			var args = new PlayerInfoEventArgs
			{
				Player = player,
				Data = data,
				PlayerId = _plrid,
				Hair = _hair,
				Style = _style,
				Difficulty = _difficulty,
				Name = _name,
			};
			PlayerInfo.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a PlayerSlot event
		/// </summary>
		public class PlayerSlotEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The Terraria playerID
			/// </summary>
			public byte PlayerId { get; set; }
			/// <summary>
			/// The slot edited
			/// </summary>
			public short Slot { get; set; }
			/// <summary>
			/// The stack edited
			/// </summary>
			public short Stack { get; set; }
			/// <summary>
			/// The item prefix
			/// </summary>
			public byte Prefix { get; set; }
			/// <summary>
			/// Item type
			/// </summary>
			public short Type { get; set; }
		}
		/// <summary>
		/// PlayerSlot - called at a PlayerSlot event
		/// </summary>
		public static HandlerList<PlayerSlotEventArgs> PlayerSlot = new HandlerList<PlayerSlotEventArgs>();
		private static bool OnPlayerSlot(TSPlayer player, MemoryStream data, byte _plr, short _slot, short _stack, byte _prefix, short _type)
		{
			if (PlayerSlot == null)
				return false;

			var args = new PlayerSlotEventArgs
			{
				Player = player,
				Data = data,
				PlayerId = _plr,
				Slot = _slot,
				Stack = _stack,
				Prefix = _prefix,
				Type = _type
			};
			PlayerSlot.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>The arguments to a GetSection packet.</summary>
		public class GetSectionEventArgs : GetDataHandledEventArgs
		{
			/// <summary>The X position requested. Or -1 for spawn.</summary>
			public int X { get; set; }

			/// <summary>The Y position requested. Or -1 for spawn.</summary>
			public int Y { get; set; }
		}
		/// <summary>The hook for a GetSection event.</summary>
		public static HandlerList<GetSectionEventArgs> GetSection = new HandlerList<GetSectionEventArgs>();
		private static bool OnGetSection(TSPlayer player, MemoryStream data, int x, int y)
		{
			if (GetSection == null)
				return false;

			var args = new GetSectionEventArgs
			{
				Player = player,
				Data = data,
				X = x,
				Y = y,
			};

			GetSection.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a PlayerUpdate event
		/// </summary>
		public class PlayerUpdateEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The Terraria playerID of the player
			/// </summary>
			public byte PlayerId { get; set; }
			/// <summary>
			/// Control direction (BitFlags)
			/// </summary>
			public ControlSet Control { get; set; }
			/// <summary>
			/// Misc Data Set 1
			/// </summary>
			public MiscDataSet1 MiscData1 { get; set; }
			/// <summary>
			/// Misc Data Set 2
			/// </summary>
			public MiscDataSet2 MiscData2 { get; set; }
			/// <summary>
			/// Misc Data Set 3
			/// </summary>
			public MiscDataSet3 MiscData3 { get; set; }
			/// <summary>
			/// The selected item in player's hand.
			/// </summary>
			public byte SelectedItem { get; set; }
			/// <summary>
			/// Position of the player.
			/// </summary>
			public Vector2 Position { get; set; }
			/// <summary>
			/// Velocity of the player.
			/// </summary>
			public Vector2 Velocity { get; set; }
			/// <summary>
			/// Original position of the player when using Potion of Return.
			/// </summary>
			public Vector2? OriginalPos { get; set; }
			/// <summary>
			/// Home Position of the player for Potion of Return.
			/// </summary>
			public Vector2? HomePos { get; set; }

		}
		/// <summary>
		/// PlayerUpdate - When the player sends it's updated information to the server
		/// </summary>
		public static HandlerList<PlayerUpdateEventArgs> PlayerUpdate = new HandlerList<PlayerUpdateEventArgs>();
		private static bool OnPlayerUpdate(
			TSPlayer player,
			MemoryStream data,
			byte plr,
			ControlSet control,
			MiscDataSet1 miscData1,
			MiscDataSet2 miscData2,
			MiscDataSet3 miscData3,
			byte selectedItem,
			Vector2 position,
			Vector2 velocity,
			Vector2? originalPos,
			Vector2? homePos)
		{
			if (PlayerUpdate == null)
				return false;

			var args = new PlayerUpdateEventArgs
			{
				Player = player,
				Data = data,
				PlayerId = plr,
				Control = control,
				MiscData1 = miscData1,
				MiscData2 = miscData2,
				MiscData3 = miscData3,
				SelectedItem = selectedItem,
				Position = position,
				Velocity = velocity,
				OriginalPos = originalPos,
				HomePos = homePos
			};
			PlayerUpdate.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a PlayerHP event
		/// </summary>
		public class PlayerHPEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The Terraria playerID of the player
			/// </summary>
			public byte PlayerId { get; set; }
			/// <summary>
			/// Current HP
			/// </summary>
			public short Current { get; set; }
			/// <summary>
			/// Maximum HP
			/// </summary>
			public short Max { get; set; }
		}
		/// <summary>
		/// PlayerHP - called at a PlayerHP event
		/// </summary>
		public static HandlerList<PlayerHPEventArgs> PlayerHP = new HandlerList<PlayerHPEventArgs>();
		private static bool OnPlayerHP(TSPlayer player, MemoryStream data, byte _plr, short _cur, short _max)
		{
			if (PlayerHP == null)
				return false;

			var args = new PlayerHPEventArgs
			{
				Player = player,
				Data = data,
				PlayerId = _plr,
				Current = _cur,
				Max = _max,
			};
			PlayerHP.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// Used when a TileEdit event is called.
		/// </summary>
		public class TileEditEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The tile coordinate on the X plane
			/// </summary>
			public int X { get; set; }

			/// <summary>
			/// The tile coordinate on the Y plane
			/// </summary>
			public int Y { get; set; }

			/// <summary>
			/// The Tile ID being edited.
			/// </summary>
			public short EditData { get; set; }
			/// <summary>
			/// The EditType.
			/// (KillTile = 0, PlaceTile = 1, KillWall = 2, PlaceWall = 3, KillTileNoItem = 4, PlaceWire = 5, KillWire = 6)
			/// </summary>
			public EditAction Action { get; set; }

			/// <summary>
			/// Did the tile get destroyed successfully.
			/// </summary>
			public EditType editDetail { get; set; }

			/// <summary>
			/// Used when a tile is placed to denote a subtype of tile. (e.g. for tile id 21: Chest = 0, Gold Chest = 1)
			/// </summary>
			public byte Style { get; set; }
		}
		/// <summary>
		/// TileEdit - called when a tile is placed or destroyed
		/// </summary>
		public static HandlerList<TileEditEventArgs> TileEdit = new HandlerList<TileEditEventArgs>();
		private static bool OnTileEdit(TSPlayer ply, MemoryStream data, int x, int y, EditAction action, EditType editDetail, short editData, byte style)
		{
			if (TileEdit == null)
				return false;

			var args = new TileEditEventArgs
			{
				Player = ply,
				Data = data,
				X = x,
				Y = y,
				Action = action,
				EditData = editData,
				editDetail = editDetail,
				Style = style
			};
			TileEdit.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// DoorUseEventArgs - the arguments for a DoorUse event
		/// </summary>
		public class DoorUseEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// X - The x position of the door being used
			/// </summary>
			public short X { get; set; }
			/// <summary>
			/// Y - The y position of the door being used
			/// </summary>
			public short Y { get; set; }
			/// <summary>
			/// Direction - Information about which way the door opens or where the player is relative to the door
			/// </summary>
			public byte Direction { get; set; }
			/// <summary>
			/// Action - The type of thing happening to the door
			/// </summary>
			public DoorAction Action { get; set; }
		}

		/// <summary>
		/// DoorUse - called when a door is opened or closed (normal or trap)
		/// </summary>
		public static HandlerList<DoorUseEventArgs> DoorUse = new HandlerList<DoorUseEventArgs>();
		private static bool OnDoorUse(TSPlayer ply, MemoryStream data, short x, short y, byte direction, DoorAction action)
		{
			if (DoorUse == null)
				return false;

			var args = new DoorUseEventArgs
			{
				Player = ply,
				X = x,
				Y = y,
				Direction = direction,
				Action = action
			};
			DoorUse.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a SendTileRect event
		/// </summary>
		public class SendTileRectEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// X position of the rectangle
			/// </summary>
			public short TileX { get; set; }

			/// <summary>
			/// Y position of the rect
			/// </summary>
			public short TileY { get; set; }

			/// <summary>
			/// Width of the rectangle
			/// </summary>
			public byte Width { get; set; }

			/// <summary>
			/// Length of the rectangle
			/// </summary>
			public byte Length { get; set; }

			/// <summary>
			/// Change type involved in the rectangle
			/// </summary>
			public TileChangeType ChangeType { get; set; }
		}

		/// <summary>
		/// When the player sends a tile square
		/// </summary>
		public static HandlerList<SendTileRectEventArgs> SendTileRect = new HandlerList<SendTileRectEventArgs>();
		private static bool OnSendTileRect(TSPlayer player, MemoryStream data, short tilex, short tiley, byte width, byte length, TileChangeType changeType = TileChangeType.None)
		{
			if (SendTileRect == null)
				return false;

			var args = new SendTileRectEventArgs
			{
				Player = player,
				Data = data,
				TileX = tilex,
				TileY = tiley,
				Width = width,
				Length = length,
				ChangeType = changeType
			};

			SendTileRect.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in an ItemDrop event
		/// </summary>
		public class ItemDropEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// ID of the item.
			/// If below 400 and NetID(Type) is 0 Then Set Null. If ItemID is 400 Then New Item
			/// </summary>
			public short ID { get; set; }
			/// <summary>
			/// Position of the item
			/// </summary>
			public Vector2 Position { get; set; }
			/// <summary>
			/// Velocity at which the item is deployed
			/// </summary>
			public Vector2 Velocity { get; set; }
			/// <summary>
			/// Stacks
			/// </summary>
			public short Stacks { get; set; }
			/// <summary>
			/// Prefix of the item
			/// </summary>
			public byte Prefix { get; set; }
			/// <summary>
			/// No Delay on pickup
			/// </summary>
			public bool NoDelay { get; set; }
			/// <summary>
			/// Item type
			/// </summary>
			public short Type { get; set; }
		}
		/// <summary>
		/// ItemDrop - Called when an item is dropped
		/// </summary>
		public static HandlerList<ItemDropEventArgs> ItemDrop = new HandlerList<ItemDropEventArgs>();
		private static bool OnItemDrop(TSPlayer player, MemoryStream data, short id, Vector2 pos, Vector2 vel, short stacks, byte prefix, bool noDelay, short type)
		{
			if (ItemDrop == null)
				return false;

			var args = new ItemDropEventArgs
			{
				Player = player,
				Data = data,
				ID = id,
				Position = pos,
				Velocity = vel,
				Stacks = stacks,
				Prefix = prefix,
				NoDelay = noDelay,
				Type = type,
			};
			ItemDrop.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a NewProjectile event
		/// </summary>
		public class NewProjectileEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// ???
			/// </summary>
			public short Identity { get; set; }
			/// <summary>
			/// Location of the projectile
			/// </summary>
			public Vector2 Position { get; set; }
			/// <summary>
			/// Velocity of the projectile
			/// </summary>
			public Vector2 Velocity { get; set; }
			/// <summary>
			/// Knockback
			/// </summary>
			public float Knockback { get; set; }
			/// <summary>
			/// Damage from the projectile
			/// </summary>
			public short Damage { get; set; }
			/// <summary>
			/// Terraria playerID owner of the projectile
			/// </summary>
			public byte Owner { get; set; }
			/// <summary>
			/// Type of projectile
			/// </summary>
			public short Type { get; set; }
			/// <summary>
			/// ???
			/// </summary>
			public int Index { get; set; }

			/// <summary>
			/// The special meaning of the projectile.
			/// </summary>
			public float[] Ai { get; set; }
		}
		/// <summary>
		/// NewProjectile - Called when a client creates a new projectile
		/// </summary>
		public static HandlerList<NewProjectileEventArgs> NewProjectile = new HandlerList<NewProjectileEventArgs>();
		private static bool OnNewProjectile(MemoryStream data, short ident, Vector2 pos, Vector2 vel, float knockback, short dmg, byte owner, short type, int index, TSPlayer player, float[] ai)
		{
			if (NewProjectile == null)
				return false;

			var args = new NewProjectileEventArgs
			{
				Data = data,
				Identity = ident,
				Position = pos,
				Velocity = vel,
				Knockback = knockback,
				Damage = dmg,
				Owner = owner,
				Type = type,
				Index = index,
				Player = player,
				Ai = ai
			};
			NewProjectile.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use with a NPCStrike event
		/// </summary>
		public class NPCStrikeEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// ???
			/// </summary>
			public short ID { get; set; }
			/// <summary>
			/// Direction the damage occurred from
			/// </summary>
			public byte Direction { get; set; }
			/// <summary>
			/// Amount of damage
			/// </summary>
			public short Damage { get; set; }
			/// <summary>
			/// Knockback
			/// </summary>
			public float Knockback { get; set; }
			/// <summary>
			/// Critical?
			/// </summary>
			public byte Critical { get; set; }
		}
		/// <summary>
		/// NPCStrike - Called when an NPC is attacked
		/// </summary>
		public static HandlerList<NPCStrikeEventArgs> NPCStrike = new HandlerList<NPCStrikeEventArgs>();
		private static bool OnNPCStrike(TSPlayer player, MemoryStream data, short id, byte dir, short dmg, float knockback, byte crit)
		{
			if (NPCStrike == null)
				return false;

			var args = new NPCStrikeEventArgs
			{
				Player = player,
				Data = data,
				ID = id,
				Direction = dir,
				Damage = dmg,
				Knockback = knockback,
				Critical = crit,
			};
			NPCStrike.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>The arguments to the ProjectileKill packet.</summary>
		public class ProjectileKillEventArgs : GetDataHandledEventArgs
		{
			/// <summary>The projectile's identity...?</summary>
			public int ProjectileIdentity;
			/// <summary>The player index of the projectile's owner (Main.players).</summary>
			public byte ProjectileOwner;
			/// <summary>The index of the projectile in Main.projectile.</summary>
			public int ProjectileIndex;
		}
		/// <summary>The event fired when a projectile kill packet is received.</summary>
		public static HandlerList<ProjectileKillEventArgs> ProjectileKill = new HandlerList<ProjectileKillEventArgs>();
		/// <summary>Fires the ProjectileKill event.</summary>
		/// <param name="player">The TSPlayer that caused the event.</param>
		/// <param name="data">The MemoryStream containing the raw event data.</param>
		/// <param name="identity">The projectile identity (from the packet).</param>
		/// <param name="owner">The projectile's owner (from the packet).</param>
		/// <param name="index">The projectile's index (from Main.projectiles).</param>
		/// <returns>bool</returns>
		private static bool OnProjectileKill(TSPlayer player, MemoryStream data, int identity, byte owner, int index)
		{
			if (ProjectileKill == null)
				return false;

			var args = new ProjectileKillEventArgs
			{
				Player = player,
				Data = data,
				ProjectileIdentity = identity,
				ProjectileOwner = owner,
				ProjectileIndex = index,
			};

			ProjectileKill.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a TogglePvp event
		/// </summary>
		public class TogglePvpEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The Terraria player ID of the player
			/// </summary>
			public byte PlayerId { get; set; }
			/// <summary>
			/// Enable/disable pvp?
			/// </summary>
			public bool Pvp { get; set; }
		}
		/// <summary>
		/// TogglePvp - called when a player toggles pvp
		/// </summary>
		public static HandlerList<TogglePvpEventArgs> TogglePvp = new HandlerList<TogglePvpEventArgs>();
		private static bool OnPvpToggled(TSPlayer player, MemoryStream data, byte _id, bool _pvp)
		{
			if (TogglePvp == null)
				return false;

			var args = new TogglePvpEventArgs
			{
				Player = player,
				Data = data,
				PlayerId = _id,
				Pvp = _pvp,
			};
			TogglePvp.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a PlayerSpawn event
		/// </summary>
		public class SpawnEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The Terraria playerID of the player
			/// </summary>
			public byte PlayerId { get; set; }
			/// <summary>
			/// X location of the player's spawn
			/// </summary>
			public int SpawnX { get; set; }
			/// <summary>
			/// Y location of the player's spawn
			/// </summary>
			public int SpawnY { get; set; }
			/// <summary>
			/// Value of the timer countdown before the player can respawn alive.
			/// If > 0, then player is still dead.
			/// </summary>
			public int RespawnTimer { get; set; }
			/// <summary>
			/// Number Of Deaths PVE
			/// </summary>
			public int NumberOfDeathsPVE { get; set; }
			/// <summary>
			/// Number Of Deaths PVP
			/// </summary>
			public int NumberOfDeathsPVP { get; set; }
			/// <summary>
			/// Context of where the player is spawning from.
			/// </summary>
			public PlayerSpawnContext SpawnContext { get; set; }
		}
		/// <summary>
		/// PlayerSpawn - When a player spawns
		/// </summary>
		public static HandlerList<SpawnEventArgs> PlayerSpawn = new HandlerList<SpawnEventArgs>();
		private static bool OnPlayerSpawn(TSPlayer player, MemoryStream data, byte pid, int spawnX, int spawnY, int respawnTimer, int numberOfDeathsPVE, int numberOfDeathsPVP, PlayerSpawnContext spawnContext)
		{
			if (PlayerSpawn == null)
				return false;

			var args = new SpawnEventArgs
			{
				Player = player,
				Data = data,
				PlayerId = pid,
				SpawnX = spawnX,
				SpawnY = spawnY,
				RespawnTimer = respawnTimer,
				NumberOfDeathsPVE = numberOfDeathsPVE,
				NumberOfDeathsPVP = numberOfDeathsPVP,
				SpawnContext = spawnContext
			};
			PlayerSpawn.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a ChestItemChange event
		/// </summary>
		public class ChestItemEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// ChestID
			/// </summary>
			public short ID { get; set; }
			/// <summary>
			/// Slot of the item
			/// </summary>
			public byte Slot { get; set; }
			/// <summary>
			/// How many?
			/// </summary>
			public short Stacks { get; set; }
			/// <summary>
			/// Item prefix
			/// </summary>
			public byte Prefix { get; set; }
			/// <summary>
			/// Item type
			/// </summary>
			public short Type { get; set; }
		}
		/// <summary>
		/// ChestItemChange - Called when an item in a chest changes
		/// </summary>
		public static HandlerList<ChestItemEventArgs> ChestItemChange = new HandlerList<ChestItemEventArgs>();
		private static bool OnChestItemChange(TSPlayer player, MemoryStream data, short id, byte slot, short stacks, byte prefix, short type)
		{
			if (ChestItemChange == null)
				return false;

			var args = new ChestItemEventArgs
			{
				Player = player,
				Data = data,
				ID = id,
				Slot = slot,
				Stacks = stacks,
				Prefix = prefix,
				Type = type,
			};
			ChestItemChange.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use with a ChestOpen event
		/// </summary>
		public class ChestOpenEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// X location of said chest
			/// </summary>
			public int X { get; set; }
			/// <summary>
			/// Y location of said chest
			/// </summary>
			public int Y { get; set; }
		}
		/// <summary>
		/// ChestOpen - Called when any chest is opened
		/// </summary>
		public static HandlerList<ChestOpenEventArgs> ChestOpen = new HandlerList<ChestOpenEventArgs>();
		private static bool OnChestOpen(MemoryStream data, int x, int y, TSPlayer player)
		{
			if (ChestOpen == null)
				return false;

			var args = new ChestOpenEventArgs
			{
				Data = data,
				X = x,
				Y = y,
				Player = player,
			};
			ChestOpen.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a PlaceChest event
		/// </summary>
		public class PlaceChestEventArgs : GetDataHandledEventArgs
		{
			/// <summary>What the packet is doing (see MP packet docs).</summary>
			public int Flag { get; set; }
			/// <summary>
			/// The X coordinate
			/// </summary>
			public int TileX { get; set; }
			/// <summary>
			/// The Y coordinate
			/// </summary>
			public int TileY { get; set; }
			/// <summary>
			/// Place style used
			/// </summary>
			public short Style { get; set; }
		}
		/// <summary>
		/// When a chest is added or removed from the world.
		/// </summary>
		public static HandlerList<PlaceChestEventArgs> PlaceChest = new HandlerList<PlaceChestEventArgs>();
		private static bool OnPlaceChest(TSPlayer player, MemoryStream data, int flag, int tilex, int tiley, short style)
		{
			if (PlaceChest == null)
				return false;

			var args = new PlaceChestEventArgs
			{
				Player = player,
				Data = data,
				Flag = flag,
				TileX = tilex,
				TileY = tiley,
				Style = style
			};
			PlaceChest.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a PlayerZone event
		/// </summary>
		public class PlayerZoneEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The Terraria playerID of the player
			/// </summary>
			public byte PlayerId { get; set; }
			/// <summary>
			/// 0 = Dungeon, 1 = Corruption,2 =Holy, 3 = Meteor, 4 = Jungle, 5 = Snow, 6 = Crimson, 7 = Water Candle
			/// </summary>
			public BitsByte Zone1 { get; set; }
			/// <summary>
			/// 0 = Peace Candle, 1 = Solar Tower, 2 = Vortex Tower, 3 = Nebula Tower, 4 = Stardust Tower, 5 = Desert, 6 = Glowshroom, 7 = Underground Desert
			/// </summary>
			public BitsByte Zone2 { get; set; }
			/// <summary>
			/// 0 = Overworld, 1 = Dirt Layer, 2 = Rock Layer, 3 = Underworld, 4 = Beach, 5 = Rain, 6 = Sandstorm
			/// </summary>
			public BitsByte Zone3 { get; set; }
			/// <summary>
			/// 0 = Old One's Army, 1 = Granite, 2 = Marble, 3 = Hive, 4 = Gem Cave, 5 = Lihzhard Temple, 6 = Graveyard
			/// </summary>
			public BitsByte Zone4 { get; set; }
			/// <summary>
			/// 0 = The Aether
			/// </summary>
			public BitsByte Zone5 { get; set; }
		}
		/// <summary>
		/// PlayerZone - When the player sends it's zone/biome information to the server
		/// </summary>
		public static HandlerList<PlayerZoneEventArgs> PlayerZone = new HandlerList<PlayerZoneEventArgs>();
		private static bool OnPlayerZone(TSPlayer player, MemoryStream data, byte plr, BitsByte zone1, BitsByte zone2, BitsByte zone3, BitsByte zone4, BitsByte zone5)
		{
			if (PlayerZone == null)
				return false;

			var args = new PlayerZoneEventArgs
			{
				Player = player,
				Data = data,
				PlayerId = plr,
				Zone1 = zone1,
				Zone2 = zone2,
				Zone3 = zone3,
				Zone4 = zone4,
				Zone5 = zone5
			};
			PlayerZone.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// Using when player trying to talk to a NPC
		/// </summary>
		public class NpcTalkEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The Terraria ID of the player talking to the NPC
			/// </summary>
			public byte PlayerId { get; set; }

			/// <summary>
			/// The NPC ID of the NPC the player is talking to
			/// </summary>
			public short NPCTalkTarget { get; set; }
		}
		public static HandlerList<NpcTalkEventArgs> NpcTalk = new HandlerList<NpcTalkEventArgs>();
		private static bool OnNpcTalk(TSPlayer player, MemoryStream data, byte _plr, short _npctarget)
		{
			if (NpcTalk == null)
			{
				return false;
			}

			var args = new NpcTalkEventArgs
			{
				Player = player,
				Data = data,
				PlayerId = _plr,
				NPCTalkTarget = _npctarget,
			};
			NpcTalk.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use with a PlayerAnimation event
		/// </summary>
		public class PlayerAnimationEventArgs : GetDataHandledEventArgs { }
		/// <summary>
		/// PlayerAnimation - Called when a player animates
		/// </summary>
		public static HandlerList<PlayerAnimationEventArgs> PlayerAnimation = new HandlerList<PlayerAnimationEventArgs>();
		private static bool OnPlayerAnimation(TSPlayer player, MemoryStream data)
		{
			if (PlayerAnimation == null)
				return false;

			var args = new PlayerAnimationEventArgs
			{
				Player = player,
				Data = data,
			};
			PlayerAnimation.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a PlayerMana event
		/// </summary>
		public class PlayerManaEventArgs : GetDataHandledEventArgs
		{
			public byte PlayerId { get; set; }
			public short Current { get; set; }
			public short Max { get; set; }
		}
		/// <summary>
		/// PlayerMana - called at a PlayerMana event
		/// </summary>
		public static HandlerList<PlayerManaEventArgs> PlayerMana = new HandlerList<PlayerManaEventArgs>();
		private static bool OnPlayerMana(TSPlayer player, MemoryStream data, byte _plr, short _cur, short _max)
		{
			if (PlayerMana == null)
				return false;

			var args = new PlayerManaEventArgs
			{
				Player = player,
				Data = data,
				PlayerId = _plr,
				Current = _cur,
				Max = _max,
			};
			PlayerMana.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a PlayerTeam event
		/// </summary>
		public class PlayerTeamEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The Terraria player ID of the player
			/// </summary>
			public byte PlayerId { get; set; }
			/// <summary>
			/// Enable/disable pvp?
			/// </summary>
			public byte Team { get; set; }
		}
		/// <summary>
		/// TogglePvp - called when a player toggles pvp
		/// </summary>
		public static HandlerList<PlayerTeamEventArgs> PlayerTeam = new HandlerList<PlayerTeamEventArgs>();
		private static bool OnPlayerTeam(TSPlayer player, MemoryStream data, byte _id, byte _team)
		{
			if (PlayerTeam == null)
				return false;

			var args = new PlayerTeamEventArgs
			{
				Player = player,
				Data = data,
				PlayerId = _id,
				Team = _team,
			};
			PlayerTeam.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a SignRead event
		/// </summary>
		public class SignReadEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// X location of the sign
			/// </summary>
			public int X { get; set; }

			/// <summary>
			/// Y location of the sign
			/// </summary>
			public int Y { get; set; }
		}

		/// <summary>
		/// Sign - Called when a sign is read
		/// </summary>
		public static HandlerList<SignReadEventArgs> SignRead = new HandlerList<SignReadEventArgs>();

		private static bool OnSignRead(TSPlayer player, MemoryStream data, int x, int y)
		{
			if (SignRead == null)
				return false;

			var args = new SignReadEventArgs
			{
				Player = player,
				Data = data,
				X = x,
				Y = y,
			};
			SignRead.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a Sign event
		/// </summary>
		public class SignEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The Terraria playerID of the player
			/// </summary>
			public short ID { get; set; }
			/// <summary>
			/// X location of the sign
			/// </summary>
			public int X { get; set; }
			/// <summary>
			/// Y location of the sign
			/// </summary>
			public int Y { get; set; }
		}
		/// <summary>
		/// Sign - Called when a sign is changed
		/// </summary>
		public static HandlerList<SignEventArgs> Sign = new HandlerList<SignEventArgs>();
		private static bool OnSignEvent(TSPlayer player, MemoryStream data, short id, int x, int y)
		{
			if (Sign == null)
				return false;

			var args = new SignEventArgs
			{
				Player = player,
				Data = data,
				ID = id,
				X = x,
				Y = y,
			};
			Sign.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a LiquidSet event
		/// </summary>
		public class LiquidSetEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// X location of the tile
			/// </summary>
			public int TileX { get; set; }
			/// <summary>
			/// Y location of the tile
			/// </summary>
			public int TileY { get; set; }
			/// <summary>
			/// Amount of liquid
			/// </summary>
			public byte Amount { get; set; }
			/// <summary>
			/// Type of Liquid: 0=water, 1=lava, 2=honey
			/// </summary>
			public LiquidType Type { get; set; }
		}

		/// <summary>
		/// LiquidType - supported liquid types
		/// </summary>
		public enum LiquidType : byte
		{
			Water = 0,
			Lava = 1,
			Honey = 2,
			Shimmer = 3,
			Removal = 255 //@Olink: lets hope they never invent 255 fluids or decide to also use this :(
		}

		/// <summary>
		/// LiquidSet - When ever a liquid is set
		/// </summary>
		public static HandlerList<LiquidSetEventArgs> LiquidSet = new HandlerList<LiquidSetEventArgs>();
		private static bool OnLiquidSet(TSPlayer player, MemoryStream data, int tilex, int tiley, byte amount, byte type)
		{
			if (LiquidSet == null)
				return false;

			var args = new LiquidSetEventArgs
			{
				Player = player,
				Data = data,
				TileX = tilex,
				TileY = tiley,
				Amount = amount,
				Type = (LiquidType)type,
			};
			LiquidSet.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a PlayerBuffUpdate event
		/// </summary>
		public class PlayerBuffUpdateEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The Terraria playerID of the player
			/// </summary>
			public byte ID { get; set; }
		}
		/// <summary>
		/// PlayerBuffUpdate - Called when a player updates buffs
		/// </summary>
		public static HandlerList<PlayerBuffUpdateEventArgs> PlayerBuffUpdate = new HandlerList<PlayerBuffUpdateEventArgs>();
		private static bool OnPlayerBuffUpdate(TSPlayer player, MemoryStream data, byte id)
		{
			if (PlayerBuffUpdate == null)
				return false;

			var args = new PlayerBuffUpdateEventArgs
			{
				Player = player,
				Data = data,
				ID = id,
			};
			PlayerBuffUpdate.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use with a NPCSpecial event
		/// </summary>
		public class NPCSpecialEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// ???
			/// </summary>
			public byte ID { get; set; }
			/// <summary>
			/// Type...?
			/// </summary>
			public byte Type { get; set; }
		}
		/// <summary>
		/// NPCSpecial - Called at some point
		/// </summary>
		public static HandlerList<NPCSpecialEventArgs> NPCSpecial = new HandlerList<NPCSpecialEventArgs>();
		private static bool OnNPCSpecial(TSPlayer player, MemoryStream data, byte id, byte type)
		{
			if (NPCSpecial == null)
				return false;

			var args = new NPCSpecialEventArgs
			{
				Player = player,
				Data = data,
				ID = id,
				Type = type,
			};
			NPCSpecial.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a NPCAddBuff event
		/// </summary>
		public class NPCAddBuffEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The ID of the npc
			/// </summary>
			public short ID { get; set; }
			/// <summary>
			/// Buff Type
			/// </summary>
			public int Type { get; set; }
			/// <summary>
			/// Time the buff lasts
			/// </summary>
			public short Time { get; set; }
		}
		/// <summary>
		/// NPCAddBuff - Called when a npc is buffed
		/// </summary>
		public static HandlerList<NPCAddBuffEventArgs> NPCAddBuff = new HandlerList<NPCAddBuffEventArgs>();
		private static bool OnNPCAddBuff(TSPlayer player, MemoryStream data, short id, int type, short time)
		{
			if (NPCAddBuff == null)
				return false;

			var args = new NPCAddBuffEventArgs
			{
				Player = player,
				Data = data,
				ID = id,
				Type = type,
				Time = time
			};
			NPCAddBuff.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a PlayerBuff event
		/// </summary>
		public class PlayerBuffEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The Terraria playerID of the player
			/// </summary>
			public byte ID { get; set; }
			/// <summary>
			/// Buff Type
			/// </summary>
			public int Type { get; set; }
			/// <summary>
			/// Time the buff lasts
			/// </summary>
			public int Time { get; set; }
		}
		/// <summary>
		/// PlayerBuff - Called when a player is buffed
		/// </summary>
		public static HandlerList<PlayerBuffEventArgs> PlayerBuff = new HandlerList<PlayerBuffEventArgs>();
		private static bool OnPlayerBuff(TSPlayer player, MemoryStream data, byte id, int type, int time)
		{
			if (PlayerBuff == null)
				return false;

			var args = new PlayerBuffEventArgs
			{
				Player = player,
				Data = data,
				ID = id,
				Type = type,
				Time = time
			};
			PlayerBuff.Invoke(null, args);
			return args.Handled;
		}

		public enum HouseholdStatus : byte
		{
			None = 0,
			Homeless = 1,
			HasRoom = 2,
		}

		/// <summary>
		/// For use in a NPCHome event
		/// </summary>
		public class NPCHomeChangeEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The Terraria playerID of the player
			/// </summary>
			public short ID { get; set; }
			/// <summary>
			/// X location of the NPC home change
			/// </summary>
			public short X { get; set; }
			/// <summary>
			/// Y location of the NPC home change
			/// </summary>
			public short Y { get; set; }
			/// <summary>
			/// HouseholdStatus of the NPC
			/// </summary>
			public HouseholdStatus HouseholdStatus { get; set; }
		}
		/// <summary>
		/// NPCHome - Called when an NPC's home is changed
		/// </summary>
		public static HandlerList<NPCHomeChangeEventArgs> NPCHome = new HandlerList<NPCHomeChangeEventArgs>();
		private static bool OnUpdateNPCHome(TSPlayer player, MemoryStream data, short id, short x, short y, byte houseHoldStatus)
		{
			if (NPCHome == null)
				return false;

			var args = new NPCHomeChangeEventArgs
			{
				Player = player,
				Data = data,
				ID = id,
				X = x,
				Y = y,
				HouseholdStatus = (HouseholdStatus)houseHoldStatus,
			};
			NPCHome.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use with a PaintTile event
		/// </summary>
		public class PaintTileEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// X Location
			/// </summary>
			public Int32 X { get; set; }
			/// <summary>
			/// Y Location
			/// </summary>
			public Int32 Y { get; set; }
			/// <summary>
			/// Type
			/// </summary>
			public byte type { get; set; }
			/// <summary>
			/// Paint Coat Tile
			/// </summary>
			public byte coatTile { get; set; }
		}
		/// <summary>
		/// NPCStrike - Called when an NPC is attacked
		/// </summary>
		public static HandlerList<PaintTileEventArgs> PaintTile = new HandlerList<PaintTileEventArgs>();
		private static bool OnPaintTile(TSPlayer player, MemoryStream data, Int32 x, Int32 y, byte t, byte ct)
		{
			if (PaintTile == null)
				return false;

			var args = new PaintTileEventArgs
			{
				Player = player,
				Data = data,
				X = x,
				Y = y,
				type = t,
				coatTile = ct
			};
			PaintTile.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use with a PaintWall event
		/// </summary>
		public class PaintWallEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// X Location
			/// </summary>
			public Int32 X { get; set; }
			/// <summary>
			/// Y Location
			/// </summary>
			public Int32 Y { get; set; }
			/// <summary>
			/// Type
			/// </summary>
			public byte type { get; set; }
			/// <summary>
			/// Paint Coat Wall
			/// </summary>
			public byte coatWall { get; set; }
		}
		/// <summary>
		/// Called When a wall is painted
		/// </summary>
		public static HandlerList<PaintWallEventArgs> PaintWall = new HandlerList<PaintWallEventArgs>();
		private static bool OnPaintWall(TSPlayer player, MemoryStream data, Int32 x, Int32 y, byte t, byte cw)
		{
			if (PaintWall == null)
				return false;

			var args = new PaintWallEventArgs
			{
				Player = player,
				Data = data,
				X = x,
				Y = y,
				type = t,
				coatWall = cw
			};
			PaintWall.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use with a NPCStrike event
		/// </summary>
		public class TeleportEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// ???
			/// </summary>
			public Int16 ID { get; set; }
			/// <summary>
			/// Flag is a bit field
			///   if the first bit is set -> 0 = player, 1 = NPC
			///	  if the second bit is set, ignore this packet
			///   if the third bit is set, "get extra info from target" is true
			///   if the fourth bit is set, extra information is valid to read
			/// </summary>
			public byte Flag { get; set; }
			/// <summary>
			/// X Location
			/// </summary>
			public float X { get; set; }
			/// <summary>
			/// Y Location
			/// </summary>
			public float Y { get; set; }
			/// <summary>
			/// Style
			/// </summary>
			public byte Style { get; set; }
			/// <summary>
			/// "Extra info"
			/// </summary>
			public int ExtraInfo { get; set; }
		}
		/// <summary>
		/// NPCStrike - Called when an NPC is attacked
		/// </summary>
		public static HandlerList<TeleportEventArgs> Teleport = new HandlerList<TeleportEventArgs>();
		private static bool OnTeleport(TSPlayer player, MemoryStream data, Int16 id, byte f, float x, float y, byte style, int extraInfo)
		{
			if (Teleport == null)
				return false;

			var args = new TeleportEventArgs
			{
				Player = player,
				Data = data,
				ID = id,
				Flag = f,
				X = x,
				Y = y,
				Style = style,
				ExtraInfo = extraInfo
			};
			Teleport.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>The event args object for the HealOtherPlayer event</summary>
		public class HealOtherPlayerEventArgs : GetDataHandledEventArgs
		{
			/// <summary>The Terraria player index of the target player</summary>
			public byte TargetPlayerIndex { get; set; }

			/// <summary>The amount to heal by</summary>
			public short Amount { get; set; }
		}
		/// <summary>When a player heals another player</summary>
		public static HandlerList<HealOtherPlayerEventArgs> HealOtherPlayer = new HandlerList<HealOtherPlayerEventArgs>();
		private static bool OnHealOtherPlayer(TSPlayer player, MemoryStream data, byte targetPlayerIndex, short amount)
		{
			if (HealOtherPlayer == null)
				return false;

			var args = new HealOtherPlayerEventArgs
			{
				Player = player,
				Data = data,
				TargetPlayerIndex = targetPlayerIndex,
				Amount = amount,
			};

			HealOtherPlayer.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// The ReleaseNPC event arguments
		/// </summary>
		public class ReleaseNpcEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The X value of where NPC released
			/// </summary>
			public int X { get; set; }

			/// <summary>
			/// The Y value of where NPC released
			/// </summary>
			public int Y { get; set; }

			/// <summary>
			/// The NPC Type that player release
			/// </summary>
			public short Type { get; set; }

			/// <summary>
			/// The NPC release style
			/// </summary>
			public byte Style { get; set; }
		}

		/// <summary>
		/// Called when player release a NPC, for checking critter released from item.
		/// </summary>
		public static HandlerList<ReleaseNpcEventArgs> ReleaseNPC = new HandlerList<ReleaseNpcEventArgs>();
		private static bool OnReleaseNpc(TSPlayer player, MemoryStream data, int _x, int _y, short _type, byte _style)
		{
			if (ReleaseNPC == null)
			{
				return false;
			}

			var args = new ReleaseNpcEventArgs
			{
				Player = player,
				Data = data,
				X = _x,
				Y = _y,
				Type = _type,
				Style = _style
			};
			ReleaseNPC.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>The arguments to the PlaceObject hook.</summary>
		public class PlaceObjectEventArgs : GetDataHandledEventArgs
		{
			/// <summary>The X location where the object was placed.</summary>
			public short X { get; set; }

			/// <summary>The Y location where the object was placed.</summary>
			public short Y { get; set; }

			/// <summary>The type of object that was placed.</summary>
			public short Type { get; set; }

			/// <summary>The style of the object was placed.</summary>
			public short Style { get; set; }

			/// <summary>Alternate variation of the object placed.</summary>
			public byte Alternate { get; set; }

			/// <summary>Related to Rubblemaker.</summary>
			public sbyte Random { get; set; }

			/// <summary>The direction the object was placed.</summary>
			public bool Direction { get; set; }
		}
		/// <summary>Fired when an object is placed in the world.</summary>
		public static HandlerList<PlaceObjectEventArgs> PlaceObject = new HandlerList<PlaceObjectEventArgs>();
		private static bool OnPlaceObject(TSPlayer player, MemoryStream data, short x, short y, short type, short style, byte alternate, sbyte random, bool direction)
		{
			if (PlaceObject == null)
				return false;

			var args = new PlaceObjectEventArgs
			{
				Player = player,
				Data = data,
				X = x,
				Y = y,
				Type = type,
				Style = style,
				Alternate = alternate,
				Random = random,
				Direction = direction
			};

			PlaceObject.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>For use in a PlaceTileEntity event.</summary>
		public class PlaceTileEntityEventArgs : GetDataHandledEventArgs
		{
			/// <summary>The X coordinate of the event.</summary>
			public short X { get; set; }

			/// <summary>The Y coordinate of the event.</summary>
			public short Y { get; set; }

			/// <summary>The Type of event.</summary>
			public byte Type { get; set; }
		}
		/// <summary>Fired when a PlaceTileEntity event occurs.</summary>
		public static HandlerList<PlaceTileEntityEventArgs> PlaceTileEntity = new HandlerList<PlaceTileEntityEventArgs>();
		private static bool OnPlaceTileEntity(TSPlayer player, MemoryStream data, short x, short y, byte type)
		{
			if (PlaceTileEntity == null)
				return false;

			var args = new PlaceTileEntityEventArgs
			{
				Player = player,
				Data = data,
				X = x,
				Y = y,
				Type = type
			};

			PlaceTileEntity.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>The arguments to the PlaceItemFrame event.</summary>
		public class PlaceItemFrameEventArgs : GetDataHandledEventArgs
		{
			/// <summary>The X coordinate of the item frame.</summary>
			public short X { get; set; }

			/// <summary>The Y coordinate of the item frame.</summary>
			public short Y { get; set; }

			/// <summary>The ItemID of the item frame.</summary>
			public short ItemID { get; set; }

			/// <summary>The prefix.</summary>
			public byte Prefix { get; set; }

			/// <summary>The stack.</summary>
			public short Stack { get; set; }

			/// <summary>The ItemFrame object associated with this event.</summary>
			public TEItemFrame ItemFrame { get; set; }
		}
		/// <summary>Fired when an ItemFrame is placed.</summary>
		public static HandlerList<PlaceItemFrameEventArgs> PlaceItemFrame = new HandlerList<PlaceItemFrameEventArgs>();
		private static bool OnPlaceItemFrame(TSPlayer player, MemoryStream data, short x, short y, short itemID, byte prefix, short stack, TEItemFrame itemFrame)
		{
			if (PlaceItemFrame == null)
				return false;

			var args = new PlaceItemFrameEventArgs
			{
				Player = player,
				Data = data,
				X = x,
				Y = y,
				ItemID = itemID,
				Prefix = prefix,
				Stack = stack,
				ItemFrame = itemFrame,
			};

			PlaceItemFrame.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>The event args object for the PortalTeleport event</summary>
		public class TeleportThroughPortalEventArgs : GetDataHandledEventArgs
		{
			/// <summary>The Terraria player index of the target player</summary>
			public byte TargetPlayerIndex { get; set; }

			/// <summary>
			/// The position the target player will be at after going through the portal
			/// </summary>
			public Vector2 NewPosition { get; set; }

			/// <summary>
			/// The velocity the target player will have after going through the portal
			/// </summary>
			public Vector2 NewVelocity { get; set; }

			/// <summary>
			/// Index of the portal's color (for use with <see cref="Terraria.GameContent.PortalHelper.GetPortalColor(int)"/>)
			/// </summary>
			public int PortalColorIndex { get; set; }
		}
		/// <summary>When a player passes through a portal</summary>
		public static HandlerList<TeleportThroughPortalEventArgs> PortalTeleport = new HandlerList<TeleportThroughPortalEventArgs>();
		private static bool OnPlayerTeleportThroughPortal(TSPlayer sender, byte targetPlayerIndex, MemoryStream data, Vector2 position, Vector2 velocity, int colorIndex)
		{
			TeleportThroughPortalEventArgs args = new TeleportThroughPortalEventArgs
			{
				TargetPlayerIndex = targetPlayerIndex,
				Data = data,
				Player = sender,
				NewPosition = position,
				NewVelocity = velocity,
				PortalColorIndex = colorIndex
			};

			PortalTeleport.Invoke(null, args);

			return args.Handled;
		}

		/// <summary>
		/// For use with a ToggleGemLock event
		/// </summary>
		public class GemLockToggleEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// X Location
			/// </summary>
			public short X { get; set; }
			/// <summary>
			/// Y Location
			/// </summary>
			public short Y { get; set; }
			/// <summary>
			/// On status
			/// </summary>
			public bool On { get; set; }
		}
		/// <summary>
		/// GemLockToggle - Called when a gem lock is switched
		/// </summary>
		public static HandlerList<GemLockToggleEventArgs> GemLockToggle = new HandlerList<GemLockToggleEventArgs>();
		private static bool OnGemLockToggle(TSPlayer player, MemoryStream data, short x, short y, bool on)
		{
			if (GemLockToggle == null)
				return false;

			var args = new GemLockToggleEventArgs
			{
				Player = player,
				Data = data,
				X = x,
				Y = y,
				On = on
			};
			GemLockToggle.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>The arguments to the MassWireOperation event.</summary>
		public class MassWireOperationEventArgs : GetDataHandledEventArgs
		{
			/// <summary>The start X point in the operation.</summary>
			public short StartX { get; set; }

			/// <summary>The start Y point in the operation.</summary>
			public short StartY { get; set; }

			/// <summary>The end X point in the operation.</summary>
			public short EndX { get; set; }

			/// <summary>The end Y point in the operation.</summary>
			public short EndY { get; set; }

			/// <summary>ToolMode</summary>
			public byte ToolMode { get; set; }
		}
		/// <summary>Fired on a mass wire edit operation.</summary>
		public static HandlerList<MassWireOperationEventArgs> MassWireOperation = new HandlerList<MassWireOperationEventArgs>();
		private static bool OnMassWireOperation(TSPlayer player, MemoryStream data, short startX, short startY, short endX, short endY, byte toolMode)
		{
			if (MassWireOperation == null)
				return false;

			var args = new MassWireOperationEventArgs
			{
				Player = player,
				Data = data,
				StartX = startX,
				StartY = startY,
				EndX = endX,
				EndY = endY,
				ToolMode = toolMode,
			};

			MassWireOperation.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a PlayerDamage event
		/// </summary>
		public class PlayerDamageEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The Terraria playerID of the player
			/// </summary>
			public byte ID { get; set; }
			/// <summary>
			/// The direction the damage is occurring from
			/// </summary>
			public byte Direction { get; set; }
			/// <summary>
			/// Amount of damage
			/// </summary>
			public short Damage { get; set; }
			/// <summary>
			/// If the player has PVP on
			/// </summary>
			public bool PVP { get; set; }
			/// <summary>
			/// Is the damage critical?
			/// </summary>
			public bool Critical { get; set; }
			/// <summary>
			/// Cooldown Counter
			/// </summary>
			public sbyte CooldownCounter { get; set; }
			/// <summary>The reason the player took damage and/or died.</summary>
			public PlayerDeathReason PlayerDeathReason { get; set; }
		}
		/// <summary>
		/// PlayerDamage - Called when a player is damaged
		/// </summary>
		public static HandlerList<PlayerDamageEventArgs> PlayerDamage = new HandlerList<PlayerDamageEventArgs>();
		private static bool OnPlayerDamage(TSPlayer player, MemoryStream data, byte id, byte dir, short dmg, bool pvp, bool crit, sbyte cooldownCounter, PlayerDeathReason playerDeathReason)
		{
			if (PlayerDamage == null)
				return false;

			var args = new PlayerDamageEventArgs
			{
				Player = player,
				Data = data,
				ID = id,
				Direction = dir,
				Damage = dmg,
				PVP = pvp,
				Critical = crit,
				CooldownCounter = cooldownCounter,
				PlayerDeathReason = playerDeathReason,
			};
			PlayerDamage.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a KillMe event
		/// </summary>
		public class KillMeEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The Terraria playerID of the player
			/// </summary>
			public byte PlayerId { get; set; }
			/// <summary>
			/// The direction the damage is coming from (?)
			/// </summary>
			public byte Direction { get; set; }
			/// <summary>
			/// Amount of damage dealt
			/// </summary>
			public short Damage { get; set; }
			/// <summary>
			/// Player's current pvp setting
			/// </summary>
			public bool Pvp { get; set; }
			/// <summary>The reason the player died.</summary>
			public PlayerDeathReason PlayerDeathReason { get; set; }
		}
		/// <summary>
		/// KillMe - Terraria's crappy way of handling damage from players
		/// </summary>
		public static HandlerList<KillMeEventArgs> KillMe = new HandlerList<KillMeEventArgs>();
		private static bool OnKillMe(TSPlayer player, MemoryStream data, byte plr, byte direction, short damage, bool pvp, PlayerDeathReason playerDeathReason)
		{
			if (KillMe == null)
				return false;

			var args = new KillMeEventArgs
			{
				Player = player,
				Data = data,
				PlayerId = plr,
				Direction = direction,
				Damage = damage,
				Pvp = pvp,
				PlayerDeathReason = playerDeathReason,
			};
			KillMe.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in an Emoji event.
		/// </summary>
		public class EmojiEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The player index in the packet, who sends the emoji.
			/// </summary>
			public byte PlayerIndex { get; set; }
			/// <summary>
			/// The ID of the emoji, that is being received.
			/// </summary>
			public byte EmojiID { get; set; }
		}
		/// <summary>
		/// Called when a player sends an emoji.
		/// </summary>
		public static HandlerList<EmojiEventArgs> Emoji = new HandlerList<EmojiEventArgs>();
		private static bool OnEmoji(TSPlayer player, MemoryStream data, byte playerIndex, byte emojiID)
		{
			if (Emoji == null)
				return false;

			var args = new EmojiEventArgs
			{
				Player = player,
				Data = data,
				PlayerIndex = playerIndex,
				EmojiID = emojiID
			};
			Emoji.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a TileEntityDisplayDollItemSync event.
		/// </summary>
		public class DisplayDollItemSyncEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The player index in the packet who modifies the DisplayDoll item slot.
			/// </summary>
			public byte PlayerIndex { get; set; }
			/// <summary>
			/// The ID of the TileEntity that is being modified.
			/// </summary>
			public int TileEntityID { get; set; }
			/// <summary>
			/// The TEDisplayDoll object that is being modified.
			/// </summary>
			public TEDisplayDoll DisplayDollEntity { get; set; }
			/// <summary>
			/// The slot of the DisplayDoll that is being modified.
			/// </summary>
			public int Slot { get; set; }
			/// <summary>
			/// Whether or not the slot that is being modified is a Dye slot.
			/// </summary>
			public bool IsDye { get; set; }
			/// <summary>
			/// The current item that is present in the slot before the modification.
			/// </summary>
			public Item OldItem { get; set; }
			/// <summary>
			/// The item that is about to replace the OldItem in the slot that is being modified.
			/// </summary>
			public Item NewItem { get; set; }
		}
		/// <summary>
		/// Called when a player modifies a DisplayDoll (Mannequin) item slot.
		/// </summary>
		public static HandlerList<DisplayDollItemSyncEventArgs> DisplayDollItemSync = new HandlerList<DisplayDollItemSyncEventArgs>();
		private static bool OnDisplayDollItemSync(TSPlayer player, MemoryStream data, byte playerIndex, int tileEntityID, TEDisplayDoll displayDollEntity, int slot, bool isDye, Item oldItem, Item newItem)
		{
			if (DisplayDollItemSync == null)
				return false;

			var args = new DisplayDollItemSyncEventArgs
			{
				Player = player,
				Data = data,
				PlayerIndex = playerIndex,
				TileEntityID = tileEntityID,
				DisplayDollEntity = displayDollEntity,
				Slot = slot,
				IsDye = isDye,
				OldItem = oldItem,
				NewItem = newItem
			};
			DisplayDollItemSync.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in an OnRequestTileEntityInteraction event.
		/// </summary>
		public class RequestTileEntityInteractionEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The TileEntity object that the player is requesting interaction with.
			/// </summary>
			public TileEntity TileEntity { get; set; }
			/// <summary>
			/// The player index in the packet who requests interaction with the TileEntity.
			/// </summary>
			public byte PlayerIndex { get; set; }
		}
		/// <summary>
		/// Called when a player requests interaction with a TileEntity.
		/// </summary>
		public static HandlerList<RequestTileEntityInteractionEventArgs> RequestTileEntityInteraction = new HandlerList<RequestTileEntityInteractionEventArgs>();
		private static bool OnRequestTileEntityInteraction(TSPlayer player, MemoryStream data, TileEntity tileEntity, byte playerIndex)
		{
			if (RequestTileEntityInteraction == null)
				return false;

			var args = new RequestTileEntityInteractionEventArgs
			{
				Player = player,
				Data = data,
				PlayerIndex = playerIndex,
				TileEntity = tileEntity
			};
			RequestTileEntityInteraction.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a SyncTilePicking event.
		/// </summary>
		public class SyncTilePickingEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The player index in the packet, who sends the tile picking data.
			/// </summary>
			public byte PlayerIndex { get; set; }
			/// <summary>
			/// The X world position of the tile that is being picked.
			/// </summary>
			public short TileX { get; set; }
			/// <summary>
			/// The Y world position of the tile that is being picked.
			/// </summary>
			public short TileY { get; set; }
			/// <summary>
			/// The damage that is being dealt on the tile.
			/// </summary>
			public byte TileDamage { get; set; }
		}
		/// <summary>
		/// Called when a player hits and damages a tile.
		/// </summary>
		public static HandlerList<SyncTilePickingEventArgs> SyncTilePicking = new HandlerList<SyncTilePickingEventArgs>();
		private static bool OnSyncTilePicking(TSPlayer player, MemoryStream data, byte playerIndex, short tileX, short tileY, byte tileDamage)
		{
			if (SyncTilePicking == null)
				return false;

			var args = new SyncTilePickingEventArgs
			{
				PlayerIndex = playerIndex,
				TileX = tileX,
				TileY = tileY,
				TileDamage = tileDamage
			};
			SyncTilePicking.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a LandBallInCup event.
		/// </summary>
		public class LandGolfBallInCupEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The player index in the packet, who puts the ball in the cup.
			/// </summary>
			public byte PlayerIndex { get; set; }
			/// <summary>
			/// The X tile position of where the ball lands in a cup.
			/// </summary>
			public ushort TileX { get; set; }
			/// <summary>
			/// The Y tile position of where the ball lands in a cup.
			/// </summary>
			public ushort TileY { get; set; }
			/// <summary>
			/// The amount of hits it took for the player to land the ball in the cup.
			/// </summary>
			public ushort Hits { get; set; }
			/// <summary>
			/// The type of the projectile that was landed in the cup. A golfball in legit cases.
			/// </summary>
			public ushort ProjectileType { get; set; }
		}

		/// <summary>
		/// Called when a player lands a golf ball in a cup.
		/// </summary>
		public static HandlerList<LandGolfBallInCupEventArgs> LandGolfBallInCup = new HandlerList<LandGolfBallInCupEventArgs>();
		private static bool OnLandGolfBallInCup(TSPlayer player, MemoryStream data, byte playerIndex, ushort tileX, ushort tileY, ushort hits, ushort projectileType)
		{
			if (LandGolfBallInCup == null)
				return false;

			var args = new LandGolfBallInCupEventArgs
			{
				Player = player,
				Data = data,
				PlayerIndex = playerIndex,
				TileX = tileX,
				TileY = tileY,
				Hits = hits,
				ProjectileType = projectileType
			};
			LandGolfBallInCup.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a FishOutNPC event.
		/// </summary>
		public class FishOutNPCEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The X world position of the spawning NPC.
			/// </summary>
			public ushort TileX { get; set; }
			/// <summary>
			/// The Y world position of the spawning NPC.
			/// </summary>
			public ushort TileY { get; set; }
			/// <summary>
			/// The NPC type that is being spawned.
			/// </summary>
			public short NpcID { get; set; }
		}
		/// <summary>
		/// Called when a player fishes out an NPC.
		/// </summary>
		public static HandlerList<FishOutNPCEventArgs> FishOutNPC = new HandlerList<FishOutNPCEventArgs>();
		private static bool OnFishOutNPC(TSPlayer player, MemoryStream data, ushort tileX, ushort tileY, short npcID)
		{
			if (FishOutNPC == null)
				return false;

			var args = new FishOutNPCEventArgs
			{
				Player = player,
				Data = data,
				TileX = tileX,
				TileY = tileY,
				NpcID = npcID
			};
			FishOutNPC.Invoke(null, args);
			return args.Handled;
		}

		public class FoodPlatterTryPlacingEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The X tile position of the placement action.
			/// </summary>
			public short TileX { get; set; }
			/// <summary>
			/// The Y tile position of the placement action.
			/// </summary>
			public short TileY { get; set; }
			/// <summary>
			/// The Item ID that is being placed in the plate.
			/// </summary>
			public short ItemID { get; set; }
			/// <summary>
			/// The prefix of the item that is being placed in the plate.
			/// </summary>
			public byte Prefix { get; set; }
			/// <summary>
			/// The stack of the item that is being placed in the plate.
			/// </summary>
			public short Stack { get; set; }
		}
		/// <summary>
		/// Called when a player is placing an item in a food plate.
		/// </summary>
		public static HandlerList<FoodPlatterTryPlacingEventArgs> FoodPlatterTryPlacing = new HandlerList<FoodPlatterTryPlacingEventArgs>();
		private static bool OnFoodPlatterTryPlacing(TSPlayer player, MemoryStream data, short tileX, short tileY, short itemID, byte prefix, short stack)
		{
			if (FoodPlatterTryPlacing == null)
				return false;

			var args = new FoodPlatterTryPlacingEventArgs
			{
				Player = player,
				Data = data,
				TileX = tileX,
				TileY = tileY,
				ItemID = itemID,
				Prefix = prefix,
				Stack = stack,
			};
			FoodPlatterTryPlacing.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// Used when a net module is loaded
		/// </summary>
		public class ReadNetModuleEventArgs : GetDataHandledEventArgs
		{
			/// <summary>
			/// The type of net module being loaded
			/// </summary>
			public NetModuleType ModuleType { get; set; }
		}

		/// <summary>
		/// Called when a net module is received
		/// </summary>
		public static HandlerList<ReadNetModuleEventArgs> ReadNetModule = new HandlerList<ReadNetModuleEventArgs>();

		private static bool OnReadNetModule(TSPlayer player, MemoryStream data, NetModuleType moduleType)
		{
			if (ReadNetModule == null)
			{
				return false;
			}

			var args = new ReadNetModuleEventArgs
			{
				Player = player,
				Data = data,
				ModuleType = moduleType
			};

			ReadNetModule.Invoke(null, args);
			return args.Handled;
		}

		#endregion

		private static bool HandlePlayerInfo(GetDataHandlerArgs args)
		{
			byte playerid = args.Data.ReadInt8();
			// 0-3 male; 4-7 female
			int skinVariant = args.Data.ReadByte();
			var hair = args.Data.ReadInt8();
			string name = args.Data.ReadString();
			byte hairDye = args.Data.ReadInt8();

			BitsByte hideVisual = args.Data.ReadInt8();
			BitsByte hideVisual2 = args.Data.ReadInt8();
			BitsByte hideMisc = args.Data.ReadInt8();

			Color hairColor = new Color(args.Data.ReadInt8(), args.Data.ReadInt8(), args.Data.ReadInt8());
			Color skinColor = new Color(args.Data.ReadInt8(), args.Data.ReadInt8(), args.Data.ReadInt8());
			Color eyeColor = new Color(args.Data.ReadInt8(), args.Data.ReadInt8(), args.Data.ReadInt8());
			Color shirtColor = new Color(args.Data.ReadInt8(), args.Data.ReadInt8(), args.Data.ReadInt8());
			Color underShirtColor = new Color(args.Data.ReadInt8(), args.Data.ReadInt8(), args.Data.ReadInt8());
			Color pantsColor = new Color(args.Data.ReadInt8(), args.Data.ReadInt8(), args.Data.ReadInt8());
			Color shoeColor = new Color(args.Data.ReadInt8(), args.Data.ReadInt8(), args.Data.ReadInt8());

			BitsByte extra = args.Data.ReadInt8();
			byte difficulty = 0;
			if (extra[0])
			{
				difficulty = 1;
			}
			else if (extra[1])
			{
				difficulty = 2;
			}
			else if (extra[3])
			{
				difficulty = 3;
			}
			bool extraSlot = extra[2];
			BitsByte torchFlags = args.Data.ReadInt8();
			bool usingBiomeTorches = torchFlags[0];
			bool happyFunTorchTime = torchFlags[1];
			bool unlockedBiomeTorches = torchFlags[2];
			bool unlockedSuperCart = torchFlags[3];
			bool enabledSuperCart = torchFlags[4];
			BitsByte bitsByte10 = args.Data.ReadInt8();
			bool usedAegisCrystal = bitsByte10[0];
			bool usedAegisFruit = bitsByte10[1];
			bool usedArcaneCrystal = bitsByte10[2];
			bool usedGalaxyPearl = bitsByte10[3];
			bool usedGummyWorm = bitsByte10[4];
			bool usedAmbrosia = bitsByte10[5];
			bool ateArtisanBread = bitsByte10[6];

			if (OnPlayerInfo(args.Player, args.Data, playerid, hair, skinVariant, difficulty, name))
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandlePlayerInfo rejected plugin phase {0}", name));
				args.Player.Kick(GetString("A plugin on this server stopped your login."), true, true);
				return true;
			}

			if (name.Trim().Length == 0)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandlePlayerInfo rejected name length 0"));
				args.Player.Kick(GetString("You have been Bounced."), true, true);
				return true;
			}

			if (name.Trim().StartsWith("tsi:") || name.Trim().StartsWith("tsn:"))
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / rejecting player for name prefix starting with tsi: or tsn:."));
				args.Player.Kick(GetString("Illegal name: prefixes tsi: and tsn: are forbidden."), true, true);
				return true;
			}

			if (args.Player.ReceivedInfo)
			{
				// Since Terraria 1.2.3 these character properties can change ingame.
				args.Player.TPlayer.hair = hair;
				args.Player.TPlayer.hairColor = hairColor;
				args.Player.TPlayer.hairDye = hairDye;
				args.Player.TPlayer.skinVariant = skinVariant;
				args.Player.TPlayer.skinColor = skinColor;
				args.Player.TPlayer.eyeColor = eyeColor;
				args.Player.TPlayer.pantsColor = pantsColor;
				args.Player.TPlayer.shirtColor = shirtColor;
				args.Player.TPlayer.underShirtColor = underShirtColor;
				args.Player.TPlayer.shoeColor = shoeColor;
				//@Olink: If you need to change bool[10], please make sure you also update the for loops below to account for it.
				//There are two arrays from terraria that we only have a single array for.  You will need to make sure that you are looking
				//at the correct terraria array (hideVisual or hideVisual2).
				args.Player.TPlayer.hideVisibleAccessory = new bool[10];
				for (int i = 0; i < 8; i++)
					args.Player.TPlayer.hideVisibleAccessory[i] = hideVisual[i];
				for (int i = 0; i < 2; i++)
					args.Player.TPlayer.hideVisibleAccessory[i + 8] = hideVisual2[i];
				args.Player.TPlayer.hideMisc = hideMisc;
				args.Player.TPlayer.extraAccessory = extraSlot;
				args.Player.TPlayer.UsingBiomeTorches = usingBiomeTorches;
				args.Player.TPlayer.happyFunTorchTime = happyFunTorchTime;
				args.Player.TPlayer.unlockedBiomeTorches = unlockedBiomeTorches;
				args.Player.TPlayer.ateArtisanBread = ateArtisanBread;
				args.Player.TPlayer.usedAegisCrystal = usedAegisCrystal;
				args.Player.TPlayer.usedAegisFruit = usedAegisFruit;
				args.Player.TPlayer.usedArcaneCrystal = usedArcaneCrystal;
				args.Player.TPlayer.usedGalaxyPearl = usedGalaxyPearl;
				args.Player.TPlayer.usedGummyWorm = usedGummyWorm;
				args.Player.TPlayer.usedAmbrosia = usedAmbrosia;
				args.Player.TPlayer.unlockedSuperCart = unlockedSuperCart;
				args.Player.TPlayer.enabledSuperCart = enabledSuperCart;

				NetMessage.SendData((int)PacketTypes.PlayerInfo, -1, args.Player.Index, NetworkText.FromLiteral(args.Player.Name), args.Player.Index);
				return true;
			}
			if (TShock.Config.Settings.SoftcoreOnly && difficulty != 0)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandlePlayerInfo rejected softcore required"));
				args.Player.Kick(GetString("You need to join with a softcore player."), true, true);
				return true;
			}
			if (TShock.Config.Settings.MediumcoreOnly && difficulty < 1)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandlePlayerInfo rejected mediumcore required"));
				args.Player.Kick(GetString("You need to join with a mediumcore player or higher."), true, true);
				return true;
			}
			if (TShock.Config.Settings.HardcoreOnly && difficulty < 2)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandlePlayerInfo rejected hardcore required"));
				args.Player.Kick(GetString("You need to join with a hardcore player."), true, true);
				return true;
			}
			args.Player.Difficulty = difficulty;
			args.TPlayer.name = name;
			args.Player.ReceivedInfo = true;

			return false;
		}

		private static bool HandlePlayerSlot(GetDataHandlerArgs args)
		{
			byte plr = args.Data.ReadInt8();
			short slot = args.Data.ReadInt16();
			short stack = args.Data.ReadInt16();
			byte prefix = args.Data.ReadInt8();
			short type = args.Data.ReadInt16();

			// Players send a slot update packet for each inventory slot right after they've joined.
			bool bypassTrashCanCheck = false;
			if (plr == args.Player.Index && !args.Player.HasSentInventory && slot == NetItem.MaxInventory)
			{
				args.Player.HasSentInventory = true;
				bypassTrashCanCheck = true;
			}

			if (OnPlayerSlot(args.Player, args.Data, plr, slot, stack, prefix, type) || plr != args.Player.Index || slot < 0 ||
				slot > NetItem.MaxInventory)
				return true;
			if (args.Player.IgnoreSSCPackets)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandlePlayerSlot rejected ignore ssc packets"));
				args.Player.SendData(PacketTypes.PlayerSlot, "", args.Player.Index, slot, prefix);
				return true;
			}

			// Garabage? Or will it cause some internal initialization or whatever?
			var item = new Item();
			item.netDefaults(type);
			item.Prefix(prefix);

			if (args.Player.IsLoggedIn)
			{
				args.Player.PlayerData.StoreSlot(slot, type, prefix, stack);
			}
			else if (Main.ServerSideCharacter && TShock.Config.Settings.DisableLoginBeforeJoin && !bypassTrashCanCheck &&
					 args.Player.HasSentInventory && !args.Player.HasPermission(Permissions.bypassssc))
			{
				// The player might have moved an item to their trash can before they performed a single login attempt yet.
				args.Player.IsDisabledPendingTrashRemoval = true;
			}

			if (slot == 58) //this is the hand
			{
				item.stack = stack;
				args.Player.ItemInHand = item;
			}

			return false;
		}

		private static bool HandleConnecting(GetDataHandlerArgs args)
		{
			var account = TShock.UserAccounts.GetUserAccountByName(args.Player.Name);//
			args.Player.DataWhenJoined = new PlayerData(args.Player);
			args.Player.DataWhenJoined.CopyCharacter(args.Player);
			args.Player.PlayerData = new PlayerData(args.Player);
			args.Player.PlayerData.CopyCharacter(args.Player);

			if (account != null && !TShock.Config.Settings.DisableUUIDLogin)
			{
				if (account.UUID == args.Player.UUID)
				{
					if (args.Player.State == 1)
						args.Player.State = 2;
					NetMessage.SendData((int)PacketTypes.WorldInfo, args.Player.Index);

					var group = TShock.Groups.GetGroupByName(account.Group);

					if (!TShock.Groups.AssertGroupValid(args.Player, group, true))
						return true;

					args.Player.PlayerData = TShock.CharacterDB.GetPlayerData(args.Player, account.ID);

					args.Player.Group = group;
					args.Player.tempGroup = null;
					args.Player.Account = account;
					args.Player.IsLoggedIn = true;
					args.Player.IsDisabledForSSC = false;

					if (Main.ServerSideCharacter)
					{
						if (args.Player.HasPermission(Permissions.bypassssc))
						{
							if (args.Player.PlayerData.exists && TShock.ServerSideCharacterConfig.Settings.WarnPlayersAboutBypassPermission)
							{
								args.Player.SendWarningMessage(GetString("Bypass SSC is enabled for your account. SSC data will not be loaded or saved."));
								TShock.Log.ConsoleInfo(GetString($"{args.Player.Name} has SSC data in the database, but has the tshock.ignore.ssc permission. This means their SSC data is being ignored."));
								TShock.Log.ConsoleInfo(GetString("You may wish to consider removing the tshock.ignore.ssc permission or negating it for this player."));
							}
							args.Player.PlayerData.CopyCharacter(args.Player);
							TShock.CharacterDB.InsertPlayerData(args.Player);
						}
						args.Player.PlayerData.RestoreCharacter(args.Player);
					}
					args.Player.LoginFailsBySsi = false;

					if (args.Player.HasPermission(Permissions.ignorestackhackdetection))
						args.Player.IsDisabledForStackDetection = false;

					if (args.Player.HasPermission(Permissions.usebanneditem))
						args.Player.IsDisabledForBannedWearable = false;

					args.Player.SendSuccessMessage(GetString($"Authenticated as {account.Name} successfully."));
					TShock.Log.ConsoleInfo(GetString($"{args.Player.Name} authenticated successfully as user {args.Player.Name}."));
					Hooks.PlayerHooks.OnPlayerPostLogin(args.Player);
					return true;
				}
			}
			else if (account != null && !TShock.Config.Settings.DisableLoginBeforeJoin)
			{
				args.Player.RequiresPassword = true;
				NetMessage.SendData((int)PacketTypes.PasswordRequired, args.Player.Index);
				return true;
			}
			else if (!string.IsNullOrEmpty(TShock.Config.Settings.ServerPassword))
			{
				args.Player.RequiresPassword = true;
				NetMessage.SendData((int)PacketTypes.PasswordRequired, args.Player.Index);
				return true;
			}

			if (args.Player.State == 1)
				args.Player.State = 2;
			NetMessage.SendData((int)PacketTypes.WorldInfo, args.Player.Index);
			return true;
		}

		private static bool HandleGetSection(GetDataHandlerArgs args)
		{
			if (OnGetSection(args.Player, args.Data, args.Data.ReadInt32(), args.Data.ReadInt32()))
				return true;

			if (TShock.Utils.GetActivePlayerCount() + 1 > TShock.Config.Settings.MaxSlots &&
				!args.Player.HasPermission(Permissions.reservedslot))
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleGetSection rejected reserve slot"));
				args.Player.Kick(TShock.Config.Settings.ServerFullReason, true, true);
				return true;
			}

			NetMessage.SendData((int)PacketTypes.TimeSet, -1, -1, NetworkText.Empty, Main.dayTime ? 1 : 0, (int)Main.time, Main.sunModY, Main.moonModY);
			return false;
		}

		private static bool HandleSpawn(GetDataHandlerArgs args)
		{
			if (args.Player.Dead && args.Player.RespawnTimer > 0)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleSpawn rejected dead player spawn request {0}", args.Player.Name));
				return true;
			}

			byte player = args.Data.ReadInt8();
			short spawnx = args.Data.ReadInt16();
			short spawny = args.Data.ReadInt16();
			int respawnTimer = args.Data.ReadInt32();
			short numberOfDeathsPVE = args.Data.ReadInt16();
			short numberOfDeathsPVP = args.Data.ReadInt16();
			PlayerSpawnContext context = (PlayerSpawnContext)args.Data.ReadByte();

			if (OnPlayerSpawn(args.Player, args.Data, player, spawnx, spawny, respawnTimer, numberOfDeathsPVE, numberOfDeathsPVP, context))
				return true;

			if ((Main.ServerSideCharacter) && (spawnx == -1 && spawny == -1)) //this means they want to spawn to vanilla spawn
			{
				args.Player.sX = Main.spawnTileX;
				args.Player.sY = Main.spawnTileY;
				args.Player.Teleport(args.Player.sX * 16, (args.Player.sY * 16) - 48);
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleSpawn force teleport 'vanilla spawn' {0}", args.Player.Name));
			}

			else if ((Main.ServerSideCharacter) && (args.Player.sX > 0) && (args.Player.sY > 0) && (args.TPlayer.SpawnX > 0) && ((args.TPlayer.SpawnX != args.Player.sX) && (args.TPlayer.SpawnY != args.Player.sY)))
			{
				args.Player.sX = args.TPlayer.SpawnX;
				args.Player.sY = args.TPlayer.SpawnY;

				if (((Main.tile[args.Player.sX, args.Player.sY - 1].active() && Main.tile[args.Player.sX, args.Player.sY - 1].type == TileID.Beds)) && (WorldGen.StartRoomCheck(args.Player.sX, args.Player.sY - 1)))
				{
					args.Player.Teleport(args.Player.sX * 16, (args.Player.sY * 16) - 48);
					TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleSpawn force teleport phase 1 {0}", args.Player.Name));
				}
			}

			else if ((Main.ServerSideCharacter) && (args.Player.sX > 0) && (args.Player.sY > 0))
			{
				if (((Main.tile[args.Player.sX, args.Player.sY - 1].active() && Main.tile[args.Player.sX, args.Player.sY - 1].type == TileID.Beds)) && (WorldGen.StartRoomCheck(args.Player.sX, args.Player.sY - 1)))
				{
					args.Player.Teleport(args.Player.sX * 16, (args.Player.sY * 16) - 48);
					TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleSpawn force teleport phase 2 {0}", args.Player.Name));
				}
			}

			if (respawnTimer > 0)
				args.Player.Dead = true;
			else
				args.Player.Dead = false;
			return false;
		}

		private static bool HandlePlayerUpdate(GetDataHandlerArgs args)
		{
			if (args.Player == null || args.TPlayer == null || args.Data == null)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / OnPlayerUpdate rejected from null player."));
				return true;
			}

			byte playerID = args.Data.ReadInt8();
			ControlSet controls = new ControlSet((BitsByte)args.Data.ReadByte());
			MiscDataSet1 miscData1 = new MiscDataSet1((BitsByte)args.Data.ReadByte());
			MiscDataSet2 miscData2 = new MiscDataSet2((BitsByte)args.Data.ReadByte());
			MiscDataSet3 miscData3 = new MiscDataSet3((BitsByte)args.Data.ReadByte());
			byte selectedItem = args.Data.ReadInt8();
			Vector2 position = args.Data.ReadVector2();

			Vector2 velocity = Vector2.Zero;
			if (miscData1.HasVelocity)
				velocity = args.Data.ReadVector2();

			Vector2? originalPosition = new Vector2?();
			Vector2? homePosition = Vector2.Zero;
			if (miscData2.CanReturnWithPotionOfReturn)
			{
				originalPosition = new Vector2?(args.Data.ReadVector2());
				homePosition = new Vector2?(args.Data.ReadVector2());
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandlePlayerUpdate home position delta {0}", args.Player.Name));
			}

			if (OnPlayerUpdate(args.Player, args.Data, playerID, controls, miscData1, miscData2, miscData3, selectedItem, position, velocity, originalPosition, homePosition))
				return true;

			return false;
		}

		private static bool HandlePlayerHp(GetDataHandlerArgs args)
		{
			var plr = args.Data.ReadInt8();
			var cur = args.Data.ReadInt16();
			var max = args.Data.ReadInt16();

			if (OnPlayerHP(args.Player, args.Data, plr, cur, max) || cur <= 0 || max <= 0 || args.Player.IgnoreSSCPackets)
				return true;

			if (max > TShock.Config.Settings.MaxHP && !args.Player.HasPermission(Permissions.ignorehp))
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandlePlayerHp rejected over max hp {0}", args.Player.Name));
				args.Player.Disable("Maximum HP beyond limit", DisableFlags.WriteToLogAndConsole);
				return true;
			}

			if (args.Player.IsLoggedIn)
			{
				args.Player.TPlayer.statLife = cur;
				args.Player.TPlayer.statLifeMax = max;
				args.Player.PlayerData.maxHealth = max;
			}

			return false;
		}

		private static bool HandleTile(GetDataHandlerArgs args)
		{
			EditAction action = (EditAction)args.Data.ReadInt8();
			short tileX = args.Data.ReadInt16();
			short tileY = args.Data.ReadInt16();
			short editData = args.Data.ReadInt16();
			EditType type = (action == EditAction.KillTile || action == EditAction.KillWall ||
							 action == EditAction.KillTileNoItem || action == EditAction.TryKillTile)
				? EditType.Fail
				: (action == EditAction.PlaceTile || action == EditAction.PlaceWall || action == EditAction.ReplaceTile || action == EditAction.ReplaceWall)
					? EditType.Type
					: EditType.Slope;

			byte style = args.Data.ReadInt8();

			if (OnTileEdit(args.Player, args.Data, tileX, tileY, action, type, editData, style))
				return true;

			return false;
		}

		private static bool HandleDoorUse(GetDataHandlerArgs args)
		{
			byte action = (byte)args.Data.ReadByte();
			short x = args.Data.ReadInt16();
			short y = args.Data.ReadInt16();
			byte direction = (byte)args.Data.ReadByte();

			DoorAction doorAction = (DoorAction)action;

			if (OnDoorUse(args.Player, args.Data, x, y, direction, doorAction))
				return true;

			ushort tileType = Main.tile[x, y].type;

			if (x >= Main.maxTilesX || y >= Main.maxTilesY || x < 0 || y < 0) // Check for out of range
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleDoorUse rejected out of range door {0}", args.Player.Name));
				return true;
			}

			if (action < 0 || action > 5)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleDoorUse rejected type 0 5 check {0}", args.Player.Name));
				return true;
			}


			if (tileType != TileID.ClosedDoor && tileType != TileID.OpenDoor
											  && tileType != TileID.TallGateClosed && tileType != TileID.TallGateOpen
											  && tileType != TileID.TrapdoorClosed && tileType != TileID.TrapdoorOpen)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleDoorUse rejected door gap check {0}", args.Player.Name));
				return true;
			}

			return false;
		}

		private static bool HandleSendTileRect(GetDataHandlerArgs args)
		{
			var player = args.Player;

			var tileX = args.Data.ReadInt16();
			var tileY = args.Data.ReadInt16();
			var width = (byte)args.Data.ReadByte();
			var length = (byte)args.Data.ReadByte();

			var changeByte = (byte)args.Data.ReadByte();
			var changeType = TileChangeType.None;
			if (Enum.IsDefined(typeof(TileChangeType), changeByte))
			{
				changeType = (TileChangeType)changeByte;
			}

			var data = args.Data;

			if (OnSendTileRect(player, data, tileX, tileY, width, length, changeType))
				return true;

			return false;
		}

		private static bool HandleItemDrop(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt16();
			var pos = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());
			var vel = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());
			var stacks = args.Data.ReadInt16();
			var prefix = args.Data.ReadInt8();
			var noDelay = args.Data.ReadInt8() == 1;
			var type = args.Data.ReadInt16();

			if (OnItemDrop(args.Player, args.Data, id, pos, vel, stacks, prefix, noDelay, type))
				return true;

			return false;
		}

		private static bool HandleItemOwner(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt16();
			var owner = args.Data.ReadInt8();

			if (id < 0 || id > 400)
				return true;

			if (id == 400 && owner == 255)
			{
				args.Player.IgnoreSSCPackets = false;
				return true;
			}

			return false;
		}

		private static bool HandleProjectileNew(GetDataHandlerArgs args)
		{
			short ident = args.Data.ReadInt16();
			Vector2 pos = args.Data.ReadVector2();
			Vector2 vel = args.Data.ReadVector2();
			byte owner = args.Data.ReadInt8();
			short type = args.Data.ReadInt16();
			BitsByte bitsByte = (BitsByte)args.Data.ReadByte();
			BitsByte bitsByte2 = (BitsByte)(bitsByte[2] ? args.Data.ReadByte() : 0);
			float[] ai = new float[Projectile.maxAI];
			for (int i = 0; i < Projectile.maxAI; ++i) ai[i] = 0f;
			ai[0] = bitsByte[0] ? args.Data.ReadSingle() : 0f;
			ai[1] = bitsByte[1] ? args.Data.ReadSingle() : 0f;
			ushort bannerId = (ushort)(bitsByte[3] ? args.Data.ReadUInt16() : 0);
			short dmg = (short)(bitsByte[4] ? args.Data.ReadInt16() : 0);
			float knockback = bitsByte[5] ? args.Data.ReadSingle() : 0f;
			short origDmg = (short)(bitsByte[6] ? args.Data.ReadInt16() : 0);
			short projUUID = (short)(bitsByte[7] ? args.Data.ReadInt16() : -1);
			if (projUUID >= 1000) projUUID = -1;
			ai[2] = (bitsByte2[0] ? args.Data.ReadSingle() : 0f);

			var index = TShock.Utils.SearchProjectile(ident, owner);

			if (OnNewProjectile(args.Data, ident, pos, vel, knockback, dmg, owner, type, index, args.Player, ai))
				return true;

			lock (args.Player.RecentlyCreatedProjectiles)
			{
				if (!args.Player.RecentlyCreatedProjectiles.Any(p => p.Index == index))
				{
					args.Player.RecentlyCreatedProjectiles.Add(new GetDataHandlers.ProjectileStruct()
					{
						Index = index,
						Type = type,
						CreatedAt = DateTime.Now
					});
				}
			}
			return false;
		}

		private static bool HandleNpcStrike(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt16();
			var dmg = args.Data.ReadInt16();
			var knockback = args.Data.ReadSingle();
			var direction = (byte)(args.Data.ReadInt8() - 1);
			var crit = args.Data.ReadInt8();

			if (OnNPCStrike(args.Player, args.Data, id, direction, dmg, knockback, crit))
				return true;

			if (Main.npc[id].townNPC && !args.Player.HasPermission(Permissions.hurttownnpc))
			{
				args.Player.SendErrorMessage(GetString("You do not have permission to hurt Town NPCs."));
				args.Player.SendData(PacketTypes.NpcUpdate, "", id);
				TShock.Log.ConsoleDebug(GetString($"GetDataHandlers / HandleNpcStrike rejected npc strike {args.Player.Name}"));
				return true;
			}

			if (Main.npc[id].netID == NPCID.EmpressButterfly)
			{
				if (!args.Player.HasPermission(Permissions.summonboss))
				{
					args.Player.SendErrorMessage(GetString("You do not have permission to summon the Empress of Light."));
					args.Player.SendData(PacketTypes.NpcUpdate, "", id);
					TShock.Log.ConsoleDebug(GetString($"GetDataHandlers / HandleNpcStrike rejected EoL summon from {args.Player.Name}"));
					return true;
				}
				else if (!TShock.Config.Settings.AnonymousBossInvasions)
				{
					TShock.Utils.Broadcast(GetString($"{args.Player.Name} summoned the Empress of Light!"), 175, 75, 255);
				}
				else
					TShock.Utils.SendLogs(GetString($"{args.Player.Name} summoned the Empress of Light!"), Color.PaleVioletRed, args.Player);
			}

			if (Main.npc[id].netID == NPCID.CultistDevote || Main.npc[id].netID == NPCID.CultistArcherBlue)
			{
				if (!args.Player.HasPermission(Permissions.summonboss))
				{
					args.Player.SendErrorMessage(GetString("You do not have permission to summon the Lunatic Cultist!"));
					args.Player.SendData(PacketTypes.NpcUpdate, "", id);
					TShock.Log.ConsoleDebug(GetString($"GetDataHandlers / HandleNpcStrike rejected Cultist summon from {args.Player.Name}"));
					return true;
				}
			}
			return false;
		}

		private static bool HandleProjectileKill(GetDataHandlerArgs args)
		{
			var ident = args.Data.ReadInt16();
			var owner = args.Data.ReadInt8();
			owner = (byte)args.Player.Index;
			var index = TShock.Utils.SearchProjectile(ident, owner);

			if (OnProjectileKill(args.Player, args.Data, ident, owner, index))
			{
				return true;
			}

			short type = (short)Main.projectile[index].type;

			// TODO: This needs to be moved somewhere else.

			if (type == ProjectileID.Tombstone)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleProjectileKill rejected tombstone {0}", args.Player.Name));
				args.Player.RemoveProjectile(ident, owner);
				return true;
			}

			if (TShock.ProjectileBans.ProjectileIsBanned(type, args.Player) && !TShock.Config.Settings.IgnoreProjKill)
			{
				// According to 2012 deathmax, this is a workaround to fix skeletron prime issues
				// https://github.com/Pryaxis/TShock/commit/a5aa9231239926f361b7246651e32144bbf28dda
				if (type == ProjectileID.Bomb || type == ProjectileID.DeathLaser)
				{
					TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleProjectileKill permitted skeletron prime exemption {0}", args.Player.Name));
					TShock.Log.ConsoleDebug(GetString("If this was not skeletron prime related, please report to TShock what happened."));
					return false;
				}
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleProjectileKill rejected banned projectile {0}", args.Player.Name));
				args.Player.RemoveProjectile(ident, owner);
				return true;
			}

			args.Player.LastKilledProjectile = type;
			lock (args.Player.RecentlyCreatedProjectiles)
			{
				args.Player.RecentlyCreatedProjectiles.ForEach(s => { if (s.Index == index) { s.Killed = true; } });
			}

			return false;
		}

		private static bool HandleTogglePvp(GetDataHandlerArgs args)
		{
			byte id = args.Data.ReadInt8();
			bool pvp = args.Data.ReadBoolean();
			if (OnPvpToggled(args.Player, args.Data, id, pvp))
				return true;

			if (id != args.Player.Index)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleTogglePvp rejected index mismatch {0}", args.Player.Name));
				return true;
			}

			string pvpMode = TShock.Config.Settings.PvPMode.ToLowerInvariant();
			if (pvpMode == "disabled" || pvpMode == "always" || pvpMode == "pvpwithnoteam" || (DateTime.UtcNow - args.Player.LastPvPTeamChange).TotalSeconds < 5)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleTogglePvp rejected fastswitch {0}", args.Player.Name));
				args.Player.SendData(PacketTypes.TogglePvp, "", id);
				return true;
			}

			args.Player.LastPvPTeamChange = DateTime.UtcNow;
			return false;
		}

		private static bool HandleChestOpen(GetDataHandlerArgs args)
		{
			var x = args.Data.ReadInt16();
			var y = args.Data.ReadInt16();

			if (OnChestOpen(args.Data, x, y, args.Player))
				return true;

			return false;
		}

		private static bool HandleChestItem(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt16();
			var slot = args.Data.ReadInt8();
			var stacks = args.Data.ReadInt16();
			var prefix = args.Data.ReadInt8();
			var type = args.Data.ReadInt16();

			if (OnChestItemChange(args.Player, args.Data, id, slot, stacks, prefix, type))
				return true;

			Item item = new Item();
			item.netDefaults(type);
			if (stacks > item.maxStack)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleChestItem rejected max stacks {0}", args.Player.Name));
				return true;
			}

			return false;
		}

		private static bool HandleChestActive(GetDataHandlerArgs args)
		{
			//chest ID
			var id = args.Data.ReadInt16();
			//chest x
			var x = args.Data.ReadInt16();
			//chest y
			var y = args.Data.ReadInt16();
			//chest name length
			var nameLen = args.Data.ReadInt8();

			if (nameLen != 0 && nameLen <= 20)
				args.Data.ReadString(); // Ignore the name

			args.Player.ActiveChest = id;

			if (!args.Player.HasBuildPermission(x, y) && TShock.Config.Settings.RegionProtectChests)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleChestActive rejected build permission and region check {0}", args.Player.Name));
				args.Player.SendData(PacketTypes.ChestOpen, "", -1);
				return true;
			}

			return false;
		}

		private static bool HandlePlaceChest(GetDataHandlerArgs args)
		{
			int flag = args.Data.ReadByte();
			int tileX = args.Data.ReadInt16();
			int tileY = args.Data.ReadInt16();
			short style = args.Data.ReadInt16();

			if (OnPlaceChest(args.Player, args.Data, flag, tileX, tileY, style))
				return true;

			return false;
		}

		private static bool HandlePlayerZone(GetDataHandlerArgs args)
		{
			if (args.Player == null || args.TPlayer == null || args.Data == null)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandlePlayerZone rejected null check"));
				return true;
			}

			var plr = args.Data.ReadInt8();
			BitsByte zone1 = args.Data.ReadInt8();
			BitsByte zone2 = args.Data.ReadInt8();
			BitsByte zone3 = args.Data.ReadInt8();
			BitsByte zone4 = args.Data.ReadInt8();
			BitsByte zone5 = args.Data.ReadInt8();

			if (OnPlayerZone(args.Player, args.Data, plr, zone1, zone2, zone3, zone4, zone5))
				return true;

			return false;
		}

		private static bool HandlePassword(GetDataHandlerArgs args)
		{
			if (!args.Player.RequiresPassword)
				return true;

			string password = args.Data.ReadString();

			if (Hooks.PlayerHooks.OnPlayerPreLogin(args.Player, args.Player.Name, password))
				return true;

			var account = TShock.UserAccounts.GetUserAccountByName(args.Player.Name);
			if (account != null && !TShock.Config.Settings.DisableLoginBeforeJoin)
			{
				if (account.VerifyPassword(password))
				{
					args.Player.RequiresPassword = false;
					args.Player.PlayerData = TShock.CharacterDB.GetPlayerData(args.Player, account.ID);

					if (args.Player.State == 1)
						args.Player.State = 2;
					NetMessage.SendData((int)PacketTypes.WorldInfo, args.Player.Index);

					var group = TShock.Groups.GetGroupByName(account.Group);

					if (!TShock.Groups.AssertGroupValid(args.Player, group, true))
						return true;

					args.Player.Group = group;
					args.Player.tempGroup = null;
					args.Player.Account = account;
					args.Player.IsLoggedIn = true;
					args.Player.IsDisabledForSSC = false;

					if (Main.ServerSideCharacter)
					{
						if (args.Player.HasPermission(Permissions.bypassssc))
						{
							args.Player.PlayerData.CopyCharacter(args.Player);
							TShock.CharacterDB.InsertPlayerData(args.Player);
						}
						args.Player.PlayerData.RestoreCharacter(args.Player);
					}
					args.Player.LoginFailsBySsi = false;

					if (args.Player.HasPermission(Permissions.ignorestackhackdetection))
						args.Player.IsDisabledForStackDetection = false;

					if (args.Player.HasPermission(Permissions.usebanneditem))
						args.Player.IsDisabledForBannedWearable = false;


					args.Player.SendMessage(GetString($"Authenticated as {args.Player.Name} successfully."), Color.LimeGreen);
					TShock.Log.ConsoleInfo(GetString($"{args.Player.Name} authenticated successfully as user {args.Player.Name}."));
					TShock.UserAccounts.SetUserAccountUUID(account, args.Player.UUID);
					Hooks.PlayerHooks.OnPlayerPostLogin(args.Player);
					return true;
				}
				args.Player.Kick(GetString("Your password did not match this character's password."), true, true);
				return true;
			}

			if (!string.IsNullOrEmpty(TShock.Config.Settings.ServerPassword))
			{
				if (TShock.Config.Settings.ServerPassword == password)
				{
					args.Player.RequiresPassword = false;
					if (args.Player.State == 1)
						args.Player.State = 2;
					NetMessage.SendData((int)PacketTypes.WorldInfo, args.Player.Index);
					return true;
				}
				args.Player.Kick(GetString("Invalid server password."), true, true);
				return true;
			}

			args.Player.Kick(GetParticularString("Likely non-vanilla client send zero-length password", "You have been Bounced for invalid password."), true, true);
			return true;
		}

		private static bool HandleNpcTalk(GetDataHandlerArgs args)
		{
			var plr = args.Data.ReadInt8();
			var npc = args.Data.ReadInt16();

			if (OnNpcTalk(args.Player, args.Data, plr, npc))
				return true;

			//Rejecting player who trying to talk to a npc if player were disabled, mainly for unregistered and logged out players. Preventing smuggling or duplicating their items if player put it in a npc's item slot
			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleNpcTalk rejected npc talk {0}", args.Player.Name));
				args.Player.SendData(PacketTypes.NpcTalk, "", plr, -1);
				return true;
			}

			if (args.Player.IsBouncerThrottled())
			{
				TShock.Log.ConsoleDebug(GetString("Bouncer / HandleNpcTalk rejected from bouncer throttle from {0}", args.Player.Name));
				return true;
			}

			// -1 is a magic value, represents not talking to an NPC
			if (npc < -1 || npc >= Main.maxNPCs)
			{
				TShock.Log.ConsoleDebug(GetString("Bouncer / HandleNpcTalk rejected from bouncer out of bounds from {0}", args.Player.Name));
				return true;
			}
			return false;
		}

		private static bool HandlePlayerAnimation(GetDataHandlerArgs args)
		{
			if (OnPlayerAnimation(args.Player, args.Data))
				return true;

			return false;
		}

		private static bool HandlePlayerMana(GetDataHandlerArgs args)
		{
			var plr = args.Data.ReadInt8();
			var cur = args.Data.ReadInt16();
			var max = args.Data.ReadInt16();

			if (OnPlayerMana(args.Player, args.Data, plr, cur, max) || cur < 0 || max < 0 || args.Player.IgnoreSSCPackets)
				return true;

			if (max > TShock.Config.Settings.MaxMP && !args.Player.HasPermission(Permissions.ignoremp))
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandlePlayerMana rejected max mana {0} {1}/{2}", args.Player.Name, max, TShock.Config.Settings.MaxMP));
				args.Player.Disable("Maximum MP beyond limit", DisableFlags.WriteToLogAndConsole);
				return true;
			}

			if (args.Player.IsLoggedIn)
			{
				args.Player.TPlayer.statMana = cur;
				args.Player.TPlayer.statManaMax = max;
				args.Player.PlayerData.maxMana = max;
			}
			return false;
		}

		private static bool HandlePlayerTeam(GetDataHandlerArgs args)
		{
			byte id = args.Data.ReadInt8();
			byte team = args.Data.ReadInt8();
			if (OnPlayerTeam(args.Player, args.Data, id, team))
				return true;

			if (id != args.Player.Index)
				return true;

			string pvpMode = TShock.Config.Settings.PvPMode.ToLowerInvariant();
			if (pvpMode == "pvpwithnoteam" || (DateTime.UtcNow - args.Player.LastPvPTeamChange).TotalSeconds < 5)
			{
				args.Player.SendData(PacketTypes.PlayerTeam, "", id);
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandlePlayerTeam rejected team fastswitch {0}", args.Player.Name));
				return true;
			}

			args.Player.LastPvPTeamChange = DateTime.UtcNow;
			return false;
		}

		private static bool HandleSignRead(GetDataHandlerArgs args)
		{
			var x = args.Data.ReadInt16();
			var y = args.Data.ReadInt16();

			if (OnSignRead(args.Player, args.Data, x, y))
				return true;

			if (x < 0 || y < 0 || x >= Main.maxTilesX || y >= Main.maxTilesY)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleSignRead rejected out of bounds {0}", args.Player.Name));
				return true;
			}

			return false;
		}

		private static bool HandleSign(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt16();
			var x = args.Data.ReadInt16();
			var y = args.Data.ReadInt16();
			args.Data.ReadString(); // Ignore sign text

			if (OnSignEvent(args.Player, args.Data, id, x, y))
				return true;

			if (!args.Player.HasBuildPermission(x, y))
			{
				args.Player.SendData(PacketTypes.SignNew, "", id);
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleSign rejected sign on build permission {0}", args.Player.Name));
				return true;
			}

			if (!args.Player.IsInRange(x, y))
			{
				args.Player.SendData(PacketTypes.SignNew, "", id);
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleSign rejected sign range check {0}", args.Player.Name));
				return true;
			}
			return false;
		}

		private static bool HandleLiquidSet(GetDataHandlerArgs args)
		{
			int tileX = args.Data.ReadInt16();
			int tileY = args.Data.ReadInt16();
			byte amount = args.Data.ReadInt8();
			byte type = args.Data.ReadInt8();

			if (OnLiquidSet(args.Player, args.Data, tileX, tileY, amount, type))
				return true;

			return false;
		}

		private static bool HandlePlayerBuffList(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt8();

			if (OnPlayerBuffUpdate(args.Player, args.Data, id))
				return true;

			for (int i = 0; i < Terraria.Player.maxBuffs; i++)
			{
				var buff = args.Data.ReadUInt16();

				if (buff == 10 && TShock.Config.Settings.DisableInvisPvP && args.TPlayer.hostile)
					buff = 0;

				if (Netplay.Clients[args.TPlayer.whoAmI].State < 2 && (buff == 156 || buff == 47 || buff == 149))
				{
					TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandlePlayerBuffList zeroed player buff due to below state 2 {0} {1}", args.Player.Name, buff));
					buff = 0;
				}

				args.TPlayer.buffType[i] = buff;
				if (args.TPlayer.buffType[i] > 0)
				{
					args.TPlayer.buffTime[i] = 60;
				}
				else
				{
					args.TPlayer.buffTime[i] = 0;
				}
			}

			TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandlePlayerBuffList handled event and sent data {0}", args.Player.Name));
			NetMessage.SendData((int)PacketTypes.PlayerBuff, -1, args.Player.Index, NetworkText.Empty, args.Player.Index);
			return true;
		}

		private static bool HandleSpecial(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt8();
			var type = args.Data.ReadInt8();

			if (OnNPCSpecial(args.Player, args.Data, id, type))
				return true;

			if (type == 1 && TShock.Config.Settings.DisableDungeonGuardian)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleSpecial rejected type 1 for {0}", args.Player.Name));
				args.Player.SendMessage(GetString("The Dungeon Guardian returned you to your spawn point."), Color.Purple);
				args.Player.Spawn(PlayerSpawnContext.RecallFromItem);
				return true;
			}

			if (type == 3)
			{
				if (!args.Player.HasPermission(Permissions.usesundial))
				{
					TShock.Log.ConsoleDebug(GetString($"GetDataHandlers / HandleSpecial rejected enchanted sundial permission {args.Player.Name}"));
					args.Player.SendErrorMessage(GetString("You do not have permission to use the Enchanted Sundial."));
					return true;
				}
				else if (TShock.Config.Settings.ForceTime != "normal")
				{
					TShock.Log.ConsoleDebug(GetString($"GetDataHandlers / HandleSpecial rejected enchanted sundial permission (ForceTime) {args.Player.Name}"));
					if (!args.Player.HasPermission(Permissions.cfgreload))
					{
						args.Player.SendErrorMessage(GetString("You cannot use the Enchanted Sundial because time is stopped."));
					}
					else
						args.Player.SendErrorMessage(GetString("You must set ForceTime to normal via config to use the Enchanted Sundial."));
					return true;
				}
			}

			return false;
		}

		private static bool HandleNPCAddBuff(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt16();
			var type = args.Data.ReadUInt16();
			var time = args.Data.ReadInt16();

			if (OnNPCAddBuff(args.Player, args.Data, id, type, time))
				return true;

			return false;
		}

		private static bool HandlePlayerAddBuff(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt8();
			var type = args.Data.ReadUInt16();
			var time = args.Data.ReadInt32();

			if (OnPlayerBuff(args.Player, args.Data, id, type, time))
				return true;

			return false;
		}

		private static bool HandleUpdateNPCHome(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt16();
			var x = args.Data.ReadInt16();
			var y = args.Data.ReadInt16();
			var householdStatus = args.Data.ReadInt8();

			if (OnUpdateNPCHome(args.Player, args.Data, id, x, y, householdStatus))
				return true;

			if (!args.Player.HasPermission(Permissions.movenpc))
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / UpdateNPCHome rejected no permission {0}", args.Player.Name));
				args.Player.SendErrorMessage(GetString("You do not have permission to relocate Town NPCs."));
				args.Player.SendData(PacketTypes.UpdateNPCHome, "", id, Main.npc[id].homeTileX, Main.npc[id].homeTileY,
					Convert.ToByte(Main.npc[id].homeless));
				return true;
			}
			return false;
		}

		private static readonly int[] invasions = { -1, -2, -3, -4, -5, -6, -7, -8, -10, -11 };
		private static readonly int[] pets = { -12, -13, -14, -15 };
		private static bool HandleSpawnBoss(GetDataHandlerArgs args)
		{
			if (args.Player.IsBouncerThrottled())
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleSpawnBoss rejected bouner throttled {0}", args.Player.Name));
				return true;
			}

			var plr = args.Data.ReadInt16();
			var thingType = args.Data.ReadInt16();

			var isKnownBoss = thingType > 0 && thingType < Terraria.ID.NPCID.Count && NPCID.Sets.MPAllowedEnemies[thingType];
			if ((isKnownBoss || thingType == -16) && !args.Player.HasPermission(Permissions.summonboss))
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleSpawnBoss rejected boss {0} {1}", args.Player.Name, thingType));
				args.Player.SendErrorMessage(GetString("You do not have permission to summon bosses."));
				return true;
			}

			if (invasions.Contains(thingType) && !args.Player.HasPermission(Permissions.startinvasion))
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleSpawnBoss rejected invasion {0} {1}", args.Player.Name, thingType));
				args.Player.SendErrorMessage(GetString("You do not have permission to start invasions."));
				return true;
			}

			if (pets.Contains(thingType) && !args.Player.HasPermission(Permissions.spawnpets))
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleSpawnBoss rejected pet {0} {1}", args.Player.Name, thingType));
				args.Player.SendErrorMessage(GetString("You do not have permission to spawn pets."));
				return true;
			}

			if (plr != args.Player.Index)
				return true;

			string thing;
			switch (thingType)
			{
				case -18:
					thing = GetString("{0} applied traveling merchant's satchel!", args.Player.Name);
					break;
				case -17:
					thing = GetString("{0} applied advanced combat techniques volume 2!", args.Player.Name);
					break;
				case -16:
					thing = GetString("{0} summoned a Mechdusa!", args.Player.Name);
					break;
				case -15:
					thing = GetString("{0} has sent a request to the slime delivery service!", args.Player.Name);
					break;
				case -14:
					thing = GetString("{0} has sent a request to the bunny delivery service!", args.Player.Name);
					break;
				case -13:
					thing = GetString("{0} has sent a request to the dog delivery service!", args.Player.Name);
					break;
				case -12:
					thing = GetString("{0} has sent a request to the cat delivery service!", args.Player.Name);
					break;
				case -11:
					thing = GetString("{0} applied advanced combat techniques!", args.Player.Name);
					break;
				case -10:
					thing = GetString("{0} summoned a Blood Moon!", args.Player.Name);
					break;
				case -8:
					thing = GetString("{0} summoned a Moon Lord!", args.Player.Name);
					break;
				case -7:
					thing = GetString("{0} summoned a Martian invasion!", args.Player.Name);
					break;
				case -6:
					thing = GetString("{0} summoned an eclipse!", args.Player.Name);
					break;
				case -5:
					thing = GetString("{0} summoned a frost moon!", args.Player.Name);
					break;
				case -4:
					thing = GetString("{0} summoned a pumpkin moon!", args.Player.Name);
					break;
				case -3:
					thing = GetString("{0} summoned the Pirates!", args.Player.Name);
					break;
				case -2:
					thing = GetString("{0} summoned the Snow Legion!", args.Player.Name);
					break;
				case -1:
					thing = GetString("{0} summoned a Goblin Invasion!", args.Player.Name);
					break;
				default:
					if (!isKnownBoss)
						TShock.Log.ConsoleDebug("GetDataHandlers / HandleSpawnBoss unknown boss {0} summoned by {1}", thingType, args.Player.Name);
					NPC npc = new NPC();
					npc.SetDefaults(thingType);
					thing = GetString("{0} summoned the {1}!", args.Player.Name, npc.FullName);
					break;
			}
			if (TShock.Config.Settings.AnonymousBossInvasions)
				TShock.Utils.SendLogs(thing, Color.PaleVioletRed, args.Player);
			else
				TShock.Utils.Broadcast(thing, 175, 75, 255);
			return false;
		}

		private static bool HasPaintSprayerAbilities(Item item)
			=> item is not null && item.stack > 0 && (
			   item.type == ItemID.PaintSprayer ||
			   item.type == ItemID.ArchitectGizmoPack ||
			   item.type == ItemID.HandOfCreation);

		private static bool HandlePaintTile(GetDataHandlerArgs args)
		{
			var x = args.Data.ReadInt16();
			var y = args.Data.ReadInt16();
			var t = args.Data.ReadInt8();
			var ct = args.Data.ReadInt8();//PaintCoatTile

			if (x < 0 || y < 0 || x >= Main.maxTilesX || y >= Main.maxTilesY || t > Main.numTileColors)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandlePaintTile rejected range check {0}", args.Player.Name));
				return true;
			}
			if (OnPaintTile(args.Player, args.Data, x, y, t, ct))
			{
				return true;
			}

			// Not selecting paintbrush or paint scraper or the spectre versions? Hacking.
			if (args.Player.SelectedItem.type != ItemID.PaintRoller &&
				args.Player.SelectedItem.type != ItemID.PaintScraper &&
				args.Player.SelectedItem.type != ItemID.Paintbrush &&
				args.Player.SelectedItem.type != ItemID.SpectrePaintRoller &&
				args.Player.SelectedItem.type != ItemID.SpectrePaintScraper &&
				args.Player.SelectedItem.type != ItemID.SpectrePaintbrush &&
				!args.Player.Accessories.Any(HasPaintSprayerAbilities) &&
				!args.Player.Inventory.Any(HasPaintSprayerAbilities))
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandlePaintTile rejected select consistency {0}", args.Player.Name));
				args.Player.SendData(PacketTypes.PaintTile, "", x, y, Main.tile[x, y].color());
				return true;
			}

			if (args.Player.IsBouncerThrottled() ||
				!args.Player.HasPaintPermission(x, y) ||
				!args.Player.IsInRange(x, y))
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandlePaintTile rejected throttle/permission/range check {0}", args.Player.Name));
				args.Player.SendData(PacketTypes.PaintTile, "", x, y, Main.tile[x, y].color());
				return true;
			}

			if (!args.Player.HasPermission(Permissions.ignorepaintdetection))
			{
				args.Player.PaintThreshold++;
			}
			return false;
		}

		private static bool HandlePaintWall(GetDataHandlerArgs args)
		{
			var x = args.Data.ReadInt16();
			var y = args.Data.ReadInt16();
			var t = args.Data.ReadInt8();
			var cw = args.Data.ReadInt8();//PaintCoatWall

			if (x < 0 || y < 0 || x >= Main.maxTilesX || y >= Main.maxTilesY || t > Main.numTileColors)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandlePaintWall rejected range check {0}", args.Player.Name));
				return true;
			}
			if (OnPaintWall(args.Player, args.Data, x, y, t, cw))
			{
				return true;
			}

			// Not selecting paint roller or paint scraper or the spectre versions? Hacking.
			if (args.Player.SelectedItem.type != ItemID.PaintRoller &&
				args.Player.SelectedItem.type != ItemID.PaintScraper &&
				args.Player.SelectedItem.type != ItemID.Paintbrush &&
				args.Player.SelectedItem.type != ItemID.SpectrePaintRoller &&
				args.Player.SelectedItem.type != ItemID.SpectrePaintScraper &&
				args.Player.SelectedItem.type != ItemID.SpectrePaintbrush &&
				!args.Player.Accessories.Any(HasPaintSprayerAbilities) &&
				!args.Player.Inventory.Any(HasPaintSprayerAbilities))
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandlePaintWall rejected selector consistency {0}", args.Player.Name));
				args.Player.SendData(PacketTypes.PaintWall, "", x, y, Main.tile[x, y].wallColor());
				return true;
			}

			if (args.Player.IsBouncerThrottled() ||
				!args.Player.HasPaintPermission(x, y) ||
				!args.Player.IsInRange(x, y))
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandlePaintWall rejected throttle/permission/range {0}", args.Player.Name));
				args.Player.SendData(PacketTypes.PaintWall, "", x, y, Main.tile[x, y].wallColor());
				return true;
			}

			if (!args.Player.HasPermission(Permissions.ignorepaintdetection))
			{
				args.Player.PaintThreshold++;
			}
			return false;
		}

		private static bool HandleTeleport(GetDataHandlerArgs args)
		{
			BitsByte flag = (BitsByte)args.Data.ReadByte();
			short id = args.Data.ReadInt16();
			var x = args.Data.ReadSingle();
			var y = args.Data.ReadSingle();
			byte style = args.Data.ReadInt8();

			int type = 0;
			bool isNPC = type == 1;
			int extraInfo = -1;
			bool getPositionFromTarget = false;

			if (flag[0])
			{
				type = 1;
			}
			if (flag[1])
			{
				type = 2;
			}
			if (flag[2])
			{
				getPositionFromTarget = true;
			}
			if (flag[3])
			{
				extraInfo = args.Data.ReadInt32();
			}

			if (OnTeleport(args.Player, args.Data, id, flag, x, y, style, extraInfo))
				return true;

			//Rod of Discord teleport (usually (may be used by modded clients to teleport))
			if (type == 0 && !args.Player.HasPermission(Permissions.rod))
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleTeleport rejected rod type {0} {1}", args.Player.Name, type));
				args.Player.SendErrorMessage(GetString("You do not have permission to teleport using items.")); // Was going to write using RoD but Hook of Disonnance and Potion of Return both use the same teleport packet as RoD. 
				args.Player.Teleport(args.TPlayer.position.X, args.TPlayer.position.Y); // Suggest renaming rod permission unless someone plans to add separate perms for the other 2 tp items.
				return true;
			}

			//NPC teleport
			if (type == 1 && id >= Main.maxNPCs)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleTeleport rejected npc teleport {0} {1}", args.Player.Name, type));
				return true;
			}

			//Player to player teleport (wormhole potion, usually (may be used by modded clients to teleport))
			if (type == 2)
			{
				if (id >= Main.maxPlayers || Main.player[id] == null || TShock.Players[id] == null)
				{
					TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleTeleport rejected p2p extents {0} {1}", args.Player.Name, type));
					return true;
				}

				if (!args.Player.HasPermission(Permissions.wormhole))
				{
					TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleTeleport rejected p2p wormhole permission {0} {1}", args.Player.Name, type));
					args.Player.SendErrorMessage(GetString("You do not have permission to teleport using Wormhole Potions."));
					args.Player.Teleport(args.TPlayer.position.X, args.TPlayer.position.Y);
					return true;
				}
			}

			return false;
		}

		private static bool HandleHealOther(GetDataHandlerArgs args)
		{
			byte plr = args.Data.ReadInt8();
			short amount = args.Data.ReadInt16();

			if (OnHealOtherPlayer(args.Player, args.Data, plr, amount))
				return true;

			return false;
		}

		private static bool HandleCatchNpc(GetDataHandlerArgs args)
		{
			var npcID = args.Data.ReadInt16();
			var who = args.Data.ReadByte();

			if (Main.npc[npcID]?.catchItem == 0)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleCatchNpc catch zero {0}", args.Player.Name));
				Main.npc[npcID].active = true;
				NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, NetworkText.Empty, npcID);
				return true;
			}

			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleCatchNpc rejected catch npc {0}", args.Player.Name));
				return true;
			}

			return false;
		}

		private static bool HandleReleaseNpc(GetDataHandlerArgs args)
		{
			var x = args.Data.ReadInt32();
			var y = args.Data.ReadInt32();
			var type = args.Data.ReadInt16();
			var style = args.Data.ReadInt8();

			if (OnReleaseNpc(args.Player, args.Data, x, y, type, style))
			{
				return true;
			}

			return false;
		}

		private static bool HandleTeleportationPotion(GetDataHandlerArgs args)
		{
			var type = args.Data.ReadByte();

			void Fail(string tpItem)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleTeleportationPotion rejected permissions {0} {1}", args.Player.Name, type));
				args.Player.SendErrorMessage(GetString("You do not have permission to teleport using {0}.", tpItem));
			}

			switch (type)
			{
				case 0: // Teleportation Potion
					if (args.Player.ItemInHand.type != ItemID.TeleportationPotion &&
						args.Player.SelectedItem.type != ItemID.TeleportationPotion)
					{
						TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleTeleportationPotion rejected not holding the correct item {0} {1}", args.Player.Name, type));
						return true;
					}

					if (!args.Player.HasPermission(Permissions.tppotion))
					{
						Fail("Teleportation Potions");
						return true;
					}
					break;
				case 1: // Magic Conch or Shellphone (Ocean)
					if (args.Player.ItemInHand.type != ItemID.MagicConch &&
						args.Player.SelectedItem.type != ItemID.MagicConch &&
						args.Player.ItemInHand.type != ItemID.ShellphoneOcean &&
						args.Player.SelectedItem.type != ItemID.ShellphoneOcean)
					{
						TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleTeleportationPotion rejected not holding the correct item {0} {1}", args.Player.Name, type));
						return true;
					}

					if (!args.Player.HasPermission(Permissions.magicconch))
					{
						if (args.Player.ItemInHand.type == ItemID.ShellphoneOcean || args.Player.SelectedItem.type == ItemID.ShellphoneOcean)
						{
							Fail("the Shellphone (Ocean)");
						}
						else
						{
							Fail("the Magic Conch");
						}
						return true;
					}
					break;
				case 2: // Demon Conch or Shellphone (Underworld)
					if (args.Player.ItemInHand.type != ItemID.DemonConch &&
						args.Player.SelectedItem.type != ItemID.DemonConch &&
						args.Player.ItemInHand.type != ItemID.ShellphoneHell &&
						args.Player.SelectedItem.type != ItemID.ShellphoneHell)
					{
						TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleTeleportationPotion rejected not holding the correct item {0} {1}", args.Player.Name, type));
						return true;
					}

					if (!args.Player.HasPermission(Permissions.demonconch))
					{
						if (args.Player.ItemInHand.type == ItemID.ShellphoneHell || args.Player.SelectedItem.type == ItemID.ShellphoneHell)
						{
							Fail("the Shellphone (Underworld)");
						}
						else
						{
							Fail("the Demon Conch");
						}
						return true;
					}
					break;
				case 3: // Shellphone (Spawn)
					if (args.Player.ItemInHand.type != ItemID.ShellphoneSpawn && args.Player.SelectedItem.type != ItemID.ShellphoneSpawn)
					{
						TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleTeleportationPotion rejected not holding the correct item {0} {1}", args.Player.Name, type));
						return true;
					}
					break;
			}

			return false;
		}

		private static bool HandleCompleteAnglerQuest(GetDataHandlerArgs args)
		{
			// Since packet 76 is NEVER sent to us, we actually have to rely on this to get the true count
			args.TPlayer.anglerQuestsFinished++;
			return false;
		}

		private static bool HandleNumberOfAnglerQuestsCompleted(GetDataHandlerArgs args)
		{
			// Never sent by vanilla client, ignore this
			TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleNumberOfAnglerQuestsCompleted surprise packet! Someone tell the TShock team! {0}", args.Player.Name));
			return true;
		}

		private static bool HandlePlaceObject(GetDataHandlerArgs args)
		{
			short x = args.Data.ReadInt16();
			short y = args.Data.ReadInt16();
			short type = args.Data.ReadInt16();
			short style = args.Data.ReadInt16();
			byte alternate = args.Data.ReadInt8();
			sbyte random = (sbyte)args.Data.ReadInt8();
			bool direction = args.Data.ReadBoolean();

			if (OnPlaceObject(args.Player, args.Data, x, y, type, style, alternate, random, direction))
				return true;

			return false;
		}

		private static bool HandleLoadNetModule(GetDataHandlerArgs args)
		{
			short moduleId = args.Data.ReadInt16();

			if (OnReadNetModule(args.Player, args.Data, (NetModuleType)moduleId))
			{
				return true;
			}

			return false;
		}

		private static bool HandlePlaceTileEntity(GetDataHandlerArgs args)
		{
			var x = args.Data.ReadInt16();
			var y = args.Data.ReadInt16();
			var type = (byte)args.Data.ReadByte();

			if (OnPlaceTileEntity(args.Player, args.Data, x, y, type))
			{
				return true;
			}

			// ItemBan subsystem

			if (TShock.TileBans.TileIsBanned((short)TileID.LogicSensor, args.Player))
			{
				args.Player.SendTileSquareCentered(x, y, 1);
				args.Player.SendErrorMessage(GetString("You do not have permission to place Logic Sensors."));
				return true;
			}

			return false;
		}

		private static bool HandlePlaceItemFrame(GetDataHandlerArgs args)
		{
			var x = args.Data.ReadInt16();
			var y = args.Data.ReadInt16();
			var itemID = args.Data.ReadInt16();
			var prefix = args.Data.ReadInt8();
			var stack = args.Data.ReadInt16();
			var itemFrame = (TEItemFrame)TileEntity.ByID[TEItemFrame.Find(x, y)];

			if (OnPlaceItemFrame(args.Player, args.Data, x, y, itemID, prefix, stack, itemFrame))
			{
				return true;
			}

			return false;
		}

		private static bool HandleSyncExtraValue(GetDataHandlerArgs args)
		{
			var npcIndex = args.Data.ReadInt16();
			var extraValue = args.Data.ReadInt32();
			var position = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());

			if (position.X < 0 || position.X >= (Main.maxTilesX * 16.0f) || position.Y < 0 || position.Y >= (Main.maxTilesY * 16.0f))
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleSyncExtraValue rejected extents check {0}", args.Player.Name));
				return true;
			}

			if (!Main.expertMode && !Main.masterMode)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleSyncExtraValue rejected expert/master mode check {0}", args.Player.Name));
				return true;
			}

			if (npcIndex < 0 || npcIndex >= Main.npc.Length)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleSyncExtraValue rejected npc id out of bounds check - NPC ID: {0}", npcIndex));
				return true;
			}

			var npc = Main.npc[npcIndex];
			if (npc == null)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleSyncExtraValue rejected npc is null - NPC ID: {0}", npcIndex));
				return true;
			}

			var distanceFromCoinPacketToNpc = Utils.Distance(position, npc.position);
			if (distanceFromCoinPacketToNpc >= (5 * 16f)) //5 tile range
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleSyncExtraValue rejected range check {0},{1} vs {2},{3} which is {4}", npc.position.X, npc.position.Y, position.X, position.Y, distanceFromCoinPacketToNpc));
				return true;
			}

			return false;
		}

		private static bool HandleKillPortal(GetDataHandlerArgs args)
		{
			short projectileIndex = args.Data.ReadInt16();
			args.Data.ReadInt8(); // Read byte projectile AI

			Projectile projectile = Main.projectile[projectileIndex];
			if (projectile != null && projectile.active)
			{
				if (projectile.owner != args.TPlayer.whoAmI)
				{
					TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleKillPortal rejected owner mismatch check {0}", args.Player.Name));
					return true;
				}
			}

			return false;
		}

		private static bool HandlePlayerPortalTeleport(GetDataHandlerArgs args)
		{
			byte plr = args.Data.ReadInt8();
			short portalColorIndex = args.Data.ReadInt16();
			float newPositionX = args.Data.ReadSingle();
			float newPositionY = args.Data.ReadSingle();
			float newVelocityX = args.Data.ReadSingle();
			float newVelocityY = args.Data.ReadSingle();

			return OnPlayerTeleportThroughPortal(
				args.Player,
				plr,
				args.Data,
				new Vector2(newPositionX, newPositionY),
				new Vector2(newVelocityX, newVelocityY),
				portalColorIndex
			);
		}

		private static bool HandleNpcTeleportPortal(GetDataHandlerArgs args)
		{
			var npcIndex = args.Data.ReadUInt16();
			var portalColorIndex = args.Data.ReadInt16();
			var newPosition = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());
			var velocity = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());
			var projectile = Main.projectile.FirstOrDefault(p => p.position.X == newPosition.X && p.position.Y == newPosition.Y); // Check for projectiles at this location

			if (projectile == null || !projectile.active)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleNpcTeleportPortal rejected null check {0}", args.Player.Name));
				NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, NetworkText.Empty, npcIndex);
				return true;
			}

			if (projectile.type != ProjectileID.PortalGunGate)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleNpcTeleportPortal rejected not thinking with portals {0}", args.Player.Name));
				NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, NetworkText.Empty, npcIndex);
				return true;
			}

			return false;
		}

		private static bool HandleGemLockToggle(GetDataHandlerArgs args)
		{
			var x = args.Data.ReadInt16();
			var y = args.Data.ReadInt16();
			var on = args.Data.ReadBoolean();

			if (OnGemLockToggle(args.Player, args.Data, x, y, on))
			{
				return true;
			}

			return false;
		}

		private static bool HandleMassWireOperation(GetDataHandlerArgs args)
		{
			short startX = args.Data.ReadInt16();
			short startY = args.Data.ReadInt16();
			short endX = args.Data.ReadInt16();
			short endY = args.Data.ReadInt16();
			byte toolMode = (byte)args.Data.ReadByte();

			if (OnMassWireOperation(args.Player, args.Data, startX, startY, endX, endY, toolMode))
				return true;

			return false;
		}

		private static bool HandleToggleParty(GetDataHandlerArgs args)
		{
			if (args.Player != null && !args.Player.HasPermission(Permissions.toggleparty))
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleToggleParty rejected no party {0}", args.Player.Name));
				args.Player.SendErrorMessage(GetString("You do not have permission to start a party."));
				return true;
			}

			return false;
		}

		private static bool HandleOldOnesArmy(GetDataHandlerArgs args)
		{
			if (args.Player.IsBouncerThrottled())
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleOldOnesArmy rejected throttled {0}", args.Player.Name));
				return true;
			}

			if (!args.Player.HasPermission(Permissions.startdd2))
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleOldOnesArmy rejected permissions {0}", args.Player.Name));
				args.Player.SendErrorMessage(GetString("You do not have permission to start the Old One's Army."));
				return true;
			}

			if (TShock.Config.Settings.AnonymousBossInvasions)
				TShock.Utils.SendLogs(GetString("{0} started the Old One's Army event!", args.Player.Name), Color.PaleVioletRed, args.Player);
			else
				TShock.Utils.Broadcast(GetString("{0} started the Old One's Army event!", args.Player.Name), 175, 75, 255);
			return false;
		}

		private static bool HandlePlayerDamageV2(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt8();
			PlayerDeathReason playerDeathReason = PlayerDeathReason.FromReader(new BinaryReader(args.Data));
			var dmg = args.Data.ReadInt16();
			var direction = (byte)(args.Data.ReadInt8() - 1);
			var bits = (BitsByte)(args.Data.ReadByte());
			var crit = bits[0];
			var pvp = bits[1];
			var cooldownCounter = (sbyte)args.Data.ReadInt8();

			if (OnPlayerDamage(args.Player, args.Data, id, direction, dmg, pvp, crit, cooldownCounter, playerDeathReason))
				return true;

			return false;
		}

		private static bool HandlePlayerKillMeV2(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt8();
			PlayerDeathReason playerDeathReason = PlayerDeathReason.FromReader(new BinaryReader(args.Data));
			var dmg = args.Data.ReadInt16();
			var direction = (byte)(args.Data.ReadInt8() - 1);
			BitsByte bits = (BitsByte)args.Data.ReadByte();
			bool pvp = bits[0];

			if (OnKillMe(args.Player, args.Data, id, direction, dmg, pvp, playerDeathReason))
				return true;

			args.Player.Dead = true;
			args.Player.RespawnTimer = TShock.Config.Settings.RespawnSeconds;

			foreach (NPC npc in Main.npc)
			{
				if (npc.active && (npc.boss || npc.type == 13 || npc.type == 14 || npc.type == 15) &&
					Math.Abs(args.TPlayer.Center.X - npc.Center.X) + Math.Abs(args.TPlayer.Center.Y - npc.Center.Y) < 4000f)
				{
					args.Player.RespawnTimer = TShock.Config.Settings.RespawnBossSeconds;
					break;
				}
			}

			// Handle kicks/bans on mediumcore/hardcore deaths.
			if (args.TPlayer.difficulty == 1 || args.TPlayer.difficulty == 2) // Player is not softcore
			{
				bool mediumcore = args.TPlayer.difficulty == 1;
				bool shouldBan = mediumcore ? TShock.Config.Settings.BanOnMediumcoreDeath : TShock.Config.Settings.BanOnHardcoreDeath;
				bool shouldKick = mediumcore ? TShock.Config.Settings.KickOnMediumcoreDeath : TShock.Config.Settings.KickOnHardcoreDeath;
				string banReason = mediumcore ? TShock.Config.Settings.MediumcoreBanReason : TShock.Config.Settings.HardcoreBanReason;
				string kickReason = mediumcore ? TShock.Config.Settings.MediumcoreKickReason : TShock.Config.Settings.HardcoreKickReason;

				if (shouldBan)
				{
					if (!args.Player.Ban(banReason, "TShock"))
					{
						TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandlePlayerKillMeV2 kicked with difficulty {0} {1}", args.Player.Name, args.TPlayer.difficulty));
						args.Player.Kick(GetString("You died! Normally, you'd be banned."), true, true);
					}
				}
				else if (shouldKick)
				{
					TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandlePlayerKillMeV2 kicked with difficulty {0} {1}", args.Player.Name, args.TPlayer.difficulty));
					args.Player.Kick(kickReason, true, true, null, false);
				}
			}

			if (args.TPlayer.difficulty == 2 && Main.ServerSideCharacter && args.Player.IsLoggedIn)
			{
				if (TShock.CharacterDB.RemovePlayer(args.Player.Account.ID))
				{
					TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandlePlayerKillMeV2 ssc delete {0} {1}", args.Player.Name, args.TPlayer.difficulty));
					args.Player.SendErrorMessage(GetString("You have fallen in hardcore mode, and your items have been lost forever."));
					TShock.CharacterDB.SeedInitialData(args.Player.Account);
				}
			}

			return false;
		}

		private static bool HandleEmoji(GetDataHandlerArgs args)
		{
			byte playerIndex = args.Data.ReadInt8();
			byte emojiID = args.Data.ReadInt8();

			if (OnEmoji(args.Player, args.Data, playerIndex, emojiID))
				return true;

			return false;
		}

		private static bool HandleTileEntityDisplayDollItemSync(GetDataHandlerArgs args)
		{
			byte playerIndex = args.Data.ReadInt8();
			int tileEntityID = args.Data.ReadInt32();
			int slot = args.Data.ReadByte();
			bool isDye = false;
			if (slot >= 8)
			{
				isDye = true;
				slot -= 8;
			}

			Item newItem = new Item();
			Item oldItem = new Item();

			if (!TileEntity.ByID.TryGetValue(tileEntityID, out TileEntity tileEntity))
				return false;

			TEDisplayDoll displayDoll = tileEntity as TEDisplayDoll;
			if (displayDoll != null)
			{
				oldItem = displayDoll._items[slot];
				if (isDye)
					oldItem = displayDoll._dyes[slot];

				ushort itemType = args.Data.ReadUInt16();
				ushort stack = args.Data.ReadUInt16();
				int prefix = args.Data.ReadByte();

				if (oldItem.type == 0 && newItem.type == 0)
					return false;

				newItem.SetDefaults(itemType);
				newItem.stack = stack;
				newItem.Prefix(prefix);

				if (OnDisplayDollItemSync(args.Player, args.Data, playerIndex, tileEntityID, displayDoll, slot, isDye, oldItem, newItem))
					return true;
			}
			return false;
		}

		private static bool HandleRequestTileEntityInteraction(GetDataHandlerArgs args)
		{
			int tileEntityID = args.Data.ReadInt32();
			byte playerIndex = args.Data.ReadInt8();

			if (!TileEntity.ByID.TryGetValue(tileEntityID, out TileEntity tileEntity))
				return false;

			if (OnRequestTileEntityInteraction(args.Player, args.Data, tileEntity, playerIndex))
				return true;

			return false;
		}

		private static bool HandleSyncTilePicking(GetDataHandlerArgs args)
		{
			byte playerIndex = args.Data.ReadInt8();
			short tileX = args.Data.ReadInt16();
			short tileY = args.Data.ReadInt16();
			byte damage = args.Data.ReadInt8();

			if (OnSyncTilePicking(args.Player, args.Data, playerIndex, tileX, tileY, damage))
				return true;

			return false;
		}

		private static bool HandleSyncRevengeMarker(GetDataHandlerArgs args)
		{
			int uniqueID = args.Data.ReadInt32();
			Vector2 location = args.Data.ReadVector2();
			int netId = args.Data.ReadInt32();
			float npcHpPercent = args.Data.ReadSingle();
			int npcTypeAgainstDiscouragement = args.Data.ReadInt32(); //tfw the argument is Type Against Discouragement
			int npcAiStyleAgainstDiscouragement = args.Data.ReadInt32(); //see ^
			int coinsValue = args.Data.ReadInt32();
			float baseValue = args.Data.ReadSingle();
			bool spawnedFromStatus = args.Data.ReadBoolean();

			return false;
		}

		private static bool HandleLandGolfBallInCup(GetDataHandlerArgs args)
		{
			byte playerIndex = args.Data.ReadInt8();
			ushort tileX = args.Data.ReadUInt16();
			ushort tileY = args.Data.ReadUInt16();
			ushort hits = args.Data.ReadUInt16();
			ushort projectileType = args.Data.ReadUInt16();

			if (OnLandGolfBallInCup(args.Player, args.Data, playerIndex, tileX, tileY, hits, projectileType))
				return true;

			return false;
		}

		private static bool HandleFishOutNPC(GetDataHandlerArgs args)
		{
			ushort tileX = args.Data.ReadUInt16();
			ushort tileY = args.Data.ReadUInt16();
			short npcType = args.Data.ReadInt16();

			if (OnFishOutNPC(args.Player, args.Data, tileX, tileY, npcType))
				return true;

			return false;
		}

		private static bool HandleFoodPlatterTryPlacing(GetDataHandlerArgs args)
		{
			short tileX = args.Data.ReadInt16();
			short tileY = args.Data.ReadInt16();
			short itemID = args.Data.ReadInt16();
			byte prefix = args.Data.ReadInt8();
			short stack = args.Data.ReadInt16();

			if (OnFoodPlatterTryPlacing(args.Player, args.Data, tileX, tileY, itemID, prefix, stack))
				return true;

			return false;
		}

		private static bool HandleSyncCavernMonsterType(GetDataHandlerArgs args)
		{
			args.Player.Kick(GetString("Exploit attempt detected!"));
			TShock.Log.ConsoleDebug(GetString($"HandleSyncCavernMonsterType: Player is trying to modify NPC cavernMonsterType; this is a crafted packet! - From {args.Player.Name}"));
			return true;
		}

		private static bool HandleSyncLoadout(GetDataHandlerArgs args)
		{
			var playerIndex = args.Data.ReadInt8();
			var loadoutIndex = args.Data.ReadInt8();

			// When syncing a player's own loadout index, they then sync it back to us...
			// So let's only care if the index has actually changed, otherwise we might end up in a loop...
			if (loadoutIndex == args.TPlayer.CurrentLoadoutIndex)
				return false;

			if (loadoutIndex >= args.TPlayer.Loadouts.Length)
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleSyncLoadout rejected loadout index sync out of bounds {0}",
					args.Player.Name));
				NetMessage.SendData((int)PacketTypes.SyncLoadout, number: args.Player.Index, number2: args.TPlayer.CurrentLoadoutIndex);

				return true;
			}

			if (args.Player.IsBeingDisabled())
			{
				TShock.Log.ConsoleDebug(GetString("GetDataHandlers / HandleSyncLoadout rejected loadout index sync {0}", args.Player.Name));
				NetMessage.SendData((int)PacketTypes.SyncLoadout, number: args.Player.Index, number2: args.TPlayer.CurrentLoadoutIndex);

				return true;
			}

			// The client does not sync slot changes when changing loadouts, it only tells the server the loadout index changed,
			// and the server will replicate the changes the client did. This means that PlayerData.StoreSlot is never called, so we need to
			// swap around the PlayerData items ourself.

			Tuple<int, int> GetArmorSlotsForLoadoutIndex(int index)
			{
				return index switch
				{
					0 => NetItem.Loadout1Armor,
					1 => NetItem.Loadout2Armor,
					2 => NetItem.Loadout3Armor
				};
			}

			Tuple<int, int> GetDyeSlotsForLoadoutIndex(int index)
			{
				return index switch
				{
					0 => NetItem.Loadout1Dye,
					1 => NetItem.Loadout2Dye,
					2 => NetItem.Loadout3Dye
				};
			}

			var (currentLoadoutArmorSlotStartIndex, _) = GetArmorSlotsForLoadoutIndex(args.TPlayer.CurrentLoadoutIndex);
			var (currentLoadoutDyeSlotStartIndex, _) = GetDyeSlotsForLoadoutIndex(args.TPlayer.CurrentLoadoutIndex);

			var (switchedLoadoutArmorSlotStartIndex, _) = GetArmorSlotsForLoadoutIndex(loadoutIndex);
			var (switchedLoadoutDyeSlotStartIndex, _) = GetDyeSlotsForLoadoutIndex(loadoutIndex);

			// Emulate what is seen in Player.TrySwitchingLoadout:
			// - Swap the current loadout items with the player's equipment
			// - Swap the switching loadout items with the player's equipment

			// At the end of all of this:
			// - The current loadout will contain the player's original equipment
			// - The switched loadout will contain the current loadout's items
			// - The player's equipment will contain the switched loadout's item

			for (var i = 0; i < NetItem.LoadoutArmorSlots; i++)
				Terraria.Utils.Swap(ref args.Player.PlayerData.inventory[currentLoadoutArmorSlotStartIndex + i],
					ref args.Player.PlayerData.inventory[NetItem.ArmorIndex.Item1 + i]);
			for (var i = 0; i < NetItem.LoadoutDyeSlots; i++)
				Terraria.Utils.Swap(ref args.Player.PlayerData.inventory[currentLoadoutDyeSlotStartIndex + i],
					ref args.Player.PlayerData.inventory[NetItem.DyeIndex.Item1 + i]);

			for (var i = 0; i < NetItem.LoadoutArmorSlots; i++)
				Terraria.Utils.Swap(ref args.Player.PlayerData.inventory[switchedLoadoutArmorSlotStartIndex + i],
					ref args.Player.PlayerData.inventory[NetItem.ArmorIndex.Item1 + i]);
			for (var i = 0; i < NetItem.LoadoutDyeSlots; i++)
				Terraria.Utils.Swap(ref args.Player.PlayerData.inventory[switchedLoadoutDyeSlotStartIndex + i],
					ref args.Player.PlayerData.inventory[NetItem.DyeIndex.Item1 + i]);

			return false;
		}

		public enum DoorAction
		{
			OpenDoor = 0,
			CloseDoor,
			OpenTrapdoor,
			CloseTrapdoor,
			OpenTallGate,
			CloseTallGate
		}

		public enum EditAction
		{
			KillTile = 0,
			PlaceTile,
			KillWall,
			PlaceWall,
			KillTileNoItem,
			PlaceWire,
			KillWire,
			PoundTile,
			PlaceActuator,
			KillActuator,
			PlaceWire2,
			KillWire2,
			PlaceWire3,
			KillWire3,
			SlopeTile,
			FrameTrack,
			PlaceWire4,
			KillWire4,
			PokeLogicGate,
			Acutate,
			TryKillTile,
			ReplaceTile,
			ReplaceWall,
			SlopePoundTile
		}
		public enum EditType
		{
			Fail = 0,
			Type,
			Slope,
		}
		/// <summary>
		/// The maximum place styles for each tile.
		/// </summary>
		public static Dictionary<int, int> MaxPlaceStyles = new Dictionary<int, int>();

		/// <summary>
		/// Tiles that can be broken without any pickaxes/etc.
		/// </summary>
		internal static int[] breakableTiles = new int[]
		{
			TileID.Books,
			TileID.Bottles,
			TileID.BreakableIce,
			TileID.Candles,
			TileID.CorruptGrass,
			TileID.Dirt,
			TileID.CrimsonGrass,
			TileID.Grass,
			TileID.HallowedGrass,
			TileID.MagicalIceBlock,
			TileID.Mannequin,
			TileID.Torches,
			TileID.WaterCandle,
			TileID.Womannequin,
		};

		/// <summary>
		/// List of Fishing rod item IDs.
		/// </summary>
		internal static readonly List<int> FishingRodItemIDs = new List<int>()
		{
			ItemID.WoodFishingPole,
			ItemID.ReinforcedFishingPole,
			ItemID.FiberglassFishingPole,
			ItemID.FisherofSouls,
			ItemID.GoldenFishingRod,
			ItemID.MechanicsRod,
			ItemID.SittingDucksFishingRod,
			ItemID.Fleshcatcher,
			ItemID.HotlineFishingHook,
			ItemID.BloodFishingRod,
			ItemID.ScarabFishingRod
		};

		/// <summary>
		/// List of NPC IDs that can be fished out by the player.
		/// </summary>
		internal static readonly List<int> FishableNpcIDs = new List<int>()
		{
			NPCID.EyeballFlyingFish,
			NPCID.ZombieMerman,
			NPCID.GoblinShark,
			NPCID.BloodEelHead,
			NPCID.BloodEelBody,
			NPCID.BloodEelTail,
			NPCID.BloodNautilus,
			NPCID.DukeFishron,
			NPCID.TownSlimeRed
		};

		/// <summary>
		/// These projectiles create tiles on death.
		/// </summary>
		internal static Dictionary<int, int> projectileCreatesTile = new Dictionary<int, int>
		{
			{ ProjectileID.DirtBall, TileID.Dirt },
			{ ProjectileID.SandBallGun, TileID.Sand },
			{ ProjectileID.EbonsandBallGun, TileID.Ebonsand },
			{ ProjectileID.PearlSandBallGun, TileID.Pearlsand },
			{ ProjectileID.CrimsandBallGun, TileID.Crimsand },
			{ ProjectileID.MysticSnakeCoil, TileID.MysticSnakeRope },
			{ ProjectileID.RopeCoil, TileID.Rope },
			{ ProjectileID.SilkRopeCoil, TileID.SilkRope },
			{ ProjectileID.VineRopeCoil, TileID.VineRope },
			{ ProjectileID.WebRopeCoil, TileID.WebRope }
		};

		internal static List<int> CoilTileIds = new List<int>()
		{
			TileID.MysticSnakeRope,
			TileID.Rope,
			TileID.SilkRope,
			TileID.VineRope,
			TileID.WebRope
		};

		internal static Dictionary<int, LiquidType> projectileCreatesLiquid = new Dictionary<int, LiquidType>
		{
			{ProjectileID.LavaBomb, LiquidType.Lava},
			{ProjectileID.LavaRocket, LiquidType.Lava },
			{ProjectileID.LavaGrenade, LiquidType.Lava },
			{ProjectileID.LavaMine, LiquidType.Lava },
			//{ProjectileID.LavaSnowmanRocket, LiquidType.Lava }, //these require additional checks.
			{ProjectileID.WetBomb, LiquidType.Water},
			{ProjectileID.WetRocket, LiquidType.Water },
			{ProjectileID.WetGrenade, LiquidType.Water},
			{ProjectileID.WetMine, LiquidType.Water},
			//{ProjectileID.WetSnowmanRocket, LiquidType.Water}, //these require additional checks.
			{ProjectileID.HoneyBomb, LiquidType.Honey},
			{ProjectileID.HoneyRocket, LiquidType.Honey },
			{ProjectileID.HoneyGrenade, LiquidType.Honey },
			{ProjectileID.HoneyMine, LiquidType.Honey },
			//{ProjectileID.HoneySnowmanRocket, LiquidType.Honey }, //these require additional checks.
			{ProjectileID.DryBomb, LiquidType.Removal },
			{ProjectileID.DryRocket, LiquidType.Removal },
			{ProjectileID.DryGrenade, LiquidType.Removal },
			{ProjectileID.DryMine, LiquidType.Removal },
			//{ProjectileID.DrySnowmanRocket, LiquidType.Removal } //these require additional checks.
		};

		internal static Dictionary<int, int> ropeCoilPlacements = new Dictionary<int, int>
		{
			{ItemID.RopeCoil, TileID.Rope},
			{ItemID.SilkRopeCoil, TileID.SilkRope},
			{ItemID.VineRopeCoil, TileID.VineRope},
			{ItemID.WebRopeCoil, TileID.WebRope}
		};

		/// <summary>
		/// Extra place style limits for strange hardcoded values in Terraria
		/// </summary>
		internal static Dictionary<int, int> ExtraneousPlaceStyles = new Dictionary<int, int>
		{
			{TileID.Presents, 6},
			{TileID.Explosives, 1},
			{TileID.MagicalIceBlock, 0},
			{TileID.Crystals, 17},
			{TileID.MinecartTrack, 3}
		};

		/// <summary>
		/// Contains brief information about a projectile
		/// </summary>
		public struct ProjectileStruct
		{
			/// <summary>
			/// Index inside Main.projectile
			/// </summary>
			public int Index { get; set; }
			/// <summary>
			/// Projectile's type ID
			/// </summary>
			public short Type { get; set; }
			/// <summary>
			/// Time at which the projectile was created
			/// </summary>
			public DateTime CreatedAt { get; set; }
			/// <summary>
			/// Whether or not the projectile has been killed
			/// </summary>
			public bool Killed { get; internal set; }
		}

		public enum NetModuleType
		{
			Liquid,
			Text,
			Ping,
			Ambience,
			Bestiary,
			CreativeUnlocks,
			CreativePowers,
			CreativeUnlocksPlayerReport,
			TeleportPylon,
			Particles,
			CreativePowerPermissions
		}

		public enum CreativePowerTypes
		{
			FreezeTime,
			SetDawn,
			SetNoon,
			SetDusk,
			SetMidnight,
			Godmode,
			WindStrength,
			RainStrength,
			TimeSpeed,
			RainFreeze,
			WindFreeze,
			IncreasePlacementRange,
			WorldDifficulty,
			BiomeSpreadFreeze,
			SetSpawnRate
		}
	}
}
