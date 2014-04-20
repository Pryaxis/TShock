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
using System.Collections.Generic;

namespace TShockAPI
{
    public class PacketBuffer : List<byte>
    {
        public byte[] GetBytes(int max)
        {
            lock (this)
            {
                if (Count < 1)
                    return null;

                var ret = new byte[Math.Min(max, Count)];
                CopyTo(0, ret, 0, ret.Length);
                return ret;
            }
        }

        public void Pop(int count)
        {
            lock (this)
            {
                RemoveRange(0, count<Count?count:Count);
            }
        }
    }
}
