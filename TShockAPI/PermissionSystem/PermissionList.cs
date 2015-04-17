using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockAPI.PermissionSystem
{
	/// <summary>
	/// A concreate permission list.
	/// </summary>
	public class PermissionList : IPermissionList
	{
		/// <summary>
		/// This set's list of permissions.
		/// </summary>
		private List<string> permissions;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public PermissionList()
		{
			permissions = new List<string>();
		}

		/// <summary>
		/// Constructor that assigns the list of permissions.
		/// </summary>
		/// <param name="permissions">The list of permissions to assign.</param>
		public PermissionList(List<string> permissions)
		{
			this.permissions = permissions;
		}

		/// <summary>
		/// Adds a permission to this set.
		/// </summary>
		/// <param name="permission">The permission to add.</param>
		public void AddPermission(string permission)
		{
			if (permissions.Contains(permission))
				return;

			permissions.Add(permission);
		}

		/// <summary>
		/// Removes a permission from this set.
		/// </summary>
		/// <param name="permission">The permission to remove.</param>
		public void RemovePermission(string permission)
		{
			if (permissions.Contains(permission))
			{
				permissions.Remove(permission);
			}

			string[] nodes = permission.Split('.');
			for (int i = nodes.Length - 1; i >= 1; i--)
			{
				nodes[i] = "*";
				var testPerm = String.Join(".", nodes, 0, i + 1);
				if (permissions.Contains(testPerm))
				{
					permissions.Remove(testPerm);
				}
			}
		}

		/// <summary>
		/// Returns all the permissions in this set.
		/// </summary>
		/// <returns>The permissions in this set.</returns>
		public List<string> GetPermissions()
		{
			return permissions;
		}

		/// <summary>
		/// Check to see if a permission is in this set.
		/// </summary>
		/// <param name="permission">The permission to check for.</param>
		/// <returns>Returns if this permission is present in this set.</returns>
		public bool HasPermission(string permission)
		{
			if (String.IsNullOrEmpty(permission) || permissions.Contains(permission))
			{
				return true;
			}

			string[] nodes = permission.Split('.');
			for (int i = nodes.Length - 1; i >= 0; i--)
			{
				nodes[i] = "*";
				if (permissions.Contains(String.Join(".", nodes, 0, i + 1)))
				{
					return true;
				}
			}

			return false;
		}

		public override string ToString()
		{
			return String.Join(",", permissions);
		}
	}
}
