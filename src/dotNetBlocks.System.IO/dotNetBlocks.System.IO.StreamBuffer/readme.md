# Stream Buffer
A stream buffer wraps a source stream, allowing the consumer code to read the stream without pulling the entire stream into memory.
# Problem solved
In the code pattern where a source wants to write into a sink stream and another process wants to read the stream to write it into another destination, developers often use a MemoryStream class.
This causes temporary spikes in memory usage, especially if the streams are files of unknown size. A 2gb file for example can kill a server along with other side effects of memory starvation, especially in cloud environments.

The stream buffer provides as destination stream that can be written to and read from at the same time, but limiting the amount of "in flight data" or buffer size.

## Example streaming a file as an MVC http response

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
