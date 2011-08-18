using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TerrariaAPI;
using XNAHelpers;

namespace TShockAPI.Net
{
    class DisconnectMsg : BaseMsg
    {
        public override PacketTypes ID
        {
            get
            {
                return PacketTypes.Disconnect;
            }
        }
        public string Reason {get;set;}
        public override void Pack(Stream stream)
        {
            stream.WriteBytes(Encoding.ASCII.GetBytes(Reason));
        }
    }
}
