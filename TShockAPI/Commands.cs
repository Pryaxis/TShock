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
    public class Commands
    {
        public delegate void CommandDelegate(CommandArgs args);

        public static List<Command> commands = new List<Command>();

        public struct CommandArgs
        {
            public string Message;
            public int PlayerX;
            public int PlayerY;
            public int PlayerID;
            /// <summary>
            /// Parameters passed to the arguement. Does not include the command name. 
            /// IE '/kick "jerk face"' will only have 1 argument
            /// </summary>
            public List<string> Parameters;

            public CommandArgs(string message, int x, int y, int id, List<string> args)
            {
                Message = message;
                PlayerX = x;
                PlayerY = y;
                PlayerID = id;
                Parameters = args;
            }
        }

        public class Command
        {
            private string name;
            private string permission;
            private CommandDelegate command;

            public Command(string cmdName, string permissionNeeded, CommandDelegate cmd)
            {
                name = cmdName;
                permission = permissionNeeded;
                command = cmd;
            }

            public bool Run(string msg, TSPlayer ply, List<string> parms)
            {
                if (!ply.group.HasPermission(permission))
                {
                    return false;
                }

                CommandArgs args = new CommandArgs();
                args.Message = msg;
                args.PlayerX = (int)ply.GetPlayer().position.X;
                args.PlayerY = (int)ply.GetPlayer().position.Y;
                args.PlayerID = ply.GetPlayerID();
                args.Parameters = parms;

                command(args);
                return true;
            }

            public string Name()
            {
                return name;
            }

            public bool CanRun(TSPlayer ply)
            {
                if (!ply.group.HasPermission(permission))
                {
                    return false;
                }
                return true;
            }
        }

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
            commands.Add(new Command("kick", "kick", Kick));
            commands.Add(new Command("ban", "ban", Ban));
            commands.Add(new Command("banip", "ban", BanIP));
            commands.Add(new Command("unban", "unban", UnBan));
            commands.Add(new Command("unbanip", "unbanip", UnBanIP));
            commands.Add(new Command("off", "maintenance", Off));
            commands.Add(new Command("off-nosave", "maintenance", OffNoSave));
            commands.Add(new Command("checkupdates", "maintenance", CheckUpdates));
            commands.Add(new Command("dropmeteor", "causeevents", DropMeteor));
            commands.Add(new Command("star", "causeevents", Star));
            commands.Add(new Command("bloodmoon", "causeevents", Bloodmoon));
            commands.Add(new Command("invade", "causeevents", Invade));
            commands.Add(new Command("eater", "spawnboss", Eater));
            commands.Add(new Command("eye", "spawnboss", Eye));
            commands.Add(new Command("skeletron", "spawnboss", Skeletron));
            commands.Add(new Command("hardcore", "spawnboss", Hardcore));
            commands.Add(new Command("spawnmob", "spawnmob", SpawnMob));
            commands.Add(new Command("home", "tp", Home));
            commands.Add(new Command("spawn", "tp", Spawn));
            commands.Add(new Command("tp", "tp", TP));
            commands.Add(new Command("tphere", "tp", TPHere));
            commands.Add(new Command("reload", "cfg", Reload));
            commands.Add(new Command("debug-config", "cfg", DebugConfiguration));
            commands.Add(new Command("password", "cfg", Password));
            commands.Add(new Command("save", "cfg", Save));
            commands.Add(new Command("maxspawns", "cfg", MaxSpawns));
            commands.Add(new Command("spawnrate", "cfg", SpawnRate));
            commands.Add(new Command("time", "cfg", Time));
            commands.Add(new Command("slap", "pvpfun", Slap));
            commands.Add(new Command("protectspawn", "editspawn", ProtectSpawn));
            commands.Add(new Command("help", "", Help));
            commands.Add(new Command("playing", "", Playing));
            commands.Add(new Command("auth", "", AuthToken));
            commands.Add(new Command("me", "", ThirdPerson));
            commands.Add(new Command("p", "", PartyChat));
            commands.Add(new Command("butcher", "cheat", Butcher));
            if (ConfigurationManager.distributationAgent != "terraria-online")
            {
                commands.Add(new Command("kill", "kill", Kill));
                commands.Add(new Command("item", "cheat", Item));
                commands.Add(new Command("give", "cheat", Give));
                commands.Add(new Command("heal", "cheat", Heal));
            }
        }

        #region Command Methods

        public static void CheckUpdates(CommandArgs args)
        {
            ThreadPool.QueueUserWorkItem(UpdateManager.CheckUpdate);
        }

        public static void PartyChat(CommandArgs args)
        {
            int playerTeam = Main.player[args.PlayerID].team;
            if (playerTeam != 0)
            {
                string msg = "<" + Main.player[args.PlayerID].name + "> " + args.Message.Remove(0, 3);
                for (int i = 0; i < Main.player.Length; i++)
                {
                    if (Main.player[i].team == Main.player[args.PlayerID].team)
                    {
                        Tools.SendMessage(i, msg, Main.teamColor[playerTeam].R, Main.teamColor[playerTeam].G, Main.teamColor[playerTeam].B);
                    }
                }
            }
            else
            {
                Tools.SendMessage(args.PlayerID, "You are not in a party!", 255f, 240f, 20f);
            }
        }

        public static void ThirdPerson(CommandArgs args)
        {
            string msg = args.Message.Remove(0, 3);
            Tools.Broadcast("*" + Tools.FindPlayer(args.PlayerID) + " " + msg, 205, 133, 63);
        }

        public static void Playing(CommandArgs args)
        {
            Tools.SendMessage(args.PlayerID, Tools.GetPlayers());
        }

        public static void DebugConfiguration(CommandArgs args)
        {
            int ply = args.PlayerID;
            Tools.SendMessage(ply, "TShock Config:");
            string lineOne = "";
            lineOne += "KickCheater : " + ConfigurationManager.kickCheater + ", ";
            lineOne += "BanCheater : " + ConfigurationManager.banCheater + ", ";
            lineOne += "KickGriefer : " + ConfigurationManager.kickGriefer + ", ";
            lineOne += "BanGriefer : " + ConfigurationManager.banGriefer;
            Tools.SendMessage(ply, lineOne, Color.Yellow);
            string lineTwo = "";
            lineTwo += "BanTnt : " + ConfigurationManager.banTnt + ", ";
            lineTwo += "KickTnt : " + ConfigurationManager.kickTnt + ", ";
            lineTwo += "BanBoom : " + ConfigurationManager.banBoom + ", ";
            lineTwo += "KickBoom : " + ConfigurationManager.kickBoom;
            Tools.SendMessage(ply, lineTwo, Color.Yellow);
            string lineThree = "";
            lineThree += "InvMultiplier : " + ConfigurationManager.invasionMultiplier + ", ";
            lineThree += "ProtectS : " + ConfigurationManager.spawnProtect + ", ";
            lineThree += "ProtectR : " + ConfigurationManager.spawnProtectRadius + ", ";
            lineThree += "DMS : " + ConfigurationManager.defaultMaxSpawns + ", ";
            lineThree += "SpawnRate: " + ConfigurationManager.defaultSpawnRate;
            Tools.SendMessage(ply, lineThree, Color.Yellow);
            string lineFour = "";
            lineFour += "MaxSlots : " + ConfigurationManager.maxSlots + ", ";
            lineFour += "RangeChecks : " + ConfigurationManager.rangeChecks + ", ";
            Tools.SendMessage(ply, lineFour, Color.Yellow);
        }

        public static void Kick(CommandArgs args)
        {
            int adminplr = args.PlayerID;

            if (args.Parameters.Count < 1)
            {
                Tools.SendMessage(adminplr, "Invalid syntax! Proper syntax: /kick <player> [reason]", Color.Red);
                return;
            }
            if (args.Parameters[0].Length == 0)
            {
                Tools.SendMessage(adminplr, "Missing player name", Color.Red);
                return;
            }

            string plStr = args.Parameters[0];
            int player = Tools.FindPlayer(plStr);
            if (player == -1)
                Tools.SendMessage(adminplr, "Invalid player!", Color.Red);
            else if (player == -2)
                Tools.SendMessage(adminplr, "More than one player matched!", Color.Red);
            else
            {
                string reason = args.Parameters.Count > 1 ? String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1)) : "Misbehaviour.";
                if (!Tools.Kick(player, reason))
                {
                    Tools.SendMessage(adminplr, "You can't kick another admin!", Color.Red);
                }
            }
        }

        public static void Ban(CommandArgs args)
        {
            int adminplr = args.PlayerID;

            if (args.Parameters.Count < 1)
            {
                Tools.SendMessage(adminplr, "Invalid syntax! Proper syntax: /ban <player> [reason]", Color.Red);
                return;
            }
            if (args.Parameters[0].Length == 0)
            {
                Tools.SendMessage(adminplr, "Missing player name", Color.Red);
                return;
            }

            string plStr = args.Parameters[0];
            int player = Tools.FindPlayer(plStr);
            if (player == -1)
                Tools.SendMessage(adminplr, "Invalid player!", Color.Red);
            else if (player == -2)
                Tools.SendMessage(adminplr, "More than one player matched!", Color.Red);
            else
            {
                string reason = args.Parameters.Count > 1 ? String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1)) : "Misbehaviour.";
                if (!Tools.Ban(player, reason))
                {
                    Tools.SendMessage(adminplr, "You can't ban another admin!", Color.Red);
                }
            }
        }

        public static void BanIP(CommandArgs args)
        {
            int adminplr = args.PlayerID;

            if (args.Parameters.Count < 1)
            {
                Tools.SendMessage(adminplr, "Syntax: /banip <ip> [reason]", Color.Red);
                return;
            }
            if (args.Parameters[0].Length == 0)
            {
                Tools.SendMessage(adminplr, "Missing IP address", Color.Red);
                return;
            }

            string ip = args.Parameters[0];
            string reason = args.Parameters.Count > 1 ? String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1)) : "Manually added IP address ban.";
            TShock.Bans.AddBan(ip, "", reason);
        }

        public static void UnBan(CommandArgs args)
        {
            int adminplr = args.PlayerID;

            if (args.Parameters.Count < 1)
            {
                Tools.SendMessage(adminplr, "Invalid syntax! Proper syntax: /unban <player>", Color.Red);
                return;
            }
            if (args.Parameters[0].Length == 0)
            {
                Tools.SendMessage(adminplr, "Missing player name", Color.Red);
                return;
            }

            string plStr = args.Parameters[0];
            var ban = TShock.Bans.GetBanByName(plStr);
            if (ban != null)
            {
                TShock.Bans.RemoveBan(ban);
                Tools.SendMessage(adminplr, string.Format("Unbanned {0} ({1})!", ban.Name, ban.IP), Color.Red);
            }
            else
            {
                Tools.SendMessage(adminplr, "Invalid player!", Color.Red);
            }
        }

        public static void UnBanIP(CommandArgs args)
        {
            int adminplr = args.PlayerID;

            if (args.Parameters.Count < 1)
            {
                Tools.SendMessage(adminplr, "Invalid syntax! Proper syntax: /unbanip <ip>", Color.Red);
                return;
            }
            if (args.Parameters[0].Length == 0)
            {
                Tools.SendMessage(adminplr, "Missing ip", Color.Red);
                return;
            }

            string plStr = args.Parameters[0];
            var ban = TShock.Bans.GetBanByIp(plStr);
            if (ban != null)
            {
                TShock.Bans.RemoveBan(ban);
                Tools.SendMessage(adminplr, string.Format("Unbanned {0} ({1})!", ban.Name, ban.IP), Color.Red);
            }
            else
            {
                Tools.SendMessage(adminplr, "Invalid player!", Color.Red);
            }
        }

        public static void Off(CommandArgs args)
        {
            for (int player = 0; player < Main.maxPlayers; player++)
            {
                if (Main.player[player].active)
                {
                    Tools.ForceKick(player, "Server shutting down!");
                }
            }
            WorldGen.saveWorld();
            Netplay.disconnect = true;
        }

        public static void OffNoSave(CommandArgs args)
        {
            Netplay.disconnect = true;
        }

        public static void Reload(CommandArgs args)
        {
            FileTools.SetupConfig();
            Tools.SendMessage(args.PlayerID, "Configuration reload complete. Some changes may require server restart.");
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
            int ply = args.PlayerID;
            Tools.Broadcast(Tools.FindPlayer(ply) + " turned on blood moon.");
            Main.bloodMoon = true;
            Main.time = 0;
            Main.dayTime = false;
            NetMessage.SendData(18, -1, -1, "", 0, 0, Main.sunModY, Main.moonModY);
            NetMessage.syncPlayers();
        }

        public static void Eater(CommandArgs args)
        {
            int x = args.PlayerX;
            int y = args.PlayerY;
            int ply = args.PlayerID;
            Tools.NewNPC((int)ConfigurationManager.NPCList.WORLD_EATER, x, y, ply);
            Tools.Broadcast(Tools.FindPlayer(ply) + " has spawned an eater of worlds!");
        }

        public static void Eye(CommandArgs args)
        {
            int x = args.PlayerX;
            int y = args.PlayerY;
            int ply = args.PlayerID;
            Tools.NewNPC((int)ConfigurationManager.NPCList.EYE, x, y, ply);
            Tools.Broadcast(Tools.FindPlayer(ply) + " has spawned an eye!");
        }

        public static void Skeletron(CommandArgs args)
        {
            int x = args.PlayerX;
            int y = args.PlayerY;
            int ply = args.PlayerID;
            Tools.NewNPC((int)ConfigurationManager.NPCList.SKELETRON, x, y, ply);
            Tools.Broadcast(Tools.FindPlayer(ply) + " has spawned skeletron!");
        }

        public static void Hardcore(CommandArgs args)
        {
            int x = args.PlayerX;
            int y = args.PlayerY;
            int ply = args.PlayerID;
            for (int i = 0; i <= 2; i++)
            {
                Tools.NewNPC(i, x, y, ply);
            }
            Tools.Broadcast(Tools.FindPlayer(ply) + " has spawned all 3 bosses!");
        }

        public static void Invade(CommandArgs args)
        {
            int ply = args.PlayerID;
            if (Main.invasionSize <= 0)
            {
                Tools.Broadcast(Main.player[ply].name + " has started an invasion.");
                TShock.StartInvasion();
            }
            else
            {
                Tools.Broadcast(Main.player[ply].name + " has ended an invasion.");
                Main.invasionSize = 0;
            }
        }

        public static void Password(CommandArgs args)
        {
            int ply = args.PlayerID;
            string passwd = args.Message.Remove(0, 9).Trim();
            Netplay.password = passwd;
            Tools.SendMessage(ply, "Server password changed to: " + passwd);
        }

        public static void Save(CommandArgs args)
        {
            int ply = args.PlayerID;
            WorldGen.saveWorld();
            Tools.SendMessage(ply, "World saved.");
        }

        public static void Home(CommandArgs args)
        {
            int ply = args.PlayerID;
            TShock.Teleport(ply, Main.player[args.PlayerID].SpawnX * 16 + 8 - Main.player[ply].width / 2,
                            Main.player[args.PlayerID].SpawnY * 16 - Main.player[ply].height);
            Tools.SendMessage(ply, "Teleported to your spawnpoint.");
        }

        public static void Spawn(CommandArgs args)
        {
            int ply = args.PlayerID;
            TShock.Teleport(ply, Main.spawnTileX * 16 + 8 - Main.player[ply].width / 2,
                            Main.spawnTileY * 16 - Main.player[ply].height);
            Tools.SendMessage(ply, "Teleported to the map's spawnpoint.");
        }

        public static void AuthToken(CommandArgs args)
        {
            if (ConfigurationManager.authToken == 0)
            {
                return;
            }
            int givenCode = Convert.ToInt32(args.Parameters[0]);
            if (givenCode == ConfigurationManager.authToken)
            {
                TextWriter tw = new StreamWriter(FileTools.SaveDir + "users.txt", true);
                tw.Write("\n" +
                         Tools.GetRealIP(
                             Convert.ToString(Netplay.serverSock[args.PlayerID].tcpClient.Client.RemoteEndPoint)) +
                         " superadmin");
                Tools.SendMessage(args.PlayerID, "SuperAdmin authenticated. Please re-connect using the same IP.");
                ConfigurationManager.authToken = 0;
                tw.Close();
            }
        }

        public static void TP(CommandArgs args)
        {
            int adminplr = args.PlayerID;

            if (args.Parameters.Count < 1)
            {
                Tools.SendMessage(adminplr, "Invalid syntax! Proper syntax: /tp <player> ", Color.Red);
                return;
            }

            string plStr = String.Join(" ", args.Parameters);
            int player = Tools.FindPlayer(plStr);
            if (player == -1)
                Tools.SendMessage(adminplr, "Invalid player!", Color.Red);
            else if (player == -2)
                Tools.SendMessage(adminplr, "More than one player matched!", Color.Red);
            else
            {
                TShock.Teleport(adminplr, Main.player[player].position.X, Main.player[player].position.Y);
                Tools.SendMessage(adminplr, "Teleported to " + Tools.FindPlayer(player));
            }
        }

        public static void TPHere(CommandArgs args)
        {
            int adminplr = args.PlayerID;

            if (args.Parameters.Count < 1)
            {
                Tools.SendMessage(adminplr, "Invalid syntax! Proper syntax: /tphere <player> ", Color.Red);
                return;
            }

            string plStr = String.Join(" ", args.Parameters);
            int player = Tools.FindPlayer(plStr);
            if (player == -1)
                Tools.SendMessage(adminplr, "Invalid player!", Color.Red);
            else if (player == -2)
                Tools.SendMessage(adminplr, "More than one player matched!", Color.Red);
            else
            {
                TShock.Teleport(player, Main.player[adminplr].position.X, Main.player[adminplr].position.Y);
                Tools.SendMessage(player, "You were teleported to " + Tools.FindPlayer(adminplr) + ".");
                Tools.SendMessage(adminplr, "You brought " + Tools.FindPlayer(player) + " here.");
            }
        }

        public static void SpawnMob(CommandArgs args)
        {
            int adminplr = args.PlayerID;

            if (args.Parameters.Count < 1 || args.Parameters.Count > 2)
            {
                Tools.SendMessage(adminplr, "Invalid syntax! Proper syntax: /spawnmob <mob name/id> [amount]", Color.Red);
                return;
            }
            if (args.Parameters[0].Length == 0)
            {
                Tools.SendMessage(adminplr, "Missing mob name/id", Color.Red);
                return;
            }

            int x = args.PlayerX;
            int y = args.PlayerY;
            int type = -1;
            int amount = 1;

            if (!int.TryParse(args.Parameters[0], out type))
                type = TShock.GetNPCID(args.Parameters[0]);
            if (args.Parameters.Count == 2 && !int.TryParse(args.Parameters[1], out amount))
            {
                Tools.SendMessage(adminplr, "Invalid syntax! Proper syntax: /spawnmob <mob name/id> [amount]", Color.Red);
                return;
            }

            if (type >= 1 && type <= 45)
            {
                int npcid = -1;
                for (int i = 0; i < amount; i++)
                    npcid = NPC.NewNPC(x, y, type, 0);
                Tools.Broadcast(string.Format("{0} was spawned {1} time(s).", Main.npc[npcid].name, amount));
            }
            else
                Tools.SendMessage(adminplr, "Invalid mob type!", Color.Red);
        }

        public static void Item(CommandArgs args)
        {
            int adminplr = args.PlayerID;

            if (args.Parameters.Count < 1)
            {
                Tools.SendMessage(adminplr, "Invalid syntax! Proper syntax: /item <item name/id>", Color.Red);
                return;
            }
            if (args.Parameters[0].Length == 0)
            {
                Tools.SendMessage(adminplr, "Missing item name/id", Color.Red);
                return;
            }

            int type = -1;
            if (!int.TryParse(args.Parameters[0], out type))
                type = TShock.GetItemID(String.Join(" ", args.Parameters));

            if (type < 1 || type > 238)
            {
                Tools.SendMessage(adminplr, "Invalid item type!", Color.Red);
                return;
            }

            bool flag = false;
            for (int i = 0; i < 40; i++)
            {
                if (!Main.player[adminplr].inventory[i].active)
                {
                    //Main.player[ply].inventory[i].SetDefaults(type);
                    //Main.player[ply].inventory[i].stack = Main.player[ply].inventory[i].maxStack;
                    int id = Terraria.Item.NewItem(0, 0, 0, 0, type, 1, true);
                    Main.item[id].position.X = args.PlayerX;
                    Main.item[id].position.Y = args.PlayerY;
                    Main.item[id].stack = Main.item[id].maxStack;
                    //TShock.SendDataAll(21, -1, "", id);
                    NetMessage.SendData(21, -1, -1, "", id, 0f, 0f, 0f);
                    Tools.SendMessage(adminplr, "Got some " + Main.item[id].name + ".");
                    //TShock.UpdateInventories();
                    flag = true;
                    break;
                }
            }
            if (!flag)
                Tools.SendMessage(adminplr, "You don't have free slots!", Color.Red);
        }

        public static void Give(CommandArgs args)
        {
            int adminplr = args.PlayerID;

            if (args.Parameters.Count != 2)
            {
                Tools.SendMessage(adminplr, "Invalid syntax! Proper syntax: /give <item type/id> <player>", Color.Red);
                return;
            }
            if (args.Parameters[0].Length == 0)
            {
                Tools.SendMessage(adminplr, "Missing item name/id", Color.Red);
                return;
            }
            if (args.Parameters[1].Length == 0)
            {
                Tools.SendMessage(adminplr, "Missing player name", Color.Red);
                return;
            }

            int type = -1;
            if (!int.TryParse(args.Parameters[0], out type))
                type = TShock.GetItemID(args.Parameters[0]);

            if (type < 1 || type > 238)
            {
                Tools.SendMessage(args.PlayerID, "Invalid item type!", Color.Red);
                return;
            }

            string plStr = args.Parameters[1];
            int player = Tools.FindPlayer(plStr);
            if (player == -1)
                Tools.SendMessage(adminplr, "Invalid player!", Color.Red);
            else if (player == -2)
                Tools.SendMessage(adminplr, "More than one player matched!", Color.Red);
            else
            {
                bool flag = false;
                for (int i = 0; i < 40; i++)
                {
                    if (!Main.player[player].inventory[i].active)
                    {
                        //Main.player[player].inventory[i].SetDefaults(type);
                        //Main.player[player].inventory[i].stack = Main.player[player].inventory[i].maxStack;
                        int id = Terraria.Item.NewItem(0, 0, 0, 0, type, 1, true);
                        Main.item[id].position.X = Main.player[player].position.X;
                        Main.item[id].position.Y = Main.player[player].position.Y;
                        Main.item[id].stack = Main.item[id].maxStack;
                        //TShock.SendDataAll(21, -1, "", id);
                        NetMessage.SendData(21, -1, -1, "", id, 0f, 0f, 0f);
                        Tools.SendMessage(adminplr,
                                          string.Format("Gave {0} some {1}.", Tools.FindPlayer(player), Main.item[id].name));
                        Tools.SendMessage(player,
                                          string.Format("{0} gave you some {1}.", Tools.FindPlayer(adminplr),
                                                        Main.item[id].name));
                        //TShock.UpdateInventories();
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                    Tools.SendMessage(args.PlayerID, "Player does not have free slots!", Color.Red);
            }
        }

        public static void Heal(CommandArgs args)
        {
            int adminplr = args.PlayerID;

            if (args.Parameters.Count > 1)
            {
                string plStr = String.Join(" ", args.Parameters);
                int player = Tools.FindPlayer(plStr);
                if (player == -1)
                    Tools.SendMessage(adminplr, "Invalid player!", Color.Red);
                else if (player == -2)
                    Tools.SendMessage(adminplr, "More than one player matched!", Color.Red);
                else
                {
                    DropHearts((int)Main.player[player].position.X, (int)Main.player[player].position.Y, 20);
                    Tools.SendMessage(adminplr, string.Format("You just healed {0}", Tools.FindPlayer(player)));
                    Tools.SendMessage(player, string.Format("{0} just healed you!", Tools.FindPlayer(adminplr)));
                }
            }
            else
            {
                DropHearts(args.PlayerX, args.PlayerY, 20);
                Tools.SendMessage(adminplr, "You just got healed!");
            }
        }

        private static void DropHearts(int x, int y, int count)
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
                if (Main.npc[i].townNPC || !Main.npc[i].active)
                    continue;
                else
                {
                    Main.npc[i].StrikeNPC(99999, 90f, 1);
                    NetMessage.SendData(28, -1, -1, "", i, 99999, 90f, 1);
                    killcount++;
                }
            }
            Tools.Broadcast("Killed " + killcount + " NPCs.");
        }

        public static void MaxSpawns(CommandArgs args)
        {
            int adminplr = args.PlayerID;

            if (args.Parameters.Count != 1)
            {
                Tools.SendMessage(adminplr, "Invalid syntax! Proper syntax: /maxspawns <maxspawns>", Color.Red);
                return;
            }

            int amount = Convert.ToInt32(args.Parameters[0]);
            int.TryParse(args.Parameters[0], out amount);
            NPC.defaultMaxSpawns = amount;
            ConfigurationManager.defaultMaxSpawns = amount;
            Tools.Broadcast(Tools.FindPlayer(adminplr) + " changed the maximum spawns to: " + amount);
        }

        public static void SpawnRate(CommandArgs args)
        {
            int adminplr = args.PlayerID;

            if (args.Parameters.Count != 1)
            {
                Tools.SendMessage(adminplr, "Invalid syntax! Proper syntax: /spawnrate <spawnrate>", Color.Red);
                return;
            }

            int amount = Convert.ToInt32(args.Parameters[0]);
            int.TryParse(args.Parameters[0], out amount);
            NPC.defaultSpawnRate = amount;
            ConfigurationManager.defaultSpawnRate = amount;
            Tools.Broadcast(Tools.FindPlayer(adminplr) + " changed the spawn rate to: " + amount);
        }

        public static void Help(CommandArgs args)
        {
            int ply = args.PlayerID;
            Tools.SendMessage(ply, "TShock Commands:");
            string tempstring = "";
            int page = 1;
            if (args.Parameters.Count > 0)
                int.TryParse(args.Parameters[0], out page);
            List<Command> cmdlist = new List<Command>();
            for (int j = 0; j < commands.Count; j++)
            {
                if (commands[j].CanRun(TShock.players[ply]))
                {
                    cmdlist.Add(commands[j]);
                }
            }
            if (cmdlist.Count > (15 * (page - 1)))
            {
                for (int j = (15 * (page - 1)); j < (15 * page); j++)
                {
                    tempstring += "/" + cmdlist[j].Name() + ", ";
                    if (j == cmdlist.Count - 1)
                    {
                        Tools.SendMessage(ply, tempstring.TrimEnd(new[] { ' ', ',' }), Color.Yellow);
                        break;
                    }
                    if ((j + 1) % 5 == 0)
                    {
                        Tools.SendMessage(ply, tempstring.TrimEnd(new[] { ' ', ',' }), Color.Yellow);
                        tempstring = "";
                    }
                }
            }
            if (cmdlist.Count > (15 * page))
            {
                Tools.SendMessage(ply, "Type /help " + (page + 1) + " for more commands.", Color.Yellow);
            }
        }

        public static void Time(CommandArgs args)
        {
            int adminplr = args.PlayerID;

            if (args.Parameters.Count != 1)
            {
                Tools.SendMessage(adminplr, "Invalid syntax! Proper syntax: /time <day/night/dusk/noon/midnight>", Color.Red);
                return;
            }

            switch (args.Parameters[0])
            {
                case "day":
                    Main.time = 0;
                    Main.dayTime = true;
                    NetMessage.SendData(18, -1, -1, "", 0, 0, Main.sunModY, Main.moonModY);
                    NetMessage.syncPlayers();
                    Tools.Broadcast(Tools.FindPlayer(args.PlayerID) + " set time to day.");
                    break;
                case "night":
                    Main.time = 0;
                    Main.dayTime = false;
                    NetMessage.SendData(18, -1, -1, "", 0, 0, Main.sunModY, Main.moonModY);
                    NetMessage.syncPlayers();
                    Tools.Broadcast(Tools.FindPlayer(args.PlayerID) + " set time to night.");
                    break;
                case "dusk":
                    Main.dayTime = false;
                    Main.time = 0.0;
                    NetMessage.SendData(18, -1, -1, "", 0, 0, Main.sunModY, Main.moonModY);
                    NetMessage.syncPlayers();
                    Tools.Broadcast(Tools.FindPlayer(args.PlayerID) + " set time to dusk.");
                    break;
                case "noon":
                    Main.dayTime = true;
                    Main.time = 27000.0;
                    NetMessage.SendData(18, -1, -1, "", 0, 0, Main.sunModY, Main.moonModY);
                    NetMessage.syncPlayers();
                    Tools.Broadcast(Tools.FindPlayer(args.PlayerID) + " set time to noon.");
                    break;
                case "midnight":
                    Main.dayTime = false;
                    Main.time = 16200.0;
                    NetMessage.SendData(18, -1, -1, "", 0, 0, Main.sunModY, Main.moonModY);
                    NetMessage.syncPlayers();
                    Tools.Broadcast(Tools.FindPlayer(args.PlayerID) + " set time to midnight.");
                    break;
                default:
                    Tools.SendMessage(adminplr, "Invalid syntax! Proper syntax: /time <day/night/dusk/noon/midnight>", Color.Red);
                    break;
            }
        }

        public static void Kill(CommandArgs args)
        {
            int adminplr = args.PlayerID;

            if (args.Parameters.Count < 1)
            {
                Tools.SendMessage(adminplr, "Invalid syntax! Proper syntax: /kill <player>", Color.Red);
                return;
            }

            string plStr = String.Join(" ", args.Parameters);
            int player = Tools.FindPlayer(plStr);
            if (player == -1)
                Tools.SendMessage(adminplr, "Invalid player!", Color.Red);
            else if (player == -2)
                Tools.SendMessage(adminplr, "More than one player matched!", Color.Red);
            else
            {
                Tools.SendMessage(adminplr, "You just killed " + Tools.FindPlayer(player) + "!");
                Tools.SendMessage(player, Tools.FindPlayer(adminplr) + " just killed you!");
                TShock.KillMe(player);
            }
        }

        public static void Slap(CommandArgs args)
        {
            int adminplr = args.PlayerID;

            if (args.Parameters.Count < 1 || args.Parameters.Count > 2)
            {
                Tools.SendMessage(adminplr, "Invalid syntax! Proper syntax: /slap <player> [dmg]", Color.Red);
                return;
            }
            if (args.Parameters[0].Length == 0)
            {
                Tools.SendMessage(adminplr, "Missing player name", Color.Red);
                return;
            }

            string plStr = args.Parameters[0];
            int player = Tools.FindPlayer(plStr);
            if (player == -1)
                Tools.SendMessage(adminplr, "Invalid player!", Color.Red);
            else if (player == -2)
                Tools.SendMessage(adminplr, "More than one player matched!", Color.Red);
            else
            {
                int damage = 5;
                if (args.Parameters.Count == 2)
                {
                    int.TryParse(args.Parameters[1], out damage);
                }
                NetMessage.SendData(26, -1, -1, "", player, ((new Random()).Next(-1, 1)), damage, (float)0);
                Tools.Broadcast(Tools.FindPlayer(adminplr) + " slapped " + Tools.FindPlayer(player) + " for " +
                                damage + " damage.");
            }
        }

        public static void ProtectSpawn(CommandArgs args)
        {
            ConfigurationManager.spawnProtect = (ConfigurationManager.spawnProtect == false);
            Tools.SendMessage(args.PlayerID,
                              "Spawn is now " + (ConfigurationManager.spawnProtect ? "protected" : "open") + ".");
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

            for (int player = 0; player < Main.maxPlayers; player++)
            {
                if (Main.player[player].active)
                {
                    Tools.ForceKick(player, "Server shutting down for update!");
                }
            }
            WorldGen.saveWorld();
            Netplay.disconnect = true;
        }

        #endregion Command Methods
    }
}