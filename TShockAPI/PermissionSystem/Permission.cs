using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TShockAPI.PermissionSystem
{
	public enum PermissionType
	{
		Normal,
		Negated,
		Allowed
	}

	public class PermissionNode
	{
		public String Permission { get; private set; }
		public PermissionType PermissionType { get; private set; }

		public PermissionNode(String permission, PermissionType type = PermissionType.Normal)
		{
			Permission = permission;
			PermissionType = type;
		}
	}
}
