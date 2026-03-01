using System.Numerics;

namespace PramanaLib;

/// <summary>
/// Represents a Gaussian integer (Z[i]) — a complex number whose real and imaginary
/// parts are both integers. Supports full arithmetic and number-theoretic operations
/// including GCD, extended GCD, primality testing, and modular arithmetic.
/// </summary>
/// <remarks>
/// <para>
/// In mathematics, the Gaussian integers are denoted Z[i]. In Pramana, this type is
/// called <c>Gint</c>. The related type <see cref="Gauss"/> represents Gaussian
/// rationals (Q[i]).
/// </para>
/// <para>
/// This type is an immutable value type (readonly struct) that uses <see cref="BigInteger"/>
/// for arbitrary-precision integer arithmetic.
/// </para>
/// </remarks>
[Serializable]
public readonly struct Gint :
    IEquatable<Gint>,
    IComparable<Gint>,
    IComparable
{
    /// <summary>Gets the real part of this Gaussian integer.</summary>
    public BigInteger Real { get; }

    /// <summary>Gets the imaginary part of this Gaussian integer.</summary>
    public BigInteger Imag { get; }

    /// <summary>
    /// Initializes a new <see cref="Gint"/> with the given real and imaginary parts.
    /// </summary>
    /// <param name="real">The real part.</param>
    /// <param name="imag">The imaginary part.</param>
    public Gint(BigInteger real, BigInteger imag)
    {
        Real = real;
        Imag = imag;
    }

    /// <summary>
    /// Initializes a new <see cref="Gint"/> with only a real part (imaginary = 0).
    /// </summary>
    /// <param name="real">The real part.</param>
    public Gint(BigInteger real) : this(real, 0) { }

    /// <summary>
    /// Initializes a new <see cref="Gint"/> with both parts equal to zero.
    /// </summary>
    public Gint() : this(0, 0) { }

    #region Properties

    /// <summary>
    /// Gets the complex conjugate (real - imag*i).
    /// </summary>
    public Gint Conjugate => new(Real, -Imag);

    /// <summary>
    /// Gets the norm (squared absolute value): real² + imag².
    /// </summary>
    /// <remarks>
    /// The norm is the square of the usual absolute value. It is always a non-negative integer.
    /// </remarks>
    public BigInteger Norm => Real * Real + Imag * Imag;

    /// <summary>
    /// Returns true if this Gaussian integer is a unit (one of 1, -1, i, -i).
    /// </summary>
    public bool IsUnit => Norm == 1;

    /// <summary>
    /// Returns true if this value is zero (0 + 0i).
    /// </summary>
    public bool IsZero => Real == 0 && Imag == 0;

    /// <summary>
    /// Returns true if this is a real integer (imaginary part is zero).
    /// </summary>
    public bool IsReal => Imag == 0;

    /// <summary>
    /// Returns true if this is purely imaginary (real part is zero, imaginary non-zero).
    /// </summary>
    public bool IsPurelyImaginary => Real == 0 && Imag != 0;

    #endregion

    #region Static Factory Methods

    /// <summary>
    /// Returns the imaginary unit i = Gint(0, 1).
    /// </summary>
    public static Gint Eye() => new(0, 1);

    /// <summary>
    /// Returns the four units of Z[i]: [1, -1, i, -i].
    /// </summary>
    public static Gint[] Units() => [new(1), new(-1), Eye(), -Eye()];

    /// <summary>
    /// Returns 1+i. A Gaussian integer has an even norm if and only if it is a multiple of 1+i.
    /// </summary>
    public static Gint Two() => new(1, 1);

    /// <summary>
    /// Returns a random Gaussian integer with components in the specified ranges.
    /// </summary>
    /// <param name="re1">Minimum real part (inclusive).</param>
    /// <param name="re2">Maximum real part (inclusive).</param>
    /// <param name="im1">Minimum imaginary part (inclusive).</param>
    /// <param name="im2">Maximum imaginary part (inclusive).</param>
    public static Gint Random(int re1 = -100, int re2 = 100, int im1 = -100, int im2 = 100)
    {
        var rng = System.Random.Shared;
        return new Gint(rng.Next(re1, re2 + 1), rng.Next(im1, im2 + 1));
    }

    /// <summary>
    /// Converts a two-element array into a Gaussian integer.
    /// </summary>
    /// <param name="arr">An array of exactly two elements: [real, imag].</param>
    /// <exception cref="ArgumentException">Thrown if the array does not have exactly 2 elements.</exception>
    public static Gint FromArray(BigInteger[] arr)
    {
        if (arr.Length != 2)
            throw new ArgumentException("Array must have exactly 2 elements", nameof(arr));
        return new Gint(arr[0], arr[1]);
    }

    /// <summary>Zero (0 + 0i).</summary>
    public static Gint Zero => new(0, 0);

    /// <summary>One (1 + 0i).</summary>
    public static Gint One => new(1, 0);

    /// <summary>Negative one (-1 + 0i).</summary>
    public static Gint MinusOne => new(-1, 0);

    /// <summary>The imaginary unit (0 + 1i).</summary>
    public static Gint I => new(0, 1);

    #endregion

    #region Arithmetic Operators

    /// <summary>Adds two Gaussian integers.</summary>
    public static Gint operator +(Gint left, Gint right) =>
        new(left.Real + right.Real, left.Imag + right.Imag);

    /// <summary>Subtracts two Gaussian integers.</summary>
    public static Gint operator -(Gint left, Gint right) =>
        new(left.Real - right.Real, left.Imag - right.Imag);

    /// <summary>Returns the additive inverse (negation).</summary>
    public static Gint operator -(Gint value) =>
        new(-value.Real, -value.Imag);

    /// <summary>Returns the value unchanged (unary plus).</summary>
    public static Gint operator +(Gint value) => value;

    /// <summary>
    /// Multiplies two Gaussian integers using (a+bi)(c+di) = (ac-bd) + (ad+bc)i.
    /// </summary>
    public static Gint operator *(Gint left, Gint right) =>
        new(left.Real * right.Real - left.Imag * right.Imag,
            left.Real * right.Imag + left.Imag * right.Real);

    /// <summary>
    /// Divides two Gaussian integers exactly, returning a <see cref="Gauss"/>.
    /// </summary>
    public static Gauss operator /(Gint left, Gint right)
    {
        var leftGauss = left.ToGauss();
        var rightGauss = right.ToGauss();
        return leftGauss / rightGauss;
    }

    /// <summary>
    /// Floor division using rounding (not floor) — returns the closest Gaussian integer
    /// approximation to the quotient.
    /// </summary>
    /// <param name="left">The dividend.</param>
    /// <param name="right">The divisor.</param>
    /// <returns>The nearest Gaussian integer to the exact quotient.</returns>
    public static Gint FloorDiv(Gint left, Gint right)
    {
        // Multiply by conjugate: left * conj(right) / |right|²
        var numerator = left * right.Conjugate;
        var denominator = right.Norm;

        // Round each component to nearest integer
        var re = RoundingDivide(numerator.Real, denominator);
        var im = RoundingDivide(numerator.Imag, denominator);

        return new Gint(re, im);
    }

    /// <summary>
    /// Modulo operation — returns the remainder from <see cref="ModifiedDivmod"/>.
    /// </summary>
    public static Gint operator %(Gint left, Gint right)
    {
        var (_, r) = ModifiedDivmod(left, right);
        return r;
    }

    /// <summary>
    /// Raises a Gaussian integer to an integer power.
    /// If n == 0, returns 1. If n &lt; 0, returns a <see cref="Gauss"/>.
    /// </summary>
    public static Gint Pow(Gint baseValue, int exponent)
    {
        if (exponent == 0) return One;
        if (exponent < 0)
            throw new ArgumentException("Negative exponents produce Gaussian rationals. Use Gauss.Pow instead.");
        if (exponent == 1) return baseValue;

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

    /// <summary>Increments the real part by one.</summary>
    public static Gint operator ++(Gint value) => new(value.Real + 1, value.Imag);

    /// <summary>Decrements the real part by one.</summary>
    public static Gint operator --(Gint value) => new(value.Real - 1, value.Imag);

    /// <summary>
    /// Integer division with rounding toward nearest (not floor).
    /// </summary>
    private static BigInteger RoundingDivide(BigInteger numerator, BigInteger denominator)
    {
        // Ensure denominator is positive for consistent rounding
        if (denominator < 0)
        {
            numerator = -numerator;
            denominator = -denominator;
        }

        // Round to nearest: add half the denominator before dividing
        if (numerator >= 0)
            return (numerator + denominator / 2) / denominator;
        else
            return (numerator - denominator / 2) / denominator;
    }

    #endregion

    #region Implicit/Explicit Conversions

    /// <summary>Implicitly converts an <see cref="int"/> to a real <see cref="Gint"/>.</summary>
    public static implicit operator Gint(int value) => new(value);

    /// <summary>Implicitly converts a <see cref="long"/> to a real <see cref="Gint"/>.</summary>
    public static implicit operator Gint(long value) => new(value);

    /// <summary>Implicitly converts a <see cref="BigInteger"/> to a real <see cref="Gint"/>.</summary>
    public static implicit operator Gint(BigInteger value) => new(value);

    /// <summary>
    /// Explicitly converts a <see cref="Gint"/> to a <see cref="Gauss"/>.
    /// </summary>
    public static implicit operator Gauss(Gint value) => value.ToGauss();

    /// <summary>
    /// Explicitly converts a <see cref="Gauss"/> to a <see cref="Gint"/>.
    /// Throws if the Gauss is not a Gaussian integer.
    /// </summary>
    /// <exception cref="InvalidCastException">Thrown if the Gauss has non-integer parts.</exception>
    public static explicit operator Gint(Gauss value)
    {
        if (!value.IsGaussianInteger)
            throw new InvalidCastException("Cannot convert non-integer Gauss to Gint");
        return new Gint(value.A, value.C);
    }

    /// <summary>
    /// Converts to array [real, imaginary] as BigIntegers.
    /// </summary>
    public static explicit operator BigInteger[](Gint g) => [g.Real, g.Imag];

    /// <summary>
    /// Converts to array [real, imaginary] as ints.
    /// </summary>
    public static explicit operator int[](Gint g) => [(int)g.Real, (int)g.Imag];

    #endregion

    #region Instance Methods

    /// <summary>
    /// Converts this Gaussian integer to an equivalent <see cref="Gauss"/>.
    /// </summary>
    public Gauss ToGauss() => new(Real, 1, Imag, 1);

    /// <summary>
    /// Returns a list of this Gint's three non-trivial associates (multiplied by -1, i, -i).
    /// </summary>
    /// <remarks>
    /// Two Gaussian integers are associates if one equals the other times a unit.
    /// Every Gaussian integer has exactly four associates (including itself).
    /// This method returns the three associates that are not equal to this value.
    /// </remarks>
    public Gint[] Associates()
    {
        return [-this, this * Eye(), this * -Eye()];
    }

    /// <summary>
    /// Returns true if <paramref name="other"/> is an associate of this Gaussian integer.
    /// </summary>
    /// <param name="other">The Gaussian integer to test.</param>
    public bool IsAssociate(Gint other)
    {
        if (other.IsZero) return IsZero;
        var q = FloorDiv(this, other);
        if (q * other == this)
            return q.IsUnit;
        return false;
    }

    #endregion

    #region Number-Theoretic Methods

    /// <summary>
    /// The modified divmod algorithm for Gaussian integers.
    /// Returns (q, r) such that a = b * q + r, where r.Norm &lt; b.Norm / 2.
    /// </summary>
    /// <remarks>
    /// This implements the Modified Division Theorem described in
    /// "The Gaussian Integers" by Keith Conrad. It uses rounding instead of floor division.
    /// See: https://kconrad.math.uconn.edu/blurbs/ugradnumthy/Zinotes.pdf
    /// </remarks>
    /// <param name="a">The dividend.</param>
    /// <param name="b">The divisor. Must be non-zero.</param>
    /// <returns>A tuple (quotient, remainder).</returns>
    /// <exception cref="DivideByZeroException">Thrown when <paramref name="b"/> is zero.</exception>
    public static (Gint Quotient, Gint Remainder) ModifiedDivmod(Gint a, Gint b)
    {
        if (b.IsZero)
            throw new DivideByZeroException("Cannot divide by zero");

        // q = round(a * conj(b) / |b|²)
        var numerator = a * b.Conjugate;
        var denominator = b.Norm;

        var qRe = RoundingDivide(numerator.Real, denominator);
        var qIm = RoundingDivide(numerator.Imag, denominator);
        var q = new Gint(qRe, qIm);

        var r = a - b * q;
        return (q, r);
    }

    /// <summary>
    /// Computes the greatest common divisor of two Gaussian integers using the Euclidean algorithm.
    /// </summary>
    /// <param name="a">First Gaussian integer. Must be non-zero.</param>
    /// <param name="b">Second Gaussian integer. Must be non-zero.</param>
    /// <returns>The GCD of <paramref name="a"/> and <paramref name="b"/>.</returns>
    /// <exception cref="ArgumentException">Thrown if both inputs are zero.</exception>
    public static Gint Gcd(Gint a, Gint b)
    {
        if (a.IsZero && b.IsZero)
            throw new ArgumentException("Both inputs must be non-zero");

        var r1 = a;
        var r2 = b;
        while (!r2.IsZero)
        {
            var (_, remainder) = ModifiedDivmod(r1, r2);
            r1 = r2;
            r2 = remainder;
        }
        return r1;
    }

    /// <summary>
    /// The Extended Euclidean Algorithm for Gaussian integers.
    /// Returns (gcd, x, y) such that gcd = alpha * x + beta * y (Bezout's coefficients).
    /// </summary>
    /// <param name="alpha">First Gaussian integer.</param>
    /// <param name="beta">Second Gaussian integer.</param>
    /// <returns>A tuple (gcd, x, y) where gcd = alpha * x + beta * y.</returns>
    public static (Gint Gcd, Gint X, Gint Y) Xgcd(Gint alpha, Gint beta)
    {
        var a = alpha;
        var b = beta;
        Gint x = One, nextX = Zero;
        Gint y = Zero, nextY = One;

        while (!b.IsZero)
        {
            var q = FloorDiv(a, b);
            (nextX, x) = (x - q * nextX, nextX);
            (nextY, y) = (y - q * nextY, nextY);
            (a, b) = (b, a % b);
        }

        return (a, x, y);
    }

    /// <summary>
    /// Tests whether a is congruent to b modulo c.
    /// Returns (true/false, (a-b)/c as a Gauss).
    /// </summary>
    /// <param name="a">First Gaussian integer.</param>
    /// <param name="b">Second Gaussian integer.</param>
    /// <param name="c">The modulus.</param>
    /// <returns>A tuple (isCongruent, quotient) where isCongruent is true if (a-b)/c is a Gaussian integer.</returns>
    public static (bool IsCongruent, Gauss Result) CongruentModulo(Gint a, Gint b, Gint c)
    {
        var diff = a - b;
        var result = diff / c;
        bool isCongruent = result.IsGaussianInteger;
        return (isCongruent, result);
    }

    /// <summary>
    /// Returns true if a and b are relatively prime (their GCD is a unit).
    /// </summary>
    public static bool IsRelativelyPrime(Gint a, Gint b)
    {
        return Gcd(a, b).IsUnit;
    }

    /// <summary>
    /// Returns true if x is a Gaussian prime.
    /// </summary>
    /// <remarks>
    /// See https://mathworld.wolfram.com/GaussianPrime.html
    /// <list type="bullet">
    ///   <item>If both parts are nonzero: prime iff norm is prime</item>
    ///   <item>If one part is zero: the nonzero part must be prime AND congruent to 3 (mod 4)</item>
    /// </list>
    /// </remarks>
    /// <param name="x">The value to test. Can be a Gint or will be treated as a real Gaussian integer.</param>
    public static bool IsGaussianPrime(Gint x)
    {
        var re = BigInteger.Abs(x.Real);
        var im = BigInteger.Abs(x.Imag);
        var norm = x.Norm;

        if (re != 0 && im != 0)
            return NumberTheory.IsPrime(norm);

        if (re == 0)
            return NumberTheory.IsPrime(im) && im % 4 == 3;

        // im == 0
        return NumberTheory.IsPrime(re) && re % 4 == 3;
    }

    /// <summary>
    /// Divides the larger norm by the smaller norm. If they divide evenly,
    /// returns the quotient; otherwise returns null.
    /// </summary>
    /// <param name="a">First Gaussian integer.</param>
    /// <param name="b">Second Gaussian integer.</param>
    /// <returns>The quotient of norms if they divide evenly, or null if they don't.</returns>
    public static BigInteger? NormsDivide(Gint a, Gint b)
    {
        var x = a.Norm;
        var y = b.Norm;
        var sm = BigInteger.Min(x, y);
        var lg = BigInteger.Max(x, y);

        if (sm == 0) return null;
        if (lg % sm == 0)
            return lg / sm;
        return null;
    }

    #endregion

    #region Equality & Comparison

    /// <summary>
    /// Determines whether this value equals another <see cref="Gint"/>.
    /// </summary>
    public bool Equals(Gint other) => Real == other.Real && Imag == other.Imag;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Gint other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Real, Imag);

    /// <summary>Returns true if the two values are equal.</summary>
    public static bool operator ==(Gint left, Gint right) => left.Equals(right);

    /// <summary>Returns true if the two values are not equal.</summary>
    public static bool operator !=(Gint left, Gint right) => !left.Equals(right);

    /// <summary>
    /// Compares by real part first, then by imaginary part.
    /// </summary>
    public int CompareTo(Gint other)
    {
        var realCompare = Real.CompareTo(other.Real);
        return realCompare != 0 ? realCompare : Imag.CompareTo(other.Imag);
    }

    /// <inheritdoc/>
    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is Gint other) return CompareTo(other);
        throw new ArgumentException($"Object must be of type {nameof(Gint)}");
    }

    /// <summary>Returns true if left is less than right.</summary>
    public static bool operator <(Gint left, Gint right) => left.CompareTo(right) < 0;

    /// <summary>Returns true if left is greater than right.</summary>
    public static bool operator >(Gint left, Gint right) => left.CompareTo(right) > 0;

    /// <summary>Returns true if left is less than or equal to right.</summary>
    public static bool operator <=(Gint left, Gint right) => left.CompareTo(right) <= 0;

    /// <summary>Returns true if left is greater than or equal to right.</summary>
    public static bool operator >=(Gint left, Gint right) => left.CompareTo(right) >= 0;

    #endregion

    #region String Representations

    /// <summary>
    /// Returns a string in the form "Gint(real, imag)" or "Gint(real)" if imaginary is zero.
    /// </summary>
    public override string ToString()
    {
        if (Imag == 0)
            return $"{Real}";
        if (Real == 0)
        {
            if (Imag == 1) return "i";
            if (Imag == -1) return "-i";
            return $"{Imag}i";
        }
        if (Imag == 1) return $"{Real} + i";
        if (Imag == -1) return $"{Real} - i";
        if (Imag > 0) return $"{Real} + {Imag}i";
        return $"{Real} - {-Imag}i";
    }

    /// <summary>
    /// Returns the raw representation as "(real, imag)".
    /// </summary>
    public string ToRawString() => $"({Real}, {Imag})";

    #endregion
}
