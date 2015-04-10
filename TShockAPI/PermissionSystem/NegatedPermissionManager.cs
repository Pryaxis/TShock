using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockAPI.PermissionSystem
{
	public class NegatedPermissionManager : IPermissionManager
	{
		private IPermissionList permissions;
		private IPermissionList negatedPermissions;

		public const String NegatedPrefix = "!";
 
		public NegatedPermissionManager()
		{
			this.permissions = new PermissionList();
			this.negatedPermissions = new PermissionList();
		}

		public NegatedPermissionManager(string permissionlist)
		{
			this.permissions = new PermissionList();
			this.negatedPermissions = new PermissionList();
			Parse(permissionlist);
		}

		public NegatedPermissionManager(PermissionList permissions)
		{
			this.permissions = permissions;
			this.negatedPermissions = new PermissionList();
		}

		public NegatedPermissionManager(PermissionList permissions, PermissionList negatedPermissions)
		{
			this.permissions = permissions;
			this.negatedPermissions = negatedPermissions;
		}

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

		public void AddPermission(string permission)
		{
			if (permission.Length > 0)
			{
				if (permission.Substring(0, 1) == NegatedPrefix)
				{
					AddPermission(new PermissionNode(permission.Substring(1), PermissionType.Negated));
				}
				else
				{
					AddPermission(new PermissionNode(permission));
				}
			}
		}

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

		public void RemovePermission(string permission)
		{
			if (permission.Length > 0)
			{
				if (permission.Substring(0, 1) == NegatedPrefix)
				{
					RemovePermission(new PermissionNode(permission.Substring(1), PermissionType.Negated));
				}
				else
				{
					RemovePermission(new PermissionNode(permission));
				}
			}
		}

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

		public IPermissionList GetPermissions()
		{
			return permissions;
		}

		public IPermissionList GetNegatedPermissions()
		{
			return negatedPermissions;
		}

		public IPermissionList TotalPermissions()
		{
			List<String> perms = permissions.GetPermissions();
			perms.RemoveAll(p => GetNegatedPermissions().HasPermission(p));
			return new PermissionList(perms);
		}


		public void Parse(string list)
		{
			this.permissions = new PermissionList();
			this.negatedPermissions = new PermissionList();

			if (String.IsNullOrWhiteSpace(list))
				return;

			string[] perms = list.Split(',');
			foreach (var permission in perms)
			{
				if (permission.StartsWith(NegatedPrefix))
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

		public void Clone(IPermissionManager manager)
		{
			var permManager = manager as NegatedPermissionManager;
			permissions = permManager.permissions;
			negatedPermissions = permManager.negatedPermissions;
		}

		public override String ToString()
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
