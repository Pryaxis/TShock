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
using System.Linq;

namespace TShockAPI.CLI
{
	/// <summary>
	/// Describes a set of flags that are responsible for one CL argument
	/// </summary>
	public class FlagSet : IEquatable<FlagSet>
	{
		private IEnumerable<string> _flags;

		internal object callback;
		internal Action continuation;

		/// <summary>
		/// Whether or not the set of flags represented by this FlagSet is followed by an argument
		/// </summary>
		public bool NoArgs { get; set; }

		/// <summary>
		/// Creates a new <see cref="FlagSet"/> with the given flags
		/// </summary>
		/// <param name="flags">Flags represented by this FlagSet</param>
		public FlagSet(params string[] flags)
		{
			if (flags == null)
			{
				throw new ArgumentNullException(nameof(flags));
			}

			_flags = flags.Select(f => f.ToLowerInvariant());
		}

		/// <summary>
		/// Creates a new <see cref="FlagSet"/> with the given flags and arguments option
		/// </summary>
		/// <param name="flags">Flags represented by this FlagSet</param>
		/// <param name="noArgs">Whether or not the flags specified will be followed by an argument</param>
		public FlagSet(string[] flags, bool noArgs) : this(flags)
		{
			NoArgs = noArgs;
		}

		/// <summary>
		/// Determines whether or not this flag set contains the given flag
		/// </summary>
		/// <param name="flag"></param>
		/// <returns></returns>
		public bool Contains(string flag)
		{
			return _flags.Contains(flag);
		}

		/// <summary>
		/// Determines whether or not this flag set is equatable to another
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(FlagSet other)
		{
			if (other == null)
			{
				return false;
			}

			return other._flags == _flags;
		}
	}
}
