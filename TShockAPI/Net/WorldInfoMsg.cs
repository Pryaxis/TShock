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
using System.IO;
using System.IO.Streams;
using System.Text;
using Terraria;

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

	[Flags]
	public enum BossFlags3 : byte
	{
		None = 0,
		ExpertMode = 1,
		FastForwardTime = 2,
		SlimeRain = 4,
		DownedKingSlime = 8,
		DownedQueenBee = 16,
		DownedFishron = 32,
		DownedMartians = 64,
		DownedAncientCultist = 128
	}

	[Flags]
	public enum BossFlags4 : byte
	{
		None = 0,
		DownedMoonLord = 1,
		DownedHalloweenKing = 2,
		DownedHalloweenTree = 4,
		DownedChristmasIceQueen = 8,
		DownedChristmasSantank = 16,
		DownedChristmasTree = 32
	}

	public class WorldInfoMsg : BaseMsg
	{
		public int Time { get; set; }
		public bool DayTime { get; set; }
		public byte MoonPhase { get; set; }
		public bool BloodMoon { get; set; }
		public bool Eclipse { get; set; }
		public short MaxTilesX { get; set; }
		public short MaxTilesY { get; set; }
		public short SpawnX { get; set; }
		public short SpawnY { get; set; }
		public short WorldSurface { get; set; }
		public short RockLayer { get; set; }
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
		public BossFlags3 BossFlags3 { get; set; }
		public BossFlags4 BossFlags4 { get; set; }
		public float Rain { get; set; }
		public string WorldName { get; set; }

		public override PacketTypes ID
		{
			get { return PacketTypes.WorldInfo; }
		}

		public override void Pack(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream);
			writer.Write(Time);
			BitsByte worldinfo = new BitsByte(DayTime, BloodMoon, Eclipse);
			writer.Write(worldinfo);
			writer.Write(MoonPhase);
			writer.Write(MaxTilesX);
			writer.Write(MaxTilesY);
			writer.Write(SpawnX);
			writer.Write(SpawnY);
			writer.Write(WorldSurface);
			writer.Write(RockLayer);
			writer.Write(WorldID);
			writer.Write(WorldName);
			writer.Write(MoonType);

			writer.Write(SetBG0);
			writer.Write(SetBG1);
			writer.Write(SetBG2);
			writer.Write(SetBG3);
			writer.Write(SetBG4);
			writer.Write(SetBG5);
			writer.Write(SetBG6);
			writer.Write(SetBG7);
			writer.Write(IceBackStyle);
			writer.Write(JungleBackStyle);
			writer.Write(HellBackStyle);
			writer.Write(WindSpeed);
			writer.Write(NumberOfClouds);

			writer.Write(TreeX0);
			writer.Write(TreeX1);
			writer.Write(TreeX2);
			writer.Write(TreeStyle0);
			writer.Write(TreeStyle1);
			writer.Write(TreeStyle2);
			writer.Write(TreeStyle3);
			writer.Write(CaveBackX0);
			writer.Write(CaveBackX1);
			writer.Write(CaveBackX2);
			writer.Write(CaveBackStyle0);
			writer.Write(CaveBackStyle1);
			writer.Write(CaveBackStyle2);
			writer.Write(CaveBackStyle3);

			writer.Write(Rain);

			BitsByte bosses1 = new BitsByte((BossFlags & BossFlags.OrbSmashed) == BossFlags.OrbSmashed,
				(BossFlags & BossFlags.DownedBoss1) == BossFlags.DownedBoss1,
				(BossFlags & BossFlags.DownedBoss2) == BossFlags.DownedBoss2,
				(BossFlags & BossFlags.DownedBoss3) == BossFlags.DownedBoss3,
				(BossFlags & BossFlags.HardMode) == BossFlags.HardMode,
				(BossFlags & BossFlags.DownedClown) == BossFlags.DownedClown,
				(BossFlags & BossFlags.ServerSideCharacter) == BossFlags.ServerSideCharacter,
				(BossFlags & BossFlags.DownedPlantBoss) == BossFlags.DownedPlantBoss);
			writer.Write(bosses1);

			BitsByte bosses2 = new BitsByte((BossFlags2 & BossFlags2.DownedMechBoss1) == BossFlags2.DownedMechBoss1,
				(BossFlags2 & BossFlags2.DownedMechBoss2) == BossFlags2.DownedMechBoss2,
				(BossFlags2 & BossFlags2.DownedMechBoss3) == BossFlags2.DownedMechBoss3,
				(BossFlags2 & BossFlags2.DownedMechBossAny) == BossFlags2.DownedMechBossAny,
				(BossFlags2 & BossFlags2.CloudBg) == BossFlags2.CloudBg,
				(BossFlags2 & BossFlags2.Crimson) == BossFlags2.Crimson,
				(BossFlags2 & BossFlags2.PumpkinMoon) == BossFlags2.PumpkinMoon,
				(BossFlags2 & BossFlags2.SnowMoon) == BossFlags2.SnowMoon);
			writer.Write(bosses2);

			BitsByte bosses3 = new BitsByte((BossFlags3 & BossFlags3.ExpertMode) == BossFlags3.ExpertMode,
				(BossFlags3 & BossFlags3.FastForwardTime) == BossFlags3.FastForwardTime,
				(BossFlags3 & BossFlags3.SlimeRain) == BossFlags3.SlimeRain,
				(BossFlags3 & BossFlags3.DownedKingSlime) == BossFlags3.DownedKingSlime,
				(BossFlags3 & BossFlags3.DownedQueenBee) == BossFlags3.DownedQueenBee,
				(BossFlags3 & BossFlags3.DownedFishron) == BossFlags3.DownedFishron,
				(BossFlags3 & BossFlags3.DownedMartians) == BossFlags3.DownedMartians,
				(BossFlags3 & BossFlags3.DownedAncientCultist) == BossFlags3.DownedAncientCultist);
			writer.Write(bosses3);

			BitsByte bosses4 = new BitsByte((BossFlags4 & BossFlags4.DownedMoonLord) == BossFlags4.DownedMoonLord,
				(BossFlags4 & BossFlags4.DownedHalloweenKing) == BossFlags4.DownedHalloweenKing,
				(BossFlags4 & BossFlags4.DownedHalloweenTree) == BossFlags4.DownedHalloweenTree,
				(BossFlags4 & BossFlags4.DownedChristmasIceQueen) == BossFlags4.DownedChristmasIceQueen,
				(BossFlags4 & BossFlags4.DownedChristmasSantank) == BossFlags4.DownedChristmasSantank,
				(BossFlags4 & BossFlags4.DownedChristmasTree) == BossFlags4.DownedChristmasTree);
			writer.Write(bosses4);

			writer.Write((sbyte)Main.invasionType);
			writer.Write(Main.LobbyId);
		}
	}
}