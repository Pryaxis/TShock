using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockAPI.PermissionSystem
{
	/// <summary>
	/// A list of permissions that are common, ie Allowed, Negated.
	/// </summary>
	public interface IPermissionList
	{
		/// <summary>
		/// Add a permission to this set.
		/// </summary>
		/// <param name="permission">The permission to add.</param>
		void AddPermission(String permission);

		/// <summary>
		/// Remove a permission from this set.
		/// </summary>
		/// <param name="permission">The permission to remove.</param>
		void RemovePermission(String permission);

		/// <summary>
		/// Gets a List of permission string that are in this set.
		/// </summary>
		/// <returns>The list of permissions.</returns>
		List<String> GetPermissions();

		/// <summary>
		/// Checks to see if a permission string is present in this set.
		/// </summary>
		/// <param name="permission">The permission string to check for.</param>
		/// <returns>Returns if the permission string is contained in this set.</returns>
		bool HasPermission(String permission);
	}
}
