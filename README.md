# DotMpi

![Nuget](https://img.shields.io/nuget/v/HigginsSoft.Math.svg?style=flat-square) ![Release Tests](https://github.com/alexhiggins732/HigginsSoft.Math/actions/workflows/release.yml/badge.svg) ![Pre-Release Tests](https://github.com/alexhiggins732/HigginsSoft.Math/actions/workflows/pre-release.yml/badge.svg)

# Installation

HigginsSoft.Math is available as [nuget package](https://www.nuget.org/packages/HigginsSoft.Math/) can be installed using nuget:
- Visual Studio Package Manager
 - `Install-Package HigginsSoft.Math` 
- DotNet CLI
 - `dotnet add package HigginsSoft.Math` 
-  PackageReference
 - `<PackageReference Include="HigginsSoft.Math" />` 

Instructions for other package managers are availabe on the nuget package page.

# Introduction

HigginsSoft.Math is a free DotNet library inspired by GmpLib, Yafu, GGNFS, and other libraries that aims to bring high-performance artihmetic and number theory operations to the DotNet world.

It offers specific implemenations as well as wrappers for some of those libraries so they can be used in DotNet as any other type would be without worrying about the underlying low-level implemtations of those libaries.


## Usage

To use HigginsSoft.Math, simply reference the HigginsSoft.Math namespace in your code and use the available functions.

Wrappers for GmpLib's `mpz_t` (multi-precision integer) is available as `GmpInt` while the `mpz_f` (multi-precision float) is available as GmpFloat.

Both wrappers have the standard artithmetic, binary, compare and cast operators built-in for implicit/explicit conversion with the built-in DotNet numeric data types, including `System.Numerics.BigInteger`

``` csharp
    GmpInt a = 1;
    GmpInt b = a << 80;
    BigInteger check = BigInteger.Power(2, 80);
    Assert.IsTrue(() => b == check);
    GmpInt c = check;
    Assert.IsTrue(() => c == check);
```

##  Overview

### GmpInt and GmpFloat

#### Cast Operators:
- Implicit cast operators are defined for all integer based system types including `byte`, `sbyte`, `int`, `unit`, `long`, `ulong`, `BigInteger`, as well `string` as well as the `GmpInt` and `mpz_t` types.
- Explicit cast operators are defined for all decimal types system types including `float`, `double`, and `decimal`,  as well as the `GmpFloat` and `mpz_f` types
- Constructors exist in both `GmpInt` and `GmpFloat` for the system types.

Note: The explicit operators will truncate the decimal types which could lead to unexpected results, so take care when converting between the types.

### Comparison Operators:
- The standard comparison operators (`<`, `<=`, `==`, `!=`, `>`, `>=`, and `>`) and `CompareTo` are defined for the `GmpInt` and `GmpFloat` types and all corresponding system numeric types.

### Arithmetic Operators:
- The standard binary arithemtic operators (`+`, `-`, `*`, `/`, `%`, `--`,  `++`) as well as unary operators (`+`, `-'`) are defined for the `GmpInt` and `GmpFloat` types.

### Binary Operators:
- The standard binary  operators (`^`, `~`, and `xor`) are defined for the `GmpInt` and `GmpFloat` types.

### Named Methods:
The following named methods are available, many with various overloads:

| - | -  | - |
|--|--|---|
| `Abs` | `GetHashCode` | `ModAsUint32` | 
| `Add` | `HammingDistance` | `Multiply` | 
| `And` | `IndexOfOne` | `Negate` | 
| `Binomial` | `IndexOfZero` | `NextPrimeGMP` | 
| `BitCount` | `InverseModExists` | `Or` | 
| `BitLength` | `InvertMod` | `PopCount` | 
| `Bytes` | `IsDivisibleBy` | `Power` | 
| `Clone` | `IsEven` | `PowerMod` | 
| `CompareToAbs` | `IsOdd` | `RawData` | 
| `CompareToAbs` | `IsOne` | `Remainder` | 
| `Complement` | `IsPerfectPower` | `RemoveFactor` | 
| `CountOnes` | `IsPerfectSquare` | `Root` | 
| `DigitCount` | `IsPowerOfTwo` | `SetBit` | 
| `Divide` | `IsProbablyPrimeRabinMiller` | `ShiftLeft` | 
| `DivideExactly` | `IsZero` | `ShiftRight` | 
| `DivideMod` | `JacobiSymbol` | `Sign` | 
| `Equals` | `KroneckerSymbol` | `Sqrt` | 
| `EqualsMod` | `Lcm` | `Square` | 
| `Factorial` | `LegendreSymbol` | `Subtract` | 
| `Fibonacci` | `Lucas` | `ToString` | 
| `Gcd` | `Mod` | `TryInvertMod` | 
| `GetBit` | `ModAsInt32` | `Xor` | 

### Prime Generator Methods:

A `PrimeGenerator` can be used to generate a list of primes. The default constructor will generate all primes starting from 2. You can also use the constructor overload to specify a starting and/or maximum prime.

The generator is able to produce all primes up to 2^31 in .5 seconds when ran in parallel mode using [HigginsSoft.DotMpi](https://github.com/alexhiggins732/DotMpi) to launch multi-processor sieving threads.

## Roadmap

Detail the following:
1. - [ ]   Introduction
	   - [ ]  Brief overview of the project
2.   - [ ] Motivation
		- [ ]   Discuss why HigginsSoft.Math was created and the problem it solves
3.  - [ ]  Features
	    - [ ]   List of features HigginsSoft.Math provides
4.   - [ ] Usage
		- [ ]   Quickstart guide to using HigginsSoft.Math in your own project
		- [ ]   In-depth explanation of each feature and how to use it
5.   - [ ] Future plans
6.   - [ ] Performance
		- [ ]   Discussion on the performance benefits of HigginsSoft.Math
		- [ ]  Benchmarking data
7.  - [ ]  Contributing
		- [ ]   Guidelines for contributing to HigginsSoft.Math
8.   - [ ] License
		- [ ]   Information about the project's license

## Checklist

 - [ ] Introduction
     - [ ] Title
     - [ ] Brief overview of the project
 - [ ] Motivation
     - [ ] Explanation of why DotMpi was created and the problem it solves
 - [ ] Features
     - [ ]  List of features DotMpi provides
 - [ ]  Usage
    - [ ] Quickstart guide
         - [ ]  Installation
         - [ ]  Setting up a project
         - [ ] Basic usage example
    - [ ] In-depth explanation of each feature and how to use it
 - [ ] Future plans
	 - [ ] Authentication
	 - [ ] Remote connections
		 - [ ] Agent installe
	 - [ ] Custom serializers
	 - [ ] Event Api
     - [ ] Code Cleanup / Refactoring
     - [ ] Unit Tests
     - [ ] Docs
     - [ ] Wiki
     - [ ] Nuget Package
 - [ ] Performance
     - [ ] Discussion on the performance benefits of HigginsSoft.Math
    - [ ] Benchmarking data
 - [ ] Contributing
     - [ ] Guidelines for contributing to HigginsSoft.Math
 - [ ] License
     - [ ]   Information about the project's license



## Contributing

If you are interested in contributing to HigginsSoft.Math, please submit a pull request with your changes and a description of the changes made.

## License

HigginsSoft.Nath is licensed under the [GNU General Public License v3.0](COPYING). See the [LICENSE](LICENSE) file for details about using and redistributing this software.

## Acknowledgments

Special thanks to the creators of [GmpInt](https://www.open-mpi.org/doc/v1.8/man1/mpirun.1.php) and [bbuhrow's Yafu](https://github.com/bbuhrow/yafu) for inspiring and guiding the development of DotMpi.