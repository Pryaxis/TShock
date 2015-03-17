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

		private List<String> currentAllowPermissions = new List<string>();

		public const String NeverPrefix = "-";
		public const String NegatedPrefix = "!";
 
		public PermissionManager()
		{
			this.permissions = new PermissionList();
			this.negatedPermissions = new PermissionList();
			this.neverPermissions = new PermissionList();
			Refresh();
		}

		public PermissionManager(string permissionlist)
		{
			this.permissions = new PermissionList();
			this.negatedPermissions = new PermissionList();
			this.neverPermissions = new PermissionList();
			Parse(permissionlist);
		}

		public PermissionManager(PermissionList permissions)
		{
			this.permissions = permissions;
			this.negatedPermissions = new PermissionList();
			this.neverPermissions = new PermissionList();
			Refresh();
		}

		public PermissionManager(PermissionList permissions, PermissionList negatedPermissions)
		{
			this.permissions = permissions;
			this.negatedPermissions = negatedPermissions;
			this.neverPermissions = new PermissionList();
			Refresh();
		}

		public PermissionManager(PermissionList permissions, PermissionList negatedPermissions, PermissionList neverPermissions)
		{
			this.permissions = permissions;
			this.negatedPermissions = negatedPermissions;
			this.neverPermissions = neverPermissions;
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
			return permissions;
		}

		public IPermissionList GetNegatedPermissions()
		{
			return negatedPermissions;
		}

		public IPermissionList GetNeverPermissions()
		{
			return neverPermissions;
		}

		public IPermissionList TotalPermissions()
		{
			List<String> perms = permissions.GetPermissions();
			perms.RemoveAll(p => GetNegatedPermissions().HasPermission(p) || GetNeverPermissions().HasPermission(p));
			return new PermissionList(perms);
		}

		public void Refresh()
		{
			currentAllowPermissions = TotalPermissions().GetPermissions();
		}

		public void Parse(string list)
		{
			this.permissions = new PermissionList();
			this.negatedPermissions = new PermissionList();
			this.neverPermissions = new PermissionList();

			if (String.IsNullOrWhiteSpace(list))
				return;

			string[] perms = list.Split(',');
			foreach (var permission in perms)
			{
				if (permission.StartsWith(NeverPrefix))
				{
					var perm = permission.Substring(1);
					neverPermissions.AddPermission(perm);
				}
				else if (permission.StartsWith(NegatedPrefix))
				{
					var perm = permission.Substring(1);
					negatedPermissions.AddPermission(perm);
				}
				else
				{
					permissions.AddPermission(permission);
				}
			}
		}

		public void Parse(List<String> list)
		{
			Parse(String.Join(",", list));
		}

		public void Clone(IPermissionManager manager)
		{
			permissions = manager.GetPermissions();
			negatedPermissions = manager.GetNegatedPermissions();
			neverPermissions = manager.GetNeverPermissions();
			Refresh();
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
	}
}
