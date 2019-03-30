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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

// Since the permission nodes have annotations that say what they are, we don't need XML comments.
#pragma warning disable 1591

namespace TShockAPI
{
	/// <summary>Contains the permission nodes used in TShock.</summary>
	public static class Permissions
	{
		// tshock.account nodes

		[Description("User can register account in game.")]
		public static readonly string canregister = "tshock.account.register";

		[Description("User can login in game.")]
		public static readonly string canlogin = "tshock.account.login";

		[Description("User can logout in game.")]
		public static readonly string canlogout = "tshock.account.logout";

		[Description("User can change password in game.")]
		public static readonly string canchangepassword = "tshock.account.changepassword";

		// tshock.admin nodes

		[Description("User can set build protection status.")]
		public static readonly string antibuild = "tshock.admin.antibuild";

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

		[Description("User can manage projectile bans.")]
		public static readonly string manageprojectile = "tshock.admin.projectileban";

		[Description("User can manage tile bans.")]
		public static readonly string managetile = "tshock.admin.tileban";

		[Description("User can manage groups.")]
		public static readonly string managegroup = "tshock.admin.group";

		[Description("User can manage regions.")]
		public static readonly string manageregion = "tshock.admin.region";

		[Description("User can mute and unmute users.")]
		public static readonly string mute = "tshock.admin.mute";

		[Description("User can see the id of players with /who.")]
		public static readonly string seeids = "tshock.admin.seeplayerids";

		[Description("User can save all the players SSI (server side character) state.")]
		public static readonly string savessc = "tshock.admin.savessi";

		[Description("User can upload their joined character data as SSC data.")]
		public static readonly string uploaddata = "tshock.ssc.upload";

		[Description("User can upload other players join data to the SSC database.")]
		public static readonly string uploadothersdata = "tshock.ssc.upload.others";

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

		[Description("User can create reference files of Terraria IDs and the permission matrix in the server folder.")]
		public static readonly string createdumps = "tshock.cfg.createdumps";

		// tshock.ignore nodes

		[Description("Prevents you from being reverted by kill tile abuse detection.")]
		public static readonly string ignorekilltiledetection = "tshock.ignore.removetile";

		[Description("Prevents you from being reverted by place tile abuse detection.")]
		public static readonly string ignoreplacetiledetection = "tshock.ignore.placetile";

		[Description("Prevents you from being disabled by liquid set abuse detection.")]
		public static readonly string ignoreliquidsetdetection = "tshock.ignore.liquid";

		[Description("Prevents you from being disabled by projectile abuse detection.")]
		public static readonly string ignoreprojectiledetection = "tshock.ignore.projectile";

		[Description("Prevents you from being disabled by paint abuse detection.")]
		public static readonly string ignorepaintdetection = "tshock.ignore.paint";

		[Description("Prevents you from being disabled by stack hack detection.")]
		public static readonly string ignorestackhackdetection = "tshock.ignore.itemstack";

		[Description("Prevents your actions from being ignored if damage is too high.")]
		public static readonly string ignoredamagecap = "tshock.ignore.damage";

		[Description("Bypass server side character checks.")]
		public static readonly string bypassssc = "tshock.ignore.ssc";

		[Description("Allow unrestricted SendTileSquare usage, for client side world editing.")]
		public static readonly string allowclientsideworldedit = "tshock.ignore.sendtilesquare";

		[Description("Allow dropping banned items without the item being eaten.")]
		public static readonly string allowdroppingbanneditems = "tshock.ignore.dropbanneditem";

		[Description("Prevents you from being disabled by abnormal HP.")]
		public static readonly string ignorehp = "tshock.ignore.hp";

		[Description("Prevents you from being disabled by abnormal MP.")]
		public static readonly string ignoremp = "tshock.ignore.mp";

		// tshock.item nodes

		[Description("User can give items.")]
		public static readonly string give = "tshock.item.give";

		[Description("User can spawn items.")]
		public static readonly string item = "tshock.item.spawn";

		[Description("Allows you to use banned items.")]
		public static readonly string usebanneditem = "tshock.item.usebanned";

		// tshock.npc nodes

		[Description("User can edit the max spawns.")]
		public static readonly string maxspawns = "tshock.npc.maxspawns";

		[Description("User can edit the spawnrate.")]
		public static readonly string spawnrate = "tshock.npc.spawnrate";

		[Description("User can start an invasion.")]
		public static readonly string invade = "tshock.npc.invade";

		[Description("User can hurt town NPCs.")]
		public static readonly string hurttownnpc = "tshock.npc.hurttown";

		[Description("User can spawn bosses.")]
		public static readonly string spawnboss = "tshock.npc.spawnboss";

		[Description("User can rename NPCs.")]
		public static readonly string renamenpc = "tshock.npc.rename";

		[Description("User can spawn npcs.")]
		public static readonly string spawnmob = "tshock.npc.spawnmob";

		[Description("User can kill all enemy npcs.")]
		public static readonly string butcher = "tshock.npc.butcher";

		[Description("User can summon bosses using items.")]
		public static readonly string summonboss = "tshock.npc.summonboss";

		[Description("User can start invasions (Goblin/Snow Legion) using items.")]
		public static readonly string startinvasion = "tshock.npc.startinvasion";

		[Description("User can start the dd2 event.")]
		public static readonly string startdd2 = "tshock.npc.startdd2";

		[Description("User can clear the list of users who have completed an angler quest that day.")]
		public static readonly string clearangler = "tshock.npc.clearanglerquests";

		[Description("Meant for super admins only.")]
		public static readonly string user = "tshock.superadmin.user";

		[Description("Allows a user to elevate to superadmin for 10 minutes.")]
		public static readonly string su = "tshock.su";

		// tshock.tp nodes

		[Description("User can teleport *everyone* to them.")]
		public static readonly string tpallothers = "tshock.tp.allothers";

		[Description("User can teleport to others.")]
		public static readonly string tp = "tshock.tp.self";

		[Description("User can teleport other people.")]
		public static readonly string tpothers = "tshock.tp.others";

		[Description("User can teleport to tile positions.")]
		public static readonly string tppos = "tshock.tp.pos";

		[Description("User can get the position of players.")]
		public static readonly string getpos = "tshock.tp.getpos";

		[Description("User can teleport to an NPC.")]
		public static readonly string tpnpc = "tshock.tp.npc";

		[Description("Users can stop people from teleporting.")]
		public static readonly string tpallow = "tshock.tp.block";

		[Description("Users can override teleport blocks.")]
		public static readonly string tpoverride = "tshock.tp.override";

		[Description("Users can teleport to people without showing a notice")]
		public static readonly string tpsilent = "tshock.tp.silent";

		[Description("User can use /home.")]
		public static readonly string home = "tshock.tp.home";

		[Description("User can use /spawn.")]
		public static readonly string spawn = "tshock.tp.spawn";

		[Description("User can use the Rod of Discord.")]
		public static readonly string rod = "tshock.tp.rod";

		[Description("User can use wormhole potions.")]
		public static readonly string wormhole = "tshock.tp.wormhole";

		// tshock.world nodes

		[Description("User can change expert state.")]
		public static readonly string toggleexpert = "tshock.world.toggleexpert";

		[Description("Allows you to edit the spawn.")]
		public static readonly string editspawn = "tshock.world.editspawn";

		[Description("Allows you to edit regions.")]
		public static readonly string editregion = "tshock.world.editregion";

		[Description("User can force a blood moon.")]
		public static readonly string bloodmoon = "tshock.world.time.bloodmoon";

		[Description("User can set the time.")]
		public static readonly string time = "tshock.world.time.set";

		[Description("Player can use the Enchanted Sundial item.")]
		public static readonly string usesundial = "tshock.world.time.usesundial";

		[Description("User can grow plants.")]
		public static readonly string grow = "tshock.world.grow";

		[Description("User can change hardmode state.")]
		public static readonly string hardmode = "tshock.world.hardmode";

		[Description("User can change the homes of NPCs.")]
		public static readonly string movenpc = "tshock.world.movenpc";

		[Description("User can convert hallow into corruption and vice-versa.")]
		public static readonly string converthardmode = "tshock.world.converthardmode";

		[Description("User can force the server to Halloween mode.")]
		public static readonly string halloween = "tshock.world.sethalloween";

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

		[Description("User can set the dungeon's location.")]
		public static readonly string dungeonposition = "tshock.world.setdungeon";

		[Description("User can drop a meteor.")]
		public static readonly string dropmeteor = "tshock.world.time.dropmeteor";

		[Description("User can force an eclipse.")]
		public static readonly string eclipse = "tshock.world.time.eclipse";

		[Description("User can force a full moon.")]
		public static readonly string fullmoon = "tshock.world.time.fullmoon";

		[Description("User can modify the world.")]
		public static readonly string canbuild = "tshock.world.modify";

		[Description("User can paint tiles.")]
		public static readonly string canpaint = "tshock.world.paint";

		[Description("User can turn on or off sandstorms.")]
		public static readonly string sandstorm = "tshock.world.sandstorm";

		[Description("User can turn on or off the rain.")]
		public static readonly string rain = "tshock.world.rain";

		[Description("User can modify the wind.")]
		public static readonly string wind = "tshock.world.wind";

		[Description("Player can toggle party event.")]
		public static readonly string toggleparty = "tshock.world.toggleparty";

		// Non-grouped

		[Description("User can clear items or projectiles.")]
		public static readonly string clear = "tshock.clear";

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

		[Description("User can use party chat in game.")]
		public static readonly string canpartychat = "tshock.partychat";

		[Description("User can talk in third person.")]
		public static readonly string cantalkinthird = "tshock.thirdperson";

		[Description("User can get the server info.")]
		public static readonly string serverinfo = "tshock.info";

		[Description("Player recovers health as damage is taken.  Can be one shotted.")]
		public static readonly string godmode = "tshock.godmode";

		[Description("User can godmode other players.")]
		public static readonly string godmodeother = "tshock.godmode.other";

		[Description("Player can chat.")]
		public static readonly string canchat = "tshock.canchat";

		[Description("Player can use banned projectiles.")]
		public static readonly string canusebannedprojectiles = "tshock.projectiles.usebanned";

		[Description("Player can place banned tiles.")]
		public static readonly string canusebannedtiles = "tshock.tiles.usebanned";

		[Description("Player can check if a username is registered and see its last login time.")]
		public static readonly string checkaccountinfo = "tshock.accountinfo.check";

		[Description("Player can see advanced information about any user account.")]
		public static readonly string advaccountinfo = "tshock.accountinfo.details";

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

				sb.AppendLine("{0}".SFormat(name));
				sb.AppendLine("Description: {0}  ".SFormat(desc));
				sb.AppendLine("Commands: {0}  ".SFormat(strs.Count() > 0 ? string.Join(" ", strs) : "None"));
				sb.AppendLine();
			}

			File.WriteAllText("PermissionsDescriptions.txt", sb.ToString());
		}
	}
}
