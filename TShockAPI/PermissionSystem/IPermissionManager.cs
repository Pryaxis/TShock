using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockAPI.PermissionSystem
{
	public interface IPermissionManager
	{
		void AddPermission(String permission);
		void RemovePermission(String permission);
		bool HasPermission(String permission);
		IPermissionList GetPermissions();
		IPermissionList GetNegatedPermissions();
		IPermissionList GetNeverPermissions();
		IPermissionList TotalPermissions();
		void AddParent(IPermissionManager parent);
		void RemoveParent(IPermissionManager parent);
		List<IPermissionManager> GetParents(); 
		void Refresh();
	}
}
