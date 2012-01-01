using System;
using System.Globalization;
[Serializable]
public struct Vector2 : IEquatable<Vector2>
{
	public float X;
	public float Y;
	private static Vector2 _zero;
	private static Vector2 _one;
	private static Vector2 _unitX;
	private static Vector2 _unitY;
	public static Vector2 Zero
	{
		get
		{
			return Vector2._zero;
		}
	}
	public static Vector2 One
	{
		get
		{
			return Vector2._one;
		}
	}
	public static Vector2 UnitX
	{
		get
		{
			return Vector2._unitX;
		}
	}
	public static Vector2 UnitY
	{
		get
		{
			return Vector2._unitY;
		}
	}
	public Vector2(float x, float y)
	{
		this.X = x;
		this.Y = y;
	}
	public Vector2(float value)
	{
		this.Y = value;
		this.X = value;
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
	public bool Equals(Vector2 other)
	{
		return this.X == other.X && this.Y == other.Y;
	}
	public override bool Equals(object obj)
	{
		bool result = false;
		if (obj is Vector2)
		{
			result = this.Equals((Vector2)obj);
		}
		return result;
	}
	public Vector2 Add(Vector2 value2)
	{
		Vector2 zero = Vector2.Zero;
		zero.X = this.X + value2.X;
		zero.Y = this.Y + value2.Y;
		return zero;
	}
	public Vector2 Multiply(float scaleFactor)
	{
		Vector2 zero = Vector2.Zero;
		zero.X = this.X * scaleFactor;
		zero.Y = this.Y * scaleFactor;
		return zero;
	}
	public override int GetHashCode()
	{
		return this.X.GetHashCode() + this.Y.GetHashCode();
	}
	public float Length()
	{
		float num = this.X * this.X + this.Y * this.Y;
		return (float)Math.Sqrt((double)num);
	}
	public float LengthSquared()
	{
		return this.X * this.X + this.Y * this.Y;
	}
	public static float Distance(Vector2 value1, Vector2 value2)
	{
		float num = value1.X - value2.X;
		float num2 = value1.Y - value2.Y;
		float num3 = num * num + num2 * num2;
		return (float)Math.Sqrt((double)num3);
	}
	public static void Distance(ref Vector2 value1, ref Vector2 value2, out float result)
	{
		float num = value1.X - value2.X;
		float num2 = value1.Y - value2.Y;
		float num3 = num * num + num2 * num2;
		result = (float)Math.Sqrt((double)num3);
	}
	public static float DistanceSquared(Vector2 value1, Vector2 value2)
	{
		float num = value1.X - value2.X;
		float num2 = value1.Y - value2.Y;
		return num * num + num2 * num2;
	}
	public static void DistanceSquared(ref Vector2 value1, ref Vector2 value2, out float result)
	{
		float num = value1.X - value2.X;
		float num2 = value1.Y - value2.Y;
		result = num * num + num2 * num2;
	}
	public static float Dot(Vector2 value1, Vector2 value2)
	{
		return value1.X * value2.X + value1.Y * value2.Y;
	}
	public static void Dot(ref Vector2 value1, ref Vector2 value2, out float result)
	{
		result = value1.X * value2.X + value1.Y * value2.Y;
	}
	public void Normalize()
	{
		float num = this.X * this.X + this.Y * this.Y;
		float num2 = 1f / (float)Math.Sqrt((double)num);
		this.X *= num2;
		this.Y *= num2;
	}
	public static Vector2 Normalize(Vector2 value)
	{
		float num = value.X * value.X + value.Y * value.Y;
		float num2 = 1f / (float)Math.Sqrt((double)num);
		Vector2 result;
		result.X = value.X * num2;
		result.Y = value.Y * num2;
		return result;
	}
	public static void Normalize(ref Vector2 value, out Vector2 result)
	{
		float num = value.X * value.X + value.Y * value.Y;
		float num2 = 1f / (float)Math.Sqrt((double)num);
		result.X = value.X * num2;
		result.Y = value.Y * num2;
	}
	public static Vector2 Reflect(Vector2 vector, Vector2 normal)
	{
		float num = vector.X * normal.X + vector.Y * normal.Y;
		Vector2 result;
		result.X = vector.X - 2f * num * normal.X;
		result.Y = vector.Y - 2f * num * normal.Y;
		return result;
	}
	public static void Reflect(ref Vector2 vector, ref Vector2 normal, out Vector2 result)
	{
		float num = vector.X * normal.X + vector.Y * normal.Y;
		result.X = vector.X - 2f * num * normal.X;
		result.Y = vector.Y - 2f * num * normal.Y;
	}
	public static Vector2 Min(Vector2 value1, Vector2 value2)
	{
		Vector2 result;
		result.X = ((value1.X < value2.X) ? value1.X : value2.X);
		result.Y = ((value1.Y < value2.Y) ? value1.Y : value2.Y);
		return result;
	}
	public static void Min(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
	{
		result.X = ((value1.X < value2.X) ? value1.X : value2.X);
		result.Y = ((value1.Y < value2.Y) ? value1.Y : value2.Y);
	}
	public static Vector2 Max(Vector2 value1, Vector2 value2)
	{
		Vector2 result;
		result.X = ((value1.X > value2.X) ? value1.X : value2.X);
		result.Y = ((value1.Y > value2.Y) ? value1.Y : value2.Y);
		return result;
	}
	public static void Max(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
	{
		result.X = ((value1.X > value2.X) ? value1.X : value2.X);
		result.Y = ((value1.Y > value2.Y) ? value1.Y : value2.Y);
	}
	public static Vector2 Clamp(Vector2 value1, Vector2 min, Vector2 max)
	{
		float num = value1.X;
		num = ((num > max.X) ? max.X : num);
		num = ((num < min.X) ? min.X : num);
		float num2 = value1.Y;
		num2 = ((num2 > max.Y) ? max.Y : num2);
		num2 = ((num2 < min.Y) ? min.Y : num2);
		Vector2 result;
		result.X = num;
		result.Y = num2;
		return result;
	}
	public static void Clamp(ref Vector2 value1, ref Vector2 min, ref Vector2 max, out Vector2 result)
	{
		float num = value1.X;
		num = ((num > max.X) ? max.X : num);
		num = ((num < min.X) ? min.X : num);
		float num2 = value1.Y;
		num2 = ((num2 > max.Y) ? max.Y : num2);
		num2 = ((num2 < min.Y) ? min.Y : num2);
		result.X = num;
		result.Y = num2;
	}
	public static Vector2 Lerp(Vector2 value1, Vector2 value2, float amount)
	{
		Vector2 result;
		result.X = value1.X + (value2.X - value1.X) * amount;
		result.Y = value1.Y + (value2.Y - value1.Y) * amount;
		return result;
	}
	public static void Lerp(ref Vector2 value1, ref Vector2 value2, float amount, out Vector2 result)
	{
		result.X = value1.X + (value2.X - value1.X) * amount;
		result.Y = value1.Y + (value2.Y - value1.Y) * amount;
	}
	public static Vector2 Barycentric(Vector2 value1, Vector2 value2, Vector2 value3, float amount1, float amount2)
	{
		Vector2 result;
		result.X = value1.X + amount1 * (value2.X - value1.X) + amount2 * (value3.X - value1.X);
		result.Y = value1.Y + amount1 * (value2.Y - value1.Y) + amount2 * (value3.Y - value1.Y);
		return result;
	}
	public static void Barycentric(ref Vector2 value1, ref Vector2 value2, ref Vector2 value3, float amount1, float amount2, out Vector2 result)
	{
		result.X = value1.X + amount1 * (value2.X - value1.X) + amount2 * (value3.X - value1.X);
		result.Y = value1.Y + amount1 * (value2.Y - value1.Y) + amount2 * (value3.Y - value1.Y);
	}
	public static Vector2 SmoothStep(Vector2 value1, Vector2 value2, float amount)
	{
		amount = ((amount > 1f) ? 1f : ((amount < 0f) ? 0f : amount));
		amount = amount * amount * (3f - 2f * amount);
		Vector2 result;
		result.X = value1.X + (value2.X - value1.X) * amount;
		result.Y = value1.Y + (value2.Y - value1.Y) * amount;
		return result;
	}
	public static void SmoothStep(ref Vector2 value1, ref Vector2 value2, float amount, out Vector2 result)
	{
		amount = ((amount > 1f) ? 1f : ((amount < 0f) ? 0f : amount));
		amount = amount * amount * (3f - 2f * amount);
		result.X = value1.X + (value2.X - value1.X) * amount;
		result.Y = value1.Y + (value2.Y - value1.Y) * amount;
	}
	public static Vector2 CatmullRom(Vector2 value1, Vector2 value2, Vector2 value3, Vector2 value4, float amount)
	{
		float num = amount * amount;
		float num2 = amount * num;
		Vector2 result;
		result.X = 0.5f * (2f * value2.X + (-value1.X + value3.X) * amount + (2f * value1.X - 5f * value2.X + 4f * value3.X - value4.X) * num + (-value1.X + 3f * value2.X - 3f * value3.X + value4.X) * num2);
		result.Y = 0.5f * (2f * value2.Y + (-value1.Y + value3.Y) * amount + (2f * value1.Y - 5f * value2.Y + 4f * value3.Y - value4.Y) * num + (-value1.Y + 3f * value2.Y - 3f * value3.Y + value4.Y) * num2);
		return result;
	}
	public static void CatmullRom(ref Vector2 value1, ref Vector2 value2, ref Vector2 value3, ref Vector2 value4, float amount, out Vector2 result)
	{
		float num = amount * amount;
		float num2 = amount * num;
		result.X = 0.5f * (2f * value2.X + (-value1.X + value3.X) * amount + (2f * value1.X - 5f * value2.X + 4f * value3.X - value4.X) * num + (-value1.X + 3f * value2.X - 3f * value3.X + value4.X) * num2);
		result.Y = 0.5f * (2f * value2.Y + (-value1.Y + value3.Y) * amount + (2f * value1.Y - 5f * value2.Y + 4f * value3.Y - value4.Y) * num + (-value1.Y + 3f * value2.Y - 3f * value3.Y + value4.Y) * num2);
	}
	public static Vector2 Hermite(Vector2 value1, Vector2 tangent1, Vector2 value2, Vector2 tangent2, float amount)
	{
		float num = amount * amount;
		float num2 = amount * num;
		float num3 = 2f * num2 - 3f * num + 1f;
		float num4 = -2f * num2 + 3f * num;
		float num5 = num2 - 2f * num + amount;
		float num6 = num2 - num;
		Vector2 result;
		result.X = value1.X * num3 + value2.X * num4 + tangent1.X * num5 + tangent2.X * num6;
		result.Y = value1.Y * num3 + value2.Y * num4 + tangent1.Y * num5 + tangent2.Y * num6;
		return result;
	}
	public static void Hermite(ref Vector2 value1, ref Vector2 tangent1, ref Vector2 value2, ref Vector2 tangent2, float amount, out Vector2 result)
	{
		float num = amount * amount;
		float num2 = amount * num;
		float num3 = 2f * num2 - 3f * num + 1f;
		float num4 = -2f * num2 + 3f * num;
		float num5 = num2 - 2f * num + amount;
		float num6 = num2 - num;
		result.X = value1.X * num3 + value2.X * num4 + tangent1.X * num5 + tangent2.X * num6;
		result.Y = value1.Y * num3 + value2.Y * num4 + tangent1.Y * num5 + tangent2.Y * num6;
	}
	public static Vector2 Negate(Vector2 value)
	{
		Vector2 result;
		result.X = -value.X;
		result.Y = -value.Y;
		return result;
	}
	public static void Negate(ref Vector2 value, out Vector2 result)
	{
		result.X = -value.X;
		result.Y = -value.Y;
	}
	public static Vector2 Add(Vector2 value1, Vector2 value2)
	{
		Vector2 result;
		result.X = value1.X + value2.X;
		result.Y = value1.Y + value2.Y;
		return result;
	}
	public static void Add(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
	{
		result.X = value1.X + value2.X;
		result.Y = value1.Y + value2.Y;
	}
	public static Vector2 Subtract(Vector2 value1, Vector2 value2)
	{
		Vector2 result;
		result.X = value1.X - value2.X;
		result.Y = value1.Y - value2.Y;
		return result;
	}
	public static void Subtract(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
	{
		result.X = value1.X - value2.X;
		result.Y = value1.Y - value2.Y;
	}
	public static Vector2 Multiply(Vector2 value1, Vector2 value2)
	{
		Vector2 result;
		result.X = value1.X * value2.X;
		result.Y = value1.Y * value2.Y;
		return result;
	}
	public static void Multiply(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
	{
		result.X = value1.X * value2.X;
		result.Y = value1.Y * value2.Y;
	}
	public static Vector2 Multiply(Vector2 value1, float scaleFactor)
	{
		Vector2 result;
		result.X = value1.X * scaleFactor;
		result.Y = value1.Y * scaleFactor;
		return result;
	}
	public static void Multiply(ref Vector2 value1, float scaleFactor, out Vector2 result)
	{
		result.X = value1.X * scaleFactor;
		result.Y = value1.Y * scaleFactor;
	}
	public static Vector2 Divide(Vector2 value1, Vector2 value2)
	{
		Vector2 result;
		result.X = value1.X / value2.X;
		result.Y = value1.Y / value2.Y;
		return result;
	}
	public static void Divide(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
	{
		result.X = value1.X / value2.X;
		result.Y = value1.Y / value2.Y;
	}
	public static Vector2 Divide(Vector2 value1, float divider)
	{
		float num = 1f / divider;
		Vector2 result;
		result.X = value1.X * num;
		result.Y = value1.Y * num;
		return result;
	}
	public static void Divide(ref Vector2 value1, float divider, out Vector2 result)
	{
		float num = 1f / divider;
		result.X = value1.X * num;
		result.Y = value1.Y * num;
	}
	public static Vector2 operator -(Vector2 value)
	{
		Vector2 result;
		result.X = -value.X;
		result.Y = -value.Y;
		return result;
	}
	public static bool operator ==(Vector2 value1, Vector2 value2)
	{
		return value1.X == value2.X && value1.Y == value2.Y;
	}
	public static bool operator !=(Vector2 value1, Vector2 value2)
	{
		return value1.X != value2.X || value1.Y != value2.Y;
	}
	public static Vector2 operator +(Vector2 value1, Vector2 value2)
	{
		Vector2 result;
		result.X = value1.X + value2.X;
		result.Y = value1.Y + value2.Y;
		return result;
	}
	public static Vector2 operator -(Vector2 value1, Vector2 value2)
	{
		Vector2 result;
		result.X = value1.X - value2.X;
		result.Y = value1.Y - value2.Y;
		return result;
	}
	public static Vector2 operator *(Vector2 value1, Vector2 value2)
	{
		Vector2 result;
		result.X = value1.X * value2.X;
		result.Y = value1.Y * value2.Y;
		return result;
	}
	public static Vector2 operator *(Vector2 value, float scaleFactor)
	{
		Vector2 result;
		result.X = value.X * scaleFactor;
		result.Y = value.Y * scaleFactor;
		return result;
	}
	public static Vector2 operator *(float scaleFactor, Vector2 value)
	{
		Vector2 result;
		result.X = value.X * scaleFactor;
		result.Y = value.Y * scaleFactor;
		return result;
	}
	public static Vector2 operator /(Vector2 value1, Vector2 value2)
	{
		Vector2 result;
		result.X = value1.X / value2.X;
		result.Y = value1.Y / value2.Y;
		return result;
	}
	public static Vector2 operator /(Vector2 value1, float divider)
	{
		float num = 1f / divider;
		Vector2 result;
		result.X = value1.X * num;
		result.Y = value1.Y * num;
		return result;
	}
	static Vector2()
	{
		Vector2._zero = Vector2.Zero;
		Vector2._one = new Vector2(1f, 1f);
		Vector2._unitX = new Vector2(1f, 0f);
		Vector2._unitY = new Vector2(0f, 1f);
	}
}
