using System;
using System.Globalization;
[Serializable]
public struct Rectangle : IEquatable<Rectangle>
{
	public int X;
	public int Y;
	public int Width;
	public int Height;
	private static Rectangle _empty;
	public int Left
	{
		get
		{
			return this.X;
		}
	}
	public int Right
	{
		get
		{
			return this.X + this.Width;
		}
	}
	public int Top
	{
		get
		{
			return this.Y;
		}
	}
	public int Bottom
	{
		get
		{
			return this.Y + this.Height;
		}
	}
	public Point Location
	{
		get
		{
			return new Point(this.X, this.Y);
		}
		set
		{
			this.X = value.X;
			this.Y = value.Y;
		}
	}
	public Point Center
	{
		get
		{
			return new Point(this.X + this.Width / 2, this.Y + this.Height / 2);
		}
	}
	public static Rectangle Empty
	{
		get
		{
			return Rectangle._empty;
		}
	}
	public bool IsEmpty
	{
		get
		{
			return this.Width == 0 && this.Height == 0 && this.X == 0 && this.Y == 0;
		}
	}
	public Rectangle(int x, int y, int width, int height)
	{
		this.X = x;
		this.Y = y;
		this.Width = width;
		this.Height = height;
	}
	public void Offset(Point amount)
	{
		this.X += amount.X;
		this.Y += amount.Y;
	}
	public void Offset(int offsetX, int offsetY)
	{
		this.X += offsetX;
		this.Y += offsetY;
	}
	public void Inflate(int horizontalAmount, int verticalAmount)
	{
		this.X -= horizontalAmount;
		this.Y -= verticalAmount;
		this.Width += horizontalAmount * 2;
		this.Height += verticalAmount * 2;
	}
	public bool Contains(int x, int y)
	{
		return this.X <= x && x < this.X + this.Width && this.Y <= y && y < this.Y + this.Height;
	}
	public bool Contains(Point value)
	{
		return this.X <= value.X && value.X < this.X + this.Width && this.Y <= value.Y && value.Y < this.Y + this.Height;
	}
	public void Contains(ref Point value, out bool result)
	{
		result = (this.X <= value.X && value.X < this.X + this.Width && this.Y <= value.Y && value.Y < this.Y + this.Height);
	}
	public bool Contains(Rectangle value)
	{
		return this.X <= value.X && value.X + value.Width <= this.X + this.Width && this.Y <= value.Y && value.Y + value.Height <= this.Y + this.Height;
	}
	public void Contains(ref Rectangle value, out bool result)
	{
		result = (this.X <= value.X && value.X + value.Width <= this.X + this.Width && this.Y <= value.Y && value.Y + value.Height <= this.Y + this.Height);
	}
	public bool Intersects(Rectangle value)
	{
		return value.X < this.X + this.Width && this.X < value.X + value.Width && value.Y < this.Y + this.Height && this.Y < value.Y + value.Height;
	}
	public void Intersects(ref Rectangle value, out bool result)
	{
		result = (value.X < this.X + this.Width && this.X < value.X + value.Width && value.Y < this.Y + this.Height && this.Y < value.Y + value.Height);
	}
	public static Rectangle Intersect(Rectangle value1, Rectangle value2)
	{
		int num = value1.X + value1.Width;
		int num2 = value2.X + value2.Width;
		int num3 = value1.Y + value1.Height;
		int num4 = value2.Y + value2.Height;
		int num5 = (value1.X > value2.X) ? value1.X : value2.X;
		int num6 = (value1.Y > value2.Y) ? value1.Y : value2.Y;
		int num7 = (num < num2) ? num : num2;
		int num8 = (num3 < num4) ? num3 : num4;
		Rectangle result;
		if (num7 > num5 && num8 > num6)
		{
			result.X = num5;
			result.Y = num6;
			result.Width = num7 - num5;
			result.Height = num8 - num6;
			return result;
		}
		result.X = 0;
		result.Y = 0;
		result.Width = 0;
		result.Height = 0;
		return result;
	}
	public static void Intersect(ref Rectangle value1, ref Rectangle value2, out Rectangle result)
	{
		int num = value1.X + value1.Width;
		int num2 = value2.X + value2.Width;
		int num3 = value1.Y + value1.Height;
		int num4 = value2.Y + value2.Height;
		int num5 = (value1.X > value2.X) ? value1.X : value2.X;
		int num6 = (value1.Y > value2.Y) ? value1.Y : value2.Y;
		int num7 = (num < num2) ? num : num2;
		int num8 = (num3 < num4) ? num3 : num4;
		if (num7 > num5 && num8 > num6)
		{
			result.X = num5;
			result.Y = num6;
			result.Width = num7 - num5;
			result.Height = num8 - num6;
			return;
		}
		result.X = 0;
		result.Y = 0;
		result.Width = 0;
		result.Height = 0;
	}
	public static Rectangle Union(Rectangle value1, Rectangle value2)
	{
		int num = value1.X + value1.Width;
		int num2 = value2.X + value2.Width;
		int num3 = value1.Y + value1.Height;
		int num4 = value2.Y + value2.Height;
		int num5 = (value1.X < value2.X) ? value1.X : value2.X;
		int num6 = (value1.Y < value2.Y) ? value1.Y : value2.Y;
		int num7 = (num > num2) ? num : num2;
		int num8 = (num3 > num4) ? num3 : num4;
		Rectangle result;
		result.X = num5;
		result.Y = num6;
		result.Width = num7 - num5;
		result.Height = num8 - num6;
		return result;
	}
	public static void Union(ref Rectangle value1, ref Rectangle value2, out Rectangle result)
	{
		int num = value1.X + value1.Width;
		int num2 = value2.X + value2.Width;
		int num3 = value1.Y + value1.Height;
		int num4 = value2.Y + value2.Height;
		int num5 = (value1.X < value2.X) ? value1.X : value2.X;
		int num6 = (value1.Y < value2.Y) ? value1.Y : value2.Y;
		int num7 = (num > num2) ? num : num2;
		int num8 = (num3 > num4) ? num3 : num4;
		result.X = num5;
		result.Y = num6;
		result.Width = num7 - num5;
		result.Height = num8 - num6;
	}
	public bool Equals(Rectangle other)
	{
		return this.X == other.X && this.Y == other.Y && this.Width == other.Width && this.Height == other.Height;
	}
	public override bool Equals(object obj)
	{
		bool result = false;
		if (obj is Rectangle)
		{
			result = this.Equals((Rectangle)obj);
		}
		return result;
	}
	public override string ToString()
	{
		CultureInfo currentCulture = CultureInfo.CurrentCulture;
		return string.Format(currentCulture, "{{X:{0} Y:{1} Width:{2} Height:{3}}}", new object[]
		{
			this.X.ToString(currentCulture), 
			this.Y.ToString(currentCulture), 
			this.Width.ToString(currentCulture), 
			this.Height.ToString(currentCulture)
		});
	}
	public override int GetHashCode()
	{
		return this.X.GetHashCode() + this.Y.GetHashCode() + this.Width.GetHashCode() + this.Height.GetHashCode();
	}
	public static bool operator ==(Rectangle a, Rectangle b)
	{
		return a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height;
	}
	public static bool operator !=(Rectangle a, Rectangle b)
	{
		return a.X != b.X || a.Y != b.Y || a.Width != b.Width || a.Height != b.Height;
	}
	static Rectangle()
	{
		Rectangle._empty = default(Rectangle);
	}
}
