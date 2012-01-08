using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShock
{
	public class TShockGame	: IGame
	{
		protected TShockPlayerList playerslist = new TShockPlayerList();
		public IList<IPlayer> Players
		{
			get
			{
				return playerslist;
			}
		}
	}
}
