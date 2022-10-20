using GetText;

namespace TShockPluginManager
{
	static class I18n
	{
		static string TranslationsDirectory => Path.Combine(AppContext.BaseDirectory, "i18n");
		// we share the same translations catalog as TShockAPI
		public static Catalog C = new Catalog("TShockAPI", TranslationsDirectory);
	}
}
