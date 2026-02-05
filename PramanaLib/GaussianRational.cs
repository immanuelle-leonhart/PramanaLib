using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace PramanaLib;

/// <summary>
/// Represents a complex number as a/b + (c/d)*i where a,b,c,d are BigIntegers.
/// Stored as a normalized vector <a,b,c,d> with fractions in lowest terms.
/// </summary>
[Serializable]
public readonly struct GaussianRational :
    IEquatable<GaussianRational>,
    IComparable<GaussianRational>,
    IComparable,
    IFormattable
{
    private static readonly Guid PramanaNamespace = new("a6613321-e9f6-4348-8f8b-29d2a3c86349");

    // Real part = A/B, Imaginary part = C/D
    public BigInteger A { get; }
    public BigInteger B { get; }
    public BigInteger C { get; }
    public BigInteger D { get; }

    /// <summary>
    /// Creates a GaussianRational from components a/b + (c/d)*i.
    /// Automatically normalizes to lowest terms.
    /// </summary>
    public GaussianRational(BigInteger a, BigInteger b, BigInteger c, BigInteger d)
    {
        if (b == 0) throw new DivideByZeroException("Real denominator cannot be zero");
        if (d == 0) throw new DivideByZeroException("Imaginary denominator cannot be zero");

        // Normalize real part
        (A, B) = Normalize(a, b);
        // Normalize imaginary part
        (C, D) = Normalize(c, d);
    }

    /// <summary>
    /// Creates a real GaussianRational (imaginary part = 0).
    /// </summary>
    public GaussianRational(BigInteger real, BigInteger imaginary) : this(real, 1, imaginary, 1) { }

    /// <summary>
    /// Creates an integer GaussianRational.
    /// </summary>
    public GaussianRational(BigInteger value) : this(value, 1, 0, 1) { }

    private static (BigInteger num, BigInteger den) Normalize(BigInteger numerator, BigInteger denominator)
    {
        if (denominator == 0) throw new DivideByZeroException();

        // Handle zero numerator
        if (numerator == 0) return (0, 1);

        // Ensure denominator is positive
        if (denominator < 0)
        {
            numerator = -numerator;
            denominator = -denominator;
        }

        // Reduce to lowest terms
        var gcd = BigInteger.GreatestCommonDivisor(BigInteger.Abs(numerator), denominator);
        return (numerator / gcd, denominator / gcd);
    }

    /// <summary>
    /// Gets the PramanaId - a UUIDv5 generated from the canonical string representation.
    /// </summary>
    public Guid PramanaId => GenerateUuidV5(PramanaNamespace, $"{A},{B},{C},{D}");

    /// <summary>
    /// Gets the Pramana entity URL for this number.
    /// </summary>
    public string PramanaUrl => $"https://pramana-data.ca/entity/{PramanaId}";

    private static Guid GenerateUuidV5(Guid namespaceId, string name)
    {
        byte[] namespaceBytes = namespaceId.ToByteArray();
        // Convert to big-endian for UUID spec compliance
        SwapByteOrder(namespaceBytes);

        byte[] nameBytes = Encoding.UTF8.GetBytes(name);
        byte[] data = new byte[namespaceBytes.Length + nameBytes.Length];
        namespaceBytes.CopyTo(data, 0);
        nameBytes.CopyTo(data, namespaceBytes.Length);

        byte[] hash = SHA1.HashData(data);

        byte[] result = new byte[16];
        Array.Copy(hash, 0, result, 0, 16);

        // Set version (5) and variant bits
        result[6] = (byte)((result[6] & 0x0F) | 0x50);
        result[8] = (byte)((result[8] & 0x3F) | 0x80);

        SwapByteOrder(result);
        return new Guid(result);
    }

    private static void SwapByteOrder(byte[] guid)
    {
        (guid[0], guid[3]) = (guid[3], guid[0]);
        (guid[1], guid[2]) = (guid[2], guid[1]);
        (guid[4], guid[5]) = (guid[5], guid[4]);
        (guid[6], guid[7]) = (guid[7], guid[6]);
    }

    #region String Representations

    /// <summary>
    /// Returns the raw vector form: <a,b,c,d>
    /// </summary>
    public string ToRawString() => $"<{A},{B},{C},{D}>";

    /// <summary>
    /// Returns human-readable form: "1", "1/2", "1 + i", "1/2 + 1/2 i"
    /// </summary>
    public override string ToString()
    {
        string realPart = FormatRational(A, B);
        string imagPart = FormatImaginary(C, D);

        if (C == 0) return realPart;
        if (A == 0) return imagPart;

        if (C > 0)
            return $"{realPart} + {imagPart}";
        else
            return $"{realPart} - {FormatImaginary(-C, D)}";
    }

    /// <summary>
    /// Returns decimal form approximation.
    /// </summary>
    public string ToDecimalString(int precision = 15)
    {
        throw new NotSupportedException("ToDecimalString loses precision. Use ToString() for exact representation.");
    }

    private static string FormatRational(BigInteger num, BigInteger den)
    {
        if (den == 1) return num.ToString();

        // Use mixed fraction format: "1 & 1/2" instead of "3/2"
        BigInteger whole = num / den;
        BigInteger remainder = BigInteger.Abs(num % den);

        if (whole == 0)
        {
            // Proper fraction only (e.g., "1/2" or "-1/2")
            return $"{num}/{den}";
        }

        if (remainder == 0)
        {
            // Whole number
            return whole.ToString();
        }

        // Mixed fraction (e.g., "1 & 1/2" or "-1 & 1/2")
        return $"{whole} & {remainder}/{den}";
    }

    private static string FormatImaginary(BigInteger num, BigInteger den)
    {
        if (num == 0) return "0";
        if (num == 1 && den == 1) return "i";
        if (num == -1 && den == 1) return "-i";
        if (den == 1) return $"{num}i";

        // Use mixed fraction format for imaginary part
        BigInteger whole = num / den;
        BigInteger remainder = BigInteger.Abs(num % den);

        if (whole == 0)
        {
            // Proper fraction only
            return $"{num}/{den} i";
        }

        if (remainder == 0)
        {
            // Whole number
            return $"{whole}i";
        }

        // Mixed fraction
        return $"{whole} & {remainder}/{den} i";
    }

    #endregion

    #region Implicit Conversions (FROM other types)

    public static implicit operator GaussianRational(int value) => new(value);
    public static implicit operator GaussianRational(long value) => new(value);
    public static implicit operator GaussianRational(BigInteger value) => new(value);

    public static implicit operator GaussianRational(float value) => FromDouble(value);
    public static implicit operator GaussianRational(double value) => FromDouble(value);
    public static implicit operator GaussianRational(decimal value) => FromDecimal(value);

    private static GaussianRational FromDouble(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            throw new ArgumentException("Cannot convert NaN or Infinity to GaussianRational");

        // Convert to exact fraction representation
        var (num, den) = DoubleToFraction(value);
        return new GaussianRational(num, den, 0, 1);
    }

    private static GaussianRational FromDecimal(decimal value)
    {
        // Get the bits of the decimal
        int[] bits = decimal.GetBits(value);
        int scale = (bits[3] >> 16) & 0xFF;
        bool negative = (bits[3] & 0x80000000) != 0;

        // Construct the mantissa
        BigInteger mantissa = new BigInteger((uint)bits[2]);
        mantissa = (mantissa << 32) | (uint)bits[1];
        mantissa = (mantissa << 32) | (uint)bits[0];

        if (negative) mantissa = -mantissa;

        BigInteger denominator = BigInteger.Pow(10, scale);
        return new GaussianRational(mantissa, denominator, 0, 1);
    }

    private static (BigInteger num, BigInteger den) DoubleToFraction(double value)
    {
        if (value == 0) return (0, 1);

        bool negative = value < 0;
        value = Math.Abs(value);

        // Use continued fraction algorithm for best rational approximation
        const double tolerance = 1e-15;
        const int maxIterations = 64;

        BigInteger num1 = 1, num2 = 0;
        BigInteger den1 = 0, den2 = 1;
        double x = value;

        for (int i = 0; i < maxIterations; i++)
        {
            BigInteger intPart = (BigInteger)Math.Floor(x);
            BigInteger num = intPart * num1 + num2;
            BigInteger den = intPart * den1 + den2;

            double approx = (double)num / (double)den;
            if (Math.Abs(approx - value) < tolerance * value)
            {
                return (negative ? -num : num, den);
            }

            num2 = num1; num1 = num;
            den2 = den1; den1 = den;

            double frac = x - (double)intPart;
            if (frac < tolerance) break;
            x = 1.0 / frac;
        }

        return (negative ? -num1 : num1, den1);
    }

    #endregion

    #region Explicit Conversions (TO other types)

    private void ThrowIfNotReal()
    {
        if (C != 0)
            throw new InvalidCastException("Cannot convert complex number with non-zero imaginary part to real number type");
    }

    public static explicit operator int(GaussianRational gr)
    {
        gr.ThrowIfNotReal();
        if (gr.B != 1)
            throw new InvalidCastException($"Cannot convert non-integer rational {gr.A}/{gr.B} to int");
        return (int)gr.A;
    }

    public static explicit operator long(GaussianRational gr)
    {
        gr.ThrowIfNotReal();
        if (gr.B != 1)
            throw new InvalidCastException($"Cannot convert non-integer rational {gr.A}/{gr.B} to long");
        return (long)gr.A;
    }

    public static explicit operator BigInteger(GaussianRational gr)
    {
        gr.ThrowIfNotReal();
        if (gr.B != 1)
            throw new InvalidCastException($"Cannot convert non-integer rational {gr.A}/{gr.B} to BigInteger");
        return gr.A;
    }

    public static explicit operator float(GaussianRational gr)
    {
        gr.ThrowIfNotReal();
        return (float)gr.A / (float)gr.B;
    }

    public static explicit operator double(GaussianRational gr)
    {
        gr.ThrowIfNotReal();
        return (double)gr.A / (double)gr.B;
    }

    public static explicit operator decimal(GaussianRational gr)
    {
        gr.ThrowIfNotReal();
        return (decimal)gr.A / (decimal)gr.B;
    }

    /// <summary>
    /// Converts to array [real, imaginary] as doubles.
    /// </summary>
    public static explicit operator double[](GaussianRational gr)
    {
        return [(double)gr.A / (double)gr.B, (double)gr.C / (double)gr.D];
    }

    /// <summary>
    /// Converts to array [real, imaginary] as integers (throws if not integer values).
    /// </summary>
    public static explicit operator int[](GaussianRational gr)
    {
        if (gr.B != 1 || gr.D != 1)
            throw new InvalidCastException("Cannot convert non-integer GaussianRational to int[]");
        return [(int)gr.A, (int)gr.C];
    }

    /// <summary>
    /// Converts to array [real, imaginary] as BigIntegers (throws if not integer values).
    /// </summary>
    public static explicit operator BigInteger[](GaussianRational gr)
    {
        if (gr.B != 1 || gr.D != 1)
            throw new InvalidCastException("Cannot convert non-integer GaussianRational to BigInteger[]");
        return [gr.A, gr.C];
    }

    #endregion

    #region Mathematical Operations

    public static GaussianRational operator +(GaussianRational left, GaussianRational right)
    {
        // (a/b + c/d*i) + (e/f + g/h*i) = (a/b + e/f) + (c/d + g/h)*i
        var (realNum, realDen) = AddFractions(left.A, left.B, right.A, right.B);
        var (imagNum, imagDen) = AddFractions(left.C, left.D, right.C, right.D);
        return new GaussianRational(realNum, realDen, imagNum, imagDen);
    }

    public static GaussianRational operator -(GaussianRational left, GaussianRational right)
    {
        var (realNum, realDen) = AddFractions(left.A, left.B, -right.A, right.B);
        var (imagNum, imagDen) = AddFractions(left.C, left.D, -right.C, right.D);
        return new GaussianRational(realNum, realDen, imagNum, imagDen);
    }

    public static GaussianRational operator -(GaussianRational value)
    {
        return new GaussianRational(-value.A, value.B, -value.C, value.D);
    }

    public static GaussianRational operator ++(GaussianRational value)
    {
        return value + One;
    }

    public static GaussianRational operator --(GaussianRational value)
    {
        return value - One;
    }

    public static GaussianRational operator *(GaussianRational left, GaussianRational right)
    {
        // (a + bi)(c + di) = (ac - bd) + (ad + bc)i
        // With fractions: (a/b + c/d*i)(e/f + g/h*i)
        // Real: (a/b)(e/f) - (c/d)(g/h) = ae/bf - cg/dh
        // Imag: (a/b)(g/h) + (c/d)(e/f) = ag/bh + ce/df

        var (r1Num, r1Den) = MultiplyFractions(left.A, left.B, right.A, right.B);
        var (r2Num, r2Den) = MultiplyFractions(left.C, left.D, right.C, right.D);
        var (realNum, realDen) = AddFractions(r1Num, r1Den, -r2Num, r2Den);

        var (i1Num, i1Den) = MultiplyFractions(left.A, left.B, right.C, right.D);
        var (i2Num, i2Den) = MultiplyFractions(left.C, left.D, right.A, right.B);
        var (imagNum, imagDen) = AddFractions(i1Num, i1Den, i2Num, i2Den);

        return new GaussianRational(realNum, realDen, imagNum, imagDen);
    }

    public static GaussianRational operator /(GaussianRational left, GaussianRational right)
    {
        // (a + bi)/(c + di) = (a + bi)(c - di) / (c² + d²)
        var conjugate = right.Conjugate;
        var numerator = left * conjugate;
        var denominator = right * conjugate; // This will be real (c² + d²)

        // denominator.C should be 0, denominator is (c² + d²)/1
        if (denominator.A == 0)
            throw new DivideByZeroException("Cannot divide by zero");

        // Divide numerator by denominator (which is real)
        var (realNum, realDen) = DivideFractions(numerator.A, numerator.B, denominator.A, denominator.B);
        var (imagNum, imagDen) = DivideFractions(numerator.C, numerator.D, denominator.A, denominator.B);

        return new GaussianRational(realNum, realDen, imagNum, imagDen);
    }

    private static (BigInteger num, BigInteger den) AddFractions(BigInteger a, BigInteger b, BigInteger c, BigInteger d)
    {
        // a/b + c/d = (ad + bc) / bd
        return (a * d + b * c, b * d);
    }

    private static (BigInteger num, BigInteger den) MultiplyFractions(BigInteger a, BigInteger b, BigInteger c, BigInteger d)
    {
        // (a/b) * (c/d) = ac/bd
        return (a * c, b * d);
    }

    private static (BigInteger num, BigInteger den) DivideFractions(BigInteger a, BigInteger b, BigInteger c, BigInteger d)
    {
        // (a/b) / (c/d) = ad/bc
        if (c == 0) throw new DivideByZeroException();
        return (a * d, b * c);
    }

    /// <summary>
    /// Gets the complex conjugate (a/b - c/d*i).
    /// </summary>
    public GaussianRational Conjugate => new(A, B, -C, D);

    /// <summary>
    /// Gets the magnitude squared (|z|² = a²/b² + c²/d²) as a GaussianRational.
    /// </summary>
    public GaussianRational MagnitudeSquared
    {
        get
        {
            var (r1, r2) = MultiplyFractions(A, B, A, B);
            var (i1, i2) = MultiplyFractions(C, D, C, D);
            var (num, den) = AddFractions(r1, r2, i1, i2);
            return new GaussianRational(num, den, 0, 1);
        }
    }

    /// <summary>
    /// Gets the magnitude (|z|) as a double.
    /// </summary>
    public double Magnitude => throw new NotSupportedException("Magnitude loses precision. Use MagnitudeSquared for exact representation.");

    /// <summary>
    /// Gets the phase angle (argument) in radians.
    /// </summary>
    public double Phase => throw new NotSupportedException("Phase loses precision and cannot be exactly represented as a rational.");

    /// <summary>
    /// Gets the polar form as (magnitude, phase).
    /// </summary>
    public (double Magnitude, double Phase) ToPolar() => throw new NotSupportedException("ToPolar loses precision. Polar form cannot be exactly represented as rationals.");

    /// <summary>
    /// Creates a GaussianRational from polar coordinates.
    /// </summary>
    public static GaussianRational FromPolar(double magnitude, double phase)
    {
        throw new NotSupportedException("FromPolar loses precision. Polar coordinates cannot be exactly converted to rationals.");
    }

    /// <summary>
    /// Gets the real part as a GaussianRational.
    /// </summary>
    public GaussianRational RealPart => new(A, B, 0, 1);

    /// <summary>
    /// Gets the imaginary part as a GaussianRational (without the i).
    /// </summary>
    public GaussianRational ImaginaryPart => new(C, D, 0, 1);

    /// <summary>
    /// Returns true if this is a real number (imaginary part is zero).
    /// </summary>
    public bool IsReal => C == 0;

    /// <summary>
    /// Returns true if this is purely imaginary (real part is zero, imaginary non-zero).
    /// </summary>
    public bool IsPurelyImaginary => A == 0 && C != 0;

    /// <summary>
    /// Returns true if this represents an integer (both parts are integers with zero imaginary).
    /// </summary>
    public bool IsInteger => B == 1 && C == 0;

    /// <summary>
    /// Returns true if this represents a Gaussian integer (both parts are integers).
    /// </summary>
    public bool IsGaussianInteger => B == 1 && D == 1;

    /// <summary>
    /// The imaginary unit i.
    /// </summary>
    public static GaussianRational I => new(0, 1, 1, 1);

    /// <summary>
    /// Zero.
    /// </summary>
    public static GaussianRational Zero => new(0, 1, 0, 1);

    /// <summary>
    /// One.
    /// </summary>
    public static GaussianRational One => new(1, 1, 0, 1);

    /// <summary>
    /// Negative one.
    /// </summary>
    public static GaussianRational MinusOne => new(-1, 1, 0, 1);

    /// <summary>
    /// Modulo operation for real GaussianRationals.
    /// </summary>
    public static GaussianRational operator %(GaussianRational left, GaussianRational right)
    {
        if (!left.IsReal || !right.IsReal)
            throw new InvalidOperationException("Modulo operation only supported for real numbers");
        if (right.A == 0)
            throw new DivideByZeroException("Cannot compute modulo with zero divisor");

        // (a/b) % (c/d) = ((a*d) % (c*b)) / (b*d)
        var num = (left.A * right.B) % (right.A * left.B);
        var den = left.B * right.B;
        return new GaussianRational(num, den, 0, 1);
    }

    /// <summary>
    /// Returns the absolute value (magnitude) as a GaussianRational.
    /// For real numbers, returns the absolute value exactly.
    /// For complex numbers, returns the magnitude squared (use Magnitude property for double).
    /// </summary>
    public static GaussianRational Abs(GaussianRational value)
    {
        if (value.IsReal)
        {
            return value.A >= 0 ? value : -value;
        }
        // For complex, return magnitude squared to stay in rationals
        return value.MagnitudeSquared;
    }

    /// <summary>
    /// Returns the sign of the real part: -1, 0, or 1.
    /// </summary>
    public static int Sign(GaussianRational value)
    {
        return value.A.Sign;
    }

    /// <summary>
    /// Returns the floor of the real part (largest integer <= real part).
    /// Only valid for real GaussianRationals.
    /// </summary>
    public static GaussianRational Floor(GaussianRational value)
    {
        if (!value.IsReal)
            throw new InvalidOperationException("Floor only supported for real numbers");

        // Floor of a/b
        if (value.B == 1) return value;
        var floor = value.A / value.B;
        if (value.A < 0 && value.A % value.B != 0)
            floor--;
        return new GaussianRational(floor);
    }

    /// <summary>
    /// Returns the ceiling of the real part (smallest integer >= real part).
    /// Only valid for real GaussianRationals.
    /// </summary>
    public static GaussianRational Ceiling(GaussianRational value)
    {
        if (!value.IsReal)
            throw new InvalidOperationException("Ceiling only supported for real numbers");

        if (value.B == 1) return value;
        var ceil = value.A / value.B;
        if (value.A > 0 && value.A % value.B != 0)
            ceil++;
        return new GaussianRational(ceil);
    }

    /// <summary>
    /// Returns the truncated integer part (rounds toward zero).
    /// Only valid for real GaussianRationals.
    /// </summary>
    public static GaussianRational Truncate(GaussianRational value)
    {
        if (!value.IsReal)
            throw new InvalidOperationException("Truncate only supported for real numbers");

        return new GaussianRational(value.A / value.B);
    }

    /// <summary>
    /// Raises the GaussianRational to an integer power.
    /// </summary>
    public static GaussianRational Pow(GaussianRational baseValue, int exponent)
    {
        if (exponent == 0) return One;
        if (exponent == 1) return baseValue;

        if (exponent < 0)
        {
            baseValue = One / baseValue;
            exponent = -exponent;
        }

        var result = One;
        var current = baseValue;

        while (exponent > 0)
        {
            if ((exponent & 1) == 1)
                result *= current;
            current *= current;
            exponent >>= 1;
        }

        return result;
    }

    /// <summary>
    /// Returns the minimum of two GaussianRationals (compared by real part).
    /// </summary>
    public static GaussianRational Min(GaussianRational a, GaussianRational b)
    {
        return a <= b ? a : b;
    }

    /// <summary>
    /// Returns the maximum of two GaussianRationals (compared by real part).
    /// </summary>
    public static GaussianRational Max(GaussianRational a, GaussianRational b)
    {
        return a >= b ? a : b;
    }

    /// <summary>
    /// Clamps the value between min and max (compared by real part).
    /// </summary>
    public static GaussianRational Clamp(GaussianRational value, GaussianRational min, GaussianRational max)
    {
        if (min > max)
            throw new ArgumentException("min must be less than or equal to max");
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }

    /// <summary>
    /// Returns the reciprocal (1/z).
    /// </summary>
    public GaussianRational Reciprocal => One / this;

    /// <summary>
    /// Returns true if this value is zero.
    /// </summary>
    public bool IsZero => A == 0 && C == 0;

    /// <summary>
    /// Returns true if this value is one.
    /// </summary>
    public bool IsOne => A == 1 && B == 1 && C == 0;

    /// <summary>
    /// Returns true if this is a negative real number.
    /// </summary>
    public bool IsNegative => IsReal && A < 0;

    /// <summary>
    /// Returns true if this is a positive real number.
    /// </summary>
    public bool IsPositive => IsReal && A > 0;

    #endregion

    #region Equality

    public bool Equals(GaussianRational other)
    {
        return A == other.A && B == other.B && C == other.C && D == other.D;
    }

    public override bool Equals(object? obj)
    {
        return obj is GaussianRational other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(A, B, C, D);
    }

    public static bool operator ==(GaussianRational left, GaussianRational right) => left.Equals(right);
    public static bool operator !=(GaussianRational left, GaussianRational right) => !left.Equals(right);

    #endregion

    #region Comparison

    /// <summary>
    /// Compares based on the real part (a/b). For complex numbers with equal real parts,
    /// compares by imaginary part.
    /// </summary>
    public int CompareTo(GaussianRational other)
    {
        // Compare real parts: a/b vs e/f  =>  a*f vs e*b
        var realCompare = (A * other.B).CompareTo(other.A * B);
        if (realCompare != 0) return realCompare;

        // If real parts equal, compare imaginary parts: c/d vs g/h  =>  c*h vs g*d
        return (C * other.D).CompareTo(other.C * D);
    }

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is GaussianRational other) return CompareTo(other);
        throw new ArgumentException($"Object must be of type {nameof(GaussianRational)}");
    }

    public static bool operator <(GaussianRational left, GaussianRational right) => left.CompareTo(right) < 0;
    public static bool operator >(GaussianRational left, GaussianRational right) => left.CompareTo(right) > 0;
    public static bool operator <=(GaussianRational left, GaussianRational right) => left.CompareTo(right) <= 0;
    public static bool operator >=(GaussianRational left, GaussianRational right) => left.CompareTo(right) >= 0;

    #endregion

    #region IFormattable

    /// <summary>
    /// Formats the GaussianRational using the specified format.
    /// Supported formats:
    /// - "G" or null: Default human-readable format (e.g., "1 & 1/2 + 3i")
    /// - "R": Raw vector format (e.g., "<3,2,3,1>")
    /// - "D" or "F": Decimal format (e.g., "1.5 + 3i")
    /// - "I": Improper fraction format (e.g., "3/2 + 3i")
    /// </summary>
    public string ToString(string? format, IFormatProvider? formatProvider = null)
    {
        if (string.IsNullOrEmpty(format)) format = "G";

        return format.ToUpperInvariant() switch
        {
            "G" => ToString(),
            "R" => ToRawString(),
            "D" or "F" => throw new NotSupportedException("Decimal format loses precision. Use 'G' for exact representation."),
            "I" => ToImproperFractionString(),
            _ => throw new FormatException($"The '{format}' format string is not supported.")
        };
    }

    /// <summary>
    /// Returns improper fraction format (e.g., "3/2" instead of "1 & 1/2").
    /// </summary>
    public string ToImproperFractionString()
    {
        string realPart = B == 1 ? A.ToString() : $"{A}/{B}";
        string imagPart = FormatImaginaryImproper(C, D);

        if (C == 0) return realPart;
        if (A == 0) return imagPart;

        if (C > 0)
            return $"{realPart} + {imagPart}";
        else
            return $"{realPart} - {FormatImaginaryImproper(-C, D)}";
    }

    private static string FormatImaginaryImproper(BigInteger num, BigInteger den)
    {
        if (num == 0) return "0";
        if (num == 1 && den == 1) return "i";
        if (num == -1 && den == 1) return "-i";
        if (den == 1) return $"{num}i";
        return $"{num}/{den} i";
    }

    #endregion

    #region Parsing

    /// <summary>
    /// Parses from the canonical form "a,b,c,d".
    /// </summary>
    public static GaussianRational Parse(string s)
    {
        var parts = s.Split(',');
        if (parts.Length != 4)
            throw new FormatException("Expected format: a,b,c,d");

        return new GaussianRational(
            BigInteger.Parse(parts[0].Trim()),
            BigInteger.Parse(parts[1].Trim()),
            BigInteger.Parse(parts[2].Trim()),
            BigInteger.Parse(parts[3].Trim())
        );
    }

    /// <summary>
    /// Tries to parse from the canonical form "a,b,c,d".
    /// </summary>
    public static bool TryParse(string s, out GaussianRational result)
    {
        result = default;
        var parts = s.Split(',');
        if (parts.Length != 4) return false;

        if (!BigInteger.TryParse(parts[0].Trim(), out var a)) return false;
        if (!BigInteger.TryParse(parts[1].Trim(), out var b)) return false;
        if (!BigInteger.TryParse(parts[2].Trim(), out var c)) return false;
        if (!BigInteger.TryParse(parts[3].Trim(), out var d)) return false;

        try
        {
            result = new GaussianRational(a, b, c, d);
            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion
}
