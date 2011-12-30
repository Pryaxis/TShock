using System.IO;
using System.IO.Streams;

namespace TShockAPI.Net
{
	public class ProjectileRemoveMsg : BaseMsg
	{
		public override PacketTypes ID
		{
			get { return PacketTypes.ProjectileNew; }
		}

		public short Index { get; set; }
		public byte Owner { get; set; }

		public override void Pack(Stream stream)
		{
			stream.WriteInt16(Index);
			stream.WriteSingle(-1);
			stream.WriteSingle(-1);
			stream.WriteSingle(0);
			stream.WriteSingle(0);
			stream.WriteSingle(0);
			stream.WriteInt16(0);
			stream.WriteByte(Owner);
			stream.WriteByte(0);
			stream.WriteSingle(0);
			stream.WriteSingle(0);
			stream.WriteSingle(0);
		}
	}
}