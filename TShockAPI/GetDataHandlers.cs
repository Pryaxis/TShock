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
            int namelength = (int)(args.Data.Length - args.Data.Position - 1);

            if (namelength > 0)
            {
                string itemname = Encoding.ASCII.GetString(args.Data.ReadBytes(namelength));

                if (!args.Player.Group.HasPermission(Permissions.usebanneditem) && TShock.Itembans.ItemIsBanned(itemname))
                    args.Player.Disconnect("Using banned item: " + itemname + ", remove it and rejoin");
                if (itemname == "KANNIBALE BLADE"
                    || itemname == "Super Gel")
                    return TShock.Utils.HandleCheater(args.Player, string.Format(TShock.Config.GriefClientReason, "KANNIBALE"));
                if (TShock.Utils.GetItemByName(itemname).Count == 0 && !args.Player.Group.HasPermission(Permissions.ignorecheatdetection)
                    && TShock.Config.KickCustomItems)
                    args.Player.Disconnect("Using custom item: " + itemname + ", remove it and region");
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
                }
            }

            if (changed)
                TSPlayer.All.SendTileSquare(tilex, tiley, 3);

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
                    if (!(type == 1 && ((tiletype == 0 && args.Player.TPlayer.selectedItem == 114) || (tiletype == 53 && args.Player.TPlayer.selectedItem == 266))))
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
                    args.Player.TilesDestroyed.Add(coords, Main.tile[x, y].Data);
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

            if (ident > Main.maxProjectiles || ident < 0)
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


            if (type == 29 || type == 28 || type == 37)
            {
                Log.Debug(string.Format("Explosive(PlyXY:{0}_{1}, Type:{2})", args.Player.TileX, args.Player.TileY, type));
                if (TShock.Config.DisableExplosives && (!args.Player.Group.HasPermission(Permissions.useexplosives) || !args.Player.Group.HasPermission(Permissions.ignoregriefdetection)))
                {
                    Main.projectile[ident].type = 0;
                    args.Player.SendData(PacketTypes.ProjectileNew, "", ident);
                    args.Player.SendMessage("Explosives are disabled!", Color.Red);
                    args.Player.LastExplosive = DateTime.UtcNow;
                    //return true;
                }
                else
                    return TShock.Utils.HandleExplosivesUser(args.Player, TShock.Config.ExplosiveAbuseReason);
            }
            return false;
        }

        private static bool HandlePlayerKillMe(GetDataHandlerArgs args)
        {
            byte id = args.Data.ReadInt8();
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
            for (int i = 0; i < 44; i++)
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
    }
}
