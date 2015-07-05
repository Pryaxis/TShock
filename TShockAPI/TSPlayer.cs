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
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Timers;
using Terraria;
using TShockAPI.DB;
using TShockAPI.Net;
using Timer = System.Timers.Timer;

namespace TShockAPI
{
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

		public Vector2 TeleportCoords = new Vector2(-1, -1);

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
		/// Whether the player is a real, human, player on the server.
		/// </summary>
		public bool RealPlayer
		{
			get { return Index >= 0 && Index < Main.maxNetPlayers && Main.player[Index] != null; }
		}

		public bool ConnectionAlive
		{
			get
			{
				return RealPlayer
					&& (Netplay.Clients[Index] != null && Netplay.Clients[Index].IsActive && !Netplay.Clients[Index].PendingTermination);
			}
		}

		/// <summary>
		/// Gets the player's selected item.
		/// </summary>
		public Item SelectedItem
		{
			get { return TPlayer.inventory[TPlayer.selectedItem]; }
		}

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

		public IEnumerable<Item> Accessories
		{
			get
			{
				for (int i = 3; i < 8; i++)
					yield return TPlayer.armor[i];
			}
		}

		/// <summary>
		/// Saves the player's inventory to SSI
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
		/// Terraria Player
		/// </summary>
		public Player TPlayer
		{
			get { return FakePlayer ?? Main.player[Index]; }
		}

		public string Name
		{
			get { return TPlayer.name; }
		}

		public bool Active
		{
			get { return TPlayer != null && TPlayer.active; }
		}

		public int Team
		{
			get { return TPlayer.team; }
		}

		public float X
		{
			get { return RealPlayer ? TPlayer.position.X : Main.spawnTileX*16; }
		}

		public float Y
		{
			get { return RealPlayer ? TPlayer.position.Y : Main.spawnTileY*16; }
		}

		public int TileX
		{
			get { return (int) (X/16); }
		}

		public int TileY
		{
			get { return (int) (Y/16); }
		}

		public bool TpLock;

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

		public TSPlayer(int index)
		{
			TilesDestroyed = new Dictionary<Vector2, Tile>();
			TilesCreated = new Dictionary<Vector2, Tile>();
			Index = index;
			Group = Group.DefaultGroup;
			IceTiles = new List<Point>();
			AwaitingResponse = new Dictionary<string, Action<object>>();
		}

		protected TSPlayer(String playerName)
		{
			TilesDestroyed = new Dictionary<Vector2, Tile>();
			TilesCreated = new Dictionary<Vector2, Tile>();
			Index = -1;
			FakePlayer = new Player {name = playerName, whoAmI = -1};
			Group = Group.DefaultGroup;
			AwaitingResponse = new Dictionary<string, Action<object>>();
		}

		public virtual void Disconnect(string reason)
		{
			SendData(PacketTypes.Disconnect, reason);
		}

		public virtual void Flush()
		{
			var client = Netplay.Clients[Index];
			if (client == null)
				return;

			TShock.PacketBuffer.Flush(client);
		}


		public void TempGroupTimerElapsed(object sender, ElapsedEventArgs args)
		{
			SendWarningMessage("Your temporary group access has expired.");

			tempGroup = null;
			if (sender != null)
			{
				((Timer)sender).Stop();
			}
		}

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

		public void Heal(int health = 600)
		{
			NetMessage.SendData((int)PacketTypes.PlayerHealOther, -1, -1, "", this.TPlayer.whoAmI, health);
		}

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

		public bool GiveItemCheck(int type, string name, int width, int height, int stack, int prefix = 0)
		{
			if ((TShock.Itembans.ItemIsBanned(name) && TShock.Config.PreventBannedItemSpawn) && 
				(TShock.Itembans.ItemIsBanned(name, this) || !TShock.Config.AllowAllowedGroupsToSpawnBannedItems))
					return false;

			GiveItem(type,name,width,height,stack,prefix);
			return true;
		}

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

		public virtual void SendInfoMessage(string msg)
		{
			SendMessage(msg, Color.Yellow);
		}

		public void SendInfoMessage(string format, params object[] args)
		{
			SendInfoMessage(string.Format(format, args));
		}

		public virtual void SendSuccessMessage(string msg)
		{
			SendMessage(msg, Color.Green);
		}

		public void SendSuccessMessage(string format, params object[] args)
		{
			SendSuccessMessage(string.Format(format, args));
		}

		public virtual void SendWarningMessage(string msg)
		{
			SendMessage(msg, Color.OrangeRed);
		}

		public void SendWarningMessage(string format, params object[] args)
		{
			SendWarningMessage(string.Format(format, args));
		}

		public virtual void SendErrorMessage(string msg)
		{
			SendMessage(msg, Color.Red);
		}

		public void SendErrorMessage(string format, params object[] args)
		{
			SendErrorMessage(string.Format(format, args));
		}

		public virtual void SendMessage(string msg, Color color)
		{
			SendMessage(msg, color.R, color.G, color.B);
		}

		public virtual void SendMessage(string msg, byte red, byte green, byte blue)
		{
			SendData(PacketTypes.ChatText, msg, 255, red, green, blue);
		}

		public virtual void SendMessageFromPlayer(string msg, byte red, byte green, byte blue, int ply)
		{
			SendDataFromPlayer(PacketTypes.ChatText, ply, msg, red, green, blue, 0);
		}

		public virtual void DamagePlayer(int damage)
		{
			NetMessage.SendData((int) PacketTypes.PlayerDamage, -1, -1, "", Index, ((new Random()).Next(-1, 1)), damage,
								(float) 0);
		}

		public virtual void SetTeam(int team)
		{
			Main.player[Index].team = team;
			SendData(PacketTypes.PlayerTeam, "", Index);
		}

		private DateTime LastDisableNotification = DateTime.UtcNow;
		public int ActiveChest = -1;
		public Item ItemInHand = new Item();

		public virtual void Disable(string reason = "", bool displayConsole = true)
		{
			LastThreat = DateTime.UtcNow;
			SetBuff(33, 330, true); //Weak
			SetBuff(32, 330, true); //Slow
			SetBuff(23, 330, true); //Cursed
			SetBuff(47, 330, true); //Frozen

			if (ActiveChest != -1)
			{
				SendData(PacketTypes.ChestOpen, "", -1);
			}

			if (!string.IsNullOrEmpty(reason))
			{
				if ((DateTime.UtcNow - LastDisableNotification).TotalMilliseconds > 5000)
				{
					if (displayConsole)
					{
						TShock.Log.ConsoleInfo("Player {0} has been disabled for {1}.", Name, reason);
					}
					else
					{
						TShock.Log.Info("Player {0} has been disabled for {1}.", Name, reason);
					}
					LastDisableNotification = DateTime.UtcNow;
				}
			}
			var trace = new StackTrace();
			StackFrame frame = null;
			frame = trace.GetFrame(1);
			if (frame != null && frame.GetMethod().DeclaringType != null)
				TShock.Log.Debug(frame.GetMethod().DeclaringType.Name + " called Disable().");
		}

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

		public virtual void SetBuff(int type, int time = 3600, bool bypass = false)
		{
			if ((DateTime.UtcNow - LastThreat).TotalMilliseconds < 5000 && !bypass)
				return;

			SendData(PacketTypes.PlayerAddBuff, number: Index, number2: type, number3: time);
		}

		//Todo: Separate this into a few functions. SendTo, SendToAll, etc
		public virtual void SendData(PacketTypes msgType, string text = "", int number = 0, float number2 = 0f,
			float number3 = 0f, float number4 = 0f, int number5 = 0)
		{
			if (RealPlayer && !ConnectionAlive)
				return;

			NetMessage.SendData((int) msgType, Index, -1, text, number, number2, number3, number4, number5);
		}

		public virtual void SendDataFromPlayer(PacketTypes msgType, int ply, string text = "", float number2 = 0f,
			float number3 = 0f, float number4 = 0f, int number5 = 0)
		{
			if (RealPlayer && !ConnectionAlive)
				return;

			NetMessage.SendData((int) msgType, Index, -1, text, ply, number2, number3, number4, number5);
		}

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

	public class TSServerPlayer : TSPlayer
	{
		public static string AccountName = "ServerConsole";

		public TSServerPlayer()
			: base("Server")
		{
			Group = new SuperAdminGroup();
			User = new User{Name = AccountName};
		}

		public override void SendErrorMessage(string msg)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(msg);
			Console.ResetColor();
		}

		public override void SendInfoMessage(string msg)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(msg);
			Console.ResetColor();
		}

		public override void SendSuccessMessage(string msg)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(msg);
			Console.ResetColor();
		}

		public override void SendWarningMessage(string msg)
		{
			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.WriteLine(msg);
			Console.ResetColor();
		}

		public override void SendMessage(string msg, Color color)
		{
			SendMessage(msg, color.R, color.G, color.B);
		}

		public override void SendMessage(string msg, byte red, byte green, byte blue)
		{
			Console.WriteLine(msg);
		}

		public void SetFullMoon()
		{
			Main.dayTime = false;
			Main.moonPhase = 0;
			Main.time = 0.0;
			TSPlayer.All.SendData(PacketTypes.WorldInfo);
		}

		public void SetBloodMoon(bool bloodMoon)
		{
			if (bloodMoon)
			{
				Main.dayTime = false;
				Main.bloodMoon = true;
				Main.time = 0.0;
			}
			else
				Main.bloodMoon = false;
			TSPlayer.All.SendData(PacketTypes.WorldInfo);
		}

		public void SetFrostMoon(bool snowMoon)
		{
			if (snowMoon)
			{
				Main.dayTime = false;
				Main.snowMoon = true;
				Main.time = 0.0;
			}
			else
				Main.snowMoon = false;
			TSPlayer.All.SendData(PacketTypes.WorldInfo);
		}

		public void SetPumpkinMoon(bool pumpkinMoon)
		{
			if (pumpkinMoon)
			{
				Main.dayTime = false;
				Main.pumpkinMoon = true;
				Main.time = 0.0;
			}
			else
				Main.pumpkinMoon = false;
			TSPlayer.All.SendData(PacketTypes.WorldInfo);
		}
		
		public void SetEclipse(bool eclipse)
		{
			if (eclipse)
			{
				Main.dayTime = Main.eclipse = true;
				Main.time = 0.0;
			}
			else
				Main.eclipse = false;
			TSPlayer.All.SendData(PacketTypes.WorldInfo);
		}

		public void SetTime(bool dayTime, double time)
		{
			Main.dayTime = dayTime;
			Main.time = time;
			TSPlayer.All.SendData(PacketTypes.TimeSet, "", dayTime ? 1 : 0, (int)time, Main.sunModY, Main.moonModY);
		}

		public void SpawnNPC(int type, string name, int amount, int startTileX, int startTileY, int tileXRange = 100,
			int tileYRange = 50)
		{
			for (int i = 0; i < amount; i++)
			{
				int spawnTileX;
				int spawnTileY;
				TShock.Utils.GetRandomClearTileWithInRange(startTileX, startTileY, tileXRange, tileYRange, out spawnTileX,
															 out spawnTileY);
				int npcid = NPC.NewNPC(spawnTileX*16, spawnTileY*16, type, 0);
				// This is for special slimes
				Main.npc[npcid].SetDefaults(name);
			}
		}

		public void StrikeNPC(int npcid, int damage, float knockBack, int hitDirection)
		{
			// Main.rand is thread static.
			if (Main.rand == null)
				Main.rand = new Random();

			Main.npc[npcid].StrikeNPC(damage, knockBack, hitDirection);
			NetMessage.SendData((int) PacketTypes.NpcStrike, -1, -1, "", npcid, damage, knockBack, hitDirection);
		}

		public void RevertTiles(Dictionary<Vector2, Tile> tiles)
		{
			// Update Main.Tile first so that when tile sqaure is sent it is correct
			foreach (KeyValuePair<Vector2, Tile> entry in tiles)
			{
				Main.tile[(int) entry.Key.X, (int) entry.Key.Y] = entry.Value;
			}
			// Send all players updated tile sqaures
			foreach (Vector2 coords in tiles.Keys)
			{
				All.SendTileSquare((int) coords.X, (int) coords.Y, 3);
			}
		}
	}

	public class PlayerData
	{
		public NetItem[] inventory = new NetItem[NetItem.MaxInventory];
		public int health = TShock.ServerSideCharacterConfig.StartingHealth;
		public int maxHealth = TShock.ServerSideCharacterConfig.StartingHealth;
		public int mana = TShock.ServerSideCharacterConfig.StartingMana;
		public int maxMana = TShock.ServerSideCharacterConfig.StartingMana;
		public bool exists;
		public int spawnX= -1;
		public int spawnY= -1;
		public int? hair;
		public byte hairDye;
		public Color? hairColor;
		public Color? pantsColor;
		public Color? shirtColor;
		public Color? underShirtColor;
		public Color? shoeColor;
		public Color? skinColor;
		public Color? eyeColor;
		public bool[] hideVisuals;
		public int questsCompleted;

		public PlayerData(TSPlayer player)
		{
			for (int i = 0; i < NetItem.MaxInventory; i++)
			{
				this.inventory[i] = new NetItem();
			}

			for (int i = 0; i < TShock.ServerSideCharacterConfig.StartingInventory.Count; i++)
			{
				var item = TShock.ServerSideCharacterConfig.StartingInventory[i];
				StoreSlot(i, item.NetId, item.PrefixId, item.Stack);
			}
		}

		public void StoreSlot(int slot, int netID, byte prefix, int stack)
		{
			if(slot > (this.inventory.Length - 1)) //if the slot is out of range then dont save
			{
				return;
			}

			this.inventory[slot] = new NetItem(netID, stack, prefix);
		}

		public void CopyCharacter(TSPlayer player)
		{
			this.health = player.TPlayer.statLife > 0 ? player.TPlayer.statLife : 1;
			this.maxHealth = player.TPlayer.statLifeMax;
			this.mana = player.TPlayer.statMana;
			this.maxMana = player.TPlayer.statManaMax;
			if (player.sX > 0 && player.sY > 0)
			{
				this.spawnX = player.sX;
				this.spawnY = player.sY;
			}
			else
			{
				this.spawnX = player.TPlayer.SpawnX;
				this.spawnY = player.TPlayer.SpawnY;
			}
			this.hair = player.TPlayer.hair;
			this.hairDye = player.TPlayer.hairDye;
			this.hairColor = player.TPlayer.hairColor;
			this.pantsColor = player.TPlayer.pantsColor;
			this.shirtColor = player.TPlayer.shirtColor;
			this.underShirtColor = player.TPlayer.underShirtColor;
			this.shoeColor = player.TPlayer.shoeColor;
			this.hideVisuals = player.TPlayer.hideVisual;
			this.skinColor = player.TPlayer.skinColor;
			this.eyeColor = player.TPlayer.eyeColor;
			this.questsCompleted = player.TPlayer.anglerQuestsFinished;

			Item[] inventory = player.TPlayer.inventory;
			Item[] armor = player.TPlayer.armor;
			Item[] dye = player.TPlayer.dye;
			for (int i = 0; i < NetItem.MaxInventory; i++)
			{
				if (i < NetItem.MaxInventory - (NetItem.ArmorSlots + NetItem.DyeSlots))
				{
					this.inventory[i] = (NetItem)inventory[i];
				}
				else if (i < NetItem.MaxInventory - NetItem.DyeSlots)
				{
					var index = i - (NetItem.MaxInventory - (NetItem.ArmorSlots + NetItem.DyeSlots));
					this.inventory[i] = (NetItem)armor[index];
				}
				else
				{
					var index = i - (NetItem.MaxInventory - NetItem.DyeSlots);
					this.inventory[i] = (NetItem)dye[index];
				}
			}
		}

		public void RestoreCharacter(TSPlayer player)
		{
			// Start ignoring SSC-related packets! This is critical so that we don't send or receive dirty data!
			player.IgnoreSSCPackets = true;

			player.TPlayer.statLife = this.health;
			player.TPlayer.statLifeMax = this.maxHealth;
			player.TPlayer.statMana = this.maxMana;
			player.TPlayer.statManaMax = this.maxMana;
			player.TPlayer.SpawnX = this.spawnX;
			player.TPlayer.SpawnY = this.spawnY;
			player.sX = this.spawnX;
			player.sY = this.spawnY;
			player.TPlayer.hairDye = this.hairDye;
			player.TPlayer.anglerQuestsFinished = this.questsCompleted;

			if (this.hair != null)
				player.TPlayer.hair = this.hair.Value;
			if (this.hairColor != null)
				player.TPlayer.hairColor = this.hairColor.Value;
			if (this.pantsColor != null)
				player.TPlayer.pantsColor = this.pantsColor.Value;
			if (this.shirtColor != null)
				player.TPlayer.shirtColor = this.shirtColor.Value;
			if (this.underShirtColor != null)
				player.TPlayer.underShirtColor = this.underShirtColor.Value;
			if (this.shoeColor != null)
				player.TPlayer.shoeColor = this.shoeColor.Value;
			if (this.skinColor != null)
				player.TPlayer.skinColor = this.skinColor.Value;
			if (this.eyeColor != null)
				player.TPlayer.eyeColor = this.eyeColor.Value;

			if (this.hideVisuals != null)
				player.TPlayer.hideVisual = this.hideVisuals;
			else
				player.TPlayer.hideVisual = new bool[player.TPlayer.hideVisual.Length];
			
			for (int i = 0; i < NetItem.MaxInventory; i++)
			{
				if (i < NetItem.MaxInventory - (NetItem.ArmorSlots + NetItem.DyeSlots))
				{
					player.TPlayer.inventory[i].netDefaults(this.inventory[i].NetId);

					if (player.TPlayer.inventory[i].netID != 0)
					{
						player.TPlayer.inventory[i].stack = this.inventory[i].Stack;
						player.TPlayer.inventory[i].prefix = this.inventory[i].PrefixId;
					}
				}
				else if (i < NetItem.MaxInventory - NetItem.DyeSlots)
				{
					var index = i - (NetItem.MaxInventory - (NetItem.ArmorSlots + NetItem.DyeSlots));
					player.TPlayer.armor[index].netDefaults(this.inventory[i].NetId);

					if (player.TPlayer.armor[index].netID != 0)
					{
						player.TPlayer.armor[index].stack = this.inventory[i].Stack;
						player.TPlayer.armor[index].prefix = (byte)this.inventory[i].PrefixId;
					}
				}
				else
				{
					var index = i - (NetItem.MaxInventory - NetItem.DyeSlots);
					player.TPlayer.dye[index].netDefaults(this.inventory[i].NetId);

					if (player.TPlayer.dye[index].netID != 0)
					{
						player.TPlayer.dye[index].stack = this.inventory[i].Stack;
						player.TPlayer.dye[index].prefix = (byte)this.inventory[i].PrefixId;
					}
				}
			}

			for (int k = 0; k < 59; k++)
			{
				NetMessage.SendData(5, -1, -1, Main.player[player.Index].inventory[k].name, player.Index, (float)k, (float)Main.player[player.Index].inventory[k].prefix, 0f, 0);
			}
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].armor[0].name, player.Index, 59f, (float)Main.player[player.Index].armor[0].prefix, 0f, 0);
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].armor[1].name, player.Index, 60f, (float)Main.player[player.Index].armor[1].prefix, 0f, 0);
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].armor[2].name, player.Index, 61f, (float)Main.player[player.Index].armor[2].prefix, 0f, 0);
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].armor[3].name, player.Index, 62f, (float)Main.player[player.Index].armor[3].prefix, 0f, 0);
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].armor[4].name, player.Index, 63f, (float)Main.player[player.Index].armor[4].prefix, 0f, 0);
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].armor[5].name, player.Index, 64f, (float)Main.player[player.Index].armor[5].prefix, 0f, 0);
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].armor[6].name, player.Index, 65f, (float)Main.player[player.Index].armor[6].prefix, 0f, 0);
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].armor[7].name, player.Index, 66f, (float)Main.player[player.Index].armor[7].prefix, 0f, 0);
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].armor[8].name, player.Index, 67f, (float)Main.player[player.Index].armor[8].prefix, 0f, 0);
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].armor[9].name, player.Index, 68f, (float)Main.player[player.Index].armor[9].prefix, 0f, 0);
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].armor[10].name, player.Index, 69f, (float)Main.player[player.Index].armor[10].prefix, 0f, 0);
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].armor[11].name, player.Index, 70f, (float)Main.player[player.Index].armor[11].prefix, 0f, 0);
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].armor[12].name, player.Index, 71f, (float)Main.player[player.Index].armor[12].prefix, 0f, 0);
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].armor[13].name, player.Index, 72f, (float)Main.player[player.Index].armor[13].prefix, 0f, 0);
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].armor[14].name, player.Index, 73f, (float)Main.player[player.Index].armor[14].prefix, 0f, 0);
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].armor[15].name, player.Index, 74f, (float)Main.player[player.Index].armor[15].prefix, 0f, 0);
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].dye[0].name, player.Index, 75f, (float)Main.player[player.Index].dye[0].prefix, 0f, 0);
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].dye[1].name, player.Index, 76f, (float)Main.player[player.Index].dye[1].prefix, 0f, 0);
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].dye[2].name, player.Index, 77f, (float)Main.player[player.Index].dye[2].prefix, 0f, 0);
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].dye[3].name, player.Index, 78f, (float)Main.player[player.Index].dye[3].prefix, 0f, 0);
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].dye[4].name, player.Index, 79f, (float)Main.player[player.Index].dye[4].prefix, 0f, 0);
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].dye[5].name, player.Index, 80f, (float)Main.player[player.Index].dye[5].prefix, 0f, 0);
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].dye[6].name, player.Index, 81f, (float)Main.player[player.Index].dye[6].prefix, 0f, 0);
			NetMessage.SendData(5, -1, -1, Main.player[player.Index].dye[7].name, player.Index, 82f, (float)Main.player[player.Index].dye[7].prefix, 0f, 0);
			NetMessage.SendData(4, -1, -1, player.Name, player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData(42, -1, -1, "", player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData(16, -1, -1, "", player.Index, 0f, 0f, 0f, 0);

			for (int k = 0; k < 59; k++)
			{
				NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].inventory[k].name, player.Index, (float)k, (float)Main.player[player.Index].inventory[k].prefix, 0f, 0);
			}

			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].armor[0].name, player.Index, 59f, (float)Main.player[player.Index].armor[0].prefix, 0f, 0);
			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].armor[1].name, player.Index, 60f, (float)Main.player[player.Index].armor[1].prefix, 0f, 0);
			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].armor[2].name, player.Index, 61f, (float)Main.player[player.Index].armor[2].prefix, 0f, 0);
			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].armor[3].name, player.Index, 62f, (float)Main.player[player.Index].armor[3].prefix, 0f, 0);
			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].armor[4].name, player.Index, 63f, (float)Main.player[player.Index].armor[4].prefix, 0f, 0);
			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].armor[5].name, player.Index, 64f, (float)Main.player[player.Index].armor[5].prefix, 0f, 0);
			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].armor[6].name, player.Index, 65f, (float)Main.player[player.Index].armor[6].prefix, 0f, 0);
			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].armor[7].name, player.Index, 66f, (float)Main.player[player.Index].armor[7].prefix, 0f, 0);
			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].armor[8].name, player.Index, 67f, (float)Main.player[player.Index].armor[8].prefix, 0f, 0);
			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].armor[9].name, player.Index, 68f, (float)Main.player[player.Index].armor[9].prefix, 0f, 0);
			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].armor[10].name, player.Index, 69f, (float)Main.player[player.Index].armor[10].prefix, 0f, 0);
			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].armor[11].name, player.Index, 70f, (float)Main.player[player.Index].armor[11].prefix, 0f, 0);
			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].armor[12].name, player.Index, 71f, (float)Main.player[player.Index].armor[12].prefix, 0f, 0);
			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].armor[13].name, player.Index, 72f, (float)Main.player[player.Index].armor[13].prefix, 0f, 0);
			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].armor[14].name, player.Index, 73f, (float)Main.player[player.Index].armor[14].prefix, 0f, 0);
			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].armor[15].name, player.Index, 74f, (float)Main.player[player.Index].armor[15].prefix, 0f, 0);
			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].dye[0].name, player.Index, 75f, (float)Main.player[player.Index].dye[0].prefix, 0f, 0);
			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].dye[1].name, player.Index, 76f, (float)Main.player[player.Index].dye[1].prefix, 0f, 0);
			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].dye[2].name, player.Index, 77f, (float)Main.player[player.Index].dye[2].prefix, 0f, 0);
			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].dye[3].name, player.Index, 78f, (float)Main.player[player.Index].dye[3].prefix, 0f, 0);
			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].dye[4].name, player.Index, 79f, (float)Main.player[player.Index].dye[4].prefix, 0f, 0);
			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].dye[5].name, player.Index, 80f, (float)Main.player[player.Index].dye[5].prefix, 0f, 0);
			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].dye[6].name, player.Index, 81f, (float)Main.player[player.Index].dye[6].prefix, 0f, 0);
			NetMessage.SendData(5, player.Index, -1, Main.player[player.Index].dye[7].name, player.Index, 82f, (float)Main.player[player.Index].dye[7].prefix, 0f, 0);
			NetMessage.SendData(4, player.Index, -1, player.Name, player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData(42, player.Index, -1, "", player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData(16, player.Index, -1, "", player.Index, 0f, 0f, 0f, 0);

			for (int k = 0; k < 22; k++)
			{
				player.TPlayer.buffType[k] = 0;
			}
			NetMessage.SendData(50, -1, -1, "", player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData(50, player.Index, -1, "", player.Index, 0f, 0f, 0f, 0);
			NetMessage.SendData(76, -1, -1, "", player.Index);

			NetMessage.SendData(39, player.Index, -1, "", 400);
		}
	}
}
