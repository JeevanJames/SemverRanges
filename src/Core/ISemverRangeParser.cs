namespace Jeevan.SemverRanges
{
    public interface ISemverRangeParser
    {
        bool TryParse(string version, out SemverRange range);
    }
}
