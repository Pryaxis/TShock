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

namespace TShockAPI.Net
{
	public class BaseMsg : IPackable
	{
		public virtual PacketTypes ID
		{
			get { throw new NotImplementedException("Msg ID not implemented"); }
		}

		public void PackFull(Stream stream)
		{
			long start = stream.Position;
			stream.WriteInt16(0);
			stream.WriteInt8((byte) ID);
			Pack(stream);
			long end = stream.Position;
			stream.Position = start;
			stream.WriteInt16((short)end);
			stream.Position = end;
		}

		public virtual void Unpack(Stream stream)
		{
			throw new NotImplementedException();
		}

		public virtual void Pack(Stream stream)
		{
			throw new NotImplementedException();
		}
	}
}