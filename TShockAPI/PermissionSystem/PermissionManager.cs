using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockAPI.PermissionSystem
{
	public class PermissionManager : IPermissionManager
	{
		private IPermissionList permissions;
		private IPermissionList negatedPermissions;
		private IPermissionList neverPermissions;
		private List<IPermissionManager> parents;

		private List<String> currentAllowPermissions = new List<string>();

		public const String NeverPrefix = "-";
		public const String NegatedPrefix = "!";
 
		public PermissionManager()
		{
			this.permissions = new PermissionList();
			this.negatedPermissions = new PermissionList();
			this.neverPermissions = new PermissionList();
			this.parents = new List<IPermissionManager>();
			Refresh();
		}

		public PermissionManager(string permissionlist)
		{
			this.permissions = new PermissionList();
			this.negatedPermissions = new PermissionList();
			this.neverPermissions = new PermissionList();
			this.parents = new List<IPermissionManager>();

			string[] perms = permissionlist.Split(',');
			foreach (var permission in perms)
			{
				if (permission.StartsWith(NeverPrefix))
				{
					neverPermissions.AddPermission(permission);
				}
				else if (permission.StartsWith(NegatedPrefix))
				{
					negatedPermissions.AddPermission(permission);
				}
				else
				{
					permissions.AddPermission(permission);
				}
			}
		}

		public PermissionManager(PermissionList permissions)
		{
			this.permissions = permissions;
			this.negatedPermissions = new PermissionList();
			this.neverPermissions = new PermissionList();
			this.parents = new List<IPermissionManager>();
			Refresh();
		}

		public PermissionManager(PermissionList permissions, PermissionList negatedPermissions)
		{
			this.permissions = permissions;
			this.negatedPermissions = negatedPermissions;
			this.neverPermissions = new PermissionList();
			this.parents = new List<IPermissionManager>();
			Refresh();
		}

		public PermissionManager(PermissionList permissions, PermissionList negatedPermissions, PermissionList neverPermissions)
		{
			this.permissions = permissions;
			this.negatedPermissions = negatedPermissions;
			this.neverPermissions = neverPermissions;
			this.parents = new List<IPermissionManager>();
			Refresh();
		}

		public PermissionManager(PermissionList permissions, PermissionList negatedPermissions, PermissionList neverPermissions, List<IPermissionManager> parents)
		{
			this.permissions = permissions;
			this.negatedPermissions = negatedPermissions;
			this.neverPermissions = neverPermissions;
			this.parents = parents;
			Refresh();
		}

		public void AddPermission(string permission)
		{
			if (permission.StartsWith(NeverPrefix))
			{
				var perm = permission.Substring(1);
				neverPermissions.AddPermission(perm);
				negatedPermissions.RemovePermission(perm);
				permissions.RemovePermission(perm);
			}
			else if (permission.StartsWith(NegatedPrefix))
			{
				var perm = permission.Substring(1);
				negatedPermissions.AddPermission(perm);
				neverPermissions.RemovePermission(perm);
				permissions.RemovePermission(perm);
			}
			else
			{
				permissions.AddPermission(permission);
				negatedPermissions.RemovePermission(permission);
				neverPermissions.RemovePermission(permission);
			}
			Refresh();
		}

		public void RemovePermission(string permission)
		{
			if (permission.StartsWith(NeverPrefix))
			{
				var perm = permission.Substring(1);
				neverPermissions.RemovePermission(perm);
			}
			else if (permission.StartsWith(NegatedPrefix))
			{
				var perm = permission.Substring(1);
				negatedPermissions.RemovePermission(perm);
			}
			else
			{
				permissions.RemovePermission(permission);
			}
			Refresh();
		}

		public bool HasPermission(string permission)
		{
			return currentAllowPermissions.Contains(permission);
		}

		public IPermissionList GetPermissions()
		{
			List<String> perms = permissions.GetPermissions();
			foreach (var parent in parents)
			{
				perms.AddRange(parent.GetPermissions().GetPermissions().Where(p => !perms.Contains(p)));
			}
			return new PermissionList(perms);
		}

		public IPermissionList GetNegatedPermissions()
		{
			List<String> perms = negatedPermissions.GetPermissions();
			foreach (var parent in parents)
			{
				perms.AddRange(parent.GetNegatedPermissions().GetPermissions().Where(p => !perms.Contains(p)));
			}
			return new PermissionList(perms);
		}

		public IPermissionList GetNeverPermissions()
		{
			List<String> perms = neverPermissions.GetPermissions();
			foreach (var parent in parents)
			{
				perms.AddRange(parent.GetNeverPermissions().GetPermissions().Where(p => !perms.Contains(p)));
			}
			return new PermissionList(perms);
		}

		public IPermissionList TotalPermissions()
		{
			List<String> perms = GetPermissions().GetPermissions();
			perms.RemoveAll(p => GetNegatedPermissions().HasPermission(p) || GetNeverPermissions().HasPermission(p));
			return new PermissionList(perms);
		}

		public void AddParent(IPermissionManager parent)
		{
			if (parents.Contains(parent))
			{
				return;
			}
			
			parents.Add(parent);
			Refresh();
		}

		public void RemoveParent(IPermissionManager parent)
		{
			parents.Remove(parent);
			Refresh();
		}

		public void Refresh()
		{
			currentAllowPermissions = TotalPermissions().GetPermissions();
		}

		private List<String> AllowedPermissions()
		{
			List<String> permissions = this.permissions.GetPermissions();
			permissions.RemoveAll(
				p => negatedPermissions.GetPermissions().Contains(p) || neverPermissions.GetPermissions().Contains(p));
			return permissions;
		}

		public override String ToString()
		{
			StringBuilder builder = new StringBuilder();
			foreach (var perm in neverPermissions.GetPermissions())
			{
				if (builder.Length > 0)
					builder.Append(",");
				builder.Append(NeverPrefix + perm);
			}

			foreach (var perm in negatedPermissions.GetPermissions())
			{
				if (builder.Length > 0)
					builder.Append(",");
				builder.Append(NegatedPrefix + perm);
			}

			foreach (var perm in AllowedPermissions())
			{
				if (builder.Length > 0)
					builder.Append(",");
				builder.Append(perm);
			}

			return builder.ToString();
		}

		public List<IPermissionManager> GetParents()
		{
			return parents;
		}
	}
}
