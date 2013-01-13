/*
TShock, a server mod for Terraria
Copyright (C) 2011-2012 The TShock Team

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
using System.Threading;
using Terraria;
using TShockAPI.Net;

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
        /// The amount of liquid( in tiles ) that the player has placed in the last second.
        /// </summary>
		public int TileLiquidThreshold { get; set; }

        /// <summary>
        /// The number of projectiles created by the player in the last second.
        /// </summary>
		public int ProjectileThreshold { get; set; }

        /// <summary>
        /// A queue of tiles destroyed by the player for reverting.
        /// </summary>
		public Dictionary<Vector2, TileData> TilesDestroyed { get; protected set; }

        /// <summary>
        /// A queue of tiles placed by the player for reverting.
        /// </summary>
		public Dictionary<Vector2, TileData> TilesCreated { get; protected set; }

		public int FirstMaxHP { get; set; }

		public int FirstMaxMP { get; set; }

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

	    private Group group = null;

		public bool ReceivedInfo { get; set; }

        /// <summary>
        /// The players index in the player array( Main.players[] ).
        /// </summary>
		public int Index { get; protected set; }

        /// <summary>
        /// The last time the player changed their team or pvp status.  
        /// </summary>
		public DateTime LastPvpChange;

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

        /// <summary>
        /// The last time a player broke a grief check.
        /// </summary>
		public DateTime LastThreat { get; set; }

        /// <summary>
        /// Not used, can be removed.
        /// </summary>
		public DateTime LastTileChangeNotify { get; set; }

		public bool InitSpawn;

        /// <summary>
        /// Whether the player should see logs.
        /// </summary>
		public bool DisplayLogs = true;

		public Vector2 oldSpawn = Vector2.Zero;

        /// <summary>
        /// The last player that the player whispered with( to or from ).
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
		public string UserAccountName { get; set; }

        /// <summary>
        /// Unused can be removed.
        /// </summary>
		public bool HasBeenSpammedWithBuildMessage;

        /// <summary>
        /// Whether the player is logged in or not.
        /// </summary>
		public bool IsLoggedIn;

        /// <summary>
        /// The player's user id( from the db ).
        /// </summary>
		public int UserID = -1;

        /// <summary>
        /// Whether the player has been nagged about logging in.
        /// </summary>
		public bool HasBeenNaggedAboutLoggingIn;

        public bool TPAllow = true;

        /// <summary>
        /// Whether the player is muted or not.
        /// </summary>
		public bool mute;

		public bool TpLock;

		private Player FakePlayer;

		public bool RequestedSection;

        /// <summary>
        /// The last time the player died.
        /// </summary>
		public DateTime LastDeath { get; set; }

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
				return RealPlayer &&
					   (Netplay.serverSock[Index] != null && Netplay.serverSock[Index].active && !Netplay.serverSock[Index].kill);
			}
		}

		public int State
		{
			get { return Netplay.serverSock[Index].state; }
			set { Netplay.serverSock[Index].state = value; }
		}

		public string IP
		{
			get
			{
				if (string.IsNullOrEmpty(CacheIP))
					return
						CacheIP =
						RealPlayer
							? (Netplay.serverSock[Index].tcpClient.Connected
								? TShock.Utils.GetRealIP(Netplay.serverSock[Index].tcpClient.Client.RemoteEndPoint.ToString())
								: "")
							: "";
				else
					return CacheIP;
			}
		}

        /// <summary>
        /// Saves the player's inventory to SSI
        /// </summary>
        /// <returns>bool - True/false if it saved successfully</returns>
        public bool SaveServerInventory()
        {
            if (!TShock.Config.ServerSideInventory)
            {
                return false;
            }
            try
            {
                PlayerData.CopyInventory(this);
                TShock.InventoryDB.InsertPlayerData(this);
                return true;
            } catch (Exception e)
            {
                Log.Error(e.Message);
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

		public bool InventorySlotAvailable
		{
			get
			{
				bool flag = false;
				if (RealPlayer)
				{
					for (int i = 0; i < 40; i++) //41 is trash can, 42-45 is coins, 46-49 is ammo
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
			TilesDestroyed = new Dictionary<Vector2, TileData>();
			TilesCreated = new Dictionary<Vector2, TileData>();
			Index = index;
			Group = new Group(TShock.Config.DefaultGuestGroupName);
			IceTiles = new List<Point>();
            AwaitingResponse = new Dictionary<string, Action<object>>();
		}

		protected TSPlayer(String playerName)
		{
			TilesDestroyed = new Dictionary<Vector2, TileData>();
			TilesCreated = new Dictionary<Vector2, TileData>();
			Index = -1;
			FakePlayer = new Player {name = playerName, whoAmi = -1};
			Group = new Group(TShock.Config.DefaultGuestGroupName);
            AwaitingResponse = new Dictionary<string, Action<object>>();
		}

		public virtual void Disconnect(string reason)
		{
			SendData(PacketTypes.Disconnect, reason);
		}

		public virtual void Flush()
		{
			var sock = Netplay.serverSock[Index];
			if (sock == null)
				return;

			TShock.PacketBuffer.Flush(sock);
		}


		public void SendWorldInfo(int tilex, int tiley, bool fakeid)
		{
			using (var ms = new MemoryStream())
			{
				var msg = new WorldInfoMsg
							{
								Time = (int) Main.time,
								DayTime = Main.dayTime,
								MoonPhase = (byte) Main.moonPhase,
								BloodMoon = Main.bloodMoon,
								MaxTilesX = Main.maxTilesX,
								MaxTilesY = Main.maxTilesY,
								SpawnX = tilex,
								SpawnY = tiley,
								WorldSurface = (int) Main.worldSurface,
								RockLayer = (int) Main.rockLayer,
								//Sending a fake world id causes the client to not be able to find a stored spawnx/y.
								//This fixes the bed spawn point bug. With a fake world id it wont be able to find the bed spawn.
								WorldID = !fakeid ? Main.worldID : -1,
								WorldFlags = (WorldGen.shadowOrbSmashed ? WorldInfoFlag.OrbSmashed : WorldInfoFlag.None) |
											 (NPC.downedBoss1 ? WorldInfoFlag.DownedBoss1 : WorldInfoFlag.None) |
											 (NPC.downedBoss2 ? WorldInfoFlag.DownedBoss2 : WorldInfoFlag.None) |
											 (NPC.downedBoss3 ? WorldInfoFlag.DownedBoss3 : WorldInfoFlag.None) |
											 (Main.hardMode ? WorldInfoFlag.HardMode : WorldInfoFlag.None) |
											 (NPC.downedClown ? WorldInfoFlag.DownedClown : WorldInfoFlag.None),
								WorldName = TShock.Config.UseServerName ? TShock.Config.ServerName : Main.worldName
							};
				msg.PackFull(ms);
				SendRawData(ms.ToArray());
			}
		}

		public bool Teleport(int tilex, int tiley)
		{
			InitSpawn = false;

			SendWorldInfo(tilex, tiley, true);

			//150 Should avoid all client crash errors
			//The error occurs when a tile trys to update which the client hasnt load yet, Clients only update tiles withen 150 blocks
			//Try 300 if it does not work (Higher number - Longer load times - Less chance of error)
			//Should we properly send sections so that clients don't get tiles twice?
			SendTileSquare(tilex, tiley, 150);

/*	//We shouldn't need this section anymore -it can prevent otherwise acceptable teleportation under some circumstances. 
		
			if (!SendTileSquare(tilex, tiley, 150))
			{
				InitSpawn = true;
				SendWorldInfo(Main.spawnTileX, Main.spawnTileY, false);
				return false;
			}

*/
			Spawn(-1, -1);

			SendWorldInfo(Main.spawnTileX, Main.spawnTileY, false);

			TPlayer.position.X = (float)(tilex * 16 + 8 - TPlayer.width /2);
			TPlayer.position.Y = (float)(tiley * 16 - TPlayer.height);
			//We need to send the tile data again to prevent clients from thinking they *really* destroyed blocks just now.

			SendTileSquare(tilex, tiley, 10);

			return true;
		}

		public void Spawn()
		{
			Spawn(TPlayer.SpawnX, TPlayer.SpawnY);
		}

		public void Spawn(int tilex, int tiley)
		{
			using (var ms = new MemoryStream())
			{
				var msg = new SpawnMsg
							{
								PlayerIndex = (byte) Index,
								TileX = tilex,
								TileY = tiley
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
		int m_x=0;
		int m_y=0;

		if (x - num <0){
		   m_x=0;
		   }else{
		   m_x = x - num;
		   }

		if (y - num <0){
		   m_y=0;
		   }else{
		   m_y = y - num;
		   }

		if (m_x + size > Main.maxTilesX){
		   m_x=Main.maxTilesX - size;
		   }

		if (m_y + size > Main.maxTilesY){
		   m_y=Main.maxTilesY - size;
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
                Log.Error(ex.ToString());
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
			int itemid = Item.NewItem((int) X, (int) Y, width, height, type, stack, true, prefix);

			// This is for special pickaxe/hammers/swords etc
			Main.item[itemid].SetDefaults(name);
			// The set default overrides the wet and stack set by NewItem
			Main.item[itemid].wet = Collision.WetCollision(Main.item[itemid].position, Main.item[itemid].width,
														   Main.item[itemid].height);
			Main.item[itemid].stack = stack;
			Main.item[itemid].owner = Index;
			Main.item[itemid].prefix = (byte) prefix;
			NetMessage.SendData((int) PacketTypes.ItemDrop, -1, -1, "", itemid, 0f, 0f, 0f);
			NetMessage.SendData((int) PacketTypes.ItemOwner, -1, -1, "", itemid, 0f, 0f, 0f);
		}

        public virtual void SendInfoMessage(string msg)
        {
            SendMessage(msg, Color.Yellow);
        }

        public virtual void SendSuccessMessage(string msg)
        {
            SendMessage(msg, Color.Green);
        }

        public virtual void SendWarningMessage(string msg)
        {
            SendMessage(msg, Color.OrangeRed);
        }

        public virtual void SendErrorMessage(string msg)
        {
            SendMessage(msg, Color.Red);
        }

        [Obsolete("Use SendErrorMessage, SendInfoMessage, or SendWarningMessage, or a custom color instead.")]
		public virtual void SendMessage(string msg)
		{
			SendMessage(msg, 0, 255, 0);
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

		public virtual void Disable(string reason = "")
		{
			LastThreat = DateTime.UtcNow;
			SetBuff(33, 330, true); //Weak
			SetBuff(32, 330, true); //Slow
			SetBuff(23, 330, true); //Cursed
			if (!string.IsNullOrEmpty(reason))
				Log.ConsoleInfo(string.Format("Player {0} has been disabled for {1}.", Name, reason));

			var trace = new StackTrace();
			StackFrame frame = null;
			frame = trace.GetFrame(1);
			if (frame != null && frame.GetMethod().DeclaringType != null)
				Log.Debug(frame.GetMethod().DeclaringType.Name + " called Disable().");
		}

		public virtual void Whoopie(object time)
		{
			var time2 = (int) time;
			var launch = DateTime.UtcNow;
			var startname = Name;
			SendMessage("You are now being annoyed.", Color.Red);
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

        public virtual void SendDataFromPlayer(PacketTypes msgType, int ply, string text = "", float number2 = 0f, float number3 = 0f, float number4 = 0f, int number5 = 0)
        {
            if (RealPlayer && !ConnectionAlive)
                return;

            NetMessage.SendData((int) msgType, Index, -1, text, ply, number2, number3, number4, number5);
        }

		public virtual bool SendRawData(byte[] data)
		{
			if (!RealPlayer || !ConnectionAlive)
				return false;

			return TShock.SendBytes(Netplay.serverSock[Index], data);
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

	public class TSRestPlayer : TSServerPlayer
	{
		internal List<string> CommandReturn = new List<string>();

		public TSRestPlayer()
		{
			Group = new SuperAdminGroup();
            AwaitingResponse = new Dictionary<string, Action<object>>();
		}

		public override void SendMessage(string msg)
		{
			SendMessage(msg, 0, 255, 0);
		}

		public override void SendMessage(string msg, Color color)
		{
			SendMessage(msg, color.R, color.G, color.B);
		}

		public override void SendMessage(string msg, byte red, byte green, byte blue)
		{
			CommandReturn.Add(msg);
		}

		public List<string> GetCommandOutput()
		{
			return CommandReturn;
		}
	}

	public class TSServerPlayer : TSPlayer
	{
		public TSServerPlayer()
			: base("Server")
		{
			Group = new SuperAdminGroup();
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

		public override void SendMessage(string msg)
		{
			SendMessage(msg, 0, 255, 0);
		}

		public override void SendMessage(string msg, Color color)
		{
			SendMessage(msg, color.R, color.G, color.B);
		}

		public override void SendMessage(string msg, byte red, byte green, byte blue)
		{
			Console.WriteLine(msg);
			//RconHandler.Response += msg + "\n";
		}

		public void SetFullMoon(bool fullmoon)
		{
			Main.moonPhase = 0;
			SetTime(false, 0);
		}

		public void SetBloodMoon(bool bloodMoon)
		{
			Main.bloodMoon = bloodMoon;
			SetTime(false, 0);
		}

		public void SetTime(bool dayTime, double time)
		{
			Main.dayTime = dayTime;
			Main.time = time;
			NetMessage.SendData((int) PacketTypes.TimeSet, -1, -1, "", 0, 0, Main.sunModY, Main.moonModY);
			NetMessage.syncPlayers();
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
			Main.npc[npcid].StrikeNPC(damage, knockBack, hitDirection);
			NetMessage.SendData((int) PacketTypes.NpcStrike, -1, -1, "", npcid, damage, knockBack, hitDirection);
		}

		public void RevertTiles(Dictionary<Vector2, TileData> tiles)
		{
			// Update Main.Tile first so that when tile sqaure is sent it is correct
			foreach (KeyValuePair<Vector2, TileData> entry in tiles)
			{
				Main.tile[(int) entry.Key.X, (int) entry.Key.Y].Data = entry.Value;
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
		public NetItem[] inventory = new NetItem[NetItem.maxNetInventory];
		public int maxHealth = 100;
		//public int maxMana = 100;
		public bool exists;

		public PlayerData(TSPlayer player)
		{
			for (int i = 0; i < NetItem.maxNetInventory; i++)
			{
				this.inventory[i] = new NetItem();
			}
			this.inventory[0].netID = -15;
			this.inventory[0].stack = 1;
			if (player.TPlayer.inventory[0] != null && player.TPlayer.inventory[0].netID == -15)
				this.inventory[0].prefix = player.TPlayer.inventory[0].prefix;
			this.inventory[1].netID = -13;
			this.inventory[1].stack = 1;
			if (player.TPlayer.inventory[1] != null && player.TPlayer.inventory[1].netID == -13)
				this.inventory[1].prefix = player.TPlayer.inventory[1].prefix;
			this.inventory[2].netID = -16;
			this.inventory[2].stack = 1;
			if (player.TPlayer.inventory[2] != null && player.TPlayer.inventory[2].netID == -16)
				this.inventory[2].prefix = player.TPlayer.inventory[2].prefix;
		}

		public void StoreSlot(int slot, int netID, int prefix, int stack)
		{
			if(slot > (this.inventory.Length - 1)) //if the slot is out of range then dont save
			{
				return;
			}	
			
			this.inventory[slot].netID = netID;
			if (this.inventory[slot].netID != 0)
			{
				this.inventory[slot].stack = stack;
				this.inventory[slot].prefix = prefix;
			}
			else
			{
				this.inventory[slot].stack = 0;
				this.inventory[slot].prefix = 0;
			}
		}

		public void CopyInventory(TSPlayer player)
		{
			this.maxHealth = player.TPlayer.statLifeMax;
			Item[] inventory = player.TPlayer.inventory;
			Item[] armor = player.TPlayer.armor;
			for (int i = 0; i < NetItem.maxNetInventory; i++)
			{
				if (i < 49)
				{
					if (player.TPlayer.inventory[i] != null)
					{
						this.inventory[i].netID = inventory[i].netID;
					}
					else
					{
						this.inventory[i].netID = 0;
					}

					if (this.inventory[i].netID != 0)
					{
						this.inventory[i].stack = inventory[i].stack;
						this.inventory[i].prefix = inventory[i].prefix;
					}
					else
					{
						this.inventory[i].stack = 0;
						this.inventory[i].prefix = 0;
					}
				}
				else
				{
					if (player.TPlayer.armor[i - 48] != null)
					{
						this.inventory[i].netID = armor[i - 48].netID;
					}
					else
					{
						this.inventory[i].netID = 0;
					}

					if (this.inventory[i].netID != 0)
					{
						this.inventory[i].stack = armor[i - 48].stack;
						this.inventory[i].prefix = armor[i - 48].prefix;
					}
					else
					{
						this.inventory[i].stack = 0;
						this.inventory[i].prefix = 0;
					}
				}
			}
		}
	}

	public class NetItem
	{
		public static int maxNetInventory = 59;
		public int netID;
		public int stack;
		public int prefix;

		public static string ToString(NetItem[] inventory)
		{
			string inventoryString = "";
			for (int i = 0; i < maxNetInventory; i++)
			{
				if (i != 0)
					inventoryString += "~";
				inventoryString += inventory[i].netID;
				if (inventory[i].netID != 0)
				{
					inventoryString += "," + inventory[i].stack;
					inventoryString += "," + inventory[i].prefix;
				}
				else
				{
					inventoryString += ",0,0";
				}
			}
			return inventoryString;
		}

		public static NetItem[] Parse(string data)
		{
			NetItem[] inventory = new NetItem[maxNetInventory];
			int i;
			for (i = 0; i < maxNetInventory; i++)
			{
				inventory[i] = new NetItem();
			}
			string[] items = data.Split('~');
			i = 0;
			foreach (string item in items)
			{
				string[] idata = item.Split(',');
				inventory[i].netID = int.Parse(idata[0]);
				inventory[i].stack = int.Parse(idata[1]);
				inventory[i].prefix = int.Parse(idata[2]);
				i++;
			}
			return inventory;
		}
	}
}
