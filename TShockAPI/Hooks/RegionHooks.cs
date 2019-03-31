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

using TShockAPI.DB;

namespace TShockAPI.Hooks
{
	public class RegionHooks
	{
		public class RegionEnteredEventArgs
		{
			public TSPlayer Player { get; private set; }

			public Region Region { get; private set; }

			public RegionEnteredEventArgs(TSPlayer ply, Region region)
			{
				Player = ply;
				Region = region;
			}
		}

		public delegate void RegionEnteredD(RegionEnteredEventArgs args);
		public static event RegionEnteredD RegionEntered;
		public static void OnRegionEntered(TSPlayer player, Region region)
		{
			if (RegionEntered == null)
			{
				return;
			}

			RegionEntered(new RegionEnteredEventArgs(player, region));
		}

		public class RegionLeftEventArgs
		{
			public TSPlayer Player { get; private set; }
			public Region Region { get; private set; }

			public RegionLeftEventArgs(TSPlayer ply, Region region)
			{
				Player = ply;
				Region = region;
			}
		}

		public delegate void RegionLeftD(RegionLeftEventArgs args);
		public static event RegionLeftD RegionLeft;
		public static void OnRegionLeft(TSPlayer player, Region region)
		{
			if (RegionLeft == null)
			{
				return;
			}

			RegionLeft(new RegionLeftEventArgs(player, region));
		}

		public class RegionCreatedEventArgs
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

		public class RegionDeletedEventArgs
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
		
		public class RegionRenamedEventArgs
		{
			public Region Region { get; private set; }
			public string OldName { get; private set; }
			public string NewName { get; private set; }

			public RegionRenamedEventArgs(Region region, string oldName, string newName)
			{
				Region = region;
				OldName = oldName;
				NewName = newName;
			}
		}

		public delegate void RegionRenamedD(RegionRenamedEventArgs args);
		public static event RegionRenamedD RegionRenamed;
		public static void OnRegionRenamed(Region region, string oldName, string newName)
		{
			if (RegionRenamed == null)
				return;

			RegionRenamed(new RegionRenamedEventArgs(region, oldName, newName));
		}
	}
}
