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
		/// Gets or Sets the Auto Reuse All Weapons flag on the backing field
		/// </summary>
		public bool AutoReuseAllWeapons
		{
			get => bitsbyte[1];
			set => bitsbyte[1] = value;
		}

		/// <summary>
		/// Gets or Sets the Control Down Hold flag on the backing field
		/// </summary>
		public bool ControlDownHold
		{
			get => bitsbyte[2];
			set => bitsbyte[2] = value;
		}

		/// <summary>
		/// Gets or Sets the Is Operating Another Entity flag on the backing field
		/// </summary>
		public bool IsOperatingAnotherEntity
		{
			get => bitsbyte[3];
			set => bitsbyte[3] = value;
		}

		/// <summary>
		/// Gets or Sets the Control Use Tile flag on the backing field
		/// </summary>
		public bool ControlUseTile
		{
			get => bitsbyte[4];
			set => bitsbyte[4] = value;
		}

		/// <summary>
		/// Gets or Sets the Has Camera Target flag on the backing field
		/// </summary>
		public bool HasCameraTarget
		{
			get => bitsbyte[5];
			set => bitsbyte[5] = value;
		}

		/// <summary>
		/// Gets or Sets the Last Item Use Attempt Success flag on the backing field
		/// </summary>
		public bool LastItemUseAttemptSuccess
		{
			get => bitsbyte[6];
			set => bitsbyte[6] = value;
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
