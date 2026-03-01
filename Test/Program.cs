using PramanaLib;
using System.Numerics;

Console.WriteLine("=== Gint (Gaussian Integer) Testing ===\n");

// Basic construction
Console.WriteLine("--- Construction ---");
var alpha = new Gint(11, 3);
var beta = new Gint(1, 8);
Console.WriteLine($"alpha = {alpha}");
Console.WriteLine($"beta = {beta}");
Console.WriteLine($"Gint() = {new Gint()}");
Console.WriteLine($"Gint(5) = {new Gint(5)}");
Console.WriteLine($"Gint.Eye() = {Gint.Eye()}");
Console.WriteLine($"Gint.Two() = {Gint.Two()}");
Console.WriteLine();

// Properties
Console.WriteLine("--- Properties ---");
Console.WriteLine($"alpha.Real = {alpha.Real}");
Console.WriteLine($"alpha.Imag = {alpha.Imag}");
Console.WriteLine($"alpha.Conjugate = {alpha.Conjugate}");
Console.WriteLine($"alpha.Norm = {alpha.Norm}");
Console.WriteLine($"alpha.IsUnit = {alpha.IsUnit}");
Console.WriteLine($"Gint.Eye().IsUnit = {Gint.Eye().IsUnit}");
Console.WriteLine();

// Arithmetic
Console.WriteLine("--- Arithmetic ---");
Console.WriteLine($"alpha + beta = {alpha + beta}");
Console.WriteLine($"alpha - beta = {alpha - beta}");
Console.WriteLine($"alpha * beta = {alpha * beta}");
Console.WriteLine($"alpha / beta = {alpha / beta}");
Console.WriteLine($"alpha % beta = {alpha % beta}");
Console.WriteLine($"-alpha = {-alpha}");
Console.WriteLine($"Gint.Pow(alpha, 3) = {Gint.Pow(alpha, 3)}");
Console.WriteLine();

// Units and Associates
Console.WriteLine("--- Units & Associates ---");
Console.Write("Units: ");
foreach (var u in Gint.Units()) Console.Write($"{u}  ");
Console.WriteLine();
Console.Write("Associates of alpha: ");
foreach (var a in alpha.Associates()) Console.Write($"{a}  ");
Console.WriteLine();
Console.WriteLine($"alpha.IsAssociate(-alpha) = {alpha.IsAssociate(-alpha)}");
Console.WriteLine($"alpha.IsAssociate(beta) = {alpha.IsAssociate(beta)}");
Console.WriteLine();

// GCD & Extended GCD
Console.WriteLine("--- GCD & Extended GCD ---");
var gcd = Gint.Gcd(alpha, beta);
Console.WriteLine($"Gint.Gcd({alpha}, {beta}) = {gcd}");

var (g, x, y) = Gint.Xgcd(alpha, beta);
Console.WriteLine($"Gint.Xgcd({alpha}, {beta}) = (gcd={g}, x={x}, y={y})");
Console.WriteLine($"Verify: {alpha}*{x} + {beta}*{y} = {alpha * x + beta * y}");
Console.WriteLine();

// Modified Divmod
Console.WriteLine("--- Modified Divmod ---");
var (q, r) = Gint.ModifiedDivmod(alpha, beta);
Console.WriteLine($"ModifiedDivmod({alpha}, {beta}) = (q={q}, r={r})");
Console.WriteLine($"Verify: {beta}*{q} + {r} = {beta * q + r}");
Console.WriteLine();

// Primality
Console.WriteLine("--- Gaussian Primality ---");
Console.WriteLine($"IsGaussianPrime(3) = {Gint.IsGaussianPrime(new Gint(3))}");
Console.WriteLine($"IsGaussianPrime(5) = {Gint.IsGaussianPrime(new Gint(5))}");
Console.WriteLine($"IsGaussianPrime(7) = {Gint.IsGaussianPrime(new Gint(7))}");
Console.WriteLine($"IsGaussianPrime(2+i) = {Gint.IsGaussianPrime(new Gint(2, 1))}");
Console.WriteLine($"IsGaussianPrime(1+i) = {Gint.IsGaussianPrime(new Gint(1, 1))}");
Console.WriteLine();

// Number theory
Console.WriteLine("--- Number Theory ---");
Console.WriteLine($"NumberTheory.IsPrime(17) = {NumberTheory.IsPrime(17)}");
Console.WriteLine($"NumberTheory.IsPrime(15) = {NumberTheory.IsPrime(15)}");
Console.WriteLine($"IsRelativelyPrime({alpha}, {beta}) = {Gint.IsRelativelyPrime(alpha, beta)}");
var (isCong, cResult) = Gint.CongruentModulo(new Gint(7, 2), new Gint(3, 1), new Gint(2, 0));
Console.WriteLine($"CongruentModulo(7+2i, 3+i, 2) = ({isCong}, {cResult})");
var nd = Gint.NormsDivide(alpha, beta);
Console.WriteLine($"NormsDivide({alpha}, {beta}) = {(nd.HasValue ? nd.Value.ToString() : "false")}");
Console.WriteLine();

// Conversion to Gauss
Console.WriteLine("--- Conversion ---");
Gauss alphaGauss = alpha;
Console.WriteLine($"alpha as Gauss = {alphaGauss}");
Console.WriteLine($"alpha as Gauss raw = {alphaGauss.ToRawString()}");
Console.WriteLine();

// FloorDiv
Console.WriteLine("--- Floor Division ---");
Console.WriteLine($"FloorDiv({alpha}, {beta}) = {Gint.FloorDiv(alpha, beta)}");
Console.WriteLine();

Console.WriteLine("=== Gauss (Gaussian Rational) New Features ===\n");

// Eye, Units, Associates
Console.WriteLine("--- Eye, Units, Associates ---");
Console.WriteLine($"Gauss.Eye() = {Gauss.Eye()}");
Console.Write("Gauss.GaussUnits(): ");
foreach (var u in Gauss.GaussUnits()) Console.Write($"{u}  ");
Console.WriteLine();

var gr = new Gauss(3, 1, 2, 1); // 3 + 2i
Console.Write($"Associates of {gr}: ");
foreach (var a in gr.Associates()) Console.Write($"{a}  ");
Console.WriteLine();
Console.WriteLine($"gr.IsAssociate(-gr) = {gr.IsAssociate(-gr)}");
Console.WriteLine();

// Norm and Inverse
Console.WriteLine("--- Norm & Inverse ---");
Console.WriteLine($"({gr}).Norm = {gr.Norm}");
Console.WriteLine($"({gr}).Inverse = {gr.Inverse}");
Console.WriteLine($"({gr}) * ({gr}).Inverse = {gr * gr.Inverse}");
Console.WriteLine();

// Existing Gauss tests
Console.WriteLine("--- Existing Gauss Arithmetic ---");
Gauss i = Gauss.I;
Console.WriteLine($"i * i = {i * i}");
Console.WriteLine($"i = {i}");

var a1 = new Gauss(3, 1, 2, 1);
var b1 = new Gauss(1, 1, 4, 1);
Console.WriteLine($"a = {a1}");
Console.WriteLine($"b = {b1}");
Console.WriteLine($"a + b = {a1 + b1}");
Console.WriteLine($"a - b = {a1 - b1}");
Console.WriteLine($"a * b = {a1 * b1}");
Console.WriteLine($"a / b = {a1 / b1}");
Console.WriteLine();

Console.WriteLine("=== Pramana Serialization ===\n");

var g1 = new Gauss(3, 1, 2, 1); // 3 + 2i
var gi1 = new Gint(3, 2);       // 3 + 2i

Console.WriteLine($"Gauss(3+2i).PramanaString = {g1.PramanaString}");
Console.WriteLine($"Gint(3+2i).PramanaString  = {gi1.PramanaString}");
Console.WriteLine($"Gauss(3+2i).PramanaId     = {g1.PramanaId}");
Console.WriteLine($"Gint(3+2i).PramanaId      = {gi1.PramanaId}");
Console.WriteLine($"IDs match: {g1.PramanaId == gi1.PramanaId}");
Console.WriteLine($"Gauss(3+2i).PramanaUrl     = {g1.PramanaUrl}");
Console.WriteLine($"Gauss(3+2i).PramanaHashUrl = {g1.PramanaHashUrl}");
Console.WriteLine($"Gint(3+2i).PramanaUrl      = {gi1.PramanaUrl}");
Console.WriteLine($"Gint(3+2i).PramanaHashUrl  = {gi1.PramanaHashUrl}");
Console.WriteLine();

Gauss one = Gauss.One;
Console.WriteLine($"Gauss.One.PramanaString   = {one.PramanaString}");
Console.WriteLine($"Gauss.One.PramanaId       = {one.PramanaId}");
Console.WriteLine($"Gauss(1/2).PramanaString  = {new Gauss(1, 2, 0, 1).PramanaString}");
Console.WriteLine($"Gauss(1/2).ToDecimalString() = {new Gauss(1, 2, 0, 1).ToDecimalString(4)}");
Console.WriteLine();

Console.WriteLine("=== All Tests Complete ===");
