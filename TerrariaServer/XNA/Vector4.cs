using System;
using System.Globalization;
[Serializable]
public struct Vector4 : IEquatable<Vector4>
{
	public float X;
	public float Y;
	public float Z;
	public float W;
	private static Vector4 _zero;
	private static Vector4 _one;
	private static Vector4 _unitX;
	private static Vector4 _unitY;
	private static Vector4 _unitZ;
	private static Vector4 _unitW;
	public static Vector4 Zero
	{
		get
		{
			return Vector4._zero;
		}
	}
	public static Vector4 One
	{
		get
		{
			return Vector4._one;
		}
	}
	public static Vector4 UnitX
	{
		get
		{
			return Vector4._unitX;
		}
	}
	public static Vector4 UnitY
	{
		get
		{
			return Vector4._unitY;
		}
	}
	public static Vector4 UnitZ
	{
		get
		{
			return Vector4._unitZ;
		}
	}
	public static Vector4 UnitW
	{
		get
		{
			return Vector4._unitW;
		}
	}
	public Vector4(float x, float y, float z, float w)
	{
		this.X = x;
		this.Y = y;
		this.Z = z;
		this.W = w;
	}
	public Vector4(Vector2 value, float z, float w)
	{
		this.X = value.X;
		this.Y = value.Y;
		this.Z = z;
		this.W = w;
	}
	public Vector4(float value)
	{
		this.W = value;
		this.Z = value;
		this.Y = value;
		this.X = value;
	}
	public override string ToString()
	{
		CultureInfo currentCulture = CultureInfo.CurrentCulture;
		return string.Format(currentCulture, "{{X:{0} Y:{1} Z:{2} W:{3}}}", new object[]
		{
			this.X.ToString(currentCulture), 
			this.Y.ToString(currentCulture), 
			this.Z.ToString(currentCulture), 
			this.W.ToString(currentCulture)
		});
	}
	public bool Equals(Vector4 other)
	{
		return this.X == other.X && this.Y == other.Y && this.Z == other.Z && this.W == other.W;
	}
	public override bool Equals(object obj)
	{
		bool result = false;
		if (obj is Vector4)
		{
			result = this.Equals((Vector4)obj);
		}
		return result;
	}
	public override int GetHashCode()
	{
		return this.X.GetHashCode() + this.Y.GetHashCode() + this.Z.GetHashCode() + this.W.GetHashCode();
	}
	public float Length()
	{
		float num = this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W;
		return (float)Math.Sqrt((double)num);
	}
	public float LengthSquared()
	{
		return this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W;
	}
	public static float Distance(Vector4 value1, Vector4 value2)
	{
		float num = value1.X - value2.X;
		float num2 = value1.Y - value2.Y;
		float num3 = value1.Z - value2.Z;
		float num4 = value1.W - value2.W;
		float num5 = num * num + num2 * num2 + num3 * num3 + num4 * num4;
		return (float)Math.Sqrt((double)num5);
	}
	public static void Distance(ref Vector4 value1, ref Vector4 value2, out float result)
	{
		float num = value1.X - value2.X;
		float num2 = value1.Y - value2.Y;
		float num3 = value1.Z - value2.Z;
		float num4 = value1.W - value2.W;
		float num5 = num * num + num2 * num2 + num3 * num3 + num4 * num4;
		result = (float)Math.Sqrt((double)num5);
	}
	public static float DistanceSquared(Vector4 value1, Vector4 value2)
	{
		float num = value1.X - value2.X;
		float num2 = value1.Y - value2.Y;
		float num3 = value1.Z - value2.Z;
		float num4 = value1.W - value2.W;
		return num * num + num2 * num2 + num3 * num3 + num4 * num4;
	}
	public static void DistanceSquared(ref Vector4 value1, ref Vector4 value2, out float result)
	{
		float num = value1.X - value2.X;
		float num2 = value1.Y - value2.Y;
		float num3 = value1.Z - value2.Z;
		float num4 = value1.W - value2.W;
		result = num * num + num2 * num2 + num3 * num3 + num4 * num4;
	}
	public static float Dot(Vector4 vector1, Vector4 vector2)
	{
		return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z + vector1.W * vector2.W;
	}
	public static void Dot(ref Vector4 vector1, ref Vector4 vector2, out float result)
	{
		result = vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z + vector1.W * vector2.W;
	}
	public void Normalize()
	{
		float num = this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W;
		float num2 = 1f / (float)Math.Sqrt((double)num);
		this.X *= num2;
		this.Y *= num2;
		this.Z *= num2;
		this.W *= num2;
	}
	public static Vector4 Normalize(Vector4 vector)
	{
		float num = vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z + vector.W * vector.W;
		float num2 = 1f / (float)Math.Sqrt((double)num);
		Vector4 result;
		result.X = vector.X * num2;
		result.Y = vector.Y * num2;
		result.Z = vector.Z * num2;
		result.W = vector.W * num2;
		return result;
	}
	public static void Normalize(ref Vector4 vector, out Vector4 result)
	{
		float num = vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z + vector.W * vector.W;
		float num2 = 1f / (float)Math.Sqrt((double)num);
		result.X = vector.X * num2;
		result.Y = vector.Y * num2;
		result.Z = vector.Z * num2;
		result.W = vector.W * num2;
	}
	public static Vector4 Min(Vector4 value1, Vector4 value2)
	{
		Vector4 result;
		result.X = ((value1.X < value2.X) ? value1.X : value2.X);
		result.Y = ((value1.Y < value2.Y) ? value1.Y : value2.Y);
		result.Z = ((value1.Z < value2.Z) ? value1.Z : value2.Z);
		result.W = ((value1.W < value2.W) ? value1.W : value2.W);
		return result;
	}
	public static void Min(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
	{
		result.X = ((value1.X < value2.X) ? value1.X : value2.X);
		result.Y = ((value1.Y < value2.Y) ? value1.Y : value2.Y);
		result.Z = ((value1.Z < value2.Z) ? value1.Z : value2.Z);
		result.W = ((value1.W < value2.W) ? value1.W : value2.W);
	}
	public static Vector4 Max(Vector4 value1, Vector4 value2)
	{
		Vector4 result;
		result.X = ((value1.X > value2.X) ? value1.X : value2.X);
		result.Y = ((value1.Y > value2.Y) ? value1.Y : value2.Y);
		result.Z = ((value1.Z > value2.Z) ? value1.Z : value2.Z);
		result.W = ((value1.W > value2.W) ? value1.W : value2.W);
		return result;
	}
	public static void Max(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
	{
		result.X = ((value1.X > value2.X) ? value1.X : value2.X);
		result.Y = ((value1.Y > value2.Y) ? value1.Y : value2.Y);
		result.Z = ((value1.Z > value2.Z) ? value1.Z : value2.Z);
		result.W = ((value1.W > value2.W) ? value1.W : value2.W);
	}
	public static Vector4 Clamp(Vector4 value1, Vector4 min, Vector4 max)
	{
		float num = value1.X;
		num = ((num > max.X) ? max.X : num);
		num = ((num < min.X) ? min.X : num);
		float num2 = value1.Y;
		num2 = ((num2 > max.Y) ? max.Y : num2);
		num2 = ((num2 < min.Y) ? min.Y : num2);
		float num3 = value1.Z;
		num3 = ((num3 > max.Z) ? max.Z : num3);
		num3 = ((num3 < min.Z) ? min.Z : num3);
		float num4 = value1.W;
		num4 = ((num4 > max.W) ? max.W : num4);
		num4 = ((num4 < min.W) ? min.W : num4);
		Vector4 result;
		result.X = num;
		result.Y = num2;
		result.Z = num3;
		result.W = num4;
		return result;
	}
	public static void Clamp(ref Vector4 value1, ref Vector4 min, ref Vector4 max, out Vector4 result)
	{
		float num = value1.X;
		num = ((num > max.X) ? max.X : num);
		num = ((num < min.X) ? min.X : num);
		float num2 = value1.Y;
		num2 = ((num2 > max.Y) ? max.Y : num2);
		num2 = ((num2 < min.Y) ? min.Y : num2);
		float num3 = value1.Z;
		num3 = ((num3 > max.Z) ? max.Z : num3);
		num3 = ((num3 < min.Z) ? min.Z : num3);
		float num4 = value1.W;
		num4 = ((num4 > max.W) ? max.W : num4);
		num4 = ((num4 < min.W) ? min.W : num4);
		result.X = num;
		result.Y = num2;
		result.Z = num3;
		result.W = num4;
	}
	public static Vector4 Lerp(Vector4 value1, Vector4 value2, float amount)
	{
		Vector4 result;
		result.X = value1.X + (value2.X - value1.X) * amount;
		result.Y = value1.Y + (value2.Y - value1.Y) * amount;
		result.Z = value1.Z + (value2.Z - value1.Z) * amount;
		result.W = value1.W + (value2.W - value1.W) * amount;
		return result;
	}
	public static void Lerp(ref Vector4 value1, ref Vector4 value2, float amount, out Vector4 result)
	{
		result.X = value1.X + (value2.X - value1.X) * amount;
		result.Y = value1.Y + (value2.Y - value1.Y) * amount;
		result.Z = value1.Z + (value2.Z - value1.Z) * amount;
		result.W = value1.W + (value2.W - value1.W) * amount;
	}
	public static Vector4 Barycentric(Vector4 value1, Vector4 value2, Vector4 value3, float amount1, float amount2)
	{
		Vector4 result;
		result.X = value1.X + amount1 * (value2.X - value1.X) + amount2 * (value3.X - value1.X);
		result.Y = value1.Y + amount1 * (value2.Y - value1.Y) + amount2 * (value3.Y - value1.Y);
		result.Z = value1.Z + amount1 * (value2.Z - value1.Z) + amount2 * (value3.Z - value1.Z);
		result.W = value1.W + amount1 * (value2.W - value1.W) + amount2 * (value3.W - value1.W);
		return result;
	}
	public static void Barycentric(ref Vector4 value1, ref Vector4 value2, ref Vector4 value3, float amount1, float amount2, out Vector4 result)
	{
		result.X = value1.X + amount1 * (value2.X - value1.X) + amount2 * (value3.X - value1.X);
		result.Y = value1.Y + amount1 * (value2.Y - value1.Y) + amount2 * (value3.Y - value1.Y);
		result.Z = value1.Z + amount1 * (value2.Z - value1.Z) + amount2 * (value3.Z - value1.Z);
		result.W = value1.W + amount1 * (value2.W - value1.W) + amount2 * (value3.W - value1.W);
	}
	public static Vector4 SmoothStep(Vector4 value1, Vector4 value2, float amount)
	{
		amount = ((amount > 1f) ? 1f : ((amount < 0f) ? 0f : amount));
		amount = amount * amount * (3f - 2f * amount);
		Vector4 result;
		result.X = value1.X + (value2.X - value1.X) * amount;
		result.Y = value1.Y + (value2.Y - value1.Y) * amount;
		result.Z = value1.Z + (value2.Z - value1.Z) * amount;
		result.W = value1.W + (value2.W - value1.W) * amount;
		return result;
	}
	public static void SmoothStep(ref Vector4 value1, ref Vector4 value2, float amount, out Vector4 result)
	{
		amount = ((amount > 1f) ? 1f : ((amount < 0f) ? 0f : amount));
		amount = amount * amount * (3f - 2f * amount);
		result.X = value1.X + (value2.X - value1.X) * amount;
		result.Y = value1.Y + (value2.Y - value1.Y) * amount;
		result.Z = value1.Z + (value2.Z - value1.Z) * amount;
		result.W = value1.W + (value2.W - value1.W) * amount;
	}
	public static Vector4 CatmullRom(Vector4 value1, Vector4 value2, Vector4 value3, Vector4 value4, float amount)
	{
		float num = amount * amount;
		float num2 = amount * num;
		Vector4 result;
		result.X = 0.5f * (2f * value2.X + (-value1.X + value3.X) * amount + (2f * value1.X - 5f * value2.X + 4f * value3.X - value4.X) * num + (-value1.X + 3f * value2.X - 3f * value3.X + value4.X) * num2);
		result.Y = 0.5f * (2f * value2.Y + (-value1.Y + value3.Y) * amount + (2f * value1.Y - 5f * value2.Y + 4f * value3.Y - value4.Y) * num + (-value1.Y + 3f * value2.Y - 3f * value3.Y + value4.Y) * num2);
		result.Z = 0.5f * (2f * value2.Z + (-value1.Z + value3.Z) * amount + (2f * value1.Z - 5f * value2.Z + 4f * value3.Z - value4.Z) * num + (-value1.Z + 3f * value2.Z - 3f * value3.Z + value4.Z) * num2);
		result.W = 0.5f * (2f * value2.W + (-value1.W + value3.W) * amount + (2f * value1.W - 5f * value2.W + 4f * value3.W - value4.W) * num + (-value1.W + 3f * value2.W - 3f * value3.W + value4.W) * num2);
		return result;
	}
	public static void CatmullRom(ref Vector4 value1, ref Vector4 value2, ref Vector4 value3, ref Vector4 value4, float amount, out Vector4 result)
	{
		float num = amount * amount;
		float num2 = amount * num;
		result.X = 0.5f * (2f * value2.X + (-value1.X + value3.X) * amount + (2f * value1.X - 5f * value2.X + 4f * value3.X - value4.X) * num + (-value1.X + 3f * value2.X - 3f * value3.X + value4.X) * num2);
		result.Y = 0.5f * (2f * value2.Y + (-value1.Y + value3.Y) * amount + (2f * value1.Y - 5f * value2.Y + 4f * value3.Y - value4.Y) * num + (-value1.Y + 3f * value2.Y - 3f * value3.Y + value4.Y) * num2);
		result.Z = 0.5f * (2f * value2.Z + (-value1.Z + value3.Z) * amount + (2f * value1.Z - 5f * value2.Z + 4f * value3.Z - value4.Z) * num + (-value1.Z + 3f * value2.Z - 3f * value3.Z + value4.Z) * num2);
		result.W = 0.5f * (2f * value2.W + (-value1.W + value3.W) * amount + (2f * value1.W - 5f * value2.W + 4f * value3.W - value4.W) * num + (-value1.W + 3f * value2.W - 3f * value3.W + value4.W) * num2);
	}
	public static Vector4 Hermite(Vector4 value1, Vector4 tangent1, Vector4 value2, Vector4 tangent2, float amount)
	{
		float num = amount * amount;
		float num2 = amount * num;
		float num3 = 2f * num2 - 3f * num + 1f;
		float num4 = -2f * num2 + 3f * num;
		float num5 = num2 - 2f * num + amount;
		float num6 = num2 - num;
		Vector4 result;
		result.X = value1.X * num3 + value2.X * num4 + tangent1.X * num5 + tangent2.X * num6;
		result.Y = value1.Y * num3 + value2.Y * num4 + tangent1.Y * num5 + tangent2.Y * num6;
		result.Z = value1.Z * num3 + value2.Z * num4 + tangent1.Z * num5 + tangent2.Z * num6;
		result.W = value1.W * num3 + value2.W * num4 + tangent1.W * num5 + tangent2.W * num6;
		return result;
	}
	public static void Hermite(ref Vector4 value1, ref Vector4 tangent1, ref Vector4 value2, ref Vector4 tangent2, float amount, out Vector4 result)
	{
		float num = amount * amount;
		float num2 = amount * num;
		float num3 = 2f * num2 - 3f * num + 1f;
		float num4 = -2f * num2 + 3f * num;
		float num5 = num2 - 2f * num + amount;
		float num6 = num2 - num;
		result.X = value1.X * num3 + value2.X * num4 + tangent1.X * num5 + tangent2.X * num6;
		result.Y = value1.Y * num3 + value2.Y * num4 + tangent1.Y * num5 + tangent2.Y * num6;
		result.Z = value1.Z * num3 + value2.Z * num4 + tangent1.Z * num5 + tangent2.Z * num6;
		result.W = value1.W * num3 + value2.W * num4 + tangent1.W * num5 + tangent2.W * num6;
	}
	public static Vector4 Negate(Vector4 value)
	{
		Vector4 result;
		result.X = -value.X;
		result.Y = -value.Y;
		result.Z = -value.Z;
		result.W = -value.W;
		return result;
	}
	public static void Negate(ref Vector4 value, out Vector4 result)
	{
		result.X = -value.X;
		result.Y = -value.Y;
		result.Z = -value.Z;
		result.W = -value.W;
	}
	public static Vector4 Add(Vector4 value1, Vector4 value2)
	{
		Vector4 result;
		result.X = value1.X + value2.X;
		result.Y = value1.Y + value2.Y;
		result.Z = value1.Z + value2.Z;
		result.W = value1.W + value2.W;
		return result;
	}
	public static void Add(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
	{
		result.X = value1.X + value2.X;
		result.Y = value1.Y + value2.Y;
		result.Z = value1.Z + value2.Z;
		result.W = value1.W + value2.W;
	}
	public static Vector4 Subtract(Vector4 value1, Vector4 value2)
	{
		Vector4 result;
		result.X = value1.X - value2.X;
		result.Y = value1.Y - value2.Y;
		result.Z = value1.Z - value2.Z;
		result.W = value1.W - value2.W;
		return result;
	}
	public static void Subtract(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
	{
		result.X = value1.X - value2.X;
		result.Y = value1.Y - value2.Y;
		result.Z = value1.Z - value2.Z;
		result.W = value1.W - value2.W;
	}
	public static Vector4 Multiply(Vector4 value1, Vector4 value2)
	{
		Vector4 result;
		result.X = value1.X * value2.X;
		result.Y = value1.Y * value2.Y;
		result.Z = value1.Z * value2.Z;
		result.W = value1.W * value2.W;
		return result;
	}
	public static void Multiply(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
	{
		result.X = value1.X * value2.X;
		result.Y = value1.Y * value2.Y;
		result.Z = value1.Z * value2.Z;
		result.W = value1.W * value2.W;
	}
	public static Vector4 Multiply(Vector4 value1, float scaleFactor)
	{
		Vector4 result;
		result.X = value1.X * scaleFactor;
		result.Y = value1.Y * scaleFactor;
		result.Z = value1.Z * scaleFactor;
		result.W = value1.W * scaleFactor;
		return result;
	}
	public static void Multiply(ref Vector4 value1, float scaleFactor, out Vector4 result)
	{
		result.X = value1.X * scaleFactor;
		result.Y = value1.Y * scaleFactor;
		result.Z = value1.Z * scaleFactor;
		result.W = value1.W * scaleFactor;
	}
	public static Vector4 Divide(Vector4 value1, Vector4 value2)
	{
		Vector4 result;
		result.X = value1.X / value2.X;
		result.Y = value1.Y / value2.Y;
		result.Z = value1.Z / value2.Z;
		result.W = value1.W / value2.W;
		return result;
	}
	public static void Divide(ref Vector4 value1, ref Vector4 value2, out Vector4 result)
	{
		result.X = value1.X / value2.X;
		result.Y = value1.Y / value2.Y;
		result.Z = value1.Z / value2.Z;
		result.W = value1.W / value2.W;
	}
	public static Vector4 Divide(Vector4 value1, float divider)
	{
		float num = 1f / divider;
		Vector4 result;
		result.X = value1.X * num;
		result.Y = value1.Y * num;
		result.Z = value1.Z * num;
		result.W = value1.W * num;
		return result;
	}
	public static void Divide(ref Vector4 value1, float divider, out Vector4 result)
	{
		float num = 1f / divider;
		result.X = value1.X * num;
		result.Y = value1.Y * num;
		result.Z = value1.Z * num;
		result.W = value1.W * num;
	}
	public static Vector4 operator -(Vector4 value)
	{
		Vector4 result;
		result.X = -value.X;
		result.Y = -value.Y;
		result.Z = -value.Z;
		result.W = -value.W;
		return result;
	}
	public static bool operator ==(Vector4 value1, Vector4 value2)
	{
		return value1.X == value2.X && value1.Y == value2.Y && value1.Z == value2.Z && value1.W == value2.W;
	}
	public static bool operator !=(Vector4 value1, Vector4 value2)
	{
		return value1.X != value2.X || value1.Y != value2.Y || value1.Z != value2.Z || value1.W != value2.W;
	}
	public static Vector4 operator +(Vector4 value1, Vector4 value2)
	{
		Vector4 result;
		result.X = value1.X + value2.X;
		result.Y = value1.Y + value2.Y;
		result.Z = value1.Z + value2.Z;
		result.W = value1.W + value2.W;
		return result;
	}
	public static Vector4 operator -(Vector4 value1, Vector4 value2)
	{
		Vector4 result;
		result.X = value1.X - value2.X;
		result.Y = value1.Y - value2.Y;
		result.Z = value1.Z - value2.Z;
		result.W = value1.W - value2.W;
		return result;
	}
	public static Vector4 operator *(Vector4 value1, Vector4 value2)
	{
		Vector4 result;
		result.X = value1.X * value2.X;
		result.Y = value1.Y * value2.Y;
		result.Z = value1.Z * value2.Z;
		result.W = value1.W * value2.W;
		return result;
	}
	public static Vector4 operator *(Vector4 value1, float scaleFactor)
	{
		Vector4 result;
		result.X = value1.X * scaleFactor;
		result.Y = value1.Y * scaleFactor;
		result.Z = value1.Z * scaleFactor;
		result.W = value1.W * scaleFactor;
		return result;
	}
	public static Vector4 operator *(float scaleFactor, Vector4 value1)
	{
		Vector4 result;
		result.X = value1.X * scaleFactor;
		result.Y = value1.Y * scaleFactor;
		result.Z = value1.Z * scaleFactor;
		result.W = value1.W * scaleFactor;
		return result;
	}
	public static Vector4 operator /(Vector4 value1, Vector4 value2)
	{
		Vector4 result;
		result.X = value1.X / value2.X;
		result.Y = value1.Y / value2.Y;
		result.Z = value1.Z / value2.Z;
		result.W = value1.W / value2.W;
		return result;
	}
	public static Vector4 operator /(Vector4 value1, float divider)
	{
		float num = 1f / divider;
		Vector4 result;
		result.X = value1.X * num;
		result.Y = value1.Y * num;
		result.Z = value1.Z * num;
		result.W = value1.W * num;
		return result;
	}
	static Vector4()
	{
		Vector4._zero = default(Vector4);
		Vector4._one = new Vector4(1f, 1f, 1f, 1f);
		Vector4._unitX = new Vector4(1f, 0f, 0f, 0f);
		Vector4._unitY = new Vector4(0f, 1f, 0f, 0f);
		Vector4._unitZ = new Vector4(0f, 0f, 1f, 0f);
		Vector4._unitW = new Vector4(0f, 0f, 0f, 1f);
	}
}
