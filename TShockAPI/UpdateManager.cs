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
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;

namespace TShockAPI
{
    class UpdateManager
    {
        static string updateUrl = "http://shankshock.com/tshock-update.txt";
        public static bool updateCmd;
        public static DateTime lastcheck = DateTime.MinValue;
        public static string[] globalChanges = {};
        /// <summary>
        /// Check once every X minutes.
        /// </summary>
        static readonly int CheckXMinutes = 30;

        public static void UpdateProcedureCheck()
        {
            if ((DateTime.Now - lastcheck).TotalMinutes >= CheckXMinutes)
            {
                ThreadPool.QueueUserWorkItem(CheckUpdate);
                lastcheck = DateTime.Now;
            }
        }

        public static void CheckUpdate(object o)
        {
            if (ServerIsOutOfDate())
            {
                NotifyAdministrators(globalChanges);
            }
        }

        /// <summary>
        /// Checks to see if the server is out of date.
        /// </summary>
        /// <returns></returns>
        private static bool ServerIsOutOfDate()
        {
            WebClient client = new WebClient();
            client.Headers.Add("user-agent",
                               "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.0.3705;)");
            try
            {
                string updateString = client.DownloadString(updateUrl);
                string[] changes = updateString.Split(',');
                Version updateVersion = new Version(Convert.ToInt32(changes[0]), Convert.ToInt32(changes[1]),
                                                    Convert.ToInt32(changes[2]), Convert.ToInt32(changes[3]));
                if (TShock.VersionNum.CompareTo(updateVersion) < 0)
                {
                    globalChanges = changes;
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
            return false;
        }

        private static void NotifyAdministrators(string[] changes)
        {
            NotifyAdministrator(TSPlayer.Server, changes);
            foreach (TSPlayer player in TShock.Players)
            {
                if (player != null && player.Active && player.Group.HasPermission("maintenance"))
                {
                    NotifyAdministrator(player, changes);
                }
            }
        }

        private static void NotifyAdministrator(TSPlayer player, string[] changes)
        {
            player.SendMessage("The server is out of date.", Color.Red);
            for (int j = 4; j < changes.Length; j++)
            {
                player.SendMessage(changes[j], Color.Red);
            }
        }
    }
}
