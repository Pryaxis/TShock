/*
TShock, a server mod for Terraria
Copyright (C) 2011 The TShock Team

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
using System.IO;
using System.IO.Streams;
using System.Text;

namespace TShockAPI.Net
{
	[Flags]
	public enum WorldInfoFlag : byte
	{
		None = 0,
		OrbSmashed = 1,
		DownedBoss1 = 2,
		DownedBoss2 = 4,
		DownedBoss3 = 8,
		HardMode = 16,
		DownedClown = 32
	}

	public class WorldInfoMsg : BaseMsg
	{
		public int Time { get; set; }
		public bool DayTime { get; set; }
		public byte MoonPhase { get; set; }
		public bool BloodMoon { get; set; }
		public int MaxTilesX { get; set; }
		public int MaxTilesY { get; set; }
		public int SpawnX { get; set; }
		public int SpawnY { get; set; }
		public int WorldSurface { get; set; }
		public int RockLayer { get; set; }
		public int WorldID { get; set; }
		public WorldInfoFlag WorldFlags { get; set; }
		public string WorldName { get; set; }

		public override PacketTypes ID
		{
			get { return PacketTypes.WorldInfo; }
		}

		public override void Pack(Stream stream)
		{
			stream.WriteInt32(Time);
			stream.WriteBoolean(DayTime);
			stream.WriteInt8(MoonPhase);
			stream.WriteBoolean(BloodMoon);
			stream.WriteInt32(MaxTilesX);
			stream.WriteInt32(MaxTilesY);
			stream.WriteInt32(SpawnX);
			stream.WriteInt32(SpawnY);
			stream.WriteInt32(WorldSurface);
			stream.WriteInt32(RockLayer);
			stream.WriteInt32(WorldID);
			stream.WriteInt8((byte) WorldFlags);
			stream.WriteBytes(Encoding.UTF8.GetBytes(WorldName));
		}
	}
}