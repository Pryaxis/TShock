using System;
using Terraria;

namespace Hooks
{
	public static class ProjectileHooks
	{
        public static event SetDefaultsD<Projectile, int> SetDefaults;

        public static void OnSetDefaults(ref int type, Projectile proj)
        {
            if (SetDefaults == null)
            {
                return;
            }
            SetDefaultsEventArgs<Projectile, int> setDefaultsEventArgs = new SetDefaultsEventArgs<Projectile, int>
            {
                Object = proj,
                Info = type
            };
            SetDefaults(setDefaultsEventArgs);
            type = setDefaultsEventArgs.Info;
        }
	}
}
