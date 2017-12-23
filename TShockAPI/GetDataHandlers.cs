/*
TShock, a server mod for Terraria
Copyright (C) 2011-2017 Nyx Studios (fka. The TShock Team)

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
using OTAPI.Tile;
using TShockAPI.Localization;

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
		public static int[] WhitelistBuffMaxTime;
		#region Events

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
			public byte Slot { get; set; }
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
		private static bool OnPlayerSlot(TSPlayer player, MemoryStream data, byte _plr, byte _slot, short _stack, byte _prefix, short _type)
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
		/// For use in a PlayerMana event
		/// </summary>
		public class PlayerManaEventArgs : HandledEventArgs
		{
			public byte PlayerId { get; set; }
			public short Current { get; set; }
			public short Max { get; set; }
		}
		/// <summary>
		/// PlayerMana - called at a PlayerMana event
		/// </summary>
		public static HandlerList<PlayerManaEventArgs> PlayerMana = new HandlerList<PlayerManaEventArgs>();

		private static bool OnPlayerMana(byte _plr, short _cur, short _max)
		{
			if (PlayerMana == null)
				return false;

			var args = new PlayerManaEventArgs
			{
				PlayerId = _plr,
				Current = _cur,
				Max = _max,
			};
			PlayerMana.Invoke(null, args);
			return args.Handled;
		}

		public class PlayerInfoEventArgs : HandledEventArgs
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
		/// If this is cancelled, the server will ForceKick the player. If this should be changed in the future, let someone know.
		/// </summary>
		public static HandlerList<PlayerInfoEventArgs> PlayerInfo = new HandlerList<PlayerInfoEventArgs>();

		private static bool OnPlayerInfo(byte _plrid, byte _hair, int _style, byte _difficulty, string _name)
		{
			if (PlayerInfo == null)
				return false;

			var args = new PlayerInfoEventArgs
			{
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
		/// For use in a PlaceChest event
		/// </summary>
		public class PlaceChestEventArgs : HandledEventArgs
		{
			/// <summary>The TSPlayer that triggered the event</summary>
			public TSPlayer Player { get; set; }
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
		}
		/// <summary>
		/// When a chest is added or removed from the world.
		/// </summary>
		public static HandlerList<PlaceChestEventArgs> PlaceChest = new HandlerList<PlaceChestEventArgs>();

		private static bool OnPlaceChest(TSPlayer player, int flag, int tilex, int tiley)
		{
			if (PlaceChest == null)
				return false;

			var args = new PlaceChestEventArgs
			{
				Player = player,
				Flag = flag,
				TileX = tilex,
				TileY = tiley,
			};
			PlaceChest.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>The arguments to the ProjectileKill packet.</summary>
		public class ProjectileKillEventArgs : HandledEventArgs
		{
			/// <summary>The TSPlayer that fired the event.</summary>
			public TSPlayer Player;
			/// <summary>The projectile's identity...?</summary>
			public int ProjectileIdentity;
			/// <summary>The the player index of the projectile's owner (Main.players).</summary>
			public byte ProjectileOwner;
			/// <summary>The index of the projectile in Main.projectile.</summary>
			public int ProjectileIndex;
		}

		/// <summary>The event fired when a projectile kill packet is received.</summary>
		public static HandlerList<ProjectileKillEventArgs> ProjectileKill = new HandlerList<ProjectileKillEventArgs>();

		/// <summary>Fires the ProjectileKill event.</summary>
		/// <param name="player">The TSPlayer that caused the event.</param>
		/// <param name="identity">The projectile identity (from the packet).</param>
		/// <param name="owner">The projectile's owner (from the packet).</param>
		/// <param name="index">The projectile's index (from Main.projectiles).</param>
		/// <returns>bool</returns>
		private static bool OnProjectileKill(TSPlayer player, int identity, byte owner, int index)
		{
			if (ProjectileKill == null)
				return false;

			var args = new ProjectileKillEventArgs
			{
				Player = player,
				ProjectileIdentity = identity,
				ProjectileOwner = owner,
				ProjectileIndex = index,
			};

			ProjectileKill.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a KillMe event
		/// </summary>
		public class KillMeEventArgs : HandledEventArgs
		{
			/// <summary>The TSPlayer that triggered the event.</summary>
			public TSPlayer Player { get; set; }
			/// <summary>
			/// The Terraria playerID of the player
			/// </summary>
			public byte PlayerId { get; set; }
			/// <summary>
			/// The direction the damage is coming from (?)
			/// </summary>
			public byte Direction { get; set; }
			/// <summary>
			/// Amount of damage delt
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

		private static bool OnKillMe(TSPlayer player, byte plr, byte direction, short damage, bool pvp, PlayerDeathReason playerDeathReason)
		{
			if (KillMe == null)
				return false;

			var args = new KillMeEventArgs
			{
				Player = player,
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
		/// For use in a PlayerUpdate event
		/// </summary>
		public class PlayerUpdateEventArgs : HandledEventArgs
		{
			/// <summary>The TSPlayer object that triggered the event</summary>
			public TSPlayer Player { get; set; }
			/// <summary>
			/// The Terraria playerID of the player
			/// </summary>
			public byte PlayerId { get; set; }
			/// <summary>
			/// Control direction (BitFlags)
			/// </summary>
			public byte Control { get; set; }
			/// <summary>
			/// Selected item
			/// </summary>
			public byte Item { get; set; }
			/// <summary>
			/// Position of the player
			/// </summary>
			public Vector2 Position { get; set; }
			/// <summary>
			/// Velocity of the player
			/// </summary>
			public Vector2 Velocity { get; set; }
			/// <summary>Pulley update (BitFlags)</summary>
			public byte Pulley { get; set; }
		}
		/// <summary>
		/// PlayerUpdate - When the player sends it's updated information to the server
		/// </summary>
		public static HandlerList<PlayerUpdateEventArgs> PlayerUpdate = new HandlerList<PlayerUpdateEventArgs>();

		private static bool OnPlayerUpdate(TSPlayer player, byte plr, byte control, byte item, Vector2 position, Vector2 velocity, byte pulley)
		{
			if (PlayerUpdate == null)
				return false;

			var args = new PlayerUpdateEventArgs
			{
				Player = player,
				PlayerId = plr,
				Control = control,
				Item = item,
				Position = position,
				Velocity = velocity,
				Pulley = pulley
			};
			PlayerUpdate.Invoke(null, args);
			return args.Handled;
		}
		public static bool TSCheckNoclip(Vector2 Position, int Width, int Height)
		{
			int num = (int)(Position.X / 16f);
			int num2 = (int)((Position.X + (float)Width) / 16f);
			int num3 = (int)(Position.Y / 16f);
			int num4 = (int)((Position.Y + (float)Height) / 16f);
			if (num < 0)
			{
				num = 0;
			}
			if (num2 > Main.maxTilesX)
			{
				num2 = Main.maxTilesX;
			}
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num4 > Main.maxTilesY)
			{
				num4 = Main.maxTilesY;
			}
			for (int c = num; c < num2; c++)
			{
				for (int d = num3; d < num4; d++)
				{
					if (Main.tile[c, d].liquid != 0)
						return false;
				}
			}
			for (int i = num; i < num2; i++)
			{
				for (int j = num3; j < num4; j++)
				{
					if (Main.tile[i, j] == null || Main.tileSand[Main.tile[i, j].type]
						|| !TShock.Utils.TileSolid(i, j) || !TShock.Utils.TileSolid(i + 1, j) || !TShock.Utils.TileSolid(i - 1, j)
						|| !TShock.Utils.TileSolid(i, j + 1) || !TShock.Utils.TileSolid(i + 1, j + 1) || !TShock.Utils.TileSolid(i - 1, j + 1)
						|| !TShock.Utils.TileSolid(i, j - 1) || !TShock.Utils.TileSolid(i + 1, j - 1) || !TShock.Utils.TileSolid(i - 1, j - 1)
						|| Main.tileSolidTop[(int)Main.tile[i, j].type])
					{
						continue;
					}

					Vector2 vector;
					vector.X = (float)(i * 16);
					vector.Y = (float)(j * 16);
					if (Position.X + (float)Width > vector.X && Position.X < vector.X + 16f && Position.Y + (float)Height > vector.Y && Position.Y < vector.Y + 16f)
					{
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>The event args object for the HealOtherPlayer event</summary>
		public class HealOtherPlayerEventArgs : HandledEventArgs
		{
			/// <summary>The TSPlayer object that caused the event</summary>
			public TSPlayer Player { get; set; }

			/// <summary>The Terraria player index of the target player</summary>
			public byte TargetPlayerIndex { get; set; }

			/// <summary>The amount to heal by</summary>
			public short Amount { get; set; }
		}

		/// <summary>When a player heals another player</summary>
		public static HandlerList<HealOtherPlayerEventArgs> HealOtherPlayer = new HandlerList<HealOtherPlayerEventArgs>();

		/// <summary>Fires the HealOtherPlayer event</summary>
		/// <param name="player">The TSPlayer that started the event</param>
		/// <param name="targetPlayerIndex">The Terraria player index that the event targets</param>
		/// <param name="amount">The amount to heal</param>
		/// <returns>bool</returns>
		private static bool OnHealOtherPlayer(TSPlayer player, byte targetPlayerIndex, short amount)
		{
			if (HealOtherPlayer == null)
				return false;

			var args = new HealOtherPlayerEventArgs
			{
				Player = player,
				TargetPlayerIndex = targetPlayerIndex,
				Amount = amount,
			};

			HealOtherPlayer.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a SendTileSquare event
		/// </summary>
		public class SendTileSquareEventArgs : HandledEventArgs
		{
			/// <summary>The TSPlayer that triggered the event.</summary>
			public TSPlayer Player { get; set; }

			/// <summary>The raw memory stream from the original event</summary>
			public MemoryStream Data { get; set; }

			/// <summary>
			/// Size of the area
			/// </summary>
			public short Size { get; set; }

			/// <summary>
			/// A corner of the section
			/// </summary>
			public int TileX { get; set; }

			/// <summary>
			/// A corner of the section
			/// </summary>
			public int TileY { get; set; }
		}
		/// <summary>
		/// When the player sends a tile square
		/// </summary>
		public static HandlerList<SendTileSquareEventArgs> SendTileSquare = new HandlerList<SendTileSquareEventArgs>();

		private static bool OnSendTileSquare(TSPlayer player, MemoryStream data, short size, int tilex, int tiley)
		{
			if (SendTileSquare == null)
				return false;

			var args = new SendTileSquareEventArgs
			{
				Player = player,
				Data = data,
				Size = size,
				TileX = tilex,
				TileY = tiley,
			};

			SendTileSquare.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>The arguments to the PlaceObject hook.</summary>
		public class PlaceObjectEventArgs : HandledEventArgs
		{
			/// <summary>The calling Player.</summary>
			public TSPlayer Player { get; set; }

			/// <summary>The X location where the object was placed.</summary>
			public short X { get; set ; }

			/// <summary>The Y location where the object was placed.</summary>
			public short Y { get; set; }

			/// <summary>The type of object that was placed.</summary>
			public short Type { get; set; }

			/// <summary>The style of the object was placed.</summary>
			public short Style { get; set; }

			/// <summary>Alternate variation of the object placed.</summary>
			public byte Alternate { get; set; }

			/// <summary>The direction the object was placed.</summary>
			public bool Direction { get; set; }
		}

		/// <summary>Fired when an object is placed in the world.</summary>
		public static HandlerList<PlaceObjectEventArgs> PlaceObject = new HandlerList<PlaceObjectEventArgs>();

		/// <summary>Fires the PlaceObject hook. To be called when an object is placed in the world.</summary>
		/// <param name="player">The originating player.</param>
		/// <param name="x">The x position where the object is placed.</param>
		/// <param name="y">The y position where the object is placed.</param>
		/// <param name="type">The type of object.</param>
		/// <param name="style">The object's style data.</param>
		/// <param name="alternate">The object's alternate data.</param>
		/// <param name="direction">The direction of the object.</param>
		/// <returns>bool</returns>
		private static bool OnPlaceObject(TSPlayer player, short x, short y, short type, short style, byte alternate, bool direction)
		{
			if (PlaceObject == null)
				return false;

			var args = new PlaceObjectEventArgs
			{
				Player = player,
				X = x,
				Y = y,
				Type = type,
				Style = style,
				Alternate = alternate,
				Direction = direction
			};

			PlaceObject.Invoke(null, args);
			return args.Handled;
		}


		/// <summary>
		/// For use in a NewProjectile event
		/// </summary>
		public class NewProjectileEventArgs : HandledEventArgs
		{
			/// <summary>The TSPlayer that triggered the new projectile.</summary>
			public TSPlayer Player { get; set; }
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
		}
		/// <summary>
		/// NewProjectile - Called when a client creates a new projectile
		/// </summary>
		public static HandlerList<NewProjectileEventArgs> NewProjectile = new HandlerList<NewProjectileEventArgs>();

		private static bool OnNewProjectile(short ident, Vector2 pos, Vector2 vel, float knockback, short dmg, byte owner, short type, int index, TSPlayer player)
		{
			if (NewProjectile == null)
				return false;

			var args = new NewProjectileEventArgs
			{
				Identity = ident,
				Position = pos,
				Velocity = vel,
				Knockback = knockback,
				Damage = dmg,
				Owner = owner,
				Type = type,
				Index = index,
				Player = player,
			};
			NewProjectile.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a LiquidSet event
		/// </summary>
		public class LiquidSetEventArgs : HandledEventArgs
		{
			/// <summary>The TSPlayer that triggered the event.</summary>
			public TSPlayer Player { get; set; }
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
			/// Type of Liquid: 0=water, 1=lave, 2=honey
			/// </summary>
			public byte Type { get; set; }
		}
		/// <summary>
		/// LiquidSet - When ever a liquid is set
		/// </summary>
		public static HandlerList<LiquidSetEventArgs> LiquidSet = new HandlerList<LiquidSetEventArgs>();

		private static bool OnLiquidSet(TSPlayer player, int tilex, int tiley, byte amount, byte type)
		{
			if (LiquidSet == null)
				return false;

			var args = new LiquidSetEventArgs
			{
				Player = player,
				TileX = tilex,
				TileY = tiley,
				Amount = amount,
				Type = type,
			};
			LiquidSet.Invoke(null, args);
			return args.Handled;
		}
		/// <summary>
		/// For use in a PlayerSpawn event
		/// </summary>
		public class SpawnEventArgs : HandledEventArgs
		{
			/// <summary>
			/// The Terraria playerID of the player
			/// </summary>
			public byte Player { get; set; }
			/// <summary>
			/// X location of the player's spawn
			/// </summary>
			public int SpawnX { get; set; }
			/// <summary>
			/// Y location of the player's spawn
			/// </summary>
			public int SpawnY { get; set; }
		}
		/// <summary>
		/// PlayerSpawn - When a player spawns
		/// </summary>
		public static HandlerList<SpawnEventArgs> PlayerSpawn = new HandlerList<SpawnEventArgs>();

		private static bool OnPlayerSpawn(byte player, int spawnX, int spawnY)
		{
			if (PlayerSpawn == null)
				return false;

			var args = new SpawnEventArgs
			{
				Player = player,
				SpawnX = spawnX,
				SpawnY = spawnY,
			};
			PlayerSpawn.Invoke(null, args);
			return args.Handled;
		}
		/// <summary>
		/// For use with a ChestOpen event
		/// </summary>
		public class ChestOpenEventArgs : HandledEventArgs
		{
			/// <summary>
			/// X location of said chest
			/// </summary>
			public int X { get; set; }
			/// <summary>
			/// Y location of said chest
			/// </summary>
			public int Y { get; set; }

			/// <summary>
			/// The player opening the chest
			/// </summary>
			public TSPlayer Player { get; set; }
		}
		/// <summary>
		/// ChestOpen - Called when any chest is opened
		/// </summary>
		public static HandlerList<ChestOpenEventArgs> ChestOpen = new HandlerList<ChestOpenEventArgs>();

		private static bool OnChestOpen(int x, int y, TSPlayer player)
		{
			if (ChestOpen == null)
				return false;

			var args = new ChestOpenEventArgs
			{
				X = x,
				Y = y,
				Player = player,
			};
			ChestOpen.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a ChestItemChange event
		/// </summary>
		public class ChestItemEventArgs : HandledEventArgs
		{
			/// <summary>The TSPlayer that triggered the event.</summary>
			public TSPlayer Player { get; set; }
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

		private static bool OnChestItemChange(TSPlayer player, short id, byte slot, short stacks, byte prefix, short type)
		{
			if (ChestItemChange == null)
				return false;

			var args = new ChestItemEventArgs
			{
				Player = player,
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
		/// For use in a Sign event
		/// </summary>
		public class SignEventArgs : HandledEventArgs
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

		private static bool OnSignEvent(short id, int x, int y)
		{
			if (Sign == null)
				return false;

			var args = new SignEventArgs
			{
				ID = id,
				X = x,
				Y = y,
			};
			Sign.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a NPCHome event
		/// </summary>
		public class NPCHomeChangeEventArgs : HandledEventArgs
		{
			/// <summary>The TSPlayer that caused the event.</summary>
			public TSPlayer Player { get; set; }
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
			/// ByteBool homeless
			/// </summary>
			public byte Homeless { get; set; }
		}
		/// <summary>
		/// NPCHome - Called when an NPC's home is changed
		/// </summary>
		public static HandlerList<NPCHomeChangeEventArgs> NPCHome = new HandlerList<NPCHomeChangeEventArgs>();

		private static bool OnUpdateNPCHome(TSPlayer player, short id, short x, short y, byte homeless)
		{
			if (NPCHome == null)
				return false;

			var args = new NPCHomeChangeEventArgs
			{
				Player = player,
				ID = id,
				X = x,
				Y = y,
				Homeless = homeless,
			};
			NPCHome.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a PlayerBuff event
		/// </summary>
		public class PlayerBuffEventArgs : HandledEventArgs
		{
			public TSPlayer Player { get; set; }
			/// <summary>
			/// The Terraria playerID of the player
			/// </summary>
			public byte ID { get; set; }
			/// <summary>
			/// Buff Type
			/// </summary>
			public byte Type { get; set; }
			/// <summary>
			/// Time the buff lasts
			/// </summary>
			public int Time { get; set; }
		}
		/// <summary>
		/// PlayerBuff - Called when a player is buffed
		/// </summary>
		public static HandlerList<PlayerBuffEventArgs> PlayerBuff = new HandlerList<PlayerBuffEventArgs>();

		private static bool OnPlayerBuff(TSPlayer player, byte id, byte type, int time)
		{
			if (PlayerBuff == null)
				return false;

			var args = new PlayerBuffEventArgs
			{
				Player = player,
				ID = id,
				Type = type,
				Time = time
			};
			PlayerBuff.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in an ItemDrop event
		/// </summary>
		public class ItemDropEventArgs : HandledEventArgs
		{
			/// <summary>
			/// The player who sent message
			/// </summary>
			public TSPlayer Player { get; set; }
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

		private static bool OnItemDrop(TSPlayer player, short id, Vector2 pos, Vector2 vel, short stacks, byte prefix, bool noDelay, short type)
		{
			if (ItemDrop == null)
				return false;

			var args = new ItemDropEventArgs
			{
				Player = player,
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
		/// For use in a PlayerDamage event
		/// </summary>
		public class PlayerDamageEventArgs : HandledEventArgs
		{
			public TSPlayer Player { get; set; }
			/// <summary>
			/// The Terraria playerID of the player
			/// </summary>
			public byte ID { get; set; }
			/// <summary>
			/// The direction the damage is occuring from
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
			/// <summary>The reason the player took damage and/or died.</summary>
			public PlayerDeathReason PlayerDeathReason { get; set; }
		}
		/// <summary>
		/// PlayerDamage - Called when a player is damaged
		/// </summary>
		public static HandlerList<PlayerDamageEventArgs> PlayerDamage = new HandlerList<PlayerDamageEventArgs>();

		private static bool OnPlayerDamage(TSPlayer player, byte id, byte dir, short dmg, bool pvp, bool crit, PlayerDeathReason playerDeathReason)
		{
			if (PlayerDamage == null)
				return false;

			var args = new PlayerDamageEventArgs
			{
				Player = player,
				ID = id,
				Direction = dir,
				Damage = dmg,
				PVP = pvp,
				Critical = crit,
				PlayerDeathReason = playerDeathReason,
			};
			PlayerDamage.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use with a NPCStrike event
		/// </summary>
		public class NPCStrikeEventArgs : HandledEventArgs
		{
			/// <summary>The TSPlayer that triggered the event.</summary>
			public TSPlayer Player { get; set; }
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

		private static bool OnNPCStrike(TSPlayer player, short id, byte dir, short dmg, float knockback, byte crit)
		{
			if (NPCStrike == null)
				return false;

			var args = new NPCStrikeEventArgs
			{
				Player = player,
				ID = id,
				Direction = dir,
				Damage = dmg,
				Knockback = knockback,
				Critical = crit,
			};
			NPCStrike.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>The arguments to the MassWireOperation event.</summary>
		public class MassWireOperationEventArgs : HandledEventArgs
		{
			/// <summary>The TSPlayer that triggered the event.</summary>
			public TSPlayer Player { get; set; }

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

		private static bool OnMassWireOperation(TSPlayer player, short startX, short startY, short endX, short endY, byte toolMode)
		{
			if (MassWireOperation == null)
				return false;

			var args = new MassWireOperationEventArgs
			{
				Player = player,
				StartX = startX,
				StartY = startY,
				EndX = endX,
				EndY = endY,
				ToolMode = toolMode,
			};

			MassWireOperation.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>For use in a PlaceTileEntity event.</summary>
		public class PlaceTileEntityEventArgs : HandledEventArgs
		{
			/// <summary>The TSPlayer that triggered the event.</summary>
			public TSPlayer Player { get; set; }

			/// <summary>The X coordinate of the event.</summary>
			public short X { get; set; }

			/// <summary>The Y coordinate of the event.</summary>
			public short Y { get; set; }

			/// <summary>The Type of event.</summary>
			public byte Type { get; set; }
		}

		/// <summary>Fired when a PlaceTileEntity event occurs.</summary>
		public static HandlerList<PlaceTileEntityEventArgs> PlaceTileEntity = new HandlerList<PlaceTileEntityEventArgs>();

		private static bool OnPlaceTileEntity(TSPlayer player, short x, short y, byte type)
		{
			if (PlaceTileEntity == null)
				return false;

			var args = new PlaceTileEntityEventArgs
			{
				Player = player,
				X = x,
				Y = y,
				Type = type
			};

			PlaceTileEntity.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use with a NPCSpecial event
		/// </summary>
		public class NPCSpecialEventArgs : HandledEventArgs
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

		private static bool OnNPCSpecial(byte id, byte type)
		{
			if (NPCSpecial == null)
				return false;

			var args = new NPCSpecialEventArgs
			{
				ID = id,
				Type = type,
			};
			NPCSpecial.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use with a PlayerAnimation event
		/// </summary>
		public class PlayerAnimationEventArgs : HandledEventArgs
		{
			/// <summary>The TSPlayer that triggered the event.</summary>
			public TSPlayer Player { get; set; }
		}

		/// <summary>
		/// PlayerAnimation - Called when a player animates
		/// </summary>
		public static HandlerList<PlayerAnimationEventArgs> PlayerAnimation = new HandlerList<PlayerAnimationEventArgs>();

		private static bool OnPlayerAnimation(TSPlayer player)
		{
			if (PlayerAnimation == null)
				return false;

			var args = new PlayerAnimationEventArgs 
			{
				Player = player,
			};
			PlayerAnimation.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a PlayerBuffUpdate event
		/// </summary>
		public class PlayerBuffUpdateEventArgs : HandledEventArgs
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

		private static bool OnPlayerBuffUpdate(byte id)
		{
			if (PlayerBuffUpdate == null)
				return false;

			var args = new PlayerBuffUpdateEventArgs
			{
				ID = id,
			};
			PlayerBuffUpdate.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use with a NPCStrike event
		/// </summary>
		public class TeleportEventArgs : HandledEventArgs
		{
			/// <summary>
			/// ???
			/// </summary>
			public Int16 ID { get; set; }
			/// <summary>
			/// Flag is a bit field
			///   if the first bit is set -> 0 = player, 1 = NPC
			///	  if the second bit is set, ignore this packet
			///   if the third bit is set, style +1
			///   if the fourth bit is set, style +1
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
		}
		/// <summary>
		/// NPCStrike - Called when an NPC is attacked
		/// </summary>
		public static HandlerList<TeleportEventArgs> Teleport = new HandlerList<TeleportEventArgs>();

		private static bool OnTeleport(Int16 id, byte f, float x, float y)
		{
			if (Teleport == null)
				return false;

			var args = new TeleportEventArgs
			{
				ID = id,
				Flag = f,
				X = x,
				Y = y
			};
			Teleport.Invoke(null, args);
			return args.Handled;
		}

		#endregion
		public static void InitGetDataHandler()
		{
			#region Blacklists

			WhitelistBuffMaxTime = new int[Main.maxBuffTypes];
			WhitelistBuffMaxTime[20] = 600;
			WhitelistBuffMaxTime[0x18] = 1200;
			WhitelistBuffMaxTime[0x1f] = 120;
			WhitelistBuffMaxTime[0x27] = 420;

			#endregion Blacklists

			GetDataHandlerDelegates = new Dictionary<PacketTypes, GetDataHandlerDelegate>
				{
					{ PacketTypes.PlayerInfo, HandlePlayerInfo },
					{ PacketTypes.PlayerUpdate, HandlePlayerUpdate },
					{ PacketTypes.Tile, HandleTile },
					{ PacketTypes.PlaceObject, HandlePlaceObject },
					{ PacketTypes.TileSendSquare, HandleSendTileSquare },
					{ PacketTypes.ProjectileNew, HandleProjectileNew },
					{ PacketTypes.TogglePvp, HandleTogglePvp },
					{ PacketTypes.PlayerTeam, HandlePlayerTeam },
					{ PacketTypes.PlaceChest, HandlePlaceChest },
					{ PacketTypes.LiquidSet, HandleLiquidSet },
					{ PacketTypes.PlayerSpawn, HandleSpawn },
					{ PacketTypes.ChestGetContents, HandleChestOpen },
					{ PacketTypes.ChestOpen, HandleChestActive },
					{ PacketTypes.ChestItem, HandleChestItem },
					{ PacketTypes.SignNew, HandleSign },
					{ PacketTypes.PlayerSlot, HandlePlayerSlot },
					{ PacketTypes.TileGetSection, HandleGetSection },
					{ PacketTypes.UpdateNPCHome, UpdateNPCHome },
					{ PacketTypes.PlayerAddBuff, HandlePlayerAddBuff },
					{ PacketTypes.ItemDrop, HandleItemDrop },
					{ PacketTypes.UpdateItemDrop, HandleItemDrop },
					{ PacketTypes.ItemOwner, HandleItemOwner },
					{ PacketTypes.PlayerHp, HandlePlayerHp },
					{ PacketTypes.PlayerMana, HandlePlayerMana },
					{ PacketTypes.NpcStrike, HandleNpcStrike },
					{ PacketTypes.NpcSpecial, HandleSpecial },
					{ PacketTypes.PlayerAnimation, HandlePlayerAnimation },
					{ PacketTypes.PlayerBuff, HandlePlayerBuffList },
					{ PacketTypes.PasswordSend, HandlePassword },
					{ PacketTypes.ContinueConnecting2, HandleConnecting },
					{ PacketTypes.ProjectileDestroy, HandleProjectileKill },
					{ PacketTypes.SpawnBossorInvasion, HandleSpawnBoss },
					{ PacketTypes.Teleport, HandleTeleport },
					{ PacketTypes.PaintTile, HandlePaintTile },
					{ PacketTypes.PaintWall, HandlePaintWall },
					{ PacketTypes.DoorUse, HandleDoorUse },
					{ PacketTypes.CompleteAnglerQuest, HandleCompleteAnglerQuest },
					{ PacketTypes.NumberOfAnglerQuestsCompleted, HandleNumberOfAnglerQuestsCompleted },
					{ PacketTypes.MassWireOperation, HandleMassWireOperation },
					{ PacketTypes.GemLockToggle, HandleGemLockToggle },
					{ PacketTypes.CatchNPC, HandleCatchNpc },
					{ PacketTypes.NpcTeleportPortal, HandleNpcTeleportPortal },
					{ PacketTypes.KillPortal, HandleKillPortal },
					{ PacketTypes.PlaceTileEntity, HandlePlaceTileEntity },
					{ PacketTypes.PlaceItemFrame, HandlePlaceItemFrame },
					{ PacketTypes.SyncExtraValue, HandleSyncExtraValue },
					{ PacketTypes.LoadNetModule, HandleLoadNetModule },
					{ PacketTypes.ToggleParty, HandleToggleParty },
					{ PacketTypes.PlayerHealOther, HandleHealOther },
					{ PacketTypes.CrystalInvasionStart, HandleOldOnesArmy },
					{ PacketTypes.PlayerHurtV2, HandlePlayerDamageV2 },
					{ PacketTypes.PlayerDeathV2, HandlePlayerKillMeV2 }
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

		private static bool HandleHealOther(GetDataHandlerArgs args)
		{
			byte plr = args.Data.ReadInt8();
			short amount = args.Data.ReadInt16();

			if (OnHealOtherPlayer(args.Player, plr, amount))
				return true;

			return false;
		}

		private static bool HandlePlayerSlot(GetDataHandlerArgs args)
		{
			byte plr = args.Data.ReadInt8();
			byte slot = args.Data.ReadInt8();
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
			else if (Main.ServerSideCharacter && TShock.Config.DisableLoginBeforeJoin && !bypassTrashCanCheck &&
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

		public static bool HandlePlayerHp(GetDataHandlerArgs args)
		{
			var plr = args.Data.ReadInt8();
			var cur = args.Data.ReadInt16();
			var max = args.Data.ReadInt16();

			if (OnPlayerHP(args.Player, args.Data, plr, cur, max) || cur <= 0 || max <= 0 || args.Player.IgnoreSSCPackets)
				return true;

			if (max > TShock.Config.MaxHP && !args.Player.HasPermission(Permissions.ignorehp))
			{
				args.Player.Disable("Maximum HP beyond limit", DisableFlags.WriteToLogAndConsole);
				return true;
			}

			if (args.Player.IsLoggedIn)
			{
				args.Player.TPlayer.statLife = cur;
				args.Player.TPlayer.statLifeMax = max;
				args.Player.PlayerData.maxHealth = max;
			}

			if (args.Player.GodMode && (cur < max))
			{
				args.Player.Heal(args.TPlayer.statLifeMax2);
			}
			return false;
		}

		private static bool HandlePlayerMana(GetDataHandlerArgs args)
		{
			var plr = args.Data.ReadInt8();
			var cur = args.Data.ReadInt16();
			var max = args.Data.ReadInt16();

			if (OnPlayerMana(plr, cur, max) || cur < 0 || max < 0 || args.Player.IgnoreSSCPackets)
				return true;

			if (max > TShock.Config.MaxMP && !args.Player.HasPermission(Permissions.ignoremp))
			{
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
				difficulty++;
			}
			else if (extra[1])
			{
				difficulty += 2;
			}

			bool extraSlot = extra[2];

			if (OnPlayerInfo(playerid, hair, skinVariant, difficulty, name))
			{
				TShock.Utils.ForceKick(args.Player, "A plugin cancelled the event.", true);
				return true;
			}

			if (name.Trim().Length == 0)
			{
				TShock.Utils.ForceKick(args.Player, "Empty Name.", true);
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
				args.Player.TPlayer.hideVisual = new bool[10];
				for (int i = 0; i < 8; i++)
					args.Player.TPlayer.hideVisual[i] = hideVisual[i];
				for (int i = 8; i < 10; i++)
					args.Player.TPlayer.hideVisual[i] = hideVisual2[i];
				args.Player.TPlayer.hideMisc = hideMisc;
				args.Player.TPlayer.extraAccessory = extraSlot;
				NetMessage.SendData((int)PacketTypes.PlayerInfo, -1, args.Player.Index, NetworkText.FromLiteral(args.Player.Name), args.Player.Index);
				return true;
			}
			if (TShock.Config.MediumcoreOnly && difficulty < 1)
			{
				TShock.Utils.ForceKick(args.Player, "Server is set to mediumcore and above characters only!", true);
				return true;
			}
			if (TShock.Config.HardcoreOnly && difficulty < 2)
			{
				TShock.Utils.ForceKick(args.Player, "Server is set to hardcore characters only!", true);
				return true;
			}
			args.Player.Difficulty = difficulty;
			args.TPlayer.name = name;
			args.Player.ReceivedInfo = true;

			return false;
		}

		private static bool HandleConnecting(GetDataHandlerArgs args)
		{
			var account = TShock.UserAccounts.GetUserAccountByName(args.Player.Name);
			args.Player.DataWhenJoined = new PlayerData(args.Player);
			args.Player.DataWhenJoined.CopyCharacter(args.Player);

			if (account != null && !TShock.Config.DisableUUIDLogin)
			{
				if (account.UUID == args.Player.UUID)
				{
					if (args.Player.State == 1)
						args.Player.State = 2;
					NetMessage.SendData((int)PacketTypes.WorldInfo, args.Player.Index);

					args.Player.PlayerData = TShock.CharacterDB.GetPlayerData(args.Player, account.ID);

					var group = TShock.Utils.GetGroup(account.Group);

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

					args.Player.SendSuccessMessage("Authenticated as " + account.Name + " successfully.");
					TShock.Log.ConsoleInfo(args.Player.Name + " authenticated successfully as user " + args.Player.Name + ".");
					Hooks.PlayerHooks.OnPlayerPostLogin(args.Player);
					return true;
				}
			}
			else if (account != null && !TShock.Config.DisableLoginBeforeJoin)
			{
				args.Player.RequiresPassword = true;
				NetMessage.SendData((int)PacketTypes.PasswordRequired, args.Player.Index);
				return true;
			}
			else if (!string.IsNullOrEmpty(TShock.Config.ServerPassword))
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

		private static bool HandlePassword(GetDataHandlerArgs args)
		{
			if (!args.Player.RequiresPassword)
				return true;

			string password = args.Data.ReadString();

			if (Hooks.PlayerHooks.OnPlayerPreLogin(args.Player, args.Player.Name, password))
				return true;

			var account = TShock.UserAccounts.GetUserAccountByName(args.Player.Name);
			if (account != null && !TShock.Config.DisableLoginBeforeJoin)
			{
				if (account.VerifyPassword(password))
				{
					args.Player.RequiresPassword = false;
					args.Player.PlayerData = TShock.CharacterDB.GetPlayerData(args.Player, account.ID);

					if (args.Player.State == 1)
						args.Player.State = 2;
					NetMessage.SendData((int)PacketTypes.WorldInfo, args.Player.Index);

					var group = TShock.Utils.GetGroup(account.Group);

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


					args.Player.SendMessage("Authenticated as " + args.Player.Name + " successfully.", Color.LimeGreen);
					TShock.Log.ConsoleInfo(args.Player.Name + " authenticated successfully as user " + args.Player.Name + ".");
					TShock.UserAccounts.SetUserAccountUUID(account, args.Player.UUID);
					Hooks.PlayerHooks.OnPlayerPostLogin(args.Player);
					return true;
				}
				TShock.Utils.ForceKick(args.Player, "Invalid user account password.", true);
				return true;
			}
			if (!string.IsNullOrEmpty(TShock.Config.ServerPassword))
			{
				if (TShock.Config.ServerPassword == password)
				{
					args.Player.RequiresPassword = false;
					if (args.Player.State == 1)
						args.Player.State = 2;
					NetMessage.SendData((int)PacketTypes.WorldInfo, args.Player.Index);
					return true;
				}
				TShock.Utils.ForceKick(args.Player, "Incorrect server password", true);
				return true;
			}

			TShock.Utils.ForceKick(args.Player, "Bad password attempt", true);
			return true;
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

		/// <summary>Fires a GetSection event.</summary>
		/// <param name="player">The TSPlayer that caused the GetSection.</param>
		/// <param name="data">The raw MP protocol data.</param>
		/// <param name="x">The x coordinate requested or -1 for spawn.</param>
		/// <param name="y">The y coordinate requested or -1 for spawn.</param>
		/// <returns>bool</returns>
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

		private static bool HandleGetSection(GetDataHandlerArgs args)
		{
			if (OnGetSection(args.Player, args.Data, args.Data.ReadInt32(), args.Data.ReadInt32()))
				return true;

			if (TShock.Utils.ActivePlayers() + 1 > TShock.Config.MaxSlots &&
				!args.Player.HasPermission(Permissions.reservedslot))
			{
				TShock.Utils.ForceKick(args.Player, TShock.Config.ServerFullReason, true);
				return true;
			}

			NetMessage.SendData((int)PacketTypes.TimeSet, -1, -1, NetworkText.Empty, Main.dayTime ? 1 : 0, (int)Main.time, Main.sunModY, Main.moonModY);
			return false;
		}

		private static bool HandleSendTileSquare(GetDataHandlerArgs args)
		{
			var player = args.Player;
			var size = args.Data.ReadInt16();
			var tileX = args.Data.ReadInt16();
			var tileY = args.Data.ReadInt16();
			var data = args.Data;

			if (OnSendTileSquare(player, data, size, tileX, tileY))
				return true;

			return false;
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
			KillWire4
		}
		public enum EditType
		{
			Fail = 0,
			Type,
			Slope,
		}

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
			TileID.FleshGrass,
			TileID.Grass,
			TileID.HallowedGrass,
			TileID.MagicalIceBlock,
			TileID.Mannequin,
			TileID.Torches,
			TileID.WaterCandle,
			TileID.Womannequin,
		};
		/// <summary>
		/// The maximum place styles for each tile.
		/// </summary>
		public static Dictionary<int, int> MaxPlaceStyles = new Dictionary<int, int>();
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
			{TileID.MinecartTrack, 3}
		};

		private static bool HandleTile(GetDataHandlerArgs args)
		{
			EditAction action = (EditAction)args.Data.ReadInt8();
			var tileX = args.Data.ReadInt16();
			var tileY = args.Data.ReadInt16();
			var editData = args.Data.ReadInt16();
			EditType type = (action == EditAction.KillTile || action == EditAction.KillWall ||
							 action == EditAction.KillTileNoItem)
							? EditType.Fail
							: (action == EditAction.PlaceTile || action == EditAction.PlaceWall)
								? EditType.Type
								: EditType.Slope;

			var style = args.Data.ReadInt8();

			if (OnTileEdit(args.Player, args.Data, tileX, tileY, action, type, editData, style))
				return true;

			return false;
		}

		/// <summary>
		/// Handle PlaceObject event
		/// </summary>
		private static bool HandlePlaceObject(GetDataHandlerArgs args)
		{
			short x = args.Data.ReadInt16();
			short y = args.Data.ReadInt16();
			short type = args.Data.ReadInt16();
			short style = args.Data.ReadInt16();
			byte alternate = args.Data.ReadInt8();
			bool direction = args.Data.ReadBoolean();

			if (OnPlaceObject(args.Player, x, y, type, style, alternate, direction))
				return true;

			return false;
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
		}
		/// <summary>
		/// NPCStrike - Called when an NPC is attacked
		/// </summary>
		public static HandlerList<PaintTileEventArgs> PaintTile = new HandlerList<PaintTileEventArgs>();

		private static bool OnPaintTile(TSPlayer player, MemoryStream data, Int32 x, Int32 y, byte t)
		{
			if (PaintTile == null)
				return false;

			var args = new PaintTileEventArgs
			{
				Player = player,
				Data = data,
				X = x,
				Y = y,
				type = t
			};
			PaintTile.Invoke(null, args);
			return args.Handled;
		}


		/// <summary>
		/// For use with a PaintWall event
		/// </summary>
		public class PaintWallEventArgs : HandledEventArgs
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
		}
		/// <summary>
		/// Called When a wall is painted
		/// </summary>
		public static HandlerList<PaintWallEventArgs> PaintWall = new HandlerList<PaintWallEventArgs>();

		private static bool OnPaintWall(Int32 x, Int32 y, byte t)
		{
			if (PaintWall == null)
				return false;

			var args = new PaintWallEventArgs
			{
				X = x,
				Y = y,
				type = t
			};
			PaintWall.Invoke(null, args);
			return args.Handled;
		}

		private static bool HandleTogglePvp(GetDataHandlerArgs args)
		{
			byte id = args.Data.ReadInt8();
			bool pvp = args.Data.ReadBoolean();
			if (OnPvpToggled(args.Player, args.Data, id, pvp))
				return true;

			if (id != args.Player.Index)
				return true;

			string pvpMode = TShock.Config.PvPMode.ToLowerInvariant();
			if (pvpMode == "disabled" || pvpMode == "always" || (DateTime.UtcNow - args.Player.LastPvPTeamChange).TotalSeconds < 5)
			{
				args.Player.SendData(PacketTypes.TogglePvp, "", id);
				return true;
			}

			args.Player.LastPvPTeamChange = DateTime.UtcNow;
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

			if ((DateTime.UtcNow - args.Player.LastPvPTeamChange).TotalSeconds < 5)
			{
				args.Player.SendData(PacketTypes.PlayerTeam, "", id);
				return true;
			}

			args.Player.LastPvPTeamChange = DateTime.UtcNow;
			return false;
		}

		private static bool HandlePlayerUpdate(GetDataHandlerArgs args)
		{
			if (args.Player == null || args.TPlayer == null || args.Data == null)
			{
				return true;
			}

			byte plr = args.Data.ReadInt8();
			BitsByte control = args.Data.ReadInt8();
			BitsByte pulley = args.Data.ReadInt8();
			byte item = args.Data.ReadInt8();
			var pos = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());
			var vel = Vector2.Zero;
			if (pulley[2])
				vel = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());

			if (OnPlayerUpdate(args.Player, plr, control, item, pos, vel, pulley))
				return true;

			if (control[5])
			{
				// ItemBan system
				string itemName = args.TPlayer.inventory[item].Name;
				if (TShock.Itembans.ItemIsBanned(EnglishLanguage.GetItemNameById(args.TPlayer.inventory[item].netID), args.Player))
				{
					control[5] = false;
					args.Player.Disable("using a banned item ({0})".SFormat(itemName), DisableFlags.WriteToLogAndConsole);
					args.Player.SendErrorMessage("You cannot use {0} on this server. Your actions are being ignored.", itemName);
				}

				// Reimplementation of normal Terraria stuff?
				if (args.TPlayer.inventory[item].Name == "Mana Crystal" && args.Player.TPlayer.statManaMax <= 180)
				{
					args.Player.TPlayer.statMana += 20;
					args.Player.TPlayer.statManaMax += 20;
					args.Player.PlayerData.maxMana += 20;
				}
				else if (args.TPlayer.inventory[item].Name == "Life Crystal" && args.Player.TPlayer.statLifeMax <= 380)
				{
					args.TPlayer.statLife += 20;
					args.TPlayer.statLifeMax += 20;
					args.Player.PlayerData.maxHealth += 20;
				}
				else if (args.TPlayer.inventory[item].Name == "Life Fruit" && args.Player.TPlayer.statLifeMax >= 400 && args.Player.TPlayer.statLifeMax <= 495)
				{
					args.TPlayer.statLife += 5;
					args.TPlayer.statLifeMax += 5;
					args.Player.PlayerData.maxHealth += 5;
				}
			}

			// Where we rebuild sync data for Terraria?
			args.TPlayer.selectedItem = item;
			args.TPlayer.position = pos;
			args.TPlayer.oldVelocity = args.TPlayer.velocity;
			args.TPlayer.velocity = vel;
			args.TPlayer.fallStart = (int)(pos.Y / 16f);
			args.TPlayer.controlUp = false;
			args.TPlayer.controlDown = false;
			args.TPlayer.controlLeft = false;
			args.TPlayer.controlRight = false;
			args.TPlayer.controlJump = false;
			args.TPlayer.controlUseItem = false;
			args.TPlayer.pulley = pulley[0];

			if (pulley[0])
				args.TPlayer.pulleyDir = (byte)(pulley[1] ? 2 : 1);

			if (pulley[3])
				args.TPlayer.vortexStealthActive = true;

			args.TPlayer.gravDir = pulley[4] ? 1f : -1f;

			args.TPlayer.direction = -1;

			if (control[0])
			{
				args.TPlayer.controlUp = true;
			}
			if (control[1])
			{
				args.TPlayer.controlDown = true;
			}
			if (control[2])
			{
				args.TPlayer.controlLeft = true;
			}
			if (control[3])
			{
				args.TPlayer.controlRight = true;
			}
			if (control[4])
			{
				args.TPlayer.controlJump = true;
			}
			if (control[5])
			{
				args.TPlayer.controlUseItem = true;
			}
			if (control[6])
			{
				args.TPlayer.direction = 1;
			}
			else
			{
				args.TPlayer.direction = -1;
			}

			if (args.Player.Confused && Main.ServerSideCharacter && args.Player.IsLoggedIn)
			{
				if (args.TPlayer.controlUp)
				{
					args.TPlayer.controlDown = true;
					args.TPlayer.controlUp = false;
				}
				else if (args.TPlayer.controlDown)
				{
					args.TPlayer.controlDown = false;
					args.TPlayer.controlUp = true;
				}

				if (args.TPlayer.controlLeft)
				{
					args.TPlayer.controlRight = true;
					args.TPlayer.controlLeft = false;
				}
				else if (args.TPlayer.controlRight)
				{
					args.TPlayer.controlRight = false;
					args.TPlayer.controlLeft = true;
				}

				args.TPlayer.Update(args.TPlayer.whoAmI);
				NetMessage.SendData((int)PacketTypes.PlayerUpdate, -1, -1, NetworkText.Empty, args.Player.Index);
				return true;
			}

			NetMessage.SendData((int)PacketTypes.PlayerUpdate, -1, args.Player.Index, NetworkText.Empty, args.Player.Index);
			return true;
		}

		private static bool HandleProjectileNew(GetDataHandlerArgs args)
		{
			short ident = args.Data.ReadInt16();
			var pos = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());
			var vel = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());
			float knockback = args.Data.ReadSingle();
			short dmg = args.Data.ReadInt16();
			byte owner = args.Data.ReadInt8();
			short type = args.Data.ReadInt16();
			BitsByte bits = args.Data.ReadInt8();
			//owner = (byte)args.Player.Index;
			float[] ai = new float[Projectile.maxAI];

			for (int i = 0; i < Projectile.maxAI; i++)
			{
				if (bits[i])
					ai[i] = args.Data.ReadSingle();
				else
					ai[i] = 0f;
			}

			var index = TShock.Utils.SearchProjectile(ident, owner);

			if (OnNewProjectile(ident, pos, vel, knockback, dmg, owner, type, index, args.Player))
				return true;

			return false;
		}

		private static bool HandleProjectileKill(GetDataHandlerArgs args)
		{
			var ident = args.Data.ReadInt16();
			var owner = args.Data.ReadInt8();
			owner = (byte)args.Player.Index;
			var index = TShock.Utils.SearchProjectile(ident, owner);

			if (OnProjectileKill(args.Player, ident, owner, index))
			{
				return true;
			}

			var type = Main.projectile[index].type;

			// TODO: This needs to be moved somewhere else.
			if (!args.Player.HasProjectilePermission(index, type) && type != 102 && type != 100 && !TShock.Config.IgnoreProjKill)
			{
				args.Player.Disable("Does not have projectile permission to kill projectile.", DisableFlags.WriteToLogAndConsole);
				args.Player.RemoveProjectile(ident, owner);
				return true;
			}

			args.Player.LastKilledProjectile = type;

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

			if (OnKillMe(args.Player, id, direction, dmg, pvp, playerDeathReason))
				return true;

			args.Player.Dead = true;
			args.Player.RespawnTimer = TShock.Config.RespawnSeconds;

			foreach (NPC npc in Main.npc)
			{
				if (npc.active && (npc.boss || npc.type == 13 || npc.type == 14 || npc.type == 15) &&
					Math.Abs(args.TPlayer.Center.X - npc.Center.X) + Math.Abs(args.TPlayer.Center.Y - npc.Center.Y) < 4000f)
				{
					args.Player.RespawnTimer = TShock.Config.RespawnBossSeconds;
					break;
				}
			}

			if (args.TPlayer.difficulty == 2 && (TShock.Config.KickOnHardcoreDeath || TShock.Config.BanOnHardcoreDeath))
			{
				if (TShock.Config.BanOnHardcoreDeath)
				{
					if (!TShock.Utils.Ban(args.Player, TShock.Config.HardcoreBanReason, false, "hardcore-death"))
						TShock.Utils.ForceKick(args.Player, "Death results in a ban, but you are immune to bans.", true);
				}
				else
				{
					TShock.Utils.ForceKick(args.Player, TShock.Config.HardcoreKickReason, true, false);
				}
			}

			if (args.TPlayer.difficulty == 2 && Main.ServerSideCharacter && args.Player.IsLoggedIn)
			{
				if (TShock.CharacterDB.RemovePlayer(args.Player.Account.ID))
				{
					args.Player.SendErrorMessage("You have fallen in hardcore mode, and your items have been lost forever.");
					TShock.CharacterDB.SeedInitialData(args.Player.Account);
				}
			}

			return false;
		}

		private static bool HandleLiquidSet(GetDataHandlerArgs args)
		{
			int tileX = args.Data.ReadInt16();
			int tileY = args.Data.ReadInt16();
			byte amount = args.Data.ReadInt8();
			byte type = args.Data.ReadInt8();

			if (OnLiquidSet(args.Player, tileX, tileY, amount, type))
				return true;

			return false;
		}

		private static bool HandlePlaceChest(GetDataHandlerArgs args)
		{
			int flag = args.Data.ReadByte();
			int tileX = args.Data.ReadInt16();
			int tileY = args.Data.ReadInt16();
			args.Data.ReadInt16(); // Ignore style

			if (OnPlaceChest(args.Player, flag, tileX, tileY))
				return true;
			
			return false;
		}

		private static bool HandleSpawn(GetDataHandlerArgs args)
		{
			var player = args.Data.ReadInt8();
			var spawnx = args.Data.ReadInt16();
			var spawny = args.Data.ReadInt16();

			if (OnPlayerSpawn(player, spawnx, spawny))
				return true;

			if (args.Player.InitSpawn && args.TPlayer.inventory[args.TPlayer.selectedItem].type != 50)
			{
				if (args.TPlayer.difficulty == 1 && (TShock.Config.KickOnMediumcoreDeath || TShock.Config.BanOnMediumcoreDeath))
				{
					if (args.TPlayer.selectedItem != 50)
					{
						if (TShock.Config.BanOnMediumcoreDeath)
						{
							if (!TShock.Utils.Ban(args.Player, TShock.Config.MediumcoreBanReason, false, "mediumcore-death"))
								TShock.Utils.ForceKick(args.Player, "Death results in a ban, but you are immune to bans.", true);
						}
						else
						{
							TShock.Utils.ForceKick(args.Player, TShock.Config.MediumcoreKickReason, true, false);
						}
						return true;
					}
				}
			}
			else
				args.Player.InitSpawn = true;

			if ((Main.ServerSideCharacter) && (args.Player.sX > 0) && (args.Player.sY > 0) && (args.TPlayer.SpawnX > 0) && ((args.TPlayer.SpawnX != args.Player.sX) && (args.TPlayer.SpawnY != args.Player.sY)))
			{

				args.Player.sX = args.TPlayer.SpawnX;
				args.Player.sY = args.TPlayer.SpawnY;

				if (((Main.tile[args.Player.sX, args.Player.sY - 1].active() && Main.tile[args.Player.sX, args.Player.sY - 1].type == 79)) && (WorldGen.StartRoomCheck(args.Player.sX, args.Player.sY - 1)))
					args.Player.Teleport(args.Player.sX * 16, (args.Player.sY * 16) - 48);
			}

			else if ((Main.ServerSideCharacter) && (args.Player.sX > 0) && (args.Player.sY > 0))
			{
				if (((Main.tile[args.Player.sX, args.Player.sY - 1].active() && Main.tile[args.Player.sX, args.Player.sY - 1].type == 79)) && (WorldGen.StartRoomCheck(args.Player.sX, args.Player.sY - 1)))
					args.Player.Teleport(args.Player.sX * 16, (args.Player.sY * 16) - 48);
			}

			args.Player.Dead = false;
			return false;
		}

		private static bool HandleChestOpen(GetDataHandlerArgs args)
		{
			var x = args.Data.ReadInt16();
			var y = args.Data.ReadInt16();

			if (OnChestOpen(x, y, args.Player))
				return true;

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

			if (TShock.CheckTilePermission(args.Player, x, y) && TShock.Config.RegionProtectChests)
			{
				args.Player.SendData(PacketTypes.ChestOpen, "", -1);
				return true;
			}

			return false;
		}

		private static bool HandleChestItem(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt16();
			var slot = args.Data.ReadInt8();
			var stacks = args.Data.ReadInt16();
			var prefix = args.Data.ReadInt8();
			var type = args.Data.ReadInt16();

			if (OnChestItemChange(args.Player, id, slot, stacks, prefix, type))
				return true;

			Item item = new Item();
			item.netDefaults(type);
			if (stacks > item.maxStack || TShock.Itembans.ItemIsBanned(EnglishLanguage.GetItemNameById(item.type), args.Player))
			{
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

			if (OnSignEvent(id, x, y))
				return true;

			if (TShock.CheckTilePermission(args.Player, x, y))
			{
				args.Player.SendData(PacketTypes.SignNew, "", id);
				return true;
			}

			if (TShock.CheckRangePermission(args.Player, x, y))
			{
				args.Player.SendData(PacketTypes.SignNew, "", id);
				return true;
			}
			return false;
		}

		private static bool UpdateNPCHome(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt16();
			var x = args.Data.ReadInt16();
			var y = args.Data.ReadInt16();
			var homeless = args.Data.ReadInt8();

			if (OnUpdateNPCHome(args.Player, id, x, y, homeless))
				return true;

			if (!args.Player.HasPermission(Permissions.movenpc))
			{
				args.Player.SendErrorMessage("You do not have permission to relocate NPCs.");
				args.Player.SendData(PacketTypes.UpdateNPCHome, "", id, Main.npc[id].homeTileX, Main.npc[id].homeTileY,
									 Convert.ToByte(Main.npc[id].homeless));
				return true;
			}
			return false;
		}

		private static bool HandlePlayerAddBuff(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt8();
			var type = args.Data.ReadInt8();
			var time = args.Data.ReadInt32();

			if (OnPlayerBuff(args.Player, id, type, time))
				return true;

			args.Player.SendData(PacketTypes.PlayerAddBuff, "", id);
			return true;
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

			if (OnItemDrop(args.Player, id, pos, vel, stacks, prefix, noDelay, type))
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

		private static bool HandlePlayerDamageV2(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt8();
			PlayerDeathReason playerDeathReason = PlayerDeathReason.FromReader(new BinaryReader(args.Data));
			var dmg = args.Data.ReadInt16();
			var direction = (byte)(args.Data.ReadInt8() - 1);
			var bits = (BitsByte)(args.Data.ReadByte());
			var crit = bits[0];
			var pvp = bits[1];

			if (OnPlayerDamage(args.Player, id, direction, dmg, pvp, crit, playerDeathReason))
				return true;

			if (TShock.Players[id].GodMode)
			{
				TShock.Players[id].Heal(args.TPlayer.statLifeMax);
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

			if (OnNPCStrike(args.Player, id, direction, dmg, knockback, crit))
				return true;

			if (Main.npc[id].townNPC && !args.Player.HasPermission(Permissions.hurttownnpc))
			{
				args.Player.SendErrorMessage("You do not have permission to hurt this NPC.");
				args.Player.SendData(PacketTypes.NpcUpdate, "", id);
				return true;
			}

			return false;
		}

		private static bool HandleSpecial(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt8();
			var type = args.Data.ReadInt8();

			if (OnNPCSpecial(id, type))
				return true;

			if (type == 1 && TShock.Config.DisableDungeonGuardian)
			{
				args.Player.SendMessage("The Dungeon Guardian returned you to your spawn point", Color.Purple);
				args.Player.Spawn();
				return true;
			}

			if (type == 3 & !args.Player.HasPermission(Permissions.usesundial))
			{
				args.Player.SendErrorMessage("You do not have permission to use the Enchanted Sundial!");
				return true;
			}

			return false;
		}

		private static bool HandlePlayerAnimation(GetDataHandlerArgs args)
		{
			if (OnPlayerAnimation(args.Player))
				return true;

			return false;
		}

		private static bool HandlePlayerBuffList(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt8();

			if (OnPlayerBuffUpdate(id))
				return true;

			for (int i = 0; i < Terraria.Player.maxBuffs; i++)
			{
				var buff = args.Data.ReadInt8();

				if (buff == 10 && TShock.Config.DisableInvisPvP && args.TPlayer.hostile)
					buff = 0;

				if (Netplay.Clients[args.TPlayer.whoAmI].State < 2 && (buff == 156 || buff == 47 || buff == 149))
					buff = 0;

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


			NetMessage.SendData((int)PacketTypes.PlayerBuff, -1, args.Player.Index, NetworkText.Empty, args.Player.Index);
			return true;
		}

		private static bool HandleSpawnBoss(GetDataHandlerArgs args)
		{
			if (args.Player.IsBouncerThrottled())
			{
				return true;
			}

			var spawnboss = false;
			var invasion = false;
			var plr = args.Data.ReadInt16();
			var Type = args.Data.ReadInt16();
			NPC npc = new NPC();
			npc.SetDefaults(Type);
			spawnboss = npc.boss;
			if (!spawnboss)
			{
				switch (Type)
				{
					case -1:
					case -2:
					case -3:
					case -4:
					case -5:
					case -6:
					case -7:
					case -8:
						invasion = true;
						break;
					case 4:
					case 13:
					case 50:
					case 75:
					case 125:
					case 126:
					case 127:
					case 128:
					case 129:
					case 130:
					case 131:
					case 134:
					case 222:
					case 245:
					case 266:
					case 370:
					case 398:
					case 422:
					case 439:
					case 493:
					case 507:
					case 517:
						spawnboss = true;
						break;
				}
			}
			if (spawnboss && !args.Player.HasPermission(Permissions.summonboss))
			{
				args.Player.SendErrorMessage("You don't have permission to summon a boss.");
				return true;
			}
			if (invasion && !args.Player.HasPermission(Permissions.startinvasion))
			{
				args.Player.SendErrorMessage("You don't have permission to start an invasion.");
				return true;
			}
			if (!spawnboss && !invasion)
				return true;

			if (plr != args.Player.Index)
				return true;

			string boss;
			switch (Type)
			{
				case -8:
					boss = "a Moon Lord";
					break;
				case -7:
					boss = "a Martian invasion";
					break;
				case -6:
					boss = "an eclipse";
					break;
				case -5:
					boss = "a frost moon";
					break;
				case -4:
					boss = "a pumpkin moon";
					break;
				case -3:
					boss = "the Pirates";
					break;
				case -2:
					boss = "the Snow Legion";
					break;
				case -1:
					boss = "a Goblin Invasion";
					break;
				default:
					boss = String.Format("the {0}", npc.FullName);
					break;
			}
			if (TShock.Config.AnonymousBossInvasions)
				TShock.Utils.SendLogs(string.Format("{0} summoned {1}!", args.Player.Name, boss), Color.PaleVioletRed, args.Player);
			else
				TShock.Utils.Broadcast(String.Format("{0} summoned {1}!", args.Player.Name, boss), 175, 75, 255);
			return false;
		}

		private static bool HandlePaintTile(GetDataHandlerArgs args)
		{
			var x = args.Data.ReadInt16();
			var y = args.Data.ReadInt16();
			var t = args.Data.ReadInt8();

			if (x < 0 || y < 0 || x >= Main.maxTilesX || y >= Main.maxTilesY || t > Main.numTileColors)
			{
				return true;
			}
			if (OnPaintTile(args.Player, args.Data, x, y, t))
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
				!args.Player.Accessories.Any(i => i != null && i.stack > 0 &&
					(i.type == ItemID.PaintSprayer || i.type == ItemID.ArchitectGizmoPack)))
			{
				args.Player.SendData(PacketTypes.PaintTile, "", x, y, Main.tile[x, y].color());
				return true;
			}

			if (args.Player.IsBouncerThrottled() ||
				TShock.CheckTilePermission(args.Player, x, y, true) ||
				TShock.CheckRangePermission(args.Player, x, y))
			{
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

			if (x < 0 || y < 0 || x >= Main.maxTilesX || y >= Main.maxTilesY || t > Main.numTileColors)
			{
				return true;
			}
			if (OnPaintWall(x, y, t))
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
				!args.Player.Accessories.Any(i => i != null && i.stack > 0 &&
					(i.type == ItemID.PaintSprayer || i.type == ItemID.ArchitectGizmoPack)))
			{
				args.Player.SendData(PacketTypes.PaintWall, "", x, y, Main.tile[x, y].wallColor());
				return true;
			}

			if (args.Player.IsBouncerThrottled() ||
				TShock.CheckTilePermission(args.Player, x, y, true) ||
				TShock.CheckRangePermission(args.Player, x, y))
			{
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

			if (OnTeleport(id, flag, x, y))
				return true;

			int type = 0;
			byte style = 0;
			bool isNPC = type == 1;

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
				style++;
			}
			if (flag[3])
			{
				style += 2;
			}

			//Rod of Discord teleport (usually (may be used by modded clients to teleport))
			if (type == 0 && !args.Player.HasPermission(Permissions.rod))
			{
				args.Player.SendErrorMessage("You do not have permission to teleport.");
				args.Player.Teleport(args.TPlayer.position.X, args.TPlayer.position.Y);
				return true;
			}

			//NPC teleport
			if (type == 1 && id >= Main.maxNPCs)
			{
				return true;
			}

			//Player to player teleport (wormhole potion, usually (may be used by modded clients to teleport))
			if (type == 2)
			{
				if (id >= Main.maxPlayers || Main.player[id] == null || TShock.Players[id] == null)
				{
					return true;
				}

				if (!args.Player.HasPermission(Permissions.wormhole))
				{
					args.Player.SendErrorMessage("You do not have permission to teleport.");
					args.Player.Teleport(args.TPlayer.position.X, args.TPlayer.position.Y);
					return true;
				}
			}

			return false;
		}

		private static bool HandleDoorUse(GetDataHandlerArgs args)
		{
			byte type = (byte)args.Data.ReadByte();
			short x = args.Data.ReadInt16();
			short y = args.Data.ReadInt16();
			args.Data.ReadByte(); //Ignore direction

			if (x >= Main.maxTilesX || y >= Main.maxTilesY || x < 0 || y < 0) // Check for out of range
			{
				return true;
			}

			if (type < 0 || type > 5)
			{
				return true;
			}

			ushort tileType = Main.tile[x, y].type;

			if (tileType != TileID.ClosedDoor && tileType != TileID.OpenDoor
				&& tileType != TileID.TallGateClosed && tileType != TileID.TallGateOpen
				&& tileType != TileID.TrapdoorClosed && tileType != TileID.TrapdoorOpen)
			{
				return true;
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
			return true;
		}

		private static bool HandleMassWireOperation(GetDataHandlerArgs args)
		{
			short startX = args.Data.ReadInt16();
			short startY = args.Data.ReadInt16();
			short endX = args.Data.ReadInt16();
			short endY = args.Data.ReadInt16();
			byte toolMode = (byte) args.Data.ReadByte();

			if (OnMassWireOperation(args.Player, startX, startY, endX, endY, toolMode))
				return true;

			return false;
		}

		/// <summary>The arguments to the PlaceItemFrame event.</summary>
		public class PlaceItemFrameEventArgs : HandledEventArgs
		{
			/// <summary>The TSPlayer that triggered the event.</summary>
			public TSPlayer Player { get; set; }
			
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

		private static bool OnPlaceItemFrame(TSPlayer player, short x, short y, short itemID, byte prefix, short stack, TEItemFrame itemFrame)
		{
			if (PlaceItemFrame == null)
				return false;

			var args = new PlaceItemFrameEventArgs
			{
				Player = player,
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

		/// <summary>
		/// For use with a ToggleGemLock event
		/// </summary>
		public class GemLockToggleEventArgs : HandledEventArgs
		{
			/// <summary>The TSPlayer that triggered the event.</summary>
			public TSPlayer Player { get; set; }
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

		private static bool OnGemLockToggle(short x, short y, bool on)
		{
			if (GemLockToggle == null)
				return false;

			var args = new GemLockToggleEventArgs
			{
				X = x,
				Y = y,
				On = on
			};
			GemLockToggle.Invoke(null, args);
			return args.Handled;
		}

		private static bool HandleGemLockToggle(GetDataHandlerArgs args)
		{
			var x = args.Data.ReadInt16();
			var y = args.Data.ReadInt16();
			var on = args.Data.ReadBoolean();

			if (OnGemLockToggle(x, y, on))
			{
				return true;
			}

			if (!TShock.Config.RegionProtectGemLocks)
			{
				return false;
			}

			return false;
		}

		private static bool HandleCatchNpc(GetDataHandlerArgs args)
		{
			var npcID = args.Data.ReadInt16();
			var who = args.Data.ReadByte();

			if (Main.npc[npcID]?.catchItem == 0)
			{
				Main.npc[npcID].active = true;
				NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, NetworkText.Empty, npcID);
				return true;
			}

			return false;
		}

		private static bool HandleNpcTeleportPortal(GetDataHandlerArgs args)
		{
			var npcIndex = args.Data.ReadByte();
			var portalColorIndex = args.Data.ReadInt16();
			var newPosition = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());
			var velocity = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());
			var projectile = Main.projectile.FirstOrDefault(p => p.position.X == newPosition.X && p.position.Y == newPosition.Y); // Check for projectiles at this location

			if (projectile == null || !projectile.active)
			{
				NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, NetworkText.Empty, npcIndex);
				return true;
			}

			if (projectile.type != ProjectileID.PortalGunGate)
			{
				NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, NetworkText.Empty, npcIndex);
				return true;
			}

			return false;
		}

		private static bool HandleKillPortal(GetDataHandlerArgs args)
		{
			short projectileIndex = args.Data.ReadInt16();

			Projectile projectile = Main.projectile[projectileIndex];
			if (projectile != null && projectile.active)
			{
				if (projectile.owner != args.TPlayer.whoAmI)
				{
					return true;
				}
			}

			return false;
		}

		private static bool HandlePlaceTileEntity(GetDataHandlerArgs args)
		{
			var x = args.Data.ReadInt16();
			var y = args.Data.ReadInt16();
			var type = (byte) args.Data.ReadByte();

			if (OnPlaceTileEntity(args.Player, x, y, type))
			{
				return true;
			}

			// ItemBan subsystem

			if (TShock.TileBans.TileIsBanned((short)TileID.LogicSensor, args.Player))
			{
				args.Player.SendTileSquare(x, y, 1);
				args.Player.SendErrorMessage("You do not have permission to place Logic Sensors.");
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

			if (OnPlaceItemFrame(args.Player, x, y, itemID, prefix, stack, itemFrame))
			{
				return true;
			}

			return false;
		}

		private static bool HandleSyncExtraValue(GetDataHandlerArgs args)
		{
			var npcIndex = args.Data.ReadInt16();
			var extraValue = args.Data.ReadSingle();
			var position = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());

			if (position.X < 0 || position.X >= Main.maxTilesX || position.Y < 0 || position.Y >= Main.maxTilesY)
			{
				return true;
			}

			if (!Main.expertMode)
			{
				return true;
			}

			if (TShock.CheckRangePermission(args.Player, (int)position.X, (int)position.Y))
			{
				return true;
			}

			return false;
		}

		private static bool HandleLoadNetModule(GetDataHandlerArgs args)
		{
			// Since this packet is never actually sent to us, every attempt at sending it can be considered as a liquid exploit attempt
			return true;
		}

		private static bool HandleToggleParty(GetDataHandlerArgs args)
		{
			if (args.Player != null && !args.Player.HasPermission(Permissions.toggleparty))
			{
				args.Player.SendErrorMessage("You do not have permission to start a party.");
				return true;
			}

			return false;
		}

		private static bool HandleOldOnesArmy(GetDataHandlerArgs args)
		{
			if (args.Player.IsBouncerThrottled())
			{
				return true;
			}

			if (!args.Player.HasPermission(Permissions.startdd2))
			{
				args.Player.SendErrorMessage("You don't have permission to start the Old One's Army event.");
				return true;
			}

			if (TShock.Config.AnonymousBossInvasions)
				TShock.Utils.SendLogs(string.Format("{0} started the Old One's Army event!", args.Player.Name), Color.PaleVioletRed, args.Player);
			else
				TShock.Utils.Broadcast(string.Format("{0} started the Old One's Army event!", args.Player.Name), 175, 75, 255);
			return false;
		}
	}
}
