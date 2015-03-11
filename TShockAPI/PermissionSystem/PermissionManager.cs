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
 
		public PermissionManager()
		{
			this.permissions = new PermissionList();
			this.negatedPermissions = new PermissionList();
			this.neverPermissions = new PermissionList();
			this.parents = new List<IPermissionManager>();
			currentAllowPermissions = TotalPermissions().GetPermissions();
		}

		public PermissionManager(PermissionList permissions)
		{
			this.permissions = permissions;
			this.negatedPermissions = new PermissionList();
			this.neverPermissions = new PermissionList();
			this.parents = new List<IPermissionManager>();
			currentAllowPermissions = TotalPermissions().GetPermissions();
		}

		public PermissionManager(PermissionList permissions, PermissionList negatedPermissions)
		{
			this.permissions = permissions;
			this.negatedPermissions = negatedPermissions;
			this.neverPermissions = new PermissionList();
			this.parents = new List<IPermissionManager>();
			currentAllowPermissions = TotalPermissions().GetPermissions();
		}

		public PermissionManager(PermissionList permissions, PermissionList negatedPermissions, PermissionList neverPermissions)
		{
			this.permissions = permissions;
			this.negatedPermissions = negatedPermissions;
			this.neverPermissions = neverPermissions;
			this.parents = new List<IPermissionManager>();
			currentAllowPermissions = TotalPermissions().GetPermissions();
		}

		public PermissionManager(PermissionList permissions, PermissionList negatedPermissions, PermissionList neverPermissions, List<IPermissionManager> parents)
		{
			this.permissions = permissions;
			this.negatedPermissions = negatedPermissions;
			this.neverPermissions = neverPermissions;
			this.parents = parents;
			currentAllowPermissions = TotalPermissions().GetPermissions();
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
			perms.RemoveAll(p => GetNegatedPermissions().GetPermissions().Contains(p));
			perms.RemoveAll(p => GetNeverPermissions().GetPermissions().Contains(p));
			return new PermissionList(perms);
		}
	}
}
