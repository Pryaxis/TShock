/*
TShock, a server mod for Terraria
Copyright (C) 2011 The TShock Team

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
using System.IO;
using System.IO.Streams;
using System.Linq;
using System.Text;
using Terraria;
using TShockAPI.Net;

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
			public byte Type { get; set; }
			/// <summary>
			/// The EditType.
			/// (KillTile = 0, PlaceTile = 1, KillWall = 2, PlaceWall = 3, KillTileNoItem = 4, PlaceWire = 5, KillWire = 6)
			/// </summary>
			public byte EditType { get; set; }
		} 

		/// <summary>
		/// TileEdit - called when a tile is placed or destroyed
		/// </summary>
		public static HandlerList<TileEditEventArgs> TileEdit;
		private static bool OnTileEdit(int x, int y, byte type, byte editType)
		{
			if (TileEdit == null)
				return false;

			var args = new TileEditEventArgs
			{
				X = x,
				Y = y,
				Type = type,
				EditType = editType
			};
			TileEdit.Invoke(null, args);
			return args.Handled;
		}

		public class TogglePvpEventArgs : HandledEventArgs
		{
			public byte PlayerId { get; set; }
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

		
		public class PlayerSlotEventArgs : HandledEventArgs
		{
			public byte PlayerId { get; set; }
			public byte Slot { get; set; }
			public byte Stack { get; set; }
			public byte Prefix { get; set; }
			public short Type { get; set; }
		}
		/// <summary>
		/// PlayerSlot - called at a PlayerSlot event
		/// </summary>
		public static HandlerList<PlayerSlotEventArgs> PlayerSlot;
		private static bool OnPlayerSlot(byte _plr, byte _slot, byte _stack, byte _prefix, short _type)
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

		
		public class PlayerHPEventArgs : HandledEventArgs
		{
			public byte PlayerId { get; set; }
			public short Current { get; set; }
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
			public byte PlayerId { get; set; }
			public byte Hair { get; set; }
			public bool Male { get; set; }
			public byte Difficulty { get; set; }
			public string Name { get; set; }
		}
		/// <summary>
		/// PlayerInfo - called at a PlayerInfo event
		/// If this is cancelled, the server will ForceKick the player. If this should be changed in the future, let someone know.
		/// </summary>
		public static HandlerList<PlayerInfoEventArgs> PlayerInfo;

		private static bool OnPlayerInfo(byte _plrid, byte _hair, bool _male, byte _difficulty, string _name)
		{
			if (PlayerInfo == null)
				return false;

			var args = new PlayerInfoEventArgs
			{
				PlayerId = _plrid,
				Hair = _hair,
				Male = _male,
				Difficulty = _difficulty,
				Name = _name,
			};
			PlayerInfo.Invoke(null, args);
			return args.Handled;
		}

		public class TileKillEventArgs : HandledEventArgs
		{
			public int TileX { get; set; }
			public int TileY { get; set; }
		}
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

		public class KillMeEventArgs : HandledEventArgs
		{
			public byte PlayerId { get; set; }
			public byte Direction { get; set; }
			public short Damage { get; set; }
			public bool Pvp { get; set; }
		}
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

		public class PlayerUpdateEventArgs : HandledEventArgs
		{
			public byte PlayerId { get; set; }
			public byte Control { get; set; }
			public byte Item { get; set; }
			public Vector2 Position { get; set; }
			public Vector2 Velocity { get; set; }
		}
		public static HandlerList<PlayerUpdateEventArgs> PlayerUpdate;

		private static bool OnPlayerUpdate(byte player, byte control, byte item, Vector2 position, Vector2 velocity)
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
			};
			PlayerUpdate.Invoke(null, args);
			return args.Handled;
		}

		public class SendTileSquareEventArgs : HandledEventArgs
		{
			public short Size { get; set; }
			public int TileX { get; set; }
			public int TileY { get; set; }
		}
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

		public class NewProjectileEventArgs : HandledEventArgs
		{
			public short Identity { get; set; }
			public Vector2 Position { get; set; }
			public Vector2 Velocity { get; set; }
			public float Knockback { get; set; }
			public short Damage { get; set; }
			public byte Owner { get; set; }
			public byte Type { get; set; }
			public int Index { get; set; }
		}
		public static HandlerList<NewProjectileEventArgs> NewProjectile;

		private static bool OnNewProjectile(short ident, Vector2 pos, Vector2 vel, float knockback, short dmg, byte owner, byte type, int index)
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

		public class LiquidSetEventArgs : HandledEventArgs
		{
			public int TileX { get; set; }
			public int TileY { get; set; }
			public byte Liquid { get; set;}
			public bool Lava { get; set; }
		}
		public static HandlerList<LiquidSetEventArgs> LiquidSet;

		private static bool OnLiquidSet(int tilex, int tiley, byte liquid, bool lava)
		{
			if (LiquidSet == null)
				return false;

			var args = new LiquidSetEventArgs
			{
				TileX = tilex,
				TileY = tiley,
				Liquid = liquid,
				Lava = lava,
			};
			LiquidSet.Invoke(null, args);
			return args.Handled;
		}

		public class SpawnEventArgs : HandledEventArgs
		{
			public byte Player { get; set; }
			public int SpawnX { get; set; }
			public int SpawnY { get; set; }
		}
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

		public class ChestOpenEventArgs : HandledEventArgs
		{
			public int X { get; set; }
			public int Y { get; set; }
		}
		public static HandlerList<ChestOpenEventArgs> ChestOpen;

		private static bool OnChestOpen(int x, int y)
		{
			if (ChestOpen == null)
				return false;

			var args = new ChestOpenEventArgs
			{
				X = x,
				Y = y,
			};
			ChestOpen.Invoke(null, args);
			return args.Handled;
		}

		public class ChestItemEventArgs : HandledEventArgs
		{
			public short ID { get; set; }
			public byte Slot { get; set; }
			public byte Stacks { get; set; }
			public byte Prefix { get; set; }
			public short Type { get; set; }
		}
		public static HandlerList<ChestItemEventArgs> ChestItemChange;

		private static bool OnChestItemChange(short id, byte slot, byte stacks, byte prefix, short type)
		{
			if (PlayerSpawn == null)
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

		public class SignEventArgs : HandledEventArgs
		{
			public short ID { get; set; }
			public int X { get; set; }
			public int Y { get; set; }
		}
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

		public class NPCHomeChangeEventArgs : HandledEventArgs
		{
			public short ID { get; set; }
			public short X { get; set; }
			public short Y { get; set; }
			public byte Homeless { get; set; }
		}
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

		public class PlayerBuffEventArgs : HandledEventArgs
		{
			public byte ID { get; set; }
			public byte Type { get; set; }
			public short Time { get; set; }
		}
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

		public class ItemDropEventArgs : HandledEventArgs
		{
			public short ID { get; set; }
			public Vector2 Position { get; set; }
			public Vector2 Velocity { get; set; }
			public byte Stacks { get; set; }
			public byte Prefix { get; set; }
			public short Type { get; set; }
		}
		public static HandlerList<ItemDropEventArgs> ItemDrop;

		private static bool OnItemDrop(short id, Vector2 pos, Vector2 vel, byte stacks, byte prefix, short type)
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
				Type = type,
			};
			ItemDrop.Invoke(null, args);
			return args.Handled;
		}

		public class PlayerDamageEventArgs : HandledEventArgs
		{
			public byte ID { get; set; }
			public byte Direction { get; set; }
			public short Damage { get; set; }
			public byte PVP { get; set; }
			public byte Critical { get; set; }
		}
		public static HandlerList<PlayerDamageEventArgs> PlayerDamage;

		private static bool OnPlayerDamage(byte id, byte dir, short dmg, byte pvp, byte crit)
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

		public class NPCStrikeEventArgs : HandledEventArgs
		{
			public byte ID { get; set; }
			public byte Direction { get; set; }
			public short Damage { get; set; }
			public byte PVP { get; set; }
			public byte Critical { get; set; }
		}
		public static HandlerList<NPCStrikeEventArgs> NPCStrike;

		private static bool OnNPCStrike(byte id, byte dir, short dmg, byte pvp, byte crit)
		{
			if (NPCStrike == null)
				return false;

			var args = new NPCStrikeEventArgs
			{
				ID = id,
				Direction = dir,
				Damage = dmg,
				PVP = pvp,
				Critical = crit,
			};
			NPCStrike.Invoke(null, args);
			return args.Handled;
		}

		public class NPCSpecialEventArgs : HandledEventArgs
		{
			public byte ID { get; set; }
			public byte Type { get; set; }
		}
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

		public class PlayerAnimationEventArgs : HandledEventArgs
		{
		}
		public static HandlerList<PlayerAnimationEventArgs> PlayerAnimation;

		private static bool OnPlayerAnimation()
		{
			if (PlayerAnimation == null)
				return false;

			var args = new PlayerAnimationEventArgs {};
			PlayerAnimation.Invoke(null, args);
			return args.Handled;
		}

		public class PlayerBuffUpdateEventArgs : HandledEventArgs
		{
			public byte ID { get; set; }
		}
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

		#endregion
		public static void InitGetDataHandler()
		{
			#region Blacklists

			WhitelistBuffMaxTime = new int[Main.maxBuffs];
			WhitelistBuffMaxTime[20] = 600;
			WhitelistBuffMaxTime[0x18] = 1200;
			WhitelistBuffMaxTime[0x1f] = 120;
			WhitelistBuffMaxTime[0x27] = 420;

			#endregion Blacklists

			GetDataHandlerDelegates = new Dictionary<PacketTypes, GetDataHandlerDelegate>
										{
											{PacketTypes.PlayerInfo, HandlePlayerInfo},
											{PacketTypes.PlayerUpdate, HandlePlayerUpdate},
											{PacketTypes.Tile, HandleTile},
											{PacketTypes.TileSendSquare, HandleSendTileSquare},
											{PacketTypes.ProjectileNew, HandleProjectileNew},
											{PacketTypes.TogglePvp, HandleTogglePvp},
											{PacketTypes.TileKill, HandleTileKill},
											{PacketTypes.PlayerKillMe, HandlePlayerKillMe},
											{PacketTypes.LiquidSet, HandleLiquidSet},
											{PacketTypes.PlayerSpawn, HandleSpawn},
											{PacketTypes.ChestGetContents, HandleChestOpen},
											{PacketTypes.ChestItem, HandleChestItem},
											{PacketTypes.SignNew, HandleSign},
											{PacketTypes.PlayerSlot, HandlePlayerSlot},
											{PacketTypes.TileGetSection, HandleGetSection},
											{PacketTypes.UpdateNPCHome, UpdateNPCHome},
											{PacketTypes.PlayerAddBuff, HandlePlayerBuff},
											{PacketTypes.ItemDrop, HandleItemDrop},
											{PacketTypes.PlayerHp, HandlePlayerHp},
											{PacketTypes.PlayerMana, HandlePlayerMana},
											{PacketTypes.PlayerDamage, HandlePlayerDamage},
											{PacketTypes.NpcStrike, HandleNpcStrike},
											{PacketTypes.NpcSpecial, HandleSpecial},
											{PacketTypes.PlayerAnimation, HandlePlayerAnimation},
											{PacketTypes.PlayerBuff, HandlePlayerBuffUpdate},
											{PacketTypes.PasswordSend, HandlePassword},
											{PacketTypes.ContinueConnecting2, HandleConnecting},
											{PacketTypes.ProjectileDestroy, HandleProjectileKill},
                                            {PacketTypes.SpawnBossorInvasion, HandleSpawnBoss}
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
					Log.Error(ex.ToString());
				}
			}
			return false;
		}

		private static bool HandlePlayerSlot(GetDataHandlerArgs args)
		{
			byte plr = args.Data.ReadInt8();
			byte slot = args.Data.ReadInt8();
			byte stack = args.Data.ReadInt8();
			byte prefix = args.Data.ReadInt8();
			short type = args.Data.ReadInt16();

			if (OnPlayerSlot(plr, slot, stack, prefix, type))
				return true;

			if (plr != args.Player.Index)
			{
				return true;
			}

			if (slot < 0 || slot > NetItem.maxNetInventory)
			{
				return true;
			}

			var item = new Item();
			item.netDefaults(type);
			item.Prefix(prefix);

			if (args.Player.IsLoggedIn)
			{
				args.Player.PlayerData.StoreSlot(slot, type, prefix, stack);
			}

			return false;
		}

		public static bool HandlePlayerHp(GetDataHandlerArgs args)
		{
			var plr = args.Data.ReadInt8();
			var cur = args.Data.ReadInt16();
			var max = args.Data.ReadInt16();

			if (OnPlayerHP(plr, cur, max))
				return true;

			if (args.Player.FirstMaxHP == 0)
				args.Player.FirstMaxHP = max;

			if (max > 400 && max > args.Player.FirstMaxHP)
			{
				TShock.Utils.ForceKick(args.Player, "Hacked Client Detected.");
				return false;
			}

			if (args.Player.IsLoggedIn)
			{
				args.Player.PlayerData.maxHealth = max;
			}

			return false;
		}

		private static bool HandlePlayerMana(GetDataHandlerArgs args)
		{
			var plr = args.Data.ReadInt8();
			var cur = args.Data.ReadInt16();
			var max = args.Data.ReadInt16();

			if (OnPlayerMana(plr, cur, max))
				return true;

			if (args.Player.FirstMaxMP == 0)
				args.Player.FirstMaxMP = max;

			if (max > 400 && max > args.Player.FirstMaxMP)
			{
				TShock.Utils.ForceKick(args.Player, "Hacked Client Detected.");
				return false;
			}

			return false;
		}

		private static bool HandlePlayerInfo(GetDataHandlerArgs args)
		{
			var playerid = args.Data.ReadInt8();
			var hair = args.Data.ReadInt8();
			var male = args.Data.ReadBoolean();
			args.Data.Position += 21;
			var difficulty = args.Data.ReadInt8();
			string name = Encoding.UTF8.GetString(args.Data.ReadBytes((int) (args.Data.Length - args.Data.Position - 1)));

			if (OnPlayerInfo(playerid, hair, male, difficulty, name))
			{
				TShock.Utils.ForceKick(args.Player, "A plugin cancelled the event.");
				return true;
			}

			/*if (!TShock.Utils.ValidString(name))
			{
				TShock.Utils.ForceKick(args.Player, "Unprintable character in name");
				return true;
			}*/
			if (name.Trim().Length == 0)
			{
				TShock.Utils.ForceKick(args.Player, "Empty Name.");
				return true;
			}
			var ban = TShock.Bans.GetBanByName(name);
			if (ban != null)
			{
				TShock.Utils.ForceKick(args.Player, string.Format("You are banned: {0}", ban.Reason));
				return true;
			}
			if (args.Player.ReceivedInfo)
			{
				return true;
			}
			if (TShock.Config.MediumcoreOnly && difficulty < 1)
			{
				TShock.Utils.ForceKick(args.Player, "Server is set to mediumcore and above characters only!");
				return true;
			}
			if (TShock.Config.HardcoreOnly && difficulty < 2)
			{
				TShock.Utils.ForceKick(args.Player, "Server is set to hardcore characters only!");
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
			if (user != null && !TShock.Config.DisableLoginBeforeJoin)
			{
				args.Player.RequiresPassword = true;
				NetMessage.SendData((int) PacketTypes.PasswordRequired, args.Player.Index);
				return true;
			}
			else if (!string.IsNullOrEmpty(TShock.Config.ServerPassword))
			{
				args.Player.RequiresPassword = true;
				NetMessage.SendData((int) PacketTypes.PasswordRequired, args.Player.Index);
				return true;
			}

			if (args.Player.State == 1)
				args.Player.State = 2;
			NetMessage.SendData((int) PacketTypes.WorldInfo, args.Player.Index);
			return true;
		}

		private static bool HandlePassword(GetDataHandlerArgs args)
		{
			if (!args.Player.RequiresPassword)
				return true;

			string password = Encoding.UTF8.GetString(args.Data.ReadBytes((int) (args.Data.Length - args.Data.Position - 1)));
			var user = TShock.Users.GetUserByName(args.Player.Name);
			if (user != null)
			{
				string encrPass = TShock.Utils.HashPassword(password);
				if (user.Password.ToUpper() == encrPass.ToUpper())
				{
				    args.Player.RequiresPassword = false;
				    args.Player.PlayerData = TShock.InventoryDB.GetPlayerData(args.Player, TShock.Users.GetUserID(args.Player.Name));

				    if (args.Player.State == 1)
				        args.Player.State = 2;
				    NetMessage.SendData((int) PacketTypes.WorldInfo, args.Player.Index);

				    var group = TShock.Utils.GetGroup(user.Group);

				    if (TShock.Config.ServerSideInventory)
				    {
				        if (group.HasPermission(Permissions.bypassinventorychecks))
				        {
				            args.Player.IgnoreActionsForClearingTrashCan = false;
				        }
				        else if (!TShock.CheckInventory(args.Player))
				        {
				            args.Player.SendMessage("Login Failed, Please fix the above errors then /login again.", Color.Cyan);
				            args.Player.IgnoreActionsForClearingTrashCan = true;
				            return true;
				        }
				    }

				    if (group.HasPermission(Permissions.ignorestackhackdetection))
				        args.Player.IgnoreActionsForCheating = "none";

				    if (group.HasPermission(Permissions.usebanneditem))
				        args.Player.IgnoreActionsForDisabledArmor = "none";

				    args.Player.Group = group;
				    args.Player.UserAccountName = args.Player.Name;
				    args.Player.UserID = TShock.Users.GetUserID(args.Player.UserAccountName);
				    args.Player.IsLoggedIn = true;
				    args.Player.IgnoreActionsForInventory = "none";

				    if (!args.Player.IgnoreActionsForClearingTrashCan)
                    {
				        args.Player.PlayerData.CopyInventory(args.Player);
				        TShock.InventoryDB.InsertPlayerData(args.Player);
			        }
			        args.Player.SendMessage("Authenticated as " + args.Player.Name + " successfully.", Color.LimeGreen);
					Log.ConsoleInfo(args.Player.Name + " authenticated successfully as user: " + args.Player.Name);
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
					NetMessage.SendData((int) PacketTypes.WorldInfo, args.Player.Index);
					return true;
				}
				TShock.Utils.ForceKick(args.Player, "Incorrect Server Password");
				return true;
			}

			TShock.Utils.ForceKick(args.Player, "Bad Password Attempt");
			return true;
		}

		private static bool HandleGetSection(GetDataHandlerArgs args)
		{
			if (args.Player.RequestedSection)
				return true;

			args.Player.RequestedSection = true;
			if (TShock.HackedHealth(args.Player) && !args.Player.Group.HasPermission(Permissions.ignorestathackdetection))
			{
				TShock.Utils.ForceKick(args.Player, "You have Hacked Health/Mana, Please use a different character.");
			}

			if (!args.Player.Group.HasPermission(Permissions.ignorestackhackdetection))
			{
				TShock.HackedInventory(args.Player);
			}

			if (TShock.Utils.ActivePlayers() + 1 > TShock.Config.MaxSlots &&
				!args.Player.Group.HasPermission(Permissions.reservedslot))
			{
				args.Player.SilentKickInProgress = true;
				TShock.Utils.ForceKick(args.Player, TShock.Config.ServerFullReason);
				return true;
			}

			NetMessage.SendData((int) PacketTypes.TimeSet, -1, -1, "", 0, 0, Main.sunModY, Main.moonModY);

			if (TShock.Config.EnableGeoIP && TShock.Geo != null)
			{
				Log.Info(string.Format("{0} ({1}) from '{2}' group from '{3}' joined. ({4}/{5})", args.Player.Name, args.Player.IP,
									   args.Player.Group.Name, args.Player.Country, TShock.Utils.ActivePlayers(),
									   TShock.Config.MaxSlots));
				TShock.Utils.Broadcast(args.Player.Name + " has joined from the " + args.Player.Country, Color.Yellow);
			}
			else
			{
				Log.Info(string.Format("{0} ({1}) from '{2}' group joined. ({3}/{4})", args.Player.Name, args.Player.IP,
									   args.Player.Group.Name, TShock.Utils.ActivePlayers(), TShock.Config.MaxSlots));
				TShock.Utils.Broadcast(args.Player.Name + " has joined", Color.Yellow);
			}

			if (TShock.Config.DisplayIPToAdmins)
				TShock.Utils.SendLogs(string.Format("{0} has joined. IP: {1}", args.Player.Name, args.Player.IP), Color.Blue);

			return false;
		}

		private static bool HandleSendTileSquare(GetDataHandlerArgs args)
		{
			if (args.Player.Group.HasPermission(Permissions.allowclientsideworldedit))
				return false;

			var size = args.Data.ReadInt16();
			var tileX = args.Data.ReadInt32();
			var tileY = args.Data.ReadInt32();

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
				args.Player.SendTileSquare(tileX, tileY);
				return true;
			}

			var tiles = new NetTile[size,size];

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
					if (TShock.CheckTilePermission(args.Player, realx, realy))
					{
						continue;
					}
                    // Server now has a range check built in
					/*if (TShock.CheckRangePermission(args.Player, realx, realy))
					{
						continue;
					}*/
					if ((tile.type == 128 && newtile.Type == 128) || (tile.type == 105 && newtile.Type == 105))
					{
						if (TShock.Config.EnableInsecureTileFixes)
						{
							return false;
						}
					}

					if (tile.type == 0x17 && newtile.Type == 0x2)
					{
						tile.type = 0x2;
						changed = true;
					}
					else if (tile.type == 0x19 && newtile.Type == 0x1)
					{
						tile.type = 0x1;
						changed = true;
					}
					else if ((tile.type == 0xF && newtile.Type == 0xF) ||
							 (tile.type == 0x4F && newtile.Type == 0x4F))
					{
						tile.frameX = newtile.FrameX;
						tile.frameY = newtile.FrameY;
						changed = true;
					}
						// Holy water/Unholy water
					else if (tile.type == 1 && newtile.Type == 117)
					{
						tile.type = 117;
						changed = true;
					}
					else if (tile.type == 1 && newtile.Type == 25)
					{
						tile.type = 25;
						changed = true;
					}
					else if (tile.type == 117 && newtile.Type == 25)
					{
						tile.type = 25;
						changed = true;
					}
					else if (tile.type == 25 && newtile.Type == 117)
					{
						tile.type = 117;
						changed = true;
					}
					else if (tile.type == 2 && newtile.Type == 23)
					{
						tile.type = 23;
						changed = true;
					}
					else if (tile.type == 2 && newtile.Type == 109)
					{
						tile.type = 109;
						changed = true;
					}
					else if (tile.type == 23 && newtile.Type == 109)
					{
						tile.type = 109;
						changed = true;
					}
					else if (tile.type == 109 && newtile.Type == 23)
					{
						tile.type = 23;
						changed = true;
					}
					else if (tile.type == 23 && newtile.Type == 109)
					{
						tile.type = 109;
						changed = true;
					}
					else if (tile.type == 53 && newtile.Type == 116)
					{
						tile.type = 116;
						changed = true;
					}
					else if (tile.type == 53 && newtile.Type == 112)
					{
						tile.type = 112;
						changed = true;
					}
					else if (tile.type == 112 && newtile.Type == 116)
					{
						tile.type = 116;
						changed = true;
					}
					else if (tile.type == 116 && newtile.Type == 112)
					{
						tile.type = 112;
						changed = true;
					}
					else if (tile.type == 112 && newtile.Type == 53)
					{
						tile.type = 53;
						changed = true;
					}
					else if (tile.type == 109 && newtile.Type == 2)
					{
						tile.type = 2;
						changed = true;
					}
					else if (tile.type == 116 && newtile.Type == 53)
					{
						tile.type = 53;
						changed = true;
					}
					else if (tile.type == 117 && newtile.Type == 1)
					{
						tile.type = 1;
						changed = true;
					}
				}
			}

			if (changed)
			{
				TSPlayer.All.SendTileSquare(tileX, tileY, size);
				WorldGen.RangeFrame(tileX, tileY, tileX + size, tileY + size);
			}
			else
			{
				args.Player.SendTileSquare(tileX, tileY, size);
			}
			return true;
		}

		private static bool HandleTile(GetDataHandlerArgs args)
		{
			var type = args.Data.ReadInt8();
			var tileX = args.Data.ReadInt32();
			var tileY = args.Data.ReadInt32();
			var tiletype = args.Data.ReadInt8();
			if (OnTileEdit(tileX, tileY, tiletype, type))
				return true;
			if (tileX < 0 || tileX >= Main.maxTilesX || tileY < 0 || tileY >= Main.maxTilesY)
				return false;

			if (args.Player.AwaitingName)
			{
				var protectedregions = TShock.Regions.InAreaRegionName(tileX, tileY);
				if (protectedregions.Count == 0)
				{
					args.Player.SendMessage("Region is not protected", Color.Yellow);
				}
				else
				{
					string regionlist = string.Join(",", protectedregions.ToArray());
					args.Player.SendMessage("Region Name(s): " + regionlist, Color.Yellow);
				}
				args.Player.SendTileSquare(tileX, tileY);
				args.Player.AwaitingName = false;
				return true;
			}

			if (args.Player.AwaitingTempPoint > 0)
			{
				args.Player.TempPoints[args.Player.AwaitingTempPoint - 1].X = tileX;
				args.Player.TempPoints[args.Player.AwaitingTempPoint - 1].Y = tileY;
				args.Player.SendMessage("Set Temp Point " + args.Player.AwaitingTempPoint, Color.Yellow);
				args.Player.SendTileSquare(tileX, tileY);
				args.Player.AwaitingTempPoint = 0;
				return true;
			}

			if (type == 1 || type == 3)
			{
				if (tiletype >= ((type == 1) ? Main.maxTileSets : Main.maxWallTypes))
				{
					return true;
				}
				if (tiletype == 29 && tiletype == 97 && TShock.Config.ServerSideInventory)
				{
					args.Player.SendMessage("You cannot place this tile, Server side inventory is enabled.", Color.Red);
					args.Player.SendTileSquare(tileX, tileY);
					return true;
				}
				if (tiletype == 48 && !args.Player.Group.HasPermission(Permissions.usebanneditem) &&
					TShock.Itembans.ItemIsBanned("Spike", args.Player))
				{
					args.Player.Disable("Using banned spikes without permissions");
					args.Player.SendTileSquare(tileX, tileY);
					return true;
				}
				if (type == 1 && tiletype == 21 && TShock.Utils.MaxChests())
				{
					args.Player.SendMessage("Reached world's max chest limit, unable to place more!", Color.Red);
					args.Player.SendTileSquare(tileX, tileY);
					return true;
				}
				if (tiletype == 141 && !args.Player.Group.HasPermission(Permissions.usebanneditem) &&
					TShock.Itembans.ItemIsBanned("Explosives", args.Player))
				{
                    args.Player.Disable("Using banned explosives tile without permissions");
					args.Player.SendTileSquare(tileX, tileY);
					return true;
				}
			}

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendTileSquare(tileX, tileY);
				return true;
			}

			if (TShock.CheckTilePermission(args.Player, tileX, tileY, tiletype, type))
			{
				args.Player.SendTileSquare(tileX, tileY);
				return true;
			}

			if ((tiletype == 127 || Main.tileCut[tiletype]) && (type == 0 || type == 4))
			{
				return false;
			}

			if (TShock.CheckRangePermission(args.Player, tileX, tileY))
			{
				args.Player.SendTileSquare(tileX, tileY);
				return true;
			}

			if (args.Player.TileKillThreshold >= TShock.Config.TileKillThreshold)
			{
				args.Player.Disable("Reached TileKill threshold");
				args.Player.SendTileSquare(tileX, tileY);
				return true;
			}

			if (args.Player.TilePlaceThreshold >= TShock.Config.TilePlaceThreshold)
			{
				args.Player.Disable("Reached TilePlace threshold");
				args.Player.SendTileSquare(tileX, tileY);
				return true;
			}

			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
			{
				args.Player.SendTileSquare(tileX, tileY);
				return true;
			}

			if (type == 1 && !args.Player.Group.HasPermission(Permissions.ignoreplacetiledetection))
			{
				args.Player.TilePlaceThreshold++;
				var coords = new Vector2(tileX, tileY);
				if (!args.Player.TilesCreated.ContainsKey(coords))
					args.Player.TilesCreated.Add(coords, Main.tile[tileX, tileY].Data);
			}

			if ((type == 0 || type == 4) && Main.tileSolid[Main.tile[tileX, tileY].type] &&
				!args.Player.Group.HasPermission(Permissions.ignorekilltiledetection))
			{
				args.Player.TileKillThreshold++;
				var coords = new Vector2(tileX, tileY);
				if (!args.Player.TilesDestroyed.ContainsKey(coords))
					args.Player.TilesDestroyed.Add(coords, Main.tile[tileX, tileY].Data);
			}

			return false;
		}

		private static bool HandleTogglePvp(GetDataHandlerArgs args)
		{
			byte id = args.Data.ReadInt8();
			bool pvp = args.Data.ReadBoolean();
			if (OnPvpToggled(id, pvp))
				return true;

			if (id != args.Player.Index)
			{
				return true;
			}

			if (TShock.Config.PvPMode == "disabled")
			{
				return true;
			}

			if (args.TPlayer.hostile != pvp)
			{
				long seconds = (long) (DateTime.UtcNow - args.Player.LastPvpChange).TotalSeconds;
				if (seconds > 5)
				{
					TSPlayer.All.SendMessage(string.Format("{0} has {1} PvP!", args.Player.Name, pvp ? "enabled" : "disabled"),
											 Main.teamColor[args.Player.Team]);
				}
				args.Player.LastPvpChange = DateTime.UtcNow;
			}

			args.TPlayer.hostile = pvp;

			if (TShock.Config.PvPMode == "always")
			{
				if (!pvp)
					args.Player.Spawn();
			}

			NetMessage.SendData((int) PacketTypes.TogglePvp, -1, -1, "", args.Player.Index);

			return true;
		}

		private static bool HandlePlayerUpdate(GetDataHandlerArgs args)
		{
			var plr = args.Data.ReadInt8();
			var control = args.Data.ReadInt8();
			var item = args.Data.ReadInt8();
			var pos = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());
			var vel = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());
			if (OnPlayerUpdate(plr, control, item, pos, vel))
				return true;
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
				float distance = Vector2.Distance(new Vector2(pos.X/16f, pos.Y/16f),
												  new Vector2(args.Player.LastNetPosition.X/16f, args.Player.LastNetPosition.Y/16f));
				if (TShock.CheckIgnores(args.Player))
				{
					if (distance > TShock.Config.MaxRangeForDisabled)
					{
						if (args.Player.IgnoreActionsForCheating != "none")
						{
							args.Player.SendMessage("Disabled for cheating: " + args.Player.IgnoreActionsForCheating,
													Color.Red);
						}
						else if (args.Player.IgnoreActionsForDisabledArmor != "none")
						{
							args.Player.SendMessage(
								"Disabled for banned armor: " + args.Player.IgnoreActionsForDisabledArmor, Color.Red);
						}
						else if (args.Player.IgnoreActionsForInventory != "none")
						{
							args.Player.SendMessage(
								"Disabled for Server Side Inventory: " + args.Player.IgnoreActionsForInventory,
								Color.Red);
						}
						else if (TShock.Config.RequireLogin && !args.Player.IsLoggedIn)
						{
							args.Player.SendMessage("Please /register or /login to play!", Color.Red);
						}
						else if (args.Player.IgnoreActionsForClearingTrashCan)
						{
							args.Player.SendMessage("You need to rejoin to ensure your trash can is cleared!", Color.Red);
						}
						else if (TShock.Config.PvPMode == "always" && !args.TPlayer.hostile)
						{
							args.Player.SendMessage("PvP is forced! Enable PvP else you can't move or do anything!",
													Color.Red);
						}
						int lastTileX = (int) (args.Player.LastNetPosition.X/16f);
						int lastTileY = (int) (args.Player.LastNetPosition.Y/16f);
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

				if (!args.Player.Group.HasPermission(Permissions.ignorenoclipdetection) &&
					Collision.SolidCollision(pos, args.TPlayer.width, args.TPlayer.height) && !TShock.Config.IgnoreNoClip)
				{
					int lastTileX = (int) (args.Player.LastNetPosition.X/16f);
					int lastTileY = (int) (args.Player.LastNetPosition.Y/16f);
					if (!args.Player.Teleport(lastTileX, lastTileY + 3))
					{
						args.Player.SendMessage("You got stuck in a solid object, Sent to spawn point.");
						args.Player.Spawn();
					}
					return true;
				}
				args.Player.LastNetPosition = pos;
			}

			if ((control & 32) == 32)
			{
				if (!args.Player.Group.HasPermission(Permissions.usebanneditem) &&
					TShock.Itembans.ItemIsBanned(args.TPlayer.inventory[item].name, args.Player))
				{
					control -= 32;
					args.Player.Disable("Using banned item");
					args.Player.SendMessage(
						string.Format("You cannot use {0} on this server. Your actions are being ignored.",
									  args.TPlayer.inventory[item].name), Color.Red);
				}
			}

			args.TPlayer.selectedItem = item;
			args.TPlayer.position = pos;
			args.TPlayer.velocity = vel;
			args.TPlayer.oldVelocity = args.TPlayer.velocity;
			args.TPlayer.fallStart = (int) (pos.Y/16f);
			args.TPlayer.controlUp = false;
			args.TPlayer.controlDown = false;
			args.TPlayer.controlLeft = false;
			args.TPlayer.controlRight = false;
			args.TPlayer.controlJump = false;
			args.TPlayer.controlUseItem = false;
			args.TPlayer.direction = -1;
			if ((control & 1) == 1)
			{
				args.TPlayer.controlUp = true;
			}
			if ((control & 2) == 2)
			{
				args.TPlayer.controlDown = true;
			}
			if ((control & 4) == 4)
			{
				args.TPlayer.controlLeft = true;
			}
			if ((control & 8) == 8)
			{
				args.TPlayer.controlRight = true;
			}
			if ((control & 16) == 16)
			{
				args.TPlayer.controlJump = true;
			}
			if ((control & 32) == 32)
			{
				args.TPlayer.controlUseItem = true;
			}
			if ((control & 64) == 64)
			{
				args.TPlayer.direction = 1;
			}
			NetMessage.SendData((int) PacketTypes.PlayerUpdate, -1, args.Player.Index, "", args.Player.Index);

			return true;
		}

		private static bool HandleProjectileNew(GetDataHandlerArgs args)
		{
			var ident = args.Data.ReadInt16();
			var pos = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());
			var vel = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());
			var knockback = args.Data.ReadSingle();
			var dmg = args.Data.ReadInt16();
			var owner = args.Data.ReadInt8();
			var type = args.Data.ReadInt8();
		    owner = (byte)args.Player.Index;
			var index = TShock.Utils.SearchProjectile(ident, owner);

			if (OnNewProjectile(ident, pos, vel, knockback, dmg, owner, type, index))
				return true;

			if (index > Main.maxProjectiles || index < 0)
			{
				return false;
			}

            // Server now checks owner + ident, if owner is different, server will create new projectile.
			/*if (args.Player.Index != owner)
			{
                args.Player.Disable("Owner and player ID does not match to update projectile");
				args.Player.RemoveProjectile(ident, owner);
				return true;
			}*/

            if (dmg > TShock.Config.MaxProjDamage && !args.Player.Group.HasPermission(Permissions.ignoredamagecap))
			{
				args.Player.Disable(String.Format("Projectile damage is higher than {0}", TShock.Config.MaxProjDamage));
				args.Player.RemoveProjectile(ident, owner);
				return true;
			}

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.RemoveProjectile(ident, owner);
				return true;
			}

			if (!TShock.Config.IgnoreProjUpdate && TShock.CheckProjectilePermission(args.Player, index, type))
			{
				args.Player.Disable("Does not have projectile permission to update projectile.");
				args.Player.RemoveProjectile(ident, owner);
				return true;
			}

			if (args.Player.ProjectileThreshold >= TShock.Config.ProjectileThreshold)
			{
				args.Player.Disable("Reached projectile update threshold");
				args.Player.RemoveProjectile(ident, owner);
				return true;
			}

			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
			{
				args.Player.RemoveProjectile(ident, owner);
				return true;
			}

			if (!args.Player.Group.HasPermission(Permissions.ignoreprojectiledetection))
			{
				args.Player.ProjectileThreshold++;
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
				args.Player.Disable("Owner and player ID does not match to kill projectile");
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
				args.Player.Disable("Does not have projectile permission to kill projectile");
				args.Player.RemoveProjectile(ident, owner);
				return true;
			}

			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
			{
				args.Player.RemoveProjectile(ident, owner);
				return true;
			}

			return false;
		}

		private static bool HandlePlayerKillMe(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt8();
			var direction = args.Data.ReadInt8();
			var dmg = args.Data.ReadInt16();
			var pvp = args.Data.ReadInt8() == 0;
			if (OnKillMe(id, direction, dmg, pvp))
				return true;
			int textlength = (int) (args.Data.Length - args.Data.Position - 1);
			string deathtext = "";
			if (textlength > 0)
			{
				deathtext = Encoding.UTF8.GetString(args.Data.ReadBytes(textlength));
				/*if (!TShock.Utils.ValidString(deathtext))
				{
					return true;
				}*/
			}

			args.Player.LastDeath = DateTime.Now;
			args.Player.Dead = true;

			return false;
		}

		private static bool HandleLiquidSet(GetDataHandlerArgs args)
		{
			int tileX = args.Data.ReadInt32();
			int tileY = args.Data.ReadInt32();
			byte liquid = args.Data.ReadInt8();
			bool lava = args.Data.ReadBoolean();

			if (OnLiquidSet(tileX, tileY, liquid, lava))
				return true;

			//The liquid was picked up.
			if (liquid == 0)
				return false;

			if (tileX < 0 || tileX >= Main.maxTilesX || tileY < 0 || tileY >= Main.maxTilesY)
				return false;

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendTileSquare(tileX, tileY);
				return true;
			}

			if (args.Player.TileLiquidThreshold >= TShock.Config.TileLiquidThreshold)
			{
				args.Player.Disable("Reached TileLiquid threshold");
				args.Player.SendTileSquare(tileX, tileY);
				return true;
			}

			if (!args.Player.Group.HasPermission(Permissions.ignoreliquidsetdetection))
			{
				args.Player.TileLiquidThreshold++;
			}

			int bucket = 0;
			if (args.TPlayer.inventory[args.TPlayer.selectedItem].type == 206)
			{
				bucket = 1;
			}
			else if (args.TPlayer.inventory[args.TPlayer.selectedItem].type == 207)
			{
				bucket = 2;
			}

			if (lava && bucket != 2 && !args.Player.Group.HasPermission(Permissions.usebanneditem) &&
				TShock.Itembans.ItemIsBanned("Lava Bucket", args.Player))
			{
                args.Player.Disable("Using banned lava bucket without permissions");
				args.Player.SendTileSquare(tileX, tileY);
				return true;
			}

			if (!lava && bucket != 1 && !args.Player.Group.HasPermission(Permissions.usebanneditem) &&
				TShock.Itembans.ItemIsBanned("Water Bucket", args.Player))
			{
                args.Player.Disable("Using banned water bucket without permissions");
				args.Player.SendTileSquare(tileX, tileY);
				return true;
			}

			if (TShock.CheckTilePermission(args.Player, tileX, tileY))
			{
				args.Player.SendTileSquare(tileX, tileY);
				return true;
			}

			if (TShock.CheckRangePermission(args.Player, tileX, tileY, 16))
			{
				args.Player.SendTileSquare(tileX, tileY);
				return true;
			}

			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
			{
				args.Player.SendTileSquare(tileX, tileY);
				return true;
			}

			return false;
		}

		private static bool HandleTileKill(GetDataHandlerArgs args)
		{
			var tileX = args.Data.ReadInt32();
			var tileY = args.Data.ReadInt32();
			if (OnTileKill(tileX, tileY))
				return true;
			if (tileX < 0 || tileX >= Main.maxTilesX || tileY < 0 || tileY >= Main.maxTilesY)
				return false;

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendTileSquare(tileX, tileY);
				return true;
			}

			if (Main.tile[tileX, tileY].type != 0x15 && (!TShock.Utils.MaxChests() && Main.tile[tileX, tileY].type != 0)) //Chest
			{
				args.Player.SendTileSquare(tileX, tileY);
				return true;
			}

			if (TShock.CheckTilePermission(args.Player, tileX, tileY))
			{
				args.Player.SendTileSquare(tileX, tileY);
				return true;
			}

			if (TShock.CheckRangePermission(args.Player, tileX, tileY))
			{
				args.Player.SendTileSquare(tileX, tileY);
				return true;
			}

			return false;
		}

		private static bool HandleSpawn(GetDataHandlerArgs args)
		{
			var player = args.Data.ReadInt8();
			var spawnx = args.Data.ReadInt32();
			var spawny = args.Data.ReadInt32();

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
							if (!TShock.Utils.Ban(args.Player, TShock.Config.MediumcoreBanReason))
								TShock.Utils.ForceKick(args.Player, "Death results in a ban, but can't ban you");
						}
						else
						{
							TShock.Utils.ForceKick(args.Player, TShock.Config.MediumcoreKickReason);
						}
						return true;
					}
				}
			}
			else
				args.Player.InitSpawn = true;

			args.Player.Dead = false;
			return false;
		}

		private static bool HandleChestOpen(GetDataHandlerArgs args)
		{
			var x = args.Data.ReadInt32();
			var y = args.Data.ReadInt32();

			if (OnChestOpen(x, y))
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

			return false;
		}

		private static bool HandleChestItem(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt16();
			var slot = args.Data.ReadInt8();
			var stacks = args.Data.ReadInt8();
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
			var x = args.Data.ReadInt32();
			var y = args.Data.ReadInt32();

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

			if (!args.Player.Group.HasPermission(Permissions.movenpc))
			{
				args.Player.SendMessage("You do not have permission to relocate NPCs.", Color.Red);
				args.Player.SendData(PacketTypes.UpdateNPCHome, "", id, Main.npc[id].homeTileX, Main.npc[id].homeTileY,
									 Convert.ToByte(Main.npc[id].homeless));
				return true;
			}

			if (TShock.CheckTilePermission(args.Player, x, y))
			{
                args.Player.SendMessage( "You do not have access to modify this area.", Color.Red);
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

		private static bool HandlePlayerBuff(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt8();
			var type = args.Data.ReadInt8();
			var time = args.Data.ReadInt16();

			if (OnPlayerBuff(id, type, time))
				return true;

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendData(PacketTypes.PlayerBuff, "", id);
				return true;
			}
			if (!TShock.Players[id].TPlayer.hostile)
			{
				args.Player.SendData(PacketTypes.PlayerBuff, "", id);
				return true;
			}
			if (TShock.CheckRangePermission(args.Player, TShock.Players[id].TileX, TShock.Players[id].TileY, 50))
			{
				args.Player.SendData(PacketTypes.PlayerBuff, "", id);
				return true;
			}
			if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
			{
				args.Player.SendData(PacketTypes.PlayerBuff, "", id);
				return true;
			}

			if (WhitelistBuffMaxTime[type] > 0 && time <= WhitelistBuffMaxTime[type])
			{
				return false;
			}

			args.Player.SendData(PacketTypes.PlayerBuff, "", id);
			return true;
		}

		private static bool HandleItemDrop(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt16();
			var pos = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());
			var vel = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());
			var stacks = args.Data.ReadInt8();
			var prefix = args.Data.ReadInt8();
			var type = args.Data.ReadInt16();

			if (OnItemDrop(id, pos, vel, stacks, prefix, type))
				return true;

			if (type == 0) //Item removed, let client do this to prevent item duplication client side
			{
				return false;
			}

			if (TShock.CheckRangePermission(args.Player, (int) (pos.X/16f), (int) (pos.Y/16f)))
			{
				args.Player.SendData(PacketTypes.ItemDrop, "", id);
				return true;
			}

			Item item = new Item();
			item.netDefaults(type);
			if (stacks > item.maxStack || TShock.Itembans.ItemIsBanned(item.name, args.Player))
			{
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

		private static bool HandlePlayerDamage(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt8();
			var direction = args.Data.ReadInt8();
			var dmg = args.Data.ReadInt16();
			var pvp = args.Data.ReadInt8();
			var crit = args.Data.ReadInt8();

			if (OnPlayerDamage(id, direction, dmg, pvp, crit))
				return true;

			int textlength = (int) (args.Data.Length - args.Data.Position - 1);
			string deathtext = "";
			if (textlength > 0)
			{
				deathtext = Encoding.UTF8.GetString(args.Data.ReadBytes(textlength));
				/*if (!TShock.Utils.ValidString(deathtext))
				{
					return true;
				}*/
			}

			if (TShock.Players[id] == null)
				return true;

            if (dmg > TShock.Config.MaxDamage && !args.Player.Group.HasPermission(Permissions.ignoredamagecap))
			{
                args.Player.Disable(String.Format("Player damage exceeded {0}", TShock.Config.MaxDamage ) );
				args.Player.SendData(PacketTypes.PlayerHp, "", id);
				args.Player.SendData(PacketTypes.PlayerUpdate, "", id);
				return true;
			}

			if (!TShock.Players[id].TPlayer.hostile)
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

			return false;
		}

		private static bool HandleNpcStrike(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt8();
			var direction = args.Data.ReadInt8();
			var dmg = args.Data.ReadInt16();
			var pvp = args.Data.ReadInt8();
			var crit = args.Data.ReadInt8();

			if (OnNPCStrike(id, direction, dmg, pvp, crit))
				return true;

			if (Main.npc[id] == null)
				return true;

            if (dmg > TShock.Config.MaxDamage && !args.Player.Group.HasPermission(Permissions.ignoredamagecap))
			{
                args.Player.Disable(String.Format("NPC damage exceeded {0}", TShock.Config.MaxDamage ) );
				args.Player.SendData(PacketTypes.NpcUpdate, "", id);
				return true;
			}

			if (TShock.CheckIgnores(args.Player))
			{
				args.Player.SendData(PacketTypes.NpcUpdate, "", id);
				return true;
			}

			if (Main.npc[id].townNPC && !args.Player.Group.HasPermission(Permissions.movenpc))
			{
                args.Player.SendMessage( "You don't have permission to move the NPC", Color.Yellow);
				args.Player.SendData(PacketTypes.NpcUpdate, "", id);
				return true;
			}

			if (TShock.Config.RangeChecks &&
				TShock.CheckRangePermission(args.Player, (int) (Main.npc[id].position.X/16f), (int) (Main.npc[id].position.Y/16f),
											128))
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

		private static bool HandlePlayerBuffUpdate(GetDataHandlerArgs args)
		{
			var id = args.Data.ReadInt8();

			if (OnPlayerBuffUpdate(id))
				return true;

			for (int i = 0; i < 10; i++)
			{
				var buff = args.Data.ReadInt8();

				if (buff == 10)
				{
					if (!args.Player.Group.HasPermission(Permissions.usebanneditem) &&
						TShock.Itembans.ItemIsBanned("Invisibility Potion", args.Player))
						buff = 0;
					else if (TShock.Config.DisableInvisPvP && args.TPlayer.hostile)
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
			NetMessage.SendData((int) PacketTypes.PlayerBuff, -1, args.Player.Index, "", args.Player.Index);
			return true;
		}

		private static bool HandleSpawnBoss(GetDataHandlerArgs args)
		{
		    var spawnboss = false;
		    var invasion = -1;
		    var plr = args.Data.ReadInt32();
		    var Type = args.Data.ReadInt32();
            spawnboss = (Type == 4 || Type == 13 || (Type == 50 || Type == 125) || (Type == 126 || Type == 134 || (Type == (int) sbyte.MaxValue || Type == 128)));
            if (!spawnboss)
            {
                switch (Type)
                {
                    case -1:
                        invasion = 1;
                        break;
                    case -2:
                        invasion = 2;
                        break;
                }
            }
            if (spawnboss && !args.Player.Group.HasPermission(Permissions.summonboss))
            {
                args.Player.SendMessage("You don't have permission to summon a boss.", Color.Red);
                return true;
            }
            if (invasion != -1 && !args.Player.Group.HasPermission(Permissions.startinvasion))
            {
                args.Player.SendMessage("You don't have permission to start an invasion.", Color.Red);
                return true;
            }
            if (!spawnboss && invasion == -1)
                return true;
            if (plr != args.Player.Index)
                return true;
		    return false;
		}
	}
}