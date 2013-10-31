using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockAPI
{
	/// <summary>
	/// Custom packet types for use with Raptor.
	/// </summary>
	public enum RaptorPacketTypes : byte
	{
		/// <summary>
		/// The packet sent to the server to be acknowledged as a Raptor client.
		/// </summary>
		Acknowledge = 0,
		/// <summary>
		/// The packet sent to the client which dictates its permissions.
		/// </summary>
		Permissions,
		/// <summary>
		/// The packet sent which sets region info.
		/// </summary>
		Region,
		/// <summary>
		/// The packet sent to delete a region.
		/// </summary>
		RegionDelete,
		/// <summary>
		/// The packet sent which sets warp info.
		/// </summary>
		Warp,
		/// <summary>
		/// The packet sent to delete a warp.
		/// </summary>
		WarpDelete
	}
}
