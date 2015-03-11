using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockAPI.PermissionSystem
{
	public interface IPermissionManager
	{
		bool HasPermission(String permission);
		IPermissionList GetPermissions();
		IPermissionList GetNegatedPermissions();
		IPermissionList GetNeverPermissions();
		IPermissionList TotalPermissions();
		List<IPermissionManager> GetParents(); 
		void Refresh();
	}
}
