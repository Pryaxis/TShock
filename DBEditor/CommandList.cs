using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockDBEditor
{
    public class Commandlist
    {
        public static List<string> CommandList = new List<string>();

        public static void AddCommands()
        {
            CommandList.Add("reservedslot");
            CommandList.Add("canwater");
            CommandList.Add("canlava");
            CommandList.Add("warp");
            CommandList.Add("kick");
            CommandList.Add("ban");
            CommandList.Add("unban");
            CommandList.Add("whitelist");
            CommandList.Add("maintenace");
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
            CommandList.Add("cheat");
            CommandList.Add("immunetokick");
            CommandList.Add("immunetoban");
            CommandList.Add("ignorecheatdetection");
            CommandList.Add("ignoregriefdetection");
            CommandList.Add("usebanneditem");
        }
    }
}
