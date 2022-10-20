/*
TShock, a server mod for Terraria
Copyright (C) 2022 Janet Blackquill

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
using System.Globalization;
using System.IO;
using GetText;

namespace TShockAPI
{
	static class I18n {
		static string TranslationsDirectory => Path.Combine(AppContext.BaseDirectory, "i18n");
		static CultureInfo TranslationCultureInfo
		{
			get
			{
				// cross-platform mapping of cultureinfos can be a bit screwy, so give our users
				// the chance to explicitly spell out which translation they would like to use.
				// this is an environment variable instead of a flag because this needs to be
				// valid whether the passed flags are in a sane state or not.
				if (Environment.GetEnvironmentVariable("TSHOCK_LANGUAGE") is string overrideLang)
				{
					return new CultureInfo(overrideLang);
				}
				return CultureInfo.CurrentUICulture;
			}
		}
		/// <value>Instance of a <c>GetText.Catalog</c> loaded with TShockAPI translations for user's specified language</value>
		public static Catalog C = new Catalog("TShockAPI", TranslationsDirectory, TranslationCultureInfo);
	}
}
