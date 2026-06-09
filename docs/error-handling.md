# Error Handling

XmlTvSharp reports read and write failures with library-specific exceptions.

## Read Errors

Read failures throw `XmlTvReadException`:

```csharp
try
{
    XmlTvDocument document = await XmlTvReader.ReadAsync("guide.xml");
}
catch (XmlTvReadException exception)
{
    Console.WriteLine(exception.Message);
    Console.WriteLine($"{exception.LineNumber}:{exception.LinePosition}");
}
```

When the underlying XML reader provides line information, `LineNumber` and `LinePosition` identify where the failure was detected.

## Write Errors

Write failures throw `XmlTvWriteException`:

```csharp
try
{
    await XmlTvWriter.WriteAsync(document, "guide.xml");
}
catch (XmlTvWriteException exception)
{
    Console.WriteLine(exception.Message);
}
```

The writer validates required fields while writing. For example:

- a channel must have at least one display name
- a programme must have at least one title
- channel IDs must be unique within a writer
- channels must be written before programmes

When a raw XML writer or I/O failure is available, it is exposed as the inner exception.
