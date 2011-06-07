using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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

            public CommandArgs(string message, int x, int y, int id)
            {
                Message = message;
                PlayerX = x;
                PlayerY = y;
                PlayerID = id;
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

            public bool Run(string msg, TSPlayer ply)
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

        public static void InitCommands()
        {
            commands.Add(new Command("kick", "kick", Kick));
            commands.Add(new Command("ban", "ban", Ban));
            commands.Add(new Command("banip", "ban", BanIP));
            commands.Add(new Command("unban", "unban", UnBan));
            commands.Add(new Command("unbanip", "unbanip", UnBanIP));
            commands.Add(new Command("off", "maintenance", Off));
            commands.Add(new Command("reload", "cfg", Reload));
            commands.Add(new Command("dropmeteor", "causeevents", DropMeteor));
            commands.Add(new Command("star", "causeevents", Star));
            commands.Add(new Command("bloodmoon", "causeevents", Bloodmoon));
            commands.Add(new Command("eater", "spawnboss", Eater));
            commands.Add(new Command("eye", "spawnboss", Eye));
            commands.Add(new Command("skeletron", "spawnboss", Skeletron));
            commands.Add(new Command("hardcore", "spawnboss", Hardcore));
            commands.Add(new Command("invade", "causeevents", Invade));
            commands.Add(new Command("password", "cfg", Password));
            commands.Add(new Command("save", "cfg", Save));
            commands.Add(new Command("home", "tp", Home));
            commands.Add(new Command("spawn", "tp", Spawn));
            commands.Add(new Command("tp", "tp", TP));
            commands.Add(new Command("tphere", "tp", TPHere));
            commands.Add(new Command("spawnmob", "spawnmob", SpawnMob));
            commands.Add(new Command("butcher", "cheat", Butcher));
            commands.Add(new Command("maxspawns", "cfg", MaxSpawns));
            commands.Add(new Command("spawnrate", "cfg", SpawnRate));
            commands.Add(new Command("time", "cfg", Time));
            commands.Add(new Command("help", "", Help));
            commands.Add(new Command("slap", "pvpfun", Slap));
            commands.Add(new Command("off-nosave", "maintenance", OffNoSave));
            commands.Add(new Command("protectspawn", "editspawn", ProtectSpawn));
            commands.Add(new Command("debug-config", "cfg", DebugConfiguration));
            commands.Add(new Command("playing", "", Playing));
            commands.Add(new Command("auth", "", AuthToken));
            commands.Add(new Command("me", "", ThirdPerson));
            commands.Add(new Command("p", "", PartyChat));
            if (ConfigurationManager.distributationAgent != "terraria-online")
            {
                commands.Add(new Command("kill", "kill", Kill));
                commands.Add(new Command("item", "cheat", Item));
                commands.Add(new Command("give", "cheat", Give));
                commands.Add(new Command("heal", "cheat", Heal));
            }
        }

        #region Command Methods

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
                        Tools.SendMessage(i, msg, new float[] { (float)Main.teamColor[playerTeam].R, (float)Main.teamColor[playerTeam].G, (float)Main.teamColor[playerTeam].B });
                    }
                }
            }
            else
            {
                Tools.SendMessage(args.PlayerID, "You are not in a party!", new float[] { 255f, 240f, 20f });
            }
        }

        public static void ThirdPerson(CommandArgs args)
        {
            string msg = args.Message.Remove(0, 3);
            Tools.Broadcast("*" + Tools.FindPlayer(args.PlayerID) + " " + msg, new float[] { 205, 133, 63 });
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
            Tools.SendMessage(ply, lineOne, new[] { 255f, 255f, 0f });
            string lineTwo = "";
            lineTwo += "BanTnt : " + ConfigurationManager.banTnt + ", ";
            lineTwo += "KickTnt : " + ConfigurationManager.kickTnt + ", ";
            lineTwo += "BanBoom : " + ConfigurationManager.banBoom + ", ";
            lineTwo += "KickBoom : " + ConfigurationManager.kickBoom;
            Tools.SendMessage(ply, lineTwo, new[] { 255f, 255f, 0f });
            string lineThree = "";
            lineThree += "InvMultiplier : " + ConfigurationManager.invasionMultiplier + ", ";
            lineThree += "ProtectS : " + ConfigurationManager.spawnProtect + ", ";
            lineThree += "ProtectR : " + ConfigurationManager.spawnProtectRadius + ", ";
            lineThree += "DMS : " + ConfigurationManager.defaultMaxSpawns + ", ";
            lineThree += "SpawnRate: " + ConfigurationManager.defaultSpawnRate;
            Tools.SendMessage(ply, lineThree, new[] { 255f, 255f, 0f });
            string lineFour = "";
            lineFour += "MaxSlots : " + ConfigurationManager.maxSlots + ", ";
            Tools.SendMessage(ply, lineFour, new[] { 255f, 255f, 0f });
        }

        public static void Kick(CommandArgs args)
        {
            string plStr = args.Message.Remove(0, 5).Trim().TrimEnd('"').TrimStart('"');
            int ply = args.PlayerID;
            int player = Tools.FindPlayer(plStr);
            if (!(player == -1 || player == -2 || plStr == ""))
            {
                if (!TShock.players[Tools.FindPlayer(plStr)].group.HasPermission("immunetokick"))
                {
                    Tools.Kick(player, "You were kicked.");
                    Tools.Broadcast(Tools.FindPlayer(player) + " was kicked by " + Tools.FindPlayer(ply));
                }
                else
                    Tools.SendMessage(ply, "You can't kick another admin!", new[] { 255f, 0f, 0f });
            }
            else if (Tools.FindPlayer(plStr) == -2)
                Tools.SendMessage(ply, "More than one player matched!", new[] { 255f, 0f, 0f });
            else
                Tools.SendMessage(ply, "Invalid player!", new[] { 255f, 0f, 0f });
        }

        public static void BanIP(CommandArgs args)
        {
            if (args.Message.Split(' ').Length == 2)
            {
                string ip = args.Message.Split(' ')[1];
                TShock.Bans.AddBan(ip, "", "Manually added IP address ban.");
            } else if (args.Message.Split(' ').Length > 2)
            {
                string reason = "";
                for (int i = 2; i > args.Message.Split(' ').Length;i++)
                {
                    reason += args.Message.Split(' ')[i];
                }
                string ip = args.Message.Split(' ')[1];
                TShock.Bans.AddBan(ip, "", reason);
            }
            else
            {
                Tools.SendMessage(args.PlayerID, "Syntax: /banip <ip> <reason>");
            }
        }

        public static void Ban(CommandArgs args)
        {
            string plStr = args.Message.Remove(0, 4).Trim().TrimEnd('"').TrimStart('"').Split(' ')[0];
            string[] reason = plStr.Split(' ');
            string banReason = "";
            for (int i = 0; i < reason.Length; i++)
            {
                if (reason[i].Contains("\""))
                    reason[i] = "";
            }
            for (int i = 0; i < reason.Length; i++)
            {
                banReason += reason[i];
            }
            int adminplr = args.PlayerID;
            int player = Tools.FindPlayer(plStr);
            if (!(player == -1 || player == -2 || plStr == ""))
            {
                if (!TShock.players[Tools.FindPlayer(plStr)].group.HasPermission("immunetoban"))
                {
                    TShock.Bans.AddBan(Tools.GetPlayerIP(player), Main.player[player].name);
                    Tools.Kick(player, "You were banned.");
                    Tools.Broadcast(Tools.FindPlayer(adminplr) + " banned " + Tools.FindPlayer(player) + " with reason " + reason + "!");
                }
                else
                    Tools.SendMessage(adminplr, "You can't ban another admin!", new[] { 255f, 0f, 0f });
            }
            else if (Tools.FindPlayer(plStr) == -2)
                Tools.SendMessage(adminplr, "More than one player matched!", new[] { 255f, 0f, 0f });
            else
                Tools.SendMessage(adminplr, "Invalid player!", new[] { 255f, 0f, 0f });
        }

        public static void UnBan(CommandArgs args)
        {
            string plStr = args.Message.Remove(0, 6);
            int adminplr = args.PlayerID;
            var ban = TShock.Bans.GetBanByName(plStr);
            if (ban != null)
            {
                TShock.Bans.RemoveBan(ban);
                Tools.SendMessage(adminplr, string.Format("Unbanned {0} ({1})!", ban.Name, ban.IP), new[] { 255f, 0f, 0f });
            }
            else
            {
                Tools.SendMessage(adminplr, "Invalid player!", new[] { 255f, 0f, 0f });
            }
        }

        public static void UnBanIP(CommandArgs args)
        {
            string plStr = args.Message.Remove(0, 8);
            int adminplr = args.PlayerID;
            var ban = TShock.Bans.GetBanByIp(plStr);
            if (ban != null)
            {
                TShock.Bans.RemoveBan(ban);
                Tools.SendMessage(adminplr, string.Format("Unbanned {0} ({1})!", ban.Name, ban.IP), new[] { 255f, 0f, 0f });
            }
            else
            {
                Tools.SendMessage(adminplr, "Invalid player!", new[] { 255f, 0f, 0f });
            }
        }

        public static void Off(CommandArgs args)
        {
            for (int player = 0; player < Main.maxPlayers; player++)
            {
                if (Main.player[player].active)
                {
                    Tools.Kick(player, "Server shutting down!");
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
            int givenCode = Convert.ToInt32(args.Message.Remove(0, 5));
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
            int ply = args.PlayerID;
            string player = args.Message.Remove(0, 3).Trim().TrimEnd('"').TrimStart('"');
            if (Tools.FindPlayer(player) != -1 && Tools.FindPlayer(player) != -2 && player != "")
            {
                TShock.Teleport(ply, Main.player[Tools.FindPlayer(player)].position.X,
                                Main.player[Tools.FindPlayer(player)].position.Y);
                Tools.SendMessage(ply, "Teleported to " + player);
            }
            else
                Tools.SendMessage(ply, "Invalid player!", new[] { 255f, 0f, 0f });
        }

        public static void TPHere(CommandArgs args)
        {
            int ply = args.PlayerID;
            string player = args.Message.Remove(0, 7).Trim().TrimEnd('"').TrimStart('"');
            if (Tools.FindPlayer(player) != -1 && Tools.FindPlayer(player) != -2 && player != "")
            {
                TShock.Teleport(Tools.FindPlayer(player), Main.player[ply].position.X, Main.player[ply].position.Y);
                Tools.SendMessage(Tools.FindPlayer(player), "You were teleported to " + Tools.FindPlayer(ply) + ".");
                Tools.SendMessage(ply, "You brought " + player + " here.");
            }
            else
                Tools.SendMessage(ply, "Invalid player!", new[] { 255f, 0f, 0f });
        }

        public static void SpawnMob(CommandArgs args)
        {
            int x = args.PlayerX;
            int y = args.PlayerY;
            var msgargs = Regex.Split(args.Message, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            if (msgargs.Length >= 2 && msgargs.Length <= 3)
            {
                for (int i = 1; i < msgargs.Length; i++)
                    msgargs[i] = ((msgargs[i].TrimEnd('"')).TrimStart('"'));
                string inputtype = "";
                int amount = 1;
                int npcid = -1;
                int type = -1;
                inputtype = msgargs[1];
                if (msgargs.Length == 3)
                    int.TryParse(msgargs[2], out amount);

                if (!int.TryParse(inputtype, out type))
                    type = TShock.GetNPCID(inputtype);
                if (type >= 1 && type <= 45)
                {
                    for (int i = 0; i < amount; i++)
                        npcid = NPC.NewNPC(x, y, type, 0);
                    Tools.Broadcast(string.Format("{0} was spawned {1} time(s).", Main.npc[npcid].name, amount));
                    ;
                }
            }
            else
                Tools.SendMessage(args.PlayerID, "Invalid syntax! Proper syntax: /spawnmob <mob name/id> [amount]",
                                  new[] { 255f, 0f, 0f });
        }

        public static void Item(CommandArgs args)
        {
            var msgargs =
                Regex.Split(args.Message, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)")[1];
            int ply = args.PlayerID;
            bool flag = false;
            if (msgargs.Length >= 2)
            {
                msgargs = ((msgargs.TrimEnd('"')).TrimStart('"'));
                int type = 0;
                if (!int.TryParse(msgargs, out type))
                    type = TShock.GetItemID(msgargs);
                if (type >= 1 && type <= 238)
                {
                    for (int i = 0; i < 40; i++)
                    {
                        if (!Main.player[ply].inventory[i].active)
                        {
                            //Main.player[ply].inventory[i].SetDefaults(type);
                            //Main.player[ply].inventory[i].stack = Main.player[ply].inventory[i].maxStack;
                            int id = Terraria.Item.NewItem(0, 0, 0, 0, type, 1, true);
                            Main.item[id].position.X = args.PlayerX;
                            Main.item[id].position.Y = args.PlayerY;
                            Main.item[id].stack = Main.item[id].maxStack;
                            //TShock.SendDataAll(21, -1, "", id);
                            NetMessage.SendData(21, -1, -1, "", id, 0f, 0f, 0f);
                            Tools.SendMessage(ply, "Got some " + Main.item[id].name + ".");
                            //TShock.UpdateInventories();
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                        Tools.SendMessage(args.PlayerID, "You don't have free slots!", new[] { 255f, 0f, 0f });
                }
                else
                    Tools.SendMessage(args.PlayerID, "Invalid item type!", new[] { 255f, 0f, 0f });
            }
            else
                Tools.SendMessage(args.PlayerID, "Invalid syntax! Proper syntax: /item <item name/id>",
                                  new[] { 255f, 0f, 0f });
        }

        public static void Give(CommandArgs args)
        {
            var msgargs = Regex.Split(args.Message, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            int ply = args.PlayerID;
            bool flag = false;
            if (msgargs.Length == 3)
            {
                for (int i = 1; i < msgargs.Length; i++)
                    msgargs[i] = ((msgargs[i].TrimEnd('"')).TrimStart('"'));
                int type = 0;
                int player = -1;
                if (!int.TryParse(msgargs[1], out type))
                    type = TShock.GetItemID(msgargs[1]);
                if (type >= 1 && type <= 238)
                {
                    player = Tools.FindPlayer(msgargs[2]);
                    if (player != -1)
                    {
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
                                Tools.SendMessage(ply,
                                                  string.Format("Gave {0} some {1}.", msgargs[2], Main.item[id].name));
                                Tools.SendMessage(player,
                                                  string.Format("{0} gave you some {1}.", Tools.FindPlayer(ply),
                                                                Main.item[id].name));
                                //TShock.UpdateInventories();
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                            Tools.SendMessage(args.PlayerID, "Player does not have free slots!", new[] { 255f, 0f, 0f });
                    }
                    else
                        Tools.SendMessage(args.PlayerID, "Invalid player!", new[] { 255f, 0f, 0f });
                }
                else
                    Tools.SendMessage(args.PlayerID, "Invalid item type!", new[] { 255f, 0f, 0f });
            }
            else
                Tools.SendMessage(args.PlayerID, "Invalid syntax! Proper syntax: /give <item type/id> <player>",
                                  new[] { 255f, 0f, 0f });
        }

        public static void Heal(CommandArgs args)
        {
            int ply = args.PlayerID;
            int x = args.PlayerX;
            int y = args.PlayerY;
            var msgargs = Regex.Split(args.Message, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            int player = ply;
            if (msgargs.Length == 2)
                player = Tools.FindPlayer((msgargs[1].TrimEnd('"')).TrimStart('"'));
            if (player != ply && player >= 0)
            {
                Tools.SendMessage(ply, string.Format("You just healed {0}", (msgargs[1].TrimEnd('"')).TrimStart('"')));
                Tools.SendMessage(player, string.Format("{0} just healed you!", Tools.FindPlayer(ply)));
                x = (int)Main.player[player].position.X;
                y = (int)Main.player[player].position.Y;
            }
            else
                Tools.SendMessage(ply, "You just got healed!");
            for (int i = 0; i < 20; i++)
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
            int ply = args.PlayerID;
            int amount = Convert.ToInt32(args.Message.Remove(0, 10));
            int.TryParse(args.Message.Remove(0, 10), out amount);
            NPC.defaultMaxSpawns = amount;
            Tools.Broadcast(Tools.FindPlayer(ply) + " changed the maximum spawns to: " + amount);
        }

        public static void SpawnRate(CommandArgs args)
        {
            int ply = args.PlayerID;
            int amount = Convert.ToInt32(args.Message.Remove(0, 10));
            int.TryParse(args.Message.Remove(0, 10), out amount);
            NPC.defaultSpawnRate = amount;
            Tools.Broadcast(Tools.FindPlayer(ply) + " changed the spawn rate to: " + amount);
        }

        public static void Help(CommandArgs args)
        {
            int ply = args.PlayerID;
            Tools.SendMessage(ply, "TShock Commands:");
            string tempstring = "";
            int page = 1;
            if (args.Message.Split(' ').Length == 2)
                int.TryParse(args.Message.Split(' ')[1], out page);
            List<Command> cmdlist = new List<Command>();
            for (int j = 0; j < commands.Count; j++)
            {
                if (commands[j].CanRun(TShock.players[args.PlayerID]))
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
                        Tools.SendMessage(ply, tempstring.TrimEnd(new[] { ' ', ',' }), new[] { 255f, 255f, 0f });
                        break;
                    }
                    if ((j + 1) % 5 == 0)
                    {
                        Tools.SendMessage(ply, tempstring.TrimEnd(new[] { ' ', ',' }), new[] { 255f, 255f, 0f });
                        tempstring = "";
                    }
                }
            }
            if (cmdlist.Count > (15 * page))
            {
                Tools.SendMessage(ply, "Type /help " + (page + 1) + " for more commands.", new[] { 255f, 0f, 255f });
            }
        }

        public static void Time(CommandArgs args)
        {
            var arg = args.Message.Split(' ');
            if (arg.Length == 2)
            {
                if (arg[1] == "day")
                {
                    Main.time = 0;
                    Main.dayTime = true;
                    NetMessage.SendData(18, -1, -1, "", 0, 0, Main.sunModY, Main.moonModY);
                    NetMessage.syncPlayers();
                    Tools.Broadcast(Tools.FindPlayer(args.PlayerID) + " set time to day.");
                }
                else if (arg[1] == "night")
                {
                    Main.time = 0;
                    Main.dayTime = false;
                    NetMessage.SendData(18, -1, -1, "", 0, 0, Main.sunModY, Main.moonModY);
                    NetMessage.syncPlayers();
                    Tools.Broadcast(Tools.FindPlayer(args.PlayerID) + " set time to night.");
                }
                else if (arg[1] == "dusk")
                {
                    Main.dayTime = false;
                    Main.time = 0.0;
                    NetMessage.SendData(18, -1, -1, "", 0, 0, Main.sunModY, Main.moonModY);
                    NetMessage.syncPlayers();
                    Tools.Broadcast(Tools.FindPlayer(args.PlayerID) + " set time to dusk.");
                }
                else if (arg[1] == "noon")
                {
                    Main.dayTime = true;
                    Main.time = 27000.0;
                    NetMessage.SendData(18, -1, -1, "", 0, 0, Main.sunModY, Main.moonModY);
                    NetMessage.syncPlayers();
                    Tools.Broadcast(Tools.FindPlayer(args.PlayerID) + " set time to noon.");
                }
                else if (arg[1] == "midnight")
                {
                    Main.dayTime = false;
                    Main.time = 16200.0;
                    NetMessage.SendData(18, -1, -1, "", 0, 0, Main.sunModY, Main.moonModY);
                    NetMessage.syncPlayers();
                    Tools.Broadcast(Tools.FindPlayer(args.PlayerID) + " set time to midnight.");
                }
                else
                    Tools.SendMessage(args.PlayerID, "Invalid syntax! Proper syntax: /time <day/night/dusk/noon/midnight>",
                                      new[] { 255f, 0f, 0f });
            }
            else
                Tools.SendMessage(args.PlayerID, "Invalid syntax! Proper syntax: /time <day/night/dusk/noon/midnight>",
                                  new[] { 255f, 0f, 0f });
        }

        public static void Kill(CommandArgs args)
        {
            var msgargs = Regex.Split(args.Message, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            if (msgargs.Length == 2)
            {
                int player = -1;
                player = Tools.FindPlayer((msgargs[1].TrimEnd('"')).TrimStart('"'));
                Tools.SendMessage(args.PlayerID, "You just killed " + Tools.FindPlayer(player) + "!");
                Tools.SendMessage(player, Tools.FindPlayer(args.PlayerID) + " just killed you!");
                TShock.KillMe(player);
            }
        }

        public static void Slap(CommandArgs args)
        {
            var msgargs = Regex.Split(args.Message, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
            for (int i = 0; i < msgargs.Length; i++)
                msgargs[i] = (msgargs[i].TrimStart('"')).TrimEnd('"');
            if (msgargs.Length == 1)
                Tools.SendMessage(args.PlayerID, "Invalid syntax! Proper syntax: /slap <player> [dmg]",
                                  new[] { 255f, 0f, 0f });
            else if (msgargs.Length == 2)
            {
                int player = Tools.FindPlayer(msgargs[1]);
                if (player == -1)
                    Tools.SendMessage(args.PlayerID, "Invalid player!", new[] { 255f, 0f, 0f });
                else
                {
                    NetMessage.SendData(26, -1, -1, "", player, ((new Random()).Next(1, 20)), 5, (float)0);
                    Tools.Broadcast(Tools.FindPlayer(args.PlayerID) + " slapped " + Tools.FindPlayer(player) +
                                    " for 5 damage.");
                }
            }
            else if (msgargs.Length == 3)
            {
                int player = Tools.FindPlayer(msgargs[1]);
                int damage = 5;
                int.TryParse(msgargs[2], out damage);
                if (player == -1)
                    Tools.SendMessage(args.PlayerID, "Invalid player!", new[] { 255f, 0f, 0f });
                else
                {
                    NetMessage.SendData(26, -1, -1, "", player, ((new Random()).Next(-1, 1)), damage, (float)0);
                    Tools.Broadcast(Tools.FindPlayer(args.PlayerID) + " slapped " + Tools.FindPlayer(player) + " for " +
                                    damage + " damage.");
                }
            }
            else
                Tools.SendMessage(args.PlayerID, "Invalid syntax! Proper syntax: /slap <player> [dmg]",
                                  new[] { 255f, 0f, 0f });
        }

        public static void ProtectSpawn(CommandArgs args)
        {
            ConfigurationManager.spawnProtect = (ConfigurationManager.spawnProtect == false);
            Tools.SendMessage(args.PlayerID,
                              "Spawn is now " + (ConfigurationManager.spawnProtect ? "protected" : "open") + ".");
        }

        #endregion Command Methods
    }
}