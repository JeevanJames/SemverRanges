using System;

namespace Jeevan.SemverRanges
{
    internal sealed class NpmVersionRangeParser : ISemverRangeParser
    {
        bool ISemverRangeParser.TryParse(string version, out SemverRange range)
        {
            throw new NotImplementedException();
        }
    }
}
