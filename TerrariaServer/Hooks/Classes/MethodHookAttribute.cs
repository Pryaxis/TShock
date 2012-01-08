using System;

namespace TerrariaServer.Hooks.Classes
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	internal class MethodHookAttribute : Attribute
	{
	}
}
