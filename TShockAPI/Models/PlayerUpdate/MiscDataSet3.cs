using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TShockAPI.Models.PlayerUpdate
{
	/// <summary>
	/// Model for the third set of misc data sent with a player update packet
	/// </summary>
	public struct MiscDataSet3
	{
		public BitsByte bitsbyte;

		/// <summary>
		/// Gets or Sets the Sleeping flag on the backing field
		/// </summary>
		public bool IsSleeping
		{
			get => bitsbyte[0];
			set => bitsbyte[0] = value;
		}

		/// <summary>
		/// Constructs a new instance of MiscDataSet3 with the given backing BitsByte
		/// </summary>
		/// <param name="bitsbyte"></param>
		public MiscDataSet3(BitsByte bitsbyte)
		{
			this.bitsbyte = bitsbyte;
		}
	}
}
