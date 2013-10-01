/*
TShock, a server mod for Terraria
Copyright (C) 2011-2013 Nyx Studios (fka. The TShock Team)

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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace TShockAPI
{
	public static class Permissions
	{
		// tshock.account nodes

		[Description("User can register account in game")]
		public static readonly string canregister = "tshock.account.register";

		[Description("User can login in game")]
		public static readonly string canlogin = "tshock.account.login";

		[Description("User can change password in game")]
		public static readonly string canchangepassword = "tshock.account.changepassword";

		// tshock.admin nodes

		[Description("Prevents you from being kicked.")]
		public static readonly string immunetokick = "tshock.admin.nokick";

		[Description("Prevents you from being banned.")]
		public static readonly string immunetoban = "tshock.admin.noban";

		[Description("Specific log messages are sent to users with this permission.")]
		public static readonly string logs = "tshock.admin.viewlogs";

		[Description("User can kick others.")]
		public static readonly string kick = "tshock.admin.kick";

		[Description("User can ban others.")]
		public static readonly string ban = "tshock.admin.ban";

		[Description("User can manage warps.")]
		public static readonly string managewarp = "tshock.admin.warp";

		[Description("User can manage item bans.")]
		public static readonly string manageitem = "tshock.admin.itemban";

		[Description("User can manage groups.")]
		public static readonly string managegroup = "tshock.admin.group";

		[Description("User can manage regions.")]
		public static readonly string manageregion = "tshock.admin.region";

		[Description("User can mute and unmute users")]
		public static readonly string mute = "tshock.admin.mute";

		[Description("User can see the id of players with /who")]
		public static readonly string seeids = "tshock.admin.seeplayerids";

		[Description("User can save all the players SSI state.")]
		public static readonly string savessi = "tshock.admin.savessi";

		[Description("User can elevate other users' groups temporarily.")]
		public static readonly string settempgroup = "tshock.admin.tempgroup";

		[Description("User can broadcast messages.")]
		public static readonly string broadcast = "tshock.admin.broadcast";

		[Description("User can get other users' info.")]
		public static readonly string userinfo = "tshock.admin.userinfo";

		// tshock.buff nodes

		[Description("User can buff self.")]
		public static readonly string buff = "tshock.buff.self";

		[Description("User can buff other players.")]
		public static readonly string buffplayer = "tshock.buff.others";

		// tshock.cfg nodes

		[Description("User is notified when an update is available, user can turn off / restart the server.")]
		public static readonly string maintenance = "tshock.cfg.maintenance";

		[Description("User can modify the whitelist.")]
		public static readonly string whitelist = "tshock.cfg.whitelist";

		[Description("User can edit the server password.")]
		public static readonly string cfgpassword = "tshock.cfg.password";

		[Description("User can reload the configurations file.")]
		public static readonly string cfgreload = "tshock.cfg.reload";

		[Description("User can edit the max spawns.")]
		public static readonly string cfgmaxspawns = "tshock.cfg.maxspawns";

		[Description("User can edit the spawnrate.")]
		public static readonly string cfgspawnrate = "tshock.cfg.spawnrate";

		[Description("User can download updates to plugins that are currently running.")]
		public static readonly string updateplugins = "tshock.cfg.updateplugins";

		// tshock.ignore nodes

		[Description("Prevents you from being reverted by kill tile abuse detection.")]
		public static readonly string ignorekilltiledetection = "tshock.ignore.removetile";

		[Description("Prevents you from being reverted by place tile abuse detection.")]
		public static readonly string ignoreplacetiledetection = "tshock.ignore.placetile";

		[Description("Prevents you from being disabled by liquid set abuse detection.")]
		public static readonly string ignoreliquidsetdetection = "tshock.ignore.liquid";

		[Description("Prevents you from being disabled by projectile abuse detection.")]
		public static readonly string ignoreprojectiledetection = "tshock.ignore.projectile";

		[Description("Prevents you from being reverted by no clip detection.")]
		public static readonly string ignorenoclipdetection = "tshock.ignore.noclip";

		[Description("Prevents you from being disabled by stack hack detection.")]
		public static readonly string ignorestackhackdetection = "tshock.ignore.itemstack";

		[Description("Prevents you from being kicked by hacked health detection.")]
		public static readonly string ignorestathackdetection = "tshock.ignore.stats";

		[Description("Prevents your actions from being ignored if damage is too high.")]
		public static readonly string ignoredamagecap = "tshock.ignore.damage";

		[Description("Bypass server side inventory checks")]
		public static readonly string bypassinventorychecks = "tshock.ignore.ssi";

		[Description("Allow unrestricted SendTileSquare usage, for client side world editing.")]
		public static readonly string allowclientsideworldedit = "tshock.ignore.sendtilesquare";

		// tshock.item nodes

		[Description("User can spawn items.")]
		public static readonly string item = "tshock.item.spawn";

		[Description("User can clear items.")]
		public static readonly string clearitems = "tshock.item.clear";

		[Description("Allows you to use banned items.")]
		public static readonly string usebanneditem = "tshock.item.usebanned";

		// tshock.npc nodes

		[Description("User can spawn bosses.")]
		public static readonly string spawnboss = "tshock.npc.spawnboss";

		[Description("User can spawn npcs.")]
		public static readonly string spawnmob = "tshock.npc.spawnmob";

		[Description("User can kill all enemy npcs.")]
		public static readonly string butcher = "tshock.npc.butcher";

		[Description("User can summon bosses using items")]
		public static readonly string summonboss = "tshock.npc.summonboss";

		[Description("User can start invasions (Goblin/Snow Legion) using items")]
		public static readonly string startinvasion = "tshock.npc.startinvasion";

		// tshock.superadmin nodes

		[Description("Meant for super admins only.")]
		public static readonly string authverify = "tshock.superadmin.authverify";

		[Description("Meant for super admins only.")]
		public static readonly string user = "tshock.superadmin.user";

		// tshock.tp nodes

		[Description("User can teleport to others.")]
		public static readonly string tp = "tshock.tp.self";

		[Description("User can teleport people to them.")]
		public static readonly string tphere = "tshock.tp.others";

		[Description("Users can stop people from teleporting to them")]
		public static readonly string tpallow = "tshock.tp.block";

		[Description("Users can tp to anyone")]
		public static readonly string tpall = "tshock.tp.toall";

		[Description("Users can tp to people without showing a notice")]
		public static readonly string tphide = "tshock.tp.silent";

		[Description("User can use /home.")]
		public static readonly string home = "tshock.tp.home";

		[Description("User can use /spawn.")]
		public static readonly string spawn = "tshock.tp.spawn";

		// tshock.world nodes

		[Description("Allows you to edit the spawn.")]
		public static readonly string editspawn = "tshock.world.editspawn";

		[Description("User can set the time.")]
		public static readonly string time = "tshock.world.settime";

		[Description("User can grow plants.")]
		public static readonly string grow = "tshock.world.grow";

		[Description("User can change hardmode state.")]
		public static readonly string hardmode = "tshock.world.hardmode";

		[Description("User can change the homes of NPCs.")]
		public static readonly string movenpc = "tshock.world.movenpc";

		[Description("User can convert hallow into corruption and vice-versa")]
		public static readonly string converthardmode = "tshock.world.converthardmode";

		[Description("User can force the server to Christmas mode.")]
		public static readonly string xmas = "tshock.world.setxmas";

		[Description("User can save the world.")]
		public static readonly string worldsave = "tshock.world.save";

		[Description("User can settle liquids.")]
		public static readonly string worldsettle = "tshock.world.settleliquids";

		[Description("User can get the world info.")]
		public static readonly string worldinfo = "tshock.world.info";

		[Description("User can set the world spawn.")]
		public static readonly string worldspawn = "tshock.world.setspawn";

		[Description("User can cause some events.")]
		public static readonly string causeevents = "tshock.world.causeevents";

		[Description("User can modify the world.")]
		public static readonly string canbuild = "tshock.world.modify";
		
		[Description("User can paint tiles.")]
		public static readonly string canpaint = "tshock.world.paint";

		// Non-grouped

		[Description("User can kill others.")]
		public static readonly string kill = "tshock.kill";

		[Description("Allows you to bypass the max slots for up to 5 slots above your max.")]
		public static readonly string reservedslot = "tshock.reservedslot";

		[Description("User can use warps.")]
		public static readonly string warp = "tshock.warp";

		[Description("User can slap others.")]
		public static readonly string slap = "tshock.slap";

		[Description("User can whisper to others.")]
		public static readonly string whisper = "tshock.whisper";

		[Description("User can annoy others.")]
		public static readonly string annoy = "tshock.annoy";

		[Description("User can heal players.")]
		public static readonly string heal = "tshock.heal";

		[Description("User can use party chat in game")]
		public static readonly string canpartychat = "tshock.partychat";

		[Description("User can talk in third person")]
		public static readonly string cantalkinthird = "tshock.thirdperson";

		[Description("User can get the server info.")]
		public static readonly string serverinfo = "tshock.info";

		[Description("Player recovers health as damage is taken.  Can be one shotted.")]
		public static readonly string godmode = "tshock.godmode";

		[Description("Player can chat")] 
		public static readonly string canchat = "tshock.canchat";

        /// <summary>
        /// Lists all commands associated with a given permission
        /// </summary>
        /// <param name="perm">string permission - the permission to get information on</param>
        /// <returns>List of commands</returns>
		private static List<Command> GetCommands(string perm)
		{
			if (Commands.ChatCommands.Count < 1)
				Commands.InitCommands();
			return Commands.ChatCommands.Where(c => c.Permissions.Contains(perm)).ToList();
		}

        /// <summary>
        /// Dumps the descriptions of each permission to a file in Markdown format.
        /// </summary>
		public static void DumpDescriptions()
		{
			var sb = new StringBuilder();
			foreach (var field in typeof(Permissions).GetFields().OrderBy(f => f.Name))
			{
				var name = (string)field.GetValue(null);

				var descattr =
					field.GetCustomAttributes(false).FirstOrDefault(o => o is DescriptionAttribute) as DescriptionAttribute;
				var desc = descattr != null && !string.IsNullOrWhiteSpace(descattr.Description) ? descattr.Description : "None";

				var commands = GetCommands(name);
				foreach (var c in commands)
				{
					for (var i = 0; i < c.Names.Count; i++)
					{
						c.Names[i] = "/" + c.Names[i];
					}
				}
				var strs =
					commands.Select(
						c =>
						c.Name + (c.Names.Count > 1 ? "({0})".SFormat(string.Join(" ", c.Names.ToArray(), 1, c.Names.Count - 1)) : ""));

				sb.AppendLine("## <a id=\"{0}\">{0}</a>  ".SFormat(name));
				sb.AppendLine("**Description:** {0}  ".SFormat(desc));
				sb.AppendLine("**Commands:** {0}  ".SFormat(strs.Count() > 0 ? string.Join(" ", strs) : "None"));
				sb.AppendLine();
			}

			File.WriteAllText("PermissionsDescriptions.txt", sb.ToString());
		}
	}

	[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
	public sealed class TodoAttribute : Attribute
	{
		public string Info { get; private set; }

		public TodoAttribute(string info)
		{
			Info = info;
		}

		public TodoAttribute()
		{
		}
	}
}