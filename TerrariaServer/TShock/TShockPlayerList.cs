using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;

namespace TShock
{
	public class TShockPlayerList : IList<IPlayer>
	{
		[Obsolete("Not implemented", true)]
		public int IndexOf(IPlayer item)
		{
			throw new NotImplementedException();
		}
		[Obsolete("Not implemented", true)]
		public void Insert(int index, IPlayer item)
		{
			throw new NotImplementedException();
		}
		[Obsolete("Not implemented", true)]
		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		public IPlayer this[int index]
		{
			get
			{
				return new TShockPlayer(Main.player[index]);
			}
			set
			{
				throw new NotImplementedException();
			}
		}
		[Obsolete("Not implemented", true)]
		public void Add(IPlayer item)
		{
			throw new NotImplementedException();
		}
		[Obsolete("Not implemented", true)]
		public void Clear()
		{
			throw new NotImplementedException();
		}
		[Obsolete("Not implemented", true)]
		public bool Contains(IPlayer item)
		{
			throw new NotImplementedException();
		}
		[Obsolete("Not implemented", true)]
		public void CopyTo(IPlayer[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}
		[Obsolete("Not implemented", true)]
		public int Count
		{
			get { throw new NotImplementedException(); }
		}
		[Obsolete("Not implemented", true)]
		public bool IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}
		[Obsolete("Not implemented", true)]
		public bool Remove(IPlayer item)
		{
			throw new NotImplementedException();
		}
		[Obsolete("Not implemented", true)]
		public IEnumerator<IPlayer> GetEnumerator()
		{
			throw new NotImplementedException();
		}
		[Obsolete("Not implemented", true)]
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
