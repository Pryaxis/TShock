/*
TShock, a server mod for Terraria
Copyright (C) 2011-2019 Pryaxis & TShock Contributors

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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using TShockAPI.DB;
using TerrariaApi.Server;
using TShockAPI.Hooks;
using Terraria.GameContent.Events;
using Microsoft.Xna.Framework;
using TShockAPI.Localization;
using System.Text.RegularExpressions;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;

namespace TShockAPI
{
	public delegate void CommandDelegate(CommandArgs args);

	public class CommandArgs : EventArgs
	{
		public string Message { get; private set; }
		public TSPlayer Player { get; private set; }
		public bool Silent { get; private set; }

		/// <summary>
		/// Parameters passed to the argument. Does not include the command name.
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
			Silent = false;
		}

		public CommandArgs(string message, bool silent, TSPlayer ply, List<string> args)
		{
			Message = message;
			Player = ply;
			Parameters = args;
			Silent = silent;
		}
	}

	public class Command
	{
		/// <summary>
		/// Gets or sets whether to allow non-players to use this command.
		/// </summary>
		public bool AllowServer { get; set; }
		/// <summary>
		/// Gets or sets whether to do logging of this command.
		/// </summary>
		public bool DoLog { get; set; }
		/// <summary>
		/// Gets or sets the help text of this command.
		/// </summary>
		public string HelpText { get; set; }
		/// <summary>
		/// Gets or sets an extended description of this command.
		/// </summary>

		public string Show { get; set; }
		public string[] HelpDesc { get; set; }
		/// <summary>
		/// Gets the name of the command.
		/// </summary>
		public string Name { get { return Names[0]; } }
		/// <summary>
		/// Gets the names of the command.
		/// </summary>
		public List<string> Names { get; protected set; }
		/// <summary>
		/// Gets the permissions of the command.
		/// </summary>
		public List<string> Permissions { get; protected set; }

		private CommandDelegate commandDelegate;
		public CommandDelegate CommandDelegate
		{
			get { return commandDelegate; }
			set
			{
				if (value == null)
					throw new ArgumentNullException();

				commandDelegate = value;
			}
		}

		public Command(List<string> permissions, CommandDelegate cmd, params string[] names)
			: this(cmd, names)
		{
			Permissions = permissions;
		}

		public Command(string permissions, CommandDelegate cmd, params string[] names)
			: this(cmd, names)
		{
			Permissions = new List<string> { permissions };
		}

		public Command(CommandDelegate cmd, params string[] names)
		{
			if (cmd == null)
				throw new ArgumentNullException("cmd");
			if (names == null || names.Length < 1)
				throw new ArgumentException("names");

			AllowServer = true;
			CommandDelegate = cmd;
			DoLog = true;
			HelpText = "没有可用的帮助.";
			HelpDesc = null;
			Names = new List<string>(names);
			Permissions = new List<string>();
			Show = null;
		}

		public bool Run(string msg, bool silent, TSPlayer ply, List<string> parms)
		{
			if (!CanRun(ply))
				return false;

			try
			{
				CommandDelegate(new CommandArgs(msg, silent, ply, parms));
			}
			catch (Exception e)
			{
				ply.SendErrorMessage("指令执行失败,请查找日志获得更多信息.");
				TShock.Log.Error(e.ToString());
			}

			return true;
		}

		public bool Run(string msg, TSPlayer ply, List<string> parms)
		{
			return Run(msg, false, ply, parms);
		}

		public bool HasAlias(string name)
		{
			return Names.Contains(name);
		}

		public bool CanRun(TSPlayer ply)
		{
			if (Permissions == null || Permissions.Count < 1)
				return true;
			foreach (var Permission in Permissions)
			{
				if (ply.HasPermission(Permission))
					return true;
			}
			return false;
		}
	}

	public static class Commands
	{
		public static List<Command> ChatCommands = new List<Command>();
		public static ReadOnlyCollection<Command> TShockCommands = new ReadOnlyCollection<Command>(new List<Command>());

		/// <summary>
		/// The command specifier, defaults to "/"
		/// </summary>
		public static string Specifier
		{
			get { return string.IsNullOrWhiteSpace(TShock.Config.Settings.CommandSpecifier) ? "/" : TShock.Config.Settings.CommandSpecifier; }
		}

		/// <summary>
		/// The silent command specifier, defaults to "."
		/// </summary>
		public static string SilentSpecifier
		{
			get { return string.IsNullOrWhiteSpace(TShock.Config.Settings.CommandSilentSpecifier) ? "." : TShock.Config.Settings.CommandSilentSpecifier; }
		}

		private delegate void AddChatCommand(string permission, CommandDelegate command, params string[] names);

		public static void InitCommands()
		{
			List<Command> tshockCommands = new List<Command>(100);
			Action<Command> add = (cmd) =>
			{
				tshockCommands.Add(cmd);
				ChatCommands.Add(cmd);
			};

			add(new Command(SetupToken, "setup")
			{
				AllowServer = false,
				HelpText = "用于在首次设置TShock时验证身份成为超级管理员.",
				Show = "验证超级管理员"
			});
			add(new Command(Permissions.user, ManageUsers, "user")
			{
				Show = "账号管理",
				DoLog = false,
			});

			#region Account Commands
			add(new Command(Permissions.canlogin, AttemptLogin, "login")
			{
				Show = "登录",
				AllowServer = false,
				DoLog = false,
				HelpText = "登录你的账号."
			});
			add(new Command(Permissions.canlogout, Logout, "logout")
			{
				Show = "登出",
				AllowServer = false,
				DoLog = false,
				HelpText = "登出你的账号."
			});
			add(new Command(Permissions.canchangepassword, PasswordUser, "password")
			{
				Show = "更改密码",
				AllowServer = false,
				DoLog = false,
				HelpText = "更改你的账号密码."
			});
			add(new Command(Permissions.canregister, RegisterUser, "register")
			{
				Show = "注册",
				AllowServer = false,
				DoLog = false,
				HelpText = "注册一个账号."
			});
			add(new Command(Permissions.checkaccountinfo, ViewAccountInfo, "accountinfo", "ai")
			{
				Show = "用户信息",
				HelpText = "查看一个用户的账号信息."
			});
			#endregion
			#region Admin Commands
			add(new Command(Permissions.ban, Ban, "ban")
			{
				Show = "封禁",
				HelpText = "管理用户封禁."
			});
			add(new Command(Permissions.broadcast, Broadcast, "broadcast", "bc", "say")
			{
				Show = "广播",
				HelpText = "在服务器中发送广播."
			});
			add(new Command(Permissions.logs, DisplayLogs, "displaylogs")
			{
				Show = "显示日志",
				HelpText = "切换是否向你显示服务器日志."
			});
			add(new Command(Permissions.managegroup, Group, "group")
			{
				Show = "组管理",
				HelpText = "管理组."
			});
			add(new Command(Permissions.manageitem, ItemBan, "itemban")
			{
				Show = "禁用物品",
				HelpText = "管理物品禁用."
			});
			add(new Command(Permissions.manageprojectile, ProjectileBan, "projban")
			{
				Show = "禁用射弹",
				HelpText = "管理射弹禁用."
			});
			add(new Command(Permissions.managetile, TileBan, "tileban")
			{
				Show = "禁用图格",
				HelpText = "管理图格禁用."
			});
			add(new Command(Permissions.manageregion, Region, "region")
			{
				Show = "区域管理",
				HelpText = "管理区域."
			});
			add(new Command(Permissions.kick, Kick, "kick")
			{
				Show = "踢",
				HelpText = "从服务器中踢出指定玩家."
			});
			add(new Command(Permissions.mute, Mute, "mute", "unmute")
			{
				Show = "禁言",
				HelpText = "禁言指定玩家."
			});
			add(new Command(Permissions.savessc, OverrideSSC, "overridessc", "ossc")
			{
				Show = "覆盖SSC",
				HelpText = "暂时覆盖玩家的SSC云存档."
			});
			add(new Command(Permissions.savessc, SaveSSC, "savessc")
			{
				Show = "保存SSC",
				HelpText = "保存玩家的SSC云存档."
			});
			add(new Command(Permissions.uploaddata, UploadJoinData, "uploadssc")
			{
				Show = "上传SSC",
				HelpText = "上传本地角色存档数据作为服务器SSC云存档."
			});
			add(new Command(Permissions.settempgroup, TempGroup, "tempgroup")
			{
				Show = "临时组",
				HelpText = "设置其他玩家的临时组."
			});
			add(new Command(Permissions.su, SubstituteUser, "su")
			{
				Show = "临时权限",
				HelpText = "获取10分钟的临时超级管理员权限."
			});
			add(new Command(Permissions.su, SubstituteUserDo, "sudo")
			{
				Show = "超管执行",
				HelpText = "以超级管理员权限执行一个命令."
			});
			add(new Command(Permissions.userinfo, GrabUserUserInfo, "userinfo", "ui")
			{
				Show = "玩家信息",
				HelpText = "查看一个玩家的信息."
			});
			#endregion
			#region Annoy Commands
			add(new Command(Permissions.annoy, Annoy, "annoy")
			{
				Show = "骚扰",
				HelpText = "骚扰一个玩家一段时间(发出奇怪的响声)."
			});
			add(new Command(Permissions.annoy, Rocket, "rocket")
			{
				Show = "上天",
				HelpText = "让玩家上天.需要开启SSC云存档"
			});
			add(new Command(Permissions.annoy, FireWork, "firework")
			{
				Show = "烟花",
				HelpText = "在玩家面前生成一个烟花."
			});
			#endregion
			#region Configuration Commands
			add(new Command(Permissions.maintenance, CheckUpdates, "checkupdates")
			{
				Show = "检查更新",
				HelpText = "检查TShock更新."
			});
			add(new Command(Permissions.maintenance, Off, "off", "exit", "stop")
			{
				Show = "关服并保存",
				HelpText = "关闭服务器并保存地图."
			});
			add(new Command(Permissions.maintenance, OffNoSave, "off-nosave", "exit-nosave", "stop-nosave")
			{
				Show = "关服不保存",
				HelpText = "关闭服务器但不保存地图."
			});
			add(new Command(Permissions.cfgreload, Reload, "reload")
			{
				Show = "重载配置",
				HelpText = "重读服务器配置(部分插件也会重读)."
			});
			add(new Command(Permissions.cfgpassword, ServerPassword, "serverpassword")
			{
				Show = "更改进服密码",
				HelpText = "更改加入服务器的密码."
			});
			add(new Command(Permissions.maintenance, GetVersion, "version")
			{
				Show = "版本",
				HelpText = "查看TShock版本."
			});
			add(new Command(Permissions.whitelist, Whitelist, "whitelist")
			{
				Show = "拉跨白名单",
				HelpText = "管理服务器很拉跨的白名单."
			});
			#endregion
			#region Item Commands
			add(new Command(Permissions.give, Give,"give", "g")
			{
				Show = "给物品",
				HelpText = "给予指定玩家指定物品."
			});
			add(new Command(Permissions.item, Item, "item", "i")
			{
				Show = "给自己物品",
				AllowServer = false,
				HelpText = "给予你自己指定物品."
			});
			#endregion
			#region NPC Commands
			add(new Command(Permissions.butcher, Butcher, "butcher")
			{
				Show = "清除NPC",
				HelpText = "杀死敌对NPC或某种类型的NPC."
			});
			add(new Command(Permissions.renamenpc, RenameNPC, "renamenpc")
			{
				Show = "重命名城镇NPC",
				HelpText = "重命名一个城镇NPC."
			});
			add(new Command(Permissions.maxspawns, MaxSpawns, "maxspawns")
			{
				Show = "NPC最大生成量",
				HelpText = "设置NPC最大生成量."
			});
			add(new Command(Permissions.spawnboss, SpawnBoss, "spawnboss", "sb")
			{
				Show = "召唤BOSS",
				AllowServer = false,
				HelpText = "在你周围生成指定数量的指定BOSS(/sb all则生成全部)."
			});
			add(new Command(Permissions.spawnmob, SpawnMob, "spawnmob", "sm")
			{
				Show = "召唤NPC",
				AllowServer = false,
				HelpText = "在你周围生成指定数量的NPC."
			});
			add(new Command(Permissions.spawnrate, SpawnRate, "spawnrate")
			{
				Show = "NPC生成速率",
				HelpText = "设置NPC的生成速率."
			});
			add(new Command(Permissions.clearangler, ClearAnglerQuests, "clearangler")
			{
				Show = "重置渔夫任务",
				HelpText = "重置当天渔夫的任务."
			});
			#endregion
			#region TP Commands
			add(new Command(Permissions.home, Home, "home")
			{
				Show = "回家",
				AllowServer = false,
				HelpText = "送你回到出生点."
			});
			add(new Command(Permissions.spawn, Spawn, "spawn")
			{
				Show = "传送至世界出生点",
				AllowServer = false,
				HelpText = "送你回到世界出生点."
			});
			add(new Command(Permissions.tp, TP, "tp")
			{
				Show = "传送",
				AllowServer = false,
				HelpText = "传送至指定玩家身边或将指定玩家传送至另一玩家身边."
			});
			add(new Command(Permissions.tpothers, TPHere, "tphere")
			{
				Show = "传送玩家至身边",
				AllowServer = false,
				HelpText = "将指定玩家传送至你身边."
			});
			add(new Command(Permissions.tpnpc, TPNpc, "tpnpc")
			{
				Show = "传送至NPC",
				AllowServer = false,
				HelpText = "传送至指定NPC身边."
			});
			add(new Command(Permissions.tppos, TPPos, "tppos")
			{
				Show = "传送至坐标",
				AllowServer = false,
				HelpText = "传送至指定坐标位置."
			});
			add(new Command(Permissions.getpos, GetPos, "pos")
			{
				Show = "显示坐标",
				AllowServer = false,
				HelpText = "显示你当前所在的坐标."
			});
			add(new Command(Permissions.tpallow, TPAllow, "tpallow")
			{
				Show = "传送保护",
				AllowServer = false,
				HelpText = "切换是否允许其他玩家对你使用传送."
			});
			#endregion
			#region World Commands
			add(new Command(Permissions.toggleexpert, ChangeWorldMode, "worldmode", "gamemode")
			{
				Show = "更改世界模式",
				HelpText = "改变世界模式."
			});
			add(new Command(Permissions.antibuild, ToggleAntiBuild, "antibuild")
			{
				Show = "建筑保护",
				HelpText = "切换建筑保护."
			});
			add(new Command(Permissions.grow, Grow, "grow")
			{
				Show = "生成植物",
				AllowServer = false,
				HelpText = "在脚下生成指定植物."
			});
			add(new Command(Permissions.halloween, ForceHalloween, "forcehalloween")
			{
				Show = "强制万圣节",
				HelpText = "强制开启万圣节模式(糖果袋、南瓜等)."
			});
			add(new Command(Permissions.xmas, ForceXmas, "forcexmas")
			{
				Show = "强制圣诞节",
				HelpText = "强制开启圣诞节模式(圣诞老人等)."
			});
			add(new Command(Permissions.manageevents, ManageWorldEvent, "worldevent")
			{
				Show = "世界事件",
				HelpText = "启动和停止各种世界事件."
			});
			add(new Command(Permissions.hardmode, Hardmode, "hardmode")
			{
				Show = "切换困难模式",
				HelpText = "切换世界困难模式."
			});
			add(new Command(Permissions.editspawn, ProtectSpawn, "protectspawn")
			{
				Show = "出生点保护",
				HelpText = "切换出生点保护."
			});
			add(new Command(Permissions.worldsave, Save, "save")
			{
				Show = "保存世界",
				HelpText = "保存世界."
			});
			add(new Command(Permissions.worldspawn, SetSpawn, "setspawn")
			{
				Show = "设置世界出生点",
				AllowServer = false,
				HelpText = "设置世界出生点为当前位置."
			});
			add(new Command(Permissions.dungeonposition, SetDungeon, "setdungeon")
			{
				Show = "设置地牢",
				AllowServer = false,
				HelpText = "设置世界地牢位置(地牢老人刷新点)为当前位置."
			});
			add(new Command(Permissions.worldsettle, Settle, "settle")
			{
				Show = "安置液体",
				HelpText = "用户可以强制更新液体状态."
			});
			add(new Command(Permissions.time, Time, "time")
			{
				Show = "时间",
				HelpText = "查看和设置世界时间."
			});
			add(new Command(Permissions.wind, Wind, "wind")
			{
				Show = "风速",
				HelpText = "改变世界的风速."
			});
			add(new Command(Permissions.worldinfo, WorldInfo, "worldinfo")
			{
				Show = "地图信息",
				HelpText = "查看世界详细信息."
			});
			#endregion
			#region Other Commands
			add(new Command(Permissions.buff, Buff, "buff")
			{
				Show = "给自己Buff",
				AllowServer = false,
				HelpText = "给自己指定时间的指定BUFF."
			});
			add(new Command(Permissions.clear, Clear, "clear")
			{
				Show = "清理",
				HelpText = "清除世界范围内的掉落物、射弹、NPC."
			});
			add(new Command(Permissions.buffplayer, GBuff, "gbuff", "buffplayer")
			{
				Show = "给Buff",
				HelpText = "给其他玩家一段时间的BUFF或去除BUFF."
			});
			add(new Command(Permissions.godmode, ToggleGodMode, "godmode", "god")
			{
				Show = "上帝模式",
				HelpText = "为自己或其他玩家开启上帝模式."
			});
			add(new Command(Permissions.heal, Heal, "heal")
			{
				Show = "恢复",
				HelpText = "恢复一个玩家指定的HP或MP值."
			});
			add(new Command(Permissions.kill, Kill, "kill", "slay")
			{
				Show = "杀死",
				HelpText = "杀死指定玩家."
			});
			add(new Command(Permissions.cantalkinthird, ThirdPerson, "me" ,"发送")
			{
				Show = "发送消息",
				HelpText = "向所有人发送信息."
			});
			add(new Command(Permissions.canpartychat, PartyChat, "party", "p")
			{
				Show = "队内消息",
				AllowServer = false,
				HelpText = "发送队伍消息."
			});
			add(new Command(Permissions.whisper, Reply, "reply", "r")
			{
				Show = "回复私聊",
				HelpText = "回复一条私聊消息."
			});
			add(new Command(Rests.RestPermissions.restmanage, ManageRest, "rest")
			{
				Show = "设置Rest",
				HelpText = "管理临时RestAPI."
			});
			add(new Command(Permissions.slap, Slap, "slap")
			{
				Show = "扇巴掌",
				HelpText = "扇指定玩家巴掌,造成指定伤害."
			});
			add(new Command(Permissions.serverinfo, ServerInfo, "serverinfo")
			{
				Show = "服务器信息",
				HelpText = "查看服务器详细信息."
			});
			add(new Command(Permissions.warp, Warp, "warp")
			{
				Show = "传送点",
				HelpText = "传送至一个传送点."
			});
			add(new Command(Permissions.whisper, Whisper, "whisper", "w", "tell", "pm", "dm")
			{
				Show = "发送私聊",
				HelpText = "发送一个私聊给指定玩家."
			});
			add(new Command(Permissions.whisper, Wallow, "wallow", "wa")
			{
				Show = "私聊保护",
				AllowServer = false,
				HelpText = "切换为忽略或接收其他玩家的私聊."
			});
			add(new Command(Permissions.createdumps, CreateDumps, "dump-reference-data")
			{
				Show = "生成服务器帮助文档",
				HelpText = "生成服务器帮助文档."
			});
			add(new Command(Permissions.synclocalarea, SyncLocalArea, "sync")
			{
				Show = "同步图格",
				HelpText = "使客户端与服务器世界重新同步."
			});
			add(new Command(Permissions.respawn, Respawn, "respawn")
			{
				Show = "复活",
				HelpText = "让自己或其他玩家重生."
			});
			#endregion

			add(new Command(Aliases, "aliases")
			{
				Show = "指令别名",
				HelpText = "显示命令的别名."
			});
			add(new Command(Help, "help", "帮助")
			{
				Show = "指令清单",
				HelpText = "显示指令清单."
			});
			add(new Command(Motd, "motd")
			{
				Show = "进服信息",
				HelpText = "查看设置的进服消息."
			});
			add(new Command(ListConnectedPlayers, "playing", "online", "who")
			{
				Show = "在线玩家",
				HelpText = "查看在线玩家."
			});
			add(new Command(Rules, "rules")
			{
				Show = "规则",
				HelpText = "查看服务器的规则."
			});

			TShockCommands = new ReadOnlyCollection<Command>(tshockCommands);
		}

		public static bool HandleCommand(TSPlayer player, string text)
		{
			string cmdText = text.Remove(0, 1);
			string cmdPrefix = text[0].ToString();
			bool silent = false;

			if (cmdPrefix == SilentSpecifier)
				silent = true;

			int index = -1;
			for (int i = 0; i < cmdText.Length; i++)
			{
				if (IsWhiteSpace(cmdText[i]))
				{
					index = i;
					break;
				}
			}
			string cmdName;
			if (index == 0) // Space after the command specifier should not be supported
			{
				player.SendErrorMessage("没有找到这个指令呐. 试试用 {0}help 获取指令清单吧！", Specifier);
				return true;
			}
			else if (index < 0)
				cmdName = cmdText.ToLower();
			else
				cmdName = cmdText.Substring(0, index).ToLower();

			List<string> args;
			if (index < 0)
				args = new List<string>();
			else
				args = ParseParameters(cmdText.Substring(index));

			IEnumerable<Command> cmds = ChatCommands.FindAll(c => c.HasAlias(cmdName));

			if (Hooks.PlayerHooks.OnPlayerCommand(player, cmdName, cmdText, args, ref cmds, cmdPrefix))
				return true;

			if (cmds.Count() == 0)
			{
				if (player.AwaitingResponse.ContainsKey(cmdName))
				{
					Action<CommandArgs> call = player.AwaitingResponse[cmdName];
					player.AwaitingResponse.Remove(cmdName);
					call(new CommandArgs(cmdText, player, args));
					return true;
				}
				player.SendErrorMessage("没有找到这个指令呐. 试试用 {0}help 获取指令清单吧！", Specifier);
				return true;
			}
			foreach (Command cmd in cmds)
			{
				if (!cmd.CanRun(player))
				{
					TShock.Utils.SendLogs(string.Format("{0} 试图执行 {1}{2}.", player.Name, Specifier, cmdText), Color.PaleVioletRed, player);
					player.SendErrorMessage("你没有权限使用这个命令哦.");
					if (player.HasPermission(Permissions.su))
					{
						player.SendInfoMessage("你可以使用 '{0}sudo {0}{1}' 跳过权限检查.", Specifier, cmdText);
					}
				}
				else if (!cmd.AllowServer && !player.RealPlayer)
				{
					player.SendErrorMessage("你必须在游戏中使用该指令.");
				}
				else
				{
					if (cmd.DoLog)
						TShock.Utils.SendLogs(string.Format("{0} 执行: {1}{2}.", player.Name, silent ? SilentSpecifier : Specifier, cmdText), Color.PaleVioletRed, player);
					cmd.Run(cmdText, silent, player, args);
				}
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

				if (c == '\\' && ++i < str.Length)
				{
					if (str[i] != '"' && str[i] != ' ' && str[i] != '\\')
						sb.Append('\\');
					sb.Append(str[i]);
				}
				else if (c == '"')
				{
					instr = !instr;
					if (!instr)
					{
						ret.Add(sb.ToString());
						sb.Clear();
					}
					else if (sb.Length > 0)
					{
						ret.Add(sb.ToString());
						sb.Clear();
					}
				}
				else if (IsWhiteSpace(c) && !instr)
				{
					if (sb.Length > 0)
					{
						ret.Add(sb.ToString());
						sb.Clear();
					}
				}
				else
					sb.Append(c);
			}
			if (sb.Length > 0)
				ret.Add(sb.ToString());

			return ret;
		}

		private static bool IsWhiteSpace(char c)
		{
			return c == ' ' || c == '\t' || c == '\n';
		}

		#region Account commands

		private static void AttemptLogin(CommandArgs args)
		{
			if (args.Player.LoginAttempts > TShock.Config.Settings.MaximumLoginAttempts && (TShock.Config.Settings.MaximumLoginAttempts != -1))
			{
				TShock.Log.Warn(String.Format("{0}({1}) 登录失败 {2} 次或以上, 已被踢出服务器.",
					args.Player.IP, args.Player.Name, TShock.Config.Settings.MaximumLoginAttempts));
				args.Player.Kick("尝试登录次数超过上限.");
				return;
			}

			if (args.Player.IsLoggedIn)
			{
				args.Player.SendErrorMessage("你已经登录了, 不能重复登录哦.");
				return;
			}

			UserAccount account = TShock.UserAccounts.GetUserAccountByName(args.Player.Name);
			string password = "";
			bool usingUUID = false;
			if (args.Parameters.Count == 0 && !TShock.Config.Settings.DisableUUIDLogin)
			{
				if (PlayerHooks.OnPlayerPreLogin(args.Player, args.Player.Name, ""))
					return;
				usingUUID = true;
			}
			else if (args.Parameters.Count == 1)
			{
				if (PlayerHooks.OnPlayerPreLogin(args.Player, args.Player.Name, args.Parameters[0]))
					return;
				password = args.Parameters[0];
			}
			else if (args.Parameters.Count == 2 && TShock.Config.Settings.AllowLoginAnyUsername)
			{
				if (String.IsNullOrEmpty(args.Parameters[0]))
				{
					args.Player.SendErrorMessage("登录时出错(密码为空值).");
					return;
				}

				if (PlayerHooks.OnPlayerPreLogin(args.Player, args.Parameters[0], args.Parameters[1]))
					return;

				account = TShock.UserAccounts.GetUserAccountByName(args.Parameters[0]);
				password = args.Parameters[1];
			}
			else
			{
				if (!TShock.Config.Settings.DisableUUIDLogin)
					args.Player.SendMessage($"{Specifier}login - 以你的UUID和角色名作为凭证登录.", Color.White);

				if (TShock.Config.Settings.AllowLoginAnyUsername)
					args.Player.SendMessage($"{Specifier}login {"用户名".Color(Utils.GreenHighlight)} {"密码".Color(Utils.BoldHighlight)} - 以你的用户名和密码作为凭证登录.", Color.White);
				else
					args.Player.SendMessage($"{Specifier}login {"password".Color(Utils.BoldHighlight)} - 以你的角色名和密码作为凭证登录.", Color.White);

				args.Player.SendWarningMessage("如果你忘记了你的密码, 只能找服务器管理员修改.");
				return;
			}
			try
			{
				if (account == null)
				{
					args.Player.SendErrorMessage("该用户名用户不存在!");
				}
				else if (account.VerifyPassword(password) ||
						(usingUUID && account.UUID == args.Player.UUID && !TShock.Config.Settings.DisableUUIDLogin &&
						!String.IsNullOrWhiteSpace(args.Player.UUID)))
				{
					var group = TShock.Groups.GetGroupByName(account.Group);

					if (!TShock.Groups.AssertGroupValid(args.Player, group, false))
					{
						args.Player.SendErrorMessage("登录失败-请参阅上面的消息.");
						return;
					}

					args.Player.PlayerData = TShock.CharacterDB.GetPlayerData(args.Player, account.ID);

					args.Player.Group = group;
					args.Player.tempGroup = null;
					args.Player.Account = account;
					args.Player.IsLoggedIn = true;
					args.Player.IsDisabledForSSC = false;

					if (Main.ServerSideCharacter)
					{
						if (args.Player.HasPermission(Permissions.bypassssc))
						{
							args.Player.PlayerData.CopyCharacter(args.Player);
							TShock.CharacterDB.InsertPlayerData(args.Player);
						}
						args.Player.PlayerData.RestoreCharacter(args.Player);
					}
					args.Player.LoginFailsBySsi = false;

					if (args.Player.HasPermission(Permissions.ignorestackhackdetection))
						args.Player.IsDisabledForStackDetection = false;

					if (args.Player.HasPermission(Permissions.usebanneditem))
						args.Player.IsDisabledForBannedWearable = false;

					args.Player.SendSuccessMessage("登录用户 " + account.Name + " 成功.");

					TShock.Log.ConsoleInfo(args.Player.Name + " 登录为用户: " + account.Name + " 成功.");
					if ((args.Player.LoginHarassed) && (TShock.Config.Settings.RememberLeavePos))
					{
						if (TShock.RememberedPos.GetLeavePos(args.Player.Name, args.Player.IP) != Vector2.Zero)
						{
							Vector2 pos = TShock.RememberedPos.GetLeavePos(args.Player.Name, args.Player.IP);
							args.Player.Teleport((int)pos.X * 16, (int)pos.Y * 16);
						}
						args.Player.LoginHarassed = false;

					}
					TShock.UserAccounts.SetUserAccountUUID(account, args.Player.UUID);

					Hooks.PlayerHooks.OnPlayerPostLogin(args.Player);
				}
				else
				{
					if (usingUUID && !TShock.Config.Settings.DisableUUIDLogin)
					{
						args.Player.SendErrorMessage("服务器记录的UUID与你的UUID不匹配!");
					}
					else
					{
						args.Player.SendErrorMessage("密码错误!");
					}
					TShock.Log.Warn(args.Player.IP + " 登录用户: " + account.Name + "失败.");
					args.Player.LoginAttempts++;
				}
			}
			catch (Exception ex)
			{
				args.Player.SendErrorMessage("处理你的请求时出错.");
				TShock.Log.Error(ex.ToString());
			}
		}

		private static void Logout(CommandArgs args)
		{
			if (!args.Player.IsLoggedIn)
			{
				args.Player.SendErrorMessage("你还没有登录哦.");
				return;
			}

			if (args.Player.TPlayer.talkNPC != -1)
			{
				args.Player.SendErrorMessage("请关闭NPC对话窗口再登出.");
				return;
			}

			args.Player.Logout();
			args.Player.SendSuccessMessage("你已成功登出账号.");
			if (Main.ServerSideCharacter)
			{
				args.Player.SendWarningMessage("服务器SSC云存档已开启, 你需要登录才能正常游玩.");
			}
		}

		private static void PasswordUser(CommandArgs args)
		{
			try
			{
				if (args.Player.IsLoggedIn && args.Parameters.Count == 2)
				{
					string password = args.Parameters[0];
					if (args.Player.Account.VerifyPassword(password))
					{
						try
						{
							args.Player.SendSuccessMessage("密码更改成功!");
							TShock.UserAccounts.SetUserAccountPassword(args.Player.Account, args.Parameters[1]); // SetUserPassword will hash it for you.
							TShock.Log.ConsoleInfo(args.Player.IP + $"({args.Player.Name})" + " 成功更改用户 " +
												   args.Player.Account.Name + " 的密码.");
						}
						catch (ArgumentOutOfRangeException)
						{
							args.Player.SendErrorMessage("密码长度必须大于或等于 " + TShock.Config.Settings.MinimumPasswordLength + " 个字符.");
						}
					}
					else
					{
						args.Player.SendErrorMessage("密码更改失败!");
						TShock.Log.ConsoleError(args.Player.IP + $"({args.Player.Name})" + " 更改用户 " +
												args.Player.Account.Name + " 的密码失败.");
					}
				}
				else
				{
					args.Player.SendErrorMessage("未登录或格式无效! 正确格式: {0}password <旧密码> <新密码>", Specifier);
				}
			}
			catch (UserAccountManagerException ex)
			{
				args.Player.SendErrorMessage("抱歉, 发生了一个错误: " + ex.Message + ".");
				TShock.Log.ConsoleError("PasswordUser 返回了一个错误: " + ex);
			}
		}

		private static void RegisterUser(CommandArgs args)
		{
			try
			{
				var account = new UserAccount();
				string echoPassword = "";
				if (args.Parameters.Count == 1)
				{
					account.Name = args.Player.Name;
					echoPassword = args.Parameters[0];
					try
					{
						account.CreateBCryptHash(args.Parameters[0]);
					}
					catch (ArgumentOutOfRangeException)
					{
						args.Player.SendErrorMessage("密码长度必须大于或等于 " + TShock.Config.Settings.MinimumPasswordLength + " 个字符.");
						return;
					}
				}
				else if (args.Parameters.Count == 2 && TShock.Config.Settings.AllowRegisterAnyUsername)
				{
					account.Name = args.Parameters[0];
					echoPassword = args.Parameters[1];
					try
					{
						account.CreateBCryptHash(args.Parameters[1]);
					}
					catch (ArgumentOutOfRangeException)
					{
						args.Player.SendErrorMessage("密码长度必须大于或等于 " + TShock.Config.Settings.MinimumPasswordLength + " 个字符.");
						return;
					}
				}
				else
				{
					args.Player.SendErrorMessage("格式错误! 正确格式: {0}register <密码>", Specifier);
					return;
				}

				account.Group = TShock.Config.Settings.DefaultRegistrationGroupName; // FIXME -- we should get this from the DB. --Why?
				account.UUID = args.Player.UUID;

				if (TShock.UserAccounts.GetUserAccountByName(account.Name) == null && account.Name != TSServerPlayer.AccountName) // Cheap way of checking for existance of a user
				{
					args.Player.SendSuccessMessage("账号 \"{0}\" 注册成功.", account.Name);
					args.Player.SendSuccessMessage("你的密码为 {0}.", echoPassword);
					args.Player.SendWarningMessage("请牢记你的密码");
					if (!TShock.Config.Settings.DisableUUIDLogin)
						args.Player.SendMessage($"输入 {Specifier}login 以你的UUID作为凭证登录.", Color.White);

					if (TShock.Config.Settings.AllowLoginAnyUsername)
						args.Player.SendMessage($"输入 {Specifier}login \"{account.Name.Color(Utils.GreenHighlight)}\" {echoPassword.Color(Utils.BoldHighlight)} 登录.", Color.White);
					else
						args.Player.SendMessage($"输入 {Specifier}login {echoPassword.Color(Utils.BoldHighlight)} 登录.", Color.White);
					
					TShock.UserAccounts.AddUserAccount(account);
					TShock.Log.ConsoleInfo("{0} 成功注册账号: \"{1}\".", args.Player.Name, account.Name);
				}
				else
				{
					args.Player.SendErrorMessage("抱歉, 账号 " + account.Name + " 已经被其它人注册.");
					args.Player.SendErrorMessage("请使用其他用户名.");
					TShock.Log.ConsoleInfo(args.Player.Name + " 注册账号: " + account.Name + " 失败");
				}
			}
			catch (UserAccountManagerException ex)
			{
				args.Player.SendErrorMessage("抱歉, 发生了一个错误: " + ex.Message + ".");
				TShock.Log.ConsoleError("RegisterUser 返回了一个错误: " + ex);
			}
		}

		private static void ManageUsers(CommandArgs args)
		{
			// This guy needs to be here so that people don't get exceptions when they type /user
			if (args.Parameters.Count < 1)
			{
				args.Player.SendErrorMessage("无效的 'user' 用法. 试试用 {0}user help 来获取帮助吧.", Specifier);
				return;
			}

			string subcmd = args.Parameters[0];

			// Add requires a username, password, and a group specified.
			if (subcmd == "add" && args.Parameters.Count == 4)
			{
				var account = new UserAccount();

				account.Name = args.Parameters[1];
				try
				{
					account.CreateBCryptHash(args.Parameters[2]);
				}
				catch (ArgumentOutOfRangeException)
				{
					args.Player.SendErrorMessage("密码长度必须大于或等于 " + TShock.Config.Settings.MinimumPasswordLength + " 个字符.");
					return;
				}
				account.Group = args.Parameters[3];

				try
				{
					TShock.UserAccounts.AddUserAccount(account);
					args.Player.SendSuccessMessage("已将账号 " + account.Name + " 添加至组 " + account.Group + "!");
					TShock.Log.ConsoleInfo(args.Player.Name + " 将账号 " + account.Name + " 添加至组 " + account.Group + "!");
				}
				catch (GroupNotExistsException)
				{
					args.Player.SendErrorMessage("没有找到名为 " + account.Group + " 的组!");
				}
				catch (UserAccountExistsException)
				{
					args.Player.SendErrorMessage("用户 " + account.Name + " 已存在!");
				}
				catch (UserAccountManagerException e)
				{
					args.Player.SendErrorMessage("用户 " + account.Name + " 添加失败, 请查看控制台获取详细信息.");
					TShock.Log.ConsoleError(e.ToString());
				}
			}
			// User deletion requires a username
			else if (subcmd == "del" && args.Parameters.Count == 2)
			{
				var account = new UserAccount();
				account.Name = args.Parameters[1];

				try
				{
					TShock.UserAccounts.RemoveUserAccount(account);
					args.Player.SendSuccessMessage("用户移除成功.");
					TShock.Log.ConsoleInfo(args.Player.Name + " 成功移除账号 " + args.Parameters[1] + ".");
				}
				catch (UserAccountNotExistException)
				{
					args.Player.SendErrorMessage("没有找到名为 " + account.Name + " 的账号! 删除了个寂寞!");
				}
				catch (UserAccountManagerException ex)
				{
					args.Player.SendErrorMessage(ex.Message);
					TShock.Log.ConsoleError(ex.ToString());
				}
			}

			// Password changing requires a username, and a new password to set
			else if (subcmd == "password" && args.Parameters.Count == 3)
			{
				var account = new UserAccount();
				account.Name = args.Parameters[1];

				try
				{
					TShock.UserAccounts.SetUserAccountPassword(account, args.Parameters[2]);
					TShock.Log.ConsoleInfo(args.Player.Name + " 成功更改用户 " + account.Name + " 的密码");
					args.Player.SendSuccessMessage("成功更改用户 " + account.Name + " 的密码.");
				}
				catch (UserAccountNotExistException)
				{
					args.Player.SendErrorMessage("没有找到名为 " + account.Name + " 的用户!");
				}
				catch (UserAccountManagerException e)
				{
					args.Player.SendErrorMessage("修改用户 " + account.Name + " 的密码失败! 请查看控制台获取详细信息.!");
					TShock.Log.ConsoleError(e.ToString());
				}
				catch (ArgumentOutOfRangeException)
				{
					args.Player.SendErrorMessage("密码长度必须大于或等于 " + TShock.Config.Settings.MinimumPasswordLength + " 个字符.");
				}
			}
			// Group changing requires a username or IP address, and a new group to set
			else if (subcmd == "group" && args.Parameters.Count == 3)
			{
				var account = new UserAccount();
				account.Name = args.Parameters[1];

				try
				{
					TShock.UserAccounts.SetUserGroup(account, args.Parameters[2]);
					TShock.Log.ConsoleInfo(args.Player.Name + " 成功将用户 " + account.Name + " 移至组 " + args.Parameters[2] + ".");
					args.Player.SendSuccessMessage("用户 " + account.Name + " 成功被移至组 " + args.Parameters[2] + "!");
					
					//send message to player with matching account name
					var player = TShock.Players.FirstOrDefault(p => p != null && p.Account?.Name == account.Name);
					if (player != null && !args.Silent)
						player.SendSuccessMessage($"{args.Player.Name} 将你移至组 {args.Parameters[2]}");
				}
				catch (GroupNotExistsException)
				{
					args.Player.SendErrorMessage("没有找到这个组!");
				}
				catch (UserAccountNotExistException)
				{
					args.Player.SendErrorMessage("没有找到名为 " + account.Name + " 的用户!");
				}
				catch (UserAccountManagerException e)
				{
					args.Player.SendErrorMessage("用户 " + account.Name + " 添加失败, 请查看控制台获取详细信息.");
					TShock.Log.ConsoleError(e.ToString());
				}
			}
			else if (subcmd == "help")
			{
				args.Player.SendInfoMessage("User指令使用帮助:");
				args.Player.SendInfoMessage("{0}user add <用户名> <密码> <组名>      -- 添加一个指定用户", Specifier);
				args.Player.SendInfoMessage("{0}user del <用户名>              -- 删除一个指定用户", Specifier);
				args.Player.SendInfoMessage("{0}user password <用户名> <新密码>  -- 修改用户的密码", Specifier);
				args.Player.SendInfoMessage("{0}user group <用户名> <新组名>       -- 更换用户的组", Specifier);
			}
			else
			{
				args.Player.SendErrorMessage("格式错误. 使用 {0}user help 获取帮助.", Specifier);
			}
		}

		#endregion

		#region Stupid commands

		private static void ServerInfo(CommandArgs args)
		{
			args.Player.SendInfoMessage("有关服务器的信息");
			args.Player.SendInfoMessage("内存占用量: " + Process.GetCurrentProcess().WorkingSet64);
			args.Player.SendInfoMessage("已分配内存: " + Process.GetCurrentProcess().VirtualMemorySize64);
			args.Player.SendInfoMessage("服务器程序运行总时间: " + Process.GetCurrentProcess().TotalProcessorTime);
			args.Player.SendInfoMessage("服务器系统信息: " + Environment.OSVersion);
			args.Player.SendInfoMessage("进程数: " + Environment.ProcessorCount);
			args.Player.SendInfoMessage("服务器名称: " + Environment.MachineName);
		}

		private static void WorldInfo(CommandArgs args)
		{
			args.Player.SendInfoMessage("有关服务器地图的信息");
			args.Player.SendInfoMessage("名字: " + (TShock.Config.Settings.UseServerName ? TShock.Config.Settings.ServerName : Main.worldName));
			args.Player.SendInfoMessage("尺寸: {0}x{1}", Main.maxTilesX, Main.maxTilesY);
			args.Player.SendInfoMessage("世界ID: " + Main.worldID);
			args.Player.SendInfoMessage("种子: " + WorldGen.currentWorldSeed);
			args.Player.SendInfoMessage("模式: " + Main.GameMode);
			args.Player.SendInfoMessage("文件路径: " + Main.worldPathName);
		}

		#endregion

		#region Player Management Commands

		private static void GrabUserUserInfo(CommandArgs args)
		{
			if (args.Parameters.Count < 1)
			{
				args.Player.SendErrorMessage("格式错误! 正确格式: {0}userinfo <玩家名>", Specifier);
				return;
			}

			var players = TSPlayer.FindByNameOrID(args.Parameters[0]);
			if (players.Count < 1)
				args.Player.SendErrorMessage("没有找到这个玩家.");
			else if (players.Count > 1)
				args.Player.SendMultipleMatchError(players.Select(p => p.Name));
			else
			{
				var message = new StringBuilder();
				message.Append("IP 地址: ").Append(players[0].IP);
				if (players[0].Account != null && players[0].IsLoggedIn)
					message.Append(" | 登录账号: ").Append(players[0].Account.Name).Append(" | 组: ").Append(players[0].Group.Name);
				args.Player.SendSuccessMessage(message.ToString());
			}
		}

		private static void ViewAccountInfo(CommandArgs args)
		{
			if (args.Parameters.Count < 1)
			{
				args.Player.SendErrorMessage("格式错误! 正确格式: {0}accountinfo <用户名>", Specifier);
				return;
			}

			string username = String.Join(" ", args.Parameters);
			if (!string.IsNullOrWhiteSpace(username))
			{
				var account = TShock.UserAccounts.GetUserAccountByName(username);
				if (account != null)
				{
					DateTime LastSeen;
					string Timezone = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).Hours.ToString("+#;-#");

					if (DateTime.TryParse(account.LastAccessed, out LastSeen))
					{
						LastSeen = DateTime.Parse(account.LastAccessed).ToLocalTime();
						args.Player.SendSuccessMessage("{0}的最后登录时间为 {1} {2} UTC{3}.", account.Name, LastSeen.ToShortDateString(),
							LastSeen.ToShortTimeString(), Timezone);
					}

					if (args.Player.Group.HasPermission(Permissions.advaccountinfo))
					{
						List<string> KnownIps = JsonConvert.DeserializeObject<List<string>>(account.KnownIps?.ToString() ?? string.Empty);
						string ip = KnownIps?[KnownIps.Count - 1] ?? "N/A";
						DateTime Registered = DateTime.Parse(account.Registered).ToLocalTime();

						args.Player.SendSuccessMessage("{0}的组为 {1}.", account.Name, account.Group);
						args.Player.SendSuccessMessage("{0}的最后记录IP为 {1}.", account.Name, ip);
						args.Player.SendSuccessMessage("{0}的注册时间为 {1} {2} UTC{3}.", account.Name, Registered.ToShortDateString(), Registered.ToShortTimeString(), Timezone);
					}
				}
				else
					args.Player.SendErrorMessage("没有找到名为 {0} 的用户.", username);
			}
			else args.Player.SendErrorMessage("格式错误! 正确格式: {0}accountinfo <用户名>", Specifier);
		}

		private static void Kick(CommandArgs args)
		{
			if (args.Parameters.Count < 1)
			{
				args.Player.SendErrorMessage("格式错误!正确格式: {0}kick <玩家名> [理由]", Specifier);
				return;
			}
			if (args.Parameters[0].Length == 0)
			{
				args.Player.SendErrorMessage("你需要输入玩家名.");
				return;
			}

			string plStr = args.Parameters[0];
			var players = TSPlayer.FindByNameOrID(plStr);
			if (players.Count == 0)
			{
				args.Player.SendErrorMessage("没有找到这个玩家!");
			}
			else if (players.Count > 1)
			{
				args.Player.SendMultipleMatchError(players.Select(p => p.Name));
			}
			else
			{
				string reason = args.Parameters.Count > 1
									? String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1))
									: "行为不端.";
				if (!players[0].Kick(reason, !args.Player.RealPlayer, false, args.Player.Name))
				{
					args.Player.SendErrorMessage("你不可以踢出一个管理员!");
				}
			}
		}

		private static void Ban(CommandArgs args)
		{
			//Ban syntax:
			// ban add <target> [reason] [duration] [flags (default: -a -u -ip)]
			//						Duration is in the format 0d0h0m0s. Any part can be ignored. E.g., 1s is a valid ban time, as is 1d1s, etc. If no duration is specified, ban is permanent
			//						Valid flags: -a (ban account name), -u (ban UUID), -n (ban character name), -ip (ban IP address), -e (exact, ban the identifier provided as 'target')
			//						Unless -e is passed to the command, <target> is assumed to be a player or player index.
			// ban del <ban ID>
			//						Target is expected to be a ban Unique ID
			// ban list [page]
			//						Displays a paginated list of bans
			// ban details <ban ID>
			//						Target is expected to be a ban Unique ID
			//ban help [command]
			//						Provides extended help on specific ban commands

			void Help()
			{
				if (args.Parameters.Count > 1)
				{
					MoreHelp(args.Parameters[1].ToLower());
					return;
				}

				args.Player.SendMessage("Ban指令使用帮助", Color.White);
				args.Player.SendMessage("有效的Ban指令:", Color.White);
				args.Player.SendMessage($"ban {"add".Color(Utils.RedHighlight)} <目标> [标志]", Color.White);
				args.Player.SendMessage($"ban {"del".Color(Utils.RedHighlight)} <BanID>", Color.White);
				args.Player.SendMessage($"ban {"list".Color(Utils.RedHighlight)}", Color.White);
				args.Player.SendMessage($"ban {"details".Color(Utils.RedHighlight)} <BanID>", Color.White);
				args.Player.SendMessage($"快速使用: {"ban add".Color(Utils.BoldHighlight)} {args.Player.Name.Color(Utils.RedHighlight)} \"开挂\"", Color.White);
				args.Player.SendMessage($"使用 {"ban help".Color(Utils.BoldHighlight)} 或者 {"ban help examples".Color(Utils.BoldHighlight)} 获取更多帮助", Color.White);
			}

			void MoreHelp(string cmd)
			{
				switch (cmd)
				{
					case "add":
						args.Player.SendMessage("", Color.White);
						args.Player.SendMessage("添加Ban使用帮助", Color.White);
						args.Player.SendMessage($"{"ban add".Color(Utils.BoldHighlight)} <{"目标".Color(Utils.RedHighlight)}> [{"理由".Color(Utils.BoldHighlight)}] [{"封禁时间".Color(Utils.PinkHighlight)}] [{"标志".Color(Utils.GreenHighlight)}]", Color.White);
						args.Player.SendMessage($"- {"封禁时间".Color(Utils.PinkHighlight)}: 使用 {"0d0m0s".Color(Utils.PinkHighlight)} 格式确定封禁时长.", Color.White);
						args.Player.SendMessage($"   例如,使用 {"10d30m0s".Color(Utils.PinkHighlight)}将会封禁该目标10天30分钟0秒.", Color.White);
						args.Player.SendMessage($"   如果没有提供封禁时间, 默认进行永久封禁哦.", Color.White);
						args.Player.SendMessage($"- {"标志".Color(Utils.GreenHighlight)}: -a (账号名), -u (UUID), -n (角色名), -ip (IP 地址), -e (精确, {"目标".Color(Utils.RedHighlight)}将会被视为标识符)", Color.White);
						args.Player.SendMessage($"   除非将 {"-e".Color(Utils.GreenHighlight)} 传递给命令, 否则 {"目标".Color(Utils.RedHighlight)} 将会被认定为玩家或玩家索引", Color.White);
						args.Player.SendMessage($"   假如没有提供 {"标志".Color(Utils.GreenHighlight)} , 将会默认使用 {"-a -u -ip".Color(Utils.GreenHighlight)} 作为标志.", Color.White);
						args.Player.SendMessage($"示例: {"ban add".Color(Utils.BoldHighlight)} {args.Player.Name.Color(Utils.RedHighlight)} {"\"开挂作弊\"".Color(Utils.BoldHighlight)} {"10d30m0s".Color(Utils.PinkHighlight)} {"-a -u -ip".Color(Utils.GreenHighlight)}", Color.White);
						break;

					case "del":
						args.Player.SendMessage("", Color.White);
						args.Player.SendMessage("移除Ban使用帮助", Color.White);
						args.Player.SendMessage($"{"ban del".Color(Utils.BoldHighlight)} <{"BanID".Color(Utils.RedHighlight)}>", Color.White);
						args.Player.SendMessage($"- {"BanID".Color(Utils.RedHighlight)} 在添加Ban时提供, 也可使用 {"ban list".Color(Utils.BoldHighlight)} 指令获取.", Color.White);
						args.Player.SendMessage($"示例: {"ban del".Color(Utils.BoldHighlight)} {"12345".Color(Utils.RedHighlight)}", Color.White);
						break;

					case "list":
						args.Player.SendMessage("", Color.White);
						args.Player.SendMessage("列出Ban使用帮助", Color.White);
						args.Player.SendMessage($"{"ban list".Color(Utils.BoldHighlight)} [{"页码".Color(Utils.PinkHighlight)}]", Color.White);
						args.Player.SendMessage("- 列出所有有效的Ban. 随着封禁时间的缩短颜色将会呈现由红变绿的趋势", Color.White);
						args.Player.SendMessage($"示例: {"ban list".Color(Utils.BoldHighlight)}", Color.White);
						break;

					case "details":
						args.Player.SendMessage("", Color.White);
						args.Player.SendMessage("查看Ban使用帮助", Color.White);
						args.Player.SendMessage($"{"ban details".Color(Utils.BoldHighlight)} <{"BanID".Color(Utils.RedHighlight)}>", Color.White);
						args.Player.SendMessage($"- {"BanID".Color(Utils.RedHighlight)} 在添加Ban时提供, 也可使用 {"ban list".Color(Utils.BoldHighlight)} 指令获取.", Color.White);
						args.Player.SendMessage($"示例: {"ban details".Color(Utils.BoldHighlight)} {"12345".Color(Utils.RedHighlight)}", Color.White);
						break;

					case "identifiers":
						if (!PaginationTools.TryParsePageNumber(args.Parameters, 2, args.Player, out int pageNumber))
						{
							args.Player.SendMessage($"页码无效. 页码必须是数字.", Color.White);
							return;
						}

						var idents = from ident in Identifier.Available
									 select $"{ident.Color(Utils.RedHighlight)} - {ident.Description}";

						args.Player.SendMessage("", Color.White);
						PaginationTools.SendPage(args.Player, pageNumber, idents.ToList(),
							new PaginationTools.Settings
							{
								HeaderFormat = "可用标识符 ({0}/{1}):",
								FooterFormat = "使用 {0}ban help identifiers {{0}} 翻页.".SFormat(Specifier),
								NothingToDisplayString = "当前没有可用的标识符.",
								HeaderTextColor = Color.White,
								LineTextColor = Color.White
							});
						break;
					
					case "examples":
						args.Player.SendMessage("", Color.White);
						args.Player.SendMessage("Ban示例", Color.White);
						args.Player.SendMessage("- 通过账号名Ban一个在线的玩家", Color.White);
						args.Player.SendMessage($"   {Specifier}{"ban add".Color(Utils.BoldHighlight)} \"{"acc:".Color(Utils.RedHighlight)}{args.Player.Account.Color(Utils.RedHighlight)}\" {"\"不准开小号:(\"".Color(Utils.BoldHighlight)} {"-e".Color(Utils.GreenHighlight)} (永久封禁这个账号)", Color.White);
						args.Player.SendMessage("- 通过IP地址Ban一个离线玩家", Color.White);
						args.Player.SendMessage($"   {Specifier}{"ai".Color(Utils.BoldHighlight)} \"{args.Player.Account.Color(Utils.RedHighlight)}\" (查找目标IP)", Color.White);
						args.Player.SendMessage($"   {Specifier}{"ban add".Color(Utils.BoldHighlight)} {"ip:".Color(Utils.RedHighlight)}{args.Player.IP.Color(Utils.RedHighlight)} {"\"行为不端\"".Color(Utils.BoldHighlight)} {"-e".Color(Utils.GreenHighlight)} (永久封禁这个IP)", Color.White);
						args.Player.SendMessage($"- 通过玩家索引Ban一个在线玩家 (通常用于又臭又长的名字)", Color.White);
						args.Player.SendMessage($"   {Specifier}{"who".Color(Utils.BoldHighlight)} {"-i".Color(Utils.GreenHighlight)} (查询玩家的玩家索引)", Color.White);
						args.Player.SendMessage($"   {Specifier}{"ban add".Color(Utils.BoldHighlight)} {"tsi:".Color(Utils.RedHighlight)}{args.Player.Index.Color(Utils.RedHighlight)} {"\"让你名字又臭又长\"".Color(Utils.BoldHighlight)} {"-a -u -ip".Color(Utils.GreenHighlight)} (永久封禁这个用户的账号、关联IP、UUID)", Color.White);
						// Ban by account ID when?
						break;

					default:
						args.Player.SendMessage($"没有找到这个Ban指令. 请使用 {"ban help".Color(Utils.BoldHighlight)} {"add".Color(Utils.RedHighlight)}, {"del".Color(Utils.RedHighlight)}, {"list".Color(Utils.RedHighlight)}, {"details".Color(Utils.RedHighlight)}, {"identifiers".Color(Utils.RedHighlight)}, 或者 {"examples".Color(Utils.RedHighlight)}.", Color.White); break;
				}
			}

			void DisplayBanDetails(Ban ban)
			{
				args.Player.SendMessage($"{"Ban详细信息".Color(Utils.BoldHighlight)} - BanID: {ban.TicketNumber.Color(Utils.GreenHighlight)}", Color.White);
				args.Player.SendMessage($"{"标识符:".Color(Utils.BoldHighlight)} {ban.Identifier}", Color.White);
				args.Player.SendMessage($"{"理由:".Color(Utils.BoldHighlight)} {ban.Reason}", Color.White);
				args.Player.SendMessage($"{"处理者:".Color(Utils.BoldHighlight)} {ban.BanningUser.Color(Utils.GreenHighlight)} 在 {ban.BanDateTime.ToString("yyyy/MM/dd").Color(Utils.RedHighlight)} ({ban.GetPrettyTimeSinceBanString().Color(Utils.YellowHighlight)} {"前".Color(Utils.BoldHighlight)})", Color.White);
				if (ban.ExpirationDateTime < DateTime.UtcNow)
				{
					args.Player.SendMessage($"{"解Ban时间:".Color(Utils.BoldHighlight)} {ban.ExpirationDateTime.ToString("yyyy/MM/dd").Color(Utils.RedHighlight)} ({ban.GetPrettyExpirationString().Color(Utils.YellowHighlight)} {"前到期".Color(Utils.BoldHighlight)})", Color.White);
				}
				else
				{
					string remaining;
					if (ban.ExpirationDateTime == DateTime.MaxValue)
					{
						remaining = "未到期".Color(Utils.YellowHighlight);
					}
					else
					{
						remaining = $"{"剩余".Color(Utils.BoldHighlight)} {ban.GetPrettyExpirationString().Color(Utils.YellowHighlight)} ";
					}

					args.Player.SendMessage($"{"Ban到期时间:".Color(Utils.BoldHighlight)} {ban.ExpirationDateTime.ToString("yyyy/MM/dd").Color(Utils.RedHighlight)} ({remaining})", Color.White);
				}
			}

			AddBanResult DoBan(string ident, string reason, DateTime expiration)
			{
				AddBanResult banResult = TShock.Bans.InsertBan(ident, reason, args.Player.Account.Name, DateTime.UtcNow, expiration);
				if (banResult.Ban != null)
				{
					args.Player.SendSuccessMessage($"已添加BanID为{banResult.Ban.TicketNumber.Color(Utils.GreenHighlight)}的标识符: {ident.Color(Utils.WhiteHighlight)}.");
				}
				else
				{
					args.Player.SendWarningMessage($"标识符为 {ident.Color(Utils.WhiteHighlight)} 的Ban添加失败");
					args.Player.SendWarningMessage($"原因: {banResult.Message}");
				}

				return banResult;
			}

			void AddBan()
			{
				if (!args.Parameters.TryGetValue(1, out string target))
				{
					args.Player.SendMessage($"错误的BanAdd格式. 请使用 {"ban help add".Color(Utils.BoldHighlight)} 获取有关 {"ban add".Color(Utils.BoldHighlight)} 的用法", Color.White);
					return;
				}

				bool exactTarget = args.Parameters.Any(p => p == "-e");
				bool banAccount = args.Parameters.Any(p => p == "-a");
				bool banUuid = args.Parameters.Any(p => p == "-u");
				bool banName = args.Parameters.Any(p => p == "-n");
				bool banIp = args.Parameters.Any(p => p == "-ip");

				List<string> flags = new List<string>() { "-e", "-a", "-u", "-n", "-ip" };

				string reason = "你被Ban啦";
				string duration = null;
				DateTime expiration = DateTime.MaxValue;

				//This is hacky. We want flag values to be independent of order so we must force the consecutive ordering of the 'reason' and 'duration' parameters,
				//while still allowing them to be placed arbitrarily in the parameter list.
				//As an example, the following parameter lists (and more) should all be acceptable:
				//-u "reason" -a duration -ip
				//"reason" duration -u -a -ip
				//-u -a -ip "reason" duration
				//-u -a -ip
				for (int i = 2; i < args.Parameters.Count; i++)
				{
					var param = args.Parameters[i];
					if (!flags.Contains(param))
					{
						reason = param;
						break;
					}
				}
				for (int i = 3; i < args.Parameters.Count; i++)
				{
					var param = args.Parameters[i];
					if (!flags.Contains(param))
					{
						duration = param;
						break;
					}
				}

				if (TShock.Utils.TryParseTime(duration, out int seconds))
				{
					expiration = DateTime.UtcNow.AddSeconds(seconds);
				}

				//If no flags were specified, default to account, uuid, and IP
				if (!exactTarget && !banAccount && !banUuid && !banName && !banIp)
				{
					banAccount = banUuid = banIp = true;

					if (TShock.Config.Settings.DisableDefaultIPBan)
					{
						banIp = false;
					}
				}

				reason = reason ?? "你被Ban啦";

				if (exactTarget)
				{
					DoBan(target, reason, expiration);
					return;
				}

				var players = TSPlayer.FindByNameOrID(target);

				if (players.Count > 1)
				{
					args.Player.SendMultipleMatchError(players.Select(p => p.Name));
					return;
				}

				if (players.Count < 1)
				{
					args.Player.SendErrorMessage("找不到指定目标, 检查拼写是否正确.");
					return;
				}

				var player = players[0];
				AddBanResult banResult = null;

				if (banAccount)
				{
					if (player.Account != null)
					{
						banResult = DoBan($"{Identifier.Account}{player.Account.Name}", reason, expiration);
					}
				}

				if (banUuid)
				{
					banResult = DoBan($"{Identifier.UUID}{player.UUID}", reason, expiration);					
				}

				if (banName)
				{
					banResult = DoBan($"{Identifier.Name}{player.Name}", reason, expiration);
				}

				if (banIp)
				{
					banResult = DoBan($"{Identifier.IP}{player.IP}", reason, expiration);
				}

				if (banResult?.Ban != null)
				{
					player.Disconnect($"#{banResult.Ban.TicketNumber} - 你被Ban了: {banResult.Ban.Reason}.");
				}
			}

			void DelBan()
			{
				if (!args.Parameters.TryGetValue(1, out string target))
				{
					args.Player.SendMessage($"错误的BanDel格式. 请使用 {"ban help del".Color(Utils.BoldHighlight)} 获取有关 {"ban del".Color(Utils.BoldHighlight)} 的用法", Color.White);
					return;
				}

				if (!int.TryParse(target, out int banId))
				{
					args.Player.SendMessage($"BanID无效. 请使用 {"ban help del".Color(Utils.BoldHighlight)} 获取有关 {"ban del".Color(Utils.BoldHighlight)} 的用法", Color.White);
					return;
				}

				if (TShock.Bans.RemoveBan(banId))
				{
					TShock.Log.ConsoleInfo($"BanID为{banId}的标识符已被用户 {args.Player.Account.Name} 标记为无效.");
					args.Player.SendSuccessMessage($"BanID为{banId.Color(Utils.GreenHighlight)}的标识符已被标记为无效.");
				}
				else
				{
					args.Player.SendErrorMessage("Ban移除失败.");
				}
			}

			void ListBans()
			{
				string PickColorForBan(Ban ban)
				{
					double hoursRemaining = (ban.ExpirationDateTime - DateTime.UtcNow).TotalHours;
					double hoursTotal = (ban.ExpirationDateTime - ban.BanDateTime).TotalHours;
					double percentRemaining = TShock.Utils.Clamp(hoursRemaining / hoursTotal, 100, 0);

					int red = TShock.Utils.Clamp((int)(255 * 2.0f * percentRemaining), 255, 0);
					int green = TShock.Utils.Clamp((int)(255 * (2.0f * (1 - percentRemaining))), 255, 0);

					return $"{red:X2}{green:X2}{0:X2}";
				}

				if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out int pageNumber))
				{
					args.Player.SendMessage($"错误的BanList格式. 请使用 {"ban help del".Color(Utils.BoldHighlight)} 获取有关 {"ban del".Color(Utils.BoldHighlight)} 的用法", Color.White);
					return;
				}

				var bans = from ban in TShock.Bans.Bans
						   where ban.Value.ExpirationDateTime > DateTime.UtcNow
						   orderby ban.Value.ExpirationDateTime ascending
						   select $"[{ban.Key.Color(Utils.GreenHighlight)}] {ban.Value.Identifier.Color(PickColorForBan(ban.Value))}";

				PaginationTools.SendPage(args.Player, pageNumber, bans.ToList(),
					new PaginationTools.Settings
					{
						HeaderFormat = "Ban 名单 ({0}/{1}):",
						FooterFormat = "输入 {0}ban list {{0}} 翻页.".SFormat(Specifier),
						NothingToDisplayString = "这里已经没有有效的Ban了."
					});
			}

			void BanDetails()
			{
				if (!args.Parameters.TryGetValue(1, out string target))
				{
					args.Player.SendMessage($"错误的BanDetails格式. 请使用 {"ban help details".Color(Utils.BoldHighlight)} 获取有关 {"ban details".Color(Utils.BoldHighlight)} 的用法", Color.White);
					return;
				}

				if (!int.TryParse(target, out int banId))
				{
					args.Player.SendMessage($"BanID错误. 请使用 {"ban help details".Color(Utils.BoldHighlight)} 获取有关 {"ban details".Color(Utils.BoldHighlight)} 的用法", Color.White);
					return;
				}

				Ban ban = TShock.Bans.GetBanById(banId);

				if (ban == null)
				{
					args.Player.SendErrorMessage("没有找到与BanID匹配的Ban");
					return;
				}

				DisplayBanDetails(ban);
			}

			string subcmd = args.Parameters.Count == 0 ? "help" : args.Parameters[0].ToLower();
			switch (subcmd)
			{
				case "help":
					Help();
					break;

				case "add":
					AddBan();
					break;

				case "del":
					DelBan();
					break;

				case "list":
					ListBans();
					break;

				case "details":
					BanDetails();
					break;

				default:
					args.Player.SendMessage($"没有找到这个Ban子命令. Ban的子命令为: {"ban help".Color(Utils.RedHighlight)}, {"add".Color(Utils.RedHighlight)}, {"del".Color(Utils.RedHighlight)}, {"list".Color(Utils.RedHighlight)}, {"details".Color(Utils.RedHighlight)}, {"identifiers".Color(Utils.RedHighlight)}, 或者 {"examples".Color(Utils.RedHighlight)}.", Color.White); 
					break;
			}
		}

		private static void Whitelist(CommandArgs args)
		{
			if (args.Parameters.Count == 1)
			{
				using (var tw = new StreamWriter(FileTools.WhitelistPath, true))
				{
					tw.WriteLine(args.Parameters[0]);
				}
				args.Player.SendSuccessMessage("添加白名单 " + args.Parameters[0] + " 成功(不要用这个白名单，给自己找麻烦).");
			}
		}

		private static void DisplayLogs(CommandArgs args)
		{
			args.Player.DisplayLogs = (!args.Player.DisplayLogs);
			args.Player.SendSuccessMessage("你现在" + (args.Player.DisplayLogs ? "将会" : "不再") + "收到服务器日志.");
		}

		private static void SaveSSC(CommandArgs args)
		{
			if (Main.ServerSideCharacter)
			{
				args.Player.SendSuccessMessage("SSC云存档保存成功.");
				foreach (TSPlayer player in TShock.Players)
				{
					if (player != null && player.IsLoggedIn && !player.IsDisabledPendingTrashRemoval)
					{
						TShock.CharacterDB.InsertPlayerData(player, true);
					}
				}
			}
		}

		private static void OverrideSSC(CommandArgs args)
		{
			if (!Main.ServerSideCharacter)
			{
				args.Player.SendErrorMessage("SSC云存档被关闭.");
				return;
			}
			if (args.Parameters.Count < 1)
			{
				args.Player.SendErrorMessage("示例: {0}overridessc|{0}ossc <玩家名>", Specifier);
				return;
			}

			string playerNameToMatch = string.Join(" ", args.Parameters);
			var matchedPlayers = TSPlayer.FindByNameOrID(playerNameToMatch);
			if (matchedPlayers.Count < 1)
			{
				args.Player.SendErrorMessage("没有玩家与 \"{0}\"匹配.", playerNameToMatch);
				return;
			}
			else if (matchedPlayers.Count > 1)
			{
				args.Player.SendMultipleMatchError(matchedPlayers.Select(p => p.Name));
				return;
			}

			TSPlayer matchedPlayer = matchedPlayers[0];
			if (matchedPlayer.IsLoggedIn)
			{
				args.Player.SendErrorMessage("玩家 \"{0}\" 已登录.", matchedPlayer.Name);
				return;
			}
			if (!matchedPlayer.LoginFailsBySsi)
			{
				args.Player.SendErrorMessage("玩家 \"{0}\" 必须执行一次/login.", matchedPlayer.Name);
				return;
			}
			if (matchedPlayer.IsDisabledPendingTrashRemoval)
			{
				args.Player.SendErrorMessage("玩家 \"{0}\" 必须首先重新连接.", matchedPlayer.Name);
				return;
			}

			TShock.CharacterDB.InsertPlayerData(matchedPlayer);
			args.Player.SendSuccessMessage("\"{0}\" 的SSC云存档覆盖成功.", matchedPlayer.Name);
		}

		private static void UploadJoinData(CommandArgs args)
		{
			TSPlayer targetPlayer = args.Player;
			if (args.Parameters.Count == 1 && args.Player.HasPermission(Permissions.uploadothersdata))
			{
				List<TSPlayer> players = TSPlayer.FindByNameOrID(args.Parameters[0]);
				if (players.Count > 1)
				{
					args.Player.SendMultipleMatchError(players.Select(p => p.Name));
					return;
				}
				else if (players.Count == 0)
				{
					args.Player.SendErrorMessage("没有玩家与 \"{0}\"匹配.", args.Parameters[0]);
					return;
				}
				else
				{
					targetPlayer = players[0];
				}
			}
			else if (args.Parameters.Count == 1)
			{
				args.Player.SendErrorMessage("你没有权限上传其他玩家的SSC云存档.");
				return;
			}
			else if (args.Parameters.Count > 0)
			{
				args.Player.SendErrorMessage("示例: /uploadssc [玩家名]");
				return;
			}
			else if (args.Parameters.Count == 0 && args.Player is TSServerPlayer)
			{
				args.Player.SendErrorMessage("服务器无法上传所有的玩家本地存档数据, 请指定一个玩家.");
				args.Player.SendErrorMessage("示例: /uploadssc [玩家名]");
				return;
			}

			if (targetPlayer.IsLoggedIn)
			{
				if (TShock.CharacterDB.InsertSpecificPlayerData(targetPlayer, targetPlayer.DataWhenJoined))
				{
					targetPlayer.DataWhenJoined.RestoreCharacter(targetPlayer);
					targetPlayer.SendSuccessMessage("你的本地角色数据已上传到服务器.");
					args.Player.SendSuccessMessage("玩家角色数据上传成功.");
				}
				else
				{
					args.Player.SendErrorMessage("上传角色数据失败，你真的登录账号了吗?");
				}
			}
			else
			{
				args.Player.SendErrorMessage("目标玩家仍未登录.");
			}
		}

		private static void ForceHalloween(CommandArgs args)
		{
			TShock.Config.Settings.ForceHalloween = !TShock.Config.Settings.ForceHalloween;
			Main.checkHalloween();
			if (args.Silent)
				args.Player.SendInfoMessage("强制{0}了万圣节模式!", (TShock.Config.Settings.ForceHalloween ? "开始" : "结束"));
			else
				TSPlayer.All.SendInfoMessage("{0} 强制{1}了万圣节模式!", args.Player.Name, (TShock.Config.Settings.ForceHalloween ? "开始" : "结束"));
		}

		private static void ForceXmas(CommandArgs args)
		{
			TShock.Config.Settings.ForceXmas = !TShock.Config.Settings.ForceXmas;
			Main.checkXMas();
			if (args.Silent)
				args.Player.SendInfoMessage("强制{0}了圣诞节模式!", (TShock.Config.Settings.ForceXmas ? "开始" : "结束"));
			else
				TSPlayer.All.SendInfoMessage("{0} 强制{1}了圣诞节模式!", args.Player.Name, (TShock.Config.Settings.ForceXmas ? "开始" : "结束"));
		}

		private static void TempGroup(CommandArgs args)
		{
			if (args.Parameters.Count < 2)
			{
				args.Player.SendInfoMessage("用法无效");
				args.Player.SendInfoMessage("示例: {0}tempgroup <用户名> <新组> [时间]", Specifier);
				return;
			}

			List<TSPlayer> ply = TSPlayer.FindByNameOrID(args.Parameters[0]);
			if (ply.Count < 1)
			{
				args.Player.SendErrorMessage("没有找到玩家 {0}.", args.Parameters[0]);
				return;
			}

			if (ply.Count > 1)
			{
				args.Player.SendMultipleMatchError(ply.Select(p => p.Account.Name));
			}

			if (!TShock.Groups.GroupExists(args.Parameters[1]))
			{
				args.Player.SendErrorMessage("没有找到组 {0}", args.Parameters[1]);
				return;
			}

			if (args.Parameters.Count > 2)
			{
				int time;
				if (!TShock.Utils.TryParseTime(args.Parameters[2], out time))
				{
					args.Player.SendErrorMessage("时间无效! 正确格式: _d_h_m_s, 至少需要一个时间标识符.");
					args.Player.SendErrorMessage("例如, 1d 和 10h-30m+2m 都是有效的时间字符串, 但 2 无效.");
					return;
				}

				ply[0].tempGroupTimer = new System.Timers.Timer(time * 1000);
				ply[0].tempGroupTimer.Elapsed += ply[0].TempGroupTimerElapsed;
				ply[0].tempGroupTimer.Start();
			}

			Group g = TShock.Groups.GetGroupByName(args.Parameters[1]);

			ply[0].tempGroup = g;

			if (args.Parameters.Count < 3)
			{
				args.Player.SendSuccessMessage(String.Format("成功将用户 {0} 临时移至组 {1}", ply[0].Name, g.Name));
				ply[0].SendSuccessMessage(String.Format("你已被临时移动到 {0} 组", g.Name));
			}
			else
			{
				args.Player.SendSuccessMessage(String.Format("成功将 {0} 的组临时更改为 {1}, 有效时间为 {2}",
					ply[0].Name, g.Name, args.Parameters[2]));
				ply[0].SendSuccessMessage(String.Format("你的组被临时更改为 {0}, 有效时间为 {1}",
					g.Name, args.Parameters[2]));
			}
		}

		private static void SubstituteUser(CommandArgs args)
		{

			if (args.Player.tempGroup != null)
			{
				args.Player.tempGroup = null;
				args.Player.tempGroupTimer.Stop();
				args.Player.SendSuccessMessage("你的SuperAdmin权限已过期.");
				return;
			}
			else
			{
				args.Player.tempGroup = new SuperAdminGroup();
				args.Player.tempGroupTimer = new System.Timers.Timer(600 * 1000);
				args.Player.tempGroupTimer.Elapsed += args.Player.TempGroupTimerElapsed;
				args.Player.tempGroupTimer.Start();
				args.Player.SendSuccessMessage("你的账号权限被提升至SuperAdmin10分钟.");
				return;
			}
		}

		#endregion Player Management Commands

		#region Server Maintenence Commands

		// Executes a command as a superuser if you have sudo rights.
		private static void SubstituteUserDo(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Player.SendErrorMessage("示例: /sudo [/指令].");
				args.Player.SendErrorMessage("例如: /sudo /ban add Shank 2d 开挂.");
				return;
			}

			string replacementCommand = String.Join(" ", args.Parameters.Select(p => p.Contains(" ") ? $"\"{p}\"" : p));
			args.Player.tempGroup = new SuperAdminGroup();
			HandleCommand(args.Player, replacementCommand);
			args.Player.tempGroup = null;
			return;
		}

		private static void Broadcast(CommandArgs args)
		{
			string message = string.Join(" ", args.Parameters);

			TShock.Utils.Broadcast(
				"[服务器广播] " + message,
				Convert.ToByte(TShock.Config.Settings.BroadcastRGB[0]), Convert.ToByte(TShock.Config.Settings.BroadcastRGB[1]),
				Convert.ToByte(TShock.Config.Settings.BroadcastRGB[2]));
		}

		private static void Off(CommandArgs args)
		{

			if (Main.ServerSideCharacter)
			{
				foreach (TSPlayer player in TShock.Players)
				{
					if (player != null && player.IsLoggedIn && !player.IsDisabledPendingTrashRemoval)
					{
						player.SaveServerCharacter();
					}
				}
			}

			string reason = ((args.Parameters.Count > 0) ? "服务器正在关闭: " + String.Join(" ", args.Parameters) : "服务器已关闭!");
			TShock.Utils.StopServer(true, reason);
		}

		private static void OffNoSave(CommandArgs args)
		{
			string reason = ((args.Parameters.Count > 0) ? "服务器正在关闭: " + String.Join(" ", args.Parameters) : "服务器已关闭!");
			TShock.Utils.StopServer(false, reason);
		}

		private static void CheckUpdates(CommandArgs args)
		{
			args.Player.SendInfoMessage("正在检查更新, 请耐心等待...");
			try
			{
				TShock.UpdateManager.UpdateCheckAsync(null);
			}
			catch (Exception)
			{
				//swallow the exception
				return;
			}
		}

		private static void ManageRest(CommandArgs args)
		{
			string subCommand = "help";
			if (args.Parameters.Count > 0)
				subCommand = args.Parameters[0];

			switch (subCommand.ToLower())
			{
				case "listusers":
					{
						int pageNumber;
						if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out pageNumber))
							return;

						Dictionary<string, int> restUsersTokens = new Dictionary<string, int>();
						foreach (Rests.SecureRest.TokenData tokenData in TShock.RestApi.Tokens.Values)
						{
							if (restUsersTokens.ContainsKey(tokenData.Username))
								restUsersTokens[tokenData.Username]++;
							else
								restUsersTokens.Add(tokenData.Username, 1);
						}

						List<string> restUsers = new List<string>(
							restUsersTokens.Select(ut => string.Format("{0} ({1} tokens)", ut.Key, ut.Value)));

						PaginationTools.SendPage(
							args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(restUsers), new PaginationTools.Settings
							{
								NothingToDisplayString = "没有找到任何有效的临时REST用户.",
								HeaderFormat = "有效的临时REST用户 ({0}/{1}):",
								FooterFormat = "输入 {0}rest listusers {{0}} 翻页.".SFormat(Specifier)
							}
						);

						break;
					}
				case "destroytokens":
					{
						TShock.RestApi.Tokens.Clear();
						args.Player.SendSuccessMessage("所有临时RestToken已被销毁.");
						break;
					}
				default:
					{
						args.Player.SendInfoMessage("Rest命令使用帮助:");
						args.Player.SendMessage("listusers     - 列出所有临时REST用户和它们的Token.", Color.White);
						args.Player.SendMessage("destroytokens - 销毁所有临时RESTToken.", Color.White);
						break;
					}
			}
		}

		#endregion Server Maintenence Commands

		#region Cause Events and Spawn Monsters Commands

		static readonly List<string> _validEvents = new List<string>()
		{
			"meteor(陨石)",
			"fullmoon(满月)",
			"bloodmoon（血月）",
			"eclipse(日食)",
			"invasion(入侵)",
			"sandstorm(沙尘暴)",
			"rain(雨)",
			"lanternsnight(灯笼夜)"
		};
		static readonly List<string> _validInvasions = new List<string>()
		{
			"goblins(哥布林军队)",
			"snowmen(雪人军团)",
			"pirates(海盗入侵)",
			"pumpkinmoon(南瓜月)",
			"frostmoon(霜月)",
			"martians(火星暴乱)"
		};

		private static void ManageWorldEvent(CommandArgs args)
		{
			if (args.Parameters.Count < 1)
			{
				args.Player.SendErrorMessage("格式错误! 正确格式: {0}worldevent <事件类型>", Specifier);
				args.Player.SendErrorMessage("事件类型列表: {0}", String.Join(", ", _validEvents));
				args.Player.SendErrorMessage("如果召唤入侵, 则需要提供入侵类型: {0}", String.Join(", ", _validInvasions));
				return;
			}

			var eventType = args.Parameters[0].ToLowerInvariant();

			void FailedPermissionCheck()
			{
				args.Player.SendErrorMessage("你没有权限开始 {0} 事件.", eventType);
				return;
			}

			switch (eventType)
			{
				case "陨石":
				case "meteor":
					if (!args.Player.HasPermission(Permissions.dropmeteor) && !args.Player.HasPermission(Permissions.managemeteorevent))
					{
						FailedPermissionCheck();
						return;
					}

					DropMeteor(args);
					return;

				case "满月":
				case "fullmoon":
				case "full moon":
					if (!args.Player.HasPermission(Permissions.fullmoon) && !args.Player.HasPermission(Permissions.managefullmoonevent))
					{
						FailedPermissionCheck();
						return;
					}
					Fullmoon(args);
					return;

				case "血月":
				case "bloodmoon":
				case "blood moon":
					if (!args.Player.HasPermission(Permissions.bloodmoon) && !args.Player.HasPermission(Permissions.managebloodmoonevent))
					{
						FailedPermissionCheck();
						return;
					}
					Bloodmoon(args);
					return;
				case "日食":
				case "eclipse":
					if (!args.Player.HasPermission(Permissions.eclipse) && !args.Player.HasPermission(Permissions.manageeclipseevent))
					{
						FailedPermissionCheck();
						return;
					}
					Eclipse(args);
					return;

				case "入侵":
				case "invade":
				case "invasion":
					if (!args.Player.HasPermission(Permissions.invade) && !args.Player.HasPermission(Permissions.manageinvasionevent))
					{
						FailedPermissionCheck();
						return;
					}
					Invade(args);
					return;

				case "沙尘暴":
				case "sandstorm":
					if (!args.Player.HasPermission(Permissions.sandstorm) && !args.Player.HasPermission(Permissions.managesandstormevent))
					{
						FailedPermissionCheck();
						return;
					}
					Sandstorm(args);
					return;

				case "雨":
				case "rain":
					if (!args.Player.HasPermission(Permissions.rain) && !args.Player.HasPermission(Permissions.managerainevent))
					{
						FailedPermissionCheck();
						return;
					}
					Rain(args);
					return;

				case "灯笼夜":
				case "lanternsnight":
				case "lanterns":
					if (!args.Player.HasPermission(Permissions.managelanternsnightevent))
					{
						FailedPermissionCheck();
						return;
					}
					LanternsNight(args);
					return;

				default:
					args.Player.SendErrorMessage("事件类型无效! 事件类型列表: {0}", String.Join(", ", _validEvents));
					return;
			}
		}

		private static void DropMeteor(CommandArgs args)
		{
			WorldGen.spawnMeteor = false;
			WorldGen.dropMeteor();
			if (args.Silent)
			{
				args.Player.SendInfoMessage("一颗流星从天而降.");
			}
			else
			{
				TSPlayer.All.SendInfoMessage("{0} 使一颗流星从天而降.", args.Player.Name);
			}
		}

		private static void Fullmoon(CommandArgs args)
		{
			TSPlayer.Server.SetFullMoon();
			if (args.Silent)
			{
				args.Player.SendInfoMessage("满月开始了!");
			}
			else
			{
				TSPlayer.All.SendInfoMessage("{0} 开始了满月.", args.Player.Name);
			}
		}

		private static void Bloodmoon(CommandArgs args)
		{
			TSPlayer.Server.SetBloodMoon(!Main.bloodMoon);
			if (args.Silent)
			{
				args.Player.SendInfoMessage("血月{0}了.", Main.bloodMoon ? "开始" : "结束");
			}
			else
			{
				TSPlayer.All.SendInfoMessage("{0} {1}了血月.", args.Player.Name, Main.bloodMoon ? "开始" : "结束");
			}
		}

		private static void Eclipse(CommandArgs args)
		{
			TSPlayer.Server.SetEclipse(!Main.eclipse);
			if (args.Silent)
			{
				args.Player.SendInfoMessage("日食{0}了.", Main.eclipse ? "开始" : "结束");
			}
			else
			{
				TSPlayer.All.SendInfoMessage("{0} {1}了日食.", args.Player.Name, Main.eclipse ? "开始" : "结束");
			}
		}

		private static void Invade(CommandArgs args)
		{
			if (Main.invasionSize <= 0)
			{
				if (args.Parameters.Count < 2)
				{
					args.Player.SendErrorMessage("格式错误! 正确格式:  {0}worldevent invasion [入侵类型] [入侵波数]", Specifier);
					args.Player.SendErrorMessage("入侵类型列表: {0}", String.Join(", ", _validInvasions));
					return;
				}

				int wave = 1;
				switch (args.Parameters[1].ToLowerInvariant())
				{
					case "哥布林军队":
					case "goblin":
					case "goblins":
						TSPlayer.All.SendInfoMessage("{0} 召唤了一次哥布林入侵.", args.Player.Name);
						TShock.Utils.StartInvasion(1);
						break;

					case "雪人军团":
					case "snowman":
					case "snowmen":
						TSPlayer.All.SendInfoMessage("{0} 召唤了一支雪人军团.", args.Player.Name);
						TShock.Utils.StartInvasion(2);
						break;
						
					case "海盗入侵":
					case "pirate":
					case "pirates":
						TSPlayer.All.SendInfoMessage("{0} 召唤了一次海盗入侵.", args.Player.Name);
						TShock.Utils.StartInvasion(3);
						break;

					case "南瓜月":
					case "pumpkin":
					case "pumpkinmoon":
						if (args.Parameters.Count > 2)
						{
							if (!int.TryParse(args.Parameters[2], out wave) || wave <= 0)
							{
								args.Player.SendErrorMessage("入侵波数无效!");
								break;
							}
						}

						TSPlayer.Server.SetPumpkinMoon(true);
						Main.bloodMoon = false;
						NPC.waveKills = 0f;
						NPC.waveNumber = wave;
						TSPlayer.All.SendInfoMessage("{0} 开始了第 {1} 波的南瓜月!", args.Player.Name, wave);
						break;

					case "霜月":
					case "frost":
					case "frostmoon":
						if (args.Parameters.Count > 2)
						{
							if (!int.TryParse(args.Parameters[2], out wave) || wave <= 0)
							{
								args.Player.SendErrorMessage("入侵波数无效!");
								return;
							}
						}

						TSPlayer.Server.SetFrostMoon(true);
						Main.bloodMoon = false;
						NPC.waveKills = 0f;
						NPC.waveNumber = wave;
						TSPlayer.All.SendInfoMessage("{0} 开始了第 {1} 波的霜月!", args.Player.Name, wave);
						break;

					case "火星暴乱":
					case "martian":
					case "martians":
						TSPlayer.All.SendInfoMessage("{0} 开始了火星暴乱.", args.Player.Name);
						TShock.Utils.StartInvasion(4);
						break;

					default:
						args.Player.SendErrorMessage("入侵类型无效! 入侵类型列表: {0}", String.Join(", ", _validInvasions));
						break;
				}
			}
			else if (DD2Event.Ongoing)
			{
				DD2Event.StopInvasion();
				TSPlayer.All.SendInfoMessage("{0} 结束了旧日军团事件.", args.Player.Name);
			}
			else
			{
				TSPlayer.All.SendInfoMessage("{0} 结束了目前的入侵.", args.Player.Name);
				Main.invasionSize = 0;
			}
		}

		private static void Sandstorm(CommandArgs args)
		{
			if (Terraria.GameContent.Events.Sandstorm.Happening)
			{
				Terraria.GameContent.Events.Sandstorm.StopSandstorm();
				TSPlayer.All.SendInfoMessage("{0} 结束了沙尘暴事件.", args.Player.Name);
			}
			else
			{
				Terraria.GameContent.Events.Sandstorm.StartSandstorm();
				TSPlayer.All.SendInfoMessage("{0} 开始了一次沙尘暴事件.", args.Player.Name);
			}
		}

		private static void Rain(CommandArgs args)
		{
			bool slime = false;
			if ((args.Parameters.Count > 1 && args.Parameters[1].ToLowerInvariant() == "slime") || (args.Parameters.Count > 1 && args.Parameters[1].ToLowerInvariant() == "史莱姆"))
			{
				slime = true;
			}

			if (!slime)
			{
				args.Player.SendInfoMessage("使用 \"{0}worldevent rain(雨) slime(史莱姆)\" 开始一场史莱姆雨!", Specifier);
			}

			if (slime && Main.raining) //Slime rain cannot be activated during normal rain
			{
				args.Player.SendErrorMessage("在下史莱姆雨之前, 你需要停止这场雨!");
				return;
			}
			if (slime && Main.slimeRain) //Toggle slime rain off
			{
				Main.StopSlimeRain(false);
				TSPlayer.All.SendData(PacketTypes.WorldInfo);
				TSPlayer.All.SendInfoMessage("{0} 结束了这场史莱姆雨.", args.Player.Name);
				return;
			}

			if (slime && !Main.slimeRain) //Toggle slime rain on
			{
				Main.StartSlimeRain(false);
				TSPlayer.All.SendData(PacketTypes.WorldInfo);
				TSPlayer.All.SendInfoMessage("{0} 开始了一场史莱姆雨.", args.Player.Name);
			}

			if (Main.raining && !slime) //Toggle rain off
			{
				Main.StopRain();
				TSPlayer.All.SendData(PacketTypes.WorldInfo);
				TSPlayer.All.SendInfoMessage("{0} 结束了这场雨.", args.Player.Name);
				return;
			}

			if (!Main.raining && !slime) //Toggle rain on
			{
				Main.StartRain();
				TSPlayer.All.SendData(PacketTypes.WorldInfo);
				TSPlayer.All.SendInfoMessage("{0} 开始了一场雨.", args.Player.Name);
				return;
			}
		}

		private static void LanternsNight(CommandArgs args)
		{
			LanternNight.ToggleManualLanterns();
			string msg = $"{(LanternNight.LanternsUp ? "开始" : "结束")}了一场灯笼夜.";
			if (args.Silent)
			{
				args.Player.SendInfoMessage("你" + msg);
			}
			else
			{
				TSPlayer.All.SendInfoMessage(args.Player.Name + msg);
			}
		}

		private static void ClearAnglerQuests(CommandArgs args)
		{
			if (args.Parameters.Count > 0)
			{
				var result = Main.anglerWhoFinishedToday.RemoveAll(s => s.ToLower().Equals(args.Parameters[0].ToLower()));
				if (result > 0)
				{
					args.Player.SendSuccessMessage("成功从今日渔夫任务完成列表移除了 {0} 名玩家.", result);
					foreach (TSPlayer ply in TShock.Players.Where(p => p != null && p.Active && p.TPlayer.name.ToLower().Equals(args.Parameters[0].ToLower())))
					{
						//this will always tell the client that they have not done the quest today.
						ply.SendData((PacketTypes)74, "");
					}
				}
				else
					args.Player.SendErrorMessage("指定玩家没有完成今日渔夫任务.");

			}
			else
			{
				Main.anglerWhoFinishedToday.Clear();
				NetMessage.SendAnglerQuest(-1);
				args.Player.SendSuccessMessage("成功从今天的渔夫任务完成列表中清除所有用户.");
			}
		}

		static Dictionary<string, int> _worldModes = new Dictionary<string, int>
		{
			{ "经典模式",    0 },
			{ "专家模式",    1 },
			{ "大师模式",    2 },
			{ "旅行模式",    3 },
			{ "创造模式",    3 },
			{ "normal",      0 },
			{ "expert",      1 },
			{ "master",      2 },
			{ "journey",     3 },
			{ "creative",    3 }
		};

		private static void ChangeWorldMode(CommandArgs args)
		{
			if (args.Parameters.Count < 1)
			{
				args.Player.SendErrorMessage("格式错误! 正确格式: {0}worldmode <模式>", Specifier);
				args.Player.SendErrorMessage("模式列表: {0}", String.Join(", ", _worldModes.Keys));
				return;
			}

			int mode;

			if (int.TryParse(args.Parameters[0], out mode))
			{
				if (mode < 0 || mode > 3)
				{
					args.Player.SendErrorMessage("模式无效! 模式列表: {0}", String.Join(", ", _worldModes.Keys));
					return;
				}
			}
			else if (_worldModes.ContainsKey(args.Parameters[0].ToLowerInvariant()))
			{
				mode = _worldModes[args.Parameters[0].ToLowerInvariant()];
			}
			else
			{
				args.Player.SendErrorMessage("模式无效! 模式列表: {0}", String.Join(", ", _worldModes.Keys));
				return;
			}

			Main.GameMode = mode;
			args.Player.SendSuccessMessage("世界模式更改为 {0}", _worldModes.Keys.ElementAt(mode));
			TSPlayer.All.SendData(PacketTypes.WorldInfo);
		}

		private static void Hardmode(CommandArgs args)
		{
			if (Main.hardMode)
			{
				Main.hardMode = false;
				TSPlayer.All.SendData(PacketTypes.WorldInfo);
				args.Player.SendSuccessMessage("困难模式已关闭.");
			}
			else if (!TShock.Config.Settings.DisableHardmode)
			{
				WorldGen.StartHardmode();
				args.Player.SendSuccessMessage("困难模式已开启.");
			}
			else
			{
				args.Player.SendErrorMessage("config.json禁止世界进入困难模式.");
			}
		}

		private static void SpawnBoss(CommandArgs args)
		{
			if (args.Parameters.Count < 1 || args.Parameters.Count > 2)
			{
				args.Player.SendErrorMessage("格式错误! 正确格式: {0}spawnboss <Boss类型> [召唤数量]", Specifier);
				return;
			}

			int amount = 1;
			if (args.Parameters.Count == 2 && (!int.TryParse(args.Parameters[1], out amount) || amount <= 0))
			{
				args.Player.SendErrorMessage("Boss数量无效!");
				return;
			}

			string message = "{0} 召唤了 {2} 次 {1}";
			string spawnName;
			NPC npc = new NPC();
			switch (args.Parameters[0].ToLower())
			{
				case "全部":
				case "所有":
				case "*":
				case "all":
					int[] npcIds = { 4, 13, 35, 50, 125, 126, 127, 134, 222, 245, 262, 266, 370, 398, 439, 636, 657 };
					TSPlayer.Server.SetTime(false, 0.0);
					foreach (int i in npcIds)
					{
						npc.SetDefaults(i);
						TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					}
					spawnName = "全Boss";
					break;

				case "克苏鲁之脑":
				case "邪神大脑":
				case "克脑":
				case "brain":
				case "brain of cthulhu":
				case "boc":
					npc.SetDefaults(266);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "克苏鲁之脑";
					break;

				case "毁灭者":
				case "铁长直":
				case "destroyer":
					npc.SetDefaults(134);
					TSPlayer.Server.SetTime(false, 0.0);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "毁灭者";
					break;

				case "猪龙鱼公爵":
				case "猪龙鱼":
				case "猪鲨":
				case "duke":
				case "duke fishron":
				case "fishron":
					npc.SetDefaults(370);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "猪龙鱼公爵";
					break;

				case "世界吞噬怪":
				case "世界吞噬者":
				case "黑长直":
				case "eater":
				case "eater of worlds":
				case "eow":
					npc.SetDefaults(13);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "世界吞噬怪";
					break;

				case "克苏鲁之眼":
				case "克眼":
				case "eye":
				case "eye of cthulhu":
				case "eoc":
					npc.SetDefaults(4);
					TSPlayer.Server.SetTime(false, 0.0);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "克苏鲁之眼";
					break;
				case "石巨人":
				case "石头人":
				case "golem":
					npc.SetDefaults(245);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "石巨人";
					break;

				case "史莱姆王":
				case "史王":
				case "史":
				case "king":
				case "king slime":
				case "ks":
					npc.SetDefaults(50);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "史莱姆王";
					break;

				case "世纪之花":
				case "世花":
				case "plantera":
					npc.SetDefaults(262);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "世纪之花";
					break;

				case "机械骷髅王":
				case "机械吴克":
				case "机械骷髅大队长":
				case "prime":
				case "skeletron prime":
					npc.SetDefaults(127);
					TSPlayer.Server.SetTime(false, 0.0);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "机械骷髅王";
					break;

				case "蜂王":
				case "蜂后":
				case "queen bee":
				case "qb":
					npc.SetDefaults(222);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "蜂王";
					break;

				case "骷髅王":
				case "骷髅大队长":
				case "吴克":
				case "skeletron":
					npc.SetDefaults(35);
					TSPlayer.Server.SetTime(false, 0.0);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "骷髅王";
					break;

				case "双子魔眼":
				case "双子":
				case "机械魔眼":
				case "twins":
					TSPlayer.Server.SetTime(false, 0.0);
					npc.SetDefaults(125);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					npc.SetDefaults(126);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "双子魔眼";
					break;

				case "血肉墙":
				case "肉山":
				case "肉墙":
				case "wof":
				case "wall of flesh":
					if (Main.wofNPCIndex != -1)
					{
						args.Player.SendErrorMessage("世界已存在一个血肉墙!");
						return;
					}
					if (args.Player.Y / 16f < Main.maxTilesY - 205)
					{
						args.Player.SendErrorMessage("你必须在地狱生成血肉墙!");
						return;
					}
					NPC.SpawnWOF(new Vector2(args.Player.X, args.Player.Y));
					spawnName = "血肉墙";
					break;

				case "擦玻璃小哥":
				case "月亮领主":
				case "月球领主":
				case "月总":
				case "moon":
				case "moon lord":
				case "ml":
					npc.SetDefaults(398);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "月亮领主";
					break;

				case "光之女皇":
				case "光皇":
				case "光女":
				case "empress":
				case "empress of light":
				case "eol":
					npc.SetDefaults(636);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "光之女皇";
					break;

				case "史莱姆皇后":
				case "史皇":
				case "史莱姆太太":
				case "queen slime":
				case "qs":
					npc.SetDefaults(657);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "史莱姆皇后";
					break;

				case "拜月教邪教徒":
				case "教徒":
				case "拜月教徒":
				case "lunatic":
				case "lunatic cultist":
				case "cultist":
				case "lc":
					npc.SetDefaults(439);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "拜月教邪教徒";
					break;

				case "双足翼龙":
				case "翼龙":
				case "双足龙":
				case "betsy":
					npc.SetDefaults(551);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "双足翼龙";
					break;

				case "荷兰飞盗船":
				case "海盗船":
				case "flying dutchman":
				case "flying":
				case "dutchman":
					npc.SetDefaults(491);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "荷兰飞盗船";
					break;

				case "哀木":
				case "mourning wood":
					npc.SetDefaults(325);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "哀木";
					break;

				case "南瓜王":
				case "南瓜大王":
				case "pumpking":
					npc.SetDefaults(327);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "南瓜王";
					break;

				case "常绿尖叫怪":
				case "everscream":
					npc.SetDefaults(344);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "常绿尖叫怪";
					break;

				case "圣诞坦克":
				case "圣诞老人坦克":
				case "坦克":
				case "santa-nk1":
				case "santa":
					npc.SetDefaults(346);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "圣诞坦克";
					break;

				case "冰雪女王":
				case "冰霜女王":
				case "ice queen":
					npc.SetDefaults(345);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "冰雪女王";
					break;

				case "火星飞碟":
				case "UFO":
				case "飞碟":
				case "martian saucer":
					npc.SetDefaults(392);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "火星飞碟";
					break;

				case "日耀柱":
				case "solar pillar":
					npc.SetDefaults(517);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "日耀柱";
					break;

				case "星云柱":
				case "nebula pillar":
					npc.SetDefaults(507);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "星云柱";
					break;

				case "星旋柱":
				case "vortex pillar":
					npc.SetDefaults(422);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "星旋柱";
					break;

				case "星尘柱":
				case "stardust pillar":
					npc.SetDefaults(493);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "星尘柱";
					break;

				case "鹿角怪":
				case "独眼巨鹿":
				case "deerclops":
					npc.SetDefaults(668);
					TSPlayer.Server.SpawnNPC(npc.type, npc.FullName, amount, args.Player.TileX, args.Player.TileY);
					spawnName = "独眼巨鹿";
					break;
				default:
					args.Player.SendErrorMessage("Boss类型无效!");
					return;
			}

			if (args.Silent)
			{
				//"You spawned <spawn name> <x> time(s)"
				args.Player.SendSuccessMessage(message, "你", spawnName, amount);
			}
			else
			{
				//" spawned <spawn name> <x> time(s)"
				TSPlayer.All.SendSuccessMessage(message, args.Player.Name, spawnName, amount);
			}
		}

		private static void SpawnMob(CommandArgs args)
		{
			if (args.Parameters.Count < 1 || args.Parameters.Count > 2)
			{
				args.Player.SendErrorMessage("格式错误! 正确格式: {0}spawnmob <NPC类型> [生成数量]", Specifier);
				return;
			}
			if (args.Parameters[0].Length == 0)
			{
				args.Player.SendErrorMessage("NPC类型无效!");
				return;
			}

			int amount = 1;
			if (args.Parameters.Count == 2 && !int.TryParse(args.Parameters[1], out amount))
			{
				args.Player.SendErrorMessage("格式错误! 正确格式: {0}spawnmob <NPC类型> [生成数量]", Specifier);
				return;
			}

			amount = Math.Min(amount, Main.maxNPCs);

			var npcs = TShock.Utils.GetNPCByIdOrName(args.Parameters[0]);
			if (npcs.Count == 0)
			{
				args.Player.SendErrorMessage("NPC类型无效!");
			}
			else if (npcs.Count > 1)
			{
				args.Player.SendMultipleMatchError(npcs.Select(n => $"{n.FullName}({n.type})"));
			}
			else
			{
				var npc = npcs[0];
				if (npc.type >= 1 && npc.type < Main.maxNPCTypes && npc.type != 113)
				{
					TSPlayer.Server.SpawnNPC(npc.netID, npc.FullName, amount, args.Player.TileX, args.Player.TileY, 50, 20);
					if (args.Silent)
					{
						args.Player.SendSuccessMessage("成功召唤了 {1} 次 {0}.", npc.FullName, amount);
					}
					else
					{
						TSPlayer.All.SendSuccessMessage("{0} 召唤了 {2} 次 {1}.", args.Player.Name, npc.FullName, amount);
					}
				}
				else if (npc.type == 113)
				{
					if (Main.wofNPCIndex != -1 || (args.Player.Y / 16f < (Main.maxTilesY - 205)))
					{
						args.Player.SendErrorMessage("无法召唤血肉墙!");
						return;
					}
					NPC.SpawnWOF(new Vector2(args.Player.X, args.Player.Y));
					if (args.Silent)
					{
						args.Player.SendSuccessMessage("成功召唤了血肉墙!");
					}
					else
					{
						TSPlayer.All.SendSuccessMessage("{0} 召唤了血肉墙!", args.Player.Name);
					}
				}
				else
				{
					args.Player.SendErrorMessage("NPC类型无效!");
				}
			}
		}

		#endregion Cause Events and Spawn Monsters Commands

		#region Teleport Commands

		private static void Home(CommandArgs args)
		{
			if (args.Player.Dead)
			{
				args.Player.SendErrorMessage("你已经挂了.");
				return;
			}
			args.Player.Spawn(PlayerSpawnContext.RecallFromItem);
			args.Player.SendSuccessMessage("已传送至你的出生点.");
		}

		private static void Spawn(CommandArgs args)
		{
			if (args.Player.Teleport(Main.spawnTileX * 16, (Main.spawnTileY * 16) - 48))
				args.Player.SendSuccessMessage("已传送至世界出生点.");
		}

		private static void TP(CommandArgs args)
		{
			if (args.Parameters.Count != 1 && args.Parameters.Count != 2)
			{
				if (args.Player.HasPermission(Permissions.tpothers))
					args.Player.SendErrorMessage("格式错误! 正确格式: {0}tp <玩家> [玩家2]", Specifier);
				else
					args.Player.SendErrorMessage("格式错误! 正确格式: {0}tp <玩家>", Specifier);
				return;
			}

			if (args.Parameters.Count == 1)
			{
				var players = TSPlayer.FindByNameOrID(args.Parameters[0]);
				if (players.Count == 0)
					args.Player.SendErrorMessage("找不到该玩家!");
				else if (players.Count > 1)
					args.Player.SendMultipleMatchError(players.Select(p => p.Name));
				else
				{
					var target = players[0];
					if (!target.TPAllow && !args.Player.HasPermission(Permissions.tpoverride))
					{
						args.Player.SendErrorMessage("{0} 禁止玩家对其使用传送.", target.Name);
						return;
					}
					if (args.Player.Teleport(target.TPlayer.position.X, target.TPlayer.position.Y))
					{
						args.Player.SendSuccessMessage("成功传送至 {0}.", target.Name);
						if (!args.Player.HasPermission(Permissions.tpsilent))
							target.SendInfoMessage("{0} 传送到你的位置.", args.Player.Name);
					}
				}
			}
			else
			{
				if (!args.Player.HasPermission(Permissions.tpothers))
				{
					args.Player.SendErrorMessage("你没有权限使用这个命令.");
					return;
				}

				var players1 = TSPlayer.FindByNameOrID(args.Parameters[0]);
				var players2 = TSPlayer.FindByNameOrID(args.Parameters[1]);

				if (players2.Count == 0)
					args.Player.SendErrorMessage("找不到该玩家!");
				else if (players2.Count > 1)
					args.Player.SendMultipleMatchError(players2.Select(p => p.Name));
				else if (players1.Count == 0)
				{
					if (args.Parameters[0] == "*")
					{
						if (!args.Player.HasPermission(Permissions.tpallothers))
						{
							args.Player.SendErrorMessage("你没有权限使用该命令.");
							return;
						}

						var target = players2[0];
						foreach (var source in TShock.Players.Where(p => p != null && p != args.Player))
						{
							if (!target.TPAllow && !args.Player.HasPermission(Permissions.tpoverride))
								continue;
							if (source.Teleport(target.TPlayer.position.X, target.TPlayer.position.Y))
							{
								if (args.Player != source)
								{
									if (args.Player.HasPermission(Permissions.tpsilent))
										source.SendSuccessMessage("你被传送至 {0}.", target.Name);
									else
										source.SendSuccessMessage("{0} 将你传送至 {1}.", args.Player.Name, target.Name);
								}
								if (args.Player != target)
								{
									if (args.Player.HasPermission(Permissions.tpsilent))
										target.SendInfoMessage("{0} 被传送至你的位置.", source.Name);
									if (!args.Player.HasPermission(Permissions.tpsilent))
										target.SendInfoMessage("{0} 将 {1} 传送到你的位置.", args.Player.Name, source.Name);
								}
							}
						}
						args.Player.SendSuccessMessage("已将所有玩家传送至 {0}.", target.Name);
					}
					else
						args.Player.SendErrorMessage("找不到该玩家!");
				}
				else if (players1.Count > 1)
					args.Player.SendMultipleMatchError(players1.Select(p => p.Name));
				else
				{
					var source = players1[0];
					if (!source.TPAllow && !args.Player.HasPermission(Permissions.tpoverride))
					{
						args.Player.SendErrorMessage("{0} 禁止玩家对其使用传送.", source.Name);
						return;
					}
					var target = players2[0];
					if (!target.TPAllow && !args.Player.HasPermission(Permissions.tpoverride))
					{
						args.Player.SendErrorMessage("{0} 禁止玩家对其使用传送.", target.Name);
						return;
					}
					args.Player.SendSuccessMessage("成功将 {0} 传送至 {1}.", source.Name, target.Name);
					if (source.Teleport(target.TPlayer.position.X, target.TPlayer.position.Y))
					{
						if (args.Player != source)
						{
							if (args.Player.HasPermission(Permissions.tpsilent))
								source.SendSuccessMessage("你被传送到 {0}.", target.Name);
							else
								source.SendSuccessMessage("{0} 将你传送至 {1}.", args.Player.Name, target.Name);
						}
						if (args.Player != target)
						{
							if (args.Player.HasPermission(Permissions.tpsilent))
								target.SendInfoMessage("{0} 被传送至你的位置.", source.Name);
							if (!args.Player.HasPermission(Permissions.tpsilent))
								target.SendInfoMessage("{0} 将 {1} 传送至你的位置.", args.Player.Name, source.Name);
						}
					}
				}
			}
		}

		private static void TPHere(CommandArgs args)
		{
			if (args.Parameters.Count < 1)
			{
				if (args.Player.HasPermission(Permissions.tpallothers))
					args.Player.SendErrorMessage("格式错误! 正确格式: {0}tphere <玩家名|*>", Specifier);
				else
					args.Player.SendErrorMessage("格式错误! 正确格式: {0}tphere <玩家名>", Specifier);
				return;
			}

			string playerName = String.Join(" ", args.Parameters);
			var players = TSPlayer.FindByNameOrID(playerName);
			if (players.Count == 0)
			{
				if (playerName == "*")
				{
					if (!args.Player.HasPermission(Permissions.tpallothers))
					{
						args.Player.SendErrorMessage("你没有权限对所有人使用传送.");
						return;
					}
					for (int i = 0; i < Main.maxPlayers; i++)
					{
						if (Main.player[i].active && (Main.player[i] != args.TPlayer))
						{
							if (TShock.Players[i].Teleport(args.TPlayer.position.X, args.TPlayer.position.Y))
								TShock.Players[i].SendSuccessMessage(String.Format("你被传送至 {0}.", args.Player.Name));
						}
					}
					args.Player.SendSuccessMessage("所有人被传送至你的位置.");
				}
				else
					args.Player.SendErrorMessage("没有找到该玩家!");
			}
			else if (players.Count > 1)
				args.Player.SendMultipleMatchError(players.Select(p => p.Name));
			else
			{
				var plr = players[0];
				if (plr.Teleport(args.TPlayer.position.X, args.TPlayer.position.Y))
				{
					plr.SendInfoMessage("你被传送至 {0}.", args.Player.Name);
					args.Player.SendSuccessMessage("已将 {0} 传送至你的位置.", plr.Name);
				}
			}
		}

		private static void TPNpc(CommandArgs args)
		{
			if (args.Parameters.Count < 1)
			{
				args.Player.SendErrorMessage("格式错误! 正确格式: {0}tpnpc <NPC名称>", Specifier);
				return;
			}

			var npcStr = string.Join(" ", args.Parameters);
			var matches = new List<NPC>();
			foreach (var npc in Main.npc.Where(npc => npc.active))
			{
				var englishName = EnglishLanguage.GetNpcNameById(npc.netID);

				if (string.Equals(npc.FullName, npcStr, StringComparison.InvariantCultureIgnoreCase) ||
					string.Equals(englishName, npcStr, StringComparison.InvariantCultureIgnoreCase))
				{
					matches = new List<NPC> { npc };
					break;
				}
				if (npc.FullName.ToLowerInvariant().StartsWith(npcStr.ToLowerInvariant()) ||
					englishName?.StartsWith(npcStr, StringComparison.InvariantCultureIgnoreCase) == true)
					matches.Add(npc);
			}

			if (matches.Count > 1)
			{
				args.Player.SendMultipleMatchError(matches.Select(n => $"{n.FullName}({n.whoAmI})"));
				return;
			}
			if (matches.Count == 0)
			{
				args.Player.SendErrorMessage("没有找到这个NPC!");
				return;
			}

			var target = matches[0];
			args.Player.Teleport(target.position.X, target.position.Y);
			args.Player.SendSuccessMessage("成功传送至 '{0}'.", target.FullName);
		}

		private static void GetPos(CommandArgs args)
		{
			var player = args.Player.Name;
			if (args.Parameters.Count > 0)
			{
				player = String.Join(" ", args.Parameters);
			}

			var players = TSPlayer.FindByNameOrID(player);
			if (players.Count == 0)
			{
				args.Player.SendErrorMessage("没有找到该玩家!");
			}
			else if (players.Count > 1)
			{
				args.Player.SendMultipleMatchError(players.Select(p => p.Name));
			}
			else
			{
				args.Player.SendSuccessMessage("当前 {0} 的坐标为({1}, {2}).", players[0].Name, players[0].TileX, players[0].TileY);
			}
		}

		private static void TPPos(CommandArgs args)
		{
			if (args.Parameters.Count != 2)
			{
				args.Player.SendErrorMessage("格式错误! 正确格式: {0}tppos <X坐标> <Y坐标>", Specifier);
				return;
			}

			int x, y;
			if (!int.TryParse(args.Parameters[0], out x) || !int.TryParse(args.Parameters[1], out y))
			{
				args.Player.SendErrorMessage("坐标无效!");
				return;
			}
			x = Math.Max(0, x);
			y = Math.Max(0, y);
			x = Math.Min(x, Main.maxTilesX - 1);
			y = Math.Min(y, Main.maxTilesY - 1);

			args.Player.Teleport(16 * x, 16 * y);
			args.Player.SendSuccessMessage("已成功传送至({0}, {1})!", x, y);
		}

		private static void TPAllow(CommandArgs args)
		{
			if (!args.Player.TPAllow)
				args.Player.SendSuccessMessage("成功解除传送保护.");
			if (args.Player.TPAllow)
				args.Player.SendSuccessMessage("成功开启传送保护.");
			args.Player.TPAllow = !args.Player.TPAllow;
		}

		private static void Warp(CommandArgs args)
		{
			bool hasManageWarpPermission = args.Player.HasPermission(Permissions.managewarp);
			if (args.Parameters.Count < 1)
			{
				if (hasManageWarpPermission)
				{
					args.Player.SendInfoMessage("格式错误! 正确格式: {0}warp [子命令] [参数]", Specifier);
					args.Player.SendInfoMessage("子命令: add, del, hide, list, send, [传送点名]");
					args.Player.SendInfoMessage("参数: add [传送点名], del [传送点名], list [页码]");
					args.Player.SendInfoMessage("参数: send [玩家] [传送点名], hide [传送点名] [启用(true/false)]");
					args.Player.SendInfoMessage("示例: {0}warp add foobar, {0}warp hide foobar true, {0}warp foobar", Specifier);
					return;
				}
				else
				{
					args.Player.SendErrorMessage("格式错误! 正确格式: {0}warp [name] 或 {0}warp list <页码>", Specifier);
					return;
				}
			}

			if (args.Parameters[0].Equals("list"))
			{
				#region List warps
				int pageNumber;
				if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out pageNumber))
					return;
				IEnumerable<string> warpNames = from warp in TShock.Warps.Warps
												where !warp.IsPrivate
												select warp.Name;
				PaginationTools.SendPage(args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(warpNames),
					new PaginationTools.Settings
					{
						HeaderFormat = "传送点 ({0}/{1}):",
						FooterFormat = "输入 {0}warp list {{0}} 翻页.".SFormat(Specifier),
						NothingToDisplayString = "服务器还没有设置传送点."
					});
				#endregion
			}
			else if (args.Parameters[0].ToLower() == "add" && hasManageWarpPermission)
			{
				#region Add warp
				if (args.Parameters.Count == 2)
				{
					string warpName = args.Parameters[1];
					if (warpName == "list" || warpName == "hide" || warpName == "del" || warpName == "add")
					{
						args.Player.SendErrorMessage("这个传送点名已被保留, 请尝试其他名称.");
					}
					else if (TShock.Warps.Add(args.Player.TileX, args.Player.TileY, warpName))
					{
						args.Player.SendSuccessMessage("成功添加传送点 " + warpName);
					}
					else
					{
						args.Player.SendErrorMessage("传送点 " + warpName + " 已存在.");
					}
				}
				else
					args.Player.SendErrorMessage("格式错误! 正确格式: {0}warp add [传送点名]", Specifier);
				#endregion
			}
			else if (args.Parameters[0].ToLower() == "del" && hasManageWarpPermission)
			{
				#region Del warp
				if (args.Parameters.Count == 2)
				{
					string warpName = args.Parameters[1];
					if (TShock.Warps.Remove(warpName))
					{
						args.Player.SendSuccessMessage("删除传送点 " + warpName);
					}
					else
						args.Player.SendErrorMessage("没有找到这个传送点.");
				}
				else
					args.Player.SendErrorMessage("格式错误! 正确格式: {0}warp del [传送点名]", Specifier);
				#endregion
			}
			else if (args.Parameters[0].ToLower() == "hide" && hasManageWarpPermission)
			{
				#region Hide warp
				if (args.Parameters.Count == 3)
				{
					string warpName = args.Parameters[1];
					bool state = false;
					if (Boolean.TryParse(args.Parameters[2], out state))
					{
						if (TShock.Warps.Hide(args.Parameters[1], state))
						{
							if (state)
								args.Player.SendSuccessMessage("传送点 " + warpName + " 已被设为隐藏.");
							else
								args.Player.SendSuccessMessage("传送点 " + warpName + " 已被设为公开.");
						}
						else
							args.Player.SendErrorMessage("没有找到这个传送点.");
					}
					else
						args.Player.SendErrorMessage("格式错误! 正确格式: {0}warp hide [传送点名] <true/false>", Specifier);
				}
				else
					args.Player.SendErrorMessage("格式错误! 正确格式: {0}warp hide [传送点名] <true/false>", Specifier);
				#endregion
			}
			else if (args.Parameters[0].ToLower() == "send" && args.Player.HasPermission(Permissions.tpothers))
			{
				#region Warp send
				if (args.Parameters.Count < 3)
				{
					args.Player.SendErrorMessage("格式错误! 正确格式: {0}warp send [玩家名] [传送点]", Specifier);
					return;
				}

				var foundplr = TSPlayer.FindByNameOrID(args.Parameters[1]);
				if (foundplr.Count == 0)
				{
					args.Player.SendErrorMessage("没有找到该玩家!");
					return;
				}
				else if (foundplr.Count > 1)
				{
					args.Player.SendMultipleMatchError(foundplr.Select(p => p.Name));
					return;
				}

				string warpName = args.Parameters[2];
				var warp = TShock.Warps.Find(warpName);
				var plr = foundplr[0];
				if (warp != null)
				{
					if (plr.Teleport(warp.Position.X * 16, warp.Position.Y * 16))
					{
						plr.SendSuccessMessage(String.Format("{0} 将你传送到传送点 {1}.", args.Player.Name, warpName));
						args.Player.SendSuccessMessage(String.Format("你被 {0} 传送至传送点 {1}.", plr.Name, warpName));
					}
				}
				else
				{
					args.Player.SendErrorMessage("没有找到目标传送点.");
				}
				#endregion
			}
			else
			{
				string warpName = String.Join(" ", args.Parameters);
				var warp = TShock.Warps.Find(warpName);
				if (warp != null)
				{
					if (args.Player.Teleport(warp.Position.X * 16, warp.Position.Y * 16))
						args.Player.SendSuccessMessage("你已被传送至传送点 " + warpName + ".");
				}
				else
				{
					args.Player.SendErrorMessage("没有找到目标传送点.");
				}
			}
		}

		#endregion Teleport Commands

		#region Group Management

		private static void Group(CommandArgs args)
		{
			string subCmd = args.Parameters.Count == 0 ? "help" : args.Parameters[0].ToLower();

			switch (subCmd)
			{
				case "add":
					#region Add group
					{
						if (args.Parameters.Count < 2)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}group add <组名> [权限]", Specifier);
							return;
						}

						string groupName = args.Parameters[1];
						args.Parameters.RemoveRange(0, 2);
						string permissions = String.Join(",", args.Parameters);

						try
						{
							TShock.Groups.AddGroup(groupName, null, permissions, TShockAPI.Group.defaultChatColor);
							args.Player.SendSuccessMessage($"成功添加组 {groupName}!");
						}
						catch (GroupExistsException)
						{
							args.Player.SendErrorMessage($"这个组 {groupName} 已存在!");
						}
						catch (GroupManagerException ex)
						{
							args.Player.SendErrorMessage(ex.ToString());
						}
					}
					#endregion
					return;
				case "addperm":
					#region Add permissions
					{
						if (args.Parameters.Count < 3)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}group addperm <组名> <权限...>", Specifier);
							return;
						}

						string groupName = args.Parameters[1];
						args.Parameters.RemoveRange(0, 2);
						if (groupName == "*")
						{
							foreach (Group g in TShock.Groups)
							{
								TShock.Groups.AddPermissions(g.Name, args.Parameters);
							}
							args.Player.SendSuccessMessage("已修改所有组！.");
							return;
						}
						try
						{
							string response = TShock.Groups.AddPermissions(groupName, args.Parameters);
							if (response.Length > 0)
							{
								args.Player.SendSuccessMessage(response);
							}
							return;
						}
						catch (GroupManagerException ex)
						{
							args.Player.SendErrorMessage(ex.ToString());
						}
					}
					#endregion
					return;
				case "help":
					#region Help
					{
						int pageNumber;
						if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out pageNumber))
							return;

						var lines = new List<string>
						{
							"add <组名> <权限...>        - 添加一个新组.",
							"addperm <组名> <权限...>    - 为一个组添加指定权限.",
							"color <组名> <rrr,ggg,bbb>  - 改变一个组的聊天颜色.",
							"rename <组名> <新组名>      - 改变一个组的名称.",
							"del <组名>                  - 删除一个组.",
							"delperm <组名> <权限...>    - 移除一个组的指定权限.",
							"list [页码]                 - 列出所有组.",
							"listperm <组名> [页码]      - 列出一个组的权限.",
							"parent <组名> <父组>        - 改变一个组的父组.",
							"prefix <组名> <前缀>        - 修改一个组的聊天前缀.",
							"suffix <组名> <后缀>        - 修改一个组的聊天后缀."
						};

						PaginationTools.SendPage(args.Player, pageNumber, lines,
							new PaginationTools.Settings
							{
								HeaderFormat = "Group命令使用帮助 ({0}/{1}):",
								FooterFormat = "输入 {0}group help {{0}} 查看更多子命令.".SFormat(Specifier)
							}
						);
					}
					#endregion
					return;
				case "parent":
					#region Parent
					{
						if (args.Parameters.Count < 2)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}group parent <组名> [新的父组名]", Specifier);
							return;
						}

						string groupName = args.Parameters[1];
						Group group = TShock.Groups.GetGroupByName(groupName);
						if (group == null)
						{
							args.Player.SendErrorMessage("没找到组 \"{0}\".", groupName);
							return;
						}

						if (args.Parameters.Count > 2)
						{
							string newParentGroupName = string.Join(" ", args.Parameters.Skip(2));
							if (!string.IsNullOrWhiteSpace(newParentGroupName) && !TShock.Groups.GroupExists(newParentGroupName))
							{
								args.Player.SendErrorMessage("没找到组 \"{0}\".", newParentGroupName);
								return;
							}

							try
							{
								TShock.Groups.UpdateGroup(groupName, newParentGroupName, group.Permissions, group.ChatColor, group.Suffix, group.Prefix);

								if (!string.IsNullOrWhiteSpace(newParentGroupName))
									args.Player.SendSuccessMessage("组 \"{0}\" 的父组已被设为 \"{1}\".", groupName, newParentGroupName);
								else
									args.Player.SendSuccessMessage("已移除组 \"{0}\" 的父组.", groupName);
							}
							catch (GroupManagerException ex)
							{
								args.Player.SendErrorMessage(ex.Message);
							}
						}
						else
						{
							if (group.Parent != null)
								args.Player.SendSuccessMessage("组 \"{0}\" 的父组为 \"{1}\".", group.Name, group.Parent.Name);
							else
								args.Player.SendSuccessMessage("组 \"{0}\" 没有父组.", group.Name);
						}
					}
					#endregion
					return;
				case "suffix":
					#region Suffix
					{
						if (args.Parameters.Count < 2)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}group suffix <组名> [新后缀]", Specifier);
							return;
						}

						string groupName = args.Parameters[1];
						Group group = TShock.Groups.GetGroupByName(groupName);
						if (group == null)
						{
							args.Player.SendErrorMessage("没有找到组 \"{0}\".", groupName);
							return;
						}

						if (args.Parameters.Count > 2)
						{
							string newSuffix = string.Join(" ", args.Parameters.Skip(2));

							try
							{
								TShock.Groups.UpdateGroup(groupName, group.ParentName, group.Permissions, group.ChatColor, newSuffix, group.Prefix);

								if (!string.IsNullOrWhiteSpace(newSuffix))
									args.Player.SendSuccessMessage("组 \"{0}\" 的聊天后缀已被设为 \"{1}\".", groupName, newSuffix);
								else
									args.Player.SendSuccessMessage("已移除组 \"{0}\"的聊天后缀.", groupName);
							}
							catch (GroupManagerException ex)
							{
								args.Player.SendErrorMessage(ex.Message);
							}
						}
						else
						{
							if (!string.IsNullOrWhiteSpace(group.Suffix))
								args.Player.SendSuccessMessage("组 \"{0}\" 的聊天后缀 \"{1}\".", group.Name, group.Suffix);
							else
								args.Player.SendSuccessMessage("组 \"{0}\" 没有聊天后缀.", group.Name);
						}
					}
					#endregion
					return;
				case "prefix":
					#region Prefix
					{
						if (args.Parameters.Count < 2)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}group prefix <组名> [新前缀]", Specifier);
							return;
						}

						string groupName = args.Parameters[1];
						Group group = TShock.Groups.GetGroupByName(groupName);
						if (group == null)
						{
							args.Player.SendErrorMessage("没有找到组 \"{0}\".", groupName);
							return;
						}

						if (args.Parameters.Count > 2)
						{
							string newPrefix = string.Join(" ", args.Parameters.Skip(2));

							try
							{
								TShock.Groups.UpdateGroup(groupName, group.ParentName, group.Permissions, group.ChatColor, group.Suffix, newPrefix);

								if (!string.IsNullOrWhiteSpace(newPrefix))
									args.Player.SendSuccessMessage("组 \"{0}\" 的聊天前缀已被设为 \"{1}\".", groupName, newPrefix);
								else
									args.Player.SendSuccessMessage("已移除组 \"{0}\" 的聊天前缀.", groupName);
							}
							catch (GroupManagerException ex)
							{
								args.Player.SendErrorMessage(ex.Message);
							}
						}
						else
						{
							if (!string.IsNullOrWhiteSpace(group.Prefix))
								args.Player.SendSuccessMessage("组 \"{0}\" 的聊天前缀为 \"{1}\".", group.Name, group.Prefix);
							else
								args.Player.SendSuccessMessage("组 \"{0}\" 没有聊天前缀.", group.Name);
						}
					}
					#endregion
					return;
				case "color":
					#region Color
					{
						if (args.Parameters.Count < 2 || args.Parameters.Count > 3)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}group color <组名> [聊天颜色(000,000,000)]", Specifier);
							return;
						}

						string groupName = args.Parameters[1];
						Group group = TShock.Groups.GetGroupByName(groupName);
						if (group == null)
						{
							args.Player.SendErrorMessage("没有找到组 \"{0}\".", groupName);
							return;
						}

						if (args.Parameters.Count == 3)
						{
							string newColor = args.Parameters[2];

							String[] parts = newColor.Split(',');
							byte r;
							byte g;
							byte b;
							if (parts.Length == 3 && byte.TryParse(parts[0], out r) && byte.TryParse(parts[1], out g) && byte.TryParse(parts[2], out b))
							{
								try
								{
									TShock.Groups.UpdateGroup(groupName, group.ParentName, group.Permissions, newColor, group.Suffix, group.Prefix);

									args.Player.SendSuccessMessage("组 \"{0}\" 的聊天颜色已被设为 \"{1}\".", groupName, newColor);
								}
								catch (GroupManagerException ex)
								{
									args.Player.SendErrorMessage(ex.Message);
								}
							}
							else
							{
								args.Player.SendErrorMessage("颜色格式无效, 正确格式 \"rrr,ggg,bbb\"");
							}
						}
						else
						{
							args.Player.SendSuccessMessage("组 \"{0}\" 的聊天颜色为 \"{1}\".", group.Name, group.ChatColor);
						}
					}
					#endregion
					return;
				case "rename":
					#region Rename group
					{
						if (args.Parameters.Count != 3)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}group rename <组名> <新组名>", Specifier);
							return;
						}

						string group = args.Parameters[1];
						string newName = args.Parameters[2];
						try
						{
							string response = TShock.Groups.RenameGroup(group, newName);
							args.Player.SendSuccessMessage(response);
						}
						catch (GroupManagerException ex)
						{
							args.Player.SendErrorMessage(ex.Message);
						}
					}
					#endregion
					return;
				case "del":
					#region Delete group
					{
						if (args.Parameters.Count != 2)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}group del <组名>", Specifier);
							return;
						}

						try
						{
							string response = TShock.Groups.DeleteGroup(args.Parameters[1], true);
							if (response.Length > 0)
							{
								args.Player.SendSuccessMessage(response);
							}
						}
						catch (GroupManagerException ex)
						{
							args.Player.SendErrorMessage(ex.Message);
						}
					}
					#endregion
					return;
				case "delperm":
					#region Delete permissions
					{
						if (args.Parameters.Count < 3)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}group delperm <组名> <权限...>", Specifier);
							return;
						}

						string groupName = args.Parameters[1];
						args.Parameters.RemoveRange(0, 2);
						if (groupName == "*")
						{
							foreach (Group g in TShock.Groups)
							{
								TShock.Groups.DeletePermissions(g.Name, args.Parameters);
							}
							args.Player.SendSuccessMessage("成功修改了所有组.");
							return;
						}
						try
						{
							string response = TShock.Groups.DeletePermissions(groupName, args.Parameters);
							if (response.Length > 0)
							{
								args.Player.SendSuccessMessage(response);
							}
							return;
						}
						catch (GroupManagerException ex)
						{
							args.Player.SendErrorMessage(ex.Message);
						}
					}
					#endregion
					return;
				case "list":
					#region List groups
					{
						int pageNumber;
						if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out pageNumber))
							return;
						var groupNames = from grp in TShock.Groups.groups
										 select grp.Name;
						PaginationTools.SendPage(args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(groupNames),
							new PaginationTools.Settings
							{
								HeaderFormat = "组列表 ({0}/{1}):",
								FooterFormat = "输入 {0}group list {{0}} 翻页.".SFormat(Specifier)
							});
					}
					#endregion
					return;
				case "listperm":
					#region List permissions
					{
						if (args.Parameters.Count == 1)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}group listperm <组名> [页码]", Specifier);
							return;
						}
						int pageNumber;
						if (!PaginationTools.TryParsePageNumber(args.Parameters, 2, args.Player, out pageNumber))
							return;

						if (!TShock.Groups.GroupExists(args.Parameters[1]))
						{
							args.Player.SendErrorMessage("组名无效.");
							return;
						}
						Group grp = TShock.Groups.GetGroupByName(args.Parameters[1]);
						List<string> permissions = grp.TotalPermissions;

						PaginationTools.SendPage(args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(permissions),
							new PaginationTools.Settings
							{
								HeaderFormat = "组 " + grp.Name + " 的权限列表" + " ({0}/{1}):",
								FooterFormat = "输入 {0}group listperm {1} {{0}} 翻页.".SFormat(Specifier, grp.Name),
								NothingToDisplayString = "组 " + grp.Name + " 没有任何权限."
							});
					}
					#endregion
					return;
				default:
					args.Player.SendErrorMessage("无效的 'group' 指令! 输入 {0}group help 获取'group'指令的相关用法.", Specifier);
					return;
			}
		}
		#endregion Group Management

		#region Item Management

		private static void ItemBan(CommandArgs args)
		{
			string subCmd = args.Parameters.Count == 0 ? "help" : args.Parameters[0].ToLower();
			switch (subCmd)
			{
				case "add":
					#region Add item
					{
						if (args.Parameters.Count != 2)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}itemban add <物品名>", Specifier);
							return;
						}

						List<Item> items = TShock.Utils.GetItemByIdOrName(args.Parameters[1]);
						if (items.Count == 0)
						{
							args.Player.SendErrorMessage("物品名无效.");
						}
						else if (items.Count > 1)
						{
							args.Player.SendMultipleMatchError(items.Select(i => $"{i.Name}({i.netID})"));
						}
						else
						{
							// Yes this is required because of localization
							// User may have passed in localized name but itembans works on English names
							string englishNameForStorage = EnglishLanguage.GetItemNameById(items[0].type);
							TShock.ItemBans.DataModel.AddNewBan(englishNameForStorage);

							// It was decided in Telegram that we would continue to ban
							// projectiles based on whether or not their associated item was
							// banned. However, it was also decided that we'd change the way
							// this worked: in particular, we'd make it so that the item ban
							// system just adds things to the projectile ban system at the
							// command layer instead of inferring the state of projectile
							// bans based on the state of the item ban system.

							if (items[0].type == ItemID.DirtRod)
							{
								TShock.ProjectileBans.AddNewBan(ProjectileID.DirtBall);
							}

							if (items[0].type == ItemID.Sandgun)
							{
								TShock.ProjectileBans.AddNewBan(ProjectileID.SandBallGun);
								TShock.ProjectileBans.AddNewBan(ProjectileID.EbonsandBallGun);
								TShock.ProjectileBans.AddNewBan(ProjectileID.PearlSandBallGun);
							}

							// This returns the localized name to the player, not the item as it was stored.
							args.Player.SendSuccessMessage("物品 " + items[0].Name + " 已被禁用.");
						}
					}
					#endregion
					return;
				case "allow":
					#region Allow group to item
					{
						if (args.Parameters.Count != 3)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}itemban allow <物品名> <组名>", Specifier);
							return;
						}

						List<Item> items = TShock.Utils.GetItemByIdOrName(args.Parameters[1]);
						if (items.Count == 0)
						{
							args.Player.SendErrorMessage("物品名无效.");
						}
						else if (items.Count > 1)
						{
							args.Player.SendMultipleMatchError(items.Select(i => $"{i.Name}({i.netID})"));
						}
						else
						{
							if (!TShock.Groups.GroupExists(args.Parameters[2]))
							{
								args.Player.SendErrorMessage("组名无效.");
								return;
							}

							ItemBan ban = TShock.ItemBans.DataModel.GetItemBanByName(EnglishLanguage.GetItemNameById(items[0].type));
							if (ban == null)
							{
								args.Player.SendErrorMessage("{0} 没有被禁用.", items[0].Name);
								return;
							}
							if (!ban.AllowedGroups.Contains(args.Parameters[2]))
							{
								TShock.ItemBans.DataModel.AllowGroup(EnglishLanguage.GetItemNameById(items[0].type), args.Parameters[2]);
								args.Player.SendSuccessMessage("组 {0} 已被允许使用 {1}.", args.Parameters[2], items[0].Name);
							}
							else
							{
								args.Player.SendWarningMessage("组 {0} 已被允许使用 {1}.", args.Parameters[2], items[0].Name);
							}
						}
					}
					#endregion
					return;
				case "del":
					#region Delete item
					{
						if (args.Parameters.Count != 2)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}itemban del <物品名>", Specifier);
							return;
						}

						List<Item> items = TShock.Utils.GetItemByIdOrName(args.Parameters[1]);
						if (items.Count == 0)
						{
							args.Player.SendErrorMessage("物品名无效.");
						}
						else if (items.Count > 1)
						{
							args.Player.SendMultipleMatchError(items.Select(i => $"{i.Name}({i.netID})"));
						}
						else
						{
							TShock.ItemBans.DataModel.RemoveBan(EnglishLanguage.GetItemNameById(items[0].type));
							args.Player.SendSuccessMessage("物品 " + items[0].Name + " 已被允许使用.");
						}
					}
					#endregion
					return;
				case "disallow":
					#region Disllow group from item
					{
						if (args.Parameters.Count != 3)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}itemban disallow <物品名> <组名>", Specifier);
							return;
						}

						List<Item> items = TShock.Utils.GetItemByIdOrName(args.Parameters[1]);
						if (items.Count == 0)
						{
							args.Player.SendErrorMessage("物品名无效.");
						}
						else if (items.Count > 1)
						{
							args.Player.SendMultipleMatchError(items.Select(i => $"{i.Name}({i.netID})"));
						}
						else
						{
							if (!TShock.Groups.GroupExists(args.Parameters[2]))
							{
								args.Player.SendErrorMessage("组名无效.");
								return;
							}

							ItemBan ban = TShock.ItemBans.DataModel.GetItemBanByName(EnglishLanguage.GetItemNameById(items[0].type));
							if (ban == null)
							{
								args.Player.SendErrorMessage("{0} 没有被禁用.", items[0].Name);
								return;
							}
							if (ban.AllowedGroups.Contains(args.Parameters[2]))
							{
								TShock.ItemBans.DataModel.RemoveGroup(EnglishLanguage.GetItemNameById(items[0].type), args.Parameters[2]);
								args.Player.SendSuccessMessage("组 {0} 已被禁止使用 {1}.", args.Parameters[2], items[0].Name);
							}
							else
							{
								args.Player.SendWarningMessage("组 {0} 已被禁止使用 {1}.", args.Parameters[2], items[0].Name);
							}
						}
					}
					#endregion
					return;
				case "help":
					#region Help
					{
						int pageNumber;
						if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out pageNumber))
							return;

						var lines = new List<string>
						{
							"add <物品>             - 禁用指定物品.",
							"allow <物品> <组名>    - 允许指定组使用被禁用的物品.",
							"del <物品>             - 解禁指定物品.",
							"disallow <物品> <组名> - 不再允许指定组使用被禁用的物品.",
							"list [页码]            - 列出所有被禁用的物品."
						};

						PaginationTools.SendPage(args.Player, pageNumber, lines,
							new PaginationTools.Settings
							{
								HeaderFormat = "禁用物品使用帮助 ({0}/{1}):",
								FooterFormat = "输入 {0}itemban help {{0}} 翻页.".SFormat(Specifier)
							}
						);
					}
					#endregion
					return;
				case "list":
					#region List items
					{
						int pageNumber;
						if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out pageNumber))
							return;
						IEnumerable<string> itemNames = from itemBan in TShock.ItemBans.DataModel.ItemBans
														select itemBan.Name;
						PaginationTools.SendPage(args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(itemNames),
							new PaginationTools.Settings
							{
								HeaderFormat = "禁用物品列表 ({0}/{1}):",
								FooterFormat = "输入 {0}itemban list {{0}} 翻页.".SFormat(Specifier),
								NothingToDisplayString = "服务器还没有被禁用的物品哦."
							});
					}
					#endregion
					return;
				default:
					#region Default
					{
						args.Player.SendErrorMessage("无效的 'itemban' 指令! 输入 {0}itemban help 获取有关 'itemban' 的使用帮助.", Specifier);
					}
					#endregion
					return;

			}
		}
		#endregion Item Management

		#region Projectile Management

		private static void ProjectileBan(CommandArgs args)
		{
			string subCmd = args.Parameters.Count == 0 ? "help" : args.Parameters[0].ToLower();
			switch (subCmd)
			{
				case "add":
					#region Add projectile
					{
						if (args.Parameters.Count != 2)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}projban add <射弹ID>", Specifier);
							return;
						}
						short id;
						if (Int16.TryParse(args.Parameters[1], out id) && id > 0 && id < Main.maxProjectileTypes)
						{
							TShock.ProjectileBans.AddNewBan(id);
							args.Player.SendSuccessMessage("射弹ID为 {0} 的射弹被禁用.", id);
						}
						else
							args.Player.SendErrorMessage("射弹ID无效!");
					}
					#endregion
					return;
				case "allow":
					#region Allow group to projectile
					{
						if (args.Parameters.Count != 3)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}projban allow <射弹ID> <组名>", Specifier);
							return;
						}

						short id;
						if (Int16.TryParse(args.Parameters[1], out id) && id > 0 && id < Main.maxProjectileTypes)
						{
							if (!TShock.Groups.GroupExists(args.Parameters[2]))
							{
								args.Player.SendErrorMessage("组名无效.");
								return;
							}

							ProjectileBan ban = TShock.ProjectileBans.GetBanById(id);
							if (ban == null)
							{
								args.Player.SendErrorMessage("射弹为 {0} 的射弹没有被禁用.", id);
								return;
							}
							if (!ban.AllowedGroups.Contains(args.Parameters[2]))
							{
								TShock.ProjectileBans.AllowGroup(id, args.Parameters[2]);
								args.Player.SendSuccessMessage("组 {0} 已被允许生成射弹ID为 {1} 的射弹.", args.Parameters[2], id);
							}
							else
								args.Player.SendWarningMessage("组 {0} 已被允许生成射弹ID为 {1} 的射弹.", args.Parameters[2], id);
						}
						else
							args.Player.SendErrorMessage("射弹ID无效!");
					}
					#endregion
					return;
				case "del":
					#region Delete projectile
					{
						if (args.Parameters.Count != 2)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}projban del <射弹ID>", Specifier);
							return;
						}

						short id;
						if (Int16.TryParse(args.Parameters[1], out id) && id > 0 && id < Main.maxProjectileTypes)
						{
							TShock.ProjectileBans.RemoveBan(id);
							args.Player.SendSuccessMessage("射弹ID为 {0} 的射弹被允许生成.", id);
							return;
						}
						else
							args.Player.SendErrorMessage("射弹ID无效!");
					}
					#endregion
					return;
				case "disallow":
					#region Disallow group from projectile
					{
						if (args.Parameters.Count != 3)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}projban disallow <射弹ID> <组名>", Specifier);
							return;
						}

						short id;
						if (Int16.TryParse(args.Parameters[1], out id) && id > 0 && id < Main.maxProjectileTypes)
						{
							if (!TShock.Groups.GroupExists(args.Parameters[2]))
							{
								args.Player.SendErrorMessage("组名无效.");
								return;
							}

							ProjectileBan ban = TShock.ProjectileBans.GetBanById(id);
							if (ban == null)
							{
								args.Player.SendErrorMessage("射弹ID为 {0} 的射弹没有被禁用.", id);
								return;
							}
							if (ban.AllowedGroups.Contains(args.Parameters[2]))
							{
								TShock.ProjectileBans.RemoveGroup(id, args.Parameters[2]);
								args.Player.SendSuccessMessage("组 {0} 不再被允许生成射弹ID为 {1} 的射弹.", args.Parameters[2], id);
								return;
							}
							else
								args.Player.SendWarningMessage("组 {0} 不再被允许生成射弹ID为 {1} 的射弹.", args.Parameters[2], id);
						}
						else
							args.Player.SendErrorMessage("射弹ID无效!");
					}
					#endregion
					return;
				case "help":
					#region Help
					{
						int pageNumber;
						if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out pageNumber))
							return;

						var lines = new List<string>
						{
							"add <射弹ID>             - 禁用指定射弹.",
							"allow <射弹ID> <组名>    - 允许指定组生成被禁用的射弹.",
							"del <射弹ID>             - 解禁指定射弹.",
							"disallow <射弹ID> <组名> - 不再允许指定组生成被禁用的射弹.",
							"list [页码]              - 列出所有被禁用的射弹."
						};

						PaginationTools.SendPage(args.Player, pageNumber, lines,
							new PaginationTools.Settings
							{
								HeaderFormat = "禁用射弹使用帮助 ({0}/{1}):",
								FooterFormat = "输入 {0}projban help {{0}} 翻页.".SFormat(Specifier)
							}
						);
					}
					#endregion
					return;
				case "list":
					#region List projectiles
					{
						int pageNumber;
						if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out pageNumber))
							return;
						IEnumerable<Int16> projectileIds = from projectileBan in TShock.ProjectileBans.ProjectileBans
														   select projectileBan.ID;
						PaginationTools.SendPage(args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(projectileIds),
							new PaginationTools.Settings
							{
								HeaderFormat = "射弹禁用列表 ({0}/{1}):",
								FooterFormat = "输入 {0}projban list {{0}} 翻页.".SFormat(Specifier),
								NothingToDisplayString = "服务器还没有封禁如何射弹哦."
							});
					}
					#endregion
					return;
				default:
					#region Default
					{
						args.Player.SendErrorMessage("无效的 'projban' 指令! 输入 {0}projban help 获取有关 'projban' 的使用帮助.", Specifier);
					}
					#endregion
					return;
			}
		}
		#endregion Projectile Management

		#region Tile Management
		private static void TileBan(CommandArgs args)
		{
			string subCmd = args.Parameters.Count == 0 ? "help" : args.Parameters[0].ToLower();
			switch (subCmd)
			{
				case "add":
					#region Add tile
					{
						if (args.Parameters.Count != 2)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}tileban add <图格ID>", Specifier);
							return;
						}
						short id;
						if (Int16.TryParse(args.Parameters[1], out id) && id >= 0 && id < Main.maxTileSets)
						{
							TShock.TileBans.AddNewBan(id);
							args.Player.SendSuccessMessage("图格ID为 {0} 的图格被禁止建造.", id);
						}
						else
							args.Player.SendErrorMessage("图格ID无效!");
					}
					#endregion
					return;
				case "allow":
					#region Allow group to place tile
					{
						if (args.Parameters.Count != 3)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}tileban allow <图格ID> <组名>", Specifier);
							return;
						}

						short id;
						if (Int16.TryParse(args.Parameters[1], out id) && id >= 0 && id < Main.maxTileSets)
						{
							if (!TShock.Groups.GroupExists(args.Parameters[2]))
							{
								args.Player.SendErrorMessage("组名无效.");
								return;
							}

							TileBan ban = TShock.TileBans.GetBanById(id);
							if (ban == null)
							{
								args.Player.SendErrorMessage("图格ID为 {0} 的图格没有被禁止建造.", id);
								return;
							}
							if (!ban.AllowedGroups.Contains(args.Parameters[2]))
							{
								TShock.TileBans.AllowGroup(id, args.Parameters[2]);
								args.Player.SendSuccessMessage("组 {0} 已被允许建造图格ID为 {1} 的图格.", args.Parameters[2], id);
							}
							else
								args.Player.SendWarningMessage("组 {0} 已被允许建造图格ID为 {1} 的图格. {1}.", args.Parameters[2], id);
						}
						else
							args.Player.SendErrorMessage("图格ID!");
					}
					#endregion
					return;
				case "del":
					#region Delete tile ban
					{
						if (args.Parameters.Count != 2)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}tileban del <图格ID>", Specifier);
							return;
						}

						short id;
						if (Int16.TryParse(args.Parameters[1], out id) && id >= 0 && id < Main.maxTileSets)
						{
							TShock.TileBans.RemoveBan(id);
							args.Player.SendSuccessMessage("图格ID为 {0} 的图格已被允许建造.", id);
							return;
						}
						else
							args.Player.SendErrorMessage("图格ID无效!");
					}
					#endregion
					return;
				case "disallow":
					#region Disallow group from placing tile
					{
						if (args.Parameters.Count != 3)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}tileban disallow <图格ID> <组名>", Specifier);
							return;
						}

						short id;
						if (Int16.TryParse(args.Parameters[1], out id) && id >= 0 && id < Main.maxTileSets)
						{
							if (!TShock.Groups.GroupExists(args.Parameters[2]))
							{
								args.Player.SendErrorMessage("组名无效.");
								return;
							}

							TileBan ban = TShock.TileBans.GetBanById(id);
							if (ban == null)
							{
								args.Player.SendErrorMessage("图格ID为 {0} 的物块没有被禁止建造.", id);
								return;
							}
							if (ban.AllowedGroups.Contains(args.Parameters[2]))
							{
								TShock.TileBans.RemoveGroup(id, args.Parameters[2]);
								args.Player.SendSuccessMessage("组 {0} 不再被建造图格ID为 {1} 图格.", args.Parameters[2], id);
								return;
							}
							else
								args.Player.SendWarningMessage("组 {0} 不再被允许建造图格ID为 {1} 图格.", args.Parameters[2], id);
						}
						else
							args.Player.SendErrorMessage("图格ID无效!");
					}
					#endregion
					return;
				case "help":
					#region Help
					{
						int pageNumber;
						if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out pageNumber))
							return;

						var lines = new List<string>
						{
							"add <图格ID>             - 禁止建造指定图格.",
							"allow <图格ID> <组名>    - 允许指定组建造被禁止的图格.",
							"del <图格ID>             - 解禁被禁止建造的图格.",
							"disallow <图格ID> <组名> - 不再允许指定组建造被禁止的图格.",
							"list [页码]              - 列出所有被禁止建造的图格."
						};

						PaginationTools.SendPage(args.Player, pageNumber, lines,
							new PaginationTools.Settings
							{
								HeaderFormat = "禁用图格使用帮助 ({0}/{1}):",
								FooterFormat = "输入 {0}tileban help {{0}} 翻页.".SFormat(Specifier)
							}
						);
					}
					#endregion
					return;
				case "list":
					#region List tile bans
					{
						int pageNumber;
						if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out pageNumber))
							return;
						IEnumerable<Int16> tileIds = from tileBan in TShock.TileBans.TileBans
													 select tileBan.ID;
						PaginationTools.SendPage(args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(tileIds),
							new PaginationTools.Settings
							{
								HeaderFormat = "禁用图格列表 ({0}/{1}):",
								FooterFormat = "输入 {0}tileban list {{0}} 翻页.".SFormat(Specifier),
								NothingToDisplayString = "服务器还没有被禁止的图格."
							});
					}
					#endregion
					return;
				default:
					#region Default
					{
						args.Player.SendErrorMessage("无效的 'tileban' 指令! 输入 {0}tileban help 获取有关'tileban'的使用帮助.", Specifier);
					}
					#endregion
					return;
			}
		}
		#endregion Tile Management

		#region Server Config Commands

		private static void SetSpawn(CommandArgs args)
		{
			Main.spawnTileX = args.Player.TileX + 1;
			Main.spawnTileY = args.Player.TileY + 3;
			SaveManager.Instance.SaveWorld(false);
			args.Player.SendSuccessMessage("已将世界出生点设置为你所在的坐标.");
		}

		private static void SetDungeon(CommandArgs args)
		{
			Main.dungeonX = args.Player.TileX + 1;
			Main.dungeonY = args.Player.TileY + 3;
			SaveManager.Instance.SaveWorld(false);
			args.Player.SendSuccessMessage("已将地牢判定点设置为你所在的坐标.");
		}

		private static void Reload(CommandArgs args)
		{
			TShock.Utils.Reload();
			Hooks.GeneralHooks.OnReloadEvent(args.Player);

			args.Player.SendSuccessMessage(
				"配置, 权限和区域重新加载完成. 一些变更可能需要重新启动服务器才会生效.");
		}

		private static void ServerPassword(CommandArgs args)
		{
			if (args.Parameters.Count != 1)
			{
				args.Player.SendErrorMessage("格式错误! 正确格式: {0}serverpassword <新进服密码>", Specifier);
				return;
			}
			string passwd = args.Parameters[0];
			TShock.Config.Settings.ServerPassword = passwd;
			args.Player.SendSuccessMessage(string.Format("服务器进服密码被更改为: {0}.", passwd));
		}

		private static void Save(CommandArgs args)
		{
			SaveManager.Instance.SaveWorld(false);
			foreach (TSPlayer tsply in TShock.Players.Where(tsply => tsply != null))
			{
				tsply.SaveServerCharacter();
			}
		}

		private static void Settle(CommandArgs args)
		{
			if (Liquid.panicMode)
			{
				args.Player.SendWarningMessage("所有液体已被安置, 不再流动!");
				return;
			}
			Liquid.StartPanic();
			args.Player.SendInfoMessage("液体已被安置.");
		}

		private static void MaxSpawns(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Player.SendInfoMessage("当前服务器最大生成量为: {0}", TShock.Config.Settings.DefaultMaximumSpawns);
				return;
			}

			if (String.Equals(args.Parameters[0], "default", StringComparison.CurrentCultureIgnoreCase))
			{
				TShock.Config.Settings.DefaultMaximumSpawns = NPC.defaultMaxSpawns = 5;
				if (args.Silent)
				{
					args.Player.SendInfoMessage("服务器最大生成量被设为: 5.");
				}
				else
				{
					TSPlayer.All.SendInfoMessage("{0} 将服务器最大生成量设为: 5.", args.Player.Name);
				}
				return;
			}

			int maxSpawns = -1;
			if (!int.TryParse(args.Parameters[0], out maxSpawns) || maxSpawns < 0 || maxSpawns > Main.maxNPCs)
			{
				args.Player.SendWarningMessage("最大生成量无效!  生成量必须介于 {0} 到 {1} 之间", 0, Main.maxNPCs);
				return;
			}

			TShock.Config.Settings.DefaultMaximumSpawns = NPC.defaultMaxSpawns = maxSpawns;
			if (args.Silent)
			{
				args.Player.SendInfoMessage("服务器最大生成量被设为{0}.", maxSpawns);
			}
			else
			{
				TSPlayer.All.SendInfoMessage("{0} 将服务器最大生成量设为 {1}.", args.Player.Name, maxSpawns);
			}
		}

		private static void SpawnRate(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Player.SendInfoMessage("当前NPC生成率为: {0}", TShock.Config.Settings.DefaultSpawnRate);
				return;
			}

			if (String.Equals(args.Parameters[0], "default", StringComparison.CurrentCultureIgnoreCase))
			{
				TShock.Config.Settings.DefaultSpawnRate = NPC.defaultSpawnRate = 600;
				if (args.Silent)
				{
					args.Player.SendInfoMessage("NPC生成率已被改为 600.");
				}
				else
				{
					TSPlayer.All.SendInfoMessage("{0} 将NPC生成率改为 600.", args.Player.Name);
				}
				return;
			}

			int spawnRate = -1;
			if (!int.TryParse(args.Parameters[0], out spawnRate) || spawnRate < 0)
			{
				args.Player.SendWarningMessage("NPC生成率无效");
				return;
			}
			TShock.Config.Settings.DefaultSpawnRate = NPC.defaultSpawnRate = spawnRate;
			if (args.Silent)
			{
				args.Player.SendInfoMessage("NPC生成率已被改为 {0}.", spawnRate);
			}
			else
			{
				TSPlayer.All.SendInfoMessage("{0} 将NPC生成率改为 {1}.", args.Player.Name, spawnRate);
			}
		}

		#endregion Server Config Commands

		#region Time/PvpFun Commands

		private static void Time(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				double time = Main.time / 3600.0;
				time += 4.5;
				if (!Main.dayTime)
					time += 15.0;
				time = time % 24.0;
				args.Player.SendInfoMessage("当前的游戏时间为 {0}:{1:D2}.", (int)Math.Floor(time), (int)Math.Floor((time % 1.0) * 60.0));
				return;
			}

			switch (args.Parameters[0].ToLower())
			{
				case "day":
					TSPlayer.Server.SetTime(true, 0.0);
					TSPlayer.All.SendInfoMessage("{0} 将游戏时间设为 4:30.", args.Player.Name);
					break;
				case "night":
					TSPlayer.Server.SetTime(false, 0.0);
					TSPlayer.All.SendInfoMessage("{0} 将游戏时间设为 19:30.", args.Player.Name);
					break;
				case "noon":
					TSPlayer.Server.SetTime(true, 27000.0);
					TSPlayer.All.SendInfoMessage("{0} 将游戏时间设为 12:00.", args.Player.Name);
					break;
				case "midnight":
					TSPlayer.Server.SetTime(false, 16200.0);
					TSPlayer.All.SendInfoMessage("{0} 将游戏时间设为 0:00.", args.Player.Name);
					break;
				default:
					string[] array = args.Parameters[0].Split(':');
					if (array.Length != 2)
					{
						args.Player.SendErrorMessage("时间格式无效! 正确格式: hh:mm, 24小时制.");
						return;
					}

					int hours;
					int minutes;
					if (!int.TryParse(array[0], out hours) || hours < 0 || hours > 23
						|| !int.TryParse(array[1], out minutes) || minutes < 0 || minutes > 59)
					{
						args.Player.SendErrorMessage("时间格式无效! 正确格式: hh:mm, 24小时制.");
						return;
					}

					decimal time = hours + (minutes / 60.0m);
					time -= 4.50m;
					if (time < 0.00m)
						time += 24.00m;

					if (time >= 15.00m)
					{
						TSPlayer.Server.SetTime(false, (double)((time - 15.00m) * 3600.0m));
					}
					else
					{
						TSPlayer.Server.SetTime(true, (double)(time * 3600.0m));
					}
					TSPlayer.All.SendInfoMessage("{0} 将游戏时间设为 {1}:{2:D2}.", args.Player.Name, hours, minutes);
					break;
			}
		}

		private static void Slap(CommandArgs args)
		{
			if (args.Parameters.Count < 1 || args.Parameters.Count > 2)
			{
				args.Player.SendErrorMessage("格式错误! 正确格式: {0}slap <玩家名> [伤害]", Specifier);
				return;
			}
			if (args.Parameters[0].Length == 0)
			{
				args.Player.SendErrorMessage("玩家名无效!");
				return;
			}

			string plStr = args.Parameters[0];
			var players = TSPlayer.FindByNameOrID(plStr);
			if (players.Count == 0)
			{
				args.Player.SendErrorMessage("玩家名无效!");
			}
			else if (players.Count > 1)
			{
				args.Player.SendMultipleMatchError(players.Select(p => p.Name));
			}
			else
			{
				var plr = players[0];
				int damage = 5;
				if (args.Parameters.Count == 2)
				{
					int.TryParse(args.Parameters[1], out damage);
				}
				if (!args.Player.HasPermission(Permissions.kill))
				{
					damage = TShock.Utils.Clamp(damage, 15, 0);
				}
				plr.DamagePlayer(damage);
				TSPlayer.All.SendInfoMessage("{0} 扇了 {1} 一巴掌造成了{2}点伤害.", args.Player.Name, plr.Name, damage);
				TShock.Log.Info("{0} 扇了 {1} 一巴掌造成了{2}点伤害.", args.Player.Name, plr.Name, damage);
			}
		}

		private static void Wind(CommandArgs args)
		{
			if (args.Parameters.Count != 1)
			{
				args.Player.SendErrorMessage("格式错误! 正确格式: {0}wind <风速>", Specifier);
				return;
			}

			int speed;
			if (!int.TryParse(args.Parameters[0], out speed) || speed * 100 < 0)
			{
				args.Player.SendErrorMessage("风速无效!");
				return;
			}

			Main.windSpeedCurrent = speed;
			Main.windSpeedTarget = speed;
			TSPlayer.All.SendData(PacketTypes.WorldInfo);
			TSPlayer.All.SendInfoMessage("{0} 将风速改为 {1} mph.", args.Player.Name, speed);
		}

		#endregion Time/PvpFun Commands

		#region Region Commands

		private static void Region(CommandArgs args)
		{
			string cmd = "help";
			if (args.Parameters.Count > 0)
			{
				cmd = args.Parameters[0].ToLower();
			}
			switch (cmd)
			{
				case "name":
					{
						{
							args.Player.SendInfoMessage("敲击一个区域的物块获得区域名称");
							args.Player.AwaitingName = true;
							args.Player.AwaitingNameParameters = args.Parameters.Skip(1).ToArray();
						}
						break;
					}
				case "set":
					{
						int choice = 0;
						if (args.Parameters.Count == 2 &&
							int.TryParse(args.Parameters[1], out choice) &&
							choice >= 1 && choice <= 2)
						{
							args.Player.SendInfoMessage("敲击一个物块设置边角 " + choice);
							args.Player.AwaitingTempPoint = choice;
						}
						else
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: /region set <1/2>");
						}
						break;
					}
				case "define":
					{
						if (args.Parameters.Count > 1)
						{
							if (!args.Player.TempPoints.Any(p => p == Point.Zero))
							{
								string regionName = String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
								var x = Math.Min(args.Player.TempPoints[0].X, args.Player.TempPoints[1].X);
								var y = Math.Min(args.Player.TempPoints[0].Y, args.Player.TempPoints[1].Y);
								var width = Math.Abs(args.Player.TempPoints[0].X - args.Player.TempPoints[1].X);
								var height = Math.Abs(args.Player.TempPoints[0].Y - args.Player.TempPoints[1].Y);

								if (TShock.Regions.AddRegion(x, y, width, height, regionName, args.Player.Account.Name,
															 Main.worldID.ToString()))
								{
									args.Player.TempPoints[0] = Point.Zero;
									args.Player.TempPoints[1] = Point.Zero;
									args.Player.SendInfoMessage("成功设置了区域 " + regionName);
								}
								else
								{
									args.Player.SendErrorMessage("区域 " + regionName + " 已存在");
								}
							}
							else
							{
								args.Player.SendErrorMessage("尚未设置区域的边角");
							}
						}
						else
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}region define <区域名>", Specifier);
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
									args.Player.SendInfoMessage("区域 " + regionName + "已被保护");
								else
									args.Player.SendErrorMessage("没有找到该区域");
							}
							else if (args.Parameters[2].ToLower() == "false")
							{
								if (TShock.Regions.SetRegionState(regionName, false))
									args.Player.SendInfoMessage("区域 " + regionName + "已被解除保护");
								else
									args.Player.SendErrorMessage("没有找到该区域");
							}
							else
								args.Player.SendErrorMessage("格式错误! 正确格式: {0}region protect <区域名> <true/false>", Specifier);
						}
						else
							args.Player.SendErrorMessage("格式错误! 正确格式: /region protect <区域名> <true/false>", Specifier);
						break;
					}
				case "del":
				case "delete":
					{
						if (args.Parameters.Count > 1)
						{
							string regionName = String.Join(" ", args.Parameters.GetRange(1, args.Parameters.Count - 1));
							if (TShock.Regions.DeleteRegion(regionName))
							{
								args.Player.SendInfoMessage("区域 \"{0}\" 被删除.", regionName);
							}
							else
								args.Player.SendErrorMessage("没有找到该区域!");
						}
						else
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}region delete <区域名>", Specifier);
						break;
					}
				case "clear":
					{
						args.Player.TempPoints[0] = Point.Zero;
						args.Player.TempPoints[1] = Point.Zero;
						args.Player.SendInfoMessage("临时边角点已被清除.");
						args.Player.AwaitingTempPoint = 0;
						break;
					}
				case "allow":
					{
						if (args.Parameters.Count > 2)
						{
							string playerName = args.Parameters[1];
							string regionName = "";

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
							if (TShock.UserAccounts.GetUserAccountByName(playerName) != null)
							{
								if (TShock.Regions.AddNewUser(regionName, playerName))
								{
									args.Player.SendInfoMessage("玩家 " + playerName + " 已被允许进入 " + regionName);
								}
								else
									args.Player.SendErrorMessage("没有找到名为 " + regionName + " 的区域");
							}
							else
							{
								args.Player.SendErrorMessage("没有找到名为 " + playerName + " 的玩家");
							}
						}
						else
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}region allow <玩家名> <区域名 >", Specifier);
						break;
					}
				case "remove":
					if (args.Parameters.Count > 2)
					{
						string playerName = args.Parameters[1];
						string regionName = "";

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
						if (TShock.UserAccounts.GetUserAccountByName(playerName) != null)
						{
							if (TShock.Regions.RemoveUser(regionName, playerName))
							{
								args.Player.SendInfoMessage("玩家 " + playerName + " 无法再进入区域 " + regionName);
							}
							else
								args.Player.SendErrorMessage("没有找到名为 " + regionName + " 的区域");
						}
						else
						{
							args.Player.SendErrorMessage("没有找到名为 " + playerName + " 的玩家");
						}
					}
					else
						args.Player.SendErrorMessage("格式错误! 正确格式: {0}region remove <玩家名> <区域名>", Specifier);
					break;
				case "allowg":
					{
						if (args.Parameters.Count > 2)
						{
							string group = args.Parameters[1];
							string regionName = "";

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
							if (TShock.Groups.GroupExists(group))
							{
								if (TShock.Regions.AllowGroup(regionName, group))
								{
									args.Player.SendInfoMessage("已允许组 " + group + " 进入 " + regionName);
								}
								else
									args.Player.SendErrorMessage("没有找到名为 " + regionName + " 的区域");
							}
							else
							{
								args.Player.SendErrorMessage("没有找到名为 " + group + " 的组");
							}
						}
						else
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}region allowg <组名> <区域名>", Specifier);
						break;
					}
				case "removeg":
					if (args.Parameters.Count > 2)
					{
						string group = args.Parameters[1];
						string regionName = "";

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
						if (TShock.Groups.GroupExists(group))
						{
							if (TShock.Regions.RemoveGroup(regionName, group))
							{
								args.Player.SendInfoMessage("不再允许组 " + group + " 进入区域 " + regionName);
							}
							else
								args.Player.SendErrorMessage("没有找到名为 " + regionName + " 的区域");
						}
						else
						{
							args.Player.SendErrorMessage("没有找到名为 " + group + " 的组");
						}
					}
					else
						args.Player.SendErrorMessage("格式错误! 正确格式: {0}region removeg <组名> <区域名>", Specifier);
					break;
				case "list":
					{
						int pageNumber;
						if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out pageNumber))
							return;

						IEnumerable<string> regionNames = from region in TShock.Regions.Regions
														  where region.WorldID == Main.worldID.ToString()
														  select region.Name;
						PaginationTools.SendPage(args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(regionNames),
							new PaginationTools.Settings
							{
								HeaderFormat = "区域列表 ({0}/{1}):",
								FooterFormat = "输入 {0}region list {{0}} 翻页.".SFormat(Specifier),
								NothingToDisplayString = "服务器还没有设置区域哦."
							});
						break;
					}
				case "info":
					{
						if (args.Parameters.Count == 1 || args.Parameters.Count > 4)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}region info <区域> [-d] [页码]", Specifier);
							break;
						}

						string regionName = args.Parameters[1];
						bool displayBoundaries = args.Parameters.Skip(2).Any(
							p => p.Equals("-d", StringComparison.InvariantCultureIgnoreCase)
						);

						Region region = TShock.Regions.GetRegionByName(regionName);
						if (region == null)
						{
							args.Player.SendErrorMessage("没有找到名为 \"{0}\" 的区域.", regionName);
							break;
						}

						int pageNumberIndex = displayBoundaries ? 3 : 2;
						int pageNumber;
						if (!PaginationTools.TryParsePageNumber(args.Parameters, pageNumberIndex, args.Player, out pageNumber))
							break;

						List<string> lines = new List<string>
						{
							string.Format("X: {0}; Y: {1}; W: {2}; H: {3}, Z: {4}", region.Area.X, region.Area.Y, region.Area.Width, region.Area.Height, region.Z),
							string.Concat("所有者: ", region.Owner),
							string.Concat("区域保护: ", region.DisableBuild.ToString()),
						};

						if (region.AllowedIDs.Count > 0)
						{
							IEnumerable<string> sharedUsersSelector = region.AllowedIDs.Select(userId =>
							{
								UserAccount account = TShock.UserAccounts.GetUserAccountByID(userId);
								if (account != null)
									return account.Name;

								return string.Concat("{ID: ", userId, "}");
							});
							List<string> extraLines = PaginationTools.BuildLinesFromTerms(sharedUsersSelector.Distinct());
							extraLines[0] = "共享用户: " + extraLines[0];
							lines.AddRange(extraLines);
						}
						else
						{
							lines.Add("区域没有与任何用户共享.");
						}

						if (region.AllowedGroups.Count > 0)
						{
							List<string> extraLines = PaginationTools.BuildLinesFromTerms(region.AllowedGroups.Distinct());
							extraLines[0] = "共享组: " + extraLines[0];
							lines.AddRange(extraLines);
						}
						else
						{
							lines.Add("区域没有与任何组共享.");
						}

						PaginationTools.SendPage(
							args.Player, pageNumber, lines, new PaginationTools.Settings
							{
								HeaderFormat = string.Format("有关区域 \"{0}\" 的信息 ({{0}}/{{1}}):", region.Name),
								FooterFormat = string.Format("输入 {0}region info {1} {{0}} 翻页.", Specifier, regionName)
							}
						);

						if (displayBoundaries)
						{
							Rectangle regionArea = region.Area;
							foreach (Point boundaryPoint in Utils.Instance.EnumerateRegionBoundaries(regionArea))
							{
								// Preferring dotted lines as those should easily be distinguishable from actual wires.
								if ((boundaryPoint.X + boundaryPoint.Y & 1) == 0)
								{
									// Could be improved by sending raw tile data to the client instead but not really
									// worth the effort as chances are very low that overwriting the wire for a few
									// nanoseconds will cause much trouble.
									ITile tile = Main.tile[boundaryPoint.X, boundaryPoint.Y];
									bool oldWireState = tile.wire();
									tile.wire(true);

									try
									{
										args.Player.SendTileSquareCentered(boundaryPoint.X, boundaryPoint.Y, 1);
									}
									finally
									{
										tile.wire(oldWireState);
									}
								}
							}

							Timer boundaryHideTimer = null;
							boundaryHideTimer = new Timer((state) =>
							{
								foreach (Point boundaryPoint in Utils.Instance.EnumerateRegionBoundaries(regionArea))
									if ((boundaryPoint.X + boundaryPoint.Y & 1) == 0)
										args.Player.SendTileSquareCentered(boundaryPoint.X, boundaryPoint.Y, 1);

								Debug.Assert(boundaryHideTimer != null);
								boundaryHideTimer.Dispose();
							},
								null, 5000, Timeout.Infinite
							);
						}

						break;
					}
				case "z":
					{
						if (args.Parameters.Count == 3)
						{
							string regionName = args.Parameters[1];
							int z = 0;
							if (int.TryParse(args.Parameters[2], out z))
							{
								if (TShock.Regions.SetZ(regionName, z))
									args.Player.SendInfoMessage("区域的z现在是 " + z);
								else
									args.Player.SendErrorMessage("没有找到指定区域");
							}
							else
								args.Player.SendErrorMessage("格式错误! 正确格式: {0}region z <区域名> <#>", Specifier);
						}
						else
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}region z <区域名> <#>", Specifier);
						break;
					}
				case "resize":
				case "expand":
					{
						if (args.Parameters.Count == 4)
						{
							int direction;
							switch (args.Parameters[2])
							{
								case "向上":
								case "上":
								case "u":
								case "up":
									{
										direction = 0;
										break;
									}

								case "向右":
								case "右":
								case "r":
								case "right":
									{
										direction = 1;
										break;
									}
								case "向下":
								case "下":
								case "d":
								case "down":
									{
										direction = 2;
										break;
									}
								case "向左":
								case "左":
								case "l":
								case "left":
									{
										direction = 3;
										break;
									}
								default:
									{
										direction = -1;
										break;
									}
							}
							int addAmount;
							int.TryParse(args.Parameters[3], out addAmount);
							if (TShock.Regions.ResizeRegion(args.Parameters[1], addAmount, direction))
							{
								args.Player.SendInfoMessage("成功调整区域大小!");
								TShock.Regions.Reload();
							}
							else
								args.Player.SendErrorMessage("格式错误! 正确格式: {0}region resize <区域名> <u(上)/d(下)/l(左)/r(右)> <单位>", Specifier);
						}
						else
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}region resize <区域名> <u(上)/d(下)/l(左)/r(右)> <单位>", Specifier);
						break;
					}
				case "rename":
					{
						if (args.Parameters.Count != 3)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}region rename <区域名> <新区域名>", Specifier);
							break;
						}
						else
						{
							string oldName = args.Parameters[1];
							string newName = args.Parameters[2];

							if (oldName == newName)
							{
								args.Player.SendErrorMessage("错误: 你修改的区域名不能和旧区域名一致.");
								break;
							}

							Region oldRegion = TShock.Regions.GetRegionByName(oldName);

							if (oldRegion == null)
							{
								args.Player.SendErrorMessage("不存在区域 \"{0}\".", oldName);
								break;
							}

							Region newRegion = TShock.Regions.GetRegionByName(newName);

							if (newRegion != null)
							{
								args.Player.SendErrorMessage("区域 \"{0}\" 已存在.", newName);
								break;
							}

							if (TShock.Regions.RenameRegion(oldName, newName))
							{
								args.Player.SendInfoMessage("成功重命名该区域!");
							}
							else
							{
								args.Player.SendErrorMessage("重命名区域失败.");
							}
						}
						break;
					}
				case "tp":
					{
						if (!args.Player.HasPermission(Permissions.tp))
						{
							args.Player.SendErrorMessage("你没有权限进行传送.");
							break;
						}
						if (args.Parameters.Count <= 1)
						{
							args.Player.SendErrorMessage("格式错误! 正确格式: {0}region tp <区域名>.", Specifier);
							break;
						}

						string regionName = string.Join(" ", args.Parameters.Skip(1));
						Region region = TShock.Regions.GetRegionByName(regionName);
						if (region == null)
						{
							args.Player.SendErrorMessage("没有找到名为 \"{0}\" 的区域.", regionName);
							break;
						}

						args.Player.Teleport(region.Area.Center.X * 16, region.Area.Center.Y * 16);
						break;
					}
				case "help":
				default:
					{
						int pageNumber;
						int pageParamIndex = 0;
						if (args.Parameters.Count > 1)
							pageParamIndex = 1;
						if (!PaginationTools.TryParsePageNumber(args.Parameters, pageParamIndex, args.Player, out pageNumber))
							return;

						List<string> lines = new List<string> {
						  "set <1/2> - 将物块标记为临时区域边角点.",
						  "clear - 清除所有设置的临时区域边角点.",
						  "define <区域名> - 添加一个区域并为它取个名字.",
						  "delete <区域名> - 删除指定区域.",
						  "name [-u][-z][-p] - 在指定点显示该区域的名称.",
						  "rename <区域名> <新区域名> - 重命名一个区域.",
						  "list - 列出所有区域.",
						  "resize <区域名> <u/d/l/r> <数量> - 调整区域大小.",
						  "allow <用户名> <区域名> - 允许指定用户进入区域.",
						  "remove <用户名> <区域名> - 不再允许指定用户进入区域.",
						  "allowg <组名> <区域名> - 允许指定组进入区域.",
						  "removeg <组名> <区域名> - 不再允许指定组进入区域.",
						  "info <区域名> [-d] - 显示关于区域的信息.",
						  "protect <区域名> <true/false> - 切换区域保护.",
						  "z <区域名> <#> - 设置区域的z轴次序.",
						};
						if (args.Player.HasPermission(Permissions.tp))
							lines.Add("tp <区域名> - 将你传送至指定区域的中心位置.");

						PaginationTools.SendPage(
						  args.Player, pageNumber, lines,
						  new PaginationTools.Settings
						  {
							  HeaderFormat = "区域指令列表 ({0}/{1}):",
							  FooterFormat = "输入 {0}region {{0}} 翻页.".SFormat(Specifier)
						  }
						);
						break;
					}
			}
		}

		#endregion Region Commands

		#region World Protection Commands

		private static void ToggleAntiBuild(CommandArgs args)
		{
			TShock.Config.Settings.DisableBuild = !TShock.Config.Settings.DisableBuild;
			TSPlayer.All.SendSuccessMessage(string.Format("建筑保护已 {0}.", (TShock.Config.Settings.DisableBuild ? "开启" : "关闭")));
		}

		private static void ProtectSpawn(CommandArgs args)
		{
			TShock.Config.Settings.SpawnProtection = !TShock.Config.Settings.SpawnProtection;
			TSPlayer.All.SendSuccessMessage(string.Format("出生点保护已 {0}.", (TShock.Config.Settings.SpawnProtection ? "开启" : "关闭")));
		}

		#endregion World Protection Commands

		#region General Commands

		private static void Help(CommandArgs args)
		{
			if (args.Parameters.Count > 1)
			{
				args.Player.SendErrorMessage("格式错误! 正确格式: {0}help <指令/页码>", Specifier);
				return;
			}

			int pageNumber;
			if (args.Parameters.Count == 0 || int.TryParse(args.Parameters[0], out pageNumber))
			{
				if (!PaginationTools.TryParsePageNumber(args.Parameters, 0, args.Player, out pageNumber))
				{
					return;
				}
				IEnumerable<string> cmdNames = from cmd in ChatCommands
											   where cmd.CanRun(args.Player) && (cmd.Name != "setup" || TShock.SetupToken != 0)
											   select Specifier + cmd.Name +(cmd.Show==null ? "" : $"({cmd.Show})");

				PaginationTools.SendPage(args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(cmdNames),
					new PaginationTools.Settings
					{
						HeaderFormat = "指令列表 ({0}/{1}):",
						FooterFormat = "输入 {0}help {{0}} 翻页.".SFormat(Specifier)
					});
			}
			else
			{
				string commandName = args.Parameters[0].ToLower();
				if (commandName.StartsWith(Specifier))
				{
					commandName = commandName.Substring(1);
				}

				Command command = ChatCommands.Find(c => c.Names.Contains(commandName));
				if (command == null)
				{
					args.Player.SendErrorMessage("指令无效.");
					return;
				}
				if (!command.CanRun(args.Player))
				{
					args.Player.SendErrorMessage("你没有权限使用这个命令哦.");
					return;
				}

				args.Player.SendSuccessMessage("{0}{1} help: ", Specifier, command.Name);
				if (command.HelpDesc == null)
				{
					args.Player.SendInfoMessage(command.HelpText);
					return;
				}
				foreach (string line in command.HelpDesc)
				{
					args.Player.SendInfoMessage(line);
				}
			}
		}

		private static void GetVersion(CommandArgs args)
		{
			args.Player.SendMessage($"TShock: {TShock.VersionNum.Color(Utils.BoldHighlight)} {TShock.VersionCodename.Color(Utils.RedHighlight)}.", Color.White);
		}

		private static void ListConnectedPlayers(CommandArgs args)
		{
			bool invalidUsage = (args.Parameters.Count > 2);

			bool displayIdsRequested = false;
			int pageNumber = 1;
			if (!invalidUsage)
			{
				foreach (string parameter in args.Parameters)
				{
					if (parameter.Equals("-i", StringComparison.InvariantCultureIgnoreCase))
					{
						displayIdsRequested = true;
						continue;
					}

					if (!int.TryParse(parameter, out pageNumber))
					{
						invalidUsage = true;
						break;
					}
				}
			}
			if (invalidUsage)
			{
				args.Player.SendMessage($"列出在线的玩家", Color.White);
				args.Player.SendMessage($"{"在线玩家"} {"[-i]"} {"[页码]"}", Color.White);
				args.Player.SendMessage($"指令别名: {"playing".Color(Utils.GreenHighlight)}, {"online".Color(Utils.GreenHighlight)}, {"who".Color(Utils.GreenHighlight)}", Color.White);
				args.Player.SendMessage($"例子: {"who".Color(Utils.BoldHighlight)} {"-i".Color(Utils.RedHighlight)}", Color.White);
				return;
			}
			if (displayIdsRequested && !args.Player.HasPermission(Permissions.seeids))
			{
				args.Player.SendErrorMessage("你没有权限看到用户ID.");
				return;
			}

			if (TShock.Utils.GetActivePlayerCount() == 0)
			{
				args.Player.SendMessage("怪,服务器居然一个人也没有 :(.", Color.White);
				return;
			}
			args.Player.SendMessage($"在线玩家 ({TShock.Utils.GetActivePlayerCount()}/{TShock.Config.Settings.MaxSlots})", Color.White);

			var players = new List<string>();

			foreach (TSPlayer ply in TShock.Players)
			{
				if (ply != null && ply.Active)
				{
					if (displayIdsRequested)
						players.Add($"{ply.Name} (玩家索引: {ply.Index}{(ply.Account != null ? ", 账号ID: " + ply.Account.ID : "")})");
					else
						players.Add(ply.Name);
				}
			}

			PaginationTools.SendPage(
				args.Player, pageNumber, PaginationTools.BuildLinesFromTerms(players),
				new PaginationTools.Settings
				{
					IncludeHeader = false,
					FooterFormat = $"输入 {Specifier}who {(displayIdsRequested ? "-i" : string.Empty)}{Specifier} 获取在线详细信息."
				}
			);
		}

		private static void SetupToken(CommandArgs args)
		{
			if (TShock.SetupToken == 0)
			{
				args.Player.SendWarningMessage("超级管理员验证系统被禁用. 此事件已记录.");
				args.Player.SendWarningMessage("如果你无法获取超级管理员权限, 可以访问 https://tshock.co/ 获得更多帮助");
				TShock.Log.Warn("{0} 尝试使用被禁用的超级管理员验证系统.", args.Player.IP);
				return;
			}

			// If the user account is already logged in, turn off the setup system
			if (args.Player.IsLoggedIn && args.Player.tempGroup == null)
			{
				args.Player.SendSuccessMessage("你的账号已通过验证, 且 {0}setup 系统已被关闭.", Specifier);
				args.Player.SendSuccessMessage("在Bilibili上分享你的服务器吧, 和你的管理员交流经验, 在TShock官方交流群(816771079)获取帮助. -- https://tshock.co/");
				args.Player.SendSuccessMessage("再次感谢你选择使用TShock!");
				FileTools.CreateFile(Path.Combine(TShock.SavePath, "setup.lock"));
				File.Delete(Path.Combine(TShock.SavePath, "setup-code.txt"));
				TShock.SetupToken = 0;
				return;
			}

			if (args.Parameters.Count == 0)
			{
				args.Player.SendErrorMessage("你必须提供超级管理员验证码!");
				return;
			}

			int givenCode;
			if (!Int32.TryParse(args.Parameters[0], out givenCode) || givenCode != TShock.SetupToken)
			{
				args.Player.SendErrorMessage("验证码不正确. 此事件已被后台记录.");
				TShock.Log.Warn(args.Player.IP + " 尝试使用不正确的超级管理员验证码.");
				return;
			}

			if (args.Player.Group.Name != "superadmin")
				args.Player.tempGroup = new SuperAdminGroup();

			args.Player.SendInfoMessage("已为你提供了临时系统访问权限, 你现在可以使用任何指令.");
			args.Player.SendWarningMessage("你可以使用如下指令创建永久管理账号.");
			args.Player.SendWarningMessage("{0}user add <用户名> <密码> owner", Specifier);
			args.Player.SendInfoMessage("创建: 用户组为 owner 密码为 <密码> 的 <用户>.");
			args.Player.SendInfoMessage("请使用 {0}login <用户名> <密码> 登录你的永久管理账号.", Specifier);
			args.Player.SendWarningMessage("如果你理解了是什么意思, 先在就用 {0}login <username> <password> 登录你的永久账号, 然后输入 {0}setup.", Specifier);
			return;
		}

		private static void ThirdPerson(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Player.SendErrorMessage("格式错误! 正确格式: {0}me <文本>", Specifier);
				return;
			}
			if (args.Player.mute)
				args.Player.SendErrorMessage("你已被禁言.");
			else
				TSPlayer.All.SendMessage(string.Format("*{0} {1}", args.Player.Name, String.Join(" ", args.Parameters)), 205, 133, 63);
		}

		private static void PartyChat(CommandArgs args)
		{
			if (args.Parameters.Count == 0)
			{
				args.Player.SendErrorMessage("格式错误! 正确格式: {0}p <队伍内聊天文本>", Specifier);
				return;
			}
			int playerTeam = args.Player.Team;

			if (args.Player.mute)
				args.Player.SendErrorMessage("你已被禁言.");
			else if (playerTeam != 0)
			{
				string msg = string.Format("<{0}> {1}", args.Player.Name, String.Join(" ", args.Parameters));
				foreach (TSPlayer player in TShock.Players)
				{
					if (player != null && player.Active && player.Team == playerTeam)
						player.SendMessage(msg, Main.teamColor[playerTeam].R, Main.teamColor[playerTeam].G, Main.teamColor[playerTeam].B);
				}
			}
			else
				args.Player.SendErrorMessage("你不在任何队伍中!");
		}

		private static void Mute(CommandArgs args)
		{
			if (args.Parameters.Count < 1)
			{
				args.Player.SendMessage("禁言", Color.White);
				args.Player.SendMessage($"{"mute".Color(Utils.BoldHighlight)} <{"player".Color(Utils.RedHighlight)}> [{"reason".Color(Utils.GreenHighlight)}]", Color.White);
				args.Player.SendMessage($"示例: {"mute".Color(Utils.BoldHighlight)} \"{args.Player.Name.Color(Utils.RedHighlight)}\" \"{"不可以骂人哦".Color(Utils.GreenHighlight)}\"", Color.White);
				args.Player.SendMessage($"如果你想禁言玩家并不发送广播, 你可以使用静默指令符 {SilentSpecifier.Color(Utils.GreenHighlight)} 替代 {Specifier.Color(Utils.RedHighlight)}", Color.White);
				return;
			}

			var players = TSPlayer.FindByNameOrID(args.Parameters[0]);
			if (players.Count == 0)
			{
				args.Player.SendErrorMessage($"没有找到名为 \"{args.Parameters[0]}\" 的玩家");
			}
			else if (players.Count > 1)
			{
				args.Player.SendMultipleMatchError(players.Select(p => p.Name));
			}
			else if (players[0].HasPermission(Permissions.mute))
			{
				args.Player.SendErrorMessage($"你没有权限禁言 {players[0].Name}");
			}
			else if (players[0].mute)
			{
				var plr = players[0];
				plr.mute = false;
				if (args.Silent)
					args.Player.SendSuccessMessage($"你解除了 {plr.Name} 的禁言.");
				else
					TSPlayer.All.SendInfoMessage($"{args.Player.Name} 解除了 {plr.Name} 的禁言.");
			}
			else
			{
				string reason = "没有理由.";
				if (args.Parameters.Count > 1)
					reason = String.Join(" ", args.Parameters.ToArray(), 1, args.Parameters.Count - 1);
				var plr = players[0];
				plr.mute = true;
				if (args.Silent)
					args.Player.SendSuccessMessage($"你成功以 {reason} 的理由禁言 {plr.Name}");
				else
					TSPlayer.All.SendInfoMessage($"{args.Player.Name} 以 {reason} 的理由禁言 {plr.Name}.");
			}
		}

		private static void Motd(CommandArgs args)
		{
			args.Player.SendFileTextAsMessage(FileTools.MotdPath);
		}

		private static void Rules(CommandArgs args)
		{
			args.Player.SendFileTextAsMessage(FileTools.RulesPath);
		}

		public static void Whisper(CommandArgs args)
		{
			if (args.Parameters.Count < 2)
			{
				args.Player.SendMessage("私聊无效", Color.White);
				args.Player.SendMessage($"{"whisper".Color(Utils.BoldHighlight)} <{"玩家名".Color(Utils.RedHighlight)}> <{"消息".Color(Utils.PinkHighlight)}>", Color.White);
				args.Player.SendMessage($"示例: {"w".Color(Utils.BoldHighlight)} {args.Player.Name.Color(Utils.RedHighlight)} {"Woc, 迅猛龙实在是太帅啦！！！.".Color(Utils.PinkHighlight)}", Color.White);
				return;
			}
			var players = TSPlayer.FindByNameOrID(args.Parameters[0]);
			if (players.Count == 0)
			{
				args.Player.SendErrorMessage($"没有找到名为 \"{args.Parameters[0]}\" 的玩家");
			}
			else if (players.Count > 1)
			{
				args.Player.SendMultipleMatchError(players.Select(p => p.Name));
			}
			else if (args.Player.mute)
			{
				args.Player.SendErrorMessage("你已被禁言.");
			}
			else
			{
				var plr = players[0];
				if (plr == args.Player)
				{
					args.Player.SendErrorMessage("你不可以给自己发私聊哦.");
					return;
				}
				if (!plr.AcceptingWhispers)
				{
					args.Player.SendErrorMessage($"玩家 {plr.Name} 不接受任何私聊.");
					return;
				}
				var msg = string.Join(" ", args.Parameters.ToArray(), 1, args.Parameters.Count - 1);
				plr.SendMessage($"<来自 {args.Player.Name}> {msg}", Color.MediumPurple);
				args.Player.SendMessage($"<发送给 {plr.Name}> {msg}", Color.MediumPurple);
				plr.LastWhisper = args.Player;
				args.Player.LastWhisper = plr;
			}
		}

		private static void Wallow(CommandArgs args)
		{
			args.Player.AcceptingWhispers = !args.Player.AcceptingWhispers;
			args.Player.SendSuccessMessage($"你{(args.Player.AcceptingWhispers ? "将会收到" : "不再收到任何")}玩家的私聊.");
			args.Player.SendMessage($"可以使用 {Specifier.Color(Utils.GreenHighlight)}{"wa".Color(Utils.GreenHighlight)} 切换此设置.", Color.White);
		}

		private static void Reply(CommandArgs args)
		{
			if (args.Player.mute)
			{
				args.Player.SendErrorMessage("你已被禁言.");
			}
			else if (args.Player.LastWhisper != null && args.Player.LastWhisper.Active)
			{
				if (!args.Player.LastWhisper.AcceptingWhispers)
				{
					args.Player.SendErrorMessage($"玩家 {args.Player.LastWhisper.Name} 不接受任何私聊.");
					return;
				}
				var msg = string.Join(" ", args.Parameters);
				args.Player.LastWhisper.SendMessage($"<来自 {args.Player.Name}> {msg}", Color.MediumPurple);
				args.Player.SendMessage($"<发送到 {args.Player.LastWhisper.Name}> {msg}", Color.MediumPurple);
			}
			else if (args.Player.LastWhisper != null)
			{
				args.Player.SendErrorMessage($"玩家 {args.Player.LastWhisper.Name} 处于离线状态, 他无法收到你的回复.");
			}
			else
			{
				args.Player.SendErrorMessage("你还没有收到任何私聊哦.");
				args.Player.SendMessage($"你可以使用 {Specifier.Color(Utils.GreenHighlight)}{"w".Color(Utils.GreenHighlight)} 私聊一个玩家.", Color.White);
			}
		}

		private static void Annoy(CommandArgs args)
		{
			if (args.Parameters.Count != 2)
			{
				args.Player.SendMessage("烦死人指令使用帮助", Color.White);
				args.Player.SendMessage($"{"annoy".Color(Utils.BoldHighlight)} <{"玩家名".Color(Utils.RedHighlight)}> <{"持续时间(秒)".Color(Utils.PinkHighlight)}>", Color.White);
				args.Player.SendMessage($"示例: {"annoy".Color(Utils.BoldHighlight)} <{args.Player.Name.Color(Utils.RedHighlight)}> <{"10".Color(Utils.PinkHighlight)}>", Color.White);
				args.Player.SendMessage($"如果你不想给该玩家发送提示, 你可以使用静默指令符 {SilentSpecifier.Color(Utils.GreenHighlight)} 替代 {Specifier.Color(Utils.RedHighlight)}.", Color.White);
				return;
			}
			int annoy = 5;
			int.TryParse(args.Parameters[1], out annoy);

			var players = TSPlayer.FindByNameOrID(args.Parameters[0]);
			if (players.Count == 0)
				args.Player.SendErrorMessage($"没有找到名为 \"{args.Parameters[0]}\" 的玩家");
			else if (players.Count > 1)
				args.Player.SendMultipleMatchError(players.Select(p => p.Name));
			else
			{
				var ply = players[0];
				args.Player.SendSuccessMessage($"开始烦玩家 {ply.Name} 持续 {annoy} 秒.");
				if (!args.Silent)
					ply.SendMessage("你现在要被烦了.", Color.LightGoldenrodYellow);
				new Thread(ply.Whoopie).Start(annoy);
			}
		}

		private static void Rocket(CommandArgs args)
		{
			if (args.Parameters.Count != 1)
			{
				args.Player.SendMessage("上天指令使用帮助", Color.White);
				args.Player.SendMessage($"{"rocket".Color(Utils.BoldHighlight)} <{"玩家名".Color(Utils.RedHighlight)}>", Color.White);
				args.Player.SendMessage($"示例: {"rocket".Color(Utils.BoldHighlight)} {args.Player.Name.Color(Utils.RedHighlight)}", Color.White);
				args.Player.SendMessage($"如果你不想发送广播, 你可以使用静默指令符 {SilentSpecifier.Color(Utils.GreenHighlight)} 替代 {Specifier.Color(Utils.RedHighlight)}.", Color.White);
				return;
			}
			var players = TSPlayer.FindByNameOrID(args.Parameters[0]);
			if (players.Count == 0)
				args.Player.SendErrorMessage($"没有找到名为 \"{args.Parameters[0]}\" 的玩家");
			else if (players.Count > 1)
				args.Player.SendMultipleMatchError(players.Select(p => p.Name));
			else
			{
				var target = players[0];

				if (target.IsLoggedIn && Main.ServerSideCharacter)
				{
					target.TPlayer.velocity.Y = -50;
					TSPlayer.All.SendData(PacketTypes.PlayerUpdate, "", target.Index);

					if (!args.Silent)
					{
						TSPlayer.All.SendInfoMessage($"玩家 {args.Player.Name} 将{(target == args.Player ? (args.Player.TPlayer.Male ? "他自己" : "她自己") : target.Name)}扔进了太空.");
						return;
					}

					if (target == args.Player)
						args.Player.SendSuccessMessage("你飞向了太空.");
					else
						args.Player.SendSuccessMessage($"你将 {target.Name} 扔进了太空.");
				}
				else
				{
					if (!Main.ServerSideCharacter)
						args.Player.SendErrorMessage("服务器必须打开SSC云存档才能使用这个功能.");
					else
						args.Player.SendErrorMessage($"无法让 {target.Name} 上天, 因为{(target.TPlayer.Male ? "他" : "她")}没有登录.");
				}
			}
		}

		private static void FireWork(CommandArgs args)
		{
			var user = args.Player;
			if (args.Parameters.Count < 1)
			{
				// firework <player> [R|G|B|Y]
				user.SendMessage("烟花指令使用帮助", Color.White);
				user.SendMessage($"{"firework".Color(Utils.CyanHighlight)} <{"玩家名".Color(Utils.PinkHighlight)}> [{"R(红)".Color(Utils.RedHighlight)}|{"G(绿)".Color(Utils.GreenHighlight)}|{"B(蓝)".Color(Utils.BoldHighlight)}|{"Y(黄)".Color(Utils.YellowHighlight)}]", Color.White);
				user.SendMessage($"示例: {"firework".Color(Utils.CyanHighlight)} {user.Name.Color(Utils.PinkHighlight)} {"R".Color(Utils.RedHighlight)}", Color.White);
				user.SendMessage($"如果你不想为玩家发送提示信息, 你可以使用静默指令符 {SilentSpecifier.Color(Utils.GreenHighlight)} 替代 {Specifier.Color(Utils.RedHighlight)}.", Color.White);
				return;
			}
			var players = TSPlayer.FindByNameOrID(args.Parameters[0]);
			if (players.Count == 0)
				user.SendErrorMessage($"没有找到名为 \"{args.Parameters[0]}\" 的玩家");
			else if (players.Count > 1)
				user.SendMultipleMatchError(players.Select(p => p.Name));
			else
			{
				int type = ProjectileID.RocketFireworkRed;
				if (args.Parameters.Count > 1)
				{
					switch (args.Parameters[1].ToLower())
					{
						case "红":
						case "红色":
						case "red":
						case "r":
							type = ProjectileID.RocketFireworkRed;
							break;
						case "绿":
						case "绿色":
						case "green":
						case "g":
							type = ProjectileID.RocketFireworkGreen;
							break;
						case "蓝色":
						case "蓝":
						case "blue":
						case "b":
							type = ProjectileID.RocketFireworkBlue;
							break;
						case "黄":
						case "黄色":
						case "yellow":
						case "y":
							type = ProjectileID.RocketFireworkYellow;
							break;
						case "星星":
						case "红2":
						case "红色2":
						case "r2":
						case "star":
							type = ProjectileID.RocketFireworksBoxRed;
							break;
						case "螺旋":
						case "绿2":
						case "绿色2":
						case "g2":
						case "spiral":
							type = ProjectileID.RocketFireworksBoxGreen;
							break;
						case "环形":
						case "蓝2":
						case "蓝色2":
						case "b2":
						case "rings":
							type = ProjectileID.RocketFireworksBoxBlue;
							break;
						case "花花":
						case "黄2":
						case "黄色2":
						case "y2":
						case "flower":
							type = ProjectileID.RocketFireworksBoxYellow;
							break;
						default:
							type = ProjectileID.RocketFireworkRed;
							break;
					}
				}
				var target = players[0];
				int p = Projectile.NewProjectile(Projectile.GetNoneSource(), target.TPlayer.position.X, target.TPlayer.position.Y - 64f, 0f, -8f, type, 0, 0);
				Main.projectile[p].Kill();
				args.Player.SendSuccessMessage($"你在{(target == user ? "你自己" : target.Name)}身边点燃了一束烟花.");
				if (!args.Silent && target != user)
					target.SendSuccessMessage($"{user.Name} 在你身边点燃了一束烟花.");
			}
		}

		private static void Aliases(CommandArgs args)
		{
			if (args.Parameters.Count < 1)
			{
				args.Player.SendErrorMessage("格式错误! 正确格式: {0}aliases <指令或指令别名>", Specifier);
				return;
			}

			string givenCommandName = string.Join(" ", args.Parameters);
			if (string.IsNullOrWhiteSpace(givenCommandName))
			{
				args.Player.SendErrorMessage("请输入正确的指令或指令别名.");
				return;
			}

			string commandName;
			if (givenCommandName[0] == Specifier[0])
				commandName = givenCommandName.Substring(1);
			else
				commandName = givenCommandName;

			bool didMatch = false;
			foreach (Command matchingCommand in ChatCommands.Where(cmd => cmd.Names.IndexOf(commandName) != -1))
			{
				if (matchingCommand.Names.Count > 1)
					args.Player.SendInfoMessage(
						"{0}{1} 指令的别名为: {0}{2}", Specifier, matchingCommand.Name, string.Join(", {0}".SFormat(Specifier), matchingCommand.Names.Skip(1)));
				else
					args.Player.SendInfoMessage("{0}{1} 指令没有别名.", Specifier, matchingCommand.Name);

				didMatch = true;
			}

			if (!didMatch)
				args.Player.SendErrorMessage("没有找到符合 \"{0}\" 的指令.", givenCommandName);
		}

		private static void CreateDumps(CommandArgs args)
		{
			TShock.Utils.DumpPermissionMatrix("PermissionMatrix.txt");
			TShock.Utils.Dump(false);
			args.Player.SendSuccessMessage("你的服务器帮助文档已经在你的服务器目录下生成, 快去查看吧.");
			return;
		}

		private static void SyncLocalArea(CommandArgs args)
		{
			args.Player.SendTileSquareCentered((int)args.Player.TileX, (int)args.Player.TileY, 32);
			args.Player.SendWarningMessage("图格信息已同步!");
			return;
		}

		#endregion General Commands

		#region Game Commands

		private static void Clear(CommandArgs args)
		{
			var user = args.Player;
			var everyone = TSPlayer.All;
			int radius = 50;

			if (args.Parameters.Count != 1 && args.Parameters.Count != 2)
			{
				user.SendMessage("清理使用帮助", Color.White);
				user.SendMessage($"{"clear".Color(Utils.BoldHighlight)} <{"item(掉落物)".Color(Utils.GreenHighlight)}|{"npc(生物)".Color(Utils.RedHighlight)}|{"projectile(射弹)".Color(Utils.YellowHighlight)}> [{"范围".Color(Utils.PinkHighlight)}]", Color.White);
				user.SendMessage($"示例: {"clear".Color(Utils.BoldHighlight)} {"i".Color(Utils.RedHighlight)} {"10000".Color(Utils.GreenHighlight)}", Color.White);
				user.SendMessage($"示例: {"clear".Color(Utils.BoldHighlight)} {"item".Color(Utils.RedHighlight)} {"10000".Color(Utils.GreenHighlight)}", Color.White);
				user.SendMessage($"如果你没有指定半径, 服务器将会以默认半径 {radius} 进行清理.", Color.White);
				user.SendMessage($"如果你不想发送广播, 可以使用静默指令符 {SilentSpecifier.Color(Utils.GreenHighlight)} 替代 {Specifier.Color(Utils.RedHighlight)}.", Color.White);
				return;
			}

			if (args.Parameters.Count == 2)
			{
				if (!int.TryParse(args.Parameters[1], out radius) || radius <= 0)
				{
					user.SendErrorMessage($"\"{args.Parameters[1]}\" 不是一个有效的范围.");
					return;
				}
			}

			switch (args.Parameters[0].ToLower())
			{
				case "掉落物":
				case "物品":
				case "item":
				case "items":
				case "i":
					{
						int cleared = 0;
						for (int i = 0; i < Main.maxItems; i++)
						{
							float dX = Main.item[i].position.X - user.X;
							float dY = Main.item[i].position.Y - user.Y;

							if (Main.item[i].active && dX * dX + dY * dY <= radius * radius * 256f)
							{
								Main.item[i].active = false;
								everyone.SendData(PacketTypes.ItemDrop, "", i);
								cleared++;
							}
						}
						if (args.Silent)
							user.SendSuccessMessage($"服务器在半径 {radius} 内, 清理了 {cleared} 个掉落物物品.");
						else
							everyone.SendInfoMessage($"{user.Name} 使服务器在半径 {radius} 内, 清理了 {cleared} 个掉落物品.");
					}
					break;
				case "生物":
				case "npc":
				case "npcs":
				case "n":
					{
						int cleared = 0;
						for (int i = 0; i < Main.maxNPCs; i++)
						{
							float dX = Main.npc[i].position.X - user.X;
							float dY = Main.npc[i].position.Y - user.Y;

							if (Main.npc[i].active && dX * dX + dY * dY <= radius * radius * 256f)
							{
								Main.npc[i].active = false;
								Main.npc[i].type = 0;
								everyone.SendData(PacketTypes.NpcUpdate, "", i);
								cleared++;
							}
						}
						if (args.Silent)
							user.SendSuccessMessage($"服务器在半径 {radius} 内, 清理了 {cleared} 个NPC.");
						else
							everyone.SendInfoMessage($"{user.Name} 使服务器在半径 {radius} 内, 清理了 {cleared} 个NPC.");
					}
					break;
				case "射弹":
				case "proj":
				case "projectile":
				case "projectiles":
				case "p":
					{
						int cleared = 0;
						for (int i = 0; i < Main.maxProjectiles; i++)
						{
							float dX = Main.projectile[i].position.X - user.X;
							float dY = Main.projectile[i].position.Y - user.Y;

							if (Main.projectile[i].active && dX * dX + dY * dY <= radius * radius * 256f)
							{
								Main.projectile[i].active = false;
								Main.projectile[i].type = 0;
								everyone.SendData(PacketTypes.ProjectileNew, "", i);
								cleared++;
							}
						}
						if (args.Silent)
							user.SendSuccessMessage($"服务器在半径 {radius} 内, 清理了 {cleared} 个射弹.");
						else
							everyone.SendInfoMessage($"{user.Name} 使服务器在半径 {radius} 内, 清理了 {cleared} 个射弹.");
					}
					break;
				default:
					user.SendErrorMessage($"\"{args.Parameters[0]}\" 是无效的清理项目.");
					break;
			}
		}

		private static void Kill(CommandArgs args)
		{
			// To-Do: separate kill self and kill other player into two permissions
			var user = args.Player;
			if (args.Parameters.Count < 1)
			{
				user.SendMessage("Kill指令使用帮助", Color.White);
				user.SendMessage($"{"kill".Color(Utils.BoldHighlight)} <{"player".Color(Utils.RedHighlight)}>", Color.White);
				user.SendMessage($"示例: {"kill".Color(Utils.BoldHighlight)} {user.Name.Color(Utils.RedHighlight)}", Color.White);
				user.SendMessage($"如果你不想为玩家发送提示, 可以使用静默指令符 {SilentSpecifier.Color(Utils.GreenHighlight)} 替代 {Specifier.Color(Utils.RedHighlight)}.", Color.White);
				return;
			}

			string targetName = String.Join(" ", args.Parameters);
			var players = TSPlayer.FindByNameOrID(targetName);

			if (players.Count == 0)
				user.SendErrorMessage($"没有找到名为 \"{targetName}\" 的玩家.");
			else if (players.Count > 1)
				user.SendMultipleMatchError(players.Select(p => p.Name));
			else
			{
				var target = players[0];

				if (target.Dead)
				{
					user.SendErrorMessage($"{(target == user ? "你" : target.Name)}已经挂了!");
					return;
				}
				target.KillPlayer();
				user.SendSuccessMessage($"你杀死了{(target == user ? "你自己" : target.Name)}!");
				if (!args.Silent && target != user)
					target.SendErrorMessage($"{user.Name} 杀死了你!");
			}
		}

		private static void Respawn(CommandArgs args)
		{
			if (!args.Player.RealPlayer && args.Parameters.Count == 0)
			{
				args.Player.SendErrorMessage("你不能复活控制台君哦!");
				return;
			}
			TSPlayer playerToRespawn;
			if (args.Parameters.Count > 0)
			{
				if (!args.Player.HasPermission(Permissions.respawnother))
				{
					args.Player.SendErrorMessage("你没有权限复活其他玩家.");
					return;
				}
				string plStr = String.Join(" ", args.Parameters);
				var players = TSPlayer.FindByNameOrID(plStr);
				if (players.Count == 0)
				{
					args.Player.SendErrorMessage($"无法找到名为 \"{plStr}\" 的玩家");
					return;
				}
				if (players.Count > 1)
				{
					args.Player.SendMultipleMatchError(players.Select(p => p.Name));
					return;
				}
				playerToRespawn = players[0];
			}
			else
				playerToRespawn = args.Player;

			if (!playerToRespawn.Dead)
			{
				args.Player.SendErrorMessage($"{(playerToRespawn == args.Player ? "你" : playerToRespawn.Name)}还没挂呢.");
				return;
			}
			playerToRespawn.Spawn(PlayerSpawnContext.ReviveFromDeath);

			if (playerToRespawn != args.Player)
			{
				args.Player.SendSuccessMessage($"你复活了玩家 {playerToRespawn.Name}");
				if (!args.Silent)
					playerToRespawn.SendSuccessMessage($"玩家 {args.Player.Name} 复活了你.");
			}
			else
				playerToRespawn.SendSuccessMessage("你复活了你自己.");
		}

		private static void Butcher(CommandArgs args)
		{
			var user = args.Player;
			if (args.Parameters.Count > 1)
			{
				user.SendMessage("清除NPC使用帮助", Color.White);
				user.SendMessage($"{"butcher".Color(Utils.BoldHighlight)} [{"NPC名".Color(Utils.RedHighlight)}|{"NPCID".Color(Utils.RedHighlight)}]", Color.White);
				user.SendMessage($"示例: {"butcher".Color(Utils.BoldHighlight)} {"猪龙".Color(Utils.RedHighlight)}", Color.White);
				user.SendMessage("如果你不输入NPC名或ID,那么默认杀死所有NPC.", Color.White);
				user.SendMessage($"如果你想在指定范围内清除NPC并且不让其掉落物品, 你可以使用 {"clear".Color(Utils.BoldHighlight)} 指令替代.", Color.White);
				user.SendMessage($"如果你不想发送广播, 你可以使用静默指令符 {SilentSpecifier.Color(Utils.GreenHighlight)} 替代 {Specifier.Color(Utils.RedHighlight)}", Color.White);
				return;
			}

			int npcId = 0;

			if (args.Parameters.Count == 1)
			{
				var npcs = TShock.Utils.GetNPCByIdOrName(args.Parameters[0]);
				if (npcs.Count == 0)
				{
					user.SendErrorMessage($"\"{args.Parameters[0]}\" 不是有效的NPC.");
					return;
				}

				if (npcs.Count > 1)
				{
					user.SendMultipleMatchError(npcs.Select(n => $"{n.FullName}({n.type})"));
					return;
				}
				npcId = npcs[0].netID;
			}

			int kills = 0;
			for (int i = 0; i < Main.npc.Length; i++)
			{
				if (Main.npc[i].active && ((npcId == 0 && !Main.npc[i].townNPC && Main.npc[i].netID != NPCID.TargetDummy) || Main.npc[i].netID == npcId))
				{
					TSPlayer.Server.StrikeNPC(i, (int)(Main.npc[i].life + (Main.npc[i].defense * 0.6)), 0, 0);
					kills++;
				}
			}

			if (args.Silent)
				user.SendSuccessMessage($"你杀死了 {kills} 个NPC.");
			else
				TSPlayer.All.SendInfoMessage($"{user.Name} 杀死了 {kills} 个NPC.");
		}

		private static void Item(CommandArgs args)
		{
			if (args.Parameters.Count < 1)
			{
				args.Player.SendErrorMessage("格式错误! 正确格式: {0}item <物品名/物品ID> [物品数量] [前缀名/前缀ID]", Specifier);
				return;
			}

			int amountParamIndex = -1;
			int itemAmount = 0;
			for (int i = 1; i < args.Parameters.Count; i++)
			{
				if (int.TryParse(args.Parameters[i], out itemAmount))
				{
					amountParamIndex = i;
					break;
				}
			}

			string itemNameOrId;
			if (amountParamIndex == -1)
				itemNameOrId = string.Join(" ", args.Parameters);
			else
				itemNameOrId = string.Join(" ", args.Parameters.Take(amountParamIndex));

			Item item;
			List<Item> matchedItems = TShock.Utils.GetItemByIdOrName(itemNameOrId);
			if (matchedItems.Count == 0)
			{
				args.Player.SendErrorMessage("物品类型无效!");
				return;
			}
			else if (matchedItems.Count > 1)
			{
				args.Player.SendMultipleMatchError(matchedItems.Select(i => $"{i.Name}({i.netID})"));
				return;
			}
			else
			{
				item = matchedItems[0];
			}
			if (item.type < 1 && item.type >= Main.maxItemTypes)
			{
				args.Player.SendErrorMessage("物品类型 {0} 无效.", itemNameOrId);
				return;
			}

			int prefixId = 0;
			if (amountParamIndex != -1 && args.Parameters.Count > amountParamIndex + 1)
			{
				string prefixidOrName = args.Parameters[amountParamIndex + 1];
				var prefixIds = TShock.Utils.GetPrefixByIdOrName(prefixidOrName);

				if (item.accessory && prefixIds.Contains(PrefixID.Quick))
				{
					prefixIds.Remove(PrefixID.Quick);
					prefixIds.Remove(PrefixID.Quick2);
					prefixIds.Add(PrefixID.Quick2);
				}
				else if (!item.accessory && prefixIds.Contains(PrefixID.Quick))
					prefixIds.Remove(PrefixID.Quick2);

				if (prefixIds.Count > 1)
				{
					args.Player.SendMultipleMatchError(prefixIds.Select(p => p.ToString()));
					return;
				}
				else if (prefixIds.Count == 0)
				{
					args.Player.SendErrorMessage("没有找到符合 \"{0}\" 的前缀.", prefixidOrName);
					return;
				}
				else
				{
					prefixId = prefixIds[0];
				}
			}

			if (args.Player.InventorySlotAvailable || (item.type > 70 && item.type < 75) || item.ammo > 0 || item.type == 58 || item.type == 184)
			{
				if (itemAmount == 0 || itemAmount > item.maxStack)
					itemAmount = item.maxStack;

				if (args.Player.GiveItemCheck(item.type, EnglishLanguage.GetItemNameById(item.type), itemAmount, prefixId))
				{
					item.prefix = (byte)prefixId;
					args.Player.SendSuccessMessage("已给予你 {0} {1}.", itemAmount, item.AffixName());
				}
				else
				{
					args.Player.SendErrorMessage("你不能生成被禁用的物品.");
				}
			}
			else
			{
				args.Player.SendErrorMessage("你的背包已满.");
			}
		}

		private static void RenameNPC(CommandArgs args)
		{
			if (args.Parameters.Count != 2)
			{
				args.Player.SendErrorMessage("格式错误! 正确格式: {0}renameNPC <向导, 护士...> <新城镇NPC名>", Specifier);
				return;
			}
			int npcId = 0;
			if (args.Parameters.Count == 2)
			{
				List<NPC> npcs = TShock.Utils.GetNPCByIdOrName(args.Parameters[0]);
				if (npcs.Count == 0)
				{
					args.Player.SendErrorMessage("城镇NPC名无效!");
					return;
				}
				else if (npcs.Count > 1)
				{
					args.Player.SendMultipleMatchError(npcs.Select(n => $"{n.FullName}({n.type})"));
					return;
				}
				else if (args.Parameters[1].Length > 200)
				{
					args.Player.SendErrorMessage("新城镇NPC名太长了!");
					return;
				}
				else
				{
					npcId = npcs[0].netID;
				}
			}
			int done = 0;
			for (int i = 0; i < Main.npc.Length; i++)
			{
				if (Main.npc[i].active && ((npcId == 0 && !Main.npc[i].townNPC) || (Main.npc[i].netID == npcId && Main.npc[i].townNPC)))
				{
					Main.npc[i].GivenName = args.Parameters[1];
					NetMessage.SendData(56, -1, -1, NetworkText.FromLiteral(args.Parameters[1]), i, 0f, 0f, 0f, 0);
					done++;
				}
			}
			if (done > 0)
			{
				TSPlayer.All.SendInfoMessage("{0} 重命名了城镇NPC {1}.", args.Player.Name, args.Parameters[0]);
			}
			else
			{
				args.Player.SendErrorMessage("无法重命名城镇NPC {0}!", args.Parameters[0]);
			}
		}

		private static void Give(CommandArgs args)
		{
			if (args.Parameters.Count < 2)
			{
				args.Player.SendErrorMessage(
					"格式错误! 正确格式: {0}give <物品名/物品ID> <玩家名> [物品数量] [前缀名/前缀ID]", Specifier);
				return;
			}
			if (args.Parameters[0].Length == 0)
			{
				args.Player.SendErrorMessage("你没有输入参数 物品名/物品ID.");
				return;
			}
			if (args.Parameters[1].Length == 0)
			{
				args.Player.SendErrorMessage("你没有输入参数 玩家名/玩家索引.");
				return;
			}
			int itemAmount = 0;
			int prefix = 0;
			var items = TShock.Utils.GetItemByIdOrName(args.Parameters[0]);
			args.Parameters.RemoveAt(0);
			string plStr = args.Parameters[0];
			args.Parameters.RemoveAt(0);
			if (args.Parameters.Count == 1)
				int.TryParse(args.Parameters[0], out itemAmount);
			if (items.Count == 0)
			{
				args.Player.SendErrorMessage("物品类型无效!");
			}
			else if (items.Count > 1)
			{
				args.Player.SendMultipleMatchError(items.Select(i => $"{i.Name}({i.netID})"));
			}
			else
			{
				var item = items[0];

				if (args.Parameters.Count == 2)
				{
					int.TryParse(args.Parameters[0], out itemAmount);
					var prefixIds = TShock.Utils.GetPrefixByIdOrName(args.Parameters[1]);
					if (item.accessory && prefixIds.Contains(PrefixID.Quick))
					{
						prefixIds.Remove(PrefixID.Quick);
						prefixIds.Remove(PrefixID.Quick2);
						prefixIds.Add(PrefixID.Quick2);
					}
					else if (!item.accessory && prefixIds.Contains(PrefixID.Quick))
						prefixIds.Remove(PrefixID.Quick2);
					if (prefixIds.Count == 1)
						prefix = prefixIds[0];
				}

				if (item.type >= 1 && item.type < Main.maxItemTypes)
				{
					var players = TSPlayer.FindByNameOrID(plStr);
					if (players.Count == 0)
					{
						args.Player.SendErrorMessage("没有找到该玩家!");
					}
					else if (players.Count > 1)
					{
						args.Player.SendMultipleMatchError(players.Select(p => p.Name));
					}
					else
					{
						var plr = players[0];
						if (plr.InventorySlotAvailable || (item.type > 70 && item.type < 75) || item.ammo > 0 || item.type == 58 || item.type == 184)
						{
							if (itemAmount == 0 || itemAmount > item.maxStack)
								itemAmount = item.maxStack;
							if (plr.GiveItemCheck(item.type, EnglishLanguage.GetItemNameById(item.type), itemAmount, prefix))
							{
								args.Player.SendSuccessMessage(string.Format("已给予玩家 {0} {1} {2}.", plr.Name, itemAmount, item.Name));
								plr.SendSuccessMessage(string.Format("{0} 给予你 {1} {2}.", args.Player.Name, itemAmount, item.Name));
							}
							else
							{
								args.Player.SendErrorMessage("你无法生成被禁用的物品.");
							}

						}
						else
						{
							args.Player.SendErrorMessage("玩家背包已满!");
						}
					}
				}
				else
				{
					args.Player.SendErrorMessage("物品类型无效!");
				}
			}
		}

		private static void Heal(CommandArgs args)
		{
			// heal <player> [amount]
			// To-Do: break up heal self and heal other into two separate permissions
			var user = args.Player;
			if (args.Parameters.Count < 1 || args.Parameters.Count > 2)
			{
				user.SendMessage("治疗指令使用帮助", Color.White);
				user.SendMessage($"{"heal".Color(Utils.BoldHighlight)} <{"玩家名".Color(Utils.RedHighlight)}> [{"治疗量".Color(Utils.GreenHighlight)}]", Color.White);
				user.SendMessage($"示例: {"heal".Color(Utils.BoldHighlight)} {user.Name.Color(Utils.RedHighlight)} {"100".Color(Utils.GreenHighlight)}", Color.White);
				user.SendMessage($"如果你没有输入治疗量, 那将会默认回满该玩家的HP.", Color.White);
				user.SendMessage($"如果你不想为该玩家发送提示, 可以使用静态指令符 {SilentSpecifier.Color(Utils.GreenHighlight)} 替代 {Specifier.Color(Utils.RedHighlight)}", Color.White);
				return;
			}
			if (args.Parameters[0].Length == 0)
			{
				user.SendErrorMessage($"你没有输入玩家名.");
				return;
			}

			string targetName = args.Parameters[0];
			var players = TSPlayer.FindByNameOrID(targetName);
			if (players.Count == 0)
				user.SendErrorMessage($"没有找到名为 \"{targetName}\" 的玩家");
			else if (players.Count > 1)
				user.SendMultipleMatchError(players.Select(p => p.Name));
			else
			{
				var target = players[0];
				int amount = target.TPlayer.statLifeMax2;

				if (target.Dead)
				{
					user.SendErrorMessage("你不可以治疗挂掉的玩家哦!");
					return;
				}

				if (args.Parameters.Count == 2)
				{
					int.TryParse(args.Parameters[1], out amount);
				}
				target.Heal(amount);

				if (args.Silent)
					user.SendSuccessMessage($"你恢复了{(target == user ? "你自己" : target.Name)}{amount}点HP.");
				else
					TSPlayer.All.SendInfoMessage($"{user.Name}恢复{(target == user ? (target.TPlayer.Male ? "他自己" : "她自己") : target.Name)}{amount}点HP.");
			}
		}

		private static void Buff(CommandArgs args)
		{
			// buff <"buff name|ID"> [duration]
			var user = args.Player;
			if (args.Parameters.Count < 1 || args.Parameters.Count > 2)
			{
				user.SendMessage("Buff指令使用帮助", Color.White);
				user.SendMessage($"{"buff".Color(Utils.BoldHighlight)} <\"{"Buff名".Color(Utils.RedHighlight)}|{"BuffID".Color(Utils.RedHighlight)}\"> [{"持续时间(秒)".Color(Utils.GreenHighlight)}]", Color.White);
				user.SendMessage($"示例: {"buff".Color(Utils.BoldHighlight)} \"{"黑曜石皮".Color(Utils.RedHighlight)}\" {"-1".Color(Utils.GreenHighlight)}", Color.White);
				user.SendMessage($"如果你不输入持续时间, Buff持续时间将会默认为 {"60".Color(Utils.GreenHighlight)} 秒.", Color.White);
				user.SendMessage($"如果你输入 {"-1".Color(Utils.GreenHighlight)}作为持续时间, Buff持续时间将会被设为最大值.", Color.White);
				return;
			}

			int id = 0;
			int time = 60;
			var timeLimit = (int.MaxValue / 60) - 1;

			if (!int.TryParse(args.Parameters[0], out id))
			{
				var found = TShock.Utils.GetBuffByName(args.Parameters[0]);

				if (found.Count == 0)
				{
					user.SendErrorMessage($"没有找到任何符合  \"{args.Parameters[0]}\" 的Buff");
					return;
				}

				if (found.Count > 1)
				{
					user.SendMultipleMatchError(found.Select(f => Lang.GetBuffName(f)));
					return;
				}
				id = found[0];
			}

			if (args.Parameters.Count == 2)
				int.TryParse(args.Parameters[1], out time);

			if (id > 0 && id < Main.maxBuffTypes)
			{
				// Max possible buff duration as of Terraria 1.4.2.3 is 35791393 seconds (415 days).
				if (time < 0 || time > timeLimit)
					time = timeLimit;
				user.SetBuff(id, time * 60);
				user.SendSuccessMessage($"你给予自己 {TShock.Utils.GetBuffName(id)} ({TShock.Utils.GetBuffDescription(id)}) 持续 {time} 秒.");
			}
			else
				user.SendErrorMessage($"\"{id}\" 为无效BuffID!");
		}

		private static void GBuff(CommandArgs args)
		{
			var user = args.Player;
			if (args.Parameters.Count < 2 || args.Parameters.Count > 3)
			{
				user.SendMessage("gBuff指令使用帮助", Color.White);
				user.SendMessage($"{"gbuff".Color(Utils.BoldHighlight)} <{"玩家名".Color(Utils.RedHighlight)}> <{"Buff名".Color(Utils.PinkHighlight)}|{"BuffID".Color(Utils.PinkHighlight)}> [{"持续时间(秒)".Color(Utils.GreenHighlight)}]", Color.White);
				user.SendMessage($"示例: {"gbuff".Color(Utils.BoldHighlight)} {user.Name.Color(Utils.RedHighlight)} {"再生".Color(Utils.PinkHighlight)} {"-1".Color(Utils.GreenHighlight)}", Color.White);
				user.SendMessage($"如果你不想给玩家发送提示信息, 可以使用静默指令符 {SilentSpecifier.Color(Utils.RedHighlight)} 替代 {Specifier.Color(Utils.GreenHighlight)}", Color.White);
				return;
			}
			int id = 0;
			int time = 60;
			var timeLimit = (int.MaxValue / 60) - 1;
			var foundplr = TSPlayer.FindByNameOrID(args.Parameters[0]);
			if (foundplr.Count == 0)
			{
				user.SendErrorMessage($"没有找到名为 \"{args.Parameters[0]}\" 的玩家");
				return;
			}
			else if (foundplr.Count > 1)
			{
				user.SendMultipleMatchError(foundplr.Select(p => p.Name));
				return;
			}
			else
			{
				if (!int.TryParse(args.Parameters[1], out id))
				{
					var found = TShock.Utils.GetBuffByName(args.Parameters[1]);
					if (found.Count == 0)
					{
						user.SendErrorMessage($"没有找到名为 \"{args.Parameters[1]}\" 的Buff");
						return;
					}
					else if (found.Count > 1)
					{
						user.SendMultipleMatchError(found.Select(b => Lang.GetBuffName(b)));
						return;
					}
					id = found[0];
				}
				if (args.Parameters.Count == 3)
					int.TryParse(args.Parameters[2], out time);
				if (id > 0 && id < Main.maxBuffTypes)
				{
					var target = foundplr[0];
					if (time < 0 || time > timeLimit)
						time = timeLimit;
					target.SetBuff(id, time * 60);
					user.SendSuccessMessage($"你给予了{(target == user ? "你自己" : target.Name)} {TShock.Utils.GetBuffName(id)} ({TShock.Utils.GetBuffDescription(id)}) 持续 {time} 秒!");
					if (!args.Silent && target != user)
						target.SendSuccessMessage($"{user.Name} 给予了你 {TShock.Utils.GetBuffName(id)} ({TShock.Utils.GetBuffDescription(id)}) 持续 {time} 秒!");
				}
				else
					user.SendErrorMessage("BuffID无效!");
			}
		}

		public static void Grow(CommandArgs args)
		{
			bool canGrowEvil = args.Player.HasPermission(Permissions.growevil);
			string subcmd = args.Parameters.Count == 0 ? "help" : args.Parameters[0].ToLower();

			var name = "寂寞(生成失败了哦)";
			var x = args.Player.TileX;
			var y = args.Player.TileY + 3;

			if (!TShock.Regions.CanBuild(x, y, args.Player))
			{
				args.Player.SendErrorMessage("你没有权限修改此处的图格!");
				return;
			}

			switch (subcmd)
			{
				case "help":
					{
						if (!PaginationTools.TryParsePageNumber(args.Parameters, 1, args.Player, out int pageNumber))
							return;

						var lines = new List<string>
					{
						"- 默认的树 :",
						"     'basic(普通树)', 'sakura(粉樱花树)', 'willow(黄柳树)', 'boreal(针叶树)', 'mahogany(丛林树)', 'ebonwood(腐化树)', 'shadewood(猩红树)', 'pearlwood(神圣树)'.",
						"- 棕榈树 :",
						"     'palm(普通棕榈树)', 'corruptpalm(腐化棕榈树)', 'crimsonpalm(猩红棕榈树)', 'hallowpalm(神圣棕榈树)'.",
						"- 宝石树 :",
						"     'topaz(黄玉树)', 'amethyst(紫晶树)', 'sapphire(蓝玉树)', 'emerald(翡翠树)', 'ruby(红玉树)', 'diamond(钻石树)', 'amber(琥珀树)'.",
						"- 杂项 :",
						"     'cactus(仙人掌)', 'herb(草药)', 'mushroom(巨型发光蘑菇)'."
					};

						PaginationTools.SendPage(args.Player, pageNumber, lines,
								new PaginationTools.Settings
								{
									HeaderFormat = "可用的树种 & 杂项 ({0}/{1}):",
									FooterFormat = "输入 {0}grow help {{0}} 翻页.".SFormat(Commands.Specifier)
								}
							);
					}
					break;

					bool rejectCannotGrowEvil()
					{
						if (!canGrowEvil)
						{
							args.Player.SendErrorMessage("你没有权限种这种树哦");
							return false;
						}

						return true;
					}

					bool prepareAreaForGrow(ushort groundType = TileID.Grass, bool evil = false)
					{
						if (evil && !rejectCannotGrowEvil())
							return false;

						for (var i = x - 2; i < x + 3; i++)
						{
							Main.tile[i, y].active(true);
							Main.tile[i, y].type = groundType;
							Main.tile[i, y].wall = WallID.None;
						}
						Main.tile[x, y - 1].wall = WallID.None;

						return true;
					}

					bool growTree(ushort groundType, string fancyName, bool evil = false)
					{
						if (!prepareAreaForGrow(groundType, evil))
							return false;
						WorldGen.GrowTree(x, y);
						name = fancyName;

						return true;
					}

					bool growTreeByType(ushort groundType, string fancyName, ushort typeToPrepare = 2, bool evil = false)
					{
						if (!prepareAreaForGrow(typeToPrepare, evil))
							return false;
						WorldGen.TryGrowingTreeByType(groundType, x, y);
						name = fancyName;

						return true;
					}

					bool growPalmTree(ushort sandType, ushort supportingType, string properName, bool evil = false)
					{
						if (evil && !rejectCannotGrowEvil())
							return false;

						for (int i = x - 2; i < x + 3; i++)
						{
							Main.tile[i, y].active(true);
							Main.tile[i, y].type = sandType;
							Main.tile[i, y].wall = WallID.None;
						}
						for (int i = x - 2; i < x + 3; i++)
						{
							Main.tile[i, y + 1].active(true);
							Main.tile[i, y + 1].type = supportingType;
							Main.tile[i, y + 1].wall = WallID.None;
						}

						Main.tile[x, y - 1].wall = WallID.None;
						WorldGen.GrowPalmTree(x, y);

						name = properName;

						return true;
					}

				case "普通树":
				case "basic":
					growTree(TileID.Grass, "普通树");
					break;
				case "针叶树":
				case "boreal":
					growTree(TileID.SnowBlock, "针叶树");
					break;
				case "丛林树":
				case "mahogany":
					growTree(TileID.JungleGrass, "丛林树");
					break;
				case "粉樱花树":
				case "sakura":
					growTreeByType(TileID.VanityTreeSakura, "粉樱花树");
					break;
				case "黄柳树":
				case "willow":
					growTreeByType(TileID.VanityTreeYellowWillow, "黄柳树");
					break;
				case "猩红树":
				case "shadewood":
					if (!growTree(TileID.CrimsonGrass, "猩红树", true))
						return;
					break;
				case "腐化树":
				case "ebonwood":
					if (!growTree(TileID.CorruptGrass, "腐化树", true))
						return;
					break;
				case "神圣树":
				case "pearlwood":
					if (!growTree(TileID.HallowedGrass, "神圣树", true))
						return;
					break;
				case "普通棕榈树":
				case "palm":
					growPalmTree(TileID.Sand, TileID.HardenedSand, "普通棕榈树");
					break;
				case "神圣棕榈树":
				case "hallowpalm":
					if (!growPalmTree(TileID.Pearlsand, TileID.HallowHardenedSand, "神圣棕榈树", true))
						return;
					break;
				case "猩红棕榈树":
				case "crimsonpalm":
					if (!growPalmTree(TileID.Crimsand, TileID.CrimsonHardenedSand, "猩红棕榈树", true))
						return;
					break;
				case "腐化棕榈树":
				case "corruptpalm":
					if (!growPalmTree(TileID.Ebonsand, TileID.CorruptHardenedSand, "腐化棕榈树", true))
						return;
					break;
				case "黄玉树":
				case "topaz":
					growTreeByType(TileID.TreeTopaz, "黄玉树", 1);
					break;

				case "紫晶树":
				case "amethyst":
					growTreeByType(TileID.TreeAmethyst, "紫晶树", 1);
					break;
				case "蓝玉树":
				case "sapphire":
					growTreeByType(TileID.TreeSapphire, "蓝玉树", 1);
					break;
				case "翡翠树":
				case "emerald":
					growTreeByType(TileID.TreeEmerald, "翡翠树", 1);
					break;
				case "红玉树":
				case "ruby":
					growTreeByType(TileID.TreeRuby, "红玉树", 1);
					break;
				case "钻石树":
				case "diamond":
					growTreeByType(TileID.TreeDiamond, "钻石树", 1);
					break;
				case "琥珀树":
				case "amber":
					growTreeByType(TileID.TreeAmber, "琥珀树", 1);
					break;
				case "仙人掌":
				case "cactus":
					Main.tile[x, y].type = TileID.Sand;
					WorldGen.GrowCactus(x, y);
					name = "仙人掌";
					break;
				case "草药":
				case "herb":
					Main.tile[x, y].active(true);
					Main.tile[x, y].frameX = 36;
					Main.tile[x, y].type = TileID.MatureHerbs;
					WorldGen.GrowAlch(x, y);
					name = "草药";
					break;
				case "巨型发光蘑菇":
				case "mushroom":
					prepareAreaForGrow(TileID.MushroomGrass);
					WorldGen.GrowShroom(x, y);
					name = "巨型发光蘑菇";
					break;

				default:
					args.Player.SendErrorMessage("没有找到这种树哦!");
					return;
			}
			if (args.Parameters.Count == 1 && args.Parameters[1] != "help")
			{
				args.Player.SendTileSquareCentered(x - 2, y - 20, 25);
				args.Player.SendSuccessMessage("服务器尝试种植了一颗 " + name + ".");
			}
		}

		private static void ToggleGodMode(CommandArgs args)
		{
			TSPlayer playerToGod;
			if (args.Parameters.Count > 0)
			{
				if (!args.Player.HasPermission(Permissions.godmodeother))
				{
					args.Player.SendErrorMessage("你没有权限将其他玩家设为上帝模式.");
					return;
				}
				string plStr = String.Join(" ", args.Parameters);
				var players = TSPlayer.FindByNameOrID(plStr);
				if (players.Count == 0)
				{
					args.Player.SendErrorMessage("玩家名无效!");
					return;
				}
				else if (players.Count > 1)
				{
					args.Player.SendMultipleMatchError(players.Select(p => p.Name));
					return;
				}
				else
				{
					playerToGod = players[0];
				}
			}
			else if (!args.Player.RealPlayer)
			{
				args.Player.SendErrorMessage("你不能将一个非玩家设置为上帝模式!");
				return;
			}
			else
			{
				playerToGod = args.Player;
			}

			playerToGod.GodMode = !playerToGod.GodMode;

			var godPower = CreativePowerManager.Instance.GetPower<CreativePowers.GodmodePower>();
			godPower.SetEnabledState(playerToGod.Index, playerToGod.GodMode);

			if (playerToGod != args.Player)
			{
				args.Player.SendSuccessMessage(string.Format("{0} 现在{1}上帝模式.", playerToGod.Name, playerToGod.GodMode ? "处于" : "不再处于"));
			}

			if (!args.Silent || (playerToGod == args.Player))
			{
				playerToGod.SendSuccessMessage(string.Format("你现在{0}上帝模式.", playerToGod.GodMode ? "处于" : "不再处于"));
			}
		}

		#endregion Game Commands
	}
}
