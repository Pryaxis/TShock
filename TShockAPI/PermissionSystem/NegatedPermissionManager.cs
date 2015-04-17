using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockAPI.PermissionSystem
{
	/// <summary>
	/// A permission manager that supports negative permissions as well as normal permissions.
	/// </summary>
	public class NegatedPermissionManager : IPermissionManager
	{
		/// <summary>
		/// The list of normal, allowed permissions.
		/// </summary>
		private IPermissionList permissions;

		/// <summary>
		/// The list of negated permissions.
		/// </summary>
		private IPermissionList negatedPermissions;

		/// <summary>
		/// The prefix to string based permissions that define that they are negated.
		/// </summary>
		public const char NegatedPrefix = '!';
 
		/// <summary>
		/// Default constructor that news up the permission lists.
		/// </summary>
		public NegatedPermissionManager()
		{
			this.permissions = new PermissionList();
			this.negatedPermissions = new PermissionList();
		}

		/// <summary>
		/// Constructor that takes a permission list as a string, and parses it.
		/// </summary>
		/// <param name="permissionlist">The list of permissions to be parsed.</param>
		public NegatedPermissionManager(string permissionlist)
		{
			this.permissions = new PermissionList();
			this.negatedPermissions = new PermissionList();
			Parse(permissionlist);
		}

		/// <summary>
		/// Constructor that takes a list of permission strings, and parses it.
		/// </summary>
		/// <param name="permissionlist">The list of permissions to be parsed.</param>
		public NegatedPermissionManager(List<string> permissionlist)
		{
			this.permissions = new PermissionList();
			this.negatedPermissions = new PermissionList();
			Parse(permissionlist);
		}

		/// <summary>
		/// Constructor that takes a PermissionList to assing to the allowed permission set.
		/// </summary>
		/// <param name="permissions">The allowed permission set.</param>
		public NegatedPermissionManager(PermissionList permissions)
		{
			this.permissions = permissions;
			this.negatedPermissions = new PermissionList();
		}

		/// <summary>
		/// Constructor that takes both an allowed PermissionList and a negated PermissionList.
		/// </summary>
		/// <param name="permissions">The allowed permission set.</param>
		/// <param name="negatedPermissions">The negated permission set.</param>
		public NegatedPermissionManager(PermissionList permissions, PermissionList negatedPermissions)
		{
			this.permissions = permissions;
			this.negatedPermissions = negatedPermissions;
		}

		/// <summary>
		/// Adds a permission to the specified set based on the type.  This will automatically revoke it from the opposite set if it is set.
		/// </summary>
		/// <param name="permission">The permission and type to add.</param>
		public void AddPermission(PermissionNode permission)
		{
			if (permission.PermissionType == PermissionType.Negated)
			{
				negatedPermissions.AddPermission(permission.Permission);
				permissions.RemovePermission(permission.Permission);
			}
			else
			{
				permissions.AddPermission(permission.Permission);
				negatedPermissions.RemovePermission(permission.Permission);
			}
		}

		/// <summary>
		/// Adds a permission to the specified set after parsing the string permission.  This will automatically revoke it from the opposite set if it is set.
		/// </summary>
		/// <param name="permission">The permission to parse.</param>
		public void AddPermission(string permission)
		{
			if (permission.Length > 0)
			{
				string realPermission;
				AddPermission(ParsePermission(permission, out realPermission) == PermissionType.Negated
					? new PermissionNode(realPermission, PermissionType.Negated)
					: new PermissionNode(realPermission));
			}
		}

		/// <summary>
		/// Removes a permission from the specified set based on type.
		/// </summary>
		/// <param name="permission">The permission and the type that it should be removed from.</param>
		public void RemovePermission(PermissionNode permission)
		{
			if (permission.PermissionType == PermissionType.Negated)
			{
				negatedPermissions.RemovePermission(permission.Permission);
			}
			else
			{
				permissions.RemovePermission(permission.Permission);
			}
		}

		/// <summary>
		/// Removes a permission after parsing the string permission.
		/// </summary>
		/// <param name="permission">The string permission to parse.</param>
		public void RemovePermission(string permission)
		{
			if (permission.Length > 0)
			{
				string realPermission;
				RemovePermission(ParsePermission(permission, out realPermission) == PermissionType.Negated
					? new PermissionNode(realPermission, PermissionType.Negated)
					: new PermissionNode(realPermission));
			}
		}

		/// <summary>
		/// Checks to see if a permission exists in the specified set.
		/// </summary>
		/// <param name="permission">The permission and type it should be checked in.</param>
		/// <returns>If the permission exists in the specified set.</returns>
		public bool HasPermission(PermissionNode permission)
		{
			if (permission.PermissionType == PermissionType.Negated)
			{
				return negatedPermissions.HasPermission(permission.Permission);
			}
			else
			{
				return permissions.HasPermission(permission.Permission);
			}
		}

		/// <summary>
		/// Return a PermissionList with all the permissions assigned to this manager, with negated permissions removed.
		/// This is the set of permissions that this manager will allow.
		/// </summary>
		/// <returns></returns>
		public IPermissionList TotalPermissions()
		{
			List<string> perms = permissions.GetPermissions();
			perms.RemoveAll(p => negatedPermissions.HasPermission(p));
			return new PermissionList(perms);
		}

		/// <summary>
		/// Parses a permission into a raw permission, outputs that, and returns the permission type it belongs to.
		/// </summary>
		/// <param name="permission">The permission to parse.</param>
		/// <param name="realPermission">The parsed permissions raw string, ie, "!node.permission" => "node.permission"</param>
		/// <returns>Returns the permission set this permission belongs to.</returns>
		private PermissionType ParsePermission(string permission, out string realPermission)
		{
			if (!String.IsNullOrWhiteSpace(permission) && permission[0] == NegatedPrefix)
			{
				realPermission = permission.Substring(1);
				return PermissionType.Negated;
			}
			else
			{
				realPermission = permission;
				return PermissionType.Normal;
			}
		}

		/// <summary>
		/// Parses a CSV string into an allowed and negated permission set.
		/// </summary>
		/// <param name="list">The comma seperated list of permissions, ie "node.permission,!node.permission"</param>
		public void Parse(string list)
		{
			this.permissions = new PermissionList();
			this.negatedPermissions = new PermissionList();

			if (String.IsNullOrWhiteSpace(list))
				return;

			string[] perms = list.Split(',');
			foreach (var permission in perms.Select(p=> p.Trim()))
			{
				string realPermission;
				if (ParsePermission(permission, out realPermission) == PermissionType.Negated)
				{
					negatedPermissions.AddPermission(realPermission);
				}
				else
				{
					permissions.AddPermission(realPermission);
				}
			}
		}

		/// <summary>
		/// Parses a List of strings, into an allowed and negated permission set.
		/// </summary>
		/// <param name="list">The list of strings to parse.</param>
		public void Parse(List<string> list)
		{
			Parse(String.Join(",", list));
		}

		/// <summary>
		/// Returns the specified permission set.
		/// </summary>
		/// <param name="type">The type of permission set to return.</param>
		/// <returns>The specified permission set.</returns>
		public IPermissionList GetPermissions(PermissionType type)
		{
			if (type == PermissionType.Negated)
			{
				return negatedPermissions;
			}
			else if (type == PermissionType.Allowed)
			{
				return TotalPermissions();
			}
			else
			{
				return permissions;
			}
		}

		/// <summary>
		/// Clones another managers permissions sets into this ones.
		/// </summary>
		/// <param name="manager">The NegatedPermissionManager to clone from.</param>
		public void Clone(IPermissionManager manager)
		{
			var permManager = manager as NegatedPermissionManager;
			permissions = permManager.permissions;
			negatedPermissions = permManager.negatedPermissions;
		}

		/// <summary>
		/// Outputs all the permissions in comma seperated list form with specifiers.
		/// </summary>
		/// <returns>The output string.</returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();

			foreach (var perm in negatedPermissions.GetPermissions())
			{
				if (builder.Length > 0)
					builder.Append(",");
				builder.Append(NegatedPrefix + perm);
			}

			foreach (var perm in TotalPermissions().GetPermissions())
			{
				if (builder.Length > 0)
					builder.Append(",");
				builder.Append(perm);
			}

			return builder.ToString();
		}
	}
}
