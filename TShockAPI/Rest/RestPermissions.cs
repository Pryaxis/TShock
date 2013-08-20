/*
TShock, a server mod for Terraria
Copyright (C) 2011-2012 The TShock Team

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
using System.ComponentModel;
using System.Linq;

namespace Rests
{
	public static class RestPermissions
	{
		[Description("User can create REST tokens.")]
		public static readonly string restapi;

		[Description("User or REST user can destroy all REST tokens.")]
		public static readonly string restmanage;


		[Description("REST user can turn off / restart the server.")]
		public static readonly string restmaintenance;

		[Description("REST user can reload configurations, save the world and set auto save settings.")]
		public static readonly string restcfg;


		[Description("REST user can list and get detailed information about users.")]
		public static readonly string restviewusers;

		[Description("REST user can alter users.")]
		public static readonly string restmanageusers;

		[Description("REST user can list and get detailed information about bans.")]
		public static readonly string restviewbans;

		[Description("REST user can alter bans.")]
		public static readonly string restmanagebans;

		[Description("REST user can list and get detailed information about groups.")]
		public static readonly string restviewgroups;

		[Description("REST user can alter groups.")]
		public static readonly string restmanagegroups;


		[Description("REST user can get user information.")]
		public static readonly string restuserinfo;

		[Description("REST user can kick players.")]
		public static readonly string restkick;

		[Description("REST user can ban players.")]
		public static readonly string restban;

		[Description("REST user can mute and unmute players.")]
		public static readonly string restmute;

		[Description("REST user can kill players.")]
		public static readonly string restkill;


		[Description("REST user can drop meteors or change bloodmoon.")]
		public static readonly string restcauseevents;

		[Description("REST user can butcher npcs.")]
		public static readonly string restbutcher;


		[Description("REST user can run raw TShock commands (the raw command permissions are also checked though).")]
		public static readonly string restrawcommand;

		static RestPermissions()
		{
			foreach (var field in typeof (RestPermissions).GetFields())
			{
				field.SetValue(null, field.Name);
			}
		}
	}
}
