/*   
TShock, a server mod for Terraria
Copyright (C) <year>  <name of author>

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
using System.IO;
using System.Text;

namespace StreamBinary
{
    public static class StreamExt
    {
        public static void FillBuffer(this Stream stream, byte[] buf, int numBytes)
        {
            int read;
            if (numBytes == 0x1)
            {
                read = stream.ReadByte();
                if (read == -1)
                {
                    throw new EndOfStreamException("End of stream");
                }
                buf[0x0] = (byte)read;
            }
            else
            {
                int offset = 0x0;
                do
                {
                    read = stream.Read(buf, offset, numBytes - offset);
                    if (read == 0x0)
                    {
                        throw new EndOfStreamException("End of stream");
                    }
                    offset += read;
                }
                while (offset < numBytes);
            }

        }
        public static void WriteBoolean(this Stream s, bool num)
        {
            s.WriteByte((byte)(num ? 1 : 0));
        }
        public static void WriteInt8(this Stream s, byte num)
        {
            s.WriteByte(num);
        }
        public static void WriteInt16(this Stream s, Int16 num)
        {
            s.WriteInt8((byte)(num & 0xff));
            s.WriteInt8((byte)(num >> 8));
        }
        public static void WriteInt32(this Stream s, Int32 num)
        {
            s.WriteInt16((Int16)(num & 0xffff));
            s.WriteInt16((Int16)(num >> 16));
        }
        public static void WriteInt64(this Stream s, Int64 num)
        {
            s.WriteInt32((Int32)(num & 0xffffffff));
            s.WriteInt32((Int32)(num >> 32));
        }
        public static unsafe void WriteSingle(this Stream s, float num)
        {
            Int32 n1 = *((Int32*)&num);
            s.WriteInt32(n1);
        }
        public static unsafe void WriteDouble(this Stream s, double num)
        {
            Int64 n1 = *((Int64*)&num);
            s.WriteInt64(n1);
        }
        public static void WriteBytes(this Stream s, byte[] bytes)
        {
            s.Write(bytes, 0, bytes.Length);
        }
        public static void WriteBytesWithLength(this Stream s, byte[] bytes)
        {
            s.WriteInt32(bytes.Length);
            s.WriteBytes(bytes);
        }
        public static void WriteBytes(this Stream s, byte[] bytes, Int32 len)
        {
            s.Write(bytes, 0, len);
        }
        public static void WriteString(this Stream s, string str)
        {
            if (str == null)
                str = string.Empty;

            s.WriteEncodedInt((Int32)str.Length);
            if (str.Length > 0)
                s.WriteBytes(Encoding.UTF8.GetBytes(str));
        }
        public static void WriteEncodedInt(this Stream s, int value)
        {
            uint num = (uint)value;
            while (num >= 0x80)
            {
                s.WriteInt8((byte)(num | 0x80));
                num = num >> 0x7;
            }
            s.WriteInt8((byte)num);
        }

        public static byte ReadInt8(this Stream s)
        {
            int read = s.ReadByte();
            if (read == -1)
            {
                throw new EndOfStreamException("End of stream");
            }
            return (byte)read;
        }
        public static bool ReadBoolean(this Stream s)
        {
            return s.ReadInt8() != 0;
        }

        public static Int16 ReadInt16(this Stream s)
        {
            byte n1 = s.ReadInt8();
            byte n2 = s.ReadInt8();
            return (Int16)(n1 | (n2 << 8));
        }
        public static UInt16 ReadUInt16(this Stream s)
        {
            byte n1 = s.ReadInt8();
            byte n2 = s.ReadInt8();
            return (UInt16)(n1 | (n2 << 8));
        }

        public static Int32 ReadInt32(this Stream s)
        {
            Int16 n1 = s.ReadInt16();
            Int16 n2 = s.ReadInt16();
            return (Int32)(n1 | (n2 << 16));
        }
        public static UInt32 ReadUInt32(this Stream s)
        {
            UInt16 n1 = s.ReadUInt16();
            UInt16 n2 = s.ReadUInt16();
            return (UInt32)(n1 | (n2 << 16));
        }

        public static Int64 ReadInt64(this Stream s)
        {
            Int64 n1 = s.ReadInt32();
            Int64 n2 = s.ReadInt32();
            return (Int64)(n1 | (n2 << 32));
        }
        public static UInt64 ReadUInt64(this Stream s)
        {
            UInt64 n1 = s.ReadUInt32();
            UInt64 n2 = s.ReadUInt32();
            return (n1 | (n2 << 32));
        }
        public static unsafe float ReadSingle(this Stream s)
        {
            var ret = (UInt32)s.ReadUInt32();
            return *((float*)&ret);
        }
        public static unsafe double ReadDouble(this Stream s)
        {
            var ret = (UInt64)s.ReadUInt64();
            return *((double*)&ret);
        }

        public static byte[] ReadBytes(this Stream s)
        {
            Int32 len = s.ReadInt32();
            return s.ReadBytes(len);
        }
        public static byte[] ReadBytes(this Stream s, Int32 len)
        {
            byte[] ret = new byte[len];
            s.FillBuffer(ret, len);
            return ret;
        }
        public static string ReadString(this Stream s)
        {
            int len = s.ReadEncodedInt();
            if (len > 0)
                return Encoding.UTF8.GetString(s.ReadBytes(len));
            return string.Empty;
        }
        public static int ReadEncodedInt(this Stream s)
        {
            byte num3;
            int num = 0x0;
            int num2 = 0x0;
            do
            {
                if (num2 == 0x23)
                {
                    throw new FormatException("Format_Bad7BitInt32");
                }
                num3 = s.ReadInt8();
                num |= (num3 & 0x7f) << num2;
                num2 += 0x7;
            }
            while ((num3 & 0x80) != 0x0);
            return num;
        }
    }
    public static class MemoryStreamExt
    {
        public static void Reset(this MemoryStream ms)
        {
            ms.Position = 0;
        }
    }
}

namespace StreamBinary.Generic
{
    public static class StreamGenericExt
    {
        static Dictionary<Type, Action<Stream, object>> WriteFuncs = new Dictionary<Type, Action<Stream, object>>()
        {
            {typeof(bool), (s, o) => s.WriteBoolean((bool)o)},
            {typeof(byte), (s, o) => s.WriteInt8((byte)o)},
            {typeof(Int16), (s, o) => s.WriteInt16((Int16)o)},
            {typeof(Int32), (s, o) => s.WriteInt32((Int32)o)},
            {typeof(Int64), (s, o) => s.WriteInt64((Int64)o)},
            {typeof(byte[]), (s, o) => s.WriteBytes((byte[])o)},
            {typeof(string), (s, o) => s.WriteString((string)o)},
        };
        public static void Write<T>(this Stream stream, T obj)
        {
            if (WriteFuncs.ContainsKey(typeof(T)))
            {
                WriteFuncs[typeof(T)](stream, obj);
                return;
            }

            throw new NotImplementedException();
        }
        static Dictionary<Type, Func<Stream, object>> ReadFuncs = new Dictionary<Type, Func<Stream, object>>()
        {
            {typeof(bool), s => s.ReadBoolean()},
            {typeof(byte), s => s.ReadInt8()},
            {typeof(Int16), s => s.ReadInt16()},
            {typeof(Int32), s => s.ReadInt32()},
            {typeof(Int64), s => s.ReadInt64()},
            {typeof(string), s => s.ReadString()},
        };
        public static T Read<T>(this Stream stream)
        {
            if (ReadFuncs.ContainsKey(typeof(T)))
                return (T)ReadFuncs[typeof(T)](stream);

            throw new NotImplementedException();
        }
    }
}