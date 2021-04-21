# SemverRanges
[![NuGet Version](http://img.shields.io/nuget/v/Jeevan.SemverRanges.svg?style=flat)](https://www.nuget.org/packages/Jeevan.SemverRanges/) [![NuGet Downloads](https://img.shields.io/nuget/dt/Jeevan.SemverRanges.svg)](https://www.nuget.org/packages/Jeevan.SemverRanges/) [![GitHub license](https://img.shields.io/github/license/JeevanJames/SemverRanges)](https://github.com/JeevanJames/SemverRanges/blob/main/LICENSE)

SemverRanges is a .NET class library for parsing [NuGet](https://docs.microsoft.com/en-us/nuget/concepts/package-versioning) and [NPM](https://docs.npmjs.com/cli/v7/using-npm/semver#ranges) version ranges.

## Installation
SemverRanges is a `netstandard2.0` package available on https://www.nuget.org as [`Jeevan.SemverRanges`](https://www.nuget.org/packages/Jeevan.SemverRanges/).

```sh
dotnet add package Jeevan.SemverRanges
```

## Usage
Parsing version range strings:
```cs
using Jeevan.SemverRanges;

// Parse a NuGet version range - 1.0.0 <= version < 3.0.0
if (!SemverRange.NuGet.TryParse("[1.0.0,3.0.0)", out SemverRange nugetRange))
    Console.WriteLine("Could not parse NuGet version range");

// Parse a NPM version range - 1.2.3 <= version < 2.0.0
if (!SemverRange.Npm.TryParse("^1.2.3", out SemverRange npmRange))
    Console.WriteLine("Could not parse NPM version range");

// Check if a version falls within a range
bool inRange = nugetRange.VersionInRange("2.1.2"); // Method on SemversionRange
bool notInRange = SemVersion.Parse("3.0").InRange(npmRange); // Extension method on SemVersion
```

Creating `SemverRange` instances directly:
```cs
using Jeevan.SemverRanges;

// Create an exact version range - 2.0.0
SemverRange exactVer = SemverRange.Exact("2.0.0");

// Create version range 1.0.0-2.0.0 both inclusive
// 1.0.0 <= version <= 2.0.0
SemverRange inclusiveVers = SemverRange.Inclusive("1.0.0", "2.0.0");

// Create version range 1.0.0 (inclusive) - 2.0.0 (exclusive)
// 1.0.0 <= version < 2.0.0
SemverRange range = SemverRange.InclusiveMinExclusiveMax("1.0.0", "2.0.0");

// For any other use case, use the constructor
var range = new SemverRange(
    minimum: "1.0.0",
    maximum: "2.0.0",
    minimumInclusive: true,
    maximumInclusive: false);
```

## Status
SemverRanges is currently under development. The following features are pending:
* NPM version range parsing (#1) (`~1.2.3`, `^1.2.3`, etc)
* Parsing generic version range expressions (`>= 2.0.0`, `< 5.0.0`, `>= 2.0.0 & < 5.0.0`, etc)
