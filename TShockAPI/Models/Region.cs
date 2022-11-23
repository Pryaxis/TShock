using System;
using System.Collections.Generic;
using TShockAPI.DB;

namespace TShockAPI.Models;

/// <summary>
/// A rectangular section of tiles that can be marked as protected, preventing modifications.
/// </summary>
public class Region
{
	/// <summary>
	/// Database ID of the region.
	/// </summary>
	public int Id { get; set; }
	/// <summary>
	/// Whether or not the region should be protected against unauthorised changes.
	/// </summary>
	public bool Protected { get; set; }
	/// <summary>
	/// The tile-based X position of the region's top left corner.
	/// </summary>
	public int X { get; set; }
	/// <summary>
	/// The tile-based Y position of the region's top left corner.
	/// </summary>
	public int Y { get; set; }
	/// <summary>
	/// The Z-layer of the region. Lower Z-layers override higher Z-layers.
	/// </summary>
	public int Z { get; set; }
	/// <summary>
	/// The tile-based width of the region.
	/// </summary>
	public int Width { get; set; }
	/// <summary>
	/// The tile-based height of the region.
	/// </summary>
	public int Height { get; set; }
	/// <summary>
	///	The list of groups that are allowed to access the region.
	/// </summary>
	public IList<UserGroup> Groups { get; set; } = new List<UserGroup>();
	/// <summary>
	/// The list of user accounts that are allowed to access the region.
	/// </summary>
	public IList<UserAccount> Users { get; set; } = new List<UserAccount>();
	/// <summary>
	/// The user account that owns the region.
	/// </summary>
	public UserAccount Owner { get; set; }
}
