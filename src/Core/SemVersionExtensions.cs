using System;

using Semver;

namespace Jeevan.SemverRanges
{
    public static class SemVersionExtensions
    {
        public static bool InRange(this SemVersion semver, SemverRange range)
        {
            if (semver is null)
                throw new ArgumentNullException(nameof(semver));
            if (range.IsUndefined())
                throw new ArgumentNullException(nameof(range));

            return range.VersionInRange(semver);
        }
    }
}
