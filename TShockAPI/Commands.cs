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

            try
            {
                command(new CommandArgs(msg, ply, parms));
            }
            catch (Exception e)
            {
                ply.SendMessage("Command failed, check logs for more details.");
                Log.Error(e.ToString());
            }

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

        public static void InitCommands()
        {
            ChatCommands.Add(new Command("kick", "kick", Kick));
            ChatCommands.Add(new Command("ban", "ban", Ban));
            ChatCommands.Add(new Command("banip", "ban", BanIP));
            ChatCommands.Add(new Command("unban", "unban", UnBan));
            ChatCommands.Add(new Command("unbanip", "unban", UnBanIP));
            ChatCommands.Add(new Command("whitelist", "whitelist", Whitelist));
            ChatCommands.Add(new Command("off", "maintenance", Off));
            ChatCommands.Add(new Command("off-nosave", "maintenance", OffNoSave));
            ChatCommands.Add(new Command("checkupdates", "maintenance", CheckUpdates));
            ChatCommands.Add(new Command("dropmeteor", "causeevents", DropMeteor));
            ChatCommands.Add(new Command("star", "causeevents", Star));
            ChatCommands.Add(new Command("bloodmoon", "causeevents", Bloodmoon));
            ChatCommands.Add(new Command("invade", "causeevents", Invade));
            ChatCommands.Add(new Command("eater", "spawnboss", Eater));
            ChatCommands.Add(new Command("eye", "spawnboss", Eye));
            ChatCommands.Add(new Command("king", "spawnboss", King));
            ChatCommands.Add(new Command("skeletron", "spawnboss", Skeletron));
            ChatCommands.Add(new Command("hardcore", "spawnboss", Hardcore));
            ChatCommands.Add(new Command("spawnmob", "spawnmob", SpawnMob));
            ChatCommands.Add(new Command("sm", "spawnmob", SpawnMob));
            ChatCommands.Add(new Command("home", "tp", Home));
            ChatCommands.Add(new Command("spawn", "tp", Spawn));
            ChatCommands.Add(new Command("tp", "tp", TP));
            ChatCommands.Add(new Command("tphere", "tphere", TPHere));
            ChatCommands.Add(new Command("warp", "warp", UseWarp));
            ChatCommands.Add(new Command("setwarp", "managewarp", SetWarp));
            ChatCommands.Add(new Command("delwarp", "managewarp", DeleteWarp));
            ChatCommands.Add(new Command("reload", "cfg", Reload));
            ChatCommands.Add(new Command("debug-config", "cfg", DebugConfiguration));
            ChatCommands.Add(new Command("password", "cfg", Password));
            ChatCommands.Add(new Command("save", "cfg", Save));
            ChatCommands.Add(new Command("maxspawns", "cfg", MaxSpawns));
            ChatCommands.Add(new Command("spawnrate", "cfg", SpawnRate));
            ChatCommands.Add(new Command("time", "time", Time));
            ChatCommands.Add(new Command("slap", "pvpfun", Slap));
            ChatCommands.Add(new Command("antibuild", "editspawn", ToggleAntiBuild));
            ChatCommands.Add(new Command("protectspawn", "editspawn", ProtectSpawn));
            ChatCommands.Add(new Command("region", "editspawn", Region));
            ChatCommands.Add(new Command("help", "", Help));
            ChatCommands.Add(new Command("playing", "", Playing));
            ChatCommands.Add(new Command("online", "", Playing));
            ChatCommands.Add(new Command("who", "", Playing));
            ChatCommands.Add(new Command("auth", "", AuthToken));
            ChatCommands.Add(new Command("me", "", ThirdPerson));
            ChatCommands.Add(new Command("p", "", PartyChat));
            ChatCommands.Add(new Command("rules", "", Rules));
            ChatCommands.Add(new Command("displaylogs", "logs", Rules));
            ChatCommands.Add(new Command("user", "manageusers", ManageUsers));
            ChatCommands.Add(new Command("login", "", AttemptLogin));
            ChatCommands.Add(new Command("broadcast", "cfg", Broadcast));
            ChatCommands.Add(new Command("bc", "cfg", Broadcast));
            if (ConfigurationManager.DistributationAgent != "terraria-online")
            {
                ChatCommands.Add(new Command("kill", "kill", Kill));
                ChatCommands.Add(new Command("butcher", "butcher", Butcher));
                ChatCommands.Add(new Command("i", "cheat", Item));
                ChatCommands.Add(new Command("item", "cheat", Item));
                ChatCommands.Add(new Command("give", "cheat", Give));
                ChatCommands.Add(new Command("heal", "cheat", Heal));
            }
        }

        public static void AddUpdateCommand()
        {
            //Commands.ChatCommands.Add(new Command("updatenow", "maintenance", Commands.UpdateNow));
        }

        public static bool HandleCommand(TSPlayer player, string text)
        {
            string cmdText = text.Remove(0, 1);

            var args = Commands.ParseParameters(cmdText);
            if (args.Count < 1)
                return false;

            string cmdName = args[0];
            args.RemoveAt(0);

            Command cmd = null;
            foreach (Command command in Commands.ChatCommands)
            {
                if (command.Name.Equals(cmdName))
                {
                    cmd = command;
                }
            }

            if (cmd == null)
            {
                return false;
            }

            if (!cmd.CanRun(player))
            {
                Tools.SendLogs(string.Format("{0} tried to execute {1}", player.Name, cmd.Name), Color.Red);
                player.SendMessage("You do not have access to that command.", Color.Red);
            }
            else
            {
                if (!cmdText.Contains("login") || !cmdText.Contains("user"))
                {
                    Tools.SendLogs(string.Format("{0} executed: /{1}", player.Name, cmdText), Color.Red);
                }
                cmd.Run(cmdText, player, args);
            }
            return true;
        }

        /// <summary>
        /// Parses a string of parameters into a list. Handles quotes.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static List<String> ParseParameters(string str)
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

        private static char GetEscape(char c)
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

        private static bool IsWhiteSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\n';
        }

        #region Account commands

        public static void AttemptLogin(CommandArgs args)
        {

            if (args.Player.LoginAttempts > ConfigurationManager.MaximumLoginAttempts && (ConfigurationManager.MaximumLoginAttempts != -1))
            {
                Log.Warn(args.Player.IP + "(" + args.Player.Name + ") had " + ConfigurationManager.MaximumLoginAttempts + " or more invalid login attempts and was kicked automatically.");
                Tools.Kick(args.Player, "Too many invalid login attempts.");
            }

            if (args.Parameters.Count != 2)
            {
                args.Player.SendMessage("Syntax: /login [username] [password]");
                args.Player.SendMessage("If you forgot your password, there is no way to recover it.");
                return;
            }

            string encrPass = Tools.HashPassword(args.Parameters[1]);
            string[] exr = Tools.FetchHashedPasswordAndGroup(args.Parameters[0]);
            if (exr[0] == encrPass)
            {
                args.Player.Group = Tools.GetGroup(exr[1]);
                args.Player.SendMessage("Authenticated as " + args.Parameters[0] + " successfully.", Color.LimeGreen);
                return;
            } else
            {
                Log.Warn(args.Player.IP + " failed to authenticate as " + args.Parameters[0]);
                args.Player.LoginAttempts++;
                return;
            }
        }

        private static void ManageUsers(CommandArgs args)
        {
            if (args.Parameters.Count < 3)
            {
                args.Player.SendMessage("Syntax: /user add <ip/user:pass> [group]");
                args.Player.SendMessage("Note: Passwords are stored with SHA512 hashing. To reset a user's password, remove and re-add them.");
                return;
            }

            if (args.Parameters.Count > 2)
            {
                if (args.Parameters[0] == "add")
                {
                    if (args.Parameters[1].Split(':').Length == 2)
                    {
                        TextWriter tw = new StreamWriter(FileTools.UsersPath, true);
                        tw.WriteLine("\n" + args.Parameters[1].Split(':')[0] + ":" + Tools.HashPassword(args.Parameters[1].Split(':')[1]) + " " + args.Parameters[2]);
                        tw.Close();
                        args.Player.SendMessage("This player can now login!", Color.Green);
                        return;
                    }
                    else if (args.Parameters[1].Split(':').Length == 1)
                    {
                        TextWriter tw = new StreamWriter(FileTools.UsersPath, true);
                        tw.WriteLine("\n" + args.Parameters[1] + " " + args.Parameters[2]);
                        tw.Close();
                        args.Player.SendMessage("IP address admin added. If they're logged in, tell them to rejoin.", Color.Green);
                        args.Player.SendMessage("WARNING: This is insecure! It would be better to use a user account instead.", Color.Red);
                        return;
                    }
                    else
                    {
                        args.Player.SendMessage("Invalid syntax. Try /user help.", Color.Red);
                    }
                }
            }
        }
        #endregion

        #region Player Management Commands

        private static void Kick(CommandArgs args)
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
            var players = Tools.FindPlayer(plStr);
            if (players.Count == 0)
            {
                args.Player.SendMessage("Invalid player!", Color.Red);
            }
            else if (players.Count > 1)
            {
                args.Player.SendMessage("More than one player matched!", Color.Red);
            }
            else
            {
                string reason = args.Parameters.Count > 1 ? String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1)) : "Misbehaviour.";
                if (!Tools.Kick(players[0], reason))
                {
                    args.Player.SendMessage("You can't kick another admin!", Color.Red);
                }
            }
        }

        private static void Ban(CommandArgs args)
        {
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
            var players = Tools.FindPlayer(plStr);
            if (players.Count == 0)
            {
                args.Player.SendMessage("Invalid player!", Color.Red);
            }
            else if (players.Count > 1)
            {
                args.Player.SendMessage("More than one player matched!", Color.Red);
            }
            else
            {
                string reason = args.Parameters.Count > 1 ? String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1)) : "Misbehaviour.";
                if (!Tools.Ban(players[0], reason))
                {
                    args.Player.SendMessage("You can't ban another admin!", Color.Red);
                }
            }
        }

        private static void BanIP(CommandArgs args)
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

        private static void UnBan(CommandArgs args)
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

        private static void UnBanIP(CommandArgs args)
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

        public static void Whitelist(CommandArgs args)
        {
            if (args.Parameters.Count == 1)
            {
                TextWriter tw = new StreamWriter(FileTools.WhitelistPath, true);
                tw.WriteLine(args.Parameters[0]);
                tw.Close();
                args.Player.SendMessage("Added " + args.Parameters[0] + " to the whitelist.");
            }
        }

        public static void DisplayLogs(CommandArgs args)
        {
            args.Player.DisplayLogs = (!args.Player.DisplayLogs);
            args.Player.SendMessage("You now " + (args.Player.DisplayLogs ? "receive" : "stopped receiving") + " logs");
        }

        #endregion Player Management Commands

        #region Server Maintenence Commands

        private static void Broadcast(CommandArgs args)
        {
            string message = "";

            for (int i = 0; i < args.Parameters.Count; i++)
            {
                message += args.Parameters[i];
            }

            Tools.Broadcast("(Server Broadcast) " + message, Color.Red);
            return;
        }

        private static void Off(CommandArgs args)
        {
            Tools.ForceKickAll("Server shutting down!");
            WorldGen.saveWorld();
            Netplay.disconnect = true;
        }

        private static void OffNoSave(CommandArgs args)
        {
            Tools.ForceKickAll("Server shutting down!");
            Netplay.disconnect = true;
        }

        private static void CheckUpdates(CommandArgs args)
        {
            ThreadPool.QueueUserWorkItem(UpdateManager.CheckUpdate);
        }

        private static void UpdateNow(CommandArgs args)
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

        #endregion Server Maintenence Commands

        #region Cause Events and Spawn Monsters Commands

        private static void DropMeteor(CommandArgs args)
        {
            WorldGen.spawnMeteor = false;
            WorldGen.dropMeteor();
        }

        private static void Star(CommandArgs args)
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

        private static void Bloodmoon(CommandArgs args)
        {
            TSPlayer.Server.SetBloodMoon(true);
            Tools.Broadcast(string.Format("{0} turned on blood moon.", args.Player.Name));
        }

        private static void Invade(CommandArgs args)
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

        private static void Eater(CommandArgs args)
        {
            if (args.Parameters.Count > 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /eater [amount]", Color.Red);
                return;
            }
            int amount = 1;
            if (args.Parameters.Count == 1 && !int.TryParse(args.Parameters[0], out amount))
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /eater [amount]", Color.Red);
                return;
            }
            NPC eater = Tools.GetNPCById(13);
            TSPlayer.Server.SpawnNPC(eater.type, eater.name, amount, (int)args.Player.TileX, (int)args.Player.TileY);
            Tools.Broadcast(string.Format("{0} has spawned eater of worlds {1} times!", args.Player.Name, amount));
        }

        private static void Eye(CommandArgs args)
        {
            if (args.Parameters.Count > 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /eye [amount]", Color.Red);
                return;
            }
            int amount = 1;
            if (args.Parameters.Count == 1 && !int.TryParse(args.Parameters[0], out amount))
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /eye [amount]", Color.Red);
                return;
            }
            NPC eye = Tools.GetNPCById(4);
            TSPlayer.Server.SetTime(false, 0.0);
            TSPlayer.Server.SpawnNPC(eye.type, eye.name, amount, (int)args.Player.TileX, (int)args.Player.TileY);
            Tools.Broadcast(string.Format("{0} has spawned eye {1} times!", args.Player.Name, amount));
        }

        private static void King(CommandArgs args)
        {
            if (args.Parameters.Count > 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /king [amount]", Color.Red);
                return;
            }
            int amount = 1;
            if (args.Parameters.Count == 1 && !int.TryParse(args.Parameters[0], out amount))
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /king [amount]", Color.Red);
                return;
            }
            NPC king = Tools.GetNPCById(50);
            TSPlayer.Server.SpawnNPC(king.type, king.name, amount, (int)args.Player.TileX, (int)args.Player.TileY);
            Tools.Broadcast(string.Format("{0} has spawned king slime {1} times!", args.Player.Name, amount));
        }

        private static void Skeletron(CommandArgs args)
        {
            if (args.Parameters.Count > 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /skeletron [amount]", Color.Red);
                return;
            }
            int amount = 1;
            if (args.Parameters.Count == 1 && !int.TryParse(args.Parameters[0], out amount))
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /skeletron [amount]", Color.Red);
                return;
            }
            NPC skeletron = Tools.GetNPCById(35);
            TSPlayer.Server.SetTime(false, 0.0);
            TSPlayer.Server.SpawnNPC(skeletron.type, skeletron.name, amount, (int)args.Player.TileX, (int)args.Player.TileY);
            Tools.Broadcast(string.Format("{0} has spawned skeletron {1} times!", args.Player.Name, amount));
        }

        private static void Hardcore(CommandArgs args)
        {
            if (args.Parameters.Count > 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /hardcore [amount]", Color.Red);
                return;
            }
            int amount = 1;
            if (args.Parameters.Count == 1 && !int.TryParse(args.Parameters[0], out amount))
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /hardcore [amount]", Color.Red);
                return;
            }
            NPC eater = Tools.GetNPCById(13);
            NPC eye = Tools.GetNPCById(4);
            NPC king = Tools.GetNPCById(50);
            NPC skeletron = Tools.GetNPCById(35);
            TSPlayer.Server.SetTime(false, 0.0);
            TSPlayer.Server.SpawnNPC(eater.type, eater.name, amount, (int)args.Player.TileX, (int)args.Player.TileY);
            TSPlayer.Server.SpawnNPC(eye.type, eye.name, amount, (int)args.Player.TileX, (int)args.Player.TileY);
            TSPlayer.Server.SpawnNPC(king.type, king.name, amount, (int)args.Player.TileX, (int)args.Player.TileY);
            TSPlayer.Server.SpawnNPC(skeletron.type, skeletron.name, amount, (int)args.Player.TileX, (int)args.Player.TileY);
            Tools.Broadcast(string.Format("{0} has spawned all bosses {1} times!", args.Player.Name, amount));
        }

        private static void SpawnMob(CommandArgs args)
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
            int amount = 1;
            if (args.Parameters.Count == 2 && !int.TryParse(args.Parameters[1], out amount))
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /spawnmob <mob name/id> [amount]", Color.Red);
                return;
            }

            var npcs = Tools.GetNPCByIdOrName(args.Parameters[0]);
            if (npcs.Count == 0)
            {
                args.Player.SendMessage("Invalid mob type!", Color.Red);
            }
            else if (npcs.Count > 1)
            {
                args.Player.SendMessage(string.Format("More than one ({0}) mob matched!", npcs.Count), Color.Red);
            }
            else
            {
                var npc = npcs[0];
                if (npc.type >= 1 && npc.type < Main.maxNPCTypes)
                {
                    TSPlayer.Server.SpawnNPC(npc.type, npc.name, amount, (int)args.Player.TileX, (int)args.Player.TileY, 50, 20);
                    Tools.Broadcast(string.Format("{0} was spawned {1} time(s).", npc.name, amount));
                }
                else
                    args.Player.SendMessage("Invalid mob type!", Color.Red);
            }
        }

        #endregion Cause Events and Spawn Monsters Commands

        #region Teleport Commands

        private static void Home(CommandArgs args)
        {
            if (!args.Player.RealPlayer)
            {
                args.Player.SendMessage("You cannot use teleport commands!");
                return;
            }

            args.Player.Spawn();
            args.Player.SendMessage("Teleported to your spawnpoint.");
        }

        private static void Spawn(CommandArgs args)
        {
            if (!args.Player.RealPlayer)
            {
                args.Player.SendMessage("You cannot use teleport commands!");
                return;
            }

            if (args.Player.Teleport(Main.spawnTileX, Main.spawnTileY))
                args.Player.SendMessage("Teleported to the map's spawnpoint.");
        }

        private static void TP(CommandArgs args)
        {
            if (!args.Player.RealPlayer)
            {
                args.Player.SendMessage("You cannot use teleport commands!");
                return;
            }

            if (args.Parameters.Count < 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /tp <player> ", Color.Red);
                return;
            }

            string plStr = String.Join(" ", args.Parameters);
            var players = Tools.FindPlayer(plStr);
            if (players.Count == 0)
                args.Player.SendMessage("Invalid player!", Color.Red);
            else if (players.Count > 1)
                args.Player.SendMessage("More than one player matched!", Color.Red);
            else
            {
                var plr = players[0];
                if (args.Player.Teleport(plr.TileX, plr.TileY + 3))
                    args.Player.SendMessage(string.Format("Teleported to {0}", plr.Name));
            }
        }

        private static void TPHere(CommandArgs args)
        {
            if (!args.Player.RealPlayer)
            {
                args.Player.SendMessage("You cannot use teleport commands!");
                return;
            }

            if (args.Parameters.Count < 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /tphere <player> ", Color.Red);
                return;
            }

            string plStr = String.Join(" ", args.Parameters);

            if (plStr == "all" || plStr == "*")
            {
                args.Player.SendMessage(string.Format("You brought all players here."));
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (Main.player[i].active && (Main.player[i] != args.TPlayer))
                    {
                        TShock.Players[i].Teleport(args.Player.TileX, args.Player.TileY + 3);
                        TShock.Players[i].SendMessage(string.Format("You were teleported to {0}.", args.Player.Name));
                    }
                }
                return;
            }

            var players = Tools.FindPlayer(plStr);
            if (players.Count == 0)
            {
                args.Player.SendMessage("Invalid player!", Color.Red);
            }
            else if (players.Count > 1)
            {
                args.Player.SendMessage("More than one player matched!", Color.Red);
            }
            else
            {
                var plr = players[0];
                if (plr.Teleport(args.Player.TileX, args.Player.TileY + 3))
                {
                    plr.SendMessage(string.Format("You were teleported to {0}.", plr.Name));
                    args.Player.SendMessage(string.Format("You brought {0} here.", plr.Name));
                }

            }
        }

        private static void SetWarp(CommandArgs args)
        {
            if (args.Parameters.Count > 0)
            {
                string warpName = String.Join(" ", args.Parameters);
                if (warpName.Equals("list"))
                {
                    args.Player.SendMessage("Name reserved, use a different name", Color.Red);
                }
                else if (WarpsManager.AddWarp(args.Player.TileX, args.Player.TileY, warpName, Main.worldName))
                {
                    args.Player.SendMessage("Set warp " + warpName, Color.Yellow);
                    WarpsManager.WriteSettings();
                }
                else
                {
                    args.Player.SendMessage("Warp " + warpName + " already exists", Color.Red);
                }
            }
            else
                args.Player.SendMessage("Invalid syntax! Proper syntax: /setwarp [name]", Color.Red);
        }

        private static void DeleteWarp(CommandArgs args)
        {
            if (args.Parameters.Count > 0)
            {
                string warpName = String.Join(" ", args.Parameters);
                if (WarpsManager.DeleteWarp(warpName))
                    args.Player.SendMessage("Deleted warp " + warpName, Color.Yellow);
                else
                    args.Player.SendMessage("Could not find specified warp", Color.Red);
            }
            else
                args.Player.SendMessage("Invalid syntax! Proper syntax: /delwarp [name]", Color.Red);
        }

        private static void UseWarp(CommandArgs args)
        {
            if (args.Parameters.Count > 0)
            {
                if (args.Parameters[0].Equals("list"))
                {
                    args.Player.SendMessage("Current Warps:", Color.Green); 
                    int page = 1;
                    if (args.Parameters.Count > 1)
                        int.TryParse(args.Parameters[1], out page);
                    var sb = new StringBuilder();
                    if (WarpsManager.Warps.Count > (15 * (page - 1)))
                    {
                        for (int j = (15 * (page - 1)); j < (15 * page); j++)
                        {
                            if (WarpsManager.Warps[j].WorldWarpName == Main.worldName)
                            {
                                if (sb.Length != 0)
                                    sb.Append(", ");
                                sb.Append("/").Append(WarpsManager.Warps[j].WarpName);
                                if (j == WarpsManager.Warps.Count - 1)
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
                    }
                    if (WarpsManager.Warps.Count > (15 * page))
                    {
                        args.Player.SendMessage(string.Format("Type /warp list {0} for more warps.", (page + 1)), Color.Yellow);
                    }
                }
                else
                {
                    string warpName = String.Join(" ", args.Parameters);
                    var warp = WarpsManager.FindWarp(warpName);
                    if (warp != Vector2.Zero)
                    {
                        if (args.Player.Teleport((int)warp.X, (int)warp.Y + 3))
                            args.Player.SendMessage("Warped to " + warpName, Color.Yellow);
                    }
                    else
                    {
                        args.Player.SendMessage("Specified warp not found", Color.Red);
                    }
                }
            }
            else
                args.Player.SendMessage("Invalid syntax! Proper syntax: /warp [name] or warp list", Color.Red);
        }

        #endregion Teleport Commands

        #region Server Config Commands

        private static void DebugConfiguration(CommandArgs args)
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

        private static void Reload(CommandArgs args)
        {
            FileTools.SetupConfig();
            args.Player.SendMessage("Configuration reload complete. Some changes may require server restart.");
        }

        private static void Password(CommandArgs args)
        {
            if (args.Parameters.Count != 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /password \"<new password>\"", Color.Red);
                return;
            }
            string passwd = args.Parameters[0];
            Netplay.password = passwd;
            args.Player.SendMessage(string.Format("Server password changed to: {0}", passwd));
        }

        private static void Save(CommandArgs args)
        {
            Tools.Broadcast("Server map saving, potential lag spike");
            WorldGen.saveWorld();
            args.Player.SendMessage("World saved.");
        }

        private static void MaxSpawns(CommandArgs args)
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

        private static void SpawnRate(CommandArgs args)
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

        #endregion Server Config Commands

        #region Time/PvpFun Commands

        private static void Time(CommandArgs args)
        {
            if (args.Parameters.Count != 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /time <day/night/dusk/noon/midnight>", Color.Red);
                return;
            }

            switch (args.Parameters[0])
            {
                case "day":
                    TSPlayer.Server.SetTime(true, 0.0);
                    Tools.Broadcast(string.Format("{0} set time to day.", args.Player.Name));
                    break;
                case "night":
                    TSPlayer.Server.SetTime(false, 0.0);
                    Tools.Broadcast(string.Format("{0} set time to night.", args.Player.Name));
                    break;
                case "dusk":
                    TSPlayer.Server.SetTime(false, 0.0);
                    Tools.Broadcast(string.Format("{0} set time to dusk.", args.Player.Name));
                    break;
                case "noon":
                    TSPlayer.Server.SetTime(true, 27000.0);
                    Tools.Broadcast(string.Format("{0} set time to noon.", args.Player.Name));
                    break;
                case "midnight":
                    TSPlayer.Server.SetTime(false, 16200.0);
                    Tools.Broadcast(string.Format("{0} set time to midnight.", args.Player.Name));
                    break;
                default:
                    args.Player.SendMessage("Invalid syntax! Proper syntax: /time <day/night/dusk/noon/midnight>", Color.Red);
                    break;
            }
        }

        private static void Slap(CommandArgs args)
        {
            if (args.Parameters.Count < 1 || args.Parameters.Count > 2)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /slap <player> [dmg]", Color.Red);
                return;
            }
            if (args.Parameters[0].Length == 0)
            {
                args.Player.SendMessage("Missing player name", Color.Red);
                return;
            }

            string plStr = args.Parameters[0];
            var players = Tools.FindPlayer(plStr);
            if (players.Count == 0)
            {
                args.Player.SendMessage("Invalid player!", Color.Red);
            }
            else if (players.Count > 1)
            {
                args.Player.SendMessage("More than one player matched!", Color.Red);
            }
            else
            {
                var plr = players[0];
                int damage = 5;
                if (args.Parameters.Count == 2)
                {
                    int.TryParse(args.Parameters[1], out damage);
                }
                if (!args.Player.Group.HasPermission("kill"))
                {
                    damage = Tools.Clamp(damage, 15, 0);
                }
                plr.DamagePlayer(damage);
                Tools.Broadcast(string.Format("{0} slapped {1} for {2} damage.",
                                args.Player.Name, plr.Name, damage));
                Log.Info(args.Player.Name + " slapped {0} with {1} damage.", plr.Name, damage);
            }
        }

        #endregion Time/PvpFun Commands

        #region World Protection Commands

        private static void ToggleAntiBuild(CommandArgs args)
        {
            ConfigurationManager.DisableBuild = (ConfigurationManager.DisableBuild == false);
            Tools.Broadcast(string.Format("Anti-build is now {0}.", (ConfigurationManager.DisableBuild ? "on" : "off")));
        }

        private static void ProtectSpawn(CommandArgs args)
        {
            ConfigurationManager.SpawnProtect = (ConfigurationManager.SpawnProtect == false);
            Tools.Broadcast(string.Format("Spawn is now {0}.", (ConfigurationManager.SpawnProtect ? "protected" : "open")));
        }

        private static void Region(CommandArgs args)
        {
            string cmd = "help";
            if (args.Parameters.Count > 0)
            {
                cmd = args.Parameters[0].ToLower();
            }
            switch (cmd)
            {
                case "set":
                    {
                        if (args.Parameters.Count == 2)
                        {
                            if (args.Parameters[1] == "1")
                            {
                                args.Player.TempArea.X = args.Player.TileX;
                                args.Player.TempArea.Y = args.Player.TileY;
                                args.Player.SendMessage("Set Temp Point 1", Color.Yellow);
                            }
                            else if (args.Parameters[1] == "2")
                            {
                                if (args.Player.TempArea.X != 0)
                                {
                                    if (args.Player.TileX > args.Player.TempArea.X && args.Player.TileY > args.Player.TempArea.Y)
                                    {
                                        args.Player.TempArea.Width = args.Player.TileX - args.Player.TempArea.X;
                                        args.Player.TempArea.Height = (args.Player.TileY + 3) - args.Player.TempArea.Y;
                                        args.Player.SendMessage("Set Temp Point 2", Color.Yellow);
                                    }
                                    else
                                    {
                                        args.Player.SendMessage("Point 2 must be below and right of Point 1", Color.Yellow);
                                        args.Player.SendMessage("Use /region clear to start again", Color.Yellow);
                                    }
                                }
                                else
                                {
                                    args.Player.SendMessage("You have not set Point 1 yet", Color.Red);
                                }
                            }
                            else
                                args.Player.SendMessage("Invalid syntax! Proper syntax: /region set [1/2]", Color.Red);
                        }
                        else
                            args.Player.SendMessage("Invalid syntax! Proper syntax: /region set [1/2]", Color.Red);
                        break;
                    }
                case "define":
                    {
                        if (args.Parameters.Count > 1)
                        {
                            if (!args.Player.TempArea.IsEmpty)
                            {
                                string regionName = String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
                                if (RegionManager.AddRegion(args.Player.TempArea.X, args.Player.TempArea.Y, 
                                                            args.Player.TempArea.Width, args.Player.TempArea.Height, 
                                                            regionName, Main.worldName))
                                {
                                    args.Player.TempArea = Rectangle.Empty;
                                    args.Player.SendMessage("Set region " + regionName, Color.Yellow);
                                }
                                else
                                {
                                    args.Player.SendMessage("Region " + regionName + " already exists", Color.Red);
                                }
                            }
                            else
                                args.Player.SendMessage("Points not set up yet", Color.Red);
                        }
                        else
                            args.Player.SendMessage("Invalid syntax! Proper syntax: /region define [name]", Color.Red);
                        break;
                    }
                case "protect":
                    {
                        if (args.Parameters.Count == 3)
                        {
                            string regionName = args.Parameters[1];
                            if (args.Parameters[2].ToLower() == "true")
                            {
                                if (RegionManager.SetRegionState(regionName, true))
                                    args.Player.SendMessage("Protected region " + regionName, Color.Yellow);
                                else
                                    args.Player.SendMessage("Could not find specified region", Color.Red);
                            }
                            else if (args.Parameters[2].ToLower() == "false")
                            {
                                if (RegionManager.SetRegionState(regionName, false))
                                    args.Player.SendMessage("Unprotected region " + regionName, Color.Yellow);
                                else
                                    args.Player.SendMessage("Could not find specified region", Color.Red);
                            }
                            else
                                args.Player.SendMessage("Invalid syntax! Proper syntax: /region protected [name] [true/false]", Color.Red);
                        }
                        else
                            args.Player.SendMessage("Invalid syntax! Proper syntax: /region protected [name] [true/false]", Color.Red);
                        break;
                    }
                case "delete":
                    {
                        if (args.Parameters.Count > 1)
                        {
                            string regionName = String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
                            if (RegionManager.DeleteRegion(regionName))
                                args.Player.SendMessage("Deleted region " + regionName, Color.Yellow);
                            else
                                args.Player.SendMessage("Could not find specified region", Color.Red);
                        }
                        else
                            args.Player.SendMessage("Invalid syntax! Proper syntax: /region delete [name]", Color.Red);
                        break;
                    }
                case "clear":
                    {
                        args.Player.TempArea = Rectangle.Empty;
                        args.Player.SendMessage("Cleared temp area", Color.Yellow);
                        break;
                    }
                case "allow":
                    {
                        if (args.Parameters.Count > 2)
                        {
                            string playerName = args.Parameters[1];
                            string regionName = "";
                            string playerIP = null;

                            for (int i = 2; i < args.Parameters.Count; i++)
                            {
                                if (regionName == "")
                                {
                                    regionName = args.Parameters[2];
                                }
                                else
                                {
                                    regionName = regionName + " " + args.Parameters[i];
                                }
                            }
                            if ((playerIP = Tools.GetPlayerIP(playerName)) != null)
                            {
                                if (RegionManager.AddNewUser(regionName, playerIP))
                                {
                                    args.Player.SendMessage("Added user " + playerName + " to " + regionName, Color.Yellow);
                                    RegionManager.WriteSettings();
                                }
                                else
                                    args.Player.SendMessage("Region " + regionName + " not found", Color.Red);
                            }
                            else
                            {
                                args.Player.SendMessage("Player " + playerName + " not found", Color.Red);
                            }
                        }
                        else
                            args.Player.SendMessage("Invalid syntax! Proper syntax: /region allow [name] [region]", Color.Red);
                        break;
                    }
                case "help":
                default:
                    {
                        args.Player.SendMessage("Avialable region commands:", Color.Green);
                        args.Player.SendMessage("/region set [1/2] /region define [name] /region protect [name] [true/false]", Color.Yellow);
                        args.Player.SendMessage("/region delete [name] /region clear (temporary region)", Color.Yellow);
                        args.Player.SendMessage("/region allow [name] [regionname]", Color.Yellow);
                        break;
                    }
            }

        }

        #endregion World Protection Commands

        #region General Commands

        private static void Help(CommandArgs args)
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

        private static void Playing(CommandArgs args)
        {
            args.Player.SendMessage(string.Format("Current players: {0}.", Tools.GetPlayers()), 255, 240, 20);
        }

        private static void AuthToken(CommandArgs args)
        {
            if (ConfigurationManager.AuthToken == 0)
            {
                return;
            }
            int givenCode = Convert.ToInt32(args.Parameters[0]);
            if (givenCode == ConfigurationManager.AuthToken)
            {
                TextWriter tw = new StreamWriter(FileTools.UsersPath, true);
                tw.Write("\n" + args.Player.IP + " superadmin");
                args.Player.SendMessage("SuperAdmin authenticated. Please re-connect using the same IP.");
                ConfigurationManager.AuthToken = 0;
                tw.Close();
            }
        }

        private static void ThirdPerson(CommandArgs args)
        {
            if (args.Parameters.Count == 0)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /me <text>", Color.Red);
                return;
            }
            Tools.Broadcast(string.Format("*{0} {1}", args.Player.Name, String.Join(" ", args.Parameters)), 205, 133, 63);
        }

        private static void PartyChat(CommandArgs args)
        {
            if (args.Parameters.Count == 0)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /p <team chat text>", Color.Red);
                return;
            }
            int playerTeam = args.Player.Team;
            if (playerTeam != 0)
            {
                string msg = string.Format("<{0}> {1}", args.Player.Name, String.Join(" ", args.Parameters));
                foreach (TSPlayer player in TShock.Players)
                {
                    if (player != null && player.Active && player.Team == playerTeam)
                        player.SendMessage(msg, Main.teamColor[playerTeam].R, Main.teamColor[playerTeam].G, Main.teamColor[playerTeam].B);
                }
            }
            else
            {
                args.Player.SendMessage("You are not in a party!", 255, 240, 20);
            }
        }

        private static void Rules(CommandArgs args)
        {
            Tools.ShowFileToUser(args.Player, "rules.txt");
        }

        #endregion General Commands

        #region Cheat Commands

        private static void Kill(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /kill <player>", Color.Red);
                return;
            }

            string plStr = String.Join(" ", args.Parameters);
            var players = Tools.FindPlayer(plStr);
            if (players.Count == 0)
            {
                args.Player.SendMessage("Invalid player!", Color.Red);
            }
            else if (players.Count > 1)
            {
                args.Player.SendMessage("More than one player matched!", Color.Red);
            }
            else
            {
                var plr = players[0];
                plr.DamagePlayer(999999);
                args.Player.SendMessage(string.Format("You just killed {0}!", plr.Name));
                plr.SendMessage(string.Format("{0} just killed you!", args.Player.Name));
            }
        }

        private static void Butcher(CommandArgs args)
        {
            if (args.Parameters.Count > 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /butcher [killFriendly(true/false)]", Color.Red);
                return;
            }

            bool killFriendly = true;
            if (args.Parameters.Count == 1)
                bool.TryParse(args.Parameters[0], out killFriendly);

            int killcount = 0;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if ( Main.npc[i].active && !Main.npc[i].townNPC && (!Main.npc[i].friendly || killFriendly))
                {
                    TSPlayer.Server.StrikeNPC(i, 99999, 90f, 1);
                    killcount++;
                }
            }
            Tools.Broadcast(string.Format("Killed {0} NPCs.", killcount));
        }

        private static void Item(CommandArgs args)
        {
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

            var items = Tools.GetItemByIdOrName(String.Join(" ", args.Parameters));
            if (items.Count == 0)
            {
                args.Player.SendMessage("Invalid item type!", Color.Red);
            }
            else if (items.Count > 1)
            {
                args.Player.SendMessage(string.Format("More than one ({0}) item matched!", items.Count), Color.Red);
            }
            else
            {
                var item = items[0];
                if (item.type >= 1 && item.type < Main.maxItemTypes)
                {
                    if (args.Player.InventorySlotAvailable)
                    {
                        args.Player.GiveItem(item.type, item.name, item.width, item.height, item.maxStack);
                        args.Player.SendMessage(string.Format("Got some {0}.", item.name));
                    }
                    else
                    {
                        args.Player.SendMessage("You don't have free slots!", Color.Red);
                    }
                }
                else
                {
                    args.Player.SendMessage("Invalid item type!", Color.Red);
                }
            }
        }

        private static void Give(CommandArgs args)
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

            var items = Tools.GetItemByIdOrName(args.Parameters[0]);

            if (items.Count == 0)
            {
                args.Player.SendMessage("Invalid item type!", Color.Red);
            }
            else if (items.Count > 1)
            {
                args.Player.SendMessage(string.Format("More than one ({0}) item matched!", items.Count), Color.Red);
            }
            else
            {
                var item = items[0];
                if (item.type >= 1 && item.type < Main.maxItemTypes)
                {
                    string plStr = args.Parameters[1];
                    var players = Tools.FindPlayer(plStr);
                    if (players.Count == 0)
                    {
                        args.Player.SendMessage("Invalid player!", Color.Red);
                    }
                    else if (players.Count > 1)
                    {
                        args.Player.SendMessage("More than one player matched!", Color.Red);
                    }
                    else
                    {
                        var plr = players[0];
                        if (plr.InventorySlotAvailable)
                        {
                            plr.GiveItem(item.type, item.name, item.width, item.height, item.maxStack);
                            args.Player.SendMessage(string.Format("Gave {0} some {1}.", plr.Name, item.name));
                            plr.SendMessage(string.Format("{0} gave you some {1}.", args.Player.Name, item.name));
                        }
                        else
                        {
                            args.Player.SendMessage("Player does not have free slots!", Color.Red);
                        }
                    }
                }
                else
                {
                    args.Player.SendMessage("Invalid item type!", Color.Red);
                }
            }
        }

        private static void Heal(CommandArgs args)
        {
            TSPlayer playerToHeal;
            if (args.Parameters.Count > 0)
            {
                string plStr = String.Join(" ", args.Parameters);
                var players = Tools.FindPlayer(plStr);
                if (players.Count == 0)
                {
                    args.Player.SendMessage("Invalid player!", Color.Red);
                    return;
                }
                else if (players.Count > 1)
                {
                    args.Player.SendMessage("More than one player matched!", Color.Red);
                    return;
                }
                else
                {
                    playerToHeal = players[0];
                }
            }
            else if (!args.Player.RealPlayer)
            {
                args.Player.SendMessage("You cant heal yourself!");
                return;
            }
            else
            {
                playerToHeal = args.Player;
            }

            Item heart = Tools.GetItemById(58);
            Item star = Tools.GetItemById(184);
            for (int i = 0; i < 20; i++)
                playerToHeal.GiveItem(heart.type, heart.name, heart.width, heart.height, heart.maxStack);
            for (int i = 0; i < 10; i++)
                playerToHeal.GiveItem(star.type, star.name, star.width, star.height, star.maxStack);
            if (playerToHeal == args.Player)
            {
                args.Player.SendMessage("You just got healed!");
            }
            else
            {
                args.Player.SendMessage(string.Format("You just healed {0}", playerToHeal.Name));
                playerToHeal.SendMessage(string.Format("{0} just healed you!", args.Player.Name));
            }
        }

        #endregion Cheat Comamnds
    }
}
