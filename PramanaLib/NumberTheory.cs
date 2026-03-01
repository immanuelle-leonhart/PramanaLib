using System.Numerics;

namespace PramanaLib;

/// <summary>
/// Utility methods for number theory operations used by <see cref="Gint"/> and <see cref="Gauss"/>.
/// </summary>
public static class NumberTheory
{
    /// <summary>
    /// Returns true if <paramref name="n"/> is a prime number.
    /// </summary>
    /// <remarks>
    /// Uses trial division up to sqrt(n). For very large numbers, consider
    /// using a more efficient algorithm.
    /// </remarks>
    /// <param name="n">The integer to test for primality.</param>
    /// <returns>True if <paramref name="n"/> is prime; false otherwise.</returns>
    public static bool IsPrime(BigInteger n)
    {
        if (n < 2) return false;
        if (n == 2) return true;
        if (n % 2 == 0) return false;
        if (n == 3) return true;
        if (n % 3 == 0) return false;

        // Trial division up to sqrt(n)
        // For BigInteger, we compute i*i <= n instead of using sqrt
        for (BigInteger i = 5; i * i <= n; i += 6)
        {
            if (n % i == 0) return false;
            if (n % (i + 2) == 0) return false;
        }

        return true;
    }
}
