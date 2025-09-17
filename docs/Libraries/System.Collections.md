<!---
Title: DotNetBlocks.System.Collections

ShowInSidebar: true
Excerpt: Extends the System.Collections namespace with new functionality
Level: 0
Order: 0
--->

# DotNetBlocks.System.Collections

### Overview

extends functionality for collections in the System.Collections and the System.Collections.Generic namespace.

### Getting started
[ NuGet : dotnetBlocks.System.Collections](https://www.nuget.org/packages/dotNetBlocks.System.Collections)

[Licensing and other information](/)

### Functionality


# System.Collections.Generic.Extensions.

ThrowIfItemIsDefault - throws an exception if an item in the enumeration is default or null. This ensure the collection only has non-null and non-default values.
IgnoreDefaultValues - filters out default or null values when the enumeration is being executed ensures no null or non-default values are returned without interupting the enumeration.

MustHaveOneOrMore - throws an exception if the enumeration or queryable does not have at least one item in the enumeration. It more peformant then Any() and good for ensuring there are results.







# Problem solved

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