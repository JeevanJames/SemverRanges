using FluentAssertions;

using Xunit;

namespace Jeevan.SemverRanges.UnitTests
{
    public sealed class NuGetVersionRangeTests
    {
        // From https://docs.microsoft.com/en-us/nuget/concepts/package-versioning
        [Theory]
        [InlineData("1.0", "1.0.0 <= v")]
        [InlineData("(1.0,)", "1.0.0 < v")]
        [InlineData("[1.0]", "1.0.0 <= v <= 1.0.0")]
        [InlineData("(,1.0]", "v <= 1.0.0")]
        [InlineData("(,1.0)", "v < 1.0.0")]
        [InlineData("[1.0,2.0]", "1.0.0 <= v <= 2.0.0")]
        [InlineData("(1.0,2.0)", "1.0.0 < v < 2.0.0")]
        [InlineData("[1.0,2.0)", "1.0.0 <= v < 2.0.0")]
        public void TryParse_should_parse_valid_range_strings(string rangeStr, string expectedRangeStr)
        {
            bool success = SemverRange.NuGet.TryParse(rangeStr, out SemverRange range);

            success.Should().BeTrue();
            range.Should().NotBeNull();
            range.ToString().Should().Be(expectedRangeStr);
        }

        [Theory]
        [InlineData("(1.0.0)", "Exact version cannot be exclusive")]
        [InlineData("[1.0.0)", "Exact version cannot be exclusive")]
        [InlineData("(1.0.0]", "Exact version cannot be exclusive")]
        [InlineData("[1.0,2.0,3.0]", "Can only have 1-2 versions specified")]
        [InlineData("[1.0.0.0,2.0.0.0]", "Cannot use 4-point versions from System.Version")]
        [InlineData("InvalidFormat", "Any invalid format")]
        [InlineData(null, "A null value")]
        [InlineData("", "Empty string")]
        [InlineData("      ", "White-spaced string")]
        public void TryParse_should_throw_for_invalid_range_strings(string rangeStr, string reason)
        {
            bool parsed = SemverRange.NuGet.TryParse(rangeStr, out _);

            parsed.Should().BeFalse(because: reason);
        }
    }
}
