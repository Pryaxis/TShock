using System;
namespace Hooks.Classes
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	internal class MethodHookAttribute : Attribute
	{
	}
}
