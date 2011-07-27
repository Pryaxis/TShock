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
using System.Text;
using TerrariaAPI;
using XNAHelpers;

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
    }
    public class WorldInfoMsg : IPackable
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
        public void PackFull(Stream stream)
        {
            long start = stream.Position;
            stream.WriteInt32(1);
            stream.WriteInt8((byte)PacketTypes.WorldInfo);
            Pack(stream);
            long end = stream.Position;
            stream.Position = start;
            stream.WriteInt32((int)(end - start) - 4);
            stream.Position = end;
        }
        public void Pack(Stream stream)
        {
            stream.Write(Time);
            stream.Write(DayTime);
            stream.Write(MoonPhase);
            stream.Write(BloodMoon);
            stream.Write(MaxTilesX);
            stream.Write(MaxTilesY);
            stream.Write(SpawnX);
            stream.Write(SpawnY);
            stream.Write(WorldSurface);
            stream.Write(RockLayer);
            stream.Write(WorldID);
            stream.Write((byte)WorldFlags);
            stream.Write(Encoding.ASCII.GetBytes(WorldName));
        }

        public void Unpack(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
