using System;
using System.Collections.Generic;

using FluentAssertions;

using Semver;

using Xunit;

namespace Jeevan.SemverRanges.UnitTests
{
    public sealed class SemverRangeTests
    {
        [Theory]
        [MemberData(nameof(GetValidSemverRanges))]
        public void ToString_should_output_correct_version_string(string? minimum, bool minimumInclusive,
            string? maximum, bool maximumInclusive, string expectedString)
        {
            var range = new SemverRange(
                minimum is null ? null : SemVersion.Parse(minimum),
                maximum is null ? null : SemVersion.Parse(maximum),
                minimumInclusive, maximumInclusive);

            string outputStr = range.ToString();

            outputStr.Should().Be(expectedString);
        }

        private static IEnumerable<object?[]> GetValidSemverRanges()
        {
            yield return new object?[] { "1.0.0", true, "3.0.0", false, "1.0.0 <= v < 3.0.0" };
            yield return new object?[] { null, true, "3.0.0", false, "v < 3.0.0" };
        }

        [Fact]
        public void Ctor_should_throw_if_minimum_greater_than_maximum()
        {
            Action action = () => new SemverRange("2.0.0", "1.0.0");

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Ctor_should_throw_if_both_versions_are_null()
        {
            Action action = () => new SemverRange(null, null);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Ctor_should_support_single_version()
        {
            var range = new SemverRange("1.0.0");

            range.Minimum.Should().Be("1.0.0");
            range.Maximum.Should().Be("1.0.0");
            range.MinimumInclusive.Should().BeTrue();
            range.MaximumInclusive.Should().BeTrue();
        }

        [Theory]
        [InlineData("1.0.0", true, "3.0.0", false, "1.0.0")]
        [InlineData("1.0.0", true, "3.0.0", false, "2.9.9")]
        [InlineData("1.0.0", true, "3.0.0", true, "3.0.0")]
        public void VersionInRange_should_succeed(string? minimum, bool minimumInclusive, string? maximum, bool maximumInclusive,
            string versionToCheck)
        {
            var range = new SemverRange(minimum, maximum, minimumInclusive, maximumInclusive);

            bool versionInRange = range.VersionInRange(versionToCheck);

            versionInRange.Should().BeTrue();
        }
    }
}
