using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace TShockAPI.Models.PlayerUpdate
{
	/// <summary>
	/// Model for the second set of misc data sent with a player update packet
	/// </summary>
	public struct MiscDataSet2
	{
		/// <summary>
		/// Backing BitsByte field
		/// </summary>
		public BitsByte bitsbyte;

		/// <summary>
		/// Gets or Sets the keepTryingHoverUp flag on the backing field
		/// </summary>
		public bool TryHoveringUp
		{
			get => bitsbyte[0];
			set => bitsbyte[0] = value;
		}

		/// <summary>
		/// Gets or Sets the Void Vault Enabled flag on the backing field
		/// </summary>
		public bool IsVoidVaultEnabled
		{
			get => bitsbyte[1];
			set => bitsbyte[1] = value;
		}

		/// <summary>
		/// Gets or Sets the Sitting flag on the backing field
		/// </summary>
		public bool IsSitting
		{
			get => bitsbyte[2];
			set => bitsbyte[2] = value;
		}

		/// <summary>
		/// Gets or Sets the Downed DD2 Event (any difficulty) flag on the backing field
		/// </summary>
		public bool HasDownedDd2Event
		{
			get => bitsbyte[3];
			set => bitsbyte[3] = value;
		}

		/// <summary>
		/// Gets or Sets the Petting Animal flag on the backing field
		/// </summary>
		public bool IsPettingAnimal
		{
			get => bitsbyte[4];
			set => bitsbyte[4] = value;
		}

		/// <summary>
		/// Gets or Sets the Is Petted Animal Small flag on the backing field
		/// </summary>
		public bool IsPettedAnimalSmall
		{
			get => bitsbyte[5];
			set => bitsbyte[5] = value;
		}

		/// <summary>
		/// Gets or Sets the Can Return with Potion of Return flag on the backing field
		/// </summary>
		public bool CanReturnWithPotionOfReturn
		{
			get => bitsbyte[6];
			set => bitsbyte[6] = value;
		}

		/// <summary>
		/// Gets or Sets the keepTryingHoverDown flag on the backing field
		/// </summary>
		public bool TryHoveringDown
		{
			get => bitsbyte[7];
			set => bitsbyte[7] = value;
		}

		/// <summary>
		/// Constructs a new instance of MiscDataSet2 with the given backing BitsByte
		/// </summary>
		/// <param name="bitsbyte"></param>
		public MiscDataSet2(BitsByte bitsbyte)
		{
			this.bitsbyte = bitsbyte;
		}
	}
}
