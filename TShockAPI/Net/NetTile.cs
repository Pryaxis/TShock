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
		public byte Wall { get; set; }
		public byte Liquid { get; set; }
		public bool Lava { get; set; }
		public bool Wire { get; set; }

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
			Lava = false;
			Wire = false;
		}

		public NetTile(Stream stream)
			: this()
		{
			Unpack(stream);
		}

		public void Pack(Stream stream)
		{
			var flags = TileFlags.None;

			if (Active)
				flags |= TileFlags.Active;

			if (HasWall)
				flags |= TileFlags.Wall;

			if (HasLiquid)
				flags |= TileFlags.Liquid;

			if (Wire)
				flags |= TileFlags.Wire;

			stream.WriteInt8((byte) flags);

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
				stream.WriteBoolean(Lava);
			}
		}

		public void Unpack(Stream stream)
		{
			var flags = (TileFlags) stream.ReadInt8();

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

			if (flags.HasFlag(TileFlags.Wall))
			{
				Wall = stream.ReadInt8();
			}

			if (flags.HasFlag(TileFlags.Liquid))
			{
				Liquid = stream.ReadInt8();
				Lava = stream.ReadBoolean();
			}

			if (flags.HasFlag(TileFlags.Wire))
				Wire = true;
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
		Wire = 16
	}
}