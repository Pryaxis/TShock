using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShock
{
	public interface IPlayer
	{
		/// <summary>
		/// Meant to only be used by TServer.
		/// </summary>
		int Id { get; }
		string Name { get; set; }
	}
}
