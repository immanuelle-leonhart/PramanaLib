# PramanaLib

C# runtime library for Pramana pseudo-classes. Provides exact-arithmetic value types that generate deterministic Pramana IDs, staying consistent with the canonical Python implementation in the [Pramana](https://github.com/Emma-Leonhart/Pramana) knowledge graph.

## What are Pramana pseudo-classes?

Pramana uses **pseudo-classes** — deterministic namespace schemes that auto-generate UUID v5 identifiers from canonical value strings. A pseudo-class value (e.g. `num:3,4,0,1`) doesn't need to exist in the graph database beforehand; it can be resolved, classified, and linked on the fly.

PramanaLib implements the `num:` pseudo-class as a full C# struct with exact arithmetic, and is designed to host additional pseudo-class implementations as needed.

## Pseudo-class coverage

| Pseudo-class | Pramana namespace UUID | PramanaLib type | Status |
|---|---|---|---|
| `num:` | `a6613321-e9f6-4348-8f8b-29d2a3c86349` | `GaussianRational` | Implemented |
| `date:` | `924b23ca-50d5-46d5-b974-ea01d33f4e13` | — | Pramana-side only |
| `time:` | `0802c070-38cb-482e-9aac-81578f3c8463` | — | Pramana-side only |
| `interval:` | `ff206534-16f0-4e90-8286-4c730e63f86a` | — | Pramana-side only |
| `chem:` / `inchi:` | `8b526203-d299-4288-9760-ecaca9d4e1c0` | — | Pramana-side only |
| `element:` | `9a850d17-368c-4613-a34a-83f6b2159581` | — | Pramana-side only |

The `date:`, `time:`, `interval:`, `chem:`, and `element:` pseudo-classes are handled in the Pramana web app. They are listed here for reference but do not currently need dedicated C# types — the `num:` pseudo-class is the only one whose semantics (exact rational arithmetic, complex number algebra, number-hierarchy classification) justify a standalone library type.

## GaussianRational

`GaussianRational` is an immutable `readonly struct` representing a Gaussian rational number `a/b + (c/d)i` with arbitrary-precision integer components (`BigInteger`). It is the C# implementation of Pramana's `num:` pseudo-class.

### Key features

- **Exact arithmetic** — all operations (+, -, \*, /, %) are performed in rational form with no floating-point precision loss.
- **Automatic normalization** — fractions are always reduced to lowest terms with positive denominators.
- **Deterministic PramanaId** — each value produces a UUID v5 that matches what the Pramana web app generates for the same `num:` string.
- **Precision-first design** — methods that would produce irrational results (`Magnitude`, `Phase`, `ToPolar`) throw `NotSupportedException` rather than silently losing precision.

### Quick start

```csharp
using PramanaLib;

// From integers
GaussianRational five = 5;                                    // num:5
GaussianRational half = new GaussianRational(1, 2, 0, 1);    // num:1,2 → 1/2
GaussianRational z = new GaussianRational(1, 1, 1, 1);       // num:1,1,1,1 → 1 + i

// Arithmetic
var sum = half + z;           // 1 & 1/2 + i
var product = z * z;          // 2i
var quotient = five / half;   // 10

// Identity
Console.WriteLine(five.PramanaId);   // deterministic UUID v5
Console.WriteLine(five.PramanaUrl);  // https://pramana-data.ca/entity/{uuid}

// Formatting
Console.WriteLine(z.ToString());              // "1 + i"
Console.WriteLine(z.ToRawString());           // "<1,1,1,1>"
Console.WriteLine(z.ToImproperFractionString()); // "1 + i"

// Properties
Console.WriteLine(five.IsInteger);       // True
Console.WriteLine(z.IsGaussianInteger);  // True
Console.WriteLine(half.IsReal);          // True

// Complex operations
var conj = z.Conjugate;                  // 1 - i
var magSq = z.MagnitudeSquared;          // 2 (exact)
```

### Conversions

| Direction | Types | Notes |
|---|---|---|
| Implicit from | `int`, `long`, `BigInteger` | Lossless |
| Implicit from | `float`, `double` | Best rational approximation via continued fractions |
| Implicit from | `decimal` | Lossless (extracts exact mantissa/scale) |
| Explicit to | `int`, `long`, `BigInteger` | Throws if not a real integer |
| Explicit to | `float`, `double`, `decimal` | Throws if not real; may lose precision |
| Explicit to | `double[]`, `int[]`, `BigInteger[]` | Array of [real, imaginary] |

### Number hierarchy

When resolved through Pramana, each `num:` value is automatically classified:

| Value | Display | Classification |
|---|---|---|
| `num:42` | 42 | Natural Number |
| `num:0` | 0 | Whole Number |
| `num:-5` | -5 | Integer |
| `num:3,4` | 3/4 | Rational Number |
| `num:1,1,1,1` | 1 + i | Gaussian Rational |

### Parsing

```csharp
// From canonical form "a,b,c,d"
var parsed = GaussianRational.Parse("3,4,0,1");   // 3/4

// Safe parsing
if (GaussianRational.TryParse("1,2,3,4", out var result))
    Console.WriteLine(result);  // "1/2 + 3/4 i"
```

## Building

```bash
dotnet build
dotnet run --project Test
```

Requires .NET 8.0+.

## Project structure

```
PramanaLib/
  GaussianRational.cs   # num: pseudo-class implementation
  Class1.cs             # placeholder
Test/
  Program.cs            # usage examples
```

## Relationship to Pramana

This library is a companion to the [Pramana](https://github.com/Emma-Leonhart/Pramana) knowledge graph. The Python side defines pseudo-class routing, virtual entity generation, and number classification in `src/combinatoric_classes.py` and `web/app.py`. PramanaLib provides the same canonical normalization and UUID generation in C# so that .NET consumers can work with Pramana values natively.
