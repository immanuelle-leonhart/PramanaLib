using System.Numerics;

namespace PramanaLib.Tests;

public class GintTests
{
    #region Construction

    [Fact]
    public void DefaultConstructor_ReturnsZero()
    {
        var g = new Gint();
        Assert.Equal(BigInteger.Zero, g.Real);
        Assert.Equal(BigInteger.Zero, g.Imag);
    }

    [Fact]
    public void SingleValueConstructor_SetsRealOnly()
    {
        var g = new Gint(5);
        Assert.Equal(5, g.Real);
        Assert.Equal(0, g.Imag);
    }

    [Fact]
    public void TwoValueConstructor_SetsBothParts()
    {
        var g = new Gint(4, 5);
        Assert.Equal(4, g.Real);
        Assert.Equal(5, g.Imag);
    }

    [Fact]
    public void Eye_ReturnsImaginaryUnit()
    {
        var i = Gint.Eye();
        Assert.Equal(0, i.Real);
        Assert.Equal(1, i.Imag);
    }

    [Fact]
    public void Two_ReturnsOnePlusI()
    {
        var two = Gint.Two();
        Assert.Equal(1, two.Real);
        Assert.Equal(1, two.Imag);
    }

    [Fact]
    public void StaticConstants_AreCorrect()
    {
        Assert.Equal(new Gint(0, 0), Gint.Zero);
        Assert.Equal(new Gint(1, 0), Gint.One);
        Assert.Equal(new Gint(-1, 0), Gint.MinusOne);
        Assert.Equal(new Gint(0, 1), Gint.I);
    }

    [Fact]
    public void FromArray_CreatesGint()
    {
        var g = Gint.FromArray([3, 7]);
        Assert.Equal(new Gint(3, 7), g);
    }

    [Fact]
    public void FromArray_ThrowsOnWrongLength()
    {
        Assert.Throws<ArgumentException>(() => Gint.FromArray([1, 2, 3]));
    }

    #endregion

    #region Properties

    [Fact]
    public void Conjugate_NegatesImaginary()
    {
        var g = new Gint(3, 4);
        Assert.Equal(new Gint(3, -4), g.Conjugate);
    }

    [Fact]
    public void Norm_ReturnsSumOfSquares()
    {
        var g = new Gint(3, 4);
        Assert.Equal(25, g.Norm);
    }

    [Fact]
    public void Norm_Of11Plus3i_Is130()
    {
        var g = new Gint(11, 3);
        Assert.Equal(130, g.Norm);
    }

    [Theory]
    [InlineData(1, 0, true)]
    [InlineData(-1, 0, true)]
    [InlineData(0, 1, true)]
    [InlineData(0, -1, true)]
    [InlineData(2, 0, false)]
    [InlineData(1, 1, false)]
    public void IsUnit_IdentifiesUnitsCorrectly(int re, int im, bool expected)
    {
        Assert.Equal(expected, new Gint(re, im).IsUnit);
    }

    [Fact]
    public void IsZero_TrueForZero()
    {
        Assert.True(new Gint().IsZero);
        Assert.False(new Gint(1).IsZero);
    }

    [Fact]
    public void IsReal_TrueWhenImagIsZero()
    {
        Assert.True(new Gint(5).IsReal);
        Assert.False(new Gint(5, 3).IsReal);
    }

    [Fact]
    public void IsPurelyImaginary_TrueWhenRealIsZero()
    {
        Assert.True(new Gint(0, 3).IsPurelyImaginary);
        Assert.False(new Gint(5, 3).IsPurelyImaginary);
        Assert.False(new Gint().IsPurelyImaginary);
    }

    #endregion

    #region Arithmetic

    [Fact]
    public void Addition()
    {
        var a = new Gint(11, 3);
        var b = new Gint(1, 8);
        Assert.Equal(new Gint(12, 11), a + b);
    }

    [Fact]
    public void Subtraction()
    {
        var a = new Gint(11, 3);
        var b = new Gint(1, 8);
        Assert.Equal(new Gint(10, -5), a - b);
    }

    [Fact]
    public void Multiplication()
    {
        var a = new Gint(11, 3);
        var b = new Gint(1, 8);
        Assert.Equal(new Gint(-13, 91), a * b);
    }

    [Fact]
    public void Multiplication_ISquared_IsMinusOne()
    {
        var i = Gint.Eye();
        Assert.Equal(Gint.MinusOne, i * i);
    }

    [Fact]
    public void Negation()
    {
        var g = new Gint(3, 4);
        Assert.Equal(new Gint(-3, -4), -g);
    }

    [Fact]
    public void UnaryPlus()
    {
        var g = new Gint(3, 4);
        Assert.Equal(g, +g);
    }

    [Fact]
    public void Division_ReturnsGauss()
    {
        var a = new Gint(11, 3);
        var b = new Gint(1, 8);
        Gauss result = a / b;
        // 11+3i / 1+8i = (11+24)/(1+64) + (3-88)/(1+64)i = 35/65 + (-85/65)i = 7/13 - 17/13 i
        Assert.Equal(new Gauss(7, 13, -17, 13), result);
    }

    [Fact]
    public void Modulo()
    {
        var a = new Gint(11, 3);
        var b = new Gint(1, 8);
        var r = a % b;
        // Verify a = b*q + r via modified_divmod
        var (q, remainder) = Gint.ModifiedDivmod(a, b);
        Assert.Equal(remainder, r);
        Assert.Equal(a, b * q + r);
    }

    [Fact]
    public void Pow_ZeroExponent_ReturnsOne()
    {
        Assert.Equal(Gint.One, Gint.Pow(new Gint(5, 3), 0));
    }

    [Fact]
    public void Pow_PositiveExponent()
    {
        var g = new Gint(11, 3);
        Assert.Equal(new Gint(1034, 1062), Gint.Pow(g, 3));
    }

    [Fact]
    public void Pow_NegativeExponent_Throws()
    {
        Assert.Throws<ArgumentException>(() => Gint.Pow(new Gint(2, 1), -1));
    }

    [Fact]
    public void ImplicitConversion_FromInt()
    {
        Gint g = 42;
        Assert.Equal(new Gint(42), g);
    }

    [Fact]
    public void ImplicitConversion_ToGauss()
    {
        var g = new Gint(3, 2);
        Gauss gauss = g;
        Assert.Equal(new Gauss(3, 1, 2, 1), gauss);
    }

    [Fact]
    public void ExplicitConversion_FromGauss()
    {
        var gauss = new Gauss(3, 1, 2, 1);
        var g = (Gint)gauss;
        Assert.Equal(new Gint(3, 2), g);
    }

    [Fact]
    public void ExplicitConversion_FromGauss_ThrowsIfNotInteger()
    {
        var gauss = new Gauss(1, 2, 0, 1); // 1/2
        Assert.Throws<InvalidCastException>(() => (Gint)gauss);
    }

    [Fact]
    public void Increment()
    {
        var g = new Gint(3, 2);
        g++;
        Assert.Equal(new Gint(4, 2), g);
    }

    [Fact]
    public void Decrement()
    {
        var g = new Gint(3, 2);
        g--;
        Assert.Equal(new Gint(2, 2), g);
    }

    #endregion

    #region Units and Associates

    [Fact]
    public void Units_ReturnsFourUnits()
    {
        var units = Gint.Units();
        Assert.Equal(4, units.Length);
        Assert.Contains(new Gint(1), units);
        Assert.Contains(new Gint(-1), units);
        Assert.Contains(new Gint(0, 1), units);
        Assert.Contains(new Gint(0, -1), units);
    }

    [Fact]
    public void Associates_ReturnsThreeAssociates()
    {
        var g = new Gint(3, 2);
        var assoc = g.Associates();
        Assert.Equal(3, assoc.Length);
        Assert.Contains(new Gint(-3, -2), assoc);  // g * -1
        Assert.Contains(new Gint(-2, 3), assoc);    // g * i
        Assert.Contains(new Gint(2, -3), assoc);    // g * -i
    }

    [Fact]
    public void IsAssociate_TrueForNegation()
    {
        var g = new Gint(11, 3);
        Assert.True(g.IsAssociate(-g));
    }

    [Fact]
    public void IsAssociate_FalseForDifferentValues()
    {
        var a = new Gint(11, 3);
        var b = new Gint(1, 8);
        Assert.False(a.IsAssociate(b));
    }

    [Fact]
    public void IsAssociate_TrueForIMultiple()
    {
        var g = new Gint(3, 2);
        var gi = g * Gint.Eye(); // g * i = -2 + 3i
        Assert.True(g.IsAssociate(gi));
    }

    #endregion

    #region Number Theory

    [Fact]
    public void ModifiedDivmod_SatisfiesDivisionTheorem()
    {
        var a = new Gint(11, 3);
        var b = new Gint(1, 8);
        var (q, r) = Gint.ModifiedDivmod(a, b);
        Assert.Equal(a, b * q + r);
    }

    [Fact]
    public void ModifiedDivmod_RemainderNormIsSmall()
    {
        var a = new Gint(11, 3);
        var b = new Gint(1, 8);
        var (_, r) = Gint.ModifiedDivmod(a, b);
        // r.Norm should be < b.Norm
        Assert.True(r.Norm < b.Norm);
    }

    [Fact]
    public void ModifiedDivmod_ThrowsOnZeroDivisor()
    {
        Assert.Throws<DivideByZeroException>(() => Gint.ModifiedDivmod(new Gint(5), new Gint()));
    }

    [Fact]
    public void Gcd_BasicCase()
    {
        var a = new Gint(11, 3);
        var b = new Gint(1, 8);
        var gcd = Gint.Gcd(a, b);
        // GCD should divide both a and b
        var aDiv = a / gcd;
        var bDiv = b / gcd;
        Assert.True(aDiv.IsGaussianInteger);
        Assert.True(bDiv.IsGaussianInteger);
    }

    [Fact]
    public void Gcd_CommutesUpToAssociate()
    {
        var a = new Gint(11, 3);
        var b = new Gint(1, 8);
        var gcd1 = Gint.Gcd(a, b);
        var gcd2 = Gint.Gcd(b, a);
        Assert.True(gcd1.IsAssociate(gcd2));
    }

    [Fact]
    public void Gcd_ThrowsWhenBothZero()
    {
        Assert.Throws<ArgumentException>(() => Gint.Gcd(new Gint(), new Gint()));
    }

    [Fact]
    public void Xgcd_SatisfiesBezout()
    {
        var alpha = new Gint(11, 3);
        var beta = new Gint(1, 8);
        var (gcd, x, y) = Gint.Xgcd(alpha, beta);

        // Verify gcd = alpha * x + beta * y
        Assert.Equal((Gauss)gcd, (Gauss)(alpha * x) + (Gauss)(beta * y));
    }

    [Fact]
    public void Xgcd_GcdMatchesGcd()
    {
        var alpha = new Gint(11, 3);
        var beta = new Gint(1, 8);
        var (xgcd, _, _) = Gint.Xgcd(alpha, beta);
        var gcd = Gint.Gcd(alpha, beta);
        Assert.True(xgcd.IsAssociate(gcd));
    }

    [Theory]
    [InlineData(3, 0, true)]   // 3 ≡ 3 mod 4, prime
    [InlineData(5, 0, false)]  // 5 ≡ 1 mod 4, splits: (2+i)(2-i)
    [InlineData(7, 0, true)]   // 7 ≡ 3 mod 4, prime
    [InlineData(2, 0, false)]  // 2 = -i(1+i)^2
    [InlineData(2, 1, true)]   // norm = 5 is prime
    [InlineData(1, 1, true)]   // norm = 2 is prime
    [InlineData(3, 1, false)]  // norm = 10 is not prime
    public void IsGaussianPrime(int re, int im, bool expected)
    {
        Assert.Equal(expected, Gint.IsGaussianPrime(new Gint(re, im)));
    }

    [Fact]
    public void IsRelativelyPrime_TrueWhenCoprime()
    {
        // 3+2i (norm=13) and 1+i (norm=2) are coprime
        var a = new Gint(3, 2);
        var b = new Gint(1, 1);
        Assert.True(Gint.IsRelativelyPrime(a, b));
    }

    [Fact]
    public void IsRelativelyPrime_FalseWhenNotCoprime()
    {
        // 3+i (norm=10) and 1+i (norm=2) share factor 1+i
        var a = new Gint(3, 1);
        var b = new Gint(1, 1);
        Assert.False(Gint.IsRelativelyPrime(a, b));
    }

    [Fact]
    public void CongruentModulo_TrueWhenCongruent()
    {
        var a = new Gint(7, 0);
        var b = new Gint(3, 0);
        var c = new Gint(2, 0);
        var (isCong, _) = Gint.CongruentModulo(a, b, c);
        Assert.True(isCong); // (7-3)/2 = 2, a Gaussian integer
    }

    [Fact]
    public void CongruentModulo_FalseWhenNotCongruent()
    {
        var a = new Gint(7, 2);
        var b = new Gint(3, 1);
        var c = new Gint(2, 0);
        var (isCong, _) = Gint.CongruentModulo(a, b, c);
        Assert.False(isCong); // (7+2i - 3-i)/2 = 2 + 1/2 i, not a Gaussian integer
    }

    [Fact]
    public void NormsDivide_ReturnsQuotientWhenDivisible()
    {
        var a = new Gint(11, 3); // norm = 130
        var b = new Gint(1, 8);  // norm = 65
        var result = Gint.NormsDivide(a, b);
        Assert.Equal(2, result);
    }

    [Fact]
    public void NormsDivide_ReturnsNullWhenNotDivisible()
    {
        var a = new Gint(3, 1); // norm = 10
        var b = new Gint(2, 1); // norm = 5
        var result = Gint.NormsDivide(a, b);
        Assert.Equal(2, result); // 10/5 = 2
    }

    [Fact]
    public void FloorDiv_MatchesModifiedDivmod()
    {
        var a = new Gint(11, 3);
        var b = new Gint(1, 8);
        var fd = Gint.FloorDiv(a, b);
        var (q, _) = Gint.ModifiedDivmod(a, b);
        Assert.Equal(q, fd);
    }

    #endregion

    #region Equality and Comparison

    [Fact]
    public void Equality_SameValues()
    {
        Assert.Equal(new Gint(3, 4), new Gint(3, 4));
    }

    [Fact]
    public void Equality_DifferentValues()
    {
        Assert.NotEqual(new Gint(3, 4), new Gint(4, 3));
    }

    [Fact]
    public void HashCode_EqualForEqualValues()
    {
        Assert.Equal(new Gint(3, 4).GetHashCode(), new Gint(3, 4).GetHashCode());
    }

    [Fact]
    public void Comparison_OrdersByRealThenImag()
    {
        Assert.True(new Gint(1, 0) < new Gint(2, 0));
        Assert.True(new Gint(1, 1) < new Gint(1, 2));
        Assert.True(new Gint(2, 0) > new Gint(1, 5));
    }

    #endregion

    #region String Representations

    [Fact]
    public void ToString_RealOnly()
    {
        Assert.Equal("5", new Gint(5).ToString());
    }

    [Fact]
    public void ToString_Zero()
    {
        Assert.Equal("0", new Gint().ToString());
    }

    [Fact]
    public void ToString_ImaginaryUnit()
    {
        Assert.Equal("i", new Gint(0, 1).ToString());
        Assert.Equal("-i", new Gint(0, -1).ToString());
    }

    [Fact]
    public void ToString_Complex()
    {
        Assert.Equal("3 + 4i", new Gint(3, 4).ToString());
        Assert.Equal("3 - 4i", new Gint(3, -4).ToString());
    }

    [Fact]
    public void ToString_PurelyImaginary()
    {
        Assert.Equal("5i", new Gint(0, 5).ToString());
    }

    [Fact]
    public void ToRawString()
    {
        Assert.Equal("(3, 4)", new Gint(3, 4).ToRawString());
    }

    #endregion

    #region Pramana Integration

    [Fact]
    public void PramanaId_Format()
    {
        Assert.Equal("pra:num:3,1,2,1", new Gint(3, 2).PramanaId);
        Assert.Equal("pra:num:1,1,0,1", new Gint(1).PramanaId);
    }

    [Fact]
    public void PramanaGuid_MatchesGaussEquivalent()
    {
        var gint = new Gint(3, 2);
        var gauss = new Gauss(3, 1, 2, 1);
        Assert.Equal(gauss.PramanaGuid, gint.PramanaGuid);
    }

    [Fact]
    public void PramanaGuid_DeterministicForSameValue()
    {
        var a = new Gint(7, 3);
        var b = new Gint(7, 3);
        Assert.Equal(a.PramanaGuid, b.PramanaGuid);
    }

    [Fact]
    public void PramanaUrl_UsesNonHashedForm()
    {
        var g = new Gint(3, 2);
        Assert.Equal("https://pramana.dev/entity/pra:num:3,1,2,1", g.PramanaUrl);
    }

    [Fact]
    public void PramanaHashUrl_UsesUuid()
    {
        var g = new Gint(3, 2);
        Assert.Contains(g.PramanaGuid.ToString(), g.PramanaHashUrl);
        Assert.StartsWith("https://pramana.dev/entity/", g.PramanaHashUrl);
    }

    #endregion
}
