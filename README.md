[![Demo of the Streaming code in action](https://i.imgur.com/HwCWR6p.png)](https://youtu.be/5oReOatUnks)

A couple of tiny classes which let you write code like this, 
which can run on a web server or in the browser:

```csharp
await sl.StartSection("Example");
await sl.Announce("Hello world");
foreach (var continent in "Asia, Africa, North America, South America, Antarctica, Europe, Australia".Split(", ")) {
    await Task.Delay(1000);
    await sl.StartSection($"Calling {continent}");
    await Task.Delay(1000);


    await sl.Announce($"Hello from {continent}");
    await sl.EndSection();
}
await sl.EndSection();
```


Of course this is more useful if you do some intensive or slow things 
instead of calling Task.Delay :smiley:. Full example is in 
[SampleWeb/Startup.cs](SampleWeb/Startup.cs).

This was inspired by a [question I found at stackoverflow](https://stackoverflow.com/questions/42722936/asp-net-core-1-1-chunked-responses).

# Details

Web application page load times matter. No one wants to stare at a 
"Loading..." spinner for very long. If your application needs data 
from other services and those services are slow to respond, you can:

1. Use javascript to request information in the background after the main 
page has loaded. **Problem: complexity.**
1. Capture the data you need in your own cache or database, so you can send 
it to a user quickly on request. **Problem: you may have to cache a lot of 
data to satisfy all your user requests.**
1. Stream the output to the user a bit at a time. **Problem: hard to find 
examples of doing this. That's why I wrote this.**

Web browsers are quite capable of displaying a partial web page and updating it 
as more HTML content appears. How you go about doing this is going to vary 
between web frameworks. It's pretty simple to do in ASP.Net Core using
[asynchronous C#](https://docs.microsoft.com/en-us/dotnet/csharp/async).

What this means is that you can write code which can run away for minutes 
just as happily in a web server or as a standalone tool. Your code will pass
around an instance of [StructuredLogger](Lib/StructuredLogger.cs) and will call
the Announce message to both log to the console and to a web browser if
running in a web server.

In this repository there's 2 key classes, both of which are very short, and
make use of [Asynchrnous C#](https://docs.microsoft.com/en-us/dotnet/csharp/async)

Fundamentally, HTTP is simply a TCP connection, and what we're doing here
is keeping the TCP connection for a while. We send down enough HTML to get
the browser started, and then lumps of HTML as we create content, followed by
finishing up the HTML and closing the connection. On the other hand if the
TCP connection goes away then we've lost contact with the client, and the OS
and .Net will detect this and the code here will throw an exception the next
time you call Announce. This stops your code continuing with expensive computation
where the results will no longer be seen; it's a bit like pressing Control-C in a
console.

We found this useful for long running operations which can be run on demand by
users as well as run regularly from a TeamCity server. 

## The StreamedAction class

[StreamedAction](Lib/StreamedAction.cs) is a static class which generarates
a stream of writes that together form a simple HTML document. It registers
itself as a callback to [StructuredLogger](Lib/StructuredLogger.cs). 
It then invokes its callback which is expected to end up calling StructuredLogger 
static methods.

## The StructuredLogger class

[StructuredLogger](Lib/StructuredLogger.cs) is a class which provides
an Announce method for producing a line of output, and a StartSection/EndSection
pair to mark when you are doing something so that you can get structured logging.
This can be used in a console program and will work fine, printing output to the 
console with indentation if you use StartSection/EndSection. 

The SetupRecording method adds a callback, and we use this in
[StreamedAction](Lib/StreamedAction.cs).

# Dependencies

* [.Net Core 2.0](https://github.com/dotnet/core/blob/master/release-notes/download-archives/2.0.0-download.md)

Tested on Windows 10 Enterprise x86-64, but should work on MacOS and Linux as well.

# TODO 

1. Add test cases that exercise StreamedAction.
