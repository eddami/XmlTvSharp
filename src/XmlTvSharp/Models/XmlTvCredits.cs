using System.Collections.ObjectModel;

namespace XmlTvSharp.Models;

/// <summary>
///     Represents the role-grouped children of an XMLTV <c>credits</c> element.
/// </summary>
public sealed class XmlTvCredits
{
    /// <summary>Gets director credits in XML order.</summary>
    public Collection<XmlTvCredit> Directors { get; } = [];

    /// <summary>Gets actor credits in XML order.</summary>
    public Collection<XmlTvActorCredit> Actors { get; } = [];

    /// <summary>Gets writer credits in XML order.</summary>
    public Collection<XmlTvCredit> Writers { get; } = [];

    /// <summary>Gets adapter credits in XML order.</summary>
    public Collection<XmlTvCredit> Adapters { get; } = [];

    /// <summary>Gets producer credits in XML order.</summary>
    public Collection<XmlTvCredit> Producers { get; } = [];

    /// <summary>Gets composer credits in XML order.</summary>
    public Collection<XmlTvCredit> Composers { get; } = [];

    /// <summary>Gets editor credits in XML order.</summary>
    public Collection<XmlTvCredit> Editors { get; } = [];

    /// <summary>Gets presenter credits in XML order.</summary>
    public Collection<XmlTvCredit> Presenters { get; } = [];

    /// <summary>Gets commentator credits in XML order.</summary>
    public Collection<XmlTvCredit> Commentators { get; } = [];

    /// <summary>Gets guest credits in XML order.</summary>
    public Collection<XmlTvCredit> Guests { get; } = [];
}
