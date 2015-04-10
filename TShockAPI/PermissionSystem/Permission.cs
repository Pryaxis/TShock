using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockAPI.PermissionSystem
{
	/// <summary>
	/// Types of permissions
	/// </summary>
	public enum PermissionType
	{
		/// <summary>
		/// Permission set that allows a permission.
		/// </summary>
		Normal,

		/// <summary>
		/// Permission set that disallows a permission.
		/// </summary>
		Negated,

		/// <summary>
		/// Used to get the set difference of Normal and Negated, ie Normal / Negated.
		/// </summary>
		Allowed
	}

	/// <summary>
	/// A class that encapsulates a permission with a type of permission.
	/// </summary>
	public class PermissionNode
	{
		/// <summary>
		/// The permission of this node.
		/// </summary>
		public String Permission { get; private set; }

		/// <summary>
		/// The type of permission.
		/// </summary>
		public PermissionType PermissionType { get; private set; }

		/// <summary>
		/// Constructor that assings the permission and type.
		/// </summary>
		/// <param name="permission">The permission string without specifier.</param>
		/// <param name="type">The type of permission.</param>
		public PermissionNode(String permission, PermissionType type = PermissionType.Normal)
		{
			Permission = permission;
			PermissionType = type;
		}
	}
}
