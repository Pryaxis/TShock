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

using System.IO;
using System.Text;
using Terraria;

using TShockAPI.Net;
using System.IO.Streams;

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
                {PacketTypes.SyncPlayers, HandleSync},
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
                {PacketTypes.ContinueConnecting2, HandleConnecting}
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

        private static bool HandleSync(GetDataHandlerArgs args)
        {
            return TShock.Config.EnableAntiLag;
        }

        private static bool HandlePlayerSlot(GetDataHandlerArgs args)
        {
            int plr = args.Data.ReadInt8();
            int slot = args.Data.ReadInt8();
            int stack = args.Data.ReadInt8();
            short prefix = (short) args.Data.ReadInt8();
            int type = (int) args.Data.ReadInt16();

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

            if (stack > item.maxStack && type != 0 && args.Player.IgnoreActionsForCheating != "none" && !args.Player.Group.HasPermission(Permissions.ignorestackhackdetection))
            {
                args.Player.IgnoreActionsForCheating = "Item Hack: " + item.name + " (" + stack + ") exceeds max stack of " + item.maxStack;
            }

            if (args.Player.IsLoggedIn)
            {
                args.Player.PlayerData.StoreSlot(slot, type, prefix, stack);
            }

            return false;
        }

        private static bool HandlePlayerHp(GetDataHandlerArgs args)
        {
            int plr = args.Data.ReadInt8();
            int cur = args.Data.ReadInt16();
            int max = args.Data.ReadInt16();

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
            int plr = args.Data.ReadInt8();
            int cur = args.Data.ReadInt16();
            int max = args.Data.ReadInt16();

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
            var male = args.Data.ReadInt8();
            args.Data.Position += 21;
            var difficulty = args.Data.ReadInt8();
            string name = Encoding.ASCII.GetString(args.Data.ReadBytes((int)(args.Data.Length - args.Data.Position - 1)));

            if (!TShock.Utils.ValidString(name))
            {
                TShock.Utils.ForceKick(args.Player, "Unprintable character in name");
                return true;
            }
            if (name.Length > 32)
            {
                TShock.Utils.ForceKick(args.Player, "Name exceeded 32 characters.");
                return true;
            }
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
            if (user != null)
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

            string password = Encoding.ASCII.GetString(args.Data.ReadBytes((int)(args.Data.Length - args.Data.Position - 1)));
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
                    NetMessage.SendData((int)PacketTypes.WorldInfo, args.Player.Index);

                    if (TShock.Config.ServerSideInventory)
                    {
                        if (!TShock.CheckInventory(args.Player))
                        {
                            args.Player.SendMessage("Login Failed, Please fix the above errors then /login again.", Color.Cyan);
                            args.Player.IgnoreActionsForClearingTrashCan = true;
                            return true;
                        }
                    }

                    args.Player.Group = TShock.Utils.GetGroup(user.Group);
                    args.Player.UserAccountName = args.Player.Name;
                    args.Player.UserID = TShock.Users.GetUserID(args.Player.UserAccountName);
                    args.Player.IsLoggedIn = true;
                    args.Player.IgnoreActionsForInventory = false;

                    args.Player.PlayerData.CopyInventory(args.Player);
                    TShock.InventoryDB.InsertPlayerData(args.Player);

                    args.Player.SendMessage("Authenticated as " + args.Player + " successfully.", Color.LimeGreen);
                    Log.ConsoleInfo(args.Player.Name + " authenticated successfully as user: " + args.Player);

                    return true;
                }
                else
                {
                    TShock.Utils.ForceKick(args.Player, "Incorrect Account Password, use the password you created with /register");
                    return true;
                }
            }
            else if (!string.IsNullOrEmpty(TShock.Config.ServerPassword))
            {
                if(TShock.Config.ServerPassword == password)
                {
                    args.Player.RequiresPassword = false;
                    if (args.Player.State == 1)
                        args.Player.State = 2;
                    NetMessage.SendData((int)PacketTypes.WorldInfo, args.Player.Index);
                    return true;
                }
                else
                {
                    TShock.Utils.ForceKick(args.Player, "Incorrect Server Password");
                    return true;
                }
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

            if (TShock.Utils.ActivePlayers() + 1 > TShock.Config.MaxSlots && !args.Player.Group.HasPermission(Permissions.reservedslot))
            {
                TShock.Utils.ForceKick(args.Player, TShock.Config.ServerFullReason);
                return true;
            }

            NetMessage.SendData((int)PacketTypes.TimeSet, -1, -1, "", 0, 0, Main.sunModY, Main.moonModY);

            if (TShock.Config.EnableGeoIP && TShock.Geo != null)
            {
                Log.Info(string.Format("{0} ({1}) from '{2}' group from '{3}' joined. ({4}/{5})", args.Player.Name, args.Player.IP, args.Player.Group.Name, args.Player.Country, TShock.Utils.ActivePlayers(), TShock.Config.MaxSlots));
                TShock.Utils.Broadcast(args.Player.Name + " has joined from the " + args.Player.Country, Color.Yellow);
            }
            else
            {
                Log.Info(string.Format("{0} ({1}) from '{2}' group joined. ({3}/{4})", args.Player.Name, args.Player.IP, args.Player.Group.Name, TShock.Utils.ActivePlayers(), TShock.Config.MaxSlots));
                TShock.Utils.Broadcast(args.Player.Name + " has joined", Color.Yellow);
            }

            if (TShock.Config.DisplayIPToAdmins)
                TShock.Utils.SendLogs(string.Format("{0} has joined. IP: {1}", args.Player.Name, args.Player.IP), Color.Blue);

            return false;
        }

        private static bool HandleSendTileSquare(GetDataHandlerArgs args)
        {
        	
            var size = args.Data.ReadInt16();
            var tileX = args.Data.ReadInt32();
            var tileY = args.Data.ReadInt32();

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
                    if(TShock.CheckTilePermission(args.Player, x, y))
                    {
                        continue;
                    }
                    if(TShock.CheckRangePermission(args.Player, x, y))
                    {
                        continue;
                    }
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

            if (tileX < 0 || tileX >= Main.maxTilesX || tileY < 0 || tileY >= Main.maxTilesY)
                return false;

            if (args.Player.AwaitingName)
            {
                if (TShock.Regions.InAreaRegionName(tileX, tileY) == null)
                {
                    args.Player.SendMessage("Region is not protected", Color.Yellow);
                }
                else
                {
                    args.Player.SendMessage("Region Name: " + TShock.Regions.InAreaRegionName(tileX, tileY), Color.Yellow);
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
                if (tiletype == 48 && !args.Player.Group.HasPermission(Permissions.usebanneditem) && TShock.Itembans.ItemIsBanned("Spikes", args.Player))
                {
                    args.Player.SendTileSquare(tileX, tileY);
                    return true;
                }
                if (type == 1 && tiletype == 21 && TShock.Utils.MaxChests())
                {
                    args.Player.SendMessage("Reached world's max chest limit, unable to place more!", Color.Red);
                    args.Player.SendTileSquare(tileX, tileY);
                    return true;
                }
				if (tiletype == 141 && !args.Player.Group.HasPermission(Permissions.usebanneditem) && TShock.Itembans.ItemIsBanned("Explosives", args.Player))
                {
                    args.Player.SendTileSquare(tileX, tileY);
                    return true;
                }
            }

            if (TShock.CheckIgnores(args.Player))
            {
                args.Player.SendTileSquare(tileX, tileY);
                return true;
            }

            if (TShock.CheckTilePermission(args.Player, tileX, tileY))
            {
                args.Player.SendTileSquare(tileX, tileY);
                return true;
            }

            if ((tiletype == 127 || Main.tileCut[tiletype]) && type == 0) //Ice Block Kill, Bypass range checks and such
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
                args.Player.LastThreat = DateTime.UtcNow;
                args.Player.SendTileSquare(tileX, tileY);
                return true;
            }

            if (args.Player.TilePlaceThreshold >= TShock.Config.TilePlaceThreshold)
            {
                args.Player.LastThreat = DateTime.UtcNow;
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

            if ((type == 0 || type == 4) && Main.tileSolid[Main.tile[tileX, tileY].type] && !args.Player.Group.HasPermission(Permissions.ignorekilltiledetection))
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
            int id = args.Data.ReadByte();
            bool pvp = args.Data.ReadBoolean();

            if (id != args.Player.Index)
            {
                return true;
            }

            if (args.TPlayer.hostile != pvp)
            {
                long seconds = (long)(DateTime.UtcNow - args.Player.LastPvpChange).TotalSeconds;
                if (seconds > 5)
                {
                    TSPlayer.All.SendMessage(string.Format("{0} has {1} PvP!", args.Player.Name, pvp ? "enabled" : "disabled"), Main.teamColor[args.Player.Team]);
                }
                args.Player.LastPvpChange = DateTime.UtcNow;
            }

            args.TPlayer.hostile = pvp;

            if (TShock.Config.PvPMode == "always")
            {
                if (pvp == true)
                    args.Player.IgnoreActionsForPvP = false;
                else
                {
                    args.Player.Spawn();
                    args.Player.IgnoreActionsForPvP = true;
                }
            }

            NetMessage.SendData((int)PacketTypes.TogglePvp, -1, -1, "", args.Player.Index);

            return true;
        }

        private static bool HandlePlayerUpdate(GetDataHandlerArgs args)
        {
            var plr = args.Data.ReadInt8();
            var control = args.Data.ReadInt8();
            var item = args.Data.ReadInt8();
            var pos = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());
            var vel = new Vector2(args.Data.ReadSingle(), args.Data.ReadSingle());

            if (item < 0 || item >= args.TPlayer.inventory.Length)
            {
                return true;
            }

            if (!pos.Equals(args.Player.LastNetPosition))
            {
                float distance = Vector2.Distance(new Vector2((pos.X / 16f), (pos.Y / 16f)), new Vector2(Main.spawnTileX, Main.spawnTileY));
                if (TShock.CheckIgnores(args.Player) && distance > TShock.Config.MaxRangeForDisabled)
                {
                    if(args.Player.IgnoreActionsForCheating != "none")
                    {
                        args.Player.SendMessage("Disabled for cheating: " + args.Player.IgnoreActionsForCheating, Color.Red);
                    }
                    else if (TShock.Config.RequireLogin && !args.Player.IsLoggedIn)
                    {
                        args.Player.SendMessage("Please /register or /login to play!", Color.Red);
                    }
                    else if (args.Player.IgnoreActionsForInventory)
                    {
                        args.Player.SendMessage("Server Side Inventory is enabled! Please /register or /login to play!", Color.Red);
                    }
                    else if (args.Player.IgnoreActionsForClearingTrashCan)
                    {
                        args.Player.SendMessage("You need to rejoin to ensure your trash can is cleared!", Color.Red);
                    }
                    else if (args.Player.IgnoreActionsForPvP)
                    {
                        args.Player.SendMessage("PvP is forced! Enable PvP else you can't move or do anything!", Color.Red);
                    }
                    args.Player.Spawn();
                    return true;
                }

                if(!args.Player.Group.HasPermission(Permissions.ignorenoclipdetection) && TShock.CheckPlayerCollision((int)(pos.X / 16f), (int)(pos.Y / 16f)))
                {
                    int lastTileX = (int)(args.Player.LastNetPosition.X / 16f);
                    int lastTileY = (int)(args.Player.LastNetPosition.Y / 16f);
                    if(args.Player.Teleport(lastTileX, lastTileY))
                    {
                        args.Player.SendMessage("You got stuck in a solid object, Sent to last good position.");
                    }
                    else
                    {
                        args.Player.SendMessage("You got stuck in a solid object, Sent to spawn point.");
                        args.Player.Spawn();
                    }
                    return true;
                }
            }
            args.Player.LastNetPosition = pos;

            if ((control & 32) == 32)
            {
				if (!args.Player.Group.HasPermission(Permissions.usebanneditem) && TShock.Itembans.ItemIsBanned(args.TPlayer.inventory[item].name, args.Player))
                {
                    control -= 32;
                    args.Player.LastThreat = DateTime.UtcNow;
                    args.Player.SendMessage(string.Format("You cannot use {0} on this server. Your actions are being ignored.", args.TPlayer.inventory[item].name), Color.Red);
                }
            }
            
            args.TPlayer.selectedItem = item;
            args.TPlayer.position = pos;
            args.TPlayer.velocity = vel;
            args.TPlayer.oldVelocity = args.TPlayer.velocity;
            args.TPlayer.fallStart = (int)(pos.Y / 16f);
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
            NetMessage.SendData((int)PacketTypes.PlayerUpdate, -1, args.Player.Index, "", args.Player.Index);

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

            var index = TShock.Utils.SearchProjectile(ident);

            if (index > Main.maxProjectiles || index < 0)
            {
                return true;
            }

            if (args.Player.Index != owner)
            {
                args.Player.LastThreat = DateTime.UtcNow;
                args.Player.SendData(PacketTypes.ProjectileNew, "", index);
                return true;
            }

            if (dmg > 175)
            {
                args.Player.LastThreat = DateTime.UtcNow;
                args.Player.SendData(PacketTypes.ProjectileNew, "", index);
                return true;
            }

            if (TShock.CheckIgnores(args.Player))
            {
                args.Player.SendData(PacketTypes.ProjectileNew, "", index);
                return true;
            }

            if (TShock.CheckProjectilePermission(args.Player, index, type))
            {
                args.Player.LastThreat = DateTime.UtcNow;
                args.Player.SendData(PacketTypes.ProjectileNew, "", index);
                return true;
            }

            if (args.Player.ProjectileThreshold >= TShock.Config.ProjectileThreshold)
            {
                args.Player.LastThreat = DateTime.UtcNow;
                args.Player.SendData(PacketTypes.ProjectileNew, "", index);
                return true;
            }

            if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
            {
                args.Player.SendData(PacketTypes.ProjectileNew, "", index);
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

            if (args.Player.Index != owner)
            {
                args.Player.LastThreat = DateTime.UtcNow;
                return true;
            }

            var index = TShock.Utils.SearchProjectile(ident);

            if (index > Main.maxProjectiles || index < 0)
            {
                return true;
            }

            int type = Main.projectile[index].type;

            if (args.Player.Index != Main.projectile[index].owner)
            {
                args.Player.LastThreat = DateTime.UtcNow;
                args.Player.SendData(PacketTypes.ProjectileNew, "", index);
                return true;
            }

            if (TShock.CheckIgnores(args.Player))
            {
                args.Player.SendData(PacketTypes.ProjectileNew, "", index);
                return true;
            }

            if (TShock.CheckProjectilePermission(args.Player, index, type))
            {
                args.Player.LastThreat = DateTime.UtcNow;
                args.Player.SendData(PacketTypes.ProjectileNew, "", index);
                return true;
            }

            if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
            {
                args.Player.SendData(PacketTypes.ProjectileNew, "", index);
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

            int textlength = (int)(args.Data.Length - args.Data.Position - 1);
            string deathtext = "";
            if (textlength > 0)
            {
                deathtext = Encoding.ASCII.GetString(args.Data.ReadBytes(textlength));
                if (!TShock.Utils.ValidString(deathtext))
                {
                    return true;
                }
            }

            args.Player.LastDeath = DateTime.Now;

            if (args.Player.Difficulty != 2)
                args.Player.ForceSpawn = true;

            return false;
        }

        private static bool HandleLiquidSet(GetDataHandlerArgs args)
        {
            int tileX = args.Data.ReadInt32();
            int tileY = args.Data.ReadInt32();
            byte liquid = args.Data.ReadInt8();
            bool lava = args.Data.ReadBoolean();

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
                args.Player.LastThreat = DateTime.UtcNow;
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

			if (lava && bucket != 2 && !args.Player.Group.HasPermission(Permissions.usebanneditem) && TShock.Itembans.ItemIsBanned("Lava Bucket", args.Player))
            {
                args.Player.LastThreat = DateTime.UtcNow;
                args.Player.SendTileSquare(tileX, tileY);
                return true;
            }

			if (!lava && bucket != 1 && !args.Player.Group.HasPermission(Permissions.usebanneditem) && TShock.Itembans.ItemIsBanned("Water Bucket", args.Player))
            {
                args.Player.LastThreat = DateTime.UtcNow;
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

            if (tileX < 0 || tileX >= Main.maxTilesX || tileY < 0 || tileY >= Main.maxTilesY)
                return false;

            if (TShock.CheckIgnores(args.Player))
            {
                args.Player.SendTileSquare(tileX, tileY);
                return true;
            }

            if (Main.tile[tileX, tileY].type != 0x15 && (!TShock.Utils.MaxChests() && Main.tile[tileX, tileY].type != 0)) //Chest
            {
                args.Player.LastThreat = DateTime.UtcNow;
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

            args.Player.ForceSpawn = false;
            return false;
        }

        private static bool HandleChestOpen(GetDataHandlerArgs args)
        {
            var x = args.Data.ReadInt32();
            var y = args.Data.ReadInt32();

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
                return false;
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

            if (!args.Player.Group.HasPermission(Permissions.movenpc))
            {
                args.Player.SendMessage("You do not have permission to relocate NPCs.", Color.Red);
                args.Player.SendData(PacketTypes.UpdateNPCHome, "", id, Main.npc[id].homeTileX, Main.npc[id].homeTileY, Convert.ToByte(Main.npc[id].homeless));
                return true;
            }

            if (TShock.CheckTilePermission(args.Player, x, y))
            {
                args.Player.SendData(PacketTypes.UpdateNPCHome, "", id, Main.npc[id].homeTileX, Main.npc[id].homeTileY, Convert.ToByte(Main.npc[id].homeless));
                return true;
            }

            if (TShock.CheckRangePermission(args.Player, x, y))
            {
                args.Player.SendData(PacketTypes.UpdateNPCHome, "", id, Main.npc[id].homeTileX, Main.npc[id].homeTileY, Convert.ToByte(Main.npc[id].homeless));
                return true;
            }
            return false;
        }

        private static bool HandlePlayerBuff(GetDataHandlerArgs args)
        {
            var id = args.Data.ReadInt8();
            var type = args.Data.ReadInt8();
            var time = args.Data.ReadInt16();

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

            if (type == 0) //Item removed, let client do this to prevent item duplication client side
            {
                return false;
            }

            if (TShock.CheckRangePermission(args.Player, (int)(pos.X / 16f), (int)(pos.Y / 16f)))
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

            int textlength = (int)(args.Data.Length - args.Data.Position - 1);
            string deathtext = "";
            if (textlength > 0)
            {
                deathtext = Encoding.ASCII.GetString(args.Data.ReadBytes(textlength));
                if (!TShock.Utils.ValidString(deathtext))
                {
                    return true;
                }
            }

            if (TShock.Players[id] == null)
                return true;

            if (dmg > 175)
            {
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

            if (Main.npc[id] == null)
                return true;

            if (dmg > 175)
            {
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
                args.Player.SendData(PacketTypes.NpcUpdate, "", id);
                return true;
            }

            if (TShock.Config.RangeChecks && TShock.CheckRangePermission(args.Player, (int)(Main.npc[id].position.X / 16f), (int)(Main.npc[id].position.Y / 16f), 100))
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
            for (int i = 0; i < 10; i++)
            {
                var buff = args.Data.ReadInt8();

                if (buff == 10)
                {
                    if (!args.Player.Group.HasPermission(Permissions.usebanneditem) && TShock.Itembans.ItemIsBanned("Invisibility Potion", args.Player) )
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
            NetMessage.SendData((int)PacketTypes.PlayerBuff, -1, args.Player.Index, "", args.Player.Index);
            return true;
        }
    }
}
