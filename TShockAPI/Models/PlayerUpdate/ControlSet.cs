using Terraria;

namespace TShockAPI.Models.PlayerUpdate
{

	/// <summary>
	/// Model for a control event sent with a player update packet
	/// </summary>
	public struct ControlSet
	{
		/// <summary>
		/// Backing BitsByte field
		/// </summary>
		public BitsByte bitsbyte;

		/// <summary>
		/// Gets or Sets the Up flag on the backing field
		/// </summary>
		public bool MoveUp
		{
			get => bitsbyte[0];
			set => bitsbyte[0] = value;
		}

		/// <summary>
		/// Gets or Sets the Down flag on the backing field
		/// </summary>
		public bool MoveDown
		{
			get => bitsbyte[1];
			set => bitsbyte[1] = value;
		}

		/// <summary>
		/// Gets or Sets the Left flag on the backing field
		/// </summary>
		public bool MoveLeft
		{
			get => bitsbyte[2];
			set => bitsbyte[2] = value;
		}

		/// <summary>
		/// Gets or Sets the Right flag on the backing field
		/// </summary>
		public bool MoveRight
		{
			get => bitsbyte[3];
			set => bitsbyte[3] = value;
		}

		/// <summary>
		/// Gets or Sets the Jump flag on the backing field
		/// </summary>
		public bool Jump
		{
			get => bitsbyte[4];
			set => bitsbyte[4] = value;
		}

		/// <summary>
		/// Gets or Sets the ControlUseItem flag on the backing field
		/// </summary>
		public bool IsUsingItem
		{
			get => bitsbyte[5];
			set => bitsbyte[5] = value;
		}

		/// <summary>
		/// Gets or Sets the Direction flag on the backing field. True = 1, false  = -1
		/// </summary>
		public bool FaceDirection
		{
			get => bitsbyte[6];
			set => bitsbyte[6] = value;
		}

		/// <summary>
		/// Constructs a new instance of ControlsModel with the given backing bitsbyte
		/// </summary>
		/// <param name="bitsbyte"></param>
		public ControlSet(BitsByte bitsbyte)
		{
			this.bitsbyte = bitsbyte;
		}
	}
}
