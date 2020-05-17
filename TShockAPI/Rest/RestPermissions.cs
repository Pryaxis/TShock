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

using System.ComponentModel;

// Since the permission nodes have annotations that say what they are, we don't need XML comments.
#pragma warning disable 1591

namespace Rests
{
	/// <summary>Contains the REST permission nodes used in TShock.</summary>
	public static class RestPermissions
	{
		// tshock.rest.bans nodes

		[Description("REST user can list and get detailed information about bans.")]
		public const string restviewbans = "tshock.rest.bans.view";

		[Description("REST user can alter bans.")]
		public const string restmanagebans = "tshock.rest.bans.manage";

		// tshock.rest.groups nodes

		[Description("REST user can list and get detailed information about groups.")]
		public const string restviewgroups = "tshock.rest.groups.view";

		[Description("REST user can alter groups.")]
		public const string restmanagegroups = "tshock.rest.groups.manage";

		// tshock.rest.users nodes

		[Description("REST user can list and get detailed information about users.")]
		public const string restviewusers = "tshock.rest.users.view";

		[Description("REST user can alter users.")]
		public const string restmanageusers = "tshock.rest.users.manage";

		[Description("REST user can get user information.")]
		public const string restuserinfo = "tshock.rest.users.info";

		// Non-grouped nodes

		[Description("User can create REST tokens.")]
		public const string restapi = "tshock.rest.useapi";

		[Description("User or REST user can destroy all REST tokens.")]
		public const string restmanage = "tshock.rest.manage";

		[Description("REST user can turn off / restart the server.")]
		public const string restmaintenance = "tshock.rest.maintenance";

		[Description("REST user can reload configurations, save the world and set auto save settings.")]
		public const string restcfg = "tshock.rest.cfg";

		[Description("REST user can kick players.")]
		public const string restkick = "tshock.rest.kick";

		[Description("REST user can ban players.")]
		public const string restban = "tshock.rest.ban";

		[Description("REST user can mute and unmute players.")]
		public const string restmute = "tshock.rest.mute";

		[Description("REST user can kill players.")]
		public const string restkill = "tshock.rest.kill";

		[Description("REST user can drop meteors or change bloodmoon.")]
		public const string restcauseevents = "tshock.rest.causeevents";

		[Description("REST user can butcher npcs.")]
		public const string restbutcher = "tshock.rest.butcher";

		[Description("REST user can run raw TShock commands (the raw command permissions are also checked though).")]
		public const string restrawcommand = "tshock.rest.command";

		[Description("REST user can view the ips of players.")] 
		public const string viewips = "tshock.rest.viewips";
	}
}
