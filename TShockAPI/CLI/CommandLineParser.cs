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

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace TShockAPI.CLI
{
	/// <summary>
	/// A simple command-line parser for retrieving basic information from a command-line. Array types are not supported
	/// </summary>
	public class CommandLineParser
	{
		private List<FlagSet> _flags = new List<FlagSet>();
		private Dictionary<FlagSet, object> _results = new Dictionary<FlagSet, object>();
		private string[] _source;

		/// <summary>
		/// Resets the CommandLineParser, removing any results and flags, and clearing the source
		/// </summary>
		/// <returns></returns>
		public CommandLineParser Reset()
		{
			_flags.Clear();
			_results.Clear();
			_source = null;

			return this;
		}

		/// <summary>
		/// Adds a flag to be parsed
		/// </summary>
		/// <param name="flag">The flag to be added</param>
		/// <param name="noArgs">Whether or not the flag is followed by an argument</param>
		public CommandLineParser AddFlag(string flag, bool noArgs = false)
		{
			FlagSet flags = new FlagSet(flag) { NoArgs = noArgs };
			return AddFlags(flags);
		}

		/// <summary>
		/// Adds a flag to be parsed, with the given callback being invoked with the flag's argument when it is found.
		/// The callback's parameter is the argument passed to the flag
		/// </summary>
		/// <param name="flag"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		public CommandLineParser AddFlag(string flag, Action<string> callback)
		{
			FlagSet flags = new FlagSet(flag) { callback = callback };
			return AddFlags(flags);
		}

		/// <summary>
		/// Adds a flag to be parsed, with the given callback being invoked when the flag is found.
		/// This method assumes the flag has no arguments
		/// </summary>
		/// <param name="flag"></param>
		/// <param name="callback"></param>
		/// <returns></returns>
		public CommandLineParser AddFlag(string flag, Action callback)
		{
			FlagSet flags = new FlagSet(flag) { NoArgs = true, callback = callback };
			return AddFlags(flags);
		}

		/// <summary>
		/// Adds a range of flags to be parsed
		/// </summary>
		/// <param name="flags">The FlagSet to be added</param>
		/// <returns></returns>
		public CommandLineParser AddFlags(FlagSet flags)
		{
			if (_flags.Contains(flags))
			{
				return this;
			}

			_flags.Add(flags);

			return this;
		}

		/// <summary>
		/// Adds a range of flags to be parsed, with the given callback being invoked with the flag's argument when it is found.
		/// The callback's parameter is the argument passed to the flag
		/// </summary>
		/// <param name="flags">The FlagSet to be added</param>
		/// <param name="callback">An Action with a single string parameter. This parameter is the value passed to the flag</param>
		/// <returns></returns>
		public CommandLineParser AddFlags(FlagSet flags, Action<string> callback)
		{
			flags.callback = callback;
			return AddFlags(flags);
		}

		/// <summary>
		/// Adds a range of flags to be parsed, with the given callback being invoked when the flag's argument is found.
		/// This method assumes the flag has no arguments
		/// </summary>
		/// <param name="flags">The FlagSet to be added</param>
		/// <param name="callback">An Action with no parameters.</param>
		/// <returns></returns>
		public CommandLineParser AddFlags(FlagSet flags, Action callback)
		{
			flags.callback = callback;
			flags.NoArgs = true;
			return AddFlags(flags);
		}

		/// <summary>
		/// Adds a callback after a flag's parsing has been completed.
		/// This method automatically attaches the callback to the last added flag
		/// </summary>
		/// <param name="callback">An Action with no parameters.</param>
		/// <returns></returns>
		public CommandLineParser After(Action callback)
		{
			FlagSet flags = _flags.Last();
			flags.continuation = callback;
			return this;
		}

		/// <summary>
		/// Gets the result of a FlagSet, cast to the given type parameter. Array types are not supported
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="flags"></param>
		/// <returns></returns>
		public T Get<T>(FlagSet flags)
		{
			if (!_results.ContainsKey(flags))
			{
				return default(T);
			}

			object result = _results[flags];
			Type t = typeof(T);

			if (t == typeof(string))
			{
				if (result == null)
				{
					return (T)(object)string.Empty;
				}

				return (T)result;
			}

			if (t.IsValueType)
			{
				TypeConverter tc = TypeDescriptor.GetConverter(t);
				return (T)tc.ConvertFromString(result.ToString());
			}

			return (T)Activator.CreateInstance(t, result);
		}

		/// <summary>
		/// Parses the given source for flags registered with the parser
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public CommandLineParser ParseFromSource(string[] source)
		{
			_source = source;

			for (int i = 0; i < (source.Length - 1 == 0 ? 1 : source.Length); i++)
			{
				string flag = source[i].ToLowerInvariant();
				string argument = null;

				if (string.IsNullOrWhiteSpace(flag))
				{
					continue;
				}

				if (i + 1 < source.Length)
				{
					argument = source[i + 1];
				}

				FlagSet flags = _flags.FirstOrDefault(f => f.Contains(flag));
				if (flags == null)
				{
					continue;
				}

				if (flags.NoArgs)
				{
					if (flags.callback != null)
					{
						((Action)flags.callback).Invoke();
					}
					else
					{
						_results.Add(flags, true);
					}
				}
				else
				{
					if (flags.callback != null)
					{
						((Action<string>)flags.callback).Invoke(argument);
					}
					else
					{
						_results.Add(flags, argument);
					}
				}
				flags.continuation?.Invoke();
			}

			return this;
		}

		/// <summary>
		/// Gets the result of a flag, cast to the given type parameter. Array types are not supported
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="flag"></param>
		/// <returns></returns>
		public T Get<T>(string flag)
		{
			FlagSet flags = _flags.FirstOrDefault(f => f.Contains(flag));
			if (flags == null)
			{
				return default(T);
			}

			return Get<T>(flags);
		}

		/// <summary>
		/// Attempts to get the result of a flag, cast to the given type parameter. Array types are not supported
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="flag"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool TryGet<T>(string flag, out T value)
		{
			FlagSet flags = _flags.FirstOrDefault(f => f.Contains(flag));
			if (flags == null)
			{
				value = default(T);
				return false;
			}

			return TryGet(flags, out value);
		}

		/// <summary>
		/// Attempts to get the result of a FlagSet, cast to the given type parameter. Array types are not supported
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="flags"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public bool TryGet<T>(FlagSet flags, out T value)
		{
			object result = _results[flags];

			if (result == null)
			{
				//Null result shouldn't happen, but return false if it does
				value = default(T);
				return false; 
			}

			Type t = typeof(T);

			//Strings get special handling because the result object is a string
			if (t == typeof(string))
			{
				if (result == null)
				{
					//Null strings shouldn't happen, but return false if it does
					value = default(T);
					return false;
				}

				value = (T)result;
				return true;
			}

			//Value types get converted with a TypeConverter
			if (t.IsValueType)
			{
				try
				{
					TypeConverter tc = TypeDescriptor.GetConverter(t);
					value = (T)tc.ConvertFrom(result);
					return true;
				}
				catch
				{
					value = default(T);
					return false;
				}
			}

			try
			{
				//Reference types get created with an Activator
				value = (T)Activator.CreateInstance(t, result);
				return true;
			}
			catch
			{
				value = default(T);
				return false;
			}
		}
	}
}
