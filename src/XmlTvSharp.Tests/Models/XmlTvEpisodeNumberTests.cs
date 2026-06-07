namespace XmlTvSharp.Tests.Models;

public sealed class XmlTvEpisodeNumberTests
{
    [Fact]
    public void OmittedSystem_PreservesOmissionAndAppliesDefault()
    {
        var episode = new XmlTvEpisodeNumber("S01E01");

        Assert.Null(episode.System);
        Assert.Equal("onscreen", episode.EffectiveSystem);
    }
}
