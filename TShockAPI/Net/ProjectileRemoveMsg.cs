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

using System.IO;
using System.IO.Streams;

namespace TShockAPI.Net
{
	public class ProjectileRemoveMsg : BaseMsg
	{
		public override PacketTypes ID
		{
			get{ return PacketTypes.ProjectileNew; }
		}

		public short Index { get; set; }
		public byte Owner { get; set; }
		/// <summary>Slot-reuse counter of the projectile being removed. A stale generation makes
		/// clients treat the key as a fresh (type 0) projectile, which still clears the slot.</summary>
		public int Generation { get; set; }

		public override void Pack(Stream stream)
		{
			// ProjectileKey: spawner:8 | index:10 | generation:14
			int key = (Owner & 255) | (Index & 1023) << 8 | (Generation & 16383) << 18;
			stream.WriteInt32(key);
			stream.WriteSingle(-1);
			stream.WriteSingle(-1);
			stream.WriteSingle(0);
			stream.WriteSingle(0);
			stream.WriteInt16(0);
			stream.WriteByte(0);
		}
	}
}