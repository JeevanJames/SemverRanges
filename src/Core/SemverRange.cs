using System;
using System.Text;

using Semver;

namespace Jeevan.SemverRanges
{
    /// <summary>
    ///     Represents a semantic version (semver) range.
    ///     <para/>
    ///     The <see cref="Minimum"/> or <see cref="Maximum"/> version can be <c>null</c>, indicating
    ///     no lower or upper version bounds respectively. Versions in the range can be inclusive or
    ///     exclusive.
    /// </summary>
    public readonly struct SemverRange
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SemverRange"/> struct, with the specified
        ///     version range details.
        /// </summary>
        /// <param name="minimum">
        ///     The minimum version in the range. Can be <c>null</c> to avoid a minimum version.
        /// </param>
        /// <param name="maximum">
        ///     The maximum version in the range. Can be <c>null</c> to avoid a maximum version.
        /// </param>
        /// <param name="minimumInclusive">
        ///     Indicates whether the specified <paramref name="minimum"/> version is inclusive. Defaults
        ///     to <c>true</c>.
        /// </param>
        /// <param name="maximumInclusive">
        ///     Indicates whether the specified <paramref name="maximum"/> version is inclusive. Defaults
        ///     to <c>false</c>.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     Thrown if both <paramref name="minimum"/> and <paramref name="maximum"/> versions are
        ///     <c>null</c>, or if the minimum version is greater than the maximum version.
        /// </exception>
        public SemverRange(SemVersion? minimum, SemVersion? maximum, bool minimumInclusive = true,
            bool maximumInclusive = false)
        {
            if (minimum is null && maximum is null)
                throw new ArgumentException("Minimum and maximum versions cannot both be null.");
            if (minimum is not null && maximum is not null && minimum > maximum)
                throw new ArgumentException("Minimum version cannot be greater than maximum version.", nameof(minimum));

            Minimum = minimum;
            MinimumInclusive = minimumInclusive;
            Maximum = maximum;
            MaximumInclusive = maximumInclusive;
        }

        /// <summary>
        ///     Creates a <see cref="SemverRange"/> instance with an exact <paramref name="version"/>.
        /// </summary>
        /// <param name="version">The exact version to set.</param>
        /// <returns>A <see cref="SemverRange"/> instance.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="version"/> is <c>null</c>.
        /// </exception>
        public static SemverRange Exact(SemVersion version)
        {
            if (version is null)
                throw new ArgumentNullException(nameof(version));
            return new(version, version, true, true);
        }

        /// <summary>
        ///     Creates a <see cref="SemverRange"/> instance with the specified inclusive <paramref name="minimum"/>
        ///     and <paramref name="maximum"/> versions.
        /// </summary>
        /// <param name="minimum">The inclusive minimum version.</param>
        /// <param name="maximum">The inclusive maximum version.</param>
        /// <returns>A <see cref="SemverRange"/> instance.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="minimum"/> or <paramref name="maximum"/> is <c>null</c>.
        /// </exception>
        public static SemverRange AllInclusive(SemVersion minimum, SemVersion maximum)
        {
            if (minimum is null)
                throw new ArgumentNullException(nameof(minimum));
            if (maximum is null)
                throw new ArgumentNullException(nameof(maximum));
            return new(minimum, maximum, true, true);
        }

        /// <summary>
        ///     Creates a <see cref="SemverRange"/> instance for a common scenario - inclusive
        ///     <paramref name="minimum"/> version and exclusive <paramref name="maximum"/> version.
        /// </summary>
        /// <param name="minimum">The inclusive minimum version.</param>
        /// <param name="maximum">The exclusive maximum version.</param>
        /// <returns>A <see cref="SemverRange"/> instance.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="minimum"/> or <paramref name="maximum"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="minimum"/> and <paramref name="maximum"/> are equal, in which
        ///     case both need to be inclusive.
        /// </exception>
        public static SemverRange InclusiveMinAndExclusiveMax(SemVersion minimum, SemVersion maximum)
        {
            if (minimum is null)
                throw new ArgumentNullException(nameof(minimum));
            if (maximum is null)
                throw new ArgumentNullException(nameof(maximum));
            if (minimum == maximum)
                throw new ArgumentException("For the same minimum and maximum versions, both must be inclusive.");
            return new(minimum, maximum, true, false);
        }

        /// <summary>
        ///     Gets the minimum version of the range. If <c>null</c>, there is no minimum version.
        /// </summary>
        public SemVersion? Minimum { get; }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Minimum"/> version number is inclusive.
        /// </summary>
        public bool MinimumInclusive { get; }

        /// <summary>
        ///     Gets the maximum version of the range. If <c>null</c>, there is no maximum version.
        /// </summary>
        public SemVersion? Maximum { get; }

        /// <summary>
        ///     Gets a value indicating whether the <see cref="Maximum"/> version number is inclusive.
        /// </summary>
        public bool MaximumInclusive { get; }

        /// <summary>
        ///     Indicates whether this instance of <see cref="SemverRange"/> is undefined. This can
        ///     happen if the default constructor is used to create the instance.
        /// </summary>
        public bool IsUndefined()
        {
            return Minimum is null && Maximum is null;
        }

        /// <summary>
        ///     Checks whether the specified <paramref name="semver"/> is within the current version
        ///     range.
        /// </summary>
        /// <param name="semver">The semantic version to check.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="semver"/> is within the current version range; otherwise
        ///     <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="semver"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     Thrown if the current <see cref="SemverRange"/> instance is undefined.
        /// </exception>
        public bool VersionInRange(SemVersion semver)
        {
            if (semver is null)
                throw new ArgumentNullException(nameof(semver));
            if (IsUndefined())
            {
                throw new InvalidOperationException(
                    $"This {nameof(SemverRange)} instance is undefined. Use one of the provided constructors to create a valid instance.");
            }

            bool satisfiesMinimum = Minimum is null || (MinimumInclusive && semver >= Minimum) || semver > Minimum;
            bool satisfiesMaximum = Maximum is null || (MaximumInclusive && semver <= Maximum) || semver < Maximum;
            return satisfiesMinimum && satisfiesMaximum;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (IsUndefined())
                return "Undefined Range";

            var sb = new StringBuilder();
            if (Minimum is not null)
                sb.Append(Minimum).Append(' ').Append(MinimumInclusive ? "<=" : '<').Append(' ');
            sb.Append("v");
            if (Maximum is not null)
                sb.Append(' ').Append(MaximumInclusive ? "<=" : '<').Append(' ').Append(Maximum);
            return sb.ToString();
        }

        /// <summary>
        ///     Returns a <see cref="ISemverRangeParser"/> parser instance for NuGet format version
        ///     ranges.
        ///     <para/>
        ///     Refer https://docs.microsoft.com/en-us/nuget/concepts/package-versioning.
        /// </summary>
        public static readonly ISemverRangeParser NuGet = new NuGetVersionRangeParser();

        /// <summary>
        ///     Returns a <see cref="ISemverRangeParser"/> parser instance for NPM format version
        ///     ranges.
        /// </summary>
        public static readonly ISemverRangeParser Npm = new NpmVersionRangeParser();
    }
}
