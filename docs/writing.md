# Writing

XmlTvSharp can write complete documents or write top-level elements one at a time.

## Complete Document

Use `XmlTvWriter.WriteAsync` when your document is already in memory:

```csharp
var document = new XmlTvDocument();

document.Channels.Add(new XmlTvChannel("channel-one", "Channel One"));
document.Programmes.Add(
    new XmlTvProgramme(
        XmlTvDateTime.Parse("20260605120000 +0000"),
        "channel-one",
        "News")
    {
        Stop = XmlTvDateTime.Parse("20260605123000 +0000")
    });

await XmlTvWriter.WriteAsync(document, "guide.xml");
```

Channels are written before programmes.

## Forward-Only Writing

Use the writer lifecycle when you want to write channels and programmes as they become available:

```csharp
using var writer = new XmlTvWriter("guide.xml");

await writer.StartAsync();
await writer.WriteChannelAsync(new XmlTvChannel("channel-one", "Channel One"));
await writer.WriteProgrammeAsync(
    new XmlTvProgramme(
        XmlTvDateTime.Parse("20260605120000 +0000"),
        "channel-one",
        "News"));
await writer.CompleteAsync();
```

The lifecycle rules are:

- Call `StartAsync` once.
- Write all channels before the first programme.
- Call `CompleteAsync` once.
- Channel IDs must be unique within the writer.

## Writer Options

Use `XmlTvWriterOptions` for XML output and compatibility behavior. See [Writer Options](./writer-options.md).

## Validation

The writer validates the model while writing. For example, channels require at least one display name, programmes require at least one title, and channel IDs must be unique within a writer.

See [Error Handling](./error-handling.md) for read and write exception behavior.
