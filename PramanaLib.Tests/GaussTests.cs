using System.Numerics;

namespace PramanaLib.Tests;

public class GaussTests
{
    #region Construction

    [Fact]
    public void FourArgConstructor_NormalizesToLowestTerms()
    {
        var g = new Gauss(4, 6, -2, 8);
        Assert.Equal(2, g.A);
        Assert.Equal(3, g.B);
        Assert.Equal(-1, g.C);
        Assert.Equal(4, g.D);
    }

    [Fact]
    public void TwoArgConstructor_SetsIntegerParts()
    {
        var g = new Gauss(3, 2);
        Assert.Equal(3, g.A);
        Assert.Equal(1, g.B);
        Assert.Equal(2, g.C);
        Assert.Equal(1, g.D);
    }

    [Fact]
    public void SingleArgConstructor_SetsRealOnly()
    {
        var g = new Gauss(5);
        Assert.Equal(5, g.A);
        Assert.Equal(1, g.B);
        Assert.Equal(0, g.C);
        Assert.Equal(1, g.D);
    }

    [Fact]
    public void Constructor_ThrowsOnZeroDenominator()
    {
        Assert.Throws<DivideByZeroException>(() => new Gauss(1, 0, 0, 1));
        Assert.Throws<DivideByZeroException>(() => new Gauss(1, 1, 0, 0));
    }

    [Fact]
    public void Constructor_NormalizesNegativeDenominator()
    {
        var g = new Gauss(1, -2, 3, -4);
        Assert.True(g.B > 0);
        Assert.True(g.D > 0);
        Assert.Equal(-1, g.A); // 1/-2 = -1/2
        Assert.Equal(2, g.B);
    }

    [Fact]
    public void StaticConstants_AreCorrect()
    {
        Assert.Equal(new Gauss(0, 1, 0, 1), Gauss.Zero);
        Assert.Equal(new Gauss(1, 1, 0, 1), Gauss.One);
        Assert.Equal(new Gauss(-1, 1, 0, 1), Gauss.MinusOne);
        Assert.Equal(new Gauss(0, 1, 1, 1), Gauss.I);
    }

    #endregion

    #region Implicit Conversions

    [Fact]
    public void ImplicitConversion_FromInt()
    {
        Gauss g = 42;
        Assert.Equal(new Gauss(42), g);
    }

    [Fact]
    public void ImplicitConversion_FromLong()
    {
        Gauss g = 100L;
        Assert.Equal(new Gauss(100), g);
    }

    [Fact]
    public void ImplicitConversion_FromDouble()
    {
        Gauss g = 0.5;
        Assert.Equal(new Gauss(1, 2, 0, 1), g);
    }

    [Fact]
    public void ImplicitConversion_FromDecimal()
    {
        Gauss g = 0.25m;
        Assert.Equal(new Gauss(1, 4, 0, 1), g);
    }

    #endregion

    #region Explicit Conversions

    [Fact]
    public void ExplicitConversion_ToInt()
    {
        Gauss g = 42;
        Assert.Equal(42, (int)g);
    }

    [Fact]
    public void ExplicitConversion_ToLong()
    {
        Gauss g = 42;
        Assert.Equal(42L, (long)g);
    }

    [Fact]
    public void ExplicitConversion_ToDouble()
    {
        var g = new Gauss(1, 2, 0, 1);
        Assert.Equal(0.5, (double)g);
    }

    [Fact]
    public void ExplicitConversion_ToInt_ThrowsIfComplex()
    {
        Assert.Throws<InvalidCastException>(() => (int)Gauss.I);
    }

    [Fact]
    public void ExplicitConversion_ToInt_ThrowsIfFraction()
    {
        Assert.Throws<InvalidCastException>(() => (int)new Gauss(1, 2, 0, 1));
    }

    [Fact]
    public void ExplicitConversion_ToDoubleArray()
    {
        var g = new Gauss(3, 1, 2, 1);
        var arr = (double[])g;
        Assert.Equal(3.0, arr[0]);
        Assert.Equal(2.0, arr[1]);
    }

    [Fact]
    public void ExplicitConversion_ToIntArray()
    {
        var g = new Gauss(3, 1, 2, 1);
        var arr = (int[])g;
        Assert.Equal(3, arr[0]);
        Assert.Equal(2, arr[1]);
    }

    #endregion

    #region Arithmetic

    [Fact]
    public void Addition()
    {
        var a = new Gauss(3, 1, 2, 1);
        var b = new Gauss(1, 1, 4, 1);
        Assert.Equal(new Gauss(4, 1, 6, 1), a + b);
    }

    [Fact]
    public void Subtraction()
    {
        var a = new Gauss(3, 1, 2, 1);
        var b = new Gauss(1, 1, 4, 1);
        Assert.Equal(new Gauss(2, 1, -2, 1), a - b);
    }

    [Fact]
    public void Multiplication()
    {
        var a = new Gauss(3, 1, 2, 1);
        var b = new Gauss(1, 1, 4, 1);
        // (3+2i)(1+4i) = 3+12i+2i+8i² = 3+14i-8 = -5+14i
        Assert.Equal(new Gauss(-5, 1, 14, 1), a * b);
    }

    [Fact]
    public void Multiplication_ISquared_IsMinusOne()
    {
        Assert.Equal(Gauss.MinusOne, Gauss.I * Gauss.I);
    }

    [Fact]
    public void Division()
    {
        var a = new Gauss(3, 1, 2, 1);
        var b = new Gauss(1, 1, 4, 1);
        var result = a / b;
        // Verify a = b * result
        Assert.Equal(a, b * result);
    }

    [Fact]
    public void Division_ByZero_Throws()
    {
        Assert.Throws<DivideByZeroException>(() => Gauss.One / Gauss.Zero);
    }

    [Fact]
    public void Negation()
    {
        var g = new Gauss(3, 1, 2, 1);
        Assert.Equal(new Gauss(-3, 1, -2, 1), -g);
    }

    [Fact]
    public void Increment()
    {
        var g = new Gauss(3, 1, 2, 1);
        g++;
        Assert.Equal(new Gauss(4, 1, 2, 1), g);
    }

    [Fact]
    public void Decrement()
    {
        var g = new Gauss(3, 1, 2, 1);
        g--;
        Assert.Equal(new Gauss(2, 1, 2, 1), g);
    }

    [Fact]
    public void Modulo_RealNumbers()
    {
        Gauss a = 7;
        Gauss b = 3;
        Assert.Equal((Gauss)1, a % b);
    }

    [Fact]
    public void Modulo_ThrowsForComplex()
    {
        Assert.Throws<InvalidOperationException>(() => Gauss.I % Gauss.One);
    }

    [Fact]
    public void Pow_PositiveExponent()
    {
        var g = new Gauss(1, 1, 1, 1); // 1+i
        var result = Gauss.Pow(g, 2);
        Assert.Equal(new Gauss(0, 1, 2, 1), result); // (1+i)² = 2i
    }

    [Fact]
    public void Pow_ZeroExponent_ReturnsOne()
    {
        Assert.Equal(Gauss.One, Gauss.Pow(new Gauss(5, 3), 0));
    }

    [Fact]
    public void Pow_NegativeExponent_ReturnsReciprocal()
    {
        var g = new Gauss(2, 1, 0, 1); // 2
        var result = Gauss.Pow(g, -1);
        Assert.Equal(new Gauss(1, 2, 0, 1), result); // 1/2
    }

    [Fact]
    public void FractionArithmetic_StaysExact()
    {
        var a = new Gauss(1, 3, 0, 1); // 1/3
        var b = new Gauss(1, 6, 0, 1); // 1/6
        var sum = a + b;
        Assert.Equal(new Gauss(1, 2, 0, 1), sum); // 1/3 + 1/6 = 1/2
    }

    #endregion

    #region Properties

    [Fact]
    public void Conjugate()
    {
        var g = new Gauss(3, 1, 2, 1);
        Assert.Equal(new Gauss(3, 1, -2, 1), g.Conjugate);
    }

    [Fact]
    public void MagnitudeSquared()
    {
        var g = new Gauss(3, 1, 4, 1);
        Assert.Equal(new Gauss(25), g.MagnitudeSquared); // 9 + 16 = 25
    }

    [Fact]
    public void Norm_EquivalentToMagnitudeSquared()
    {
        var g = new Gauss(3, 1, 4, 1);
        Assert.Equal(g.MagnitudeSquared, g.Norm);
    }

    [Fact]
    public void Reciprocal()
    {
        var g = new Gauss(3, 1, 4, 1); // 3+4i
        var recip = g.Reciprocal;
        Assert.Equal(Gauss.One, g * recip);
    }

    [Fact]
    public void Inverse_EquivalentToReciprocal()
    {
        var g = new Gauss(3, 1, 4, 1);
        Assert.Equal(g.Reciprocal, g.Inverse);
    }

    [Fact]
    public void RealPart()
    {
        var g = new Gauss(3, 2, 5, 7);
        Assert.Equal(new Gauss(3, 2, 0, 1), g.RealPart);
    }

    [Fact]
    public void ImaginaryPart()
    {
        var g = new Gauss(3, 2, 5, 7);
        Assert.Equal(new Gauss(5, 7, 0, 1), g.ImaginaryPart);
    }

    [Fact]
    public void IsZero_And_IsOne()
    {
        Assert.True(Gauss.Zero.IsZero);
        Assert.False(Gauss.Zero.IsOne);
        Assert.True(Gauss.One.IsOne);
        Assert.False(Gauss.One.IsZero);
    }

    [Fact]
    public void IsReal_TrueForRealNumbers()
    {
        Assert.True(new Gauss(5).IsReal);
        Assert.True(new Gauss(1, 2, 0, 1).IsReal);
        Assert.False(Gauss.I.IsReal);
    }

    [Fact]
    public void IsPurelyImaginary()
    {
        Assert.True(Gauss.I.IsPurelyImaginary);
        Assert.False(Gauss.One.IsPurelyImaginary);
        Assert.False(Gauss.Zero.IsPurelyImaginary);
    }

    [Fact]
    public void IsInteger()
    {
        Assert.True(new Gauss(5).IsInteger);
        Assert.False(new Gauss(1, 2, 0, 1).IsInteger);
        Assert.False(Gauss.I.IsInteger);
    }

    [Fact]
    public void IsGaussianInteger()
    {
        Assert.True(new Gauss(3, 1, 2, 1).IsGaussianInteger);
        Assert.False(new Gauss(1, 2, 1, 1).IsGaussianInteger);
    }

    [Fact]
    public void Magnitude_ReturnsDouble()
    {
        var g = new Gauss(3, 1, 4, 1);
        Assert.Equal(5.0, g.Magnitude, 1e-10);
    }

    [Fact]
    public void Phase_ReturnsAngle()
    {
        var g = new Gauss(1, 1, 1, 1); // 1+i
        Assert.Equal(Math.PI / 4, g.Phase, 1e-10);
    }

    [Fact]
    public void ToPolar_ReturnsCorrectValues()
    {
        var g = new Gauss(3, 1, 4, 1);
        var (mag, phase) = g.ToPolar();
        Assert.Equal(5.0, mag, 1e-10);
        Assert.Equal(Math.Atan2(4.0, 3.0), phase, 1e-10);
    }

    [Fact]
    public void FromPolar_CreatesApproximation()
    {
        var g = Gauss.FromPolar(5.0, Math.Atan2(4.0, 3.0));
        // Should be approximately 3+4i
        Assert.Equal(3.0, (double)g.A / (double)g.B, 1e-10);
        Assert.Equal(4.0, (double)g.C / (double)g.D, 1e-10);
    }

    #endregion

    #region Units and Associates

    [Fact]
    public void Eye_ReturnsI()
    {
        Assert.Equal(Gauss.I, Gauss.Eye());
    }

    [Fact]
    public void GaussUnits_ReturnsFourUnits()
    {
        var units = Gauss.GaussUnits();
        Assert.Equal(4, units.Length);
        Assert.Contains(Gauss.One, units);
        Assert.Contains(Gauss.MinusOne, units);
        Assert.Contains(Gauss.I, units);
        Assert.Contains(-Gauss.I, units);
    }

    [Fact]
    public void Associates_ReturnsThreeAssociates()
    {
        var g = new Gauss(3, 1, 2, 1);
        var assoc = g.Associates();
        Assert.Equal(3, assoc.Length);
        Assert.Contains(-g, assoc);
        Assert.Contains(g * Gauss.I, assoc);
        Assert.Contains(g * -Gauss.I, assoc);
    }

    [Fact]
    public void IsAssociate_TrueForNegation()
    {
        var g = new Gauss(3, 1, 2, 1);
        Assert.True(g.IsAssociate(-g));
    }

    [Fact]
    public void IsAssociate_FalseForDifferent()
    {
        var a = new Gauss(3, 1, 2, 1);
        var b = new Gauss(5, 1, 1, 1);
        Assert.False(a.IsAssociate(b));
    }

    #endregion

    #region Static Methods

    [Fact]
    public void Abs_RealNumber()
    {
        Assert.Equal((Gauss)5, Gauss.Abs(new Gauss(-5)));
    }

    [Fact]
    public void Abs_Complex_ReturnsMagnitudeSquared()
    {
        var g = new Gauss(3, 1, 4, 1);
        Assert.Equal(g.MagnitudeSquared, Gauss.Abs(g));
    }

    [Fact]
    public void Sign_ReturnsSignOfRealPart()
    {
        Assert.Equal(1, Gauss.Sign(new Gauss(5)));
        Assert.Equal(-1, Gauss.Sign(new Gauss(-3)));
        Assert.Equal(0, Gauss.Sign(Gauss.Zero));
    }

    [Fact]
    public void Floor_RealNumber()
    {
        Assert.Equal(new Gauss(1), Gauss.Floor(new Gauss(3, 2, 0, 1))); // floor(3/2) = 1
        Assert.Equal(new Gauss(-2), Gauss.Floor(new Gauss(-3, 2, 0, 1))); // floor(-3/2) = -2
    }

    [Fact]
    public void Ceiling_RealNumber()
    {
        Assert.Equal(new Gauss(2), Gauss.Ceiling(new Gauss(3, 2, 0, 1))); // ceil(3/2) = 2
        Assert.Equal(new Gauss(-1), Gauss.Ceiling(new Gauss(-3, 2, 0, 1))); // ceil(-3/2) = -1
    }

    [Fact]
    public void Truncate_RealNumber()
    {
        Assert.Equal(new Gauss(1), Gauss.Truncate(new Gauss(3, 2, 0, 1)));
        Assert.Equal(new Gauss(-1), Gauss.Truncate(new Gauss(-3, 2, 0, 1)));
    }

    [Fact]
    public void Min_ReturnsSmaller()
    {
        Assert.Equal(Gauss.One, Gauss.Min(Gauss.One, (Gauss)5));
    }

    [Fact]
    public void Max_ReturnsLarger()
    {
        Assert.Equal((Gauss)5, Gauss.Max(Gauss.One, (Gauss)5));
    }

    [Fact]
    public void Clamp()
    {
        Assert.Equal((Gauss)3, Gauss.Clamp((Gauss)3, Gauss.One, (Gauss)5));
        Assert.Equal(Gauss.One, Gauss.Clamp(Gauss.Zero, Gauss.One, (Gauss)5));
        Assert.Equal((Gauss)5, Gauss.Clamp((Gauss)10, Gauss.One, (Gauss)5));
    }

    #endregion

    #region String Representations

    [Fact]
    public void ToString_Integer()
    {
        Assert.Equal("5", new Gauss(5).ToString());
    }

    [Fact]
    public void ToString_Fraction()
    {
        Assert.Equal("1/2", new Gauss(1, 2, 0, 1).ToString());
    }

    [Fact]
    public void ToString_MixedFraction()
    {
        Assert.Equal("1 & 1/2", new Gauss(3, 2, 0, 1).ToString());
    }

    [Fact]
    public void ToString_Complex()
    {
        Assert.Equal("3 + 2i", new Gauss(3, 1, 2, 1).ToString());
    }

    [Fact]
    public void ToString_ImaginaryUnit()
    {
        Assert.Equal("i", Gauss.I.ToString());
    }

    [Fact]
    public void ToString_NegativeImaginary()
    {
        Assert.Equal("3 - 2i", new Gauss(3, 1, -2, 1).ToString());
    }

    [Fact]
    public void ToRawString()
    {
        Assert.Equal("<3,2,1,4>", new Gauss(3, 2, 1, 4).ToRawString());
    }

    [Fact]
    public void ToImproperFractionString()
    {
        Assert.Equal("3/2", new Gauss(3, 2, 0, 1).ToImproperFractionString());
        Assert.Equal("3/2 + i", new Gauss(3, 2, 1, 1).ToImproperFractionString());
    }

    [Fact]
    public void ToDecimalString_RealNumber()
    {
        Assert.Equal("0.5000", new Gauss(1, 2, 0, 1).ToDecimalString(4));
    }

    [Fact]
    public void ToDecimalString_ComplexNumber()
    {
        var result = new Gauss(1, 2, 1, 3).ToDecimalString(4);
        Assert.Equal("0.5000 + 0.3333i", result);
    }

    [Fact]
    public void ToDecimalString_NegativeImaginary()
    {
        var result = new Gauss(1, 1, -1, 2).ToDecimalString(2);
        Assert.Equal("1.00 - 0.50i", result);
    }

    [Fact]
    public void IFormattable_Formats()
    {
        var g = new Gauss(3, 2, 1, 1);
        Assert.Equal(g.ToString(), g.ToString("G"));
        Assert.Equal(g.ToRawString(), g.ToString("R"));
        Assert.Equal(g.ToImproperFractionString(), g.ToString("I"));
        Assert.Equal(g.ToDecimalString(), g.ToString("D"));
        Assert.Equal(g.ToDecimalString(), g.ToString("F"));
    }

    #endregion

    #region Parsing

    [Fact]
    public void Parse_ValidString()
    {
        var g = Gauss.Parse("3,2,1,4");
        Assert.Equal(new Gauss(3, 2, 1, 4), g);
    }

    [Fact]
    public void Parse_ThrowsOnInvalidFormat()
    {
        Assert.Throws<FormatException>(() => Gauss.Parse("1,2,3"));
    }

    [Fact]
    public void TryParse_ValidString()
    {
        Assert.True(Gauss.TryParse("1,2,3,4", out var result));
        Assert.Equal(new Gauss(1, 2, 3, 4), result);
    }

    [Fact]
    public void TryParse_InvalidString()
    {
        Assert.False(Gauss.TryParse("invalid", out _));
    }

    #endregion

    #region Equality and Comparison

    [Fact]
    public void Equality_NormalizedValues()
    {
        Assert.Equal(new Gauss(2, 4, 0, 1), new Gauss(1, 2, 0, 1)); // 2/4 = 1/2
    }

    [Fact]
    public void Comparison_ByRealThenImag()
    {
        Assert.True(new Gauss(1) < new Gauss(2));
        Assert.True(new Gauss(1, 1, 1, 1) < new Gauss(1, 1, 2, 1));
    }

    [Fact]
    public void HashCode_EqualForEqualValues()
    {
        Assert.Equal(
            new Gauss(2, 4, 0, 1).GetHashCode(),
            new Gauss(1, 2, 0, 1).GetHashCode());
    }

    #endregion

    #region Pramana Integration

    [Fact]
    public void PramanaId_Format()
    {
        Assert.Equal("pra:num:1,1,0,1", Gauss.One.PramanaId);
        Assert.Equal("pra:num:3,1,2,1", new Gauss(3, 1, 2, 1).PramanaId);
        Assert.Equal("pra:num:1,2,0,1", new Gauss(1, 2, 0, 1).PramanaId);
    }

    [Fact]
    public void PramanaGuid_Deterministic()
    {
        var a = new Gauss(3, 1, 2, 1);
        var b = new Gauss(3, 1, 2, 1);
        Assert.Equal(a.PramanaGuid, b.PramanaGuid);
    }

    [Fact]
    public void PramanaGuid_DifferentForDifferentValues()
    {
        Assert.NotEqual(Gauss.One.PramanaGuid, Gauss.Zero.PramanaGuid);
    }

    [Fact]
    public void PramanaUrl_UsesNonHashedForm()
    {
        Assert.Equal("https://pramana.dev/entity/pra:num:1,1,0,1", Gauss.One.PramanaUrl);
    }

    [Fact]
    public void PramanaHashUrl_UsesUuid()
    {
        var g = Gauss.One;
        Assert.Contains(g.PramanaGuid.ToString(), g.PramanaHashUrl);
    }

    #endregion
}
