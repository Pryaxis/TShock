/*
TShock, a server mod for Terraria
Copyright (C) 2011-2016 Nyx Studios (fka. The TShock Team)

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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Timers;
using Terraria;
using Terraria.ID;
using TShockAPI.DB;
using TShockAPI.Hooks;
using TShockAPI.Net;
using Timer = System.Timers.Timer;

namespace TShockAPI
{
	/// <summary>
	/// Bitflags used with the <see cref="Disable(string, DisableFlags)"></see> method
	/// </summary>
	[Flags]
	public enum DisableFlags
	{
		/// <summary>
		/// Disable the player and leave no messages
		/// </summary>
		None,
		/// <summary>
		/// Write the Disable message to the console
		/// </summary>
		WriteToConsole,
		/// <summary>
		/// Write the Disable message to the log
		/// </summary>
		WriteToLog,
		/// <summary>
		/// Equivalent to WriteToConsole | WriteToLog
		/// </summary>
		WriteToLogAndConsole
	}

	public class TSPlayer
	{
		/// <summary>
		/// This represents the server as a player.
		/// </summary>
		public static readonly TSServerPlayer Server = new TSServerPlayer();

		/// <summary>
		/// This player represents all the players.
		/// </summary>
		public static readonly TSPlayer All = new TSPlayer("All");

		/// <summary>
		/// The amount of tiles that the player has killed in the last second.
		/// </summary>
		public int TileKillThreshold { get; set; }
		
		/// <summary>
		/// The amount of tiles the player has placed in the last second.
		/// </summary>
		public int TilePlaceThreshold { get; set; }

		/// <summary>
		/// The amount of liquid (in tiles) that the player has placed in the last second.
		/// </summary>
		public int TileLiquidThreshold { get; set; }

		/// <summary>
		/// The amount of tiles that the player has painted in the last second.
		/// </summary>
		public int PaintThreshold { get; set; }

		/// <summary>
		/// The number of projectiles created by the player in the last second.
		/// </summary>
		public int ProjectileThreshold { get; set; }
		
		/// <summary>
		/// A timer to keep track of whether or not the player has recently thrown an explosive
		/// </summary>
		public int RecentFuse = 0;

		/// <summary>
		/// Whether to ignore packets that are SSC-relevant.
		/// </summary>
		public bool IgnoreSSCPackets { get; set; }

		/// <summary>
		/// A system to delay Remembered Position Teleports a few seconds
		/// </summary>
		public int RPPending = 0;
		
		public int sX = -1;
		public int sY = -1;
		
		/// <summary>
		/// A queue of tiles destroyed by the player for reverting.
		/// </summary>
		public Dictionary<Vector2, Tile> TilesDestroyed { get; protected set; }

		/// <summary>
		/// A queue of tiles placed by the player for reverting.
		/// </summary>
		public Dictionary<Vector2, Tile> TilesCreated { get; protected set; }

		/// <summary>
		/// The player's group.
		/// </summary>
		public Group Group
		{
			get
			{
				if (tempGroup != null)
					return tempGroup;
				return group;
			}
			set { group = value; }
		}

		/// <summary>
		/// The player's temporary group.  This overrides the user's actual group.
		/// </summary>
		public Group tempGroup = null;
		
		public Timer tempGroupTimer;

		private Group group = null;

		public bool ReceivedInfo { get; set; }

		/// <summary>
		/// The players index in the player array( Main.players[] ).
		/// </summary>
		public int Index { get; protected set; }

		/// <summary>
		/// The last time the player changed their team or pvp status.  
		/// </summary>
		public DateTime LastPvPTeamChange;

		/// <summary>
		/// Temp points for use in regions and other plugins.
		/// </summary>
		public Point[] TempPoints = new Point[2];

		/// <summary>
		/// Whether the player is waiting to place/break a tile to set as a temp point.
		/// </summary>
		public int AwaitingTempPoint { get; set; }

		/// <summary>
		/// A list of command callbacks indexed by the command they need to do.
		/// </summary>
		public Dictionary<string, Action<object>> AwaitingResponse;  

		public bool AwaitingName { get; set; }

		public string[] AwaitingNameParameters { get; set; }

		/// <summary>
		/// The last time a player broke a grief check.
		/// </summary>
		public DateTime LastThreat { get; set; }

		public bool InitSpawn;

		/// <summary>
		/// Whether the player should see logs.
		/// </summary>
		public bool DisplayLogs = true;

		/// <summary>
		/// The last player that the player whispered with (to or from).
		/// </summary>
		public TSPlayer LastWhisper;

		/// <summary>
		/// The number of unsuccessful login attempts.
		/// </summary>
		public int LoginAttempts { get; set; }

		/// <summary>
		/// Unused.
		/// </summary>
		public Vector2 TeleportCoords = new Vector2(-1, -1);

		/// <summary>
		/// The player's last known position from PlayerUpdate packet.
		/// </summary>
		public Vector2 LastNetPosition = Vector2.Zero;

		/// <summary>
		/// The player's login name.
		/// </summary>
		[Obsolete("Use User.Name instead")]
		public string UserAccountName
		{
			get { return User == null ? null : User.Name; }
		}

		/// <summary>
		/// User object associated with the player.
		/// Set when the player logs in.
		/// </summary>
		public User User { get; set; }

		/// <summary>
		/// Whether the player performed a valid login attempt (i.e. entered valid user name and password) but is still blocked
		/// from logging in because of SSI.
		/// </summary>
		public bool LoginFailsBySsi { get; set; }

		/// <summary>
		/// Whether the player is logged in or not.
		/// </summary>
		public bool IsLoggedIn;

		/// <summary>
		/// Whether the player has sent their whole inventory to the server while connecting.
		/// </summary>
		public bool HasSentInventory { get; set; }

		/// <summary>
		/// The player's user id( from the db ).
		/// </summary>
		[Obsolete("Use User.ID instead")]
		public int UserID
		{
			get { return User == null ? -1 : User.ID; }
		}

		/// <summary>
		/// Whether the player has been nagged about logging in.
		/// </summary>
		public bool HasBeenNaggedAboutLoggingIn;

		/// <summary>
		/// Whether other players can teleport to the player.
		/// </summary>
		public bool TPAllow = true;

		/// <summary>
		/// Whether the player is muted or not.
		/// </summary>
		public bool mute;

		private Player FakePlayer;

		public bool RequestedSection;

		/// <summary>
		/// The player's respawn timer.
		/// </summary>
		public int RespawnTimer;

		/// <summary>
		/// Whether the player is dead or not.
		/// </summary>
		public bool Dead;

		public string Country = "??";

		/// <summary>
		/// The players difficulty( normal[softcore], mediumcore, hardcore ).
		/// </summary>
		public int Difficulty;

		private string CacheIP;

		public string IgnoreActionsForInventory = "none";

		public string IgnoreActionsForCheating = "none";

		public string IgnoreActionsForDisabledArmor = "none";

		public bool IgnoreActionsForClearingTrashCan;

		/// <summary>
		/// The player's server side inventory data.
		/// </summary>
		public PlayerData PlayerData;

		/// <summary>
		/// Whether the player needs to specify a password upon connection( either server or user account ).
		/// </summary>
		public bool RequiresPassword;

		public bool SilentKickInProgress;

		public bool SilentJoinInProgress;

		/// <summary>
		/// A list of points where ice tiles have been placed.
		/// </summary>
		public List<Point> IceTiles;

		/// <summary>
		/// Unused, can be removed.
		/// </summary>
		public long RPm = 1;

		/// <summary>
		/// World protection message cool down.
		/// </summary>
		public long WPm = 1;

		/// <summary>
		/// Spawn protection message cool down.
		/// </summary>
		public long SPm = 1;
			
		/// <summary>
		/// Permission to build message cool down.
		/// </summary>
		public long BPm = 1;

		/// <summary>
		/// The time in ms when the player has logged in.  
		/// </summary>
		public long LoginMS;

		/// <summary>
		/// Whether the player has been harrassed about logging in due to server side inventory or forced login.
		/// </summary>
		public bool LoginHarassed = false;

		/// <summary>
		/// Player cant die, unless onehit
		/// </summary>
		public bool GodMode = false;

		/// <summary>
		/// Players controls are inverted if using SSC
		/// </summary>
		public bool Confused = false;

		/// <summary>
		/// The last projectile type this player tried to kill.
		/// </summary>
		public int LastKilledProjectile = 0;

		/// <summary>
		/// The current region this player is in, or null if none.
		/// </summary>
		public Region CurrentRegion = null;

		/// <summary>
		/// Contains data stored by plugins
		/// </summary>
		protected ConcurrentDictionary<string, object> data = new ConcurrentDictionary<string, object>();
		
		/// <summary>
		/// Whether the player is a real, human, player on the server.
		/// </summary>
		public bool RealPlayer
		{
			get { return Index >= 0 && Index < Main.maxNetPlayers && Main.player[Index] != null; }
		}

		/// <summary>
		/// Checks if the player is active and not pending termination.
		/// </summary>
		public bool ConnectionAlive
		{
			get
			{
				return RealPlayer
					&& (Netplay.Clients[Index] != null && Netplay.Clients[Index].IsActive && !Netplay.Clients[Index].PendingTermination);
			}
		}

		/// <summary>
		/// Gets the item that the player is currently holding.
		/// </summary>
		public Item SelectedItem
		{
			get { return TPlayer.inventory[TPlayer.selectedItem]; }
		}

		/// <summary>
		/// Gets the player's Client State.
		/// </summary>
		public int State
		{
			get { return Netplay.Clients[Index].State; }
			set { Netplay.Clients[Index].State = value; }
		}

		/// <summary>
		/// Gets the player's UUID.
		/// </summary>
		public string UUID
		{
			get { return RealPlayer ? Netplay.Clients[Index].ClientUUID : ""; }
		}

		/// <summary>
		/// Gets the player's IP.
		/// </summary>
		public string IP
		{
			get
			{
				if (string.IsNullOrEmpty(CacheIP))
					return
				CacheIP = RealPlayer ? (Netplay.Clients[Index].Socket.IsConnected()
						? TShock.Utils.GetRealIP(Netplay.Clients[Index].Socket.GetRemoteAddress().ToString())
						: "")
					: "";
				else
					return CacheIP;
			}
		}

		/// <summary>
		/// Gets the player's accessories.
		/// </summary>
		public IEnumerable<Item> Accessories
		{
			get
			{
				for (int i = 3; i < 8; i++)
					yield return TPlayer.armor[i];
			}
		}

		/// <summary>
		/// Saves the player's inventory to SSC
		/// </summary>
		/// <returns>bool - True/false if it saved successfully</returns>
		public bool SaveServerCharacter()
		{
			if (!Main.ServerSideCharacter)
			{
				return false;
			}
			try
			{
				if (HasPermission(Permissions.bypassssc))
				{
					TShock.Log.ConsoleInfo("Skipping SSC Backup for " + User.Name); // Debug Code
					return true;
				}
				PlayerData.CopyCharacter(this);
				TShock.CharacterDB.InsertPlayerData(this);
				return true;
			}
			catch (Exception e)
			{
				TShock.Log.Error(e.Message);
				return false;
			}
		}

		/// <summary>
		/// Sends the players server side character to client
		/// </summary>
		/// <returns>bool - True/false if it saved successfully</returns>
		public bool SendServerCharacter()
		{
			if (!Main.ServerSideCharacter)
			{
				return false;
			}
			try
			{
				PlayerData.RestoreCharacter(this);
				return true;
			}
			catch (Exception e)
			{
				TShock.Log.Error(e.Message);
				return false;
			}

		}

		/// <summary>
		/// Gets the Terraria Player object associated with the player.
		/// </summary>
		public Player TPlayer
		{
			get { return FakePlayer ?? Main.player[Index]; }
		}

		/// <summary>
		/// Gets the player's name.
		/// </summary>
		public string Name
		{
			get { return TPlayer.name; }
		}

		/// <summary>
		/// Gets the player's active state.
		/// </summary>
		public bool Active
		{
			get { return TPlayer != null && TPlayer.active; }
		}

		/// <summary>
		/// Gets the player's team.
		/// </summary>
		public int Team
		{
			get { return TPlayer.team; }
		}

		/// <summary>
		/// Gets the player's X coordinate.
		/// </summary>
		public float X
		{
			get { return RealPlayer ? TPlayer.position.X : Main.spawnTileX*16; }
		}

		/// <summary>
		/// Gets the player's Y coordinate.
		/// </summary>
		public float Y
		{
			get { return RealPlayer ? TPlayer.position.Y : Main.spawnTileY*16; }
		}

		/// <summary>
		/// Player X coordinate divided by 16. Supposed X world coordinate.
		/// </summary>
		public int TileX
		{
			get { return (int) (X/16); }
		}

		/// <summary>
		/// Player Y cooridnate divided by 16. Supposed Y world coordinate.
		/// </summary>
		public int TileY
		{
			get { return (int) (Y/16); }
		}

		/// <summary>
		/// Unused.
		/// </summary>
		public bool TpLock;

		/// <summary>
		/// Checks if the player has any inventory slots available.
		/// </summary>
		public bool InventorySlotAvailable
		{
			get
			{
				bool flag = false;
				if (RealPlayer)
				{
					for (int i = 0; i < 50; i++) //51 is trash can, 52-55 is coins, 56-59 is ammo
					{
						if (TPlayer.inventory[i] == null || !TPlayer.inventory[i].active || TPlayer.inventory[i].name == "")
						{
							flag = true;
							break;
						}
					}
				}
				return flag;
			}
		}

		/// <summary>
		/// Determines whether the player's storage contains the given key.
		/// </summary>
		/// <param name="key">Key to test.</param>
		/// <returns></returns>
		public bool ContainsData(string key)
		{
			return data.ContainsKey(key);
		}

		/// <summary>
		/// Returns the stored object associated with the given key.
		/// </summary>
		/// <typeparam name="T">Type of the object being retrieved.</typeparam>
		/// <param name="key">Key with which to access the object.</param>
		/// <returns>The stored object, or default(T) if not found.</returns>
		public T GetData<T>(string key)
		{
			object obj;
			if (!data.TryGetValue(key, out obj))
			{
				return default(T);
			}

			return (T)obj;
		}

		/// <summary>
		/// Stores an object on this player, accessible with the given key.
		/// </summary>
		/// <typeparam name="T">Type of the object being stored.</typeparam>
		/// <param name="key">Key with which to access the object.</param>
		/// <param name="value">Object to store.</param>
		public void SetData<T>(string key, T value)
		{
			if (!data.TryAdd(key, value))
			{
				data.TryUpdate(key, value, data[key]);
			}
		}

		/// <summary>
		/// Removes the stored object associated with the given key.
		/// </summary>
		/// <param name="key">Key with which to access the object.</param>
		/// <returns>The removed object.	</returns>
		public object RemoveData(string key)
		{
			object rem;
			if (data.TryRemove(key, out rem))
			{
				return rem;
			}
			return null;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TSPlayer"/> class.
		/// </summary>
		/// <param name="index">The player's index in the.</param>
		public TSPlayer(int index)
		{
			TilesDestroyed = new Dictionary<Vector2, Tile>();
			TilesCreated = new Dictionary<Vector2, Tile>();
			Index = index;
			Group = Group.DefaultGroup;
			IceTiles = new List<Point>();
			AwaitingResponse = new Dictionary<string, Action<object>>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TSPlayer"/> class.
		/// </summary>
		/// <param name="playerName">The player's name.</param>
		protected TSPlayer(String playerName)
		{
			TilesDestroyed = new Dictionary<Vector2, Tile>();
			TilesCreated = new Dictionary<Vector2, Tile>();
			Index = -1;
			FakePlayer = new Player {name = playerName, whoAmI = -1};
			Group = Group.DefaultGroup;
			AwaitingResponse = new Dictionary<string, Action<object>>();
		}

		/// <summary>
		/// Disconnects the player from the server.
		/// </summary>
		/// <param name="reason">The reason why the player was disconnected.</param>
		public virtual void Disconnect(string reason)
		{
			SendData(PacketTypes.Disconnect, reason);
		}

		[Obsolete("This method is no longer used.")]
		public virtual void Flush()
		{
			var client = Netplay.Clients[Index];
			if (client == null)
				return;

			//TShock.PacketBuffer.Flush(client);
		}

		/// <summary>
		/// Fired when the player's temporary group access expires.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public void TempGroupTimerElapsed(object sender, ElapsedEventArgs args)
		{
			SendWarningMessage("Your temporary group access has expired.");

			tempGroup = null;
			if (sender != null)
			{
				((Timer)sender).Stop();
			}
		}

		/// <summary>
		/// Teleports the player to the given coordinates in the world.
		/// </summary>
		/// <param name="x">The X coordinate.</param>
		/// <param name="y">The Y coordinate.</param>
		/// <param name="style">The teleportation style.</param>
		/// <returns>True or false.</returns>
		public bool Teleport(float x, float y, byte style = 1)
		{
			if (x > Main.rightWorld - 992)
			{
				x = Main.rightWorld - 992;
			}
			if (x < 992)
			{
				x = 992;
			}
			if (y > Main.bottomWorld - 992)
			{
				y = Main.bottomWorld - 992;
			}
			if (y < 992)
			{
				y = 992;
			}

			SendTileSquare((int) (x/16), (int) (y/16), 15);
			TPlayer.Teleport(new Vector2(x, y), style);
			NetMessage.SendData((int)PacketTypes.Teleport, -1, -1, "", 0, TPlayer.whoAmI, x, y, style);
			return true;
		}

		/// <summary>
		/// Heals the player.
		/// </summary>
		/// <param name="health">Heal health amount.</param>
		public void Heal(int health = 600)
		{
			NetMessage.SendData((int)PacketTypes.PlayerHealOther, -1, -1, "", this.TPlayer.whoAmI, health);
		}

		/// <summary>
		/// Spawns the player at his spawn point.
		/// </summary>
		public void Spawn()
		{			
			if (this.sX > 0 && this.sY > 0)
			{
				Spawn(this.sX, this.sY);
			}
			else
			{
				Spawn(TPlayer.SpawnX, TPlayer.SpawnY);
			}
		}

		/// <summary>
		/// Spawns the player at the given coordinates.
		/// </summary>
		/// <param name="tilex">The X coordinate.</param>
		/// <param name="tiley">The Y coordinate.</param>
		public void Spawn(int tilex, int tiley)
		{
			using (var ms = new MemoryStream())
			{
				var msg = new SpawnMsg
							{
								PlayerIndex = (byte) Index,
								TileX = (short)tilex,
								TileY = (short)tiley
							};
				msg.PackFull(ms);
				SendRawData(ms.ToArray());
			}
		}

		/// <summary>
		/// Removes the projectile with the given index and owner.
		/// </summary>
		/// <param name="index">The projectile's index.</param>
		/// <param name="owner">The projectile's owner.</param>
		public void RemoveProjectile(int index, int owner)
		{
			using (var ms = new MemoryStream())
			{
				var msg = new ProjectileRemoveMsg
							{
								Index = (short) index,
								Owner = (byte) owner
							};
				msg.PackFull(ms);
				SendRawData(ms.ToArray());
			}
		}

		public virtual bool SendTileSquare(int x, int y, int size = 10)
		{
			try
			{
				int num = (size - 1)/2;
				int m_x = 0;
				int m_y = 0;

				if (x - num < 0)
				{
					m_x = 0;
				}
				else
				{
					m_x = x - num;
				}

				if (y - num < 0)
				{
					m_y = 0;
				}
				else
				{
					m_y = y - num;
				}

				if (m_x + size > Main.maxTilesX)
				{
					m_x = Main.maxTilesX - size;
				}

				if (m_y + size > Main.maxTilesY)
				{
					m_y = Main.maxTilesY - size;
				}

				SendData(PacketTypes.TileSendSquare, "", size, m_x, m_y);
				return true;
			}
			catch (IndexOutOfRangeException)
			{
				// This is expected if square exceeds array.
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}
			return false;
		}

		/// <summary>
		/// Gives an item to the player. Includes banned item spawn prevention to check if the player can spawn the item.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="stack"></param>
		/// <param name="prefix"></param>
		/// <returns>True or false, depending if the item passed the check or not.</returns>
		public bool GiveItemCheck(int type, string name, int width, int height, int stack, int prefix = 0)
		{
			if ((TShock.Itembans.ItemIsBanned(name) && TShock.Config.PreventBannedItemSpawn) && 
				(TShock.Itembans.ItemIsBanned(name, this) || !TShock.Config.AllowAllowedGroupsToSpawnBannedItems))
					return false;

			GiveItem(type,name,width,height,stack,prefix);
			return true;
		}

		/// <summary>
		/// Gives an item to the player.
		/// </summary>
		/// <param name="type">The item's netID.</param>
		/// <param name="name">The tiem's name.</param>
		/// <param name="width">The item's width.</param>
		/// <param name="height">The item's height.</param>
		/// <param name="stack">The item's stack.</param>
		/// <param name="prefix">The item's prefix.</param>
		public virtual void GiveItem(int type, string name, int width, int height, int stack, int prefix = 0)
		{
			int itemid = Item.NewItem((int) X, (int) Y, width, height, type, stack, true, prefix, true);

			// This is for special pickaxe/hammers/swords etc
			Main.item[itemid].SetDefaults(name);
			// The set default overrides the wet and stack set by NewItem
			Main.item[itemid].wet = Collision.WetCollision(Main.item[itemid].position, Main.item[itemid].width,
															 Main.item[itemid].height);
			Main.item[itemid].stack = stack;
			Main.item[itemid].owner = Index;
			Main.item[itemid].prefix = (byte) prefix;
			Main.item[itemid].noGrabDelay = 1;
			Main.item[itemid].velocity = Main.player[this.Index].velocity;
			NetMessage.SendData((int)PacketTypes.ItemDrop, -1, -1, "", itemid, 0f, 0f, 0f);
			NetMessage.SendData((int)PacketTypes.ItemOwner, -1, -1, "", itemid, 0f, 0f, 0f);
		}

		/// <summary>
		/// Sends an information message to the player.
		/// </summary>
		/// <param name="msg">The message.</param>
		public virtual void SendInfoMessage(string msg)
		{
			SendMessage(msg, Color.Yellow);
		}

		/// <summary>
		/// Sends an information message to the player.
		/// Replaces format items in the message with the string representation of a specified object.
		/// </summary>
		/// <param name="format">The message.</param>
		/// <param name="args">An array of objects to format.</param>
		public void SendInfoMessage(string format, params object[] args)
		{
			SendInfoMessage(string.Format(format, args));
		}

		/// <summary>
		/// Sends a success message to the player.
		/// </summary>
		/// <param name="msg">The message.</param>
		public virtual void SendSuccessMessage(string msg)
		{
			SendMessage(msg, Color.Green);
		}

		/// <summary>
		/// Sends a success message to the player.
		/// Replaces format items in the message with the string representation of a specified object.
		/// </summary>
		/// <param name="format">The message.</param>
		/// <param name="args">An array of objects to format.</param>
		public void SendSuccessMessage(string format, params object[] args)
		{
			SendSuccessMessage(string.Format(format, args));
		}

		/// <summary>
		/// Sends a warning message to the player.
		/// </summary>
		/// <param name="msg">The message.</param>
		public virtual void SendWarningMessage(string msg)
		{
			SendMessage(msg, Color.OrangeRed);
		}

		/// <summary>
		/// Sends a warning message to the player.
		/// Replaces format items in the message with the string representation of a specified object.
		/// </summary>
		/// <param name="format">The message.</param>
		/// <param name="args">An array of objects to format.</param>
		public void SendWarningMessage(string format, params object[] args)
		{
			SendWarningMessage(string.Format(format, args));
		}

		/// <summary>
		/// Sends an error message to the player.
		/// </summary>
		/// <param name="msg">The message.</param>
		public virtual void SendErrorMessage(string msg)
		{
			SendMessage(msg, Color.Red);
		}

		/// <summary>
		/// Sends an error message to the player.
		/// Replaces format items in the message with the string representation of a specified object
		/// </summary>
		/// <param name="format">The message.</param>
		/// <param name="args">An array of objects to format.</param>
		public void SendErrorMessage(string format, params object[] args)
		{
			SendErrorMessage(string.Format(format, args));
		}

		/// <summary>
		/// Sends a message with the specified color. 
		/// </summary>
		/// <param name="msg">The message.</param>
		/// <param name="color">The message color.</param>
		public virtual void SendMessage(string msg, Color color)
		{
			SendMessage(msg, color.R, color.G, color.B);
		}

		/// <summary>
		/// Sends a message with the specified RGB color.
		/// </summary>
		/// <param name="msg">The message.</param>
		/// <param name="red">The amount of red color to factor in. Max: 255.</param>
		/// <param name="green">The amount of green color to factor in. Max: 255</param>
		/// <param name="blue">The amount of blue color to factor in. Max: 255</param>
		public virtual void SendMessage(string msg, byte red, byte green, byte blue)
		{
			if (msg.Contains("\n"))
			{
				string[] msgs = msg.Split('\n');
				foreach (string message in msgs)
				{
					SendMessage(message, red, green, blue);
				}
				return;
			}
			SendData(PacketTypes.SmartTextMessage, msg, 255, red, green, blue, -1);
		}

		/// <summary>
		/// Sends a message to the player with the specified RGB color.
		/// </summary>
		/// <param name="msg">The message.</param>
		/// <param name="red">The amount of red color to factor in. Max: 255.</param>
		/// <param name="green">The amount of green color to factor in. Max: 255.</param>
		/// <param name="blue">The amount of blue color to factor in. Max: 255.</param>
		/// <param name="ply">The player who receives the message.</param>
		public virtual void SendMessageFromPlayer(string msg, byte red, byte green, byte blue, int ply)
		{
			if (msg.Contains("\n"))
			{
				string[] msgs = msg.Split('\n');
				foreach (string message in msgs)
				{
					SendMessageFromPlayer(message, red, green, blue, ply);
				}
				return;
			}
			SendDataFromPlayer(PacketTypes.SmartTextMessage, ply, msg, red, green, blue, -1);
		}

		/// <summary>
		/// Wounds the player with the given damage.
		/// </summary>
		/// <param name="damage">The amount of damage the player will take.</param>
		public virtual void DamagePlayer(int damage)
		{
			NetMessage.SendData((int) PacketTypes.PlayerDamage, -1, -1, "", Index, ((new Random()).Next(-1, 1)), damage,
								(float) 0);
		}

		/// <summary>
		/// Sets the player's team.
		/// </summary>
		/// <param name="team">The team color index.</param>
		public virtual void SetTeam(int team)
		{
			Main.player[Index].team = team;
			NetMessage.SendData((int)PacketTypes.PlayerTeam, -1, -1, "", Index);
			NetMessage.SendData((int)PacketTypes.PlayerTeam, -1, Index, "", Index);
		}

		private DateTime LastDisableNotification = DateTime.UtcNow;

		/// <summary>
		/// Represents the ID of the chest that the player is viewing.
		/// </summary>
		public int ActiveChest = -1;

		/// <summary>
		/// Represents the current item the player is holding.
		/// </summary>
		public Item ItemInHand = new Item();

		/// <summary>
		/// Disables the player for the given <paramref name="reason"/>.
		/// </summary>
		/// <param name="reason">The reason why the player was disabled.</param>
		/// <param name="displayConsole">Whether or not to log this event to the console.</param>
		[Obsolete("Use Disable(string, DisableFlags)")]
		public virtual void Disable(string reason = "", bool displayConsole = true)
		{
			if (displayConsole)
			{
				Disable(reason, DisableFlags.WriteToConsole);
			}
			else
			{
				Disable(reason, DisableFlags.WriteToLog);
			}
		}

		/// <summary>
		/// Disables the player for the given <paramref name="reason"/>
		/// </summary>
		/// <param name="reason">The reason why the player was disabled.</param>
		/// <param name="flags">Flags to dictate where this event is logged to.</param>
		public virtual void Disable(string reason = "", DisableFlags flags = DisableFlags.WriteToLog)
		{
			LastThreat = DateTime.UtcNow;
			SetBuff(BuffID.Frozen, 330, true);
			SetBuff(BuffID.Stoned, 330, true);
			SetBuff(BuffID.Webbed, 330, true);

			if (ActiveChest != -1)
			{
				ActiveChest = -1;
				SendData(PacketTypes.ChestOpen, "", -1);
			}

			if (!string.IsNullOrEmpty(reason))
			{
				if ((DateTime.UtcNow - LastDisableNotification).TotalMilliseconds > 5000)
				{
					if (flags.HasFlag(DisableFlags.WriteToConsole))
					{
						if (flags.HasFlag(DisableFlags.WriteToLog))
						{
							TShock.Log.ConsoleInfo("Player {0} has been disabled for {1}.", Name, reason);
						}
						else
						{
							Server.SendInfoMessage("Player {0} has been disabled for {1}.", Name, reason);
						}
					}

					LastDisableNotification = DateTime.UtcNow;
				}
			}

			/*
			 * Calling new StackTrace() is incredibly expensive, and must be disabled
			 * in release builds.  Use a conditional call instead.
			 */
			LogStackFrame();
		}

		[Conditional("DEBUG")]
		private void LogStackFrame()
		{
			var trace = new StackTrace();
			StackFrame frame = null;
			frame = trace.GetFrame(1);
			if (frame != null && frame.GetMethod().DeclaringType != null)
				TShock.Log.Debug(frame.GetMethod().DeclaringType.Name + " called Disable().");
		}

		/// <summary>
		/// Annoys the player for a specified amount of time.
		/// </summary>
		/// <param name="time">The</param>
		public virtual void Whoopie(object time)
		{
			var time2 = (int) time;
			var launch = DateTime.UtcNow;
			var startname = Name;
			SendInfoMessage("You are now being annoyed.");
			while ((DateTime.UtcNow - launch).TotalSeconds < time2 && startname == Name)
			{
				SendData(PacketTypes.NpcSpecial, number: Index, number2: 2f);
				Thread.Sleep(50);
			}
		}

		/// <summary>
		/// Applies a buff to the player.
		/// </summary>
		/// <param name="type">The buff type.</param>
		/// <param name="time">The buff duration.</param>
		/// <param name="bypass"></param>
		public virtual void SetBuff(int type, int time = 3600, bool bypass = false)
		{
			if ((DateTime.UtcNow - LastThreat).TotalMilliseconds < 5000 && !bypass)
				return;

			SendData(PacketTypes.PlayerAddBuff, number: Index, number2: type, number3: time);
		}

		//Todo: Separate this into a few functions. SendTo, SendToAll, etc
		/// <summary>
		/// Sends data to the player.
		/// </summary>
		/// <param name="msgType">The sent packet</param>
		/// <param name="text">The packet text.</param>
		/// <param name="number"></param>
		/// <param name="number2"></param>
		/// <param name="number3"></param>
		/// <param name="number4"></param>
		/// <param name="number5"></param>
		public virtual void SendData(PacketTypes msgType, string text = "", int number = 0, float number2 = 0f,
			float number3 = 0f, float number4 = 0f, int number5 = 0)
		{
			if (RealPlayer && !ConnectionAlive)
				return;

			NetMessage.SendData((int) msgType, Index, -1, text, number, number2, number3, number4, number5);
		}

		/// <summary>
		/// Sends data from the given player.
		/// </summary>
		/// <param name="msgType">The sent packet.</param>
		/// <param name="ply">The packet sender.</param>
		/// <param name="text">The packet text.</param>
		/// <param name="number2"></param>
		/// <param name="number3"></param>
		/// <param name="number4"></param>
		/// <param name="number5"></param>
		public virtual void SendDataFromPlayer(PacketTypes msgType, int ply, string text = "", float number2 = 0f,
			float number3 = 0f, float number4 = 0f, int number5 = 0)
		{
			if (RealPlayer && !ConnectionAlive)
				return;

			NetMessage.SendData((int) msgType, Index, -1, text, ply, number2, number3, number4, number5);
		}

		/// <summary>
		/// Sends raw data to the player's socket object.
		/// </summary>
		/// <param name="data">The data to send.</param>
		public virtual void SendRawData(byte[] data)
		{
			if (!RealPlayer || !ConnectionAlive)
				return;

			Netplay.Clients[Index].Socket.AsyncSend(data, 0, data.Length, Netplay.Clients[Index].ServerWriteCallBack);
		}

		/// <summary>
		/// Adds a command callback to a specified command string.
		/// </summary>
		/// <param name="name">The string representing the command i.e "yes" == /yes</param>
		/// <param name="callback">The method that will be executed on confirmation ie user accepts</param>
		public void AddResponse( string name, Action<object> callback)
		{
			if( AwaitingResponse.ContainsKey(name))
			{
				AwaitingResponse.Remove(name);
			}

			AwaitingResponse.Add(name, callback);
		}

		/// <summary>
		/// Checks to see if a player has a specific permission.
		/// Fires the <see cref="PlayerHooks.OnPlayerPermission"/> hook which may be handled to override permission checks.
		/// If the OnPlayerPermission hook is not handled and the player is assigned a temporary group, this method calls <see cref="Group.HasPermission"/> on the temporary group and returns the result.
		/// If the OnPlayerPermission hook is not handled and the player is not assigned a temporary group, this method calls <see cref="Group.HasPermission"/> on the player's current group.
		/// </summary>
		/// <param name="permission">The permission to check.</param>
		/// <returns>True if the player has that permission.</returns>
		public bool HasPermission(string permission)
		{
			if (PlayerHooks.OnPlayerPermission(this, permission))
				return true;

			if (tempGroup != null)
				return tempGroup.HasPermission(permission);
			else
				return Group.HasPermission(permission);
		}
	}

	public class TSRestPlayer : TSPlayer
	{
		internal List<string> CommandOutput = new List<string>();

		public TSRestPlayer(string playerName, Group playerGroup): base(playerName)
		{
			Group = playerGroup;
			AwaitingResponse = new Dictionary<string, Action<object>>();
		}

		public override void SendMessage(string msg, Color color)
		{
			SendMessage(msg, color.R, color.G, color.B);
		}

		public override void SendMessage(string msg, byte red, byte green, byte blue)
		{
			this.CommandOutput.Add(msg);
		}

		public override void SendInfoMessage(string msg)
		{
			SendMessage(msg, Color.Yellow);
		}

		public override void SendSuccessMessage(string msg)
		{
			SendMessage(msg, Color.Green);
		}

		public override void SendWarningMessage(string msg)
		{
			SendMessage(msg, Color.OrangeRed);
		}

		public override void SendErrorMessage(string msg)
		{
			SendMessage(msg, Color.Red);
		}

		public List<string> GetCommandOutput()
		{
			return this.CommandOutput;
		}
	}
}
