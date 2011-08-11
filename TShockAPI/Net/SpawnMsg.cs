using System.Collections.Generic;
using System.IO;
using System.Linq;
using TerrariaAPI;
using XNAHelpers;

namespace TShockAPI.Net
{
    public class SpawnMsg : BaseMsg
    {
        public override PacketTypes ID
        {
            get
            {
                return PacketTypes.PlayerSpawn;
            }
        }

        public int TileX { get; set; }
        public int TileY {get;set;}
        public byte PlayerIndex { get; set; }

        public override void Pack(Stream stream)
        {
            stream.WriteInt8(PlayerIndex);
            stream.WriteInt32(TileX);
            stream.WriteInt32(TileY);
        }
    }
}
