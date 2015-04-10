using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockAPI.PermissionSystem
{
	/// <summary>
	/// An interface to describe a class that can maintain permissions of differing types, and how to check for the presense of a permission.
	/// </summary>
	public interface IPermissionManager
	{
		/// <summary>
		/// Add the permission to the specified list of permissions.
		/// </summary>
		/// <param name="permission">A Permission and a Type of permission it should be added to</param>
		void AddPermission(PermissionNode permission);

		/// <summary>
		/// Parse the string and add it to the permission list it fits (the first char being the specifier).
		/// </summary>
		/// <param name="permission">A permission string, ie "node.permission", or "!node.permission"</param>
		void AddPermission(string permission);

		/// <summary>
		/// Remove a permission from the specified list of permissions.
		/// </summary>
		/// <param name="permission">A Permission and a Type of permission it should be removed from.</param>
		void RemovePermission(PermissionNode permission);

		/// <summary>
		/// Parse the string and remove it from the permission list it fits (the first char being the specifier).
		/// </summary>
		/// <param name="permission">A permission string, ie "node.permission", or "!node.permission"</param>
		void RemovePermission(string permission);

		/// <summary>
		/// Check to see if the permission exists in the specified list.
		/// </summary>
		/// <param name="permission">A Permission and the Type of permission that should be checked for.</param>
		/// <returns>If the permission exists in the given list.</returns>
		bool HasPermission(PermissionNode permission);

		/// <summary>
		/// Parse a CSV string into the lists of permissions.
		/// </summary>
		/// <param name="list">A comma seperated list of permissions, ie "node.permission,!node.permission"</param>
		void Parse(String list);

		/// <summary>
		/// Parse permissions from the list into their specified permission list.
		/// </summary>
		/// <param name="list">A list of permissions, ie ["node.permission", "!node.permission"]</param>
		void Parse(List<String> list);

		/// <summary>
		/// Get the specified list of permissions
		/// </summary>
		/// <param name="type">The type of permission to fetch.</param>
		/// <returns>The list of permissions of that type.</returns>
		IPermissionList GetPermissions(PermissionType type);

		/// <summary>
		/// Copy the contents of the provided Manager into us.
		/// </summary>
		/// <param name="manager">The manager to copy from.</param>
		void Clone(IPermissionManager manager);
	}
}
