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

global using static TShockAPI.I18n;

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

		#region ICatalog forwarding methods
		/// <summary>
		/// Returns <paramref name="text"/> translated into the selected language.
		/// Similar to <c>gettext</c> function.
		/// </summary>
		/// <param name="text">Text to translate.</param>
		/// <returns>Translated text.</returns>
		static string GetString(FormattableStringAdapter text)
		{
			return C.GetString(text);
		}

		/// <summary>
		/// Returns <paramref name="text"/> translated into the selected language.
		/// Similar to <c>gettext</c> function.
		/// </summary>
		/// <param name="text">Text to translate.</param>
		/// <returns>Translated text.</returns>
		public static string GetString(FormattableString text)
		{
			return C.GetString(text);
		}

		/// <summary>
		/// Returns <paramref name="text"/> translated into the selected language.
		/// Similar to <c>gettext</c> function.
		/// </summary>
		/// <param name="text">Text to translate.</param>
		/// <param name="args">Optional arguments for <see cref="string.Format(string, object[])"/> method.</param>
		/// <returns>Translated text.</returns>
		public static string GetString(FormattableStringAdapter text, params object[] args)
		{
			return C.GetString(text, args);
		}

		/// <summary>
		/// Returns the plural form for <paramref name="n"/> of the translation of <paramref name="text"/>.
		/// Similar to <c>gettext</c> function.
		/// </summary>
		/// <param name="text">Singular form of message to translate.</param>
		/// <param name="pluralText">Plural form of message to translate.</param>
		/// <param name="n">Value that determines the plural form.</param>
		/// <returns>Translated text.</returns>
		public static string GetPluralString(FormattableStringAdapter text, FormattableStringAdapter pluralText, long n)
		{
			return C.GetString(text, pluralText, n);
		}

		/// <summary>
		/// Returns the plural form for <paramref name="n"/> of the translation of <paramref name="text"/>.
		/// Similar to <c>gettext</c> function.
		/// </summary>
		/// <param name="text">Singular form of message to translate.</param>
		/// <param name="pluralText">Plural form of message to translate.</param>
		/// <param name="n">Value that determines the plural form.</param>
		/// <returns>Translated text.</returns>
		public static string GetPluralString(FormattableString text, FormattableString pluralText, long n)
		{
			return C.GetString(text, pluralText, n);
		}

		/// <summary>
		/// Returns the plural form for <paramref name="n"/> of the translation of <paramref name="text"/>.
		/// Similar to <c>gettext</c> function.
		/// </summary>
		/// <param name="text">Singular form of message to translate.</param>
		/// <param name="pluralText">Plural form of message to translate.</param>
		/// <param name="n">Value that determines the plural form.</param>
		/// <param name="args">Optional arguments for <see cref="string.Format(string, object[])"/> method.</param>
		/// <returns>Translated text.</returns>
		public static string GetPluralString(FormattableStringAdapter text, FormattableStringAdapter pluralText, long n, params object[] args)
		{
			return C.GetString(text, pluralText, n, args);
		}

		/// <summary>
		/// Returns <paramref name="text"/> translated into the selected language using given <paramref name="context"/>.
		/// Similar to <c>pgettext</c> function.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="text">Text to translate.</param>
		/// <returns>Translated text.</returns>
		public static string GetParticularString(string context, FormattableStringAdapter text)
		{
			return C.GetParticularString(context, text);
		}

		/// <summary>
		/// Returns <paramref name="text"/> translated into the selected language using given <paramref name="context"/>.
		/// Similar to <c>pgettext</c> function.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="text">Text to translate.</param>
		/// <returns>Translated text.</returns>
		public static string GetParticularString(string context, FormattableString text)
		{
			return C.GetParticularString(context, text);
		}

		/// <summary>
		/// Returns <paramref name="text"/> translated into the selected language using given <paramref name="context"/>.
		/// Similar to <c>pgettext</c> function.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="text">Text to translate.</param>
		/// <param name="args">Optional arguments for <see cref="string.Format(string, object[])"/> method.</param>
		/// <returns>Translated text.</returns>
		public static string GetParticularString(string context, FormattableStringAdapter text, params object[] args)
		{
			return C.GetParticularString(context, text, args);
		}

		/// <summary>
		/// Returns the plural form for <paramref name="n"/> of the translation of <paramref name="text"/> using given <paramref name="context"/>.
		/// Similar to <c>npgettext</c> function.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="text">Singular form of message to translate.</param>
		/// <param name="pluralText">Plural form of message to translate.</param>
		/// <param name="n">Value that determines the plural form.</param>
		/// <returns>Translated text.</returns>
		public static string GetParticularPluralString(string context, FormattableStringAdapter text, FormattableStringAdapter pluralText, long n)
		{
			return C.GetParticularString(context, text, pluralText, n);
		}

		/// <summary>
		/// Returns the plural form for <paramref name="n"/> of the translation of <paramref name="text"/> using given <paramref name="context"/>.
		/// Similar to <c>npgettext</c> function.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="text">Singular form of message to translate.</param>
		/// <param name="pluralText">Plural form of message to translate.</param>
		/// <param name="n">Value that determines the plural form.</param>
		/// <returns>Translated text.</returns>
		public static string GetParticularPluralString(string context, FormattableString text, FormattableString pluralText, long n)
		{
			return C.GetParticularString(context, text, pluralText, n);
		}

		/// <summary>
		/// Returns the plural form for <paramref name="n"/> of the translation of <paramref name="text"/> using given <paramref name="context"/>.
		/// Similar to <c>npgettext</c> function.
		/// </summary>
		/// <param name="context">Context.</param>
		/// <param name="text">Singular form of message to translate.</param>
		/// <param name="pluralText">Plural form of message to translate.</param>
		/// <param name="n">Value that determines the plural form.</param>
		/// <param name="args">Optional arguments for <see cref="string.Format(string, object[])"/> method.</param>
		/// <returns>Translated text.</returns>
		public static string GetParticularPluralString(string context, FormattableStringAdapter text, FormattableStringAdapter pluralText, long n, params object[] args)
		{
			return C.GetParticularString(context, text, pluralText, n, args);
		}
		#endregion
	}
}
