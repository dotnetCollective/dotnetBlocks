<!---
Title: DotNetBlocks.System.IO

ShowInSidebar: true
Excerpt: New stream functionality.
Level: 0
Order: 0
--->

# DotNetBlocks.System.IO

### Overview

Adds new buffering and copy functionality to streams in the system.io namespace.

### Getting started
[ NuGet : dotnetBlocks.System.IO](https://www.nuget.org/packages/dotNetBlocks.System.IO)

[Licensing and other information](/)

### Functionality


New Stream functionality.

* Stream Buffer - advanced concurrent reading and writing pass through
* Stream Copy Extensions - let you calculate the CRC or copy parts of a stream.


# Stream Buffer
A stream buffer wraps a source stream, allowing the consumer code to read the stream without pulling the entire stream into memory. Supports background process threading.
# Problem solved
In the code pattern where a source wants to write into a sink stream and another process wants to read the stream to write it into another destination, developers often use a MemoryStream class.
This causes temporary spikes in memory usage, especially if the streams are files of unknown size. A 2gb file for example can kill a server along with other side effects of memory starvation, especially in cloud environments.

The stream buffer provides as destination stream that can be written to and read from at the same time, but limiting the amount of "in flight data" or buffer size.

[How it works: Understanding pipes, flow control and not dead-locking.](https://learn.microsoft.com/en-us/dotnet/standard/io/pipelines#backpressure-and-flow-control)

# # Examples

1. streaming a file as an MVC http response

```c#

 // Using memory stream - this is the in memory problem

        var blobClient = container.GetBlobClient(blobPath);
        var resultStream = new MemoryStream();

            await blobClient.DownloadToAsync(resultStream);
            // Reset the stream.
            resultStream.position = 0;

            // Return the stream.
        return new FileStreamResult(stream, "application/pdf")
 
 ```

```c#

 // Using buffer stream - eliminates memory issue

        var blobClient = container.GetBlobClient(blobPath);
        var resultStream = new StreamBuffer();

            await blobClient.DownloadToAsync(resultStream.WriteStream);

            // Return the stream.
        return new FileStreamResult(resultStream.ReadStream, "application/pdf")
 
 ```


 This example is very basic and does not show other advantages including built in alternative threading models etc.


 * Stream Extensions - New functions to copy between streams and to calculate CRC values for a stream.




# Stream Copy extensions

# Problem solved

Existing methods on the streams to copy from one stream to another expect to copy the entire stream and block until the process is complete. For streams like the StreamBuffer implementation that block until there is capacity, the applications can hang.

These functions let you copy a subset of the stream from one stream to another and not the entire stream.

When you are testing copy functionality and working with streams where the may be hidden data issues, CRC functions can detect issues and ensure there are no errors in the process.


## Examples
2. copying parts of a a stream.

 ```cs
 
 using(var sourceStream = new FileStream("sourcefile", FileMode.Open))
 {
 // Copy 100 bytes from the source stream to the destination stream.
 using(var destinationStream = new FileStream("destinationfile", FileMode.Create))
 {
 sourceStream.CopyBytes(destinationStream, 100);
 }
 
 ```
 }