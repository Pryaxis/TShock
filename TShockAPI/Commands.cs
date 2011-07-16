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
using System.Linq;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI.DB;

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
        public string Name { get { return Names[0]; } }
        public List<string> Names { get; protected set; }
        public bool DoLog { get; set; }
        private string permission;
        private CommandDelegate command;

        public Command(string permissionneeded, CommandDelegate cmd, params string[] names)
            : this(cmd, names)
        {
            permission = permissionneeded;
        }
        public Command(CommandDelegate cmd, params string[] names)
        {
            if (names == null || names.Length < 1)
                throw new NotSupportedException();
            permission = null;
            Names = new List<string>(names);
            command = cmd;
            DoLog = true;
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

        public bool HasAlias(string name)
        {
            return Names.Contains(name);
        }

        public bool CanRun(TSPlayer ply)
        {
            return ply.Group.HasPermission(permission);
        }
    }
    public static class Commands
    {
        public static List<Command> ChatCommands = new List<Command>();

        public static void InitCommands()
        {
            ChatCommands.Add(new Command("kick", Kick, "kick"));
            ChatCommands.Add(new Command("ban", Ban, "ban"));
            ChatCommands.Add(new Command("ban", BanIP, "banip"));
            ChatCommands.Add(new Command("unban", UnBan, "unban"));
            ChatCommands.Add(new Command("unban", UnBanIP, "unbanip"));
            ChatCommands.Add(new Command("maintenance", ClearBans, "clearbans"));
            ChatCommands.Add(new Command("whitelist", Whitelist, "whitelist"));
            ChatCommands.Add(new Command("maintenance", Off, "off"));
            ChatCommands.Add(new Command("maintenance", OffNoSave, "off-nosave"));
            ChatCommands.Add(new Command("maintenance", CheckUpdates, "checkupdates"));
            ChatCommands.Add(new Command("causeevents", DropMeteor, "dropmeteor"));
            ChatCommands.Add(new Command("causeevents", Star, "star"));
            ChatCommands.Add(new Command("causeevents", Bloodmoon, "bloodmoon"));
            ChatCommands.Add(new Command("causeevents", Invade, "invade"));
            ChatCommands.Add(new Command("spawnboss", Eater, "eater"));
            ChatCommands.Add(new Command("spawnboss", Eye, "eye"));
            ChatCommands.Add(new Command("spawnboss", King, "king"));
            ChatCommands.Add(new Command("spawnboss", Skeletron, "skeletron"));
            ChatCommands.Add(new Command("spawnboss", Hardcore, "hardcore"));
            ChatCommands.Add(new Command("spawnmob", SpawnMob, "spawnmob", "sm"));
            ChatCommands.Add(new Command("tp", Home, "home"));
            ChatCommands.Add(new Command("tp", Spawn, "spawn"));
            ChatCommands.Add(new Command("tp", TP, "tp"));
            ChatCommands.Add(new Command("tphere", TPHere, "tphere"));
            ChatCommands.Add(new Command("warp", UseWarp, "warp"));
            ChatCommands.Add(new Command("managewarp", SetWarp, "setwarp"));
            ChatCommands.Add(new Command("managewarp", DeleteWarp, "delwarp"));
            ChatCommands.Add(new Command("cfg", SetSpawn, "setspawn"));
            ChatCommands.Add(new Command("cfg", Reload, "reload"));
            ChatCommands.Add(new Command("cfg", DebugConfiguration, "debug-config"));
            ChatCommands.Add(new Command("cfg", Password, "password"));
            ChatCommands.Add(new Command("cfg", Save, "save"));
            ChatCommands.Add(new Command("cfg", MaxSpawns, "maxspawns"));
            ChatCommands.Add(new Command("cfg", SpawnRate, "spawnrate"));
            ChatCommands.Add(new Command("time", Time, "time"));
            ChatCommands.Add(new Command("pvpfun", Slap, "slap"));
            ChatCommands.Add(new Command("editspawn", ToggleAntiBuild, "antibuild"));
            ChatCommands.Add(new Command("editspawn", ProtectSpawn, "protectspawn"));
            ChatCommands.Add(new Command("editspawn", Region, "region"));
            ChatCommands.Add(new Command("editspawn", DebugRegions, "debugreg"));
            ChatCommands.Add(new Command(Help, "help"));
            ChatCommands.Add(new Command(Playing, "playing", "online", "who"));
            ChatCommands.Add(new Command(AuthToken, "auth"));
            ChatCommands.Add(new Command(ThirdPerson, "me"));
            ChatCommands.Add(new Command(PartyChat, "p"));
            ChatCommands.Add(new Command(Rules, "rules"));
            ChatCommands.Add(new Command("logs", DisplayLogs, "displaylogs"));
            ChatCommands.Add(new Command("root-only", ManageUsers, "user") { DoLog = false });
            ChatCommands.Add(new Command("root-only", GrabUserIP, "ip"));
            ChatCommands.Add(new Command("root-only", AuthVerify, "auth-verify"));
            ChatCommands.Add(new Command(AttemptLogin, "login") { DoLog = false });
            ChatCommands.Add(new Command("cfg", Broadcast, "broadcast", "bc"));
            ChatCommands.Add(new Command("whisper", Whisper, "whisper", "w", "tell"));
            ChatCommands.Add(new Command("whisper", Reply, "reply", "r"));
            ChatCommands.Add(new Command("annoy", Annoy, "annoy"));
            if (TShock.Config.DistributationAgent != "terraria-online")
            {
                ChatCommands.Add(new Command("kill", Kill, "kill"));
                ChatCommands.Add(new Command("butcher", Butcher, "butcher"));
                ChatCommands.Add(new Command("item", Item, "item", "i"));
                ChatCommands.Add(new Command("item", Give, "give"));
                ChatCommands.Add(new Command("heal", Heal, "heal"));
            }
        }

        public static bool HandleCommand(TSPlayer player, string text)
        {
            string cmdText = text.Remove(0, 1);

            var args = ParseParameters(cmdText);
            if (args.Count < 1)
                return false;

            string cmdName = args[0];
            args.RemoveAt(0);

            Command cmd = ChatCommands.FirstOrDefault(c => c.HasAlias(cmdName));

            if (cmd == null)
                return false;

            if (!cmd.CanRun(player))
            {
                Tools.SendLogs(string.Format("{0} tried to execute {1}", player.Name, cmd.Name), Color.Red);
                player.SendMessage("You do not have access to that command.", Color.Red);
            }
            else
            {
                if (cmd.DoLog)
                    Tools.SendLogs(string.Format("{0} executed: /{1}", player.Name, cmdText), Color.Red);
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

            if (args.Player.LoginAttempts > TShock.Config.MaximumLoginAttempts && (TShock.Config.MaximumLoginAttempts != -1))
            {
                Log.Warn(args.Player.IP + "(" + args.Player.Name + ") had " + TShock.Config.MaximumLoginAttempts + " or more invalid login attempts and was kicked automatically.");
                Tools.Kick(args.Player, "Too many invalid login attempts.");
            }

            if (args.Parameters.Count != 2)
            {
                args.Player.SendMessage("Syntax: /login [username] [password]");
                args.Player.SendMessage("If you forgot your password, there is no way to recover it.");
                return;
            }
            try
            {
                string encrPass = Tools.HashPassword(args.Parameters[1]);
                string[] exr = TShock.Users.FetchHashedPasswordAndGroup(args.Parameters[0]);
                if (exr[0].ToUpper() == encrPass.ToUpper())
                {
                    args.Player.Group = Tools.GetGroup(exr[1]);
                    args.Player.UserAccountName = args.Parameters[0];
                    args.Player.IsLoggedIn = true;
                    args.Player.SendMessage("Authenticated as " + args.Parameters[0] + " successfully.", Color.LimeGreen);
                    Log.ConsoleInfo(args.Player.Name + " authenticated successfully as user: " + args.Parameters[0]);
                    return;
                }
                else
                {
                    Log.Warn(args.Player.IP + " failed to authenticate as user: " + args.Parameters[0]);
                    args.Player.LoginAttempts++;
                    return;
                }
            } catch (Exception e)
            {
                args.Player.SendMessage("There was an error processing your request. Maybe your account doesn't exist?", Color.Red);
                return;
            }

        }

        //Todo: Add separate help text for '/user add' and '/user del'. Also add '/user addip' and '/user delip'

        private static void ManageUsers(CommandArgs args)
        {
            if (args.Parameters.Count < 2)
            {
                args.Player.SendMessage("Syntax: /user <add/del> <ip/user:pass> [group]");
                args.Player.SendMessage("Note: Passwords are stored with SHA512 hashing. To reset a user's password, remove and re-add them.");
                return;
            }

            string subcmd = args.Parameters[0];

            if (subcmd == "add")
            {
                var namepass = args.Parameters[1].Split(':');
                var user = new User();

                try
                {
                    if (args.Parameters.Count > 2)
                    {
                        if (namepass.Length == 2)
                        {
                            user.Name = namepass[0];
                            user.Password = namepass[1];
                            user.Group = args.Parameters[2];
                        }
                        else if (namepass.Length == 1)
                        {
                            user.Address = namepass[0];
                            user.Group = args.Parameters[2];
                            user.Name = user.Address;
                        }
                        if (!string.IsNullOrEmpty(user.Address))
                        {
                            args.Player.SendMessage("IP address admin added. If they're logged in, tell them to rejoin.", Color.Green);
                            args.Player.SendMessage("WARNING: This is insecure! It would be better to use a user account instead.", Color.Red);
                            TShock.Users.AddUser(user);
                            Log.ConsoleInfo(args.Player.Name + " added IP " + user.Address + " to group " + user.Group);
                        }
                        else
                        {
                            args.Player.SendMessage("Account " + user.Name + " has been added to group " + user.Group + "!", Color.Green);
                            TShock.Users.AddUser(user);
                            Log.ConsoleInfo(args.Player.Name + " added Account " + user.Name + " to group " + user.Group);
                        }
                    }
                    else
                    {
                        args.Player.SendMessage("Invalid syntax. Try /user help.", Color.Red);
                    }
                }
                catch (UserManagerException ex)
                {
                    args.Player.SendMessage(ex.Message, Color.Green);
                    Log.ConsoleError(ex.ToString());
                }
            }
            else if (subcmd == "del" && args.Parameters.Count == 2)
            {
                var user = new User();
                if (args.Parameters[1].Contains("."))
                    user.Address = args.Parameters[1];
                else
                    user.Name = args.Parameters[1];

                try
                {
                    TShock.Users.RemoveUser(user);
                    args.Player.SendMessage("Account removed successfully.", Color.Green);
                    Log.ConsoleInfo(args.Player.Name + " successfully deleted account: " + args.Parameters[1]);
                }
                catch (UserManagerException ex)
                {
                    args.Player.SendMessage(ex.Message, Color.Red);
                    Log.ConsoleError(ex.ToString());
                }
            }
            else
            {
                args.Player.SendMessage("Invalid syntax. Try /user help.", Color.Red);
            }
        }
        #endregion

        #region Player Management Commands

        private static void GrabUserIP(CommandArgs args)
        {
            if (args.Parameters.Count < 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /ip <player>", Color.Red);
                return;
            }

            var players = Tools.FindPlayer(args.Parameters[0]);
            if (players.Count > 1)
            {
                args.Player.SendMessage("More than one player matched your query.", Color.Red);
                return;
            }
            try
            {
                args.Player.SendMessage(players[0].IP, Color.Green);
            }
            catch (Exception)
            {
                args.Player.SendMessage("Invalid player.", Color.Red);
            }
        }

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
                if (TShock.Bans.RemoveBan(ban.IP))
                    args.Player.SendMessage(string.Format("Unbanned {0} ({1})!", ban.Name, ban.IP), Color.Red);
                else
                    args.Player.SendMessage(string.Format("Failed to Unbanned {0} ({1})!", ban.Name, ban.IP), Color.Red);
            }
            else
            {
                args.Player.SendMessage("Invalid player!", Color.Red);
            }
        }

        static int ClearBansCode = -1;
        private static void ClearBans(CommandArgs args)
        {
            if (args.Parameters.Count < 1 && ClearBansCode == -1)
            {
                ClearBansCode = new Random().Next(0, short.MaxValue);
                args.Player.SendMessage("ClearBans Code: " + ClearBansCode, Color.Red);
                return;
            }
            if (args.Parameters.Count < 1)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /clearbans <code>");
                return;
            }

            int num;
            if (!int.TryParse(args.Parameters[0], out num))
            {
                args.Player.SendMessage("Invalid syntax! Expecting number");
                return;
            }

            if (num == ClearBansCode)
            {
                ClearBansCode = -1;
                if (TShock.Bans.ClearBans())
                {
                    Log.ConsoleInfo("Bans cleared");
                    args.Player.SendMessage("Bans cleared");
                }
                else
                {
                    args.Player.SendMessage("Failed to clear bans");
                }
            }
            else
            {
                args.Player.SendMessage("Incorrect clear code");
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
                if (TShock.Bans.RemoveBan(ban.IP))
                    args.Player.SendMessage(string.Format("Unbanned {0} ({1})!", ban.Name, ban.IP), Color.Red);
                else
                    args.Player.SendMessage(string.Format("Failed to Unbanned {0} ({1})!", ban.Name, ban.IP), Color.Red);
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
                message += " " + args.Parameters[i];
            }

            Tools.Broadcast("(Server Broadcast)" + message, Color.Red);
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
                        if (TShock.Players[i].Teleport(args.Player.TileX, args.Player.TileY + 3))
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
                    plr.SendMessage(string.Format("You were teleported to {0}.", args.Player.Name));
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
                else if (TShock.Warps.AddWarp(args.Player.TileX, args.Player.TileY, warpName, Main.worldID.ToString()))
                {
                    args.Player.SendMessage("Set warp " + warpName, Color.Yellow);
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
                if (TShock.Warps.RemoveWarp(warpName))
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
                    List<Warp> Warps = TShock.Warps.ListAllWarps();

                    if (Warps.Count > (15 * (page - 1)))
                    {
                        for (int j = (15 * (page - 1)); j < (15 * page); j++)
                        {
                            if (Warps[j].WorldWarpID == Main.worldID.ToString())
                            {
                                if (sb.Length != 0)
                                    sb.Append(", ");
                                sb.Append("/").Append(Warps[j].WarpName);
                                if (j == Warps.Count - 1)
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
                    if (Warps.Count > (15 * page))
                    {
                        args.Player.SendMessage(string.Format("Type /warp list {0} for more warps.", (page + 1)), Color.Yellow);
                    }
                }
                else
                {
                    string warpName = String.Join(" ", args.Parameters);
                    var warp = TShock.Warps.FindWarp(warpName);
                    if (warp.WarpPos != Vector2.Zero)
                    {
                        if (args.Player.Teleport((int)warp.WarpPos.X, (int)warp.WarpPos.Y + 3))
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

        private static void SetSpawn(CommandArgs args)
        {
            Main.spawnTileX = args.Player.TileX + 1;
            Main.spawnTileY = args.Player.TileY + 3;

            Tools.Broadcast("Server map saving, potential lag spike");
            Thread SaveWorld = new Thread(Tools.SaveWorld);
            SaveWorld.Start();
        }

        private static void DebugConfiguration(CommandArgs args)
        {
            args.Player.SendMessage("TShock Config:");
            string lineOne = string.Format("BanCheater : {0}, KickCheater : {1}, BanGriefer : {2}, KickGriefer : {3}",
                              TShock.Config.BanCheaters, TShock.Config.KickCheaters,
                              TShock.Config.BanGriefers, TShock.Config.KickGriefers);
            args.Player.SendMessage(lineOne, Color.Yellow);
            string lineTwo = string.Format("BanTnt : {0}, KickTnt : {1}, BanBoom : {2}, KickBoom : {3}",
                                           TShock.Config.BanKillTileAbusers, TShock.Config.KickKillTileAbusers,
                                           TShock.Config.BanExplosives, TShock.Config.KickExplosives);
            args.Player.SendMessage(lineTwo, Color.Yellow);
            string lineThree = string.Format("RangeChecks : {0}, DisableBuild : {1}, ProtectSpawn : {2}, ProtectRadius : {3}",
                                             TShock.Config.RangeChecks, TShock.Config.DisableBuild,
                                             TShock.Config.SpawnProtection, TShock.Config.SpawnProtectionRadius);
            args.Player.SendMessage(lineThree, Color.Yellow);
            string lineFour = string.Format("MaxSlots : {0}, SpamChecks : {1}, InvMultiplier : {2}, DMS : {3}, SpawnRate {4}",
                                           TShock.Config.MaxSlots, TShock.Config.SpamChecks,
                                           TShock.Config.InvasionMultiplier, TShock.Config.DefaultMaximumSpawns,
                                           TShock.Config.DefaultSpawnRate);
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
            Thread SaveWorld = new Thread(Tools.SaveWorld);
            SaveWorld.Start();
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
            TShock.Config.DefaultMaximumSpawns = amount;
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
            TShock.Config.DefaultSpawnRate = amount;
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
                Log.Info(args.Player.Name + " slapped " + plr.Name + " with " + damage + " damage.");
            }
        }

        #endregion Time/PvpFun Commands

        #region World Protection Commands

        private static void ToggleAntiBuild(CommandArgs args)
        {
            TShock.Config.DisableBuild = (TShock.Config.DisableBuild == false);
            Tools.Broadcast(string.Format("Anti-build is now {0}.", (TShock.Config.DisableBuild ? "on" : "off")));
        }

        private static void ProtectSpawn(CommandArgs args)
        {
            TShock.Config.SpawnProtection = (TShock.Config.SpawnProtection == false);
            Tools.Broadcast(string.Format("Spawn is now {0}.", (TShock.Config.SpawnProtection ? "protected" : "open")));
        }

        private static void DebugRegions(CommandArgs args)
        {
            foreach (Region r in TShock.Regions.Regions)
            {
                args.Player.SendMessage(r.RegionName + ": P: " + r.DisableBuild + " X: " + r.RegionArea.X + " Y: " + r.RegionArea.Y + " W: " + r.RegionArea.Width + " H: " + r.RegionArea.Height );
                foreach (string s in r.RegionAllowedIDs)
                {
                    args.Player.SendMessage(r.RegionName + ": " + s);
                }
            }
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
                                if (TShock.Regions.AddRegion(args.Player.TempArea.X, args.Player.TempArea.Y,
                                                            args.Player.TempArea.Width, args.Player.TempArea.Height,
                                                            regionName, Main.worldID.ToString()))
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
                                if (TShock.Regions.SetRegionState(regionName, true))
                                    args.Player.SendMessage("Protected region " + regionName, Color.Yellow);
                                else
                                    args.Player.SendMessage("Could not find specified region", Color.Red);
                            }
                            else if (args.Parameters[2].ToLower() == "false")
                            {
                                if (TShock.Regions.SetRegionState(regionName, false))
                                    args.Player.SendMessage("Unprotected region " + regionName, Color.Yellow);
                                else
                                    args.Player.SendMessage("Could not find specified region", Color.Red);
                            }
                            else
                                args.Player.SendMessage("Invalid syntax! Proper syntax: /region protect [name] [true/false]", Color.Red);
                        }
                        else
                            args.Player.SendMessage("Invalid syntax! Proper syntax: /region protect [name] [true/false]", Color.Red);
                        break;
                    }
                case "delete":
                    {
                        if (args.Parameters.Count > 1)
                        {
                            string regionName = String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
                            if (TShock.Regions.DeleteRegion(regionName))
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
                            User playerID;

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
                            if ((playerID = TShock.Users.GetUserByName(playerName)) != null)
                            {
                                if (TShock.Regions.AddNewUser(regionName, playerID))
                                {
                                    args.Player.SendMessage("Added user " + playerName + " to " + regionName, Color.Yellow);
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
                case "list":
                    {
                        args.Player.SendMessage("Current Regions:", Color.Green);
                        int page = 1;
                        if (args.Parameters.Count > 1)
                            int.TryParse(args.Parameters[1], out page);
                        var sb = new StringBuilder();

                        List<Region> Regions = TShock.Regions.ListAllRegions();

                        if (Regions.Count > (15 * (page - 1)))
                        {
                            for (int j = (15 * (page - 1)); j < (15 * page); j++)
                            {
                                if (Regions[j].RegionWorldID == Main.worldID.ToString())
                                {
                                    if (sb.Length != 0)
                                        sb.Append(", ");
                                    sb.Append(Regions[j].RegionName);
                                    if (j == Regions.Count - 1)
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
                        if (Regions.Count > (15 * page))
                        {
                            args.Player.SendMessage(string.Format("Type /warp list {0} for more warps.", (page + 1)), Color.Yellow);
                        }
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
            if (TShock.AuthToken == 0)
            {
                args.Player.SendMessage("Auth is disabled. This incident has been logged.", Color.Red);
                Log.Warn(args.Player.IP + " attempted to use /auth even though it's disabled.");
                return;
            }
            int givenCode = Convert.ToInt32(args.Parameters[0]);
            if (givenCode == TShock.AuthToken && args.Player.Group.Name != "superadmin")
            {
                try
                {
                    TShock.Users.AddUser(new User(args.Player.IP, "", "", "superadmin"));
                    args.Player.Group = Tools.GetGroup("superadmin");
                    args.Player.SendMessage("This IP address is now superadmin. Please perform the following command:");
                    args.Player.SendMessage("/user add <username>:<password> superadmin");
                    args.Player.SendMessage("Creates: <username> with the password <password> as part of the superadmin group.");
                    args.Player.SendMessage("Please use /login <username> <password> to login from now on.");
                    args.Player.SendMessage("If you understand, please /login <username> <password> now, and type /auth-verify");
                }
                catch (UserManagerException ex)
                {
                    Log.ConsoleError(ex.ToString());
                    args.Player.SendMessage(ex.Message);
                }
                return;
            }

            if (args.Player.Group.Name == "superadmin")
            {
                args.Player.SendMessage("Please disable the auth system! If you need help, consult the forums. http://tshock.co/");
                args.Player.SendMessage("This IP address is now superadmin. Please perform the following command:");
                args.Player.SendMessage("/user add <username>:<password> superadmin");
                args.Player.SendMessage("Creates: <username> with the password <password> as part of the superadmin group.");
                args.Player.SendMessage("Please use /login <username> <password> to login from now on.");
                args.Player.SendMessage("If you understand, please /login <username> <password> now, and type /auth-verify");
                return;
            }

            args.Player.SendMessage("Incorrect auth code. This incident has been logged.");
            Log.Warn(args.Player.IP + " attempted to use an incorrect auth code.");
        }

        private static void AuthVerify(CommandArgs args)
        {
            if (TShock.AuthToken == 0)
            {
                args.Player.SendMessage("It appears that you have already turned off the auth token.");
                args.Player.SendMessage("If this is a mistake, delete auth.lck.");
                return;
            }

            if (!args.Player.IsLoggedIn)
            {
                args.Player.SendMessage("You must be logged in to disable the auth system.");
                args.Player.SendMessage("This is a security measure designed to prevent insecure administration setups.");
                args.Player.SendMessage("Please re-run /auth and read the instructions!");
                args.Player.SendMessage("If you're still confused, consult the forums. http://tshock.co/");
                return;
            }

            args.Player.SendMessage("Your new account has been verified, and the /auth system has been turned off.");
            args.Player.SendMessage("You can always use the /user command to manage players. Don't just delete the auth.lck.");
            args.Player.SendMessage("Thankyou for using TShock! http://tshock.co/ & http://github.com/TShock/TShock");
            FileTools.CreateFile(Path.Combine(TShock.SavePath, "auth.lck"));
            File.Delete(Path.Combine(TShock.SavePath, "authcode.txt"));
            TShock.AuthToken = 0;
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

        private static void Whisper(CommandArgs args)
        {
            if (args.Parameters.Count < 2)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /whisper <player> <text>", Color.Red);
                return;
            }

            var players = Tools.FindPlayer(args.Parameters[0]);
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
                var msg = string.Join(" ", args.Parameters.ToArray(), 1, args.Parameters.Count - 1);
                plr.SendMessage("(Whisper From)" + "<" + args.Player.Name + ">" + msg, Color.MediumPurple);
                args.Player.SendMessage("(Whisper To)" + "<" + plr.Name + ">" + msg, Color.MediumPurple);
                plr.LastWhisper = args.Player;
                args.Player.LastWhisper = plr;
            }
        }

        private static void Reply(CommandArgs args)
        {
            if (args.Player.LastWhisper != null)
            {
                var msg = string.Join(" ", args.Parameters);
                args.Player.LastWhisper.SendMessage("(Whisper From)" + "<" + args.Player.Name + ">" + msg, Color.MediumPurple);
                args.Player.SendMessage("(Whisper To)" + "<" + args.Player.LastWhisper.Name + ">" + msg, Color.MediumPurple);
            }
            else
                args.Player.SendMessage("You haven't previously received any whispers. Please use /whisper to whisper to other people.", Color.Red);
        }

        private static void Annoy(CommandArgs args)
        {
            if (args.Parameters.Count != 2)
            {
                args.Player.SendMessage("Invalid syntax! Proper syntax: /annoy <player> <seconds to annoy>", Color.Red);
                return;
            }
            int annoy = 5;
            int.TryParse(args.Parameters[1], out annoy);

            var players = Tools.FindPlayer(args.Parameters[0]);
            if (players.Count == 0)
                args.Player.SendMessage("Invalid player!", Color.Red);
            else if (players.Count > 1)
                args.Player.SendMessage("More than one player matched!", Color.Red);
            else
            {
                var ply = players[0];
                args.Player.SendMessage("Annoying " + ply.Name + " for " + annoy.ToString() + " seconds.");
                (new Thread(new ParameterizedThreadStart(ply.Whoopie))).Start(annoy);
            }
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
                if (Main.npc[i].active && !Main.npc[i].townNPC && (!Main.npc[i].friendly || killFriendly))
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
                    if (args.Player.InventorySlotAvailable || item.name.Contains("Coin"))
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
                        if (plr.InventorySlotAvailable || item.name.Contains("Coin"))
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
