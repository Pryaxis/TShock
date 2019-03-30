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
using Terraria;

namespace TShockAPI.Net
{
	public class NetTile : IPackable
	{
		public bool Active { get; set; }
		public ushort Type { get; set; }
		public short FrameX { get; set; }
		public short FrameY { get; set; }
		public bool Lighted { get; set; }
		public byte Wall { get; set; }
		public byte Liquid { get; set; }
		public byte LiquidType { get; set; }
		public bool Wire { get; set; }
		public bool Wire2 { get; set; }
		public bool Wire3 { get; set; }
		public byte HalfBrick { get; set; }
		public byte Actuator { get; set; }
		public bool Inactive { get; set; }
		public bool IsHalf { get; set; }
		public bool IsActuator { get; set; }
		public byte TileColor { get; set; }
		public byte WallColor { get; set; }
		public bool Slope { get; set; }
		public bool Slope2 { get; set; }
		public bool Slope3 { get; set; }

	public bool HasColor
		{
			get { return TileColor > 0; }
		}

		public bool HasWallColor
		{
			get { return WallColor > 0; }
		}

		public bool HasWall
		{
			get { return Wall > 0; }
		}

		public bool HasLiquid
		{
			get { return Liquid > 0; }
		}

		public bool FrameImportant
		{
			get { return Main.tileFrameImportant[Type]; }
		}

		public NetTile()
		{
			Active = false;
			Type = 0;
			FrameX = -1;
			FrameY = -1;
			Wall = 0;
			Liquid = 0;
			Wire = false;
			Wire2 = false;
			Wire3 = false;
			HalfBrick = 0;
			Actuator = 0;
			Inactive = false;
			TileColor = 0;
			WallColor = 0;
			Lighted = false;
			Slope = false;
			Slope2 = false;
		}

		public NetTile(Stream stream)
			: this()
		{
			Unpack(stream);
		}

		public void Pack(Stream stream)
		{
			var bits = new BitsByte();

			if ((Active) && (!Inactive))
				bits[0] = true;

			if (HasWall)
				bits[2] = true;

			if (HasLiquid)
				bits[3] = true;

			if (Wire)
				bits[4] = true;
			
			if (IsHalf)
				bits[5] = true;

			if (IsActuator)
				bits[6] = true;

			if (Inactive)
			{
				bits[7] = true;
			}

			stream.WriteInt8((byte) bits);

			bits = new BitsByte();

			if ((Wire2))
				bits[0] = true;

			if (Wire3)
				bits[1] = true;

			if (HasColor)
				bits[2] = true;

			if (HasWallColor)
				bits[3] = true;

			if (Slope)
				bits[4] = true;

			if (Slope2)
				bits[5] = true;

			if (Slope3)
				bits[6] = true;


			stream.WriteInt8((byte)bits);

			if (HasColor)
			{
				stream.WriteByte(TileColor);
			}

			if (HasWallColor)
			{
				stream.WriteByte(WallColor);
			}

			if (Active)
			{
				stream.WriteInt16((short)Type);
				if (FrameImportant)
				{
					stream.WriteInt16(FrameX);
					stream.WriteInt16(FrameY);
				}
			}

			if (HasWall)
				stream.WriteInt8(Wall);

			if (HasLiquid)
			{
				stream.WriteInt8(Liquid);
				stream.WriteInt8(LiquidType);
			}
		}

		public void Unpack(Stream stream)
		{
			var flags = (BitsByte) stream.ReadInt8();
			var flags2 = (BitsByte)stream.ReadInt8();

			Wire2 = flags2[0];
			Wire3 = flags2[1];
			Slope = flags2[4];
			Slope2 = flags2[5];
			Slope3 = flags2[6];

			if (flags2[2])
			{
				TileColor = stream.ReadInt8();
			}

			if (flags2[3])
			{
				WallColor = stream.ReadInt8();
			}

			Active = flags[0];
			if (Active)
			{
				Type = stream.ReadUInt16();
				if (FrameImportant)
				{
					FrameX = stream.ReadInt16();
					FrameY = stream.ReadInt16();
				}
			}

			if (flags[2])
			{
				Wall = stream.ReadInt8();
			}

			if (flags[3])
			{
				Liquid = stream.ReadInt8();
				LiquidType = stream.ReadInt8();
			}

			if (flags[4])
				Wire = true;

			if (flags[5])
				IsHalf = true;
			
			if (flags[6])
				IsActuator = true;

			if (flags[7])
			{
				Inactive = true;
				Active = false;
			}
		}
	}
}
