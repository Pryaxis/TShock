using System;
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class APIVersionAttribute : Attribute
{
	public Version ApiVersion;
	public APIVersionAttribute(Version version)
	{
		this.ApiVersion = version;
	}
	public APIVersionAttribute(int major, int minor) : this(new Version(major, minor))
	{
	}
}
