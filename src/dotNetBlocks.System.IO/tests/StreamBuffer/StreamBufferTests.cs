using dotNetBlocks.System.IO;
using dotNetBlocks.System.IO.Tests.StreamBuffer;
using FluentAssertions.Extensions;
using System.Diagnostics;
using System.IO.Hashing;


namespace StreamBufferTests
{
    [TestClass]
    public partial class Background_AsyncWriter_TestAsync
    {
        /// <summary>
        /// Test data written into the buffer is readhalf from it correctly.
        /// </summary>
        [TestCategory("pass through"),TestMethod]
        public void BasicPassThroughTest()
        {
            const int testSize = 4096;
            StreamBuffer buffer = new();

            using (RandomStream sourceStream = new RandomStream(testSize))
            {
                var readHash = new Crc32();

                // Write source stream into pipe
                sourceStream.CopyToAsync(buffer.WriteStream,testSize);
                // Read destination stream
                buffer.ReadStream.ReadAndCalculateCRC(readHash,testSize);

                // Compare checksum
                CollectionAssert.AreEquivalent(sourceStream.CRC.GetCurrentHash(), readHash.GetCurrentHash());
            }


        }


        /// <summary>
        /// Full buffer blocks write until readhalf.
        /// </summary>
        [TestCategory("Buffer Blocking"), TestMethod]
        public async Task FullBufferBlocksWriteUntilReadAsync()
        {
            const int testSize = 1024; //4096;
            const int bufferSize = testSize / 2;

            StreamBuffer buffer = new(bufferSize);
            var readHash = new Crc32();
            using (RandomStream sourceStream = new RandomStream(testSize))
            {
                var position = sourceStream.Position;

                // define methods
                // Write to buffer
                int writeSize = 0;
                var write = async Task () =>  await sourceStream.CopyBytesAsync(buffer.WriteStream, writeSize);

                // read buffer
                int readSize = 0;
                var read = async Task () => await buffer.ReadStream.ReadAndCalculateCRCAsync(readHash, readSize, null);
                var readDiscardByte = async Task() => await buffer.ReadStream.ReadAndCalculateCRCAsync(new Crc32(), 1, null);

                // End define methods

                // fill the buffer. less one byte so we can complete.
                writeSize = bufferSize-1;
                await write.Should().CompleteWithinAsync(250.Milliseconds(),because:"buffer is empty");


                // Test we are blocked until a read.
                position = sourceStream.Position; // Store the source position..
                await write.Should().NotCompleteWithinAsync(250.Milliseconds(), because: "buffer is full and writer is blocked.");
                sourceStream.Position = position; // restore the source position.

                // Read the written data.
                readSize = writeSize;
                await read.Should().CompleteWithinAsync(250.Milliseconds(), because: "Read the written data");

                // Validate we read what we wrote accurately.
                sourceStream.CRC.GetCurrentHash().Should().BeEquivalentTo(readHash.GetCurrentHash(), because: "read and write checksums must match.");

                // need to read extra to unlock writing.
                await read.Should().CompleteWithinAsync(250.Milliseconds(), because: "Read extra bytes");

                // We can write again.
                await write.Should().CompleteWithinAsync(250.Milliseconds(), because: "buffer is empty again");

            }
        }

    }

}