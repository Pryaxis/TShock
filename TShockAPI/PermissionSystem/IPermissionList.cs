using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockAPI.PermissionSystem
{
	public interface IPermissionList
	{
		List<String> GetPermissions();
		bool HasPermission(String permission);
		String ToString();
	}
}
