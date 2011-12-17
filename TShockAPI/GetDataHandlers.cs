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
        public static bool[] BlacklistTiles;

        public static void InitGetDataHandler()
        {
            // Need to update to 1.1 tiles
            #region Blacklisted tiles

            BlacklistTiles = new bool[Main.maxTileSets];
            BlacklistTiles[0] = true;
            BlacklistTiles[1] = true;
            BlacklistTiles[2] = true;
            BlacklistTiles[6] = true;
            BlacklistTiles[7] = true;
            BlacklistTiles[8] = true;
            BlacklistTiles[9] = true;
            BlacklistTiles[22] = true;
            BlacklistTiles[23] = true;
            BlacklistTiles[25] = true;
            BlacklistTiles[30] = true;
            BlacklistTiles[37] = true;
            BlacklistTiles[38] = true;
            BlacklistTiles[39] = true;
            BlacklistTiles[40] = true;
            BlacklistTiles[41] = true;
            BlacklistTiles[43] = true;
            BlacklistTiles[44] = true;
            BlacklistTiles[45] = true;
            BlacklistTiles[46] = true;
            BlacklistTiles[47] = true;
            BlacklistTiles[53] = true;
            BlacklistTiles[54] = true;
            BlacklistTiles[56] = true;
            BlacklistTiles[57] = true;
            BlacklistTiles[58] = true;
            BlacklistTiles[59] = true;
            BlacklistTiles[60] = true;
            BlacklistTiles[63] = true;
            BlacklistTiles[64] = true;
            BlacklistTiles[65] = true;
            BlacklistTiles[66] = true;
            BlacklistTiles[67] = true;
            BlacklistTiles[68] = true;
            BlacklistTiles[70] = true;
            BlacklistTiles[75] = true;
            BlacklistTiles[76] = true;

            #endregion Blacklisted tiles

            GetDataHandlerDelegates = new Dictionary<PacketTypes, GetDataHandlerDelegate>
            {
                {PacketTypes.PlayerInfo, HandlePlayerInfo},
                {PacketTypes.TileSendSection, HandleSendSection},
                {PacketTypes.PlayerUpdate, HandlePlayerUpdate},
                {PacketTypes.Tile, HandleTile},
                {PacketTypes.TileSendSquare, HandleSendTileSquare},
                {PacketTypes.NpcUpdate, HandleNpcUpdate},
                {PacketTypes.PlayerDamage, HandlePlayerDamage},
                {PacketTypes.ProjectileNew, HandleProjectileNew},
                {PacketTypes.TogglePvp, HandleTogglePvp},
                {PacketTypes.TileKill, HandleTileKill},
                {PacketTypes.PlayerKillMe, HandlePlayerKillMe},
                {PacketTypes.LiquidSet, HandleLiquidSet},
                {PacketTypes.PlayerSpawn, HandleSpawn},
                {PacketTypes.SyncPlayers, HandleSync},
                {PacketTypes.ChestGetContents, HandleChest},
                {PacketTypes.SignNew, HandleSign},
                {PacketTypes.PlayerSlot, HandlePlayerSlot},
                {PacketTypes.TileGetSection, HandleGetSection},
                {PacketTypes.UpdateNPCHome, UpdateNPCHome },
                {PacketTypes.PlayerAddBuff, HandlePlayerBuff},
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

            var it = new Item();
            it.netDefaults(type);
            var itemname = it.name;

            if (!args.Player.Group.HasPermission(Permissions.usebanneditem) && TShock.Itembans.ItemIsBanned(itemname))
                args.Player.Disconnect("Using banned item: " + itemname + ", remove it and rejoin");;
            if (stack>it.maxStack)
            {
                string reason = string.Format("Item Stack Hack Detected: player has {0} {1}(s) in one stack", stack,itemname);
				if (TShock.Config.EnableItemStackChecks)
				{
					TShock.Utils.HandleCheater(args.Player, reason);
				}
            }

            return false;
        }

        private static bool HandlePlayerInfo(GetDataHandlerArgs args)
        {
            byte playerid = args.Data.ReadInt8();
            byte hair = args.Data.ReadInt8();
            byte male = args.Data.ReadInt8();
            args.Data.Position += 21;
            byte difficulty = args.Data.ReadInt8();
            string name = Encoding.ASCII.GetString(args.Data.ReadBytes((int)(args.Data.Length - args.Data.Position - 1)));

            if (hair >= Main.maxHair)
            {
                TShock.Utils.ForceKick(args.Player, "Hair crash exploit.");
                return true;
            }
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
                return TShock.Utils.HandleGriefer(args.Player, "Sent client info more than once");
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
            args.Player.ReceivedInfo = true;
            return false;
        }

        private static bool HandleSendTileSquare(GetDataHandlerArgs args)
        {
        	
            short size = args.Data.ReadInt16();
            int tilex = args.Data.ReadInt32();
            int tiley = args.Data.ReadInt32();

            if (size > 5)
                return true;

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
                int realx = tilex + x;
                if (realx < 0 || realx >= Main.maxTilesX)
                    continue;

                for (int y = 0; y < size; y++)
                {
                	int realy = tiley + y;
                	if (realy < 0 || realy >= Main.maxTilesY)
                		continue;

                	var tile = Main.tile[realx, realy];
                	var newtile = tiles[x, y];
                    if (!args.Player.Group.HasPermission(Permissions.editspawn) && !TShock.Regions.CanBuild(x, y, args.Player) && TShock.Regions.InArea(x, y))
                    {
                        if ((DateTime.UtcNow - args.Player.LastTileChangeNotify).TotalMilliseconds > 1000)
                        {
                            args.Player.SendMessage("Region Name: " + TShock.Regions.InAreaRegionName(x, y) + " protected from changes.", Color.Red);
                            args.Player.LastTileChangeNotify = DateTime.UtcNow;
                        }
                        args.Player.SendTileSquare(x, y);
                        continue;
                    }
                    if (TShock.Config.DisableBuild)
                    {
                        if (!args.Player.Group.HasPermission(Permissions.editspawn))
                        {
                            if ((DateTime.UtcNow - args.Player.LastTileChangeNotify).TotalMilliseconds > 1000)
                            {
                                args.Player.SendMessage("World protected from changes.", Color.Red);
                                args.Player.LastTileChangeNotify = DateTime.UtcNow;
                            }
                            args.Player.SendTileSquare(x, y);
                            continue;
                        }
                    }
                    if (TShock.Config.SpawnProtection)
                    {
                        if (!args.Player.Group.HasPermission(Permissions.editspawn))
                        {
                            var flag = TShock.CheckSpawn(x, y);
                            if (flag)
                            {
                                if ((DateTime.UtcNow - args.Player.LastTileChangeNotify).TotalMilliseconds > 1000)
                                {
                                    args.Player.SendMessage("Spawn protected from changes.", Color.Red);
                                    args.Player.LastTileChangeNotify = DateTime.UtcNow;
                                }
                                args.Player.SendTileSquare(x, y);
                                continue;
                            }
                        }
                    }
                	if ((tile.type == 128 && newtile.Type == 128) || (tile.type == 105 && newtile.Type == 105))
                	{
						//Console.WriteLine("SendTileSquareCalled on a 128 or 105.");
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
                }
            }

			if (changed)
			{
				TSPlayer.All.SendTileSquare(tilex, tiley, 3);
				WorldGen.RangeFrame(tilex, tiley, tilex + size, tiley + size);
			}
        	return true;
        }

        private static bool HandleTile(GetDataHandlerArgs args)
        {
            byte type = args.Data.ReadInt8();
            int x = args.Data.ReadInt32();
            int y = args.Data.ReadInt32();
            byte tiletype = args.Data.ReadInt8();
            if (args.Player.AwaitingName)
            {
                if (TShock.Regions.InAreaRegionName(x, y) == null)
                {
                    args.Player.SendMessage("Region is not protected", Color.Yellow);
                }
                else
                {
                    args.Player.SendMessage("Region Name: " + TShock.Regions.InAreaRegionName(x, y), Color.Yellow);
                }
                args.Player.SendTileSquare(x, y);
                args.Player.AwaitingName = false;
                return true;
            }

            if (args.Player.AwaitingTempPoint > 0)
            {
                args.Player.TempPoints[args.Player.AwaitingTempPoint - 1].X = x;
                args.Player.TempPoints[args.Player.AwaitingTempPoint - 1].Y = y;
                args.Player.SendMessage("Set Temp Point " + args.Player.AwaitingTempPoint, Color.Yellow);
                args.Player.SendTileSquare(x, y);
                args.Player.AwaitingTempPoint = 0;
                return true;
            }

            if (!args.Player.Group.HasPermission(Permissions.canbuild))
            {
                if (!args.Player.HasBeenSpammedWithBuildMessage)
                {
                    args.Player.SendMessage("You do not have permission to build!", Color.Red);
                    args.Player.HasBeenSpammedWithBuildMessage = true;
                }
                args.Player.SendTileSquare(x, y);
                return true;
            }
			
            if (type == 1 || type == 3)
            {
                int plyX = Math.Abs(args.Player.TileX);
                int plyY = Math.Abs(args.Player.TileY);
                int tileX = Math.Abs(x);
                int tileY = Math.Abs(y);

                if (tiletype >= ((type == 1) ? Main.maxTileSets : Main.maxWallTypes))
                {
                    TShock.Utils.HandleGriefer(args.Player, string.Format(TShock.Config.TileAbuseReason, "Invalid tile type"));
                    return true;
                }
                if (TShock.Config.RangeChecks && ((Math.Abs(plyX - tileX) > 32) || (Math.Abs(plyY - tileY) > 32)))
                {
					if ((type == 1 && ((tiletype == 0 && args.Player.TPlayer.selectedItem == 114) || (tiletype == 127 && args.Player.TPlayer.selectedItem == 496)|| (tiletype == 53 && args.Player.TPlayer.selectedItem == 266))))
					{
						if (!TShock.Config.EnableRangeCheckOverrides)
						{
							args.Player.SendMessage("This item has been disabled by the server owner.");
							return true;
						}
					} else
                    {
                        Log.Debug(string.Format("TilePlaced(PlyXY:{0}_{1}, TileXY:{2}_{3}, Result:{4}_{5}, Type:{6})",
                                                plyX, plyY, tileX, tileY, Math.Abs(plyX - tileX), Math.Abs(plyY - tileY), tiletype));
                        return TShock.Utils.HandleGriefer(args.Player, TShock.Config.RangeCheckBanReason);
                    }
                }
                if (tiletype == 48 && !args.Player.Group.HasPermission(Permissions.canspike))
                {
                    args.Player.SendMessage("You do not have permission to place spikes.", Color.Red);
                    TShock.Utils.SendLogs(string.Format("{0} tried to place spikes", args.Player.Name), Color.Red);
                    args.Player.SendTileSquare(x, y);
                    return true;
                }
                if (type == 1 && tiletype == 21 && TShock.Utils.MaxChests())
                {
                    args.Player.SendMessage("Reached world's max chest limit, unable to place more!", Color.Red);
                    Log.Info("Reached world's chest limit, unable to place more.");
                    args.Player.SendTileSquare(x, y);
                    return true;
                }
                if (tiletype == 141 && !args.Player.Group.HasPermission(Permissions.canexplosive))
                {
                    args.Player.SendMessage("You do not have permission to place explosives.", Color.Red);
                    TShock.Utils.SendLogs(string.Format("{0} tried to place explosives", args.Player.Name), Color.Red);
                    args.Player.SendTileSquare(x, y);
                    return true;
                }
            }
            if (!args.Player.Group.HasPermission(Permissions.editspawn) && !TShock.Regions.CanBuild(x, y, args.Player) && TShock.Regions.InArea(x, y))
            {
                if ((DateTime.UtcNow - args.Player.LastTileChangeNotify).TotalMilliseconds > 1000)
                {
                    args.Player.SendMessage("Region Name: " + TShock.Regions.InAreaRegionName(x, y) + " protected from changes.", Color.Red);
                    args.Player.LastTileChangeNotify = DateTime.UtcNow;
                }
                args.Player.SendTileSquare(x, y);
                return true;
            }
            if (TShock.Config.DisableBuild)
            {
                if (!args.Player.Group.HasPermission(Permissions.editspawn))
                {
                    if ((DateTime.UtcNow - args.Player.LastTileChangeNotify).TotalMilliseconds > 1000)
                    {
                        args.Player.SendMessage("World protected from changes.", Color.Red);
                        args.Player.LastTileChangeNotify = DateTime.UtcNow;
                    }
                    args.Player.SendTileSquare(x, y);
                    return true;
                }
            }
            if (TShock.Config.SpawnProtection)
            {
                if (!args.Player.Group.HasPermission(Permissions.editspawn))
                {
                    var flag = TShock.CheckSpawn(x, y);
                    if (flag)
                    {
                        if ((DateTime.UtcNow - args.Player.LastTileChangeNotify).TotalMilliseconds > 1000)
                        {
                            args.Player.SendMessage("Spawn protected from changes.", Color.Red);
                            args.Player.LastTileChangeNotify = DateTime.UtcNow;
                        }
                        args.Player.SendTileSquare(x, y);
                        return true;
                    }
                }
            }
            if (type == 0 && BlacklistTiles[Main.tile[x, y].type] && args.Player.Active)
            {
                args.Player.TileThreshold++;
                var coords = new Vector2(x, y);
                if (!args.Player.TilesDestroyed.ContainsKey(coords))
                    args.Player.TilesDestroyed.Add(coords, Main.tile[x, y]);
            }

            if ((DateTime.UtcNow - args.Player.LastExplosive).TotalMilliseconds < 1000)
            {
                args.Player.SendMessage("Please wait another " + (1000 - (DateTime.UtcNow - args.Player.LastExplosive).TotalMilliseconds) + " milliseconds before placing/destroying tiles", Color.Red);
                args.Player.SendTileSquare(x, y);
                return true;
            }
            return false;
        }

        private static bool HandleTogglePvp(GetDataHandlerArgs args)
        {
            int id = args.Data.ReadByte();
            bool pvp = args.Data.ReadBoolean();

            long seconds = (long)(DateTime.UtcNow - args.Player.LastPvpChange).TotalSeconds;
            if (TShock.Config.PvpThrottle > 0 && seconds < TShock.Config.PvpThrottle)
            {
                args.Player.SendMessage(string.Format("You cannot change pvp status for {0} seconds", TShock.Config.PvpThrottle - seconds), 255, 0, 0);
                args.Player.SetPvP(id != args.Player.Index || TShock.Config.AlwaysPvP ? true : args.TPlayer.hostile);
            }
            else
            {
                args.Player.SetPvP(id != args.Player.Index || TShock.Config.AlwaysPvP ? true : pvp);
            }
            return true;
        }

        private static bool HandleSendSection(GetDataHandlerArgs args)
        {
            return TShock.Utils.HandleGriefer(args.Player, TShock.Config.SendSectionAbuseReason);
        }

        private static bool HandleNpcUpdate(GetDataHandlerArgs args)
        {
            return TShock.Utils.HandleGriefer(args.Player, TShock.Config.NPCSpawnAbuseReason);
        }

        private static bool HandlePlayerUpdate(GetDataHandlerArgs args)
        {
            byte plr = args.Data.ReadInt8();
            byte control = args.Data.ReadInt8();
            byte item = args.Data.ReadInt8();
            float posx = args.Data.ReadSingle();
            float posy = args.Data.ReadSingle();
            float velx = args.Data.ReadSingle();
            float vely = args.Data.ReadSingle();

            if (Main.verboseNetplay)
                Debug.WriteLine("Update: {{{0},{1}}} {{{2}, {3}}}", (int)posx, (int)posy, (int)velx, (int)vely);

            if (plr != args.Player.Index)
            {
                return TShock.Utils.HandleGriefer(args.Player, TShock.Config.UpdatePlayerAbuseReason);
            }

            if (item < 0 || item >= args.TPlayer.inventory.Length)
            {
                TShock.Utils.HandleGriefer(args.Player, TShock.Config.UpdatePlayerAbuseReason);
                return true;
            }

            return false;
        }

        private static bool HandleProjectileNew(GetDataHandlerArgs args)
        {
            short ident = args.Data.ReadInt16();
            float posx = args.Data.ReadSingle();
            float posy = args.Data.ReadSingle();
            float velx = args.Data.ReadSingle();
            float vely = args.Data.ReadSingle();
            float knockback = args.Data.ReadSingle();
            short dmg = args.Data.ReadInt16();
            byte owner = args.Data.ReadInt8();
            byte type = args.Data.ReadInt8();

            var index = TShock.Utils.SearchProjectile(ident);

            if (index > Main.maxProjectiles || index < 0)
            {
                TShock.Utils.HandleGriefer(args.Player, TShock.Config.ExplosiveAbuseReason);
                return true;
            }

            if (type == 23)
            {
                if (velx == 0f && vely == 0f && dmg == 99)
                {
                    TShock.Utils.HandleGriefer(args.Player, TShock.Config.ProjectileAbuseReason);
                    return true;
                }
                else if (velx == 0f || vely == 0f)
                    return true;
            }

            if (type == 29 || type == 28 || type == 37) //need more explosives from 1.1
            {
                Log.Debug(string.Format("Explosive(PlyXY:{0}_{1}, Type:{2})", args.Player.TileX, args.Player.TileY, type));
                if (TShock.Config.DisableExplosives && (!args.Player.Group.HasPermission(Permissions.useexplosives) || !args.Player.Group.HasPermission(Permissions.ignoregriefdetection)))
                {
                    //Main.projectile[index].SetDefaults(0);
                    Main.projectile[index].type = 0;
                    //Main.projectile[index].owner = 255;
                    //Main.projectile[index].position = new Vector2(0f, 0f);
                    Main.projectile[index].identity = ident;
                    args.Player.SendData(PacketTypes.ProjectileNew, "", index);
                    args.Player.SendMessage("Explosives are disabled!", Color.Red);
                    args.Player.LastExplosive = DateTime.UtcNow;
                    return true;
                }
                else
                    return TShock.Utils.HandleExplosivesUser(args.Player, TShock.Config.ExplosiveAbuseReason);
            }
            if (args.Player.Index != owner)//ignores projectiles whose senders aren't the same as their owners
            {
                TShock.Players[args.Player.Index].SendData(PacketTypes.ProjectileNew, "", index);//update projectile on senders end so he knows it didnt get created
                return true;
            }
            Projectile proj = new Projectile();
            proj.SetDefaults(type);
            if (proj.hostile)//ignores all hostile projectiles from the client they shouldn't be sending them anyways
            {
                TShock.Players[args.Player.Index].SendData(PacketTypes.ProjectileNew, "", index);
                return true;
            }
            return false;
        }

        private static bool HandlePlayerKillMe(GetDataHandlerArgs args)
        {
            byte id = args.Data.ReadInt8();
            byte direction = args.Data.ReadInt8();
            short dmg = args.Data.ReadInt16();
            bool pvp = args.Data.ReadInt8() == 0;
            int textlength = (int)(args.Data.Length - args.Data.Position - 1);
            string deathtext = "";
            if (textlength > 0)
            {
                deathtext = Encoding.ASCII.GetString(args.Data.ReadBytes(textlength));
                if (!TShock.Utils.ValidString(deathtext))
                {
                    TShock.Utils.HandleGriefer(args.Player, "Death text exploit.");
                    return true;
                }
            }
            if (id != args.Player.Index)
            {
                return TShock.Utils.HandleGriefer(args.Player, TShock.Config.KillMeAbuseReason);
            }
            args.Player.LastDeath = DateTime.Now;
            if (args.Player.Difficulty != 2)
                args.Player.ForceSpawn = true;
            return false;
        }

        private static bool HandlePlayerDamage(GetDataHandlerArgs args)
        {
            byte playerid = args.Data.ReadInt8();
            if (TShock.Players[playerid] == null)
                return true;

            return !TShock.Players[playerid].TPlayer.hostile;
        }

        private static bool HandleLiquidSet(GetDataHandlerArgs args)
        {
            int x = args.Data.ReadInt32();
            int y = args.Data.ReadInt32();
            byte liquid = args.Data.ReadInt8();
            bool lava = args.Data.ReadBoolean();

            //The liquid was picked up.
            if (liquid == 0)
                return false;

            int plyX = Math.Abs(args.Player.TileX);
            int plyY = Math.Abs(args.Player.TileY);
            int tileX = Math.Abs(x);
            int tileY = Math.Abs(y);

            bool bucket = false;
            for (int i = 0; i < 49; i++)
            {
                if (args.TPlayer.inventory[i].type >= 205 && args.TPlayer.inventory[i].type <= 207)
                {
                    bucket = true;
                    break;
                }
            }

            if (!args.Player.Group.HasPermission(Permissions.canbuild))
            {
                args.Player.SendMessage("You do not have permission to build!", Color.Red);
                args.Player.SendTileSquare(x, y);
                return true;
            }

            if (lava && !args.Player.Group.HasPermission(Permissions.canlava))
            {
                args.Player.SendMessage("You do not have permission to use lava", Color.Red);
                TShock.Utils.SendLogs(string.Format("{0} tried using lava", args.Player.Name), Color.Red);
                args.Player.SendTileSquare(x, y);
                return true;
            }
            if (!lava && !args.Player.Group.HasPermission(Permissions.canwater))
            {
                args.Player.SendMessage("You do not have permission to use water", Color.Red);
                TShock.Utils.SendLogs(string.Format("{0} tried using water", args.Player.Name), Color.Red);
                args.Player.SendTileSquare(x, y);
                return true;
            }

            if (!bucket)
            {
                Log.Debug(string.Format("{0}(PlyXY:{1}_{2}, TileXY:{3}_{4}, Result:{5}_{6}, Amount:{7})",
                                        lava ? "Lava" : "Water", plyX, plyY, tileX, tileY,
                                        Math.Abs(plyX - tileX), Math.Abs(plyY - tileY), liquid));
                return TShock.Utils.HandleGriefer(args.Player, TShock.Config.IllogicalLiquidUseReason); ;
            }
            if (TShock.Config.RangeChecks && ((Math.Abs(plyX - tileX) > 32) || (Math.Abs(plyY - tileY) > 32)))
            {
                Log.Debug(string.Format("Liquid(PlyXY:{0}_{1}, TileXY:{2}_{3}, Result:{4}_{5}, Amount:{6})",
                                        plyX, plyY, tileX, tileY, Math.Abs(plyX - tileX), Math.Abs(plyY - tileY), liquid));
                return TShock.Utils.HandleGriefer(args.Player, TShock.Config.LiquidAbuseReason);
            }

            if (TShock.Config.SpawnProtection)
            {
                if (!args.Player.Group.HasPermission(Permissions.editspawn))
                {
                    var flag = TShock.CheckSpawn(x, y);
                    if (flag)
                    {
                        args.Player.SendMessage("The spawn is protected!", Color.Red);
                        args.Player.SendTileSquare(x, y);
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool HandleTileKill(GetDataHandlerArgs args)
        {
            int tilex = args.Data.ReadInt32();
            int tiley = args.Data.ReadInt32();
            if (tilex < 0 || tilex >= Main.maxTilesX || tiley < 0 || tiley >= Main.maxTilesY)
                return false;

            if (Main.tile[tilex, tiley].type != 0x15 && (!TShock.Utils.MaxChests() && Main.tile[tilex, tiley].type != 0)) //Chest
            {
                Log.Debug(string.Format("TileKill(TileXY:{0}_{1}, Type:{2})",
                                        tilex, tiley, Main.tile[tilex, tiley].type));
                TShock.Utils.ForceKick(args.Player, string.Format(TShock.Config.TileKillAbuseReason, Main.tile[tilex, tiley].type));
                return true;
            }
            if (!args.Player.Group.HasPermission(Permissions.canbuild))
            {
                args.Player.SendMessage("You do not have permission to build!", Color.Red);
                args.Player.SendTileSquare(tilex, tiley);
                return true;
            }
            if (!args.Player.Group.HasPermission(Permissions.editspawn) && !TShock.Regions.CanBuild(tilex, tiley, args.Player) && TShock.Regions.InArea(tilex, tiley))
            {
                args.Player.SendMessage("Region protected from changes.", Color.Red);
                args.Player.SendTileSquare(tilex, tiley);
                return true;
            }
            if (TShock.Config.DisableBuild)
            {
                if (!args.Player.Group.HasPermission(Permissions.editspawn))
                {
                    args.Player.SendMessage("World protected from changes.", Color.Red);
                    args.Player.SendTileSquare(tilex, tiley);
                    return true;
                }
            }
            if (TShock.Config.SpawnProtection)
            {
                if (!args.Player.Group.HasPermission(Permissions.editspawn))
                {
                    var flag = TShock.CheckSpawn(tilex, tiley);
                    if (flag)
                    {
                        args.Player.SendMessage("Spawn protected from changes.", Color.Red);
                        args.Player.SendTileSquare(tilex, tiley);
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool HandleSpawn(GetDataHandlerArgs args)
        {
            byte player = args.Data.ReadInt8();
            int spawnx = args.Data.ReadInt32();
            int spawny = args.Data.ReadInt32();

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

        private static bool HandleChest(GetDataHandlerArgs args)
        {
            var x = args.Data.ReadInt32();
            var y = args.Data.ReadInt32();
            if (TShock.Config.RangeChecks && ((Math.Abs(args.Player.TileX - x) > 32) || (Math.Abs(args.Player.TileY - y) > 32)))
            {
                return TShock.Utils.HandleGriefer(args.Player, TShock.Config.RangeCheckBanReason);
            }
            return false;
        }

        private static bool HandleSign(GetDataHandlerArgs args)
        {
            var id = args.Data.ReadInt16();
            var x = args.Data.ReadInt32();
            var y = args.Data.ReadInt32();
            if (TShock.Config.RangeChecks && ((Math.Abs(args.Player.TileX - x) > 32) || (Math.Abs(args.Player.TileY - y) > 32)))
            {
                return TShock.Utils.HandleGriefer(args.Player, TShock.Config.RangeCheckBanReason);
            }
            return false;
        }

        private static bool HandleGetSection(GetDataHandlerArgs args)
        {
            var x = args.Data.ReadInt32();
            var y = args.Data.ReadInt32();

            if (args.Player.RequestedSection)
            {
                TShock.Utils.ForceKick(args.Player, "Requested sections more than once.");
                return true;
            }
            args.Player.RequestedSection = true;
            return false;
        }

        private static bool UpdateNPCHome( GetDataHandlerArgs args )
        {
            if (!args.Player.Group.HasPermission(Permissions.movenpc))
            {
                args.Player.SendMessage("You do not have permission to relocate NPCs.", Color.Red);
                return true;
            }
            return false;
        }

        private static bool HandlePlayerBuff(GetDataHandlerArgs args)
        {
            return !args.Player.Group.HasPermission(Permissions.ignoregriefdetection);
        }
    }
}
