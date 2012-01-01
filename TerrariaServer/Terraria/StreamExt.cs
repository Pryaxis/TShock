using System;
using System.Text;
namespace System.IO.Streams
{
	public static class StreamExt
	{
		[ThreadStatic]
		private static byte[] _buffer;
		private static byte[] buffer
		{
			get
			{
				byte[] arg_16_0;
				if ((arg_16_0 = StreamExt._buffer) == null)
				{
					arg_16_0 = (StreamExt._buffer = new byte[16]);
				}
				return arg_16_0;
			}
		}
		public static void FillBuffer(this Stream stream, int numBytes)
		{
			if (numBytes < 0 || numBytes > StreamExt.buffer.Length)
			{
				throw new ArgumentOutOfRangeException("numBytes");
			}
			if (numBytes < 1)
			{
				return;
			}
			int num2;
			if (numBytes != 1)
			{
				int num = 0;
				while (true)
				{
					num2 = stream.Read(StreamExt.buffer, num, numBytes - num);
					if (num2 == 0)
					{
						break;
					}
					num += num2;
					if (num >= numBytes)
					{
						return;
					}
				}
				throw new EndOfStreamException("End of stream");
			}
			num2 = stream.ReadByte();
			if (num2 == -1)
			{
				throw new EndOfStreamException("End of stream");
			}
			StreamExt.buffer[0] = (byte)num2;
		}
		public static void WriteBoolean(this Stream s, bool value)
		{
			s.WriteInt8((byte)(value ? 1 : 0));
		}
		public static void WriteInt8(this Stream s, byte num)
		{
			s.WriteByte(num);
		}
		public static void WriteInt16(this Stream s, short value)
		{
			StreamExt.buffer[0] = (byte)value;
			StreamExt.buffer[1] = (byte)(value >> 8);
			s.Write(StreamExt.buffer, 0, 2);
		}
		public static void WriteInt32(this Stream s, int value)
		{
			StreamExt.buffer[0] = (byte)value;
			StreamExt.buffer[1] = (byte)(value >> 8);
			StreamExt.buffer[2] = (byte)(value >> 16);
			StreamExt.buffer[3] = (byte)(value >> 24);
			s.Write(StreamExt.buffer, 0, 4);
		}
		public static void WriteInt64(this Stream s, long value)
		{
			StreamExt.buffer[0] = (byte)value;
			StreamExt.buffer[1] = (byte)(value >> 8);
			StreamExt.buffer[2] = (byte)(value >> 16);
			StreamExt.buffer[3] = (byte)(value >> 24);
			StreamExt.buffer[4] = (byte)(value >> 32);
			StreamExt.buffer[5] = (byte)(value >> 40);
			StreamExt.buffer[6] = (byte)(value >> 48);
			StreamExt.buffer[7] = (byte)(value >> 56);
			s.Write(StreamExt.buffer, 0, 8);
		}
		public unsafe static void WriteDouble(this Stream s, double num)
		{
			long value = *(long*)(&num);
			s.WriteInt64(value);
		}
		public unsafe static void WriteSingle(this Stream s, float num)
		{
			int value = *(int*)(&num);
			s.WriteInt32(value);
		}
		public static void WriteBytesWithLength(this Stream s, byte[] bytes)
		{
			s.WriteInt32(bytes.Length);
			s.WriteBytes(bytes);
		}
		public static void WriteBytes(this Stream s, byte[] bytes, int len)
		{
			s.Write(bytes, 0, len);
		}
		public static void WriteBytes(this Stream s, byte[] bytes)
		{
			s.Write(bytes, 0, bytes.Length);
		}
		public static void WriteString(this Stream s, string str)
		{
			if (str == null)
			{
				str = string.Empty;
			}
			s.WriteEncodedInt(str.Length);
			if (str.Length > 0)
			{
				s.WriteBytes(Encoding.UTF8.GetBytes(str));
			}
		}
		public static void WriteEncodedInt(this Stream s, int value)
		{
			uint num;
			for (num = (uint)value; num >= 128u; num >>= 7)
			{
				s.WriteInt8((byte)(num | 128u));
			}
			s.WriteInt8((byte)num);
		}
		public static byte ReadInt8(this Stream s)
		{
			int num = s.ReadByte();
			if (num == -1)
			{
				throw new EndOfStreamException("End of stream");
			}
			return (byte)num;
		}
		public static bool ReadBoolean(this Stream s)
		{
			return s.ReadInt8() != 0;
		}
		public static short ReadInt16(this Stream s)
		{
			s.FillBuffer(2);
			return (short)((int)StreamExt.buffer[0] | (int)StreamExt.buffer[1] << 8);
		}
		public static ushort ReadUInt16(this Stream s)
		{
			return (ushort)s.ReadInt16();
		}
		public static int ReadInt32(this Stream s)
		{
			s.FillBuffer(4);
			return (int)StreamExt.buffer[0] | (int)StreamExt.buffer[1] << 8 | (int)StreamExt.buffer[2] << 16 | (int)StreamExt.buffer[3] << 24;
		}
		public static uint ReadUInt32(this Stream s)
		{
			return (uint)s.ReadInt32();
		}
		public static long ReadInt64(this Stream s)
		{
			s.FillBuffer(8);
			ulong num = (ulong)((int)StreamExt.buffer[0] | (int)StreamExt.buffer[1] << 8 | (int)StreamExt.buffer[2] << 16 | (int)StreamExt.buffer[3] << 24);
			ulong num2 = (ulong)((int)StreamExt.buffer[4] | (int)StreamExt.buffer[5] << 8 | (int)StreamExt.buffer[6] << 16 | (int)StreamExt.buffer[7] << 24);
			return (long)(num2 << 32 | num);
		}
		public static ulong ReadUInt64(this Stream s)
		{
			return (ulong)s.ReadInt64();
		}
		public unsafe static double ReadDouble(this Stream s)
		{
			ulong num = s.ReadUInt64();
			return *(double*)(&num);
		}
		public unsafe static float ReadSingle(this Stream s)
		{
			uint num = s.ReadUInt32();
			return *(float*)(&num);
		}
		public static byte[] ReadBytesWithLength(this Stream s)
		{
			int count = s.ReadInt32();
			return s.ReadBytes(count);
		}
		public static byte[] ReadBytes(this Stream s, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			byte[] array = new byte[count];
			int num = 0;
			do
			{
				int num2 = s.Read(array, num, count);
				if (num2 == 0)
				{
					break;
				}
				num += num2;
				count -= num2;
			}
			while (count > 0);
			if (num != array.Length)
			{
				byte[] array2 = new byte[num];
				Buffer.BlockCopy(array, 0, array2, 0, num);
				array = array2;
			}
			return array;
		}
		public static string ReadString(this Stream s)
		{
			int num = s.ReadEncodedInt();
			if (num > 0)
			{
				return Encoding.UTF8.GetString(s.ReadBytes(num));
			}
			return string.Empty;
		}
		public static int ReadEncodedInt(this Stream s)
		{
			int num = 0;
			int num2 = 0;
			while (num2 != 35)
			{
				byte b = s.ReadInt8();
				num |= (int)(b & 127) << num2;
				num2 += 7;
				if ((b & 128) == 0)
				{
					return num;
				}
			}
			throw new FormatException("Format_Bad7BitInt32");
		}
	}
}
