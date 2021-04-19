using System;

using Jeevan.SemverRanges;

if (SemverRange.NuGet.TryParse("1.0", out SemverRange range))
    Console.WriteLine(range);
