using dotNetBlocks.System.IO;
using dotNetBlocks.System.IO.Tests.StreamBuffer;
using Mono.Cecil.Cil;
using System.Diagnostics;
using System.IO.Hashing;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using Shouldly;
using dotNetBlocks.System.IO.Tests;



namespace StreamBufferTests
{
    public partial class Background_AsyncWriter_TestAsync
    {

        /// <summary>
        /// Test using a synchronous method to write in the background.
        /// </summary>
        /// <remarks> You can rapidly deadlock your self with these tests as blocking is part of the buffer design.</remarks>
        [TestCategory("Background"),TestMethod]
        public async Task Background_SyncWriter_TestAsync()
        {

            await Task.CompletedTask;

            const int blockSize = 1024;
            const int testSize = blockSize*2;
            const int bufferSize = blockSize;
            StreamBuffer buffer = new(bufferSize);

            using (RandomStream sourceStream = new RandomStream(testSize))
            {
                var readHash = new Crc32();

                // Write source stream into pipe. The pipe hangs as you hit the buffer size.


                _ = buffer.StartBackgroundWrite(async Task(s, c) => await sourceStream.CopyBytesAsync(s, blockSize, c), CancellationToken.None);

                var write = async Task () => await buffer.backgroundWriteTask!;

                write.ShouldNotCompleteIn(TimeSpan.FromMilliseconds(250), "Write blocks at size and does not complete until a read.");

                // Read destination stream
                var read = async Task() => await buffer.ReadStream.ReadAndCalculateCRCAsync(readHash, blockSize, default);
                Should.CompleteIn(read, TimeSpan.FromMilliseconds(250), " We read all the data..");

                // Compare checksum
                CollectionAssert.AreEquivalent(sourceStream.CRC.GetCurrentHash(), readHash.GetCurrentHash());
            };


        }

        /// <summary>
        /// Writes in background using an async method and then reads the result.
        /// </summary>
        [TestCategory("Background"), TestMethod]
        public async Task Background_AsyncWriter_Test_Async()
        {
            const int blockSize = 1024;
            const int bufferSize = blockSize;
            const int testSize = blockSize*2;
            StreamBuffer buffer = new(bufferSize);

            await Task.CompletedTask;

            using (RandomStream sourceStream = new RandomStream(testSize))
            {
                var readHash = new Crc32();

                // Write source stream into pipe
                _= buffer.StartBackgroundWrite(async Task (s, c) => await sourceStream.CopyBytesAsync(s, blockSize)  , CancellationToken.None);

                var  writeComplete = async Task() =>   await buffer.backgroundWriteTask!;

                writeComplete.ShouldNotCompleteIn(TimeSpan.FromMilliseconds(250), "Full buffer blocks writer.");

                // Read destination stream
                var read = async Task () =>  await buffer.ReadStream.ReadAndCalculateCRCAsync(readHash, blockSize, default);

                Should.CompleteIn(read, TimeSpan.FromMilliseconds(250), " Reading the buffer");
                Should.CompleteIn(writeComplete, TimeSpan.FromMilliseconds(250), "Buffer unlocked.");

                // Compare checksum
                readHash.GetCurrentHash().ShouldBeEquivalentTo(sourceStream.CRC.GetCurrentHash(), "Read and write CRC match.");
            }
        }

        /// <summary>
        /// Background Synchronous writer method is blocked until the data is readhalf
        /// and then it is released. Test hangs if this doesn't work properly.
        /// </summary>
        [TestCategory("Background"), TestCategory("Blocking"), TestMethod]
        public async Task Background_sync_Writer_blocked_until_read_async()
        {
            const int blockSize = 1024;
            const int testSize = blockSize * 8;

            const int bufferSize = blockSize;


            StreamBuffer buffer = new(bufferSize); // Forces the write stream to block.

            // The source data is larger than the buffer so we should block.
            using (RandomStream sourceStream = new RandomStream(testSize))
            {
                var readHash = new Crc32();


                // Write source stream into pipe in the background.
                // The write will block under the reader has readhalf everything.
                _ = buffer.StartBackgroundWrite( async (s, c) => { await sourceStream.CopyBytesAsync(s, bufferSize, c); });


                var writeTask = async Task() => await buffer.backgroundWriteTask!;

                writeTask.ShouldNotCompleteIn(TimeSpan.FromMilliseconds( 250), "Buffer is full and blocked until read.");


                await buffer.ReadStream.ReadAndCalculateCRCAsync(readHash, bufferSize, default);

                // Now the write task shoudl be complete.
                Should.CompleteIn(writeTask, TimeSpan.FromMilliseconds(250), "Unblocked writer completes.");

                // Compare checksum
                readHash.GetCurrentHash().ShouldBeEquivalentTo(sourceStream.CRC.GetCurrentHash());
            }
        }

        /// <summary>
        /// Async Background writer is blocked until the data is read half
        /// and then it is released. Test hangs if this doesn't work properly.
        /// </summary>
        [TestCategory("Background"),TestCategory("Blocking"), TestMethod]
        public async Task Background_Async_Writer_blocked_until_read_async()
        {

            const int blockSize = 1024;
            const int bufferSize = blockSize * 2;
            const int testSize = blockSize * 4;
            StreamBuffer buffer = new(bufferSize); // Forces the write stream to block.

            // The source data is larger than the buffer so we should block.
            using (RandomStream sourceStream = new RandomStream(testSize))
            {
                var readHash = new Crc32();

                // Write source stream into pipe in the background.
                // We will hang here if this doesn't work properly.

                _ = buffer.StartBackgroundWrite(
                    async (Stream s, CancellationToken c) =>
                {
                    {
                        await sourceStream.CopyToAsync(s, testSize, c);
                    }
                }, CancellationToken.None);

                Func<Task> waitComplete = async () => await buffer.backgroundWriteTask!;

                waitComplete.ShouldNotCompleteIn(TimeSpan.FromMicroseconds(250), "full buffer blocks writer");

                // Read destination stream and calculate the CRC.
                // This releases the writer.
                await buffer.ReadStream.ReadAndCalculateCRCAsync(readHash, testSize, default);

                // Now the write should complete because of the read.
                Should.CompleteIn(waitComplete,TimeSpan.FromMilliseconds(250));
                buffer.backgroundWriteTask!.IsCompletedSuccessfully.ShouldBeTrue();

                // Compare checksum
                sourceStream.CRC.GetCurrentHash().ShouldBeEquivalentTo(readHash.GetCurrentHash(), "Write and read checksums must match.");
            }
        }

        /// <summary>
        /// Async Background writer is blocked until the data is read
        /// and then it is released. Test hangs if this doesn't work properly.
        /// </summary>
        [TestCategory("Background"), TestMethod]
        public async Task Background_async_Writer_background_async_Reader()
        {

            const int blockSize = 1024;
            const int testSize = blockSize * 4;
            const int bufferSize = blockSize;
            StreamBuffer buffer = new(bufferSize); // Forces the write stream to block.

            // The source data is larger than the buffer so we should block.
            using (RandomStream sourceStream = new RandomStream(testSize))
            {
                var readHash = new Crc32();

                // Write source stream into pipe in the background.
                // Writer will block until more data is read.

                var writeWait = async () => await buffer.StartBackgroundWrite(async (s, c) => await sourceStream.CopyToAsync(s, blockSize, c), default);

                writeWait.ShouldNotCompleteIn(TimeSpan.FromMilliseconds(250), "full buffer blocks write.");

                // Read destination stream
                // This releases the writer.
                // 
                _= buffer.StartBackgroundRead(async (s, c) => await s.ReadAndCalculateCRCAsync(readHash, blockSize,c));

                var readWait = async () => await buffer.backgroundReadTask!;

                Should.CompleteIn(readWait, TimeSpan.FromMilliseconds(250), "Buffer ready to read.");


                // Reset the source stream to calculate the correct CRC.
                sourceStream.Position = blockSize;
                // Compare checksum
                readHash.GetCurrentHash().ShouldBeEquivalentTo(sourceStream.CRC.GetCurrentHash());
            }

            await Task.CompletedTask;

        }
    }
}