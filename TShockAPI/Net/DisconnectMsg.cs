using System.IO;
using System.IO.Streams;
using System.Text;

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
