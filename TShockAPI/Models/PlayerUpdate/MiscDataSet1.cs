using Terraria;

namespace TShockAPI.Models.PlayerUpdate
{
	/// <summary>
	/// Model for the first set of misc data sent with a player update packet
	/// </summary>
	public struct MiscDataSet1
	{
		/// <summary>
		/// Backing BitsByte field
		/// </summary>
		public BitsByte bitsbyte;

		/// <summary>
		/// Gets or Sets the Pulley flag on the backing field
		/// </summary>
		public bool IsUsingPulley
		{
			get => bitsbyte[0];
			set => bitsbyte[0] = value;
		}

		/// <summary>
		/// Gets or Sets the Pulley Direction flag on the backing field. True = 2, false = 1
		/// </summary>
		public bool PulleyDirection
		{
			get => bitsbyte[1];
			set => bitsbyte[1] = value;
		}

		/// <summary>
		/// Gets or Sets the Velocity > 0 flag on the backing field
		/// </summary>
		public bool HasVelocity
		{
			get => bitsbyte[2];
			set => bitsbyte[2] = value;
		}

		/// <summary>
		/// Gets or Sets the Vortex Stealth flag on the backing field
		/// </summary>
		public bool IsVortexStealthActive
		{
			get => bitsbyte[3];
			set => bitsbyte[3] = value;
		}

		/// <summary>
		/// Gets or Sets the Gravity Direction flag on the backing field. True = 1, False = -1
		/// </summary>
		public bool GravityDirection
		{
			get => bitsbyte[4];
			set => bitsbyte[4] = value;
		}

		/// <summary>
		/// Gets or Sets the Shield Raised flag on the backing field
		/// </summary>
		public bool IsShieldRaised
		{
			get => bitsbyte[5];
			set => bitsbyte[5] = value;
		}

		/// <summary>
		/// Gets or Sets the Ghost flag on the backing field
		/// </summary>
		public bool IsGhosted
		{
			get => bitsbyte[6];
			set => bitsbyte[6] = value;
		}

		/// <summary>
		/// Constructs a new instance of MiscDataSet1 with the given backing BitsByte
		/// </summary>
		/// <param name="bitsbyte"></param>
		public MiscDataSet1(BitsByte bitsbyte)
		{
			this.bitsbyte = bitsbyte;
		}
	}
}
