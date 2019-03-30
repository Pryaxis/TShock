/*
TShock, a server mod for Terraria
Copyright (C) 2011-2019 Pryaxis & TShock Contributors

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

/* GeoIPCountry.cs
 *
 * Copyright (C) 2008 MaxMind, Inc.  All Rights Reserved.
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public
 * License as published by the Free Software Foundation; either
 * version 2 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * General Public License for more details.
 *
 * You should have received a copy of the GNU General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

// This code is based on MaxMind's original C# code, which was ported from Java.
// This version is very simplified and does not support a majority of features for speed.

namespace MaxMind
{
	/// <summary>
	/// Allows for looking up a country based on an IP address.  See www.maxmind.com for more details.
	/// </summary>
	/// <example>
	///		static void Main(string[] args)
	///		{
	///			using(GeoIPCountry geo = new GeoIPCountry("GeoIP.dat"))
	///			{
	///				try
	///				{
	///					Console.WriteLine("Country code of IP address 67.15.94.80: " + geo.GetCountryCode("67.15.94.80"));
	///				}
	///				catch(Exception ex)
	///				{
	///					Console.WriteLine(ex.ToString());
	///				}
	///			}
	///		}
	/// </example>
	public sealed class GeoIPCountry : IDisposable
	{
		private Stream _geodata;
		private bool _close;

		// hard coded position of where country data starts in the data file.
		private const long COUNTRY_BEGIN = 16776960;

		private static readonly string[] CountryCodes = {
		                                                	"--", "AP", "EU", "AD", "AE", "AF", "AG", "AI", "AL", "AM", "AN",
		                                                	"AO", "AQ", "AR", "AS",
		                                                	"AT", "AU", "AW", "AZ", "BA", "BB", "BD", "BE", "BF", "BG", "BH",
		                                                	"BI", "BJ", "BM", "BN",
		                                                	"BO", "BR", "BS", "BT", "BV", "BW", "BY", "BZ", "CA", "CC", "CD",
		                                                	"CF", "CG", "CH", "CI",
		                                                	"CK", "CL", "CM", "CN", "CO", "CR", "CU", "CV", "CX", "CY", "CZ",
		                                                	"DE", "DJ", "DK", "DM",
		                                                	"DO", "DZ", "EC", "EE", "EG", "EH", "ER", "ES", "ET", "FI", "FJ",
		                                                	"FK", "FM", "FO", "FR",
		                                                	"FX", "GA", "GB", "GD", "GE", "GF", "GH", "GI", "GL", "GM", "GN",
		                                                	"GP", "GQ", "GR", "GS",
		                                                	"GT", "GU", "GW", "GY", "HK", "HM", "HN", "HR", "HT", "HU", "ID",
		                                                	"IE", "IL", "IN", "IO",
		                                                	"IQ", "IR", "IS", "IT", "JM", "JO", "JP", "KE", "KG", "KH", "KI",
		                                                	"KM", "KN", "KP", "KR",
		                                                	"KW", "KY", "KZ", "LA", "LB", "LC", "LI", "LK", "LR", "LS", "LT",
		                                                	"LU", "LV", "LY", "MA",
		                                                	"MC", "MD", "MG", "MH", "MK", "ML", "MM", "MN", "MO", "MP", "MQ",
		                                                	"MR", "MS", "MT", "MU",
		                                                	"MV", "MW", "MX", "MY", "MZ", "NA", "NC", "NE", "NF", "NG", "NI",
		                                                	"NL", "NO", "NP", "NR",
		                                                	"NU", "NZ", "OM", "PA", "PE", "PF", "PG", "PH", "PK", "PL", "PM",
		                                                	"PN", "PR", "PS", "PT",
		                                                	"PW", "PY", "QA", "RE", "RO", "RU", "RW", "SA", "SB", "SC", "SD",
		                                                	"SE", "SG", "SH", "SI",
		                                                	"SJ", "SK", "SL", "SM", "SN", "SO", "SR", "ST", "SV", "SY", "SZ",
		                                                	"TC", "TD", "TF", "TG",
		                                                	"TH", "TJ", "TK", "TM", "TN", "TO", "TL", "TR", "TT", "TV", "TW",
		                                                	"TZ", "UA", "UG", "UM",
		                                                	"US", "UY", "UZ", "VA", "VC", "VE", "VG", "VI", "VN", "VU", "WF",
		                                                	"WS", "YE", "YT", "RS",
		                                                	"ZA", "ZM", "ME", "ZW", "A1", "A2", "O1", "AX", "GG", "IM", "JE",
		                                                	"BL", "MF"
		                                                };

		private static readonly string[] CountryNames = {
		                                                	"N/A", "Asia/Pacific Region", "Europe", "Andorra",
		                                                	"United Arab Emirates", "Afghanistan",
		                                                	"Antigua and Barbuda", "Anguilla", "Albania", "Armenia",
		                                                	"Netherlands Antilles", "Angola",
		                                                	"Antarctica", "Argentina", "American Samoa", "Austria", "Australia",
		                                                	"Aruba", "Azerbaijan",
		                                                	"Bosnia and Herzegovina", "Barbados", "Bangladesh", "Belgium",
		                                                	"Burkina Faso", "Bulgaria",
		                                                	"Bahrain", "Burundi", "Benin", "Bermuda", "Brunei Darussalam",
		                                                	"Bolivia", "Brazil", "Bahamas",
		                                                	"Bhutan", "Bouvet Island", "Botswana", "Belarus", "Belize", "Canada",
		                                                	"Cocos (Keeling) Islands",
		                                                	"Congo, The Democratic Republic of the", "Central African Republic",
		                                                	"Congo", "Switzerland",
		                                                	"Cote D'Ivoire", "Cook Islands", "Chile", "Cameroon", "China",
		                                                	"Colombia", "Costa Rica", "Cuba",
		                                                	"Cape Verde", "Christmas Island", "Cyprus", "Czech Republic",
		                                                	"Germany", "Djibouti", "Denmark",
		                                                	"Dominica", "Dominican Republic", "Algeria", "Ecuador", "Estonia",
		                                                	"Egypt", "Western Sahara",
		                                                	"Eritrea", "Spain", "Ethiopia", "Finland", "Fiji",
		                                                	"Falkland Islands (Malvinas)",
		                                                	"Micronesia, Federated States of", "Faroe Islands", "France",
		                                                	"France, Metropolitan", "Gabon",
		                                                	"United Kingdom", "Grenada", "Georgia", "French Guiana", "Ghana",
		                                                	"Gibraltar", "Greenland",
		                                                	"Gambia", "Guinea", "Guadeloupe", "Equatorial Guinea", "Greece",
		                                                	"South Georgia and the South Sandwich Islands", "Guatemala", "Guam",
		                                                	"Guinea-Bissau", "Guyana",
		                                                	"Hong Kong", "Heard Island and McDonald Islands", "Honduras",
		                                                	"Croatia", "Haiti", "Hungary",
		                                                	"Indonesia", "Ireland", "Israel", "India",
		                                                	"British Indian Ocean Territory", "Iraq",
		                                                	"Iran, Islamic Republic of", "Iceland", "Italy", "Jamaica", "Jordan",
		                                                	"Japan", "Kenya",
		                                                	"Kyrgyzstan", "Cambodia", "Kiribati", "Comoros",
		                                                	"Saint Kitts and Nevis",
		                                                	"Korea, Democratic People's Republic of", "Korea, Republic of",
		                                                	"Kuwait", "Cayman Islands",
		                                                	"Kazakstan", "Lao People's Democratic Republic", "Lebanon",
		                                                	"Saint Lucia", "Liechtenstein",
		                                                	"Sri Lanka", "Liberia", "Lesotho", "Lithuania", "Luxembourg",
		                                                	"Latvia", "Libyan Arab Jamahiriya",
		                                                	"Morocco", "Monaco", "Moldova, Republic of", "Madagascar",
		                                                	"Marshall Islands", "Macedonia",
		                                                	"Mali", "Myanmar", "Mongolia", "Macau", "Northern Mariana Islands",
		                                                	"Martinique", "Mauritania",
		                                                	"Montserrat", "Malta", "Mauritius", "Maldives", "Malawi", "Mexico",
		                                                	"Malaysia", "Mozambique",
		                                                	"Namibia", "New Caledonia", "Niger", "Norfolk Island", "Nigeria",
		                                                	"Nicaragua", "Netherlands",
		                                                	"Norway", "Nepal", "Nauru", "Niue", "New Zealand", "Oman", "Panama",
		                                                	"Peru", "French Polynesia",
		                                                	"Papua New Guinea", "Philippines", "Pakistan", "Poland",
		                                                	"Saint Pierre and Miquelon",
		                                                	"Pitcairn Islands", "Puerto Rico", "Palestinian Territory",
		                                                	"Portugal", "Palau", "Paraguay",
		                                                	"Qatar", "Reunion", "Romania", "Russian Federation", "Rwanda",
		                                                	"Saudi Arabia",
		                                                	"Solomon Islands", "Seychelles", "Sudan", "Sweden", "Singapore",
		                                                	"Saint Helena", "Slovenia",
		                                                	"Svalbard and Jan Mayen", "Slovakia", "Sierra Leone", "San Marino",
		                                                	"Senegal", "Somalia",
		                                                	"Suriname", "Sao Tome and Principe", "El Salvador",
		                                                	"Syrian Arab Republic", "Swaziland",
		                                                	"Turks and Caicos Islands", "Chad", "French Southern Territories",
		                                                	"Togo", "Thailand",
		                                                	"Tajikistan", "Tokelau", "Turkmenistan", "Tunisia", "Tonga",
		                                                	"Timor-Leste", "Turkey",
		                                                	"Trinidad and Tobago", "Tuvalu", "Taiwan",
		                                                	"Tanzania, United Republic of", "Ukraine", "Uganda",
		                                                	"United States Minor Outlying Islands", "United States", "Uruguay",
		                                                	"Uzbekistan",
		                                                	"Holy See (Vatican City State)", "Saint Vincent and the Grenadines",
		                                                	"Venezuela",
		                                                	"Virgin Islands, British", "Virgin Islands, U.S.", "Vietnam",
		                                                	"Vanuatu", "Wallis and Futuna",
		                                                	"Samoa", "Yemen", "Mayotte", "Serbia", "South Africa", "Zambia",
		                                                	"Montenegro", "Zimbabwe",
		                                                	"Anonymous Proxy", "Satellite Provider", "Other", "Aland Islands",
		                                                	"Guernsey", "Isle of Man",
		                                                	"Jersey", "Saint Barthelemy", "Saint Martin"
		                                                };

		//
		// Constructor
		//

		/// <summary>
		/// Initialises a new instance of this class.
		/// </summary>
		/// <param name="datafile">An already open stream pointing to the contents of a GeoIP.dat file.</param>
		/// <remarks>The stream is not closed when this class is disposed. Be sure to clean up afterwards!</remarks>
		public GeoIPCountry(Stream datafile)
		{
			_geodata = datafile;
			_close = false;
		}

		/// <summary>
		/// Initialises a new instance of this class, using an on-disk database.
		/// </summary>
		/// <param name="filename">Path to database file.</param>
		/// <remarks>The file will be closed when this class is disposed.</remarks>
		public GeoIPCountry(string filename)
		{
			FileStream fs = new FileStream(filename, FileMode.Open);
			_geodata = fs;
			_close = true;
		}

		/// <summary>
		/// Retrieves a two-letter code, defined by MaxMind, which details the country the specified IP address is located.
		/// </summary>
		/// <param name="ip">IP address to query.</param>
		/// <returns>A two-letter code string. Throws exceptions on failure.</returns>
		/// <remarks>The IP address must be IPv4.</remarks>
		public string GetCountryCode(IPAddress ip)
		{
			return CountryCodes[FindIndex(ip)];
		}

		/// <summary>
		/// Retrieves a two-letter code, defined by MaxMind, which details the country the specified IP address is located. Does not throw exceptions on failure.
		/// </summary>
		/// <param name="ip">IP address to query.</param>
		/// <returns>Two-letter country code or null on failure.</returns>
		public string TryGetCountryCode(IPAddress ip)
		{
			try
			{
				return CountryCodes[FindIndex(ip)];
			}
			catch (Exception)
			{
				return null;
			}
		}

		/// <summary>
		/// Gets the English name of a country, by a country code.
		/// </summary>
		/// <param name="countrycode">Country code to look up, returned by GetCountryCode or TryGetCountryCode.</param>
		/// <returns>English name of the country, or null on failure.</returns>
		public static string GetCountryNameByCode(string countrycode)
		{
			int index = Array.IndexOf(CountryCodes, countrycode);
			return index == -1 ? null : CountryNames[index];
		}

		private int FindIndex(IPAddress ip)
		{
			return (int) FindCountryCode(0, AddressToLong(ip), 31);
		}

		// Converts an IPv4 address into a long, for reading from geo database
		private long AddressToLong(IPAddress ip)
		{
			if (ip.AddressFamily != AddressFamily.InterNetwork)
				throw new InvalidOperationException("IP address is not IPv4");

			long num = 0;
			byte[] bytes = ip.GetAddressBytes();
			for (int i = 0; i < 4; ++i)
			{
				long y = bytes[i];
				if (y < 0)
					y += 256;
				num += y << ((3 - i)*8);
			}

			return num;
		}

		// Traverses the GeoIP binary data looking for a country code based
		// on the IP address mask
		private long FindCountryCode(long offset, long ipnum, int depth)
		{
			byte[] buffer = new byte[6]; // 2 * MAX_RECORD_LENGTH
			long[] x = new long[2];
			if (depth < 0)
				throw new IOException("Cannot seek GeoIP database");

			_geodata.Seek(6*offset, SeekOrigin.Begin);
			_geodata.Read(buffer, 0, 6);

			for (int i = 0; i < 2; i++)
			{
				x[i] = 0;
				for (int j = 0; j < 3; j++)
				{
					int y = buffer[i*3 + j];
					if (y < 0)
						y += 256;
					x[i] += (y << (j*8));
				}
			}

			if ((ipnum & (1 << depth)) > 0)
			{
				if (x[1] >= COUNTRY_BEGIN)
					return x[1] - COUNTRY_BEGIN;
				return FindCountryCode(x[1], ipnum, depth - 1);
			}
			else
			{
				if (x[0] >= COUNTRY_BEGIN)
					return x[0] - COUNTRY_BEGIN;
				return FindCountryCode(x[0], ipnum, depth - 1);
			}
		}

		public void Dispose()
		{
			if (_close && _geodata != null)
			{
				_geodata.Close();
				_geodata = null;
			}
		}
	}
}