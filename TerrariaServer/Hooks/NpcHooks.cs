using Terraria;
using TerrariaServer.Hooks.Classes;

namespace TerrariaServer.Hooks
{
	public static class NpcHooks
	{
		public delegate void StrikeNpcD(NpcStrikeEventArgs e);

	    public delegate void SpawnNpcD(NpcSpawnEventArgs e);

		public static event SetDefaultsD<NPC, int> SetDefaultsInt;
		public static event SetDefaultsD<NPC, string> SetDefaultsString;
        public static event SetDefaultsD<NPC, int> NetDefaults;
		public static event NpcHooks.StrikeNpcD StrikeNpc;
	    public static event SpawnNpcD SpawnNpc;
		public static void OnSetDefaultsInt(ref int npctype, NPC npc)
		{
			if (NpcHooks.SetDefaultsInt == null)
			{
				return;
			}
			SetDefaultsEventArgs<NPC, int> setDefaultsEventArgs = new SetDefaultsEventArgs<NPC, int>
			{
				Object = npc, 
				Info = npctype
			};
			NpcHooks.SetDefaultsInt(setDefaultsEventArgs);
			npctype = setDefaultsEventArgs.Info;
		}
		public static void OnSetDefaultsString(ref string npcname, NPC npc)
		{
			if (NpcHooks.SetDefaultsString == null)
			{
				return;
			}
			SetDefaultsEventArgs<NPC, string> setDefaultsEventArgs = new SetDefaultsEventArgs<NPC, string>
			{
				Object = npc, 
				Info = npcname
			};
			NpcHooks.SetDefaultsString(setDefaultsEventArgs);
			npcname = setDefaultsEventArgs.Info;
		}
        public static void OnNetDefaults(ref int nettype, NPC npc)
        {
            if (NpcHooks.NetDefaults == null)
            {
                return;
            }
            SetDefaultsEventArgs<NPC, int> setDefaultsEventArgs = new SetDefaultsEventArgs<NPC, int>
            {
                Object = npc,
                Info = nettype
            };
            NpcHooks.NetDefaults(setDefaultsEventArgs);
            nettype = setDefaultsEventArgs.Info;
        }
		public static bool OnStrikeNpc(NPC npc, ref int damage, ref float knockback, ref int hitdirection, ref bool crit, ref bool noEffect, ref double retdamage)
		{
			if (NpcHooks.StrikeNpc == null)
			{
				return false;
			}
			NpcStrikeEventArgs npcStrikeEventArgs = new NpcStrikeEventArgs
			{
				Npc = npc, 
				Damage = damage, 
				KnockBack = knockback, 
				HitDirection = hitdirection, 
				Critical = crit,
                NoEffect = noEffect,
                ReturnDamage = retdamage
			};
			NpcHooks.StrikeNpc(npcStrikeEventArgs);
			crit = npcStrikeEventArgs.Critical;
			damage = npcStrikeEventArgs.Damage;
			knockback = npcStrikeEventArgs.KnockBack;
			hitdirection = npcStrikeEventArgs.HitDirection;
		    noEffect = npcStrikeEventArgs.NoEffect;
		    retdamage = npcStrikeEventArgs.ReturnDamage;
			return npcStrikeEventArgs.Handled;
		}

        public static void OnNpcSpawn( NPC type )
        {
            NpcSpawnEventArgs spawn = new NpcSpawnEventArgs
            {
                Npc = type
            };

            SpawnNpc(spawn);
        }
	}
}
