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
using Terraria;

namespace TShockAPI.Net
{
	public class NetTile : IPackable
	{
		public bool Active { get; set; }
		public byte Type { get; set; }
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
			var flags = TileFlags.None;

			if ((Active) && (!Inactive))
				flags |= TileFlags.Active;

			if (Lighted)
				flags |= TileFlags.Lighted;

			if (HasWall)
				flags |= TileFlags.Wall;

			if (HasLiquid)
				flags |= TileFlags.Liquid;

			if (Wire)
				flags |= TileFlags.Wire;
			
			if (IsHalf)
				flags |= TileFlags.HalfBrick;

			if (IsActuator)
				flags |= TileFlags.Actuator;

			if (Inactive)
			{
				flags |= TileFlags.Inactive;
			}

			stream.WriteInt8((byte) flags);

			var flags2 = TileFlags2.None;

			if ((Wire2))
				flags2 |= TileFlags2.Wire2;

			if (Wire3)
				flags2 |= TileFlags2.Wire3;

			if (HasColor)
				flags2 |= TileFlags2.Color;

			if (HasWallColor)
				flags2 |= TileFlags2.WallColor;

			if (Slope)
				flags2 |= TileFlags2.Slope;

			if (Slope2)
				flags2 |= TileFlags2.Slope2;


			stream.WriteInt8((byte)flags2);

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
				stream.WriteInt8(Type);
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
			var flags = (TileFlags) stream.ReadInt8();
			var flags2 = (TileFlags2)stream.ReadInt8();

			Wire2 = flags2.HasFlag(TileFlags2.Wire2);
			Wire3 = flags2.HasFlag(TileFlags2.Wire3);
			Slope = flags2.HasFlag(TileFlags2.Slope);
			Slope2 = flags2.HasFlag(TileFlags2.Slope2);

			if (flags2.HasFlag(TileFlags2.Color))
			{
				TileColor = stream.ReadInt8();
			}

			if (flags2.HasFlag(TileFlags2.WallColor))
			{
				WallColor = stream.ReadInt8();
			}

			Active = flags.HasFlag(TileFlags.Active);
			if (Active)
			{
				Type = stream.ReadInt8();
				if (FrameImportant)
				{
					FrameX = stream.ReadInt16();
					FrameY = stream.ReadInt16();
				}
			}

			if (flags.HasFlag(TileFlags.Lighted))
			{
				Lighted = true;
			}

			if (flags.HasFlag(TileFlags.Wall))
			{
				Wall = stream.ReadInt8();
			}

			if (flags.HasFlag(TileFlags.Liquid))
			{
				Liquid = stream.ReadInt8();
				LiquidType = stream.ReadInt8();
			}

			if (flags.HasFlag(TileFlags.Wire))
				Wire = true;

			if (flags.HasFlag(TileFlags.HalfBrick))
				IsHalf = true;
			
			if (flags.HasFlag(TileFlags.Actuator))
				IsActuator = true;

			if (flags.HasFlag(TileFlags.Inactive))
			{
				Inactive = true;
				Active = false;
			}
		}
	}

	[Flags]
	public enum TileFlags : byte
	{
		None = 0,
		Active = 1,
		Lighted = 2,
		Wall = 4,
		Liquid = 8,
		Wire = 16,
		HalfBrick = 32,
		Actuator = 64,
		Inactive = 128
	}

	[Flags]
	public enum TileFlags2 : byte
	{
		None = 0,
		Wire2 = 1,
		Wire3 = 2,
		Color = 4,
		WallColor = 8,
		Slope = 16,
		Slope2 = 32
	}
}
