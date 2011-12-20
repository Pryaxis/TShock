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
using System.Diagnostics;

using System.IO;
using System.Text;
using Terraria;

using TShockAPI.Net;
using System.IO.Streams;
using System.Net;

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
        public static bool[] WhitelistBuffs;

        public static void InitGetDataHandler()
        {
            #region Blacklists

            //Check StatusPvp for whats usable
            WhitelistBuffs = new bool[Main.maxBuffs];
            WhitelistBuffs[20] = true; //Poisoned
            WhitelistBuffs[0x18] = true; //On Fire
            WhitelistBuffs[0x1f] = true; //Confused
            WhitelistBuffs[0x27] = true; //Cursed Inferno

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
                {PacketTypes.UpdateNPCHome, UpdateNPCHome },
                {PacketTypes.PlayerAddBuff, HandlePlayerBuff},
                {PacketTypes.ItemDrop, HandleItemDrop},
                {PacketTypes.PlayerHp, HandlePlayerHp},
                {PacketTypes.PlayerMana, HandlePlayerMana},
                {PacketTypes.PlayerDamage, HandlePlayerDamage},
                {PacketTypes.NpcStrike, HandleNpcStrike},
                {PacketTypes.NpcSpecial, HandleSpecial},
                {PacketTypes.PlayerAnimation, HandlePlayerAnimation},
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

            if (type == 0)
                return true;

            var item = new Item();
            item.netDefaults(type);
            item.Prefix(prefix);

            if (stack > item.maxStack)
            {
                return TShock.Utils.HandleCheater(args.Player, String.Format("Stack cheat detected. Remove {0} ({1}) > {2} and then rejoin", item.name, stack, item.maxStack));
            }

            return false;
        }

        private static bool HandlePlayerHp(GetDataHandlerArgs args)
        {
            int plr = args.Data.ReadInt8();
            int cur = args.Data.ReadInt16();
            int max = args.Data.ReadInt16();

            if (cur > 600 || max > 600)
            {
                return TShock.Utils.HandleCheater(args.Player, "Health cheat detected. Please use a different character.");
            }

            return false;
        }

        private static bool HandlePlayerMana(GetDataHandlerArgs args)
        {
            int plr = args.Data.ReadInt8();
            int cur = args.Data.ReadInt16();
            int max = args.Data.ReadInt16();

            if (cur > 600 || max > 600)
            {
                return TShock.Utils.HandleCheater(args.Player, "Mana cheat detected. Please use a different character.");
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

            NetMessage.SendData((int)PacketTypes.TimeSet, -1, -1, "", 0, 0, Main.sunModY, Main.moonModY);

            if (TShock.Config.EnableGeoIP && TShock.Geo != null)
            {
                var code = TShock.Geo.TryGetCountryCode(IPAddress.Parse(args.Player.IP));
                args.Player.Country = code == null ? "N/A" : MaxMind.GeoIPCountry.GetCountryNameByCode(code);
                if (code == "A1")
                    if (TShock.Config.KickProxyUsers)
                        TShock.Utils.Kick(args.Player, "Proxies are not allowed");
                Log.Info(string.Format("{0} ({1}) from '{2}' group from '{3}' joined.", args.Player.Name, args.Player.IP, args.Player.Group.Name, args.Player.Country));
                TShock.Utils.Broadcast(args.Player.Name + " has joined from the " + args.Player.Country, Color.Yellow);
            }
            else
            {
                Log.Info(string.Format("{0} ({1}) from '{2}' group joined.", args.Player.Name, args.Player.IP, args.Player.Group.Name));
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
                if (tiletype == 48 && !args.Player.Group.HasPermission(Permissions.usebanneditem) && TShock.Itembans.ItemIsBanned("Spikes"))
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
                if (tiletype == 141 && !args.Player.Group.HasPermission(Permissions.usebanneditem) && TShock.Itembans.ItemIsBanned("Explosives"))
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

            if (type == 0 && Main.tileSolid[Main.tile[tileX, tileY].type] && args.Player.Active)
            {
                args.Player.TileThreshold++;
                var coords = new Vector2(tileX, tileY);
                if (!args.Player.TilesDestroyed.ContainsKey(coords))
                    args.Player.TilesDestroyed.Add(coords, Main.tile[tileX, tileY]);
            }

            if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
            {
                args.Player.SendTileSquare(tileX, tileY);
                return true;
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
                if (TShock.Config.PvpThrottle > 0 && seconds < TShock.Config.PvpThrottle)
                {
                    TSPlayer.All.SendMessage(string.Format("{0} has {1} PvP!", args.Player.Name, pvp ? "enabled" : "disabled"), Main.teamColor[args.Player.Team]);
                }
                args.Player.LastPvpChange = DateTime.UtcNow;
            }

            args.TPlayer.hostile = pvp;

            if (pvp == true && TShock.Config.AlwaysPvP)
                args.Player.IgnoreActionsForPvP = false;
            else
                args.Player.IgnoreActionsForPvP = true;

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

            if ((control & 32) == 32)
            {
                if (!args.Player.Group.HasPermission(Permissions.usebanneditem) && TShock.Itembans.ItemIsBanned(args.TPlayer.inventory[item].name))
                {
                    args.Player.LastThreat = DateTime.UtcNow;
                    args.Player.SendMessage(string.Format("You cannot use {0} on this server. Your actions are being ignored.", args.TPlayer.inventory[item].name), Color.Red);
                    return true;
                }
            }

            if (!pos.Equals(args.Player.LastNetPosition))
            {
                float distance = Vector2.Distance(new Vector2((pos.X / 16f), (pos.Y / 16f)), new Vector2(Main.spawnTileX, Main.spawnTileY));
                if (TShock.CheckIgnores(args.Player) && distance > 6f)
                {
                    if (TShock.Config.AlwaysPvP)
                    {
                        args.Player.SendMessage("PvP is forced! Enable PvP else you can't move or do anything!", Color.Red);
                    }
                    args.Player.Spawn();
                    return true;
                }
                if (TShock.CheckPlayerCollision((int)(pos.X / 16f), (int)(pos.Y / 16f))) //NoClipping or possible errors
                {
                    args.Player.SendMessage("You got stuck in a solid object! Sent you to the spawn point.", Color.Red);
                    args.Player.SendTileSquare((int)(pos.X / 16f), (int)(pos.X / 16f));
                    args.Player.Spawn();
                    return true;
                }
            }

            args.Player.LastNetPosition = pos;
            return false;
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
                args.Player.SendData(PacketTypes.ProjectileNew, "", index);
                return true;
            }

            if (dmg > 175)
            {
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

            return false;
        }

        private static bool HandleProjectileKill(GetDataHandlerArgs args)
        {
            var ident = args.Data.ReadInt16();
            var owner = args.Data.ReadInt8();

            if (args.Player.Index != owner)
            {
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

            if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
            {
                args.Player.SendTileSquare(tileX, tileY);
                return true;
            }

            bool bucket = false;
            for (int i = 0; i < 49; i++)
            {
                if (args.TPlayer.inventory[i].type >= 205 && args.TPlayer.inventory[i].type <= 207)
                {
                    bucket = true;
                    break;
                }
            }
            if (!bucket)
            {
                args.Player.SendTileSquare(tileX, tileY);
                return true;
            }

            if (lava && !args.Player.Group.HasPermission(Permissions.usebanneditem) && TShock.Itembans.ItemIsBanned("Lava Bucket"))
            {
                args.Player.SendTileSquare(tileX, tileY);
                return true;
            }

            if (!lava && !args.Player.Group.HasPermission(Permissions.usebanneditem) && TShock.Itembans.ItemIsBanned("Water Bucket"))
            {
                args.Player.SendTileSquare(tileX, tileY);
                return true;
            }
            if (TShock.CheckTilePermission(args.Player, tileX, tileY))
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
                args.Player.SendTileSquare(tileX, tileY);
                return true;
            }

            if (TShock.CheckTilePermission(args.Player, tileX, tileY))
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

            if (TShock.Config.RangeChecks && ((Math.Abs(args.Player.TileX - x) > 32) || (Math.Abs(args.Player.TileY - y) > 32)))
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
            if (stacks > item.maxStack || TShock.Itembans.ItemIsBanned(item.name))
            {
                args.Player.SendData(PacketTypes.ChestItem, "", id, slot);
                return false;
            }

            if (TShock.CheckTilePermission(args.Player, Main.chest[id].x, Main.chest[id].y))
            {
                args.Player.SendData(PacketTypes.ChestItem, "", id, slot);
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
            return false;
        }

        private static bool HandleGetSection(GetDataHandlerArgs args)
        {
            if (args.Player.RequestedSection)
                return true;

            args.Player.RequestedSection = true;
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
            if (TShock.Config.RangeChecks && ((Math.Abs(args.Player.TileX - TShock.Players[id].TileX) > 64) || (Math.Abs(args.Player.TileY - TShock.Players[id].TileY) > 64)))
            {
                args.Player.SendData(PacketTypes.PlayerBuff, "", id);
                return true;
            }
            if ((DateTime.UtcNow - args.Player.LastThreat).TotalMilliseconds < 5000)
            {
                args.Player.SendData(PacketTypes.PlayerBuff, "", id);
                return true;
            }
            if (WhitelistBuffs[type])
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

            if (TShock.Config.RangeChecks && ((Math.Abs(args.Player.TileX - (pos.X / 16f)) > 64) || (Math.Abs(args.Player.TileY - (pos.Y / 16f)) > 64)))
            {
                args.Player.SendData(PacketTypes.ItemDrop, "", id);
                return true;
            }

            Item item = new Item();
            item.netDefaults(type);
            if (stacks > item.maxStack || TShock.Itembans.ItemIsBanned(item.name))
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

            if (TShock.Config.RangeChecks && ((Math.Abs(args.Player.TileX - TShock.Players[id].TileX) > 128) || (Math.Abs(args.Player.TileY - TShock.Players[id].TileY) > 128)))
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

            if (TShock.Config.RangeChecks && ((Math.Abs(args.Player.TileX - (Main.npc[id].position.X / 16f)) > 128) || (Math.Abs(args.Player.TileY - (Main.npc[id].position.Y / 16f)) > 128)))
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
    }
}
