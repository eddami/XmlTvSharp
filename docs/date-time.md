# Date and Time

XMLTV date/time values are represented by `XmlTvDateTime`.

XMLTV supports partial precision values such as:

```text
2026
202606
20260605
2026060514
202606051430
20260605143000
20260605143000 +0000
20260605143000 Europe/London
```

`XmlTvDateTime` preserves the supplied precision and timezone token:

```csharp
XmlTvDateTime value = XmlTvDateTime.Parse("20260605143000 +09:30");

Console.WriteLine(value.Precision);       // Second
Console.WriteLine(value.Zone?.Value);     // +09:30
Console.WriteLine(value.ToXmlTvString()); // 20260605143000 +09:30
```

## Exact Instants

Only second-precision values can be converted to `DateTimeOffset`:

```csharp
DateTimeOffset instant = XmlTvDateTime
    .Parse("20260605143000 +0000")
    .ToDateTimeOffset();
```

An omitted timezone is interpreted as UTC when converting to `DateTimeOffset`.

Named timezone tokens require an explicit resolver:

```csharp
DateTimeOffset instant = XmlTvDateTime
    .Parse("20260605143000 BST")
    .ToDateTimeOffset((token, value, out offset) =>
    {
        offset = TimeSpan.FromHours(1);
        return token == "BST";
    });
```

## Creating Values

Use `FromDateTimeOffset` when you already have an exact .NET instant:

```csharp
XmlTvDateTime value = XmlTvDateTime.FromDateTimeOffset(
    new DateTimeOffset(2026, 6, 5, 14, 30, 0, TimeSpan.Zero));
```

Use the constructor when you need XMLTV partial precision:

```csharp
var day = new XmlTvDateTime(2026, 6, 5);
```
