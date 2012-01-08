using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using TerrariaServer.Hooks.Classes;

namespace TShock.Hooks.NPC
{
    public class NpcHooks
    {
        internal NpcHooks()
        {
            Spawn = new HandlerList<NpcEventArgs>();

            TerrariaServer.Hooks.NpcHooks.SpawnNpc += NpcHooks_Spawn;
        }

        void NpcHooks_Spawn( NpcSpawnEventArgs args )
        {
            var e = new NpcEventArgs(args.Npc);
            Spawn.Invoke(this, e);
        }

        /// <summary>
        /// Called when an NPC spawns
        /// </summary>
        public HandlerList<NpcEventArgs> Spawn { get; set; }
    }
}
