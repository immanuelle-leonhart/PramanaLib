using PramanaLib;
using System.Numerics;


int i = 0;

while (i <= 100)
{
    Console.WriteLine(i);
    GaussianRational g = i;
    Console.WriteLine(g);
    i++;
}
double j = 0.5;

while (j <= 100)
{
    Console.WriteLine(j);
    GaussianRational g = j;
    Console.WriteLine(g);
    j++;
}

GaussianRational g2 = 5;
g2 += 5;
Console.WriteLine(g2++);
Console.WriteLine(g2--);
Console.WriteLine(g2);


//Console.WriteLine("=== GaussianRational Testing ===\n");

//// Test integer casting
//Console.WriteLine("--- Integer Casting ---");
//GaussianRational one = 1;
//GaussianRational five = 5;
//GaussianRational negThree = -3;

//Console.WriteLine($"1 -> {one.ToRawString()} -> \"{one}\"");
//Console.WriteLine($"5 -> {five.ToRawString()} -> \"{five}\"");
//Console.WriteLine($"-3 -> {negThree.ToRawString()} -> \"{negThree}\"");
//Console.WriteLine();

//// Test double/float casting
//Console.WriteLine("--- Double/Float Casting ---");
//GaussianRational half = 0.5;
//GaussianRational quarter = 0.25;
//GaussianRational third = 1.0 / 3.0;

//Console.WriteLine($"0.5 -> {half.ToRawString()} -> \"{half}\"");
//Console.WriteLine($"0.25 -> {quarter.ToRawString()} -> \"{quarter}\"");
//Console.WriteLine($"1/3 -> {third.ToRawString()} -> \"{third}\"");
//Console.WriteLine();

//// Test complex numbers
//Console.WriteLine("--- Complex Numbers ---");
//var i = GaussianRational.I;
//var onePlusI = new GaussianRational(1, 1, 1, 1);
//var twoPlusTwoI = new GaussianRational(2, 1, 2, 1);
//var halfPlusHalfI = new GaussianRational(1, 2, 1, 2);

//Console.WriteLine($"i -> {i.ToRawString()} -> \"{i}\"");
//Console.WriteLine($"1+i -> {onePlusI.ToRawString()} -> \"{onePlusI}\"");
//Console.WriteLine($"2+2i -> {twoPlusTwoI.ToRawString()} -> \"{twoPlusTwoI}\"");
//Console.WriteLine($"1/2+1/2i -> {halfPlusHalfI.ToRawString()} -> \"{halfPlusHalfI}\"");
//Console.WriteLine();

//// Test decimal form
//Console.WriteLine("--- Decimal Form ---");
//Console.WriteLine($"1/2 decimal: {half.ToDecimalString()}");
//Console.WriteLine($"1/3 decimal: {third.ToDecimalString()}");
//Console.WriteLine($"1/2+1/2i decimal: {halfPlusHalfI.ToDecimalString()}");
//Console.WriteLine();

//// Test PramanaId and URL
//Console.WriteLine("--- PramanaId & URL ---");
//Console.WriteLine($"1 PramanaId: {one.PramanaId}");
//Console.WriteLine($"1 URL: {one.PramanaUrl}");
//Console.WriteLine($"i PramanaId: {i.PramanaId}");
//Console.WriteLine($"i URL: {i.PramanaUrl}");
//Console.WriteLine($"1+i PramanaId: {onePlusI.PramanaId}");
//Console.WriteLine();

//// Test parsing
//Console.WriteLine("--- Parsing ---");
//var parsed = GaussianRational.Parse("1,1,0,1");
//Console.WriteLine($"Parse(\"1,1,0,1\") -> {parsed.ToRawString()} -> \"{parsed}\"");
//var parsed2 = GaussianRational.Parse("1,2,1,2");
//Console.WriteLine($"Parse(\"1,2,1,2\") -> {parsed2.ToRawString()} -> \"{parsed2}\"");
//Console.WriteLine();

//// Test arithmetic
//Console.WriteLine("--- Arithmetic ---");
//var a = new GaussianRational(3, 1, 2, 1); // 3 + 2i
//var b = new GaussianRational(1, 1, 4, 1); // 1 + 4i

//Console.WriteLine($"a = {a}");
//Console.WriteLine($"b = {b}");
//Console.WriteLine($"a + b = {a + b}");
//Console.WriteLine($"a - b = {a - b}");
//Console.WriteLine($"a * b = {a * b}");
//Console.WriteLine($"a / b = {a / b}");
//Console.WriteLine($"-a = {-a}");
//Console.WriteLine($"a.Conjugate = {a.Conjugate}");
//Console.WriteLine();

//// Test magnitude and polar
//Console.WriteLine("--- Magnitude & Polar ---");
//Console.WriteLine($"|{a}| = {a.Magnitude}");
//Console.WriteLine($"|{a}|² = {a.MagnitudeSquared}");
//var (mag, phase) = a.ToPolar();
//Console.WriteLine($"Polar: ({mag}, {phase} rad)");
//Console.WriteLine();

//// Test explicit casting back to primitives
//Console.WriteLine("--- Explicit Casting to Primitives ---");
//GaussianRational intVal = 42;
//GaussianRational ratVal = new GaussianRational(7, 2, 0, 1); // 7/2

//Console.WriteLine($"{intVal} as int: {(int)intVal}");
//Console.WriteLine($"{intVal} as long: {(long)intVal}");
//Console.WriteLine($"{ratVal} as double: {(double)ratVal}");
//Console.WriteLine($"{ratVal} as float: {(float)ratVal}");

//// Casting to arrays
//double[] arr = (double[])onePlusI;
//Console.WriteLine($"{onePlusI} as double[]: [{arr[0]}, {arr[1]}]");

//int[] intArr = (int[])twoPlusTwoI;
//Console.WriteLine($"{twoPlusTwoI} as int[]: [{intArr[0]}, {intArr[1]}]");
//Console.WriteLine();

//// Test BigInteger support
//Console.WriteLine("--- BigInteger Support ---");
//BigInteger big = BigInteger.Pow(10, 50);
//GaussianRational bigRat = big;
//Console.WriteLine($"10^50 -> {bigRat.ToRawString()}");
//Console.WriteLine($"IsInteger: {bigRat.IsInteger}");
//Console.WriteLine();

//// Test properties
//Console.WriteLine("--- Properties ---");
//Console.WriteLine($"1 IsReal: {one.IsReal}");
//Console.WriteLine($"i IsReal: {i.IsReal}");
//Console.WriteLine($"i IsPurelyImaginary: {i.IsPurelyImaginary}");
//Console.WriteLine($"1 IsInteger: {one.IsInteger}");
//Console.WriteLine($"1/2 IsInteger: {half.IsInteger}");
//Console.WriteLine($"1+i IsGaussianInteger: {onePlusI.IsGaussianInteger}");
//Console.WriteLine($"1/2+1/2i IsGaussianInteger: {halfPlusHalfI.IsGaussianInteger}");
//Console.WriteLine();

//// Test equality
//Console.WriteLine("--- Equality ---");
//GaussianRational a1 = 2;
//GaussianRational a2 = new GaussianRational(4, 2, 0, 1); // 4/2 = 2
//Console.WriteLine($"{a1.ToRawString()} == {a2.ToRawString()}: {a1 == a2}");
//Console.WriteLine();

//// Test exception handling
//Console.WriteLine("--- Exception Handling ---");
//try
//{
//    int badCast = (int)onePlusI; // Should throw - has imaginary part
//}
//catch (InvalidCastException ex)
//{
//    Console.WriteLine($"Casting 1+i to int: {ex.Message}");
//}

//try
//{
//    int badCast = (int)half; // Should throw - not an integer
//}
//catch (InvalidCastException ex)
//{
//    Console.WriteLine($"Casting 1/2 to int: {ex.Message}");
//}

//Console.WriteLine("\n=== Testing Complete ===");
