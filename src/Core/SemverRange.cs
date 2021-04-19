using System;
using System.Text;

using Semver;

namespace Jeevan.SemverRanges
{
    public readonly struct SemverRange
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SemverRange"/> struct, with an exact version.
        /// </summary>
        /// <param name="version">The exact version to assign to the range.</param>
        public SemverRange(SemVersion version)
            : this(version, version, true, true)
        {
        }

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

        public static readonly ISemverRangeParser NuGet = new NuGetVersionRangeParser();

        public static readonly ISemverRangeParser Npm = new NpmVersionRangeParser();
    }
}
