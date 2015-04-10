using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockAPI.PermissionSystem
{
	public interface IPermissionManager
	{
		void AddPermission(PermissionNode permission);
		void AddPermission(string permission);
		void RemovePermission(PermissionNode permission);
		void RemovePermission(string permission);
		bool HasPermission(PermissionNode permission);
		void Parse(String list);
		void Parse(List<String> list);
		IPermissionList GetPermissions(PermissionType type);
		void Clone(IPermissionManager manager);
	}
}
