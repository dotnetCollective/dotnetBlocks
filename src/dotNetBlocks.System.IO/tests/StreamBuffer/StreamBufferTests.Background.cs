using dotNetBlocks.System.IO.Tests.StreamBuffer;
using FluentAssertions.Extensions;
using Mono.Cecil.Cil;
using System.Diagnostics;
using System.IO.Hashing;
using System.Threading.Tasks.Sources;



namespace StreamBufferTests
{
    public partial class Background_AsyncWriter_TestAsync
    {

        /// <summary>
        /// Test using a synchronous method to write in the background.
        /// </summary>
        /// <remarks> You can rapidly deadlock your self with these tests as blocking is part of the buffer design.</remarks>
        [TestMethod]
        public async Task Background_SyncWriter_TestAsync()
        {
            const int testSize = 4096;
            const int bufferSize = testSize / 2;
            StreamBuffer buffer = new( bufferSize);

            using (RandomStream sourceStream = new RandomStream(testSize))
            {
                var readHash = new Crc32();

                // Write source stream into pipe and close it so the reader doesn't wait for more data.

                _= buffer.StartBackgroundWriteAsync(async (s, c) => { await sourceStream.CopyToAsync(s); }, CancellationToken.None);

                // Read destination stream
                var read = () => buffer.ReadStream.ReadAndCalculateCRCAsync(readHash, bufferSize);

                await read.Should().CompleteWithinAsync(1.Seconds());

                // Compare checksum
                CollectionAssert.AreEquivalent(sourceStream.CRC.GetCurrentHash(), readHash.GetCurrentHash());
            }

        }

        /// <summary>
        /// Writes in background using an async method.
        /// </summary>
        [TestMethod]
        public async Task Background_AsyncWriter_Test_Async()
        {
            const int testSize = 4096;
            StreamBuffer buffer = new();

            using (RandomStream sourceStream = new RandomStream(testSize))
            {
                var readHash = new Crc32();

                // Write source stream into pipe
                buffer.StartBackgroundWriteAsync(async (s, c) => await sourceStream.CopyToAsync(s, c), CancellationToken.None).Should().NotBeNull();
                await buffer.backgroundWriteTask!;
                buffer.WriteStream.Close();
                // Read destination stream
               await buffer.ReadStream.ReadAndCalculateCRCAsync(readHash);

                // Compare checksum
                CollectionAssert.AreEquivalent(sourceStream.CRC.GetCurrentHash(), readHash.GetCurrentHash());
            }
        }

        /// <summary>
        /// Background Synchronous writer method is blocked until the data is readhalf
        /// and then it is released. Test hangs if this doesn't work properly.
        /// </summary>
        [TestMethod]
        public async Task Background_sync_Writer_blocked_until_read_async()
        {
            const int testSize = 4096;
            const int blockSize = testSize / 2;
            StreamBuffer buffer = new(blockSize); // Forces the write stream to block.

            // The source data is larger than the buffer so we should block.
            using (RandomStream sourceStream = new RandomStream(testSize))
            {
                var readHash = new Crc32();

                // Write source stream into pipe in the background.
                // The write will block under the reader has readhalf everything.

                var writeTask = buffer.StartBackgroundWriteAction((s, c) => { sourceStream.CopyTo(s); s.Close();  });


                Task.WaitAll(new Task[] { writeTask }, 100).Should().BeFalse();

                await buffer.ReadStream.ReadAndCalculateCRCAsync(readHash);
                Task.WaitAll(new Task[] { writeTask }, 100).Should().BeTrue();

                // Now the write task shoudl be complete.

                // Compare checksum
                CollectionAssert.AreEquivalent(sourceStream.CRC.GetCurrentHash(), readHash.GetCurrentHash());
            }
        }

        /// <summary>
        /// Async Background writer is blocked until the data is readhalf
        /// and then it is released. Test hangs if this doesn't work properly.
        /// </summary>
        [TestMethod]
        public async Task Background_Async_Writer_blocked_until_read_async()
        {

            const int bufferSize = 1024;
            const int testSize = bufferSize * 8;
            StreamBuffer buffer = new(bufferSize); // Forces the write stream to block.

            // The source data is larger than the buffer so we should block.
            using (RandomStream sourceStream = new RandomStream(testSize))
            {
                var readHash = new Crc32();

                // Write source stream into pipe in the background.
                // We will hang here if this doesn't work properly.

                _= buffer.StartBackgroundWriteAsync( 
                    async (Stream s, CancellationToken c) =>
                {
                    {
                        await sourceStream.CopyToAsync(s, c);
                    }
                    }, CancellationToken.None);

                //Func<Task> waitComplete = async () => await buffer.backgroundWriteTask;
                Func<Task> waitComplete = async () => await buffer.backgroundWriteTask;
                
                await waitComplete.Should().NotCompleteWithinAsync(250.Milliseconds(),because:"full buffer blocks writer");

                // Read destination stream and calculate the CRC.
                // This releases the writer.
                await buffer.ReadStream.ReadAndCalculateCRCAsync(readHash,testSize);

                await waitComplete.Should().CompleteWithinAsync(250.Milliseconds());
                buffer.backgroundWriteTask.IsCompletedSuccessfully.Should().BeTrue();

                // Compare checksum
                sourceStream.CRC.GetCurrentHash().Should().BeEquivalentTo(readHash.GetCurrentHash(), because: "Write and read checksums must match.");
            }
        }

        /// <summary>
        /// Async Background writer is blocked until the data is readhalf
        /// and then it is released. Test hangs if this doesn't work properly.
        /// </summary>
        [TestMethod]
        public async Task Background_async_Writer_background_async_Reader()
        {
            const int testSize = 4096;
            const int blockSize = testSize / 2;
            StreamBuffer buffer = new(blockSize); // Forces the write stream to block.

            // The source data is larger than the buffer so we should block.
            using (RandomStream sourceStream = new RandomStream(testSize))
            {
                var readHash = new Crc32();

                // Write source stream into pipe in the background.
                // We will hang here if this doesn't work properly.

                await buffer.StartBackgroundWriteAsync(async (s, c) => await sourceStream.CopyToAsync(s, c), CancellationToken.None);

                // Read destination stream
                // This releases the writer.
                // 
                await buffer.StartBackgroundRead(async(s, c) => await s.ReadAndCalculateCRCAsync(readHash));

                await buffer.WaitForBackgroundAsync();

                // Compare checksum
                CollectionAssert.AreEquivalent(sourceStream.CRC.GetCurrentHash(), readHash.GetCurrentHash());
            }
        }
    }
}