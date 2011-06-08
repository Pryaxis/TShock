using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace TShockAPI
{
    class UpdateManager
    {
        static string updateUrl = "http://shankshock.com/tshock-update.txt";
        public static bool updateCmd;
        public static long updateEpoch = 0;
        public static string[] globalChanges = {};
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
            for (int i = 0; i < ConfigurationManager.maxSlots; i++)
            {
                if (Terraria.Main.player[i].active)
                {
                    if (!TShock.players[i].group.HasPermission("maintenance"))
                        return;
                    Tools.SendMessage(i, "The server is out of date. To update, type /updatenow.");
                    for (int j = 4; j <= changes.Length; j++)
                    {
                        Tools.SendMessage(i, changes[i], new float[] { 255, 0, 0 });
                    }
                }
            }
        }

        public static void UpdateProcedureCheck()
        {
            long currentEpoch = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
            if (currentEpoch > UpdateManager.updateEpoch)
            {
                if (ServerIsOutOfDate())
                {
                    EnableUpdateCommand();
                    NotifyAdministrators(globalChanges);
                }
            }
        }

    }
}
