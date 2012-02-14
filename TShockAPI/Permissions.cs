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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace TShockAPI
{
	public static class Permissions
	{
		//Permissions with blank descriptions basically means its described by the commands it gives access to.

		[Description("")] public static readonly string causeevents;

		[Description("Required to be able to build (modify tiles and liquid)")] public static readonly string canbuild;

		[Description("")] public static readonly string kill;

		[Description("Allows you to use banned items")] public static readonly string usebanneditem;

		[Description("Allows you to edit the spawn")] public static readonly string editspawn;

		[Description("Prevents you from being kicked")] public static readonly string immunetokick;

		[Description("Prevents you from being banned")] public static readonly string immunetoban;

		[Description("Prevents you from being reverted by kill tile abuse detection")] public static readonly string
			ignorekilltiledetection;

		[Description("Prevents you from being reverted by place tile abuse detection")] public static readonly string
			ignoreplacetiledetection;

		[Description("Prevents you from being disabled by liquid set abuse detection")] public static readonly string
			ignoreliquidsetdetection;

		[Description("Prevents you from being disabled by liquid set abuse detection")] public static readonly string
			ignoreprojectiledetection;

		[Description("Prevents you from being reverted by no clip detection")] public static readonly string
			ignorenoclipdetection;

		[Description("Prevents you from being disabled by stack hack detection")] public static readonly string
			ignorestackhackdetection;

		[Description("Prevents you from being kicked by hacked health detection")] public static readonly string
			ignorestathackdetection;

	    [Description("Prevents your actions from being ignored if damage is too high")] public static readonly string
	        ignoredamagecap;

		[Description("Specific log messages are sent to users with this permission")] public static readonly string logs;

		[Description("Allows you to bypass the max slots for up to 5 slots above your max")] public static readonly string
			reservedslot;

		[Description("User is notified when an update is available")] public static readonly string maintenance;

		[Description("User can kick others")] public static readonly string kick;

		[Description("User can ban others")] public static readonly string ban;

		[Description("User can modify the whitelist")] public static readonly string whitelist;

		[Description("User can spawn bosses")] public static readonly string spawnboss;

		[Description("User can spawn npcs")] public static readonly string spawnmob;

		[Description("User can teleport")] public static readonly string tp;

		[Description("User can teleport people to them")] public static readonly string tphere;

		[Description("User can use warps")] public static readonly string warp;

		[Description("User can manage warps")] public static readonly string managewarp;

		[Description("User can manage item bans")] public static readonly string manageitem;

		[Description("User can manage groups")] public static readonly string managegroup;
		
		[Description("User can manage server side inventory")] public static readonly string serversideinventory;

		[Description("User can edit sevrer configurations")] public static readonly string cfg;

		[Description("")] public static readonly string time;

		[Description("")] public static readonly string pvpfun;

		[Description("User can edit regions")] public static readonly string manageregion;

		[Description("Meant for super admins only")] public static readonly string rootonly;

		[Description("User can whisper to others")] public static readonly string whisper;

		[Description("")] public static readonly string annoy;

		[Description("User can kill all enemy npcs")] public static readonly string butcher;

		[Description("User can spawn items")] public static readonly string item;

		[Description("User can clear item drops.")] public static readonly string clearitems;

		[Description("")] public static readonly string heal;

		[Description("User can buff self")] public static readonly string buff;

		[Description("User can buff other players")] public static readonly string buffplayer;

		[Description("")] public static readonly string grow;

		[Description("User can change hardmode state.")] public static readonly string hardmode;

		[Description("User can change the homes of NPCs.")] public static readonly string movenpc;

		[Description("Users can stop people from TPing to them")] public static readonly string tpallow;

		[Description("Users can tp to anyone")] public static readonly string tpall;

		[Description("Users can tp to people without showing a notice")] public static readonly string tphide;

		[Description("User can convert hallow into corruption and vice-versa")] public static readonly string converthardmode;

		[Description("User can mute and unmute users")] public static readonly string mute;

		[Description("User can register account in game")] public static readonly string canregister;

		[Description("User can login in game")] public static readonly string canlogin;

		[Description("User can change password in game")] public static readonly string canchangepassword;

		[Description("User can use party chat in game")] public static readonly string canpartychat;

		[Description("User can talk in third person")] public static readonly string cantalkinthird;

		[Description("Bypass Server Side Inventory checks")] public static readonly string bypassinventorychecks;

		[Description("Allow unrestricted Send Tile Square usage, for client side world editing")] public static readonly
			string allowclientsideworldedit;

        [Description("User can summon bosses using items")]
	    public static readonly string summonboss;

        [Description("User can start invasions (Goblin/Snow Legion) using items")]
        public static readonly string startinvasion;

        [Description("User can see the id of players with /who")]
        public static readonly string seeids;

        static Permissions()
		{
			foreach (var field in typeof (Permissions).GetFields())
			{
				field.SetValue(null, field.Name);
			}
		}

		private static List<Command> GetCommands(string perm)
		{
			if (Commands.ChatCommands.Count < 1)
				Commands.InitCommands();
			return Commands.ChatCommands.Where(c => c.Permission == perm).ToList();
		}

		public static void DumpDescriptions()
		{
			var sb = new StringBuilder();
			foreach (var field in typeof(Permissions).GetFields().OrderBy(f => f.Name))
			{
				var name = field.Name;

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

				sb.AppendLine("## <a name=\"{0}\">{0}</a>  ".SFormat(name));
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