using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockAPI.Hooks
{
    public class ReloadEventArgs
    {
        public TSPlayer Player { get; set; }
        public ReloadEventArgs(TSPlayer ply)
        {
            Player = ply;
        }
    }

    public class GeneralHooks
    {
        public delegate void ReloadEventD(ReloadEventArgs e);
        public static event ReloadEventD ReloadEvent;

        public static void OnReloadEvent(TSPlayer ply)
        {
            if(ReloadEvent == null)
                return;

            ReloadEvent(new ReloadEventArgs(ply));
        }
    }
}
