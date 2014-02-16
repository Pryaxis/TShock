/*
TShock, a server mod for Terraria
Copyright (C) 2011-2013 Nyx Studios (fka. The TShock Team)

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
	public enum BossFlags : byte
	{
		None = 0,
		OrbSmashed = 1,
		DownedBoss1 = 2,
		DownedBoss2 = 4,
		DownedBoss3 = 8,
		HardMode = 16,
		DownedClown = 32,
		ServerSideCharacter = 64,
		DownedPlantBoss = 128
	}

	[Flags]
	public enum BossFlags2 : byte
	{
		None = 0,
		DownedMechBoss1 = 1,
		DownedMechBoss2 = 2,
		DownedMechBoss3 = 4,
		DownedMechBossAny = 8,
		CloudBg = 16,
		Crimson = 32,
		PumpkinMoon = 64,
		SnowMoon = 128
	}

	public class WorldInfoMsg : BaseMsg
	{
		public int Time { get; set; }
		public bool DayTime { get; set; }
		public byte MoonPhase { get; set; }
		public bool BloodMoon { get; set; }
		public bool Eclipse { get; set; }
		public int MaxTilesX { get; set; }
		public int MaxTilesY { get; set; }
		public int SpawnX { get; set; }
		public int SpawnY { get; set; }
		public int WorldSurface { get; set; }
		public int RockLayer { get; set; }
		public int WorldID { get; set; }
		public byte MoonType { get; set; }
		public int TreeX0 { get; set; }
		public int TreeX1 { get; set; }
		public int TreeX2 { get; set; }
		public byte TreeStyle0 { get; set; }
		public byte TreeStyle1 { get; set; }
		public byte TreeStyle2 { get; set; }
		public byte TreeStyle3 { get; set; }
		public int CaveBackX0 { get; set; }
		public int CaveBackX1 { get; set; }
		public int CaveBackX2 { get; set; }
		public byte CaveBackStyle0 { get; set; }
		public byte CaveBackStyle1 { get; set; }
		public byte CaveBackStyle2 { get; set; }
		public byte CaveBackStyle3 { get; set; }
		public byte SetBG0 { get; set; }
		public byte SetBG1 { get; set; }
		public byte SetBG2 { get; set; }
		public byte SetBG3 { get; set; }
		public byte SetBG4 { get; set; }
		public byte SetBG5 { get; set; }
		public byte SetBG6 { get; set; }
		public byte SetBG7 { get; set; }
		public byte IceBackStyle { get; set; }
		public byte JungleBackStyle { get; set; }
		public byte HellBackStyle { get; set; }
		public float WindSpeed { get; set; }
		public byte NumberOfClouds { get; set; }
		public BossFlags BossFlags { get; set; }
		public BossFlags2 BossFlags2 { get; set; }
		public float Rain { get; set; }
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
			stream.WriteBoolean(Eclipse);
			stream.WriteInt32(MaxTilesX);
			stream.WriteInt32(MaxTilesY);
			stream.WriteInt32(SpawnX);
			stream.WriteInt32(SpawnY);
			stream.WriteInt32(WorldSurface);
			stream.WriteInt32(RockLayer);
			stream.WriteInt32(WorldID);
			stream.WriteByte(MoonType);
			stream.WriteInt32(TreeX0);
			stream.WriteInt32(TreeX1);
			stream.WriteInt32(TreeX2);
			stream.WriteByte(TreeStyle0);
			stream.WriteByte(TreeStyle1);
			stream.WriteByte(TreeStyle2);
			stream.WriteByte(TreeStyle3);
			stream.WriteInt32(CaveBackX0);
			stream.WriteInt32(CaveBackX1);
			stream.WriteInt32(CaveBackX2);
			stream.WriteByte(CaveBackStyle0);
			stream.WriteByte(CaveBackStyle1);
			stream.WriteByte(CaveBackStyle2);
			stream.WriteByte(CaveBackStyle3);
			stream.WriteByte(SetBG0);
			stream.WriteByte(SetBG1);
			stream.WriteByte(SetBG2);
			stream.WriteByte(SetBG3);
			stream.WriteByte(SetBG4);
			stream.WriteByte(SetBG5);
			stream.WriteByte(SetBG6);
			stream.WriteByte(SetBG7);
			stream.WriteByte(IceBackStyle);
			stream.WriteByte(JungleBackStyle);
			stream.WriteByte(HellBackStyle);
			stream.WriteSingle(WindSpeed);
			stream.WriteByte(NumberOfClouds);
			stream.WriteInt8((byte)BossFlags);
			stream.WriteInt8((byte)BossFlags2);
			stream.WriteSingle(Rain);
			stream.WriteBytes(Encoding.UTF8.GetBytes(WorldName));
		}
	}
}