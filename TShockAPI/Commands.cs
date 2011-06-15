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
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.Xna.Framework;
using Terraria;

namespace TShockAPI
{
    public delegate void CommandDelegate(CommandArgs args);
    public class CommandArgs : EventArgs
    {
        public string Message { get; private set; }
        public TSPlayer Player { get; private set; }
        /// <summary>
        /// Parameters passed to the arguement. Does not include the command name. 
        /// IE '/kick "jerk face"' will only have 1 argument
        /// </summary>
        public List<string> Parameters { get; private set; }

        public Player TPlayer
        {
            get { return Player.TPlayer; }
        }

        public int PlayerID
        {
            get { return Player.Index; }
        }

        public CommandArgs(string message, TSPlayer ply, List<string> args)
        {
            Message = message;
            Player = ply;
            Parameters = args;
        }
    }
    public class Command
    {
        public string Name { get; protected set; }
        private string permission;
        private CommandDelegate command;

        public Command(string cmdname, string permissionneeded, CommandDelegate cmd)
        {
            Name = cmdname;
            permission = permissionneeded;
            command = cmd;
        }

        public bool Run(string msg, TSPlayer ply, List<string> parms)
        {
            if (!ply.Group.HasPermission(permission))
                return false;

            command(new CommandArgs(msg, ply, parms));
            return true;
        }

        public bool CanRun(TSPlayer ply)
        {
            if (!ply.Group.HasPermission(permission))
            {
                return false;
            }
            return true;
        }
    }
    public static class Commands
    {
        public static List<Command> ChatCommands = new List<Command>();

        /// <summary>
        /// Parses a string of parameters into a list. Handles quotes.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<String> ParseParameters(string str)
        {
            var ret = new List<string>();
            var sb = new StringBuilder();
            bool instr = false;
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];

                if (instr)
                {
                    if (c == '\\')
                    {
                        if (i + 1 >= str.Length)
                            break;
                        c = GetEscape(str[++i]);
                    }
                    else if (c == '"')
                    {
                        ret.Add(sb.ToString());
                        sb.Clear();
                        instr = false;
                        continue;
                    }
                    sb.Append(c);
                }
                else
                {
                    if (IsWhiteSpace(c))
                    {
                        if (sb.Length > 0)
                        {
                            ret.Add(sb.ToString());
                            sb.Clear();
                        }
                    }
                    else if (c == '"')
                    {
                        if (sb.Length > 0)
                        {
                            ret.Add(sb.ToString());
                            sb.Clear();
                        }
                        instr = true;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }
            if (sb.Length > 0)
                ret.Add(sb.ToString());

            return ret;
        }

        static char GetEscape(char c)
        {
            switch (c)
            {
                case '\\':
                    return '\\';
                case '"':
                    return '"';
                case 't':
                    return '\t';
                default:
                    return c;
            }
        }

        static bool IsWhiteSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\n';
        }

        public static void InitCommands()
        {
            ChatCommands.Add(new Command("kick", "kick", Kick));
            ChatCommands.Add(new Command("ban", "ban", Ban));
            ChatCommands.Add(new Command("banip", "ban", BanIP));
            ChatCommands.Add(new Command("unban", "unban", UnBan));
            ChatCommands.Add(new Command("unbanip", "unban", UnBanIP));
            ChatCommands.Add(new Command("off", "maintenance", Off));
            ChatCommands.Add(new Command("off-nosave", "maintenance", OffNoSave));
            ChatCommands.Add(new Command("checkupdates", "maintenance", CheckUpdates));
            ChatCommands.Add(new Command("dropmeteor", "causeevents", DropMeteor));
            ChatCommands.Add(new Command("star", "causeevents", Star));
            ChatCommands.Add(new Command("bloodmoon", "causeevents", Bloodmoon));
            ChatCommands.Add(new Command("invade", "causeevents", Invade));
            ChatCommands.Add(new Command("eater", "spawnboss", Eater));
            ChatCommands.Add(new Command("eye", "spawnboss", Eye));
            ChatCommands.Add(new Command("skeletron", "spawnboss", Skeletron));
            ChatCommands.Add(new Command("hardcore", "spawnboss", Hardcore));
            ChatCommands.Add(new Command("spawnmob", "spawnmob", SpawnMob));
            ChatCommands.Add(new Command("home", "tp", Home));
            ChatCommands.Add(new Command("spawn", "tp", Spawn));
            ChatCommands.Add(new Command("tp", "tp", TP));
            ChatCommands.Add(new Command("tphere", "tp", TPHere));
            ChatCommands.Add(new Command("reload", "cfg", Reload));
            ChatCommands.Add(new Command("debug-config", "cfg", DebugConfiguration));
            ChatCommands.Add(new Command("password", "cfg", Password));
            ChatCommands.Add(new Command("save", "cfg", Save));
            ChatCommands.Add(new Command("maxspawns", "cfg", MaxSpawns));
            ChatCommands.Add(new Command("spawnrate", "cfg", SpawnRate));
            ChatCommands.Add(new Command("time", "cfg", Time));
            ChatCommands.Add(new Command("slap", "pvpfun", Slap));
            ChatCommands.Add(new Command("antibuild", "editspawn", ToggleAntiBuild));
            ChatCommands.Add(new Command("protectspawn", "editspawn", ProtectSpawn));
            ChatCommands.Add(new Command("help", "", Help));
            ChatCommands.Add(new Command("playing", "", Playing));
            ChatCommands.Add(new Command("online", "", Playing));
            ChatCommands.Add(new Command("who", "", Playing));
            ChatCommands.Add(new Command("auth", "", AuthToken));
            ChatCommands.Add(new Command("me", "", ThirdPerson));
            ChatCommands.Add(new Command("p", "", PartyChat));
            ChatCommands.Add(new Command("rules", "", Rules));
            if (ConfigurationManager.DistributationAgent != "terraria-online")
            {
                ChatCommands.Add(new Command("kill", "kill", Kill));
                ChatCommands.Add(new Command("butcher", "cheat", Butcher));
                ChatCommands.Add(new Command("i", "cheat", Item));
                ChatCommands.Add(new Command("item", "cheat", Item));
                ChatCommands.Add(new Command("give", "cheat", Give));
                ChatCommands.Add(new Command("heal", "cheat", Heal));
            }
        }

        #region Command Methods

        public static void Rules(CommandArgs args)
        {
            Tools.ShowFileToUser(args.Player, "rules.txt");
        }

        public static void CheckUpdates(CommandArgs args)
        {
            ThreadPool.QueueUserWorkItem(UpdateManager.CheckUpdate);
        }

        public static void PartyChat(CommandArgs args)
        {
            int playerTeam = args.Player.Team;
            if (playerTeam != 0)
            {
                string msg = string.Format("<{0}> {1}", args.Player.Name, args.Message.Remove(0, 2));
                for (int i = 0; i < TShock.Players.Length; i++)
                {
                    if (TShock.Players[i] != null && TShock.Players[i].Active && TShock.Players[i].Team == playerTeam)
                    {
                       TShock.Players[i].SendMessage(msg, Main.teamColor[playerTeam].R, Main.teamColor[playerTeam].G, Main.teamColor[playerTeam].B);
                    }
                }
            }
            else
            {
                args.Player.SendMessage("You are not in a party!", 255, 240, 20);
            }
        }

        public static void ThirdPerson(CommandArgs args)
        {
            string msg = args.Message.Remove(0, 3);
            Tools.Broadcast(string.Format("*{0} {1}", args.Player.Name, msg), 205, 133, 63);
        }

        public static void Playing(CommandArgs args)
        {
            args.Player.SendMessage(string.Format("Current players: {0}.", Tools.GetPlayers()), 255, 240, 20);
        }

        public static void Kick(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /kick <player> [reason]", Color.Red);
                return;
            }
            if (args.Parameters[0].Length == 0)
            {
                args.Player.SendMessage("Missing player name", Color.Red);
                return;
            }

            string plStr = args.Parameters[0];
            var player = Tools.FindPlayer(plStr);
            if (player.Count == 0)
            {
                args.Player.SendMessage("Invalid player!", Color.Red);
            }
            else if (player.Count > 1)
            {
                args.Player.SendMessage("More than one player matched!", Color.Red);
            }
            else
            {
                string reason = args.Parameters.Count > 1 ? String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1)) : "Misbehaviour.";
                if (!Tools.Kick(player[0], reason))
                {
                    args.Player.SendMessage("You can't kick another admin!", Color.Red);
                }
            }
        }

        public static void Ban(CommandArgs args)
        {
            int adminplr = args.PlayerID;

            if (args.Parameters.Count < 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /ban <player> [reason]", Color.Red);
                return;
            }
            if (args.Parameters[0].Length == 0)
            {
                args.Player.SendMessage("Missing player name", Color.Red);
                return;
            }

            string plStr = args.Parameters[0];
            var player = Tools.FindPlayer(plStr);
            if (player.Count == 0)
            {
                args.Player.SendMessage("Invalid player!", Color.Red);
            }
            else if (player.Count > 1)
            {
                args.Player.SendMessage("More than one player matched!", Color.Red);
            }
            else
            {
                string reason = args.Parameters.Count > 1 ? String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1)) : "Misbehaviour.";
                if (!Tools.Ban(player[0], reason))
                {
                    args.Player.SendMessage("You can't ban another admin!", Color.Red);
                }
            }
        }

        public static void BanIP(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendMessage("Syntax: /banip <ip> [reason]", Color.Red);
                return;
            }
            if (args.Parameters[0].Length == 0)
            {
                args.Player.SendMessage("Missing IP address", Color.Red);
                return;
            }

            string ip = args.Parameters[0];
            string reason = args.Parameters.Count > 1 ? String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1)) : "Manually added IP address ban.";
            TShock.Bans.AddBan(ip, "", reason);
        }

        public static void UnBan(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /unban <player>", Color.Red);
                return;
            }
            if (args.Parameters[0].Length == 0)
            {
                args.Player.SendMessage("Missing player name", Color.Red);
                return;
            }

            string plStr = args.Parameters[0];
            var ban = TShock.Bans.GetBanByName(plStr);
            if (ban != null)
            {
                TShock.Bans.RemoveBan(ban);
                args.Player.SendMessage(string.Format("Unbanned {0} ({1})!", ban.Name, ban.IP), Color.Red);
            }
            else
            {
                args.Player.SendMessage("Invalid player!", Color.Red);
            }
        }

        public static void UnBanIP(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /unbanip <ip>", Color.Red);
                return;
            }
            if (args.Parameters[0].Length == 0)
            {
                args.Player.SendMessage("Missing ip", Color.Red);
                return;
            }

            string plStr = args.Parameters[0];
            var ban = TShock.Bans.GetBanByIp(plStr);
            if (ban != null)
            {
                TShock.Bans.RemoveBan(ban);
                args.Player.SendMessage(string.Format("Unbanned {0} ({1})!", ban.Name, ban.IP), Color.Red);
            }
            else
            {
                args.Player.SendMessage("Invalid player!", Color.Red);
            }
        }

        public static void Off(CommandArgs args)
        {
            Tools.ForceKickAll("Server shutting down!");
            WorldGen.saveWorld();
            Netplay.disconnect = true;
        }

        public static void OffNoSave(CommandArgs args)
        {
            Tools.ForceKickAll("Server shutting down!");
            Netplay.disconnect = true;
        }

        public static void DebugConfiguration(CommandArgs args)
        {
            args.Player.SendMessage("TShock Config:");
            string lineOne = string.Format("BanCheater : {0}, KickCheater : {1}, BanGriefer : {2}, KickGriefer : {3}",
                              ConfigurationManager.BanCheater, ConfigurationManager.KickCheater,
                              ConfigurationManager.BanGriefer, ConfigurationManager.KickGriefer);
            args.Player.SendMessage(lineOne, Color.Yellow);
            string lineTwo = string.Format("BanTnt : {0}, KickTnt : {1}, BanBoom : {2}, KickBoom : {3}",
                                           ConfigurationManager.BanTnt, ConfigurationManager.KickTnt,
                                           ConfigurationManager.BanBoom, ConfigurationManager.KickBoom);
            args.Player.SendMessage(lineTwo, Color.Yellow);
            string lineThree = string.Format("RangeChecks : {0}, DisableBuild : {1}, ProtectSpawn : {2}, ProtectRadius : {3}",
                                             ConfigurationManager.RangeChecks, ConfigurationManager.DisableBuild,
                                             ConfigurationManager.SpawnProtect, ConfigurationManager.SpawnProtectRadius);
            args.Player.SendMessage(lineThree, Color.Yellow);
            string lineFour = string.Format("MaxSlots : {0}, SpamChecks : {1}, InvMultiplier : {2}, DMS : {3}, SpawnRate {4}",
                                           ConfigurationManager.MaxSlots, ConfigurationManager.SpamChecks,
                                           ConfigurationManager.InvasionMultiplier, ConfigurationManager.DefaultMaxSpawns,
                                           ConfigurationManager.DefaultSpawnRate);
            args.Player.SendMessage(lineFour, Color.Yellow);
        }

        public static void Reload(CommandArgs args)
        {
            FileTools.SetupConfig();
            args.Player.SendMessage("Configuration reload complete. Some changes may require server restart.");
        }

        public static void DropMeteor(CommandArgs args)
        {
            WorldGen.spawnMeteor = false;
            WorldGen.dropMeteor();
        }

        public static void Star(CommandArgs args)
        {
            int penis56 = 12;
            int penis57 = Main.rand.Next(Main.maxTilesX - 50) + 100;
            penis57 *= 0x10;
            int penis58 = Main.rand.Next((int)(Main.maxTilesY * 0.05)) * 0x10;
            Vector2 vector = new Vector2(penis57, penis58);
            float speedX = Main.rand.Next(-100, 0x65);
            float speedY = Main.rand.Next(200) + 100;
            float penis61 = (float)Math.Sqrt(((speedX * speedX) + (speedY * speedY)));
            penis61 = (penis56) / penis61;
            speedX *= penis61;
            speedY *= penis61;
            Projectile.NewProjectile(vector.X, vector.Y, speedX, speedY, 12, 0x3e8, 10f, Main.myPlayer);
        }

        public static void Bloodmoon(CommandArgs args)
        {
            Tools.Broadcast(string.Format("{0} turned on blood moon.", args.Player.Name));
            Main.bloodMoon = true;
            Main.time = 0;
            Main.dayTime = false;
            NetMessage.SendData(18, -1, -1, "", 0, 0, Main.sunModY, Main.moonModY);
            NetMessage.syncPlayers();
        }

        public static void Eater(CommandArgs args)
        {
            Tools.NewNPC(NPCList.WORLD_EATER, args.Player);
            Tools.Broadcast(string.Format("{0} has spawned an eater of worlds!", args.Player.Name));
        }

        public static void Eye(CommandArgs args)
        {
            Tools.NewNPC(NPCList.EYE, args.Player);
            Tools.Broadcast(string.Format("{0} has spawned an eye!", args.Player.Name));
        }

        public static void Skeletron(CommandArgs args)
        {
            Tools.NewNPC(NPCList.SKELETRON, args.Player);
            Tools.Broadcast(string.Format("{0} has spawned skeletron!", args.Player.Name));
        }

        public static void Hardcore(CommandArgs args)
        {
            foreach (NPCList type in Enum.GetValues(typeof(NPCList)))
            {
                Tools.NewNPC(type, args.Player);
            }
            Tools.Broadcast(string.Format("{0} has spawned all bosses!", args.Player.Name));
        }

        public static void Invade(CommandArgs args)
        {
            if (Main.invasionSize <= 0)
            {
                Tools.Broadcast(string.Format("{0} has started an invasion.", args.Player.Name));
                TShock.StartInvasion();
            }
            else
            {
                Tools.Broadcast(string.Format("{0} has ended an invasion.", args.Player.Name));
                Main.invasionSize = 0;
            }
        }

        public static void Password(CommandArgs args)
        {
            string passwd = args.Message.Remove(0, 9).Trim();
            Netplay.password = passwd;
            args.Player.SendMessage(string.Format("Server password changed to: {0}", passwd));
        }

        public static void Save(CommandArgs args)
        {
            WorldGen.saveWorld();
            args.Player.SendMessage("World saved.");
        }

        public static void Home(CommandArgs args)
        {
            int ply = args.PlayerID;
            TShock.Teleport(ply, Main.player[args.PlayerID].SpawnX * 16 + 8 - Main.player[ply].width / 2,
                            Main.player[args.PlayerID].SpawnY * 16 - Main.player[ply].height);
            args.Player.SendMessage("Teleported to your spawnpoint.");
        }

        public static void Spawn(CommandArgs args)
        {
            int ply = args.PlayerID;
            TShock.Teleport(ply, Main.spawnTileX * 16 + 8 - Main.player[ply].width / 2,
                            Main.spawnTileY * 16 - Main.player[ply].height);
            args.Player.SendMessage("Teleported to the map's spawnpoint.");
        }

        public static void AuthToken(CommandArgs args)
        {
            if (ConfigurationManager.AuthToken == 0)
            {
                return;
            }
            int givenCode = Convert.ToInt32(args.Parameters[0]);
            if (givenCode == ConfigurationManager.AuthToken)
            {
                TextWriter tw = new StreamWriter(FileTools.UsersPath, true);
                tw.Write("\n" +
                         Tools.GetRealIP(
                             Convert.ToString(Netplay.serverSock[args.PlayerID].tcpClient.Client.RemoteEndPoint)) +
                         " superadmin");
                args.Player.SendMessage("SuperAdmin authenticated. Please re-connect using the same IP.");
                ConfigurationManager.AuthToken = 0;
                tw.Close();
            }
        }

        public static void TP(CommandArgs args)
        {
            int adminplr = args.PlayerID;

            if (args.Parameters.Count < 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /tp <player> ", Color.Red);
                return;
            }

            string plStr = String.Join(" ", args.Parameters);
            var player = Tools.FindPlayer(plStr);
            if (player.Count == 0)
                args.Player.SendMessage("Invalid player!", Color.Red);
            else if (player.Count > 1)
                args.Player.SendMessage("More than one player matched!", Color.Red);
            else
            {
                var plr = player[0];
                TShock.Teleport(adminplr, plr.TPlayer.position.X, plr.TPlayer.position.Y);
                args.Player.SendMessage(string.Format("Teleported to {0}", plr.Name));
            }
        }

        public static void TPHere(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /tphere <player> ", Color.Red);
                return;
            }

            string plStr = String.Join(" ", args.Parameters);
            var player = Tools.FindPlayer(plStr);
            if (player.Count == 0)
            {
                args.Player.SendMessage("Invalid player!", Color.Red);
            }
            else if (player.Count > 1)
            {
                args.Player.SendMessage("More than one player matched!", Color.Red);
            }
            else
            {
                var plr = player[0];
                TShock.Teleport(plr.Index, args.TPlayer.position.X, args.TPlayer.position.Y);
                plr.SendMessage(string.Format("You were teleported to {0}.", plr.Name));
                args.Player.SendMessage(string.Format("You brought {0} here.", plr.Name));
            }
        }

        public static void SpawnMob(CommandArgs args)
        {
            if (args.Parameters.Count < 1 || args.Parameters.Count > 2)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /spawnmob <mob name/id> [amount]", Color.Red);
                return;
            }
            if (args.Parameters[0].Length == 0)
            {
                args.Player.SendMessage("Missing mob name/id", Color.Red);
                return;
            }

            int type = -1;
            int amount = 1;

            if (!int.TryParse(args.Parameters[0], out type))
                type = TShock.GetNPCID(args.Parameters[0]);
            if (args.Parameters.Count == 2 && !int.TryParse(args.Parameters[1], out amount))
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /spawnmob <mob name/id> [amount]", Color.Red);
                return;
            }

            if (type >= 1 && type < Main.maxNPCTypes)
            {
                int npcid = -1;
                for (int i = 0; i < amount; i++)
                    npcid = NPC.NewNPC((int)args.Player.X, (int)args.Player.Y, type, 0);
                Tools.Broadcast(string.Format("{0} was spawned {1} time(s).", Main.npc[npcid].name, amount));
            }
            else
                args.Player.SendMessage("Invalid mob type!", Color.Red);
        }

        public static void Item(CommandArgs args)
        {
            int adminplr = args.PlayerID;

            if (args.Parameters.Count < 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /item <item name/id>", Color.Red);
                return;
            }
            if (args.Parameters[0].Length == 0)
            {
                args.Player.SendMessage("Missing item name/id", Color.Red);
                return;
            }

            int type = -1;
            if (!int.TryParse(args.Parameters[0], out type))
                type = TShock.GetItemID(String.Join(" ", args.Parameters));

            if (type < 1 || type >= Main.maxItemTypes)
            {
                args.Player.SendMessage("Invalid item type!", Color.Red);
                return;
            }

            bool flag = false;
            for (int i = 0; i < 40; i++)
            {
                if (!Main.player[adminplr].inventory[i].active)
                {
                    int id = Terraria.Item.NewItem(0, 0, 0, 0, type, 1, true);
                    Main.item[id].position.X =  args.Player.X;
                    Main.item[id].position.Y = args.Player.Y;
                    Main.item[id].stack = Main.item[id].maxStack;
                    NetMessage.SendData(21, -1, -1, "", id, 0f, 0f, 0f);
                    args.Player.SendMessage(string.Format("Got some {0}.", Main.item[id].name));
                    flag = true;
                    break;
                }
            }
            if (!flag)
                args.Player.SendMessage("You don't have free slots!", Color.Red);
        }

        public static void Give(CommandArgs args)
        {
            if (args.Parameters.Count != 2)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /give <item type/id> <player>", Color.Red);
                return;
            }
            if (args.Parameters[0].Length == 0)
            {
                args.Player.SendMessage("Missing item name/id", Color.Red);
                return;
            }
            if (args.Parameters[1].Length == 0)
            {
                args.Player.SendMessage("Missing player name", Color.Red);
                return;
            }

            int type = -1;
            if (!int.TryParse(args.Parameters[0], out type))
                type = TShock.GetItemID(args.Parameters[0]);

            if (type < 1 || type >= Main.maxItemTypes)
            {
                args.Player.SendMessage("Invalid item type!", Color.Red);
                return;
            }

            string plStr = args.Parameters[1];
            var player = Tools.FindPlayer(plStr);
            if (player.Count == 0)
            {
                args.Player.SendMessage("Invalid player!", Color.Red);
            }
            else if (player.Count > 1)
            {
                args.Player.SendMessage("More than one player matched!", Color.Red);
            }
            else
            {
                var plr = player[0];
                bool flag = false;
                for (int i = 0; i < 40; i++)
                {
                    if (!plr.TPlayer.inventory[i].active)
                    {
                        //Main.player[player].inventory[i].SetDefaults(type);
                        //Main.player[player].inventory[i].stack = Main.player[player].inventory[i].maxStack;
                        int id = Terraria.Item.NewItem(0, 0, 0, 0, type, 1, true);
                        Main.item[id].position.X = plr.TPlayer.position.X;
                        Main.item[id].position.Y = plr.TPlayer.position.Y;
                        Main.item[id].stack = Main.item[id].maxStack;
                        //TShock.SendDataAll(21, -1, "", id);
                        NetMessage.SendData(21, -1, -1, "", id, 0f, 0f, 0f);
                        args.Player.SendMessage(
                                          string.Format("Gave {0} some {1}.", plr.Name, Main.item[id].name));
                        plr.SendMessage(
                                          string.Format("{0} gave you some {1}.", args.Player.Name,
                                                        Main.item[id].name));
                        //TShock.UpdateInventories();
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                    args.Player.SendMessage("Player does not have free slots!", Color.Red);
            }
        }

        public static void Heal(CommandArgs args)
        {
            if (args.Parameters.Count > 0)
            {
                string plStr = String.Join(" ", args.Parameters);
                var player = Tools.FindPlayer(plStr);
                if (player.Count == 0)
                {
                    args.Player.SendMessage("Invalid player!", Color.Red);
                }
                else if (player.Count > 1)
                {
                    args.Player.SendMessage("More than one player matched!", Color.Red);
                }
                else
                {
                    var plr = player[0];
                    DropHearts(plr.TPlayer.position.X, plr.TPlayer.position.Y, 20);
                    args.Player.SendMessage(string.Format("You just healed {0}", plr.Name));
                    plr.SendMessage(string.Format("{0} just healed you!", plr.Name));
                }
            }
            else
            {
                DropHearts(args.Player.X, args.Player.Y, 20);
                args.Player.SendMessage("You just got healed!");
            }
        }

        private static void DropHearts(float x, float y, int count)
        {
            for (int i = 0; i < count; i++)
            {
                int itemid = Terraria.Item.NewItem(1, 1, 1, 1, 58);
                Main.item[itemid].position.X = x;
                Main.item[itemid].position.Y = y;
                NetMessage.SendData(21, -1, -1, "", itemid, 0f, 0f, 0f);
            }
        }

        public static void Butcher(CommandArgs args)
        {
            int killcount = 0;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (!Main.npc[i].townNPC && !Main.npc[i].active)
                {
                    Main.npc[i].StrikeNPC(99999, 90f, 1);
                    NetMessage.SendData(28, -1, -1, "", i, 99999, 90f, 1);
                    killcount++;
                }
            }
            Tools.Broadcast(string.Format("Killed {0} NPCs.", killcount));
        }

        public static void MaxSpawns(CommandArgs args)
        {

            if (args.Parameters.Count != 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /maxspawns <maxspawns>", Color.Red);
                return;
            }

            int amount = Convert.ToInt32(args.Parameters[0]);
            int.TryParse(args.Parameters[0], out amount);
            NPC.defaultMaxSpawns = amount;
            ConfigurationManager.DefaultMaxSpawns = amount;
            Tools.Broadcast(string.Format("{0} changed the maximum spawns to: {1}", args.Player.Name, amount));
        }

        public static void SpawnRate(CommandArgs args)
        {
            if (args.Parameters.Count != 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /spawnrate <spawnrate>", Color.Red);
                return;
            }

            int amount = Convert.ToInt32(args.Parameters[0]);
            int.TryParse(args.Parameters[0], out amount);
            NPC.defaultSpawnRate = amount;
            ConfigurationManager.DefaultSpawnRate = amount;
            Tools.Broadcast(string.Format("{0} changed the spawn rate to: {1}", args.Player.Name, amount));
        }

        public static void Help(CommandArgs args)
        {
            args.Player.SendMessage("TShock Commands:");
            int page = 1;
            if (args.Parameters.Count > 0)
                int.TryParse(args.Parameters[0], out page);
            var cmdlist = new List<Command>();
            for (int j = 0; j < ChatCommands.Count; j++)
            {
                if (ChatCommands[j].CanRun(args.Player))
                {
                    cmdlist.Add(ChatCommands[j]);
                }
            }
            var sb = new StringBuilder();
            if (cmdlist.Count > (15 * (page - 1)))
            {
                for (int j = (15 * (page - 1)); j < (15 * page); j++)
                {
                    if (sb.Length != 0)
                        sb.Append(", ");
                    sb.Append("/").Append(cmdlist[j].Name);
                    if (j == cmdlist.Count - 1)
                    {
                        args.Player.SendMessage(sb.ToString(), Color.Yellow);
                        break;
                    }
                    if ((j + 1) % 5 == 0)
                    {
                        args.Player.SendMessage(sb.ToString(), Color.Yellow);
                        sb.Clear();
                    }
                }
            }
            if (cmdlist.Count > (15 * page))
            {
                args.Player.SendMessage(string.Format("Type /help {0} for more commands.", (page + 1)), Color.Yellow);
            }
        }

        public static void Time(CommandArgs args)
        {
            if (args.Parameters.Count != 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /time <day/night/dusk/noon/midnight>", Color.Red);
                return;
            }

            switch (args.Parameters[0])
            {
                case "day":
                    Main.time = 0;
                    Main.dayTime = true;
                    NetMessage.SendData(18, -1, -1, "", 0, 0, Main.sunModY, Main.moonModY);
                    NetMessage.syncPlayers();
                    Tools.Broadcast(string.Format("{0} set time to day.", args.Player.Name));
                    break;
                case "night":
                    Main.time = 0;
                    Main.dayTime = false;
                    NetMessage.SendData(18, -1, -1, "", 0, 0, Main.sunModY, Main.moonModY);
                    NetMessage.syncPlayers();
                    Tools.Broadcast(string.Format("{0} set time to night.", args.Player.Name));
                    break;
                case "dusk":
                    Main.dayTime = false;
                    Main.time = 0.0;
                    NetMessage.SendData(18, -1, -1, "", 0, 0, Main.sunModY, Main.moonModY);
                    NetMessage.syncPlayers();
                    Tools.Broadcast(string.Format("{0} set time to dusk.", args.Player.Name));
                    break;
                case "noon":
                    Main.dayTime = true;
                    Main.time = 27000.0;
                    NetMessage.SendData(18, -1, -1, "", 0, 0, Main.sunModY, Main.moonModY);
                    NetMessage.syncPlayers();
                    Tools.Broadcast(string.Format("{0} set time to noon.", args.Player.Name));
                    break;
                case "midnight":
                    Main.dayTime = false;
                    Main.time = 16200.0;
                    NetMessage.SendData(18, -1, -1, "", 0, 0, Main.sunModY, Main.moonModY);
                    NetMessage.syncPlayers();
                    Tools.Broadcast(string.Format("{0} set time to midnight.", args.Player.Name));
                    break;
                default:
                    args.Player.SendMessage("Invalid syntax! Proper syntax: /time <day/night/dusk/noon/midnight>", Color.Red);
                    break;
            }
        }

        public static void Kill(CommandArgs args)
        {
            int adminplr = args.PlayerID;

            if (args.Parameters.Count < 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /kill <player>", Color.Red);
                return;
            }

            string plStr = String.Join(" ", args.Parameters);
            var player = Tools.FindPlayer(plStr);
            if (player.Count == 0)
            {
                args.Player.SendMessage("Invalid player!", Color.Red);
            }
            else if (player.Count > 1)
            {
                args.Player.SendMessage("More than one player matched!", Color.Red);
            }
            else
            {
                var plr = player[0];
                args.Player.SendMessage(string.Format("You just killed {0}!", plr.Name));
                plr.SendMessage(string.Format("{0} just killed you!", args.Player.Name));
                TShock.PlayerDamage(plr.Index, 999999);
            }
        }

        public static void Slap(CommandArgs args)
        {
            if (args.Parameters.Count < 1 || args.Parameters.Count > 2)
            {
                args.Player.SendMessage( "Invalid syntax! Proper syntax: /slap <player> [dmg]", Color.Red);
                return;
            }
            if (args.Parameters[0].Length == 0)
            {
                args.Player.SendMessage( "Missing player name", Color.Red);
                return;
            }

            string plStr = args.Parameters[0];
            var player = Tools.FindPlayer(plStr);
            if (player.Count == 0)
            {
                args.Player.SendMessage( "Invalid player!", Color.Red);
            }
            else if (player.Count > 1)
            {
                args.Player.SendMessage( "More than one player matched!", Color.Red);
            }
            else
            {
                var plr = player[0];
                int damage = 5;
                if (args.Parameters.Count == 2)
                {
                    int.TryParse(args.Parameters[1], out damage);
                }
                TShock.PlayerDamage(plr.Index, damage);
                Tools.Broadcast(string.Format("{0} slapped {1} for {2} damage.",
                                args.Player.Name, plr.Name, damage));
            }
        }

        public static void ToggleAntiBuild(CommandArgs args)
        {
            ConfigurationManager.DisableBuild = (ConfigurationManager.DisableBuild == false);
            Tools.Broadcast(string.Format("Anti-build is now {0}.", (ConfigurationManager.DisableBuild ? "on" : "off")));
        }

        public static void ProtectSpawn(CommandArgs args)
        {
            ConfigurationManager.SpawnProtect = (ConfigurationManager.SpawnProtect == false);
            Tools.Broadcast(string.Format("Spawn is now {0}.", (ConfigurationManager.SpawnProtect ? "protected" : "open")));
        }

        public static void UpdateNow(CommandArgs args)
        {
            Process TServer = Process.GetCurrentProcess();

            StreamWriter sw = new StreamWriter("pid");
            sw.Write(TServer.Id);
            sw.Close();

            sw = new StreamWriter("pn");
            sw.Write(TServer.ProcessName + " " + Environment.CommandLine);
            sw.Close();

            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "TShock");
            byte[] updatefile = client.DownloadData("http://tsupdate.shankshock.com/UpdateTShock.exe");

            BinaryWriter bw = new BinaryWriter(new FileStream("UpdateTShock.exe", FileMode.Create));
            bw.Write(updatefile);
            bw.Close();

            Process.Start(new ProcessStartInfo("UpdateTShock.exe"));

            Tools.ForceKickAll("Server shutting down for update!");
            WorldGen.saveWorld();
            Netplay.disconnect = true;
        }

        #endregion Command Methods
    }
}