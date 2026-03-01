# PramanaLib

C# runtime library for [Pramana](https://pramana.dev) pseudo-classes. Provides exact-arithmetic value types that generate deterministic Pramana IDs, staying consistent with the canonical Python implementation in the Pramana knowledge graph.

## What is Pramana?

**Pramana** is a knowledge graph that stores entities, properties, and propositions as interconnected items identified by UUIDs. It powers a web interface (built on FastAPI) where every entity — from chemical elements to mathematical constants — has a browsable page at its canonical URL. Data is stored in `.pra` files (JSON) and served through a GraphDB-backed API.

A core design principle is that certain categories of values don't need to be manually entered into the graph. Instead, **pseudo-classes** generate them deterministically.

## What are pseudo-classes?

Pramana uses **pseudo-classes** — deterministic namespace schemes that auto-generate UUID v5 identifiers from canonical value strings. A pseudo-class value (e.g. `num:3,4,0,1`) doesn't need to exist in the graph database beforehand; it can be resolved, classified, and linked on the fly. When you visit `/data/num:42` in the Pramana web app, the system canonicalizes the input, generates a UUID v5 from the namespace, classifies the number in the hierarchy (Natural Number > Whole Number > Integer > ...), and renders a full entity page — all without a pre-existing database row.

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

### Usage examples

#### Creating values

```csharp
using PramanaLib;

// From integers (implicit conversion)
GaussianRational five = 5;
GaussianRational big = (long)1_000_000_000_000;

// From floating-point (implicit, uses continued-fraction approximation)
GaussianRational pi = 3.14159265358979;    // best rational approximation

// From components: a/b + (c/d)i
GaussianRational half = new GaussianRational(1, 2, 0, 1);    // 1/2
GaussianRational z = new GaussianRational(1, 1, 1, 1);       // 1 + i
GaussianRational w = new GaussianRational(3, 4, 5, 6);       // 3/4 + 5/6 i

// Using the imaginary unit
GaussianRational i = GaussianRational.I;                      // i
```

#### Arithmetic

```csharp
GaussianRational a = new GaussianRational(1, 2, 0, 1);  // 1/2
GaussianRational b = new GaussianRational(1, 3, 0, 1);  // 1/3

var sum        = a + b;          // 5/6
var difference = a - b;          // 1/6
var product    = a * b;          // 1/6
var quotient   = a / b;          // 1 & 1/2
var remainder  = a % b;          // 1/6
var negated    = -a;             // -1/2
var powered    = GaussianRational.Pow(a, 3);  // 1/8

// Complex multiplication
GaussianRational i = GaussianRational.I;
Console.WriteLine(i * i);       // -1
```

#### Pramana identity

```csharp
GaussianRational five = 5;

// Every value has a deterministic UUID v5
Guid id = five.PramanaId;
Console.WriteLine(id);           // always the same UUID for the value 5

// And a canonical Pramana URL
Console.WriteLine(five.PramanaUrl);
// https://pramana-data.ca/entity/{uuid}
```

#### Formatting

```csharp
var z = new GaussianRational(7, 2, 3, 4);

Console.WriteLine(z.ToString());                 // "3 & 1/2 + 3/4 i"
Console.WriteLine(z.ToImproperFractionString());  // "7/2 + 3/4 i"
Console.WriteLine(z.ToRawString());               // "<7,2,3,4>"

// IFormattable format strings
Console.WriteLine(z.ToString("G"));  // default: "3 & 1/2 + 3/4 i"
Console.WriteLine(z.ToString("R"));  // raw: "<7,2,3,4>"
Console.WriteLine(z.ToString("I"));  // improper: "7/2 + 3/4 i"
```

#### Inspecting values

```csharp
GaussianRational five = 5;
GaussianRational half = new GaussianRational(1, 2, 0, 1);
GaussianRational z = new GaussianRational(1, 1, 1, 1);

five.IsInteger;          // true
five.IsReal;             // true
five.IsPositive;         // true

half.IsReal;             // true
half.IsInteger;          // false

z.IsGaussianInteger;     // true
z.IsReal;                // false
z.IsPurelyImaginary;     // false

z.Conjugate;             // 1 - i
z.MagnitudeSquared;      // 2 (exact, as a GaussianRational)
z.RealPart;              // 1
z.ImaginaryPart;         // 1
z.Reciprocal;            // 1/2 - 1/2 i
```

#### Parsing

```csharp
// From canonical form "a,b,c,d"
var parsed = GaussianRational.Parse("3,4,0,1");   // 3/4

// Safe parsing
if (GaussianRational.TryParse("1,2,3,4", out var result))
    Console.WriteLine(result);  // "1/2 + 3/4 i"
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

### API reference

#### Constructors

| Signature | Description |
|---|---|
| `GaussianRational(a, b, c, d)` | Full form: a/b + (c/d)i |
| `GaussianRational(real, imaginary)` | Integer real + imaginary parts |
| `GaussianRational(value)` | Real integer (imaginary = 0) |

#### Static constants

| Name | Value |
|---|---|
| `GaussianRational.Zero` | 0 |
| `GaussianRational.One` | 1 |
| `GaussianRational.MinusOne` | -1 |
| `GaussianRational.I` | i (imaginary unit) |

#### Instance properties

| Property | Type | Description |
|---|---|---|
| `A` | `BigInteger` | Real numerator |
| `B` | `BigInteger` | Real denominator (always > 0) |
| `C` | `BigInteger` | Imaginary numerator |
| `D` | `BigInteger` | Imaginary denominator (always > 0) |
| `IsReal` | `bool` | True if imaginary part is zero |
| `IsPurelyImaginary` | `bool` | True if real part is zero, imaginary is not |
| `IsInteger` | `bool` | True if value is a real integer |
| `IsGaussianInteger` | `bool` | True if both parts are integers |
| `IsZero` | `bool` | True if value is 0 |
| `IsOne` | `bool` | True if value is 1 |
| `IsPositive` | `bool` | True if real and positive |
| `IsNegative` | `bool` | True if real and negative |
| `Conjugate` | `GaussianRational` | a/b - (c/d)i |
| `MagnitudeSquared` | `GaussianRational` | \|z\|^2, exact as a rational |
| `RealPart` | `GaussianRational` | Real part as a real GaussianRational |
| `ImaginaryPart` | `GaussianRational` | Imaginary coefficient as a real GaussianRational |
| `Reciprocal` | `GaussianRational` | 1/z |
| `PramanaId` | `Guid` | Deterministic UUID v5 for this value |
| `PramanaUrl` | `string` | Pramana entity URL |

#### Operators

`+`, `-` (binary and unary), `*`, `/`, `%`, `++`, `--`, `==`, `!=`, `<`, `>`, `<=`, `>=`

#### Static methods

| Method | Description |
|---|---|
| `Pow(base, exponent)` | Integer exponentiation (supports negative exponents) |
| `Abs(value)` | Absolute value (real) or magnitude squared (complex) |
| `Sign(value)` | Sign of the real part: -1, 0, or 1 |
| `Floor(value)` | Largest integer <= real part (real only) |
| `Ceiling(value)` | Smallest integer >= real part (real only) |
| `Truncate(value)` | Round toward zero (real only) |
| `Min(a, b)` | Smaller of two values |
| `Max(a, b)` | Larger of two values |
| `Clamp(value, min, max)` | Constrain between bounds |
| `Parse(string)` | Parse from "a,b,c,d" format |
| `TryParse(string, out result)` | Safe parse, returns false on failure |

#### Intentionally unsupported

These throw `NotSupportedException` because their results cannot be represented exactly as rationals:

| Member | Reason |
|---|---|
| `Magnitude` | Square root produces irrationals |
| `Phase` | Arctangent produces irrationals |
| `ToPolar()` | Both magnitude and phase are irrational |
| `FromPolar(mag, phase)` | Sine/cosine produce irrationals |
| `ToDecimalString()` | Decimal truncation loses precision |
| Format `"D"` / `"F"` | Same as `ToDecimalString` |

### Number hierarchy

When resolved through Pramana, each `num:` value is automatically classified:

| Value | Display | Classification |
|---|---|---|
| `num:42` | 42 | Natural Number |
| `num:0` | 0 | Whole Number |
| `num:-5` | -5 | Integer |
| `num:3,4` | 3/4 | Rational Number |
| `num:1,1,1,1` | 1 + i | Gaussian Rational |

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

This library is a companion to the [Pramana](https://pramana.dev) knowledge graph. The Python side defines pseudo-class routing, virtual entity generation, and number classification in `src/combinatoric_classes.py` and `web/app.py`. PramanaLib provides the same canonical normalization and UUID generation in C# so that .NET consumers can work with Pramana values natively.
