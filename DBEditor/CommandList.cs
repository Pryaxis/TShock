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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockDBEditor
{
    public class TShockCommandsList
    {
        public static void AddRemainingTShockCommands()
        {
            List<string> CommandList = new List<string>();

            CommandList.Add("reservedslot");
            CommandList.Add("canwater");
            CommandList.Add("canlava");
            CommandList.Add("canbuild");
            CommandList.Add("adminchat");
            CommandList.Add("warp");
            CommandList.Add("kick");
            CommandList.Add("ban");
            CommandList.Add("unban");
            CommandList.Add("whitelist");
            CommandList.Add("maintenance");
            CommandList.Add("causeevents");
            CommandList.Add("spawnboss");
            CommandList.Add("spawnmob");
            CommandList.Add("tp");
            CommandList.Add("tphere");
            CommandList.Add("managewarp");
            CommandList.Add("editspawn");
            CommandList.Add("cfg");
            CommandList.Add("time");
            CommandList.Add("pvpfun");
            CommandList.Add("logs");
            CommandList.Add("kill");
            CommandList.Add("butcher");
            CommandList.Add("item");
            CommandList.Add("clearitems");
            CommandList.Add("heal");
            CommandList.Add("whisper");
            CommandList.Add("annoy");
            CommandList.Add("immunetokick");
            CommandList.Add("immunetoban");
            CommandList.Add("usebanneditem");
			
            CommandList.Add("canregister");
            CommandList.Add("canlogin");
            CommandList.Add("canchangepassword");
            CommandList.Add("canpartychat");
            CommandList.Add("cantalkinthird");
            CommandList.Add("candisplayplaying");

            foreach (string command in CommandList)
            {
                if (!TShockDBEditor.CommandList.Contains(command))
                    TShockDBEditor.CommandList.Add(command);
            }
        }
    }
}
