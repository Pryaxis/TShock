/*
TShock, a server mod for Terraria
Copyright (C) 2011-2015 Nyx Studios (fka. The TShock Team)

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

	public static class GetDataHandlers
	{
		private static Dictionary<PacketTypes, GetDataHandlerDelegate> GetDataHandlerDelegates;
		public static int[] WhitelistBuffMaxTime;
		#region Events

		/// <summary>
		/// Used when a TileEdit event is called.
		/// </summary>
		public class TileEditEventArgs : HandledEventArgs
		{
			/// <summary>
			/// The TSPlayer who made the tile edit
			/// </summary>
			public TSPlayer Player { get; set; }

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
		public static HandlerList<TileEditEventArgs> TileEdit;
		private static bool OnTileEdit(TSPlayer ply, int x, int y, EditAction action, EditType editDetail, short editData, byte style)
		{
			if (TileEdit == null)
				return false;

			var args = new TileEditEventArgs
			{
				Player = ply,
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
		public class TogglePvpEventArgs : HandledEventArgs
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
		public static HandlerList<TogglePvpEventArgs> TogglePvp;
		private static bool OnPvpToggled(byte _id, bool _pvp)
		{
			if (TogglePvp == null)
				return false;

			var args = new TogglePvpEventArgs
			{
				PlayerId = _id,
				Pvp = _pvp,
			};
			TogglePvp.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a PlayerTeam event
		/// </summary>
		public class PlayerTeamEventArgs : HandledEventArgs
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
		public static HandlerList<PlayerTeamEventArgs> PlayerTeam;
		private static bool OnPlayerTeam(byte _id, byte _team)
		{
			if (PlayerTeam == null)
				return false;

			var args = new PlayerTeamEventArgs
			{
				PlayerId = _id,
				Team = _team,
			};
			PlayerTeam.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a PlayerSlot event
		/// </summary>
		public class PlayerSlotEventArgs : HandledEventArgs
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
		public static HandlerList<PlayerSlotEventArgs> PlayerSlot;
		private static bool OnPlayerSlot(byte _plr, byte _slot, short _stack, byte _prefix, short _type)
		{
			if (PlayerSlot == null)
				return false;

			var args = new PlayerSlotEventArgs
			{
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
		public class PlayerHPEventArgs : HandledEventArgs
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
		public static HandlerList<PlayerHPEventArgs> PlayerHP;

		private static bool OnPlayerHP(byte _plr, short _cur, short _max)
		{
			if (PlayerHP == null)
				return false;

			var args = new PlayerHPEventArgs
			{
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
		public static HandlerList<PlayerManaEventArgs> PlayerMana;

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
		public static HandlerList<PlayerInfoEventArgs> PlayerInfo;

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
		/// For use in a TileKill event
		/// </summary>
		public class TileKillEventArgs : HandledEventArgs
		{
			/// <summary>
			/// The X coordinate that is being killed
			/// </summary>
			public int TileX { get; set; }
			/// <summary>
			/// The Y coordinate that is being killed
			/// </summary>
			public int TileY { get; set; }
		}
		/// <summary>
		/// TileKill - When a tile is removed from the world
		/// </summary>
		public static HandlerList<TileKillEventArgs> TileKill;

		private static bool OnTileKill(int tilex, int tiley)
		{
			if (TileKill == null)
				return false;

			var args = new TileKillEventArgs
			{
				TileX = tilex,
				TileY = tiley,
			};
			TileKill.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a KillMe event
		/// </summary>
		public class KillMeEventArgs : HandledEventArgs
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
			/// Amount of damage delt
			/// </summary>
			public short Damage { get; set; }
			/// <summary>
			/// Player's current pvp setting
			/// </summary>
			public bool Pvp { get; set; }
		}
		/// <summary>
		/// KillMe - Terraria's crappy way of handling damage from players
		/// </summary>
		public static HandlerList<KillMeEventArgs> KillMe;

		private static bool OnKillMe(byte plr, byte direction, short damage, bool pvp)
		{
			if (KillMe == null)
				return false;

			var args = new KillMeEventArgs
			{
				PlayerId = plr,
				Direction = direction,
				Damage = damage,
				Pvp = pvp,
			};
			KillMe.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a PlayerUpdate event
		/// </summary>
		public class PlayerUpdateEventArgs : HandledEventArgs
		{
			/// <summary>
			/// The Terraria playerID of the player
			/// </summary>
			public byte PlayerId { get; set; }
			/// <summary>
			/// ???
			/// </summary>
			public byte Control { get; set; }
			/// <summary>
			/// Current item?
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

			public byte Pulley { get; set; }
		}
		/// <summary>
		/// PlayerUpdate - When the player sends it's updated information to the server
		/// </summary>
		public static HandlerList<PlayerUpdateEventArgs> PlayerUpdate;

		private static bool OnPlayerUpdate(byte player, byte control, byte item, Vector2 position, Vector2 velocity, byte pulley)
		{
			if (PlayerUpdate == null)
				return false;

			var args = new PlayerUpdateEventArgs
			{
				PlayerId = player,
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

		/// <summary>
		/// For use in a SendTileSquare event
		/// </summary>
		public class SendTileSquareEventArgs : HandledEventArgs
		{
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
		/// SendTileSquare - When the player sends a tile square
		/// </summary>
		public static HandlerList<SendTileSquareEventArgs> SendTileSquare;

		private static bool OnSendTileSquare(short size, int tilex, int tiley)
		{
			if (SendTileSquare == null)
				return false;

			var args = new SendTileSquareEventArgs
			{
				Size = size,
				TileX = tilex,
				TileY = tiley,
			};
			SendTileSquare.Invoke(null, args);
			return args.Handled;
		}
		/// <summary>
		/// For use in a NewProjectile event
		/// </summary>
		public class NewProjectileEventArgs : HandledEventArgs
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
		}
		/// <summary>
		/// NewProjectile - Called when a client creates a new projectile
		/// </summary>
		public static HandlerList<NewProjectileEventArgs> NewProjectile;

		private static bool OnNewProjectile(short ident, Vector2 pos, Vector2 vel, float knockback, short dmg, byte owner, short type, int index)
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
			};
			NewProjectile.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use in a LiquidSet event
		/// </summary>
		public class LiquidSetEventArgs : HandledEventArgs
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
			/// Type of Liquid: 0=water, 1=lave, 2=honey
			/// </summary>
			public byte Type { get; set; }
		}
		/// <summary>
		/// LiquidSet - When ever a liquid is set
		/// </summary>
		public static HandlerList<LiquidSetEventArgs> LiquidSet;

		private static bool OnLiquidSet(int tilex, int tiley, byte amount, byte type)
		{
			if (LiquidSet == null)
				return false;

			var args = new LiquidSetEventArgs
			{
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
		public static HandlerList<SpawnEventArgs> PlayerSpawn;

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
		public static HandlerList<ChestOpenEventArgs> ChestOpen;

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
		public static HandlerList<ChestItemEventArgs> ChestItemChange;

		private static bool OnChestItemChange(short id, byte slot, short stacks, byte prefix, short type)
		{
			if (ChestItemChange == null)
				return false;

			var args = new ChestItemEventArgs
			{
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
		public static HandlerList<SignEventArgs> Sign;

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
		public static HandlerList<NPCHomeChangeEventArgs> NPCHome;

		private static bool OnUpdateNPCHome(short id, short x, short y, byte homeless)
		{
			if (NPCHome == null)
				return false;

			var args = new NPCHomeChangeEventArgs
			{
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
			public short Time { get; set; }
		}
		/// <summary>
		/// PlayerBuff - Called when a player is buffed
		/// </summary>
		public static HandlerList<PlayerBuffEventArgs> PlayerBuff;

		private static bool OnPlayerBuff(byte id, byte type, short time)
		{
			if (PlayerBuff == null)
				return false;

			var args = new PlayerBuffEventArgs
			{
				ID = id,
				Type = type,
				Time = time,
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
			/// The Terraria playerID of the player
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
		public static HandlerList<ItemDropEventArgs> ItemDrop;

		private static bool OnItemDrop(short id, Vector2 pos, Vector2 vel, short stacks, byte prefix, bool noDelay, short type)
		{
			if (ItemDrop == null)
				return false;

			var args = new ItemDropEventArgs
			{
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
		}
		/// <summary>
		/// PlayerDamage - Called when a player is damaged
		/// </summary>
		public static HandlerList<PlayerDamageEventArgs> PlayerDamage;

		private static bool OnPlayerDamage(byte id, byte dir, short dmg, bool pvp, bool crit)
		{
			if (PlayerDamage == null)
				return false;

			var args = new PlayerDamageEventArgs
			{
				ID = id,
				Direction = dir,
				Damage = dmg,
				PVP = pvp,
				Critical = crit,
			};
			PlayerDamage.Invoke(null, args);
			return args.Handled;
		}

		/// <summary>
		/// For use with a NPCStrike event
		/// </summary>
		public class NPCStrikeEventArgs : HandledEventArgs
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
		public static HandlerList<NPCStrikeEventArgs> NPCStrike;

		private static bool OnNPCStrike(short id, byte dir, short dmg, float knockback, byte crit)
		{
			if (NPCStrike == null)
				return false;

			var args = new NPCStrikeEventArgs
			{
				ID = id,
				Direction = dir,
				Damage = dmg,
				Knockback = knockback,
				Critical = crit,
			};
			NPCStrike.Invoke(null, args);
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
		public static HandlerList<NPCSpecialEventArgs> NPCSpecial;

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
		}

		/// <summary>
		/// PlayerAnimation - Called when a player animates
		/// </summary>
		public static HandlerList<PlayerAnimationEventArgs> PlayerAnimation;

		private static bool OnPlayerAnimation()
		{
			if (PlayerAnimation == null)
				return false;

			var args = new PlayerAnimationEventArgs { };
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
		public static HandlerList<PlayerBuffUpdateEventArgs> PlayerBuffUpdate;

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
		public static HandlerList<TeleportEventArgs> Teleport;

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
					{ PacketTypes.TileKill, HandleTileKill },
					{ PacketTypes.PlayerKillMe, HandlePlayerKillMe },
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
					{ PacketTypes.UpdateItemDrop, HandleUpdateItemDrop },
					{ PacketTypes.ItemOwner, HandleItemOwner },
					{ PacketTypes.PlayerHp, HandlePlayerHp },
					{ PacketTypes.PlayerMana, HandlePlayerMana },
					{ PacketTypes.PlayerDamage, HandlePlayerDamage },
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
					{ PacketTypes.ToggleParty, HandleToggleParty }
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

			if (OnPlayerSlot(plr, slot, stack, prefix, type) || plr != args.Player.Index || slot < 0 ||
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
				args.Player.IgnoreActionsForClearingTrashCan = true;
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

			if (OnPlayerHP(plr, cur, max) || cur <= 0 || max <= 0 || args.Player.IgnoreSSCPackets)
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
				NetMessage.SendData((int)PacketTypes.PlayerInfo, -1, args.Player.Index, args.Player.Name, args.Player.Index);
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
			var user = TShock.Users.GetUserByName(args.Player.Name);

			if (user != null && !TShock.Config.DisableUUIDLogin)
			{
				if (user.UUID == args.Player.UUID)
				{
					if (args.Player.State == 1)
						args.Player.State = 2;
					NetMessage.SendData((int)PacketTypes.WorldInfo, args.Player.Index);

					args.Player.PlayerData = TShock.CharacterDB.GetPlayerData(args.Player, user.ID);

					var group = TShock.Utils.GetGroup(user.Group);

					args.Player.Group = group;
					args.Player.tempGroup = null;
					args.Player.User = user;
					args.Player.IsLoggedIn = true;
					args.Player.IgnoreActionsForInventory = "none";

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
						args.Player.IgnoreActionsForCheating = "none";

					if (args.Player.HasPermission(Permissions.usebanneditem))
						args.Player.IgnoreActionsForDisabledArmor = "none";

					args.Player.SendSuccessMessage("Authenticated as " + user.Name + " successfully.");
					TShock.Log.ConsoleInfo(args.Player.Name + " authenticated successfully as user " + args.Player.Name + ".");
					Hooks.PlayerHooks.OnPlayerPostLogin(args.Player);
					return true;
				}
			}
			else if (user != null && !TShock.Config.DisableLoginBeforeJoin)
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

			var user = TShock.Users.GetUserByName(args.Player.Name);
			if (user != null && !TShock.Config.DisableLoginBeforeJoin)
			{
				if (user.VerifyPassword(password))
				{
					args.Player.RequiresPassword = false;
					args.Player.PlayerData = TShock.CharacterDB.GetPlayerData(args.Player, user.ID);

					if (args.Player.State == 1)
						args.Player.State = 2;
					NetMessage.SendData((int)PacketTypes.WorldInfo, args.Player.Index);

					var group = TShock.Utils.GetGroup(user.Group);

					args.Player.Group = group;
					args.Player.tempGroup = null;
					args.Player.User = user;
					args.Player.IsLoggedIn = true;
					args.Player.IgnoreActionsForInventory = "none";

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
						args.Player.IgnoreActionsForCheating = "none";

					if (args.Player.HasPermission(Permissions.usebanneditem))
						args.Player.IgnoreActionsForDisabledArmor = "none";


					args.Player.SendMessage("Authenticated as " + args.Player.Name + " successfully.", Color.LimeGreen);
					TShock.Log.ConsoleInfo(args.Player.Name + " authenticated successfully as user " + args.Player.Name + ".");
					TShock.Users.SetUserUUID(user, args.Player.UUID);
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

		private static bool HandleGetSection(GetDataHandlerArgs args)
		{
			if (args.Player.RequestedSection)
				return true;
			args.Player.RequestedSection = true;
			if (String.IsNullOrEmpty(args.Player.Name))
			{
				TShock.Utils.ForceKick(args.Player, "Blank name.", true);
				return true;
			}

			if (!args.Player.HasPermission(Permissions.ignorestackhackdetection))
			{
				TShock.HackedInventory(args.Player);
			}

			if (TShock.Utils.ActivePlayers() + 1 > TShock.Config.MaxSlots &&
				!args.Player.HasPermission(Permissions.reservedslot))
			{
				TShock.Utils.ForceKick(args.Player, TShock.Config.ServerFullReason, true);
				return true;
			}

			NetMessage.SendData((int)PacketTypes.TimeSet, -1, -1, "", Main.dayTime ? 1 : 0, (int)Main.time, Main.sunModY, Main.moonModY);
			return false;
		}

		/// <summary>
		/// Tile IDs that can be oriented:
		/// Cannon,
		/// Chairs,
		/// Beds,
		/// Bathtubs,
		/// Statues,
		/// Mannequin,
		/// Traps,
		/// MusicBoxes,
		/// ChristmasTree,
		/// WaterFountain,
		/// Womannequin,
		/// MinecartTrack,
		/// WeaponsRack,
		/// LunarMonolith
		/// </summary>
		private static int[] orientableTiles = new int[]
		{
			TileID.Cannon,
			TileID.Chairs,
			TileID.Beds,
			TileID.Bathtubs,
			TileID.Statues,
			TileID.Mannequin,
			TileID.Traps,
			TileID.MusicBoxes,
			TileID.ChristmasTree,
			TileID.WaterFountain,
			TileID.Womannequin,
			TileID.MinecartTrack,
			TileID.WeaponsRack,
			TileID.ItemFrame,
			TileID.LunarMonolith,
			TileID.TargetDummy
		};

		private static bool HandleSendTileSquare(GetDataHandlerArgs args)
		{
			var size = args.Data.ReadInt16();
			var tileX = args.Data.ReadInt16();
			var tileY = args.Data.ReadInt16();

			bool isTrapdoor = false;

			if (Main.tile[tileX, tileY].type == TileID.TrapdoorClosed
				|| Main.tile[tileX, tileY].type == TileID.TrapdoorOpen)
			{
				isTrapdoor = true;
			}

			if (args.Player.HasPermission(Permissions.allowclientsideworldedit) && !isTrapdoor)
				return false;

			if (OnSendTileSquare(size, tileX, tileY))
				return true;

			if (size > 5)
				return true;

			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
			{
				args.Player.SendTileSquare(tileX, tileY, size);
				return true;
			}

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendTileSquare(tileX, tileY, size);
				return true;
			}

			try
			{
				var tiles = new NetTile[size, size];
				for (int x = 0; x < size; x++)
				{
					for (int y = 0; y < size; y++)
					{
						tiles[x, y] = new NetTile(args.Data);
					}
				}

				bool changed = false;
				for (int x = 0; x < size; x++)
				{
					int realx = tileX + x;
					if (realx < 0 || realx >= Main.maxTilesX)
						continue;

					for (int y = 0; y < size; y++)
					{
						int realy = tileY + y;
						if (realy < 0 || realy >= Main.maxTilesY)
							continue;

						var tile = Main.tile[realx, realy];
						var newtile = tiles[x, y];
						if (TShock.CheckTilePermission(args.Player, realx, realy) ||
							TShock.CheckRangePermission(args.Player, realx, realy))
						{
							continue;
						}

						// Junction Box
						if (tile.type == TileID.WirePipe)
							return false;

						// Orientable tiles
						if (tile.type == newtile.Type && orientableTiles.Contains(tile.type))
						{
							Main.tile[realx, realy].frameX = newtile.FrameX;
							Main.tile[realx, realy].frameY = newtile.FrameY;
							changed = true;
						}
						// Landmine
						if (tile.type == TileID.LandMine && !newtile.Active)
						{
							Main.tile[realx, realy].active(false);
							changed = true;
						}
						// Sensors
						if(newtile.Type == TileID.LogicSensor && !Main.tile[realx, realy].active())
						{
							Main.tile[realx, realy].type = newtile.Type;
							Main.tile[realx, realy].frameX = newtile.FrameX;
							Main.tile[realx, realy].frameY = newtile.FrameY;
							Main.tile[realx, realy].active(true);
							changed = true;
						}

						if (tile.active() && newtile.Active)
						{
							// Grass <-> Grass
							if (((tile.type == 2 || tile.type == 23 || tile.type == 60 || tile.type == 70 || tile.type == 109 || tile.type == 199) &&
								(newtile.Type == 2 || newtile.Type == 23 || newtile.Type == 60 || newtile.Type == 70 || newtile.Type == 109 || newtile.Type == 199)) ||
								// Dirt <-> Dirt
								((tile.type == 0 || tile.type == 59) &&
								(newtile.Type == 0 || newtile.Type == 59)) ||
								// Ice <-> Ice
								((tile.type == 161 || tile.type == 163 || tile.type == 164 || tile.type == 200) &&
								(newtile.Type == 161 || newtile.Type == 163 || newtile.Type == 164 || newtile.Type == 200)) ||
								// Stone <-> Stone
								((tile.type == 1 || tile.type == 25 || tile.type == 117 || tile.type == 203 || Main.tileMoss[tile.type]) &&
								(newtile.Type == 1 || newtile.Type == 25 || newtile.Type == 117 || newtile.Type == 203 || Main.tileMoss[newtile.Type])) ||
								// Sand <-> Sand
								((tile.type == 53 || tile.type == 112 || tile.type == 116 || tile.type == 234) &&
								(newtile.Type == 53 || newtile.Type == 112 || newtile.Type == 116 || newtile.Type == 234)))
							{
								Main.tile[realx, realy].type = newtile.Type;
								changed = true;
							}
						}
						// Stone wall <-> Stone wall
						if (((tile.wall == 1 || tile.wall == 3 || tile.wall == 28 || tile.wall == 83) &&
							(newtile.Wall == 1 || newtile.Wall == 3 || newtile.Wall == 28 || newtile.Wall == 83)) ||
							// Leaf wall <-> Leaf wall
							(((tile.wall >= 63 && tile.wall <= 70) || tile.wall == 81) &&
							((newtile.Wall >= 63 && newtile.Wall <= 70) || newtile.Wall == 81)))
						{
							Main.tile[realx, realy].wall = newtile.Wall;
							changed = true;
						}
					}
				}

				if (changed)
				{
					TSPlayer.All.SendTileSquare(tileX, tileY, size + 1);
					WorldGen.RangeFrame(tileX, tileY, tileX + size, tileY + size);
				}
				else
				{
					args.Player.SendTileSquare(tileX, tileY, size);
				}
			}
			catch
			{
				args.Player.SendTileSquare(tileX, tileY, size);
			}
			return true;
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
			SlopeTile
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
		private static int[] breakableTiles = new int[]
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
		private static Dictionary<int, int> projectileCreatesTile = new Dictionary<int, int>
		{
			{ ProjectileID.DirtBall, TileID.Dirt },
			{ ProjectileID.SandBallGun, TileID.Sand },
			{ ProjectileID.EbonsandBallGun, TileID.Ebonsand },
			{ ProjectileID.PearlSandBallGun, TileID.Pearlsand },
			{ ProjectileID.CrimsandBallGun, TileID.Crimsand },
		};

		/// <summary>
		/// Extra place style limits for strange hardcoded values in Terraria
		/// </summary>
		private static Dictionary<int, int> ExtraneousPlaceStyles = new Dictionary<int, int>
		{
			{TileID.MinecartTrack, 3}
		};

		private static bool HandleTile(GetDataHandlerArgs args)
		{
			EditAction action = (EditAction)args.Data.ReadInt8();
			var tileX = args.Data.ReadInt16();
			var tileY = args.Data.ReadInt16();

			try
			{
				var editData = args.Data.ReadInt16();
				EditType type = (action == EditAction.KillTile || action == EditAction.KillWall ||
								 action == EditAction.KillTileNoItem)
								? EditType.Fail
								: (action == EditAction.PlaceTile || action == EditAction.PlaceWall)
									? EditType.Type
									: EditType.Slope;

				var style = args.Data.ReadInt8();

				if (OnTileEdit(args.Player, tileX, tileY, action, type, editData, style))
					return true;
				if (!TShock.Utils.TilePlacementValid(tileX, tileY))
					return true;
				if (action == EditAction.KillTile && Main.tile[tileX, tileY].type == TileID.MagicalIceBlock)
					return false;
				if (args.Player.Dead && TShock.Config.PreventDeadModification)
					return true;

				if (args.Player.AwaitingName)
				{
					Debug.Assert(args.Player.AwaitingNameParameters != null);

					bool includeUnprotected = false;
					bool includeZIndexes = false;
					bool persistentMode = false;
					foreach (string parameter in args.Player.AwaitingNameParameters)
					{
						if (parameter.Equals("-u", StringComparison.InvariantCultureIgnoreCase))
							includeUnprotected = true;
						if (parameter.Equals("-z", StringComparison.InvariantCultureIgnoreCase))
							includeZIndexes = true;
						if (parameter.Equals("-p", StringComparison.InvariantCultureIgnoreCase))
							persistentMode = true;
					}

					List<string> outputRegions = new List<string>();
					foreach (Region region in TShock.Regions.Regions.OrderBy(r => r.Z).Reverse())
					{
						if (!includeUnprotected && !region.DisableBuild)
							continue;
						if (tileX < region.Area.Left || tileX > region.Area.Right)
							continue;
						if (tileY < region.Area.Top || tileY > region.Area.Bottom)
							continue;

						string format = "{1}";
						if (includeZIndexes)
							format = "{1} (z:{0})";

						outputRegions.Add(string.Format(format, region.Z, region.Name));
					}

					if (outputRegions.Count == 0)
					{
						if (includeUnprotected)
							args.Player.SendInfoMessage("There are no regions at this point.");
						else
							args.Player.SendInfoMessage("There are no regions at this point or they are not protected.");
					}
					else
					{
						if (includeUnprotected)
							args.Player.SendSuccessMessage("Regions at this point:");
						else
							args.Player.SendSuccessMessage("Protected regions at this point:");

						foreach (string line in PaginationTools.BuildLinesFromTerms(outputRegions))
							args.Player.SendMessage(line, Color.White);
					}

					if (!persistentMode)
					{
						args.Player.AwaitingName = false;
						args.Player.AwaitingNameParameters = null;
					}

					args.Player.SendTileSquare(tileX, tileY, 4);
					return true;
				}

				if (args.Player.AwaitingTempPoint > 0)
				{
					args.Player.TempPoints[args.Player.AwaitingTempPoint - 1].X = tileX;
					args.Player.TempPoints[args.Player.AwaitingTempPoint - 1].Y = tileY;
					args.Player.SendInfoMessage("Set temp point {0}.", args.Player.AwaitingTempPoint);
					args.Player.SendTileSquare(tileX, tileY, 4);
					args.Player.AwaitingTempPoint = 0;
					return true;
				}

				Item selectedItem = args.Player.SelectedItem;
				int lastKilledProj = args.Player.LastKilledProjectile;
				Tile tile = Main.tile[tileX, tileY];

				if (action == EditAction.PlaceTile)
				{
					if (TShock.TileBans.TileIsBanned(editData, args.Player))
					{
						args.Player.SendTileSquare(tileX, tileY, 1);
						args.Player.SendErrorMessage("You do not have permission to place this tile.");
						return true;
					}
				}

				if (action == EditAction.KillTile && !Main.tileCut[tile.type] && !breakableTiles.Contains(tile.type))
				{
					//TPlayer.mount.Type 8 => Drill Containment Unit.

					// If the tile is an axe tile and they aren't selecting an axe, they're hacking.
					if (Main.tileAxe[tile.type] && ((args.Player.TPlayer.mount.Type != 8 && selectedItem.axe == 0) && !ItemID.Sets.Explosives[selectedItem.netID] && args.Player.RecentFuse == 0))
					{
						args.Player.SendTileSquare(tileX, tileY, 4);
						return true;
					}
					// If the tile is a hammer tile and they aren't selecting a hammer, they're hacking.
					else if (Main.tileHammer[tile.type] && ((args.Player.TPlayer.mount.Type != 8 && selectedItem.hammer == 0) && !ItemID.Sets.Explosives[selectedItem.netID] && args.Player.RecentFuse == 0))
					{
						args.Player.SendTileSquare(tileX, tileY, 4);
						return true;
					}
					// If the tile is a pickaxe tile and they aren't selecting a pickaxe, they're hacking.
					else if ((!Main.tileAxe[tile.type] && !Main.tileHammer[tile.type]) && tile.wall == 0 && ((args.TPlayer.mount.Type != 8 && selectedItem.pick == 0) && !ItemID.Sets.Explosives[selectedItem.netID] && args.Player.RecentFuse == 0))
					{
						args.Player.SendTileSquare(tileX, tileY, 4);
						return true;
					}
				}
				else if (action == EditAction.KillWall)
				{
					// If they aren't selecting a hammer, they could be hacking.
					if (selectedItem.hammer == 0 && !ItemID.Sets.Explosives[selectedItem.netID] && args.Player.RecentFuse == 0 && selectedItem.createWall == 0)
					{
						args.Player.SendTileSquare(tileX, tileY, 1);
						return true;
					}
				}
				else if (action == EditAction.PlaceTile && (projectileCreatesTile.ContainsKey(lastKilledProj) && editData == projectileCreatesTile[lastKilledProj]))
				{
					args.Player.LastKilledProjectile = 0;
				}
				else if (action == EditAction.PlaceTile || action == EditAction.PlaceWall)
				{
					if ((action == EditAction.PlaceTile && TShock.Config.PreventInvalidPlaceStyle) &&
						(MaxPlaceStyles.ContainsKey(editData) && style > MaxPlaceStyles[editData]) &&
						(ExtraneousPlaceStyles.ContainsKey(editData) && style > ExtraneousPlaceStyles[editData]))
					{
						args.Player.SendTileSquare(tileX, tileY, 4);
						return true;
					}

					// If they aren't selecting the item which creates the tile or wall, they're hacking.
					if (editData != TileID.MagicalIceBlock
						&& editData != (action == EditAction.PlaceTile ? selectedItem.createTile : selectedItem.createWall))
					{
						args.Player.SendTileSquare(tileX, tileY, 4);
						return true;
					}

					// Using the actuation accessory can lead to actuator hacking
					if (TShock.Itembans.ItemIsBanned("Actuator", args.Player) && args.Player.TPlayer.autoActuator)
					{
						args.Player.SendTileSquare(tileX, tileY, 1);
						args.Player.SendErrorMessage("You do not have permission to place actuators.");
						return true;
					}
					if (TShock.Itembans.ItemIsBanned(selectedItem.name, args.Player) || editData >= (action == EditAction.PlaceTile ? Main.maxTileSets : Main.maxWallTypes))
					{
						args.Player.SendTileSquare(tileX, tileY, 4);
						return true;
					}
					if (action == EditAction.PlaceTile && (editData == 29 || editData == 97) && Main.ServerSideCharacter)
					{
						args.Player.SendErrorMessage("You cannot place this tile because server side characters are enabled.");
						args.Player.SendTileSquare(tileX, tileY, 3);
						return true;
					}
					if (action == EditAction.PlaceTile && editData == TileID.Containers)
					{
						if (TShock.Utils.MaxChests())
						{
							args.Player.SendErrorMessage("The world's chest limit has been reached - unable to place more.");
							args.Player.SendTileSquare(tileX, tileY, 3);
							return true;
						}
						if ((TShock.Utils.TilePlacementValid(tileX, tileY + 1) && Main.tile[tileX, tileY + 1].type == TileID.Boulder) ||
							(TShock.Utils.TilePlacementValid(tileX + 1, tileY + 1) && Main.tile[tileX + 1, tileY + 1].type == TileID.Boulder))
						{
							args.Player.SendTileSquare(tileX, tileY, 3);
							return true;
						}
					}
				}
				else if (action == EditAction.PlaceWire || action == EditAction.PlaceWire2 || action == EditAction.PlaceWire3)
				{
					// If they aren't selecting a wrench, they're hacking.
					// WireKite = The Grand Design
					if (selectedItem.type != ItemID.Wrench
						&& selectedItem.type != ItemID.BlueWrench
						&& selectedItem.type != ItemID.GreenWrench
						&& selectedItem.type != ItemID.YellowWrench
						&& selectedItem.type != ItemID.MulticolorWrench
						&& selectedItem.type != ItemID.WireKite)
					{
						args.Player.SendTileSquare(tileX, tileY, 1);
						return true;
					}
				}
				else if (action == EditAction.KillActuator || action == EditAction.KillWire ||
					action == EditAction.KillWire2 || action == EditAction.KillWire3)
				{
					// If they aren't selecting the wire cutter, they're hacking.
					if (selectedItem.type != ItemID.WireCutter
						&& selectedItem.type != ItemID.WireKite
						&& selectedItem.type != ItemID.MulticolorWrench)
					{
						args.Player.SendTileSquare(tileX, tileY, 1);
						return true;
					}
				}
				else if (action == EditAction.PlaceActuator)
				{
					// If they aren't selecting the actuator and don't have the Presserator equipped, they're hacking.
					if (selectedItem.type != ItemID.Actuator && !args.Player.TPlayer.autoActuator)
					{
						args.Player.SendTileSquare(tileX, tileY, 1);
						return true;
					}
				}
				if (TShock.Config.AllowCutTilesAndBreakables && Main.tileCut[Main.tile[tileX, tileY].type])
				{
					return false;
				}

				if (TShock.CheckIgnores(args.Player))
				{
					args.Player.SendTileSquare(tileX, tileY, 4);
					return true;
				}

				if (TShock.CheckTilePermission(args.Player, tileX, tileY, editData, action))
				{
					args.Player.SendTileSquare(tileX, tileY, 4);
					return true;
				}

				if (TShock.CheckRangePermission(args.Player, tileX, tileY))
				{
					if (action == EditAction.PlaceTile && (editData == TileID.Rope || editData == TileID.SilkRope || editData == TileID.VineRope || editData == TileID.WebRope))
					{
						return false;
					}

					if (action == EditAction.KillTile || action == EditAction.KillWall && ItemID.Sets.Explosives[selectedItem.netID] && args.Player.RecentFuse == 0)
					{
						return false;
					}

					args.Player.SendTileSquare(tileX, tileY, 4);
					return true;
				}

				if (args.Player.TileKillThreshold >= TShock.Config.TileKillThreshold)
				{
					args.Player.Disable("Reached TileKill threshold.", DisableFlags.WriteToLogAndConsole);
					args.Player.SendTileSquare(tileX, tileY, 4);
					return true;
				}

				if (args.Player.TilePlaceThreshold >= TShock.Config.TilePlaceThreshold)
				{
					args.Player.Disable("Reached TilePlace threshold.", DisableFlags.WriteToLogAndConsole);
					args.Player.SendTileSquare(tileX, tileY, 4);
					return true;
				}

				if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
				{
					args.Player.SendTileSquare(tileX, tileY, 4);
					return true;
				}

				if ((action == EditAction.PlaceTile || action == EditAction.PlaceWall) && !args.Player.HasPermission(Permissions.ignoreplacetiledetection))
				{
					args.Player.TilePlaceThreshold++;
					var coords = new Vector2(tileX, tileY);
					lock (args.Player.TilesCreated)
						if (!args.Player.TilesCreated.ContainsKey(coords))
							args.Player.TilesCreated.Add(coords, Main.tile[tileX, tileY]);
				}

				if ((action == EditAction.KillTile || action == EditAction.KillTileNoItem || action == EditAction.KillWall) && Main.tileSolid[Main.tile[tileX, tileY].type] &&
					!args.Player.HasPermission(Permissions.ignorekilltiledetection))
				{
					args.Player.TileKillThreshold++;
					var coords = new Vector2(tileX, tileY);
					lock (args.Player.TilesDestroyed)
						if (!args.Player.TilesDestroyed.ContainsKey(coords))
							args.Player.TilesDestroyed.Add(coords, Main.tile[tileX, tileY]);
				}
				return false;
			}
			catch
			{
				args.Player.SendTileSquare(tileX, tileY, 4);
				return true;
			}
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

			if (type < 0 || type >= Main.maxTileSets)
				return true;

			if (x < 0 || x >= Main.maxTilesX)
				return true;

			if (y < 0 || y >= Main.maxTilesY)
				return true;

			//style 52 and 53 are used by ItemID.Fake_newchest1 and ItemID.Fake_newchest2
			//These two items cause localised lag and rendering issues
			if (type == TileID.FakeContainers && (style == 52 || style == 53))
			{
				args.Player.SendTileSquare(x, y, 4);
				return true;
			}

			if (TShock.TileBans.TileIsBanned(type, args.Player))
			{
				args.Player.SendTileSquare(x, y, 1);
				args.Player.SendErrorMessage("You do not have permission to place this tile.");
				return true;
			}

			if (!TShock.Utils.TilePlacementValid(x, y))
				return true;
			if (args.Player.Dead && TShock.Config.PreventDeadModification)
				return true;

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendTileSquare(x, y, 4);
				return true;
			}

			TileObjectData tileData = TileObjectData.GetTileData(type, style, 0);
			if (tileData == null)
				return true;

			x -= tileData.Origin.X;
			y -= tileData.Origin.Y;

			for (int i = x; i < x + tileData.Width; i++)
			{
				for (int j = y; j < y + tileData.Height; j++)
				{
					if (TShock.CheckTilePermission(args.Player, i, j, type, EditAction.PlaceTile))
					{
						args.Player.SendTileSquare(i, j, 4);
						return true;
					}
				}
			}

			// Ignore rope placement range
			if ((type != TileID.Rope
					|| type != TileID.SilkRope
					|| type != TileID.VineRope
					|| type != TileID.WebRope)
					&& TShock.CheckRangePermission(args.Player, x, y))
			{
				args.Player.SendTileSquare(x, y, 4);
				return true;
			}

			if (args.Player.TilePlaceThreshold >= TShock.Config.TilePlaceThreshold)
			{
				args.Player.Disable("Reached TilePlace threshold.", DisableFlags.WriteToLogAndConsole);
				args.Player.SendTileSquare(x, y, 4);
				return true;
			}

			if (!args.Player.HasPermission(Permissions.ignoreplacetiledetection))
			{
				args.Player.TilePlaceThreshold++;
				var coords = new Vector2(x, y);
				lock (args.Player.TilesCreated)
					if (!args.Player.TilesCreated.ContainsKey(coords))
						args.Player.TilesCreated.Add(coords, Main.tile[x, y]);
			}

			return false;
		}

		/// <summary>
		/// For use with a PaintTile event
		/// </summary>
		public class PaintTileEventArgs : HandledEventArgs
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
		public static HandlerList<PaintTileEventArgs> PaintTile;

		private static bool OnPaintTile(Int32 x, Int32 y, byte t)
		{
			if (PaintTile == null)
				return false;

			var args = new PaintTileEventArgs
			{
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
		public static HandlerList<PaintWallEventArgs> PaintWall;

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
			if (OnPvpToggled(id, pvp))
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
			if (OnPlayerTeam(id, team))
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
			byte plr = args.Data.ReadInt8();
			BitsByte control = args.Data.ReadInt8();
			BitsByte pulley = args.Data.ReadInt8();
			byte item = args.Data.ReadInt8();
			var pos = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());
			var vel = Vector2.Zero;
			if (pulley[2])
				vel = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());

			if (OnPlayerUpdate(plr, control, item, pos, vel, pulley))
				return true;

			if (pos.X < 0 || pos.Y < 0 || pos.X >= Main.maxTilesX * 16 - 16 || pos.Y >= Main.maxTilesY * 16 - 16)
			{
				return true;
			}

			if (item < 0 || item >= args.TPlayer.inventory.Length)
			{
				return true;
			}

			if (args.Player.LastNetPosition == Vector2.Zero)
			{
				return true;
			}

			if (!pos.Equals(args.Player.LastNetPosition))
			{
				float distance = Vector2.Distance(new Vector2(pos.X / 16f, pos.Y / 16f),
													new Vector2(args.Player.LastNetPosition.X / 16f, args.Player.LastNetPosition.Y / 16f));
				if (TShock.CheckIgnores(args.Player))
				{
					if (distance > TShock.Config.MaxRangeForDisabled)
					{
						if (args.Player.IgnoreActionsForCheating != "none")
						{
							args.Player.SendErrorMessage("Disabled for cheating: " + args.Player.IgnoreActionsForCheating);
						}
						else if (args.Player.IgnoreActionsForDisabledArmor != "none")
						{
							args.Player.SendErrorMessage("Disabled for banned armor: " + args.Player.IgnoreActionsForDisabledArmor);
						}
						else if (args.Player.IgnoreActionsForInventory != "none")
						{
							args.Player.SendErrorMessage("Disabled for Server Side Inventory: " + args.Player.IgnoreActionsForInventory);
						}
						else if (TShock.Config.RequireLogin && !args.Player.IsLoggedIn)
						{
							args.Player.SendErrorMessage("Please /register or /login to play!");
						}
						else if (args.Player.IgnoreActionsForClearingTrashCan)
						{
							args.Player.SendErrorMessage("You need to rejoin to ensure your trash can is cleared!");
						}
						var lastTileX = args.Player.LastNetPosition.X;
						var lastTileY = args.Player.LastNetPosition.Y - 48;
						if (!args.Player.Teleport(lastTileX, lastTileY))
						{
							args.Player.Spawn();
						}
						return true;
					}
					return true;
				}

				if (args.Player.Dead)
				{
					return true;
				}

				if (!args.Player.HasPermission(Permissions.ignorenoclipdetection) &&
					TSCheckNoclip(pos, args.TPlayer.width, args.TPlayer.height - (args.TPlayer.mount.Active ? args.Player.TPlayer.mount.HeightBoost : 0)) && !TShock.Config.IgnoreNoClip
					&& !args.TPlayer.tongued)
				{
					var lastTileX = args.Player.LastNetPosition.X;
					var lastTileY = args.Player.LastNetPosition.Y;
					if (!args.Player.Teleport(lastTileX, lastTileY))
					{
						args.Player.SendErrorMessage("You got stuck in a solid object, Sent to spawn point.");
						args.Player.Spawn();
					}
					return true;
				}
				args.Player.LastNetPosition = pos;
			}

			if (control[5])
			{
				string itemName = args.TPlayer.inventory[item].name;
				if (TShock.Itembans.ItemIsBanned(itemName, args.Player))
				{
					control[5] = false;
					args.Player.Disable("using a banned item ({0})".SFormat(itemName), DisableFlags.WriteToLogAndConsole);
					args.Player.SendErrorMessage("You cannot use {0} on this server. Your actions are being ignored.", itemName);
				}

				if (args.TPlayer.inventory[item].name == "Mana Crystal" && args.Player.TPlayer.statManaMax <= 180)
				{
					args.Player.TPlayer.statMana += 20;
					args.Player.TPlayer.statManaMax += 20;
					args.Player.PlayerData.maxMana += 20;
				}
				else if (args.TPlayer.inventory[item].name == "Life Crystal" && args.Player.TPlayer.statLifeMax <= 380)
				{
					args.TPlayer.statLife += 20;
					args.TPlayer.statLifeMax += 20;
					args.Player.PlayerData.maxHealth += 20;
				}
				else if (args.TPlayer.inventory[item].name == "Life Fruit" && args.Player.TPlayer.statLifeMax >= 400 && args.Player.TPlayer.statLifeMax <= 495)
				{
					args.TPlayer.statLife += 5;
					args.TPlayer.statLifeMax += 5;
					args.Player.PlayerData.maxHealth += 5;
				}
			}

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
				NetMessage.SendData((int)PacketTypes.PlayerUpdate, -1, -1, "", args.Player.Index);
				return true;
			}



			NetMessage.SendData((int)PacketTypes.PlayerUpdate, -1, args.Player.Index, "", args.Player.Index);
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

			if (OnNewProjectile(ident, pos, vel, knockback, dmg, owner, type, index))
				return true;

			if (index > Main.maxProjectiles || index < 0)
			{
				args.Player.RemoveProjectile(ident, owner);
				return true;
			}

			if (TShock.ProjectileBans.ProjectileIsBanned(type, args.Player))
			{
				args.Player.Disable("Player does not have permission to create that projectile.", DisableFlags.WriteToLogAndConsole);
				args.Player.SendErrorMessage("You do not have permission to create that projectile.");
				args.Player.RemoveProjectile(ident, owner);
				return true;
			}

			if (dmg > TShock.Config.MaxProjDamage && !args.Player.HasPermission(Permissions.ignoredamagecap))
			{
				args.Player.Disable(String.Format("Projectile damage is higher than {0}.", TShock.Config.MaxProjDamage), DisableFlags.WriteToLogAndConsole);
				args.Player.RemoveProjectile(ident, owner);
				return true;
			}

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.RemoveProjectile(ident, owner);
				return true;
			}

			bool hasPermission = !TShock.CheckProjectilePermission(args.Player, index, type);
			if (!TShock.Config.IgnoreProjUpdate && !hasPermission && !args.Player.HasPermission(Permissions.ignoreprojectiledetection))
			{
				if (type == ProjectileID.BlowupSmokeMoonlord
					|| type == ProjectileID.PhantasmalEye
					|| type == ProjectileID.CultistBossIceMist
					|| (type >= ProjectileID.MoonlordBullet && type <= ProjectileID.MoonlordTurretLaser)
					|| type == ProjectileID.DeathLaser || type == ProjectileID.Landmine
					|| type == ProjectileID.BulletDeadeye || type == ProjectileID.BoulderStaffOfEarth
					|| (type > ProjectileID.ConfettiMelee && type < ProjectileID.SpiritHeal)
					|| (type >= ProjectileID.FlamingWood && type <= ProjectileID.GreekFire3)
					|| (type >= ProjectileID.PineNeedleHostile && type <= ProjectileID.Spike)
					|| (type >= ProjectileID.MartianTurretBolt && type <= ProjectileID.RayGunnerLaser)
					|| type == ProjectileID.CultistBossLightningOrb)
				{
					TShock.Log.Debug("Certain projectiles have been ignored for cheat detection.");
				}
				else
				{
					args.Player.Disable(String.Format("Does not have projectile permission to update projectile. ({0})", type), DisableFlags.WriteToLogAndConsole);
					args.Player.RemoveProjectile(ident, owner);
				}
				return true;
			}

			if (args.Player.ProjectileThreshold >= TShock.Config.ProjectileThreshold)
			{
				args.Player.Disable("Reached projectile update threshold.", DisableFlags.WriteToLogAndConsole);
				args.Player.RemoveProjectile(ident, owner);
				return true;
			}

			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
			{
				args.Player.RemoveProjectile(ident, owner);
				return true;
			}

			if (!args.Player.HasPermission(Permissions.ignoreprojectiledetection))
			{
				if (type == ProjectileID.CrystalShard && TShock.Config.ProjIgnoreShrapnel) // Ignore crystal shards
				{
					TShock.Log.Debug("Ignoring shrapnel per config..");
				}
				else if (!Main.projectile[index].active)
				{
					args.Player.ProjectileThreshold++; // Creating new projectile
				}
			}

			if (hasPermission &&
				(type == ProjectileID.Bomb
				|| type == ProjectileID.Dynamite
				|| type == ProjectileID.StickyBomb
				|| type == ProjectileID.StickyDynamite))
			{
				//  Denotes that the player has recently set a fuse - used for cheat detection.
				args.Player.RecentFuse = 10;
				//return true;
			}

			return false;
		}

		private static bool HandleProjectileKill(GetDataHandlerArgs args)
		{
			var ident = args.Data.ReadInt16();
			var owner = args.Data.ReadInt8();
			owner = (byte)args.Player.Index;
			var index = TShock.Utils.SearchProjectile(ident, owner);

			if (index > Main.maxProjectiles || index < 0)
			{
				return false;
			}

			var type = Main.projectile[index].type;

			// Players can no longer destroy projectiles that are not theirs as of 1.1.2
			/*if (args.Player.Index != Main.projectile[index].owner && type != 102 && type != 100 && !TShock.Config.IgnoreProjKill) // workaround for skeletron prime projectiles
			{
				args.Player.Disable(String.Format("Owner ({0}) and player ID ({1}) does not match to kill projectile of type: {3}", Main.projectile[index].owner, args.Player.Index, type));
				args.Player.RemoveProjectile(ident, owner);
				return true;
			}*/

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.RemoveProjectile(ident, owner);
				return true;
			}

			if (TShock.CheckProjectilePermission(args.Player, index, type) && type != 102 && type != 100 && !TShock.Config.IgnoreProjKill)
			{
				args.Player.Disable("Does not have projectile permission to kill projectile.", DisableFlags.WriteToLogAndConsole);
				args.Player.RemoveProjectile(ident, owner);
				return true;
			}

			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
			{
				args.Player.RemoveProjectile(ident, owner);
				return true;
			}

			args.Player.LastKilledProjectile = type;

			return false;
		}

		private static bool HandlePlayerKillMe(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt8();
			var direction = (byte)(args.Data.ReadInt8() - 1);
			var dmg = args.Data.ReadInt16();
			var pvp = args.Data.ReadInt8() == 0;
			var text = args.Data.ReadString();
			if (dmg > 20000) //Abnormal values have the potential to cause infinite loops in the server.
			{
				TShock.Utils.ForceKick(args.Player, "Crash Exploit Attempt", true);
				TShock.Log.ConsoleError("Death Exploit Attempt: Damage {0}", dmg);
				return false;
			}

			if (id >= Main.maxPlayers)
			{
				return true;
			}

			if (OnKillMe(id, direction, dmg, pvp))
				return true;

			if (text.Length > 500)
			{
				TShock.Utils.Kick(TShock.Players[id], "Crash attempt", true);
				return true;
			}

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
				if (TShock.CharacterDB.RemovePlayer(args.Player.User.ID))
				{
					TShock.CharacterDB.SeedInitialData(args.Player.User);
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

			if (OnLiquidSet(tileX, tileY, amount, type))
				return true;

			if (!TShock.Utils.TilePlacementValid(tileX, tileY) || (args.Player.Dead && TShock.Config.PreventDeadModification))
				return true;

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendTileSquare(tileX, tileY, 1);
				return true;
			}

			if (args.Player.TileLiquidThreshold >= TShock.Config.TileLiquidThreshold)
			{
				args.Player.Disable("Reached TileLiquid threshold.", DisableFlags.WriteToLogAndConsole);
				args.Player.SendTileSquare(tileX, tileY, 1);
				return true;
			}

			if (!args.Player.HasPermission(Permissions.ignoreliquidsetdetection))
			{
				args.Player.TileLiquidThreshold++;
			}
			if (amount != 0)
			{
				int bucket = -1;
				if (args.TPlayer.inventory[args.TPlayer.selectedItem].type == 205)
				{
					bucket = 0;
				}
				else if (args.TPlayer.inventory[args.TPlayer.selectedItem].type == 206)
				{
					bucket = 1;
				}
				else if (args.TPlayer.inventory[args.TPlayer.selectedItem].type == 207)
				{
					bucket = 2;
				}
				else if (args.TPlayer.inventory[args.TPlayer.selectedItem].type == 1128)
				{
					bucket = 3;
				}
				else if (args.TPlayer.inventory[args.TPlayer.selectedItem].type == 3031 ||
					args.TPlayer.inventory[args.TPlayer.selectedItem].type == 3032)
				{
					bucket = 4;
				}

				if (type == 1 && !(bucket == 2 || bucket == 0))
				{
					args.Player.SendErrorMessage("You do not have permission to perform this action.");
					args.Player.Disable("Spreading lava without holding a lava bucket", DisableFlags.WriteToLogAndConsole);
					args.Player.SendTileSquare(tileX, tileY, 1);
					return true;
				}

				if (type == 1 && TShock.Itembans.ItemIsBanned("Lava Bucket", args.Player))
				{
					args.Player.SendErrorMessage("You do not have permission to perform this action.");
					args.Player.Disable("Using banned lava bucket without permissions", DisableFlags.WriteToLogAndConsole);
					args.Player.SendTileSquare(tileX, tileY, 1);
					return true;
				}

				if (type == 0 && !(bucket == 1 || bucket == 0 || bucket == 4))
				{
					args.Player.SendErrorMessage("You do not have permission to perform this action.");
					args.Player.Disable("Spreading water without holding a water bucket", DisableFlags.WriteToLogAndConsole);
					args.Player.SendTileSquare(tileX, tileY, 1);
					return true;
				}

				if (type == 0 && TShock.Itembans.ItemIsBanned("Water Bucket", args.Player))
				{
					args.Player.SendErrorMessage("You do not have permission to perform this action.");
					args.Player.Disable("Using banned water bucket without permissions", DisableFlags.WriteToLogAndConsole);
					args.Player.SendTileSquare(tileX, tileY, 1);
					return true;
				}

				if (type == 2 && !(bucket == 3 || bucket == 0))
				{
					args.Player.SendErrorMessage("You do not have permission to perform this action.");
					args.Player.Disable("Spreading honey without holding a honey bucket", DisableFlags.WriteToLogAndConsole);
					args.Player.SendTileSquare(tileX, tileY, 1);
					return true;
				}

				if (type == 2 && TShock.Itembans.ItemIsBanned("Honey Bucket", args.Player))
				{
					args.Player.SendErrorMessage("You do not have permission to perform this action.");
					args.Player.Disable("Using banned honey bucket without permissions", DisableFlags.WriteToLogAndConsole);
					args.Player.SendTileSquare(tileX, tileY, 1);
					return true;
				}
			}

			if (TShock.CheckTilePermission(args.Player, tileX, tileY))
			{
				args.Player.SendTileSquare(tileX, tileY, 1);
				return true;
			}

			if (TShock.CheckRangePermission(args.Player, tileX, tileY, 16))
			{
				args.Player.SendTileSquare(tileX, tileY, 1);
				return true;
			}

			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
			{
				args.Player.SendTileSquare(tileX, tileY, 1);
				return true;
			}

			return false;
		}

		private static bool HandleTileKill(GetDataHandlerArgs args)
		{
			int flag = args.Data.ReadByte();
			int tileX = args.Data.ReadInt16();
			int tileY = args.Data.ReadInt16();
			args.Data.ReadInt16(); // Ignore style

			if (OnTileKill(tileX, tileY))
				return true;
			if (!TShock.Utils.TilePlacementValid(tileX, tileY) || (args.Player.Dead && TShock.Config.PreventDeadModification))
				return true;

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendTileSquare(tileX, tileY, 3);
				return true;
			}

			if (flag != 0
				&& Main.tile[tileX, tileY].type != TileID.Containers
				&& Main.tile[tileX, tileY].type != TileID.Dressers
				&& (!TShock.Utils.MaxChests() && Main.tile[tileX, tileY].type != TileID.Dirt)) //Chest
			{
				args.Player.SendTileSquare(tileX, tileY, 3);
				return true;
			}

			if (flag == 2) //place dresser
			{
				if ((TShock.Utils.TilePlacementValid(tileX, tileY + 1) && Main.tile[tileX, tileY + 1].type == TileID.Teleporter) ||
					(TShock.Utils.TilePlacementValid(tileX + 1, tileY + 1) && Main.tile[tileX + 1, tileY + 1].type == TileID.Teleporter))
				{
					//Prevent a dresser from being placed on a teleporter, as this can cause client and server crashes.
					args.Player.SendTileSquare(tileX, tileY, 3);
					return true;
				}
			}

			if (TShock.CheckTilePermission(args.Player, tileX, tileY))
			{
				args.Player.SendTileSquare(tileX, tileY, 3);
				return true;
			}

			if (TShock.CheckRangePermission(args.Player, tileX, tileY))
			{
				args.Player.SendTileSquare(tileX, tileY, 3);
				return true;
			}
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

			if (TShock.CheckIgnores(args.Player))
			{
				return true;
			}

			if (TShock.CheckRangePermission(args.Player, x, y))
			{
				return true;
			}

			if (TShock.CheckTilePermission(args.Player, x, y) && TShock.Config.RegionProtectChests)
			{
				return true;
			}

			int id = Chest.FindChest(x, y);
			args.Player.ActiveChest = id;

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

			if (OnChestItemChange(id, slot, stacks, prefix, type))
				return true;

			if (args.TPlayer.chest != id)
			{
				return false;
			}

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendData(PacketTypes.ChestItem, "", id, slot);
				return true;
			}

			Item item = new Item();
			item.netDefaults(type);
			if (stacks > item.maxStack || TShock.Itembans.ItemIsBanned(item.name, args.Player))
			{
				return false;
			}

			if (TShock.CheckTilePermission(args.Player, Main.chest[id].x, Main.chest[id].y) && TShock.Config.RegionProtectChests)
			{
				return false;
			}

			if (TShock.CheckRangePermission(args.Player, Main.chest[id].x, Main.chest[id].y))
			{
				return false;
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

			if (OnUpdateNPCHome(id, x, y, homeless))
				return true;

			if (!args.Player.HasPermission(Permissions.movenpc))
			{
				args.Player.SendErrorMessage("You do not have permission to relocate NPCs.");
				args.Player.SendData(PacketTypes.UpdateNPCHome, "", id, Main.npc[id].homeTileX, Main.npc[id].homeTileY,
									 Convert.ToByte(Main.npc[id].homeless));
				return true;
			}

			if (TShock.CheckTilePermission(args.Player, x, y))
			{
				args.Player.SendErrorMessage("You do not have access to modify this area.");
				args.Player.SendData(PacketTypes.UpdateNPCHome, "", id, Main.npc[id].homeTileX, Main.npc[id].homeTileY,
									 Convert.ToByte(Main.npc[id].homeless));
				return true;
			}

			//removed until NPC Home packet actually sends their home coords.
			/*if (TShock.CheckRangePermission(args.Player, x, y))
			{
				args.Player.SendData(PacketTypes.UpdateNPCHome, "", id, Main.npc[id].homeTileX, Main.npc[id].homeTileY,
									 Convert.ToByte(Main.npc[id].homeless));
				return true;
			}*/
			return false;
		}

		private static bool HandlePlayerAddBuff(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt8();
			var type = args.Data.ReadInt8();
			var time = args.Data.ReadInt16();

			if (OnPlayerBuff(id, type, time))
				return true;

			if (TShock.Players[id] == null)
				return false;

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendData(PacketTypes.PlayerAddBuff, "", id);
				return true;
			}

			if (id >= Main.maxPlayers)
			{
				args.Player.SendData(PacketTypes.PlayerAddBuff, "", id);
				return true;
			}

			if (!TShock.Players[id].TPlayer.hostile || !Main.pvpBuff[type])
			{
				args.Player.SendData(PacketTypes.PlayerAddBuff, "", id);
				return true;
			}
			if (TShock.CheckRangePermission(args.Player, TShock.Players[id].TileX, TShock.Players[id].TileY, 50))
			{
				args.Player.SendData(PacketTypes.PlayerAddBuff, "", id);
				return true;
			}
			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
			{
				args.Player.SendData(PacketTypes.PlayerAddBuff, "", id);
				return true;
			}

			if (WhitelistBuffMaxTime[type] > 0 && time <= WhitelistBuffMaxTime[type])
			{
				return false;
			}

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

			if (OnItemDrop(id, pos, vel, stacks, prefix, noDelay, type))
				return true;

			// player is attempting to crash clients
			if (type < -48 || type >= Main.maxItemTypes)
			{
				args.Player.SendData(PacketTypes.ItemDrop, "", id);
				return true;
			}

			if (prefix > Item.maxPrefixes) //make sure the prefix is a legit value
			{
				args.Player.SendData(PacketTypes.ItemDrop, "", id);
				return true;
			}

			if (type == 0) //Item removed, let client do this to prevent item duplication client side (but only if it passed the range check)
			{
				if (TShock.CheckRangePermission(args.Player, (int)(Main.item[id].position.X / 16f), (int)(Main.item[id].position.Y / 16f)))
				{
					args.Player.SendData(PacketTypes.ItemDrop, "", id);
					return true;
				}

				return false;
			}

			if (TShock.CheckRangePermission(args.Player, (int)(pos.X / 16f), (int)(pos.Y / 16f)))
			{
				args.Player.SendData(PacketTypes.ItemDrop, "", id);
				return true;
			}

			if (Main.item[id].active && Main.item[id].netID != type) //stop the client from changing the item type of a drop but only if the client isn't picking up the item
			{
				args.Player.SendData(PacketTypes.ItemDrop, "", id);
				return true;
			}

			Item item = new Item();
			item.netDefaults(type);
			if ((stacks > item.maxStack || stacks <= 0) || (TShock.Itembans.ItemIsBanned(item.name, args.Player) && !args.Player.HasPermission(Permissions.allowdroppingbanneditems)))
			{
				args.Player.SendData(PacketTypes.ItemDrop, "", id);
				return true;
			}
			if ((Main.ServerSideCharacter) && (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - args.Player.LoginMS < TShock.ServerSideCharacterConfig.LogonDiscardThreshold))
			{
				//Player is probably trying to sneak items onto the server in their hands!!!
				TShock.Log.ConsoleInfo("Player {0} tried to sneak {1} onto the server!", args.Player.Name, item.name);
				args.Player.SendData(PacketTypes.ItemDrop, "", id);
				return true;

			}
			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendData(PacketTypes.ItemDrop, "", id);
				return true;
			}

			return false;
		}

		private static bool HandleUpdateItemDrop(GetDataHandlerArgs args)
		{
			var itemID = args.Data.ReadInt16();
			var position = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());
			var velocity = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());
			var stacks = args.Data.ReadInt16();
			var prefix = args.Data.ReadInt8();
			var noDelay = args.Data.ReadInt8() == 1;
			var itemNetID = args.Data.ReadInt16();

			if (OnItemDrop(itemID, position, velocity, stacks, prefix, noDelay, itemNetID))
				return true;

			// Invalid Net IDs can cause client crashes
			if (itemNetID < -48 || itemNetID >= Main.maxItemTypes)
			{
				args.Player.SendData(PacketTypes.ItemDrop, "", itemID);
				return true;
			}

			if (prefix > Item.maxPrefixes) // Make sure the prefix is a legit value
			{
				args.Player.SendData(PacketTypes.ItemDrop, "", itemID);
				return true;
			}

			if (itemNetID == 0) //Item removed, let client do this to prevent item duplication client side (but only if it passed the range check)
			{
				if (TShock.CheckRangePermission(args.Player, (int)(Main.item[itemID].position.X / 16f), (int)(Main.item[itemID].position.Y / 16f)))
				{
					args.Player.SendData(PacketTypes.ItemDrop, "", itemID);
					return true;
				}

				return false;
			}

			if (TShock.CheckRangePermission(args.Player, (int)(position.X / 16f), (int)(position.Y / 16f)))
			{
				args.Player.SendData(PacketTypes.ItemDrop, "", itemID);
				return true;
			}

			if (Main.item[itemID].active && Main.item[itemID].netID != itemNetID) //stop the client from changing the item type of a drop but only if the client isn't picking up the item
			{
				args.Player.SendData(PacketTypes.ItemDrop, "", itemID);
				return true;
			}

			Item item = new Item();
			item.netDefaults(itemNetID);
			if ((stacks > item.maxStack || stacks <= 0) || (TShock.Itembans.ItemIsBanned(item.name, args.Player) && !args.Player.HasPermission(Permissions.allowdroppingbanneditems)))
			{
				args.Player.SendData(PacketTypes.ItemDrop, "", itemID);
				return true;
			}
			if ((Main.ServerSideCharacter) && (DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond - args.Player.LoginMS < TShock.ServerSideCharacterConfig.LogonDiscardThreshold))
			{
				//Player is probably trying to sneak items onto the server in their hands!!!
				TShock.Log.ConsoleInfo("Player {0} tried to sneak {1} onto the server!", args.Player.Name, item.name);
				args.Player.SendData(PacketTypes.ItemDrop, "", itemID);
				return true;

			}
			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendData(PacketTypes.ItemDrop, "", itemID);
				return true;
			}

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

		private static bool HandlePlayerDamage(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt8();
			var direction = (byte)(args.Data.ReadInt8() - 1);
			var dmg = args.Data.ReadInt16();
			args.Data.ReadString(); // don't store damage text
			var bits = (BitsByte)args.Data.ReadInt8();
			var pvp = bits[0];
			var crit = bits[1];

			if (OnPlayerDamage(id, direction, dmg, pvp, crit))
				return true;

			if (id >= Main.maxPlayers || TShock.Players[id] == null)
			{
				return true;
			}

			if (dmg > TShock.Config.MaxDamage && !args.Player.HasPermission(Permissions.ignoredamagecap) && id != args.Player.Index)
			{
				if (TShock.Config.KickOnDamageThresholdBroken)
				{
					TShock.Utils.Kick(args.Player, string.Format("Player damage exceeded {0}.", TShock.Config.MaxDamage));
					return true;
				}
				else
				{
					args.Player.Disable(String.Format("Player damage exceeded {0}.", TShock.Config.MaxDamage), DisableFlags.WriteToLogAndConsole);
				}
				args.Player.SendData(PacketTypes.PlayerHp, "", id);
				args.Player.SendData(PacketTypes.PlayerUpdate, "", id);
				return true;
			}

			if (!TShock.Players[id].TPlayer.hostile && pvp && id != args.Player.Index)
			{
				args.Player.SendData(PacketTypes.PlayerHp, "", id);
				args.Player.SendData(PacketTypes.PlayerUpdate, "", id);
				return true;
			}

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendData(PacketTypes.PlayerHp, "", id);
				args.Player.SendData(PacketTypes.PlayerUpdate, "", id);
				return true;
			}

			if (TShock.CheckRangePermission(args.Player, TShock.Players[id].TileX, TShock.Players[id].TileY, 100))
			{
				args.Player.SendData(PacketTypes.PlayerHp, "", id);
				args.Player.SendData(PacketTypes.PlayerUpdate, "", id);
				return true;
			}

			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
			{
				args.Player.SendData(PacketTypes.PlayerHp, "", id);
				args.Player.SendData(PacketTypes.PlayerUpdate, "", id);
				return true;
			}

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

			if (OnNPCStrike(id, direction, dmg, knockback, crit))
				return true;

			if (Main.npc[id] == null)
				return true;

			if (dmg > TShock.Config.MaxDamage && !args.Player.HasPermission(Permissions.ignoredamagecap))
			{
				if (TShock.Config.KickOnDamageThresholdBroken)
				{
					TShock.Utils.Kick(args.Player, string.Format("NPC damage exceeded {0}.", TShock.Config.MaxDamage));
					return true;
				}
				else
				{
					args.Player.Disable(String.Format("NPC damage exceeded {0}.", TShock.Config.MaxDamage), DisableFlags.WriteToLogAndConsole);
				}
				args.Player.SendData(PacketTypes.NpcUpdate, "", id);
				return true;
			}

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendData(PacketTypes.NpcUpdate, "", id);
				return true;
			}

			if (Main.npc[id].townNPC && !args.Player.HasPermission(Permissions.hurttownnpc))
			{
				args.Player.SendErrorMessage("You do not have permission to hurt this NPC.");
				args.Player.SendData(PacketTypes.NpcUpdate, "", id);
				return true;
			}

			if (TShock.Config.RangeChecks &&
				TShock.CheckRangePermission(args.Player, (int)(Main.npc[id].position.X / 16f), (int)(Main.npc[id].position.Y / 16f), 128))
			{
				args.Player.SendData(PacketTypes.NpcUpdate, "", id);
				return true;
			}

			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
			{
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
			if (OnPlayerAnimation())
				return true;

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendData(PacketTypes.PlayerAnimation, "", args.Player.Index);
				return true;
			}

			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
			{
				args.Player.SendData(PacketTypes.PlayerAnimation, "", args.Player.Index);
				return true;
			}

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

				/*if (TShock.Itembans.ItemBans.Any(s =>
				{
					Item item = new Item();
					item.SetDefaults(s.Name);
					return item.buffType == buff;
				}))
				{
					buff = 0;
				}*/

				if (buff == 10 && TShock.Config.DisableInvisPvP && args.TPlayer.hostile)
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


			NetMessage.SendData((int)PacketTypes.PlayerBuff, -1, args.Player.Index, "", args.Player.Index);
			return true;
		}

		private static bool HandleSpawnBoss(GetDataHandlerArgs args)
		{
			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
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
					boss = String.Format("the {0}", npc.name);
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
			if (OnPaintTile(x, y, t))
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

			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000 ||
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

			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000 ||
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
			args.Data.ReadByte(); // Ignore toolmode

			List<Point> points = Utils.Instance.GetMassWireOperationRange(
				new Point(startX, startY),
				new Point(endX, endY),
				args.Player.TPlayer.direction == 1);

			int x;
			int y;
			foreach (Point p in points)
			{
				/* Perform similar checks to TileKill
				 * The server-side nature of this packet removes the need to use SendTileSquare
				 * Range checks are currently ignored here as the items that send this seem to have infinite range */

				x = p.X;
				y = p.Y;

				if (!TShock.Utils.TilePlacementValid(x, y) || (args.Player.Dead && TShock.Config.PreventDeadModification))
					return true;

				if (TShock.CheckIgnores(args.Player))
					return true;

				if (TShock.CheckTilePermission(args.Player, x, y))
					return true;
			}

			return false;
		}

		/// <summary>
		/// For use with a ToggleGemLock event
		/// </summary>
		public class GemLockToggleEventArgs : HandledEventArgs
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
			/// On status
			/// </summary>
			public bool On { get; set; }
		}

		/// <summary>
		/// GemLockToggle - Called when a gem lock is switched
		/// </summary>
		public static HandlerList<GemLockToggleEventArgs> GemLockToggle;

		private static bool OnGemLockToggle(Int32 x, Int32 y, bool on)
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
			var x = (int)args.Data.ReadInt16();
			var y = (int)args.Data.ReadInt16();
			var on = args.Data.ReadBoolean();

			if (x < 0 || y < 0 || x >= Main.maxTilesX || y >= Main.maxTilesY)
			{
				return true;
			}

			if (OnGemLockToggle(x, y, on))
			{
				return true;
			}

			if (!TShock.Config.RegionProtectGemLocks)
			{
				return false;
			}

			if (!TShock.Utils.TilePlacementValid(x, y) || (args.Player.Dead && TShock.Config.PreventDeadModification))
			{
				return true;
			}

			if (TShock.CheckIgnores(args.Player))
			{
				return true;
			}

			if (TShock.CheckTilePermission(args.Player, x, y))
			{
				return true;
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
				NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, "", npcID);
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
				NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, "", npcIndex);
				return true;
			}

			if (projectile.type != ProjectileID.PortalGunGate)
			{
				NetMessage.SendData((int)PacketTypes.NpcUpdate, -1, -1, "", npcIndex);
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
			var type = args.Data.ReadByte();

			if (TShock.TileBans.TileIsBanned((short)TileID.LogicSensor, args.Player))
			{
				args.Player.SendTileSquare(x, y, 1);
				args.Player.SendErrorMessage("You do not have permission to place Logic Sensors.");
				return true;
			}

			if (TShock.CheckIgnores(args.Player))
			{
				return true;
			}

			if (TShock.CheckTilePermission(args.Player, x, y))
			{
				return true;
			}

			if (TShock.CheckRangePermission(args.Player, x, y))
			{
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

			if (TShock.CheckIgnores(args.Player))
			{
				NetMessage.SendData((int)PacketTypes.UpdateTileEntity, -1, -1, "", itemFrame.ID, 0, 1);
				return true;
			}

			if (TShock.CheckTilePermission(args.Player, x, y))
			{
				NetMessage.SendData((int)PacketTypes.UpdateTileEntity, -1, -1, "", itemFrame.ID, 0, 1);
				return true;
			}

			if (TShock.CheckRangePermission(args.Player, x, y))
			{
				NetMessage.SendData((int)PacketTypes.UpdateTileEntity, -1, -1, "", itemFrame.ID, 0, 1);
				return true;
			}

			if (itemFrame.item?.netID == args.TPlayer.inventory[args.TPlayer.selectedItem]?.netID)
			{
				NetMessage.SendData((int)PacketTypes.UpdateTileEntity, -1, -1, "", itemFrame.ID, 0, 1);
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
	}
}
