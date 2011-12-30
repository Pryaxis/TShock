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
            stream.WriteInt32(1);
            stream.WriteInt8((byte)ID);
            Pack(stream);
            long end = stream.Position;
            stream.Position = start;
            stream.WriteInt32((int)(end - start) - 4);
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
