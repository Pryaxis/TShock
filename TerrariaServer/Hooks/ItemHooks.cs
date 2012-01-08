using Terraria;
using TerrariaServer.Hooks.Classes;

namespace TerrariaServer.Hooks
{
	public static class ItemHooks
	{
		public static event SetDefaultsD<Item, int> SetDefaultsInt;
		public static event SetDefaultsD<Item, string> SetDefaultsString;
        public static event SetDefaultsD<Item, int> NetDefaults;
		public static void OnSetDefaultsInt(ref int itemtype, Item item)
		{
			if (ItemHooks.SetDefaultsInt == null)
			{
				return;
			}
			SetDefaultsEventArgs<Item, int> setDefaultsEventArgs = new SetDefaultsEventArgs<Item, int>
			{
				Object = item, 
				Info = itemtype
			};
			ItemHooks.SetDefaultsInt(setDefaultsEventArgs);
			itemtype = setDefaultsEventArgs.Info;
		}
		public static void OnSetDefaultsString(ref string itemname, Item item)
		{
			if (ItemHooks.SetDefaultsString == null)
			{
				return;
			}
			SetDefaultsEventArgs<Item, string> setDefaultsEventArgs = new SetDefaultsEventArgs<Item, string>
			{
				Object = item, 
				Info = itemname
			};
			ItemHooks.SetDefaultsString(setDefaultsEventArgs);
			itemname = setDefaultsEventArgs.Info;
		}
        public static void OnNetDefaults(ref int nettype, Item item)
        {
            if (ItemHooks.NetDefaults == null)
            {
                return;
            }
            SetDefaultsEventArgs<Item, int> setDefaultsEventArgs = new SetDefaultsEventArgs<Item, int>
            {
                Object = item,
                Info = nettype
            };
            ItemHooks.NetDefaults(setDefaultsEventArgs);
            nettype = setDefaultsEventArgs.Info;
        }

	}
}
