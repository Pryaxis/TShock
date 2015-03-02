/*
TShock, a server mod for Terraria
Copyright (C) 2011-2015 Nyx Studios (fka. The TShock Team)

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

using TShockAPI.DB;

namespace TShockAPI.Hooks
{
	public class RegionHooks
	{
		internal class RegionEnteredEventArgs
		{
			public TSPlayer Player { get; private set; }

			public RegionEnteredEventArgs(TSPlayer ply)
			{
				Player = ply;
			}
		}

		public delegate void RegionEnteredD(RegionEnteredEventArgs args);
		public static event RegionEnteredD RegionEntered;
		public static void OnRegionEntered(TSPlayer player)
		{
			if (RegionEntered == null)
			{
				return;
			}

			RegionEntered(new RegionEnteredEventArgs(player));
		}

		internal class RegionLeftEventArgs
		{
			public TSPlayer Player { get; private set; }

			public RegionLeftEventArgs(TSPlayer ply)
			{
				Player = ply;
			}
		}

		public delegate void RegionLeftD(RegionLeftEventArgs args);
		public static event RegionLeftD RegionLeft;
		public static void OnRegionLeft(TSPlayer player)
		{
			if (RegionLeft == null)
			{
				return;
			}

			RegionLeft(new RegionLeftEventArgs(player));
		}

		internal class RegionCreatedEventArgs
		{
			public Region Region { get; private set; }

			public RegionCreatedEventArgs(Region region)
			{
				Region = region;
			}
		}

		public delegate void RegionCreatedD(RegionCreatedEventArgs args);
		public static event RegionCreatedD RegionCreated;
		public static void OnRegionCreated(Region region)
		{
			if (RegionCreated == null)
				return;

			RegionCreated(new RegionCreatedEventArgs(region));
		}

		internal class RegionDeletedEventArgs
		{
			public Region Region { get; private set; }

			public RegionDeletedEventArgs(Region region)
			{
				Region = region;
			}
		}

		public delegate void RegionDeletedD(RegionDeletedEventArgs args);
		public static event RegionDeletedD RegionDeleted;
		public static void OnRegionDeleted(Region region)
		{
			if (RegionDeleted == null)
				return;

			RegionDeleted(new RegionDeletedEventArgs(region));
		}
	}
}
