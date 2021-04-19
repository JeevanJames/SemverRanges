using System;
using System.Text.RegularExpressions;

using Semver;

namespace Jeevan.SemverRanges
{
    internal sealed class NuGetVersionRangeParser : ISemverRangeParser
    {
        bool ISemverRangeParser.TryParse(string version, out SemverRange range)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                range = default;
                return false;
            }

            version = version.Trim();

            // Version could be just a semver
            if (SemVersion.TryParse(version, out SemVersion semver))
            {
                range = new SemverRange(semver, null, true);
                return true;
            }

            (bool parsed, SemverRange semverRange) = ParseAsNuGetRange(version);
            if (parsed)
            {
                range = semverRange;
                return true;
            }

            range = default;
            return false;
        }

        private static (bool Parsed, SemverRange Range) ParseAsNuGetRange(string version)
        {
            Match match = BracketsPattern.Match(version);
            if (!match.Success)
                return (false, default);

            bool minimumInclusive = match.Groups[1].Value == "[";
            bool maximumInclusive = match.Groups[3].Value == "]";
            string versionStr = match.Groups[2].Value;

            // Split inner version by comma and trim surrounding spaces
            string[] versionParts = Array.ConvertAll(versionStr.Split(new[] { ',' }, StringSplitOptions.None),
                ver => ver.Trim());

            // There should not be more than 2 versions
            if (versionParts.Length > 2)
                return (false, default);

            string? version1 = string.IsNullOrWhiteSpace(versionParts[0]) ? null : versionParts[0].Trim();
            string? version2;
            if (versionParts.Length > 1)
                version2 = string.IsNullOrWhiteSpace(versionParts[1]) ? null : versionParts[1].Trim();
            else
                version2 = null;

            // Both versions cannot be empty
            if (version1 is null && version2 is null)
                return (false, default);

            SemVersion? minimum = null;
            if (version1 is not null && !SemVersion.TryParse(versionParts[0].Trim(), out minimum, strict: false))
                return (false, default);

            // If only one version is specified (no comma), then we want an exact match. For this,
            // only [] brackets are allowed. Parentheses are not allowed, as we can't have exclusive
            // versions for an exact match.
            if (versionParts.Length == 1)
            {
                return minimumInclusive && maximumInclusive
                    ? (true, new SemverRange(minimum, minimum, true, true)) // Exact match
                    : (false, default);
            }

            // If only one version specified, we're done
            if (minimum is not null && versionParts.Length == 1)
                return (true, new SemverRange(minimum));

            SemVersion? maximum = null;
            if (version2 is not null && !SemVersion.TryParse(version2, out maximum, strict: false))
                return (false, default);

            return (true, new SemverRange(minimum, maximum, minimumInclusive, maximumInclusive));
        }

        // Matches against a string optionally prefixed and suffixed with a square bracket or parentheses.
        private static readonly Regex BracketsPattern = new(@"^([\[\(])([^\[\]\(\)]+)([\]\)])$");
    }
}
