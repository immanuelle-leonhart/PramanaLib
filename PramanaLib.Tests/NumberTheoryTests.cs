using System.Numerics;

namespace PramanaLib.Tests;

public class NumberTheoryTests
{
    [Theory]
    [InlineData(2, true)]
    [InlineData(3, true)]
    [InlineData(5, true)]
    [InlineData(7, true)]
    [InlineData(11, true)]
    [InlineData(13, true)]
    [InlineData(17, true)]
    [InlineData(19, true)]
    [InlineData(23, true)]
    [InlineData(97, true)]
    [InlineData(0, false)]
    [InlineData(1, false)]
    [InlineData(4, false)]
    [InlineData(6, false)]
    [InlineData(9, false)]
    [InlineData(15, false)]
    [InlineData(100, false)]
    [InlineData(-3, false)]
    public void IsPrime(int n, bool expected)
    {
        Assert.Equal(expected, NumberTheory.IsPrime(n));
    }

    [Fact]
    public void IsPrime_LargeNumber()
    {
        // 104729 is prime
        Assert.True(NumberTheory.IsPrime(104729));
        // 104730 is not
        Assert.False(NumberTheory.IsPrime(104730));
    }
}
