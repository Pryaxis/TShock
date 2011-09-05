using System.Collections.Generic;

namespace Rests
{
    public class RestVerbs : Dictionary<string, string>
    {
        /// <summary>
        /// Gets value safely, if it does not exist, return null. Sets/Adds value safely, if null it will remove.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>Returns null if key does not exist.</returns>
        public new string this[string key]
        {
            get
            {
                string ret;
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