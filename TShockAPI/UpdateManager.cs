using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        /// <summary>
        /// Checks to see if the server is out of date.
        /// </summary>
        /// <returns></returns>
        public static bool ServerIsOutOfDate()
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
                FileTools.WriteError(e.Message);
            }
            return false;
        }

        public static void EnableUpdateCommand()
        {
            Commands.commands.Add(new Commands.Command("updatenow", "maintenance", Commands.UpdateNow));
            updateCmd = true;
        }
        
        public static void NotifyAdministrators(string[] changes)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                if (Main.player[i].active)
                {
                    if (!TShock.players[i].group.HasPermission("maintenance"))
                        return;
                    Tools.SendMessage(i, "The server is out of date. To update, type /updatenow.");
                    for (int j = 4; j < changes.Length; j++)
                    {
                        Tools.SendMessage(i, changes[j],  255, 0, 0);
                    }
                }
            }
        }

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
                EnableUpdateCommand();
                NotifyAdministrators(globalChanges);
            }
        }
    }
}
