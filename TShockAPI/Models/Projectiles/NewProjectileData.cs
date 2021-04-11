using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TShockAPI.Models.Projectiles
{
	/// <summary>
	/// Model for the data sent with a new projectile packet
	/// </summary>
	public struct NewProjectileData
	{
		public BitsByte bitsbyte;

		/// <summary>
		/// Gets or Sets the keepTryingHoverDown flag on the backing field
		/// </summary>
		public bool[] AI
		{
			get
			{
				bool[] arr = new bool[Projectile.maxAI];
				for (int i = 0; i < Projectile.maxAI; i++)
				{
					arr[i] = bitsbyte[i];
				}

				return arr;
			}
			set
			{
				for (int i = 0; i < Projectile.maxAI; i++)
				{
					bitsbyte[i] = value[i];
				}
			}
		}

		public bool HasBannerIdToRespondTo
		{
			get => bitsbyte[3];
			set => bitsbyte[3] = value;
		}

		/// <summary>
		/// Gets or Sets the Damage flag on the backing field
		/// </summary>
		public bool HasDamage
		{
			get => bitsbyte[4];
			set => bitsbyte[4] = value;
		}

		/// <summary>
		/// Gets or Sets the Knockback flag on the backing field
		/// </summary>
		public bool HasKnockback
		{
			get => bitsbyte[5];
			set => bitsbyte[5] = value;
		}

		/// <summary>
		/// Gets or Sets the Original Damage flag on the backing field
		/// </summary>
		public bool HasOriginalDamage
		{
			get => bitsbyte[6];
			set => bitsbyte[6] = value;
		}

		/// <summary>
		/// Gets or Sets the UUID flag on the backing field
		/// </summary>
		public bool HasUUUID
		{
			get => bitsbyte[7];
			set => bitsbyte[7] = value;
		}

		/// <summary>
		/// Constructs a new instance of NewProjectileData with the given backing BitsByte
		/// </summary>
		/// <param name="bitsbyte"></param>
		public NewProjectileData(BitsByte bitsbyte)
		{
			this.bitsbyte = bitsbyte;
		}
	}
}
