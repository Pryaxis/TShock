using System;
using System.Globalization;
[Serializable]
public struct Color : IPackedVector<uint>, IPackedVector, IEquatable<Color>
{
	private uint packedValue;
	public byte R
	{
		get
		{
			return (byte)this.packedValue;
		}
		set
		{
			this.packedValue = ((this.packedValue & 4294967040u) | (uint)value);
		}
	}
	public byte G
	{
		get
		{
			return (byte)(this.packedValue >> 8);
		}
		set
		{
			this.packedValue = ((this.packedValue & 4294902015u) | (uint)((uint)value << 8));
		}
	}
	public byte B
	{
		get
		{
			return (byte)(this.packedValue >> 16);
		}
		set
		{
			this.packedValue = ((this.packedValue & 4278255615u) | (uint)((uint)value << 16));
		}
	}
	public byte A
	{
		get
		{
			return (byte)(this.packedValue >> 24);
		}
		set
		{
			this.packedValue = ((this.packedValue & 16777215u) | (uint)((uint)value << 24));
		}
	}
	[CLSCompliant(false)]
	public uint PackedValue
	{
		get
		{
			return this.packedValue;
		}
		set
		{
			this.packedValue = value;
		}
	}
	public static Color Transparent
	{
		get
		{
			return new Color(0u);
		}
	}
	public static Color AliceBlue
	{
		get
		{
			return new Color(4294965488u);
		}
	}
	public static Color AntiqueWhite
	{
		get
		{
			return new Color(4292340730u);
		}
	}
	public static Color Aqua
	{
		get
		{
			return new Color(4294967040u);
		}
	}
	public static Color Aquamarine
	{
		get
		{
			return new Color(4292149119u);
		}
	}
	public static Color Azure
	{
		get
		{
			return new Color(4294967280u);
		}
	}
	public static Color Beige
	{
		get
		{
			return new Color(4292670965u);
		}
	}
	public static Color Bisque
	{
		get
		{
			return new Color(4291093759u);
		}
	}
	public static Color Black
	{
		get
		{
			return new Color(4278190080u);
		}
	}
	public static Color BlanchedAlmond
	{
		get
		{
			return new Color(4291685375u);
		}
	}
	public static Color Blue
	{
		get
		{
			return new Color(4294901760u);
		}
	}
	public static Color BlueViolet
	{
		get
		{
			return new Color(4293012362u);
		}
	}
	public static Color Brown
	{
		get
		{
			return new Color(4280953509u);
		}
	}
	public static Color BurlyWood
	{
		get
		{
			return new Color(4287084766u);
		}
	}
	public static Color CadetBlue
	{
		get
		{
			return new Color(4288716383u);
		}
	}
	public static Color Chartreuse
	{
		get
		{
			return new Color(4278255487u);
		}
	}
	public static Color Chocolate
	{
		get
		{
			return new Color(4280183250u);
		}
	}
	public static Color Coral
	{
		get
		{
			return new Color(4283465727u);
		}
	}
	public static Color CornflowerBlue
	{
		get
		{
			return new Color(4293760356u);
		}
	}
	public static Color Cornsilk
	{
		get
		{
			return new Color(4292671743u);
		}
	}
	public static Color Crimson
	{
		get
		{
			return new Color(4282127580u);
		}
	}
	public static Color Cyan
	{
		get
		{
			return new Color(4294967040u);
		}
	}
	public static Color DarkBlue
	{
		get
		{
			return new Color(4287299584u);
		}
	}
	public static Color DarkCyan
	{
		get
		{
			return new Color(4287335168u);
		}
	}
	public static Color DarkGoldenrod
	{
		get
		{
			return new Color(4278945464u);
		}
	}
	public static Color DarkGray
	{
		get
		{
			return new Color(4289309097u);
		}
	}
	public static Color DarkGreen
	{
		get
		{
			return new Color(4278215680u);
		}
	}
	public static Color DarkKhaki
	{
		get
		{
			return new Color(4285249469u);
		}
	}
	public static Color DarkMagenta
	{
		get
		{
			return new Color(4287299723u);
		}
	}
	public static Color DarkOliveGreen
	{
		get
		{
			return new Color(4281297749u);
		}
	}
	public static Color DarkOrange
	{
		get
		{
			return new Color(4278226175u);
		}
	}
	public static Color DarkOrchid
	{
		get
		{
			return new Color(4291572377u);
		}
	}
	public static Color DarkRed
	{
		get
		{
			return new Color(4278190219u);
		}
	}
	public static Color DarkSalmon
	{
		get
		{
			return new Color(4286224105u);
		}
	}
	public static Color DarkSeaGreen
	{
		get
		{
			return new Color(4287347855u);
		}
	}
	public static Color DarkSlateBlue
	{
		get
		{
			return new Color(4287315272u);
		}
	}
	public static Color DarkSlateGray
	{
		get
		{
			return new Color(4283387695u);
		}
	}
	public static Color DarkTurquoise
	{
		get
		{
			return new Color(4291939840u);
		}
	}
	public static Color DarkViolet
	{
		get
		{
			return new Color(4292018324u);
		}
	}
	public static Color DeepPink
	{
		get
		{
			return new Color(4287829247u);
		}
	}
	public static Color DeepSkyBlue
	{
		get
		{
			return new Color(4294950656u);
		}
	}
	public static Color DimGray
	{
		get
		{
			return new Color(4285098345u);
		}
	}
	public static Color DodgerBlue
	{
		get
		{
			return new Color(4294938654u);
		}
	}
	public static Color Firebrick
	{
		get
		{
			return new Color(4280427186u);
		}
	}
	public static Color FloralWhite
	{
		get
		{
			return new Color(4293982975u);
		}
	}
	public static Color ForestGreen
	{
		get
		{
			return new Color(4280453922u);
		}
	}
	public static Color Fuchsia
	{
		get
		{
			return new Color(4294902015u);
		}
	}
	public static Color Gainsboro
	{
		get
		{
			return new Color(4292664540u);
		}
	}
	public static Color GhostWhite
	{
		get
		{
			return new Color(4294965496u);
		}
	}
	public static Color Gold
	{
		get
		{
			return new Color(4278245375u);
		}
	}
	public static Color Goldenrod
	{
		get
		{
			return new Color(4280329690u);
		}
	}
	public static Color Gray
	{
		get
		{
			return new Color(4286611584u);
		}
	}
	public static Color Green
	{
		get
		{
			return new Color(4278222848u);
		}
	}
	public static Color GreenYellow
	{
		get
		{
			return new Color(4281335725u);
		}
	}
	public static Color Honeydew
	{
		get
		{
			return new Color(4293984240u);
		}
	}
	public static Color HotPink
	{
		get
		{
			return new Color(4290013695u);
		}
	}
	public static Color IndianRed
	{
		get
		{
			return new Color(4284243149u);
		}
	}
	public static Color Indigo
	{
		get
		{
			return new Color(4286709835u);
		}
	}
	public static Color Ivory
	{
		get
		{
			return new Color(4293984255u);
		}
	}
	public static Color Khaki
	{
		get
		{
			return new Color(4287424240u);
		}
	}
	public static Color Lavender
	{
		get
		{
			return new Color(4294633190u);
		}
	}
	public static Color LavenderBlush
	{
		get
		{
			return new Color(4294308095u);
		}
	}
	public static Color LawnGreen
	{
		get
		{
			return new Color(4278254716u);
		}
	}
	public static Color LemonChiffon
	{
		get
		{
			return new Color(4291689215u);
		}
	}
	public static Color LightBlue
	{
		get
		{
			return new Color(4293318829u);
		}
	}
	public static Color LightCoral
	{
		get
		{
			return new Color(4286611696u);
		}
	}
	public static Color LightCyan
	{
		get
		{
			return new Color(4294967264u);
		}
	}
	public static Color LightGoldenrodYellow
	{
		get
		{
			return new Color(4292016890u);
		}
	}
	public static Color LightGreen
	{
		get
		{
			return new Color(4287688336u);
		}
	}
	public static Color LightGray
	{
		get
		{
			return new Color(4292072403u);
		}
	}
	public static Color LightPink
	{
		get
		{
			return new Color(4290885375u);
		}
	}
	public static Color LightSalmon
	{
		get
		{
			return new Color(4286226687u);
		}
	}
	public static Color LightSeaGreen
	{
		get
		{
			return new Color(4289376800u);
		}
	}
	public static Color LightSkyBlue
	{
		get
		{
			return new Color(4294626951u);
		}
	}
	public static Color LightSlateGray
	{
		get
		{
			return new Color(4288252023u);
		}
	}
	public static Color LightSteelBlue
	{
		get
		{
			return new Color(4292789424u);
		}
	}
	public static Color LightYellow
	{
		get
		{
			return new Color(4292935679u);
		}
	}
	public static Color Lime
	{
		get
		{
			return new Color(4278255360u);
		}
	}
	public static Color LimeGreen
	{
		get
		{
			return new Color(4281519410u);
		}
	}
	public static Color Linen
	{
		get
		{
			return new Color(4293325050u);
		}
	}
	public static Color Magenta
	{
		get
		{
			return new Color(4294902015u);
		}
	}
	public static Color Maroon
	{
		get
		{
			return new Color(4278190208u);
		}
	}
	public static Color MediumAquamarine
	{
		get
		{
			return new Color(4289383782u);
		}
	}
	public static Color MediumBlue
	{
		get
		{
			return new Color(4291624960u);
		}
	}
	public static Color MediumOrchid
	{
		get
		{
			return new Color(4292040122u);
		}
	}
	public static Color MediumPurple
	{
		get
		{
			return new Color(4292571283u);
		}
	}
	public static Color MediumSeaGreen
	{
		get
		{
			return new Color(4285641532u);
		}
	}
	public static Color MediumSlateBlue
	{
		get
		{
			return new Color(4293814395u);
		}
	}
	public static Color MediumSpringGreen
	{
		get
		{
			return new Color(4288346624u);
		}
	}
	public static Color MediumTurquoise
	{
		get
		{
			return new Color(4291613000u);
		}
	}
	public static Color MediumVioletRed
	{
		get
		{
			return new Color(4286911943u);
		}
	}
	public static Color MidnightBlue
	{
		get
		{
			return new Color(4285536537u);
		}
	}
	public static Color MintCream
	{
		get
		{
			return new Color(4294639605u);
		}
	}
	public static Color MistyRose
	{
		get
		{
			return new Color(4292994303u);
		}
	}
	public static Color Moccasin
	{
		get
		{
			return new Color(4290110719u);
		}
	}
	public static Color NavajoWhite
	{
		get
		{
			return new Color(4289584895u);
		}
	}
	public static Color Navy
	{
		get
		{
			return new Color(4286578688u);
		}
	}
	public static Color OldLace
	{
		get
		{
			return new Color(4293326333u);
		}
	}
	public static Color Olive
	{
		get
		{
			return new Color(4278222976u);
		}
	}
	public static Color OliveDrab
	{
		get
		{
			return new Color(4280520299u);
		}
	}
	public static Color Orange
	{
		get
		{
			return new Color(4278232575u);
		}
	}
	public static Color OrangeRed
	{
		get
		{
			return new Color(4278207999u);
		}
	}
	public static Color Orchid
	{
		get
		{
			return new Color(4292243674u);
		}
	}
	public static Color PaleGoldenrod
	{
		get
		{
			return new Color(4289390830u);
		}
	}
	public static Color PaleGreen
	{
		get
		{
			return new Color(4288215960u);
		}
	}
	public static Color PaleTurquoise
	{
		get
		{
			return new Color(4293848751u);
		}
	}
	public static Color PaleVioletRed
	{
		get
		{
			return new Color(4287852763u);
		}
	}
	public static Color PapayaWhip
	{
		get
		{
			return new Color(4292210687u);
		}
	}
	public static Color PeachPuff
	{
		get
		{
			return new Color(4290370303u);
		}
	}
	public static Color Peru
	{
		get
		{
			return new Color(4282353101u);
		}
	}
	public static Color Pink
	{
		get
		{
			return new Color(4291543295u);
		}
	}
	public static Color Plum
	{
		get
		{
			return new Color(4292714717u);
		}
	}
	public static Color PowderBlue
	{
		get
		{
			return new Color(4293320880u);
		}
	}
	public static Color Purple
	{
		get
		{
			return new Color(4286578816u);
		}
	}
	public static Color Red
	{
		get
		{
			return new Color(4278190335u);
		}
	}
	public static Color RosyBrown
	{
		get
		{
			return new Color(4287598524u);
		}
	}
	public static Color RoyalBlue
	{
		get
		{
			return new Color(4292962625u);
		}
	}
	public static Color SaddleBrown
	{
		get
		{
			return new Color(4279453067u);
		}
	}
	public static Color Salmon
	{
		get
		{
			return new Color(4285694202u);
		}
	}
	public static Color SandyBrown
	{
		get
		{
			return new Color(4284523764u);
		}
	}
	public static Color SeaGreen
	{
		get
		{
			return new Color(4283927342u);
		}
	}
	public static Color SeaShell
	{
		get
		{
			return new Color(4293850623u);
		}
	}
	public static Color Sienna
	{
		get
		{
			return new Color(4281160352u);
		}
	}
	public static Color Silver
	{
		get
		{
			return new Color(4290822336u);
		}
	}
	public static Color SkyBlue
	{
		get
		{
			return new Color(4293643911u);
		}
	}
	public static Color SlateBlue
	{
		get
		{
			return new Color(4291648106u);
		}
	}
	public static Color SlateGray
	{
		get
		{
			return new Color(4287660144u);
		}
	}
	public static Color Snow
	{
		get
		{
			return new Color(4294638335u);
		}
	}
	public static Color SpringGreen
	{
		get
		{
			return new Color(4286578432u);
		}
	}
	public static Color SteelBlue
	{
		get
		{
			return new Color(4290019910u);
		}
	}
	public static Color Tan
	{
		get
		{
			return new Color(4287411410u);
		}
	}
	public static Color Teal
	{
		get
		{
			return new Color(4286611456u);
		}
	}
	public static Color Thistle
	{
		get
		{
			return new Color(4292394968u);
		}
	}
	public static Color Tomato
	{
		get
		{
			return new Color(4282868735u);
		}
	}
	public static Color Turquoise
	{
		get
		{
			return new Color(4291878976u);
		}
	}
	public static Color Violet
	{
		get
		{
			return new Color(4293821166u);
		}
	}
	public static Color Wheat
	{
		get
		{
			return new Color(4289978101u);
		}
	}
	public static Color White
	{
		get
		{
			return new Color(4294967295u);
		}
	}
	public static Color WhiteSmoke
	{
		get
		{
			return new Color(4294309365u);
		}
	}
	public static Color Yellow
	{
		get
		{
			return new Color(4278255615u);
		}
	}
	public static Color YellowGreen
	{
		get
		{
			return new Color(4281519514u);
		}
	}
	private Color(uint packedValue)
	{
		this.packedValue = packedValue;
	}
	public Color(int r, int g, int b)
	{
		if (((r | g | b) & -256) != 0)
		{
			r = Color.ClampToByte64((long)r);
			g = Color.ClampToByte64((long)g);
			b = Color.ClampToByte64((long)b);
		}
		g <<= 8;
		b <<= 16;
		this.packedValue = (uint)(r | g | b | -16777216);
	}
	public Color(int r, int g, int b, int a)
	{
		if (((r | g | b | a) & -256) != 0)
		{
			r = Color.ClampToByte32(r);
			g = Color.ClampToByte32(g);
			b = Color.ClampToByte32(b);
			a = Color.ClampToByte32(a);
		}
		g <<= 8;
		b <<= 16;
		a <<= 24;
		this.packedValue = (uint)(r | g | b | a);
	}
	public Color(float r, float g, float b)
	{
		this.packedValue = Color.PackHelper(r, g, b, 1f);
	}
	public Color(float r, float g, float b, float a)
	{
		this.packedValue = Color.PackHelper(r, g, b, a);
	}
	public Color(Vector4 vector)
	{
		this.packedValue = Color.PackHelper(vector.X, vector.Y, vector.Z, vector.W);
	}
	void IPackedVector.PackFromVector4(Vector4 vector)
	{
		this.packedValue = Color.PackHelper(vector.X, vector.Y, vector.Z, vector.W);
	}
	public static Color FromNonPremultiplied(Vector4 vector)
	{
		Color result;
		result.packedValue = Color.PackHelper(vector.X * vector.W, vector.Y * vector.W, vector.Z * vector.W, vector.W);
		return result;
	}
	public static Color FromNonPremultiplied(int r, int g, int b, int a)
	{
		r = Color.ClampToByte64((long)(r * a) / 255L);
		g = Color.ClampToByte64((long)(g * a) / 255L);
		b = Color.ClampToByte64((long)(b * a) / 255L);
		a = Color.ClampToByte32(a);
		g <<= 8;
		b <<= 16;
		a <<= 24;
		Color result;
		result.packedValue = (uint)(r | g | b | a);
		return result;
	}
	private static uint PackHelper(float vectorX, float vectorY, float vectorZ, float vectorW)
	{
		uint num = PackUtils.PackUNorm(255f, vectorX);
		uint num2 = PackUtils.PackUNorm(255f, vectorY) << 8;
		uint num3 = PackUtils.PackUNorm(255f, vectorZ) << 16;
		uint num4 = PackUtils.PackUNorm(255f, vectorW) << 24;
		return num | num2 | num3 | num4;
	}
	private static int ClampToByte32(int value)
	{
		if (value < 0)
		{
			return 0;
		}
		if (value > 255)
		{
			return 255;
		}
		return value;
	}
	private static int ClampToByte64(long value)
	{
		if (value < 0L)
		{
			return 0;
		}
		if (value > 255L)
		{
			return 255;
		}
		return (int)value;
	}
	public Vector4 ToVector4()
	{
		Vector4 result;
		result.X = PackUtils.UnpackUNorm(255u, this.packedValue);
		result.Y = PackUtils.UnpackUNorm(255u, this.packedValue >> 8);
		result.Z = PackUtils.UnpackUNorm(255u, this.packedValue >> 16);
		result.W = PackUtils.UnpackUNorm(255u, this.packedValue >> 24);
		return result;
	}
	public static Color Lerp(Color value1, Color value2, float amount)
	{
		uint num = value1.packedValue;
		uint num2 = value2.packedValue;
		int num3 = (int)((byte)num);
		int num4 = (int)((byte)(num >> 8));
		int num5 = (int)((byte)(num >> 16));
		int num6 = (int)((byte)(num >> 24));
		int num7 = (int)((byte)num2);
		int num8 = (int)((byte)(num2 >> 8));
		int num9 = (int)((byte)(num2 >> 16));
		int num10 = (int)((byte)(num2 >> 24));
		int num11 = (int)PackUtils.PackUNorm(65536f, amount);
		int num12 = num3 + ((num7 - num3) * num11 >> 16);
		int num13 = num4 + ((num8 - num4) * num11 >> 16);
		int num14 = num5 + ((num9 - num5) * num11 >> 16);
		int num15 = num6 + ((num10 - num6) * num11 >> 16);
		Color result;
		result.packedValue = (uint)(num12 | num13 << 8 | num14 << 16 | num15 << 24);
		return result;
	}
	public static Color Multiply(Color value, float scale)
	{
		uint num = value.packedValue;
		uint num2 = (uint)((byte)num);
		uint num3 = (uint)((byte)(num >> 8));
		uint num4 = (uint)((byte)(num >> 16));
		uint num5 = (uint)((byte)(num >> 24));
		scale *= 65536f;
		uint num6;
		if (scale < 0f)
		{
			num6 = 0u;
		}
		else
		{
			if (scale > 1.677722E+07f)
			{
				num6 = 16777215u;
			}
			else
			{
				num6 = (uint)scale;
			}
		}
		num2 = num2 * num6 >> 16;
		num3 = num3 * num6 >> 16;
		num4 = num4 * num6 >> 16;
		num5 = num5 * num6 >> 16;
		if (num2 > 255u)
		{
			num2 = 255u;
		}
		if (num3 > 255u)
		{
			num3 = 255u;
		}
		if (num4 > 255u)
		{
			num4 = 255u;
		}
		if (num5 > 255u)
		{
			num5 = 255u;
		}
		Color result;
		result.packedValue = (num2 | num3 << 8 | num4 << 16 | num5 << 24);
		return result;
	}
	public static Color operator *(Color value, float scale)
	{
		uint num = value.packedValue;
		uint num2 = (uint)((byte)num);
		uint num3 = (uint)((byte)(num >> 8));
		uint num4 = (uint)((byte)(num >> 16));
		uint num5 = (uint)((byte)(num >> 24));
		scale *= 65536f;
		uint num6;
		if (scale < 0f)
		{
			num6 = 0u;
		}
		else
		{
			if (scale > 1.677722E+07f)
			{
				num6 = 16777215u;
			}
			else
			{
				num6 = (uint)scale;
			}
		}
		num2 = num2 * num6 >> 16;
		num3 = num3 * num6 >> 16;
		num4 = num4 * num6 >> 16;
		num5 = num5 * num6 >> 16;
		if (num2 > 255u)
		{
			num2 = 255u;
		}
		if (num3 > 255u)
		{
			num3 = 255u;
		}
		if (num4 > 255u)
		{
			num4 = 255u;
		}
		if (num5 > 255u)
		{
			num5 = 255u;
		}
		Color result;
		result.packedValue = (num2 | num3 << 8 | num4 << 16 | num5 << 24);
		return result;
	}
	public override string ToString()
	{
		return string.Format(CultureInfo.CurrentCulture, "{{R:{0} G:{1} B:{2} A:{3}}}", new object[]
		{
			this.R, 
			this.G, 
			this.B, 
			this.A
		});
	}
	public override int GetHashCode()
	{
		return this.packedValue.GetHashCode();
	}
	public override bool Equals(object obj)
	{
		return obj is Color && this.Equals((Color)obj);
	}
	public bool Equals(Color other)
	{
		return this.packedValue.Equals(other.packedValue);
	}
	public static bool operator ==(Color a, Color b)
	{
		return a.Equals(b);
	}
	public static bool operator !=(Color a, Color b)
	{
		return !a.Equals(b);
	}
}
