using System;
using System.Collections.Generic;
namespace System.IO.Streams.Generic
{
	public static class StreamGenericExt
	{
		private static Dictionary<Type, Action<Stream, object>> WriteFuncs;
		private static Dictionary<Type, Func<Stream, object>> ReadFuncs;
		public static void Write<T>(this Stream stream, T obj)
		{
			if (StreamGenericExt.WriteFuncs.ContainsKey(typeof(T)))
			{
				StreamGenericExt.WriteFuncs[typeof(T)](stream, obj);
				return;
			}
			throw new NotImplementedException();
		}
		public static T Read<T>(this Stream stream)
		{
			if (StreamGenericExt.ReadFuncs.ContainsKey(typeof(T)))
			{
				return (T)StreamGenericExt.ReadFuncs[typeof(T)](stream);
			}
			throw new NotImplementedException();
		}
		static StreamGenericExt()
		{
			// Note: this type is marked as 'beforefieldinit'.
			Dictionary<Type, Action<Stream, object>> dictionary = new Dictionary<Type, Action<Stream, object>>();
			dictionary.Add(typeof(bool), delegate(Stream s, object o)
			{
				s.WriteBoolean((bool)o);
			}
			);
			dictionary.Add(typeof(byte), delegate(Stream s, object o)
			{
				s.WriteInt8((byte)o);
			}
			);
			dictionary.Add(typeof(short), delegate(Stream bw, object data)
			{
				bw.WriteInt16((short)data);
			}
			);
			dictionary.Add(typeof(int), delegate(Stream bw, object data)
			{
				bw.WriteInt32((int)data);
			}
			);
			dictionary.Add(typeof(long), delegate(Stream bw, object data)
			{
				bw.WriteInt64((long)data);
			}
			);
			dictionary.Add(typeof(float), delegate(Stream bw, object data)
			{
				bw.WriteSingle((float)data);
			}
			);
			dictionary.Add(typeof(double), delegate(Stream bw, object data)
			{
				bw.WriteDouble((double)data);
			}
			);
			dictionary.Add(typeof(byte[]), delegate(Stream s, object o)
			{
				s.WriteBytesWithLength((byte[])o);
			}
			);
			dictionary.Add(typeof(string), delegate(Stream s, object o)
			{
				s.WriteString((string)o);
			}
			);
			StreamGenericExt.WriteFuncs = dictionary;
			Dictionary<Type, Func<Stream, object>> dictionary2 = new Dictionary<Type, Func<Stream, object>>();
			dictionary2.Add(typeof(bool), (Stream s) => s.ReadBoolean());
			dictionary2.Add(typeof(byte), (Stream s) => s.ReadInt8());
			dictionary2.Add(typeof(short), (Stream br) => br.ReadInt16());
			dictionary2.Add(typeof(int), (Stream br) => br.ReadInt32());
			dictionary2.Add(typeof(long), (Stream br) => br.ReadInt64());
			dictionary2.Add(typeof(float), (Stream br) => br.ReadSingle());
			dictionary2.Add(typeof(double), (Stream br) => br.ReadDouble());
			dictionary2.Add(typeof(byte[]), (Stream s) => s.ReadBytesWithLength());
			dictionary2.Add(typeof(string), (Stream s) => s.ReadString());
			StreamGenericExt.ReadFuncs = dictionary2;
		}
	}
}
