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

		public RestObject(string status)
		{
			Status = status;
		}

		/// <summary>
		/// Gets value safely, if it does not exist, return null. Sets/Adds value safely, if null it will remove.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
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