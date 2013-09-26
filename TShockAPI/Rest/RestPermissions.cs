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

using System.ComponentModel;

namespace Rests
{
	public static class RestPermissions
	{
		// tshock.rest.bans nodes

		[Description("REST user can list and get detailed information about bans.")]
		public static readonly string restviewbans = "tshock.rest.bans.view";

		[Description("REST user can alter bans.")]
		public static readonly string restmanagebans = "tshock.rest.bans.manage";

		// tshock.rest.groups nodes

		[Description("REST user can list and get detailed information about groups.")]
		public static readonly string restviewgroups = "tshock.rest.groups.view";

		[Description("REST user can alter groups.")]
		public static readonly string restmanagegroups = "tshock.rest.groups.manage";

		// tshock.rest.users nodes

		[Description("REST user can list and get detailed information about users.")]
		public static readonly string restviewusers = "tshock.rest.users.view";

		[Description("REST user can alter users.")]
		public static readonly string restmanageusers = "tshock.rest.users.manage";

		[Description("REST user can get user information.")]
		public static readonly string restuserinfo = "tshock.rest.users.info";

		// Non-grouped nodes

		[Description("User can create REST tokens.")]
		public static readonly string restapi = "tshock.rest.useapi";

		[Description("User or REST user can destroy all REST tokens.")]
		public static readonly string restmanage = "tshock.rest.manage";

		[Description("REST user can turn off / restart the server.")]
		public static readonly string restmaintenance = "tshock.rest.maintenance";

		[Description("REST user can reload configurations, save the world and set auto save settings.")]
		public static readonly string restcfg = "tshock.rest.cfg";

		[Description("REST user can kick players.")]
		public static readonly string restkick = "tshock.rest.kick";

		[Description("REST user can ban players.")]
		public static readonly string restban = "tshock.rest.ban";

		[Description("REST user can mute and unmute players.")]
		public static readonly string restmute = "tshock.rest.mute";

		[Description("REST user can kill players.")]
		public static readonly string restkill = "tshock.rest.kill";

		[Description("REST user can drop meteors or change bloodmoon.")]
		public static readonly string restcauseevents = "tshock.rest.causeevents";

		[Description("REST user can butcher npcs.")]
		public static readonly string restbutcher = "tshock.rest.butcher";

		[Description("REST user can run raw TShock commands (the raw command permissions are also checked though).")]
		public static readonly string restrawcommand = "tshock.rest.command";
	}
}
