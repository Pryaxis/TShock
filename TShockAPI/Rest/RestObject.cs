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

using System;
using System.Collections.Generic;

namespace Rests
{
	[Serializable]
	public class RestObject : Dictionary<string, object>
	{
		public string Status
		{
			get { return this["status"] as string; }
			set { this["status"] = value; }
		}

		public string Error
		{
			get { return this["error"] as string; }
			set { this["error"] = value; }
		}

		public string Response
		{
			get { return this["response"] as string; }
			set { this["response"] = value; }
		}

		// Parameterless constructor for deseralisation required by JavaScriptSerializer.Deserialize in TShockRestTestPlugin
		// Note: The constructor with all defaults isn't good enough :(
		public RestObject()
		{
			Status = "200";
		}

		public RestObject(string status = "200")
		{
			Status = status;
		}

		/// <summary>
		/// Gets value safely, if it does not exist, return null. Sets/Adds value safely, if null it will remove.
		/// </summary>
		/// <param name="key">the key</param>
		/// <returns>Returns null if key does not exist.</returns>
		public new object this[string key]
		{
			get
			{
				object ret;
				if (TryGetValue(key, out ret))
					return ret;
				return null;
			}
			set
			{
				if (!ContainsKey(key))
				{
					if (value == null)
						return;
					Add(key, value);
				}
				else
				{
					if (value != null)
						base[key] = value;
					else
						Remove(key);
				}
			}
		}
	}
}