using System;
using System.Globalization;
[Serializable]
public struct Point : IEquatable<Point>
{
	public int X;
	public int Y;
	private static Point _zero;
	public static Point Zero
	{
		get
		{
			return Point._zero;
		}
	}
	public Point(int x, int y)
	{
		this.X = x;
		this.Y = y;
	}
	public bool Equals(Point other)
	{
		return this.X == other.X && this.Y == other.Y;
	}
	public override bool Equals(object obj)
	{
		bool result = false;
		if (obj is Point)
		{
			result = this.Equals((Point)obj);
		}
		return result;
	}
	public override int GetHashCode()
	{
		return this.X.GetHashCode() + this.Y.GetHashCode();
	}
	public override string ToString()
	{
		CultureInfo currentCulture = CultureInfo.CurrentCulture;
		return string.Format(currentCulture, "{{X:{0} Y:{1}}}", new object[]
		{
			this.X.ToString(currentCulture), 
			this.Y.ToString(currentCulture)
		});
	}
	public static bool operator ==(Point a, Point b)
	{
		return a.Equals(b);
	}
	public static bool operator !=(Point a, Point b)
	{
		return a.X != b.X || a.Y != b.Y;
	}
	static Point()
	{
		Point._zero = default(Point);
	}
}
