using System.Diagnostics;
using System.IO.Hashing;


namespace StreamBufferTests
{
    public partial class StreamBufferTests
    {

        /// <summary>
        /// Test using a synchronous method to write in the background.
        /// </summary>
        [TestMethod]
        public void Background_SyncWriter_Test()
        {
            const int testSize = 4096;
            StreamBuffer buffer = new();

            using (RandomStream sourceStream = new RandomStream(testSize))
            {
                var readHash = new Crc32();

                // Write source stream into pipe
                buffer.StartBackgroundWrite((s, c) => sourceStream.CopyTo(s), default, CancellationToken.None);
                // Read destination stream
                buffer.ReadStream.ReadAndCalculateCRC(testSize, readHash);

                // Compare checksum
                CollectionAssert.AreEquivalent(sourceStream.CRC.GetCurrentHash(), readHash.GetCurrentHash());
            }

        }

        /// <summary>
        /// Writes in background using an async method.
        /// </summary>
        [TestMethod]
        public void Background_AsyncWriter_Test()
        {
            const int testSize = 4096;
            StreamBuffer buffer = new();

            using (RandomStream sourceStream = new RandomStream(testSize))
            {
                var readHash = new Crc32();

                // Write source stream into pipe
                buffer.StartBackgroundWrite(async (s, c) => await sourceStream.CopyToAsync(s, c), default, CancellationToken.None);
                // Read destination stream
                buffer.ReadStream.ReadAndCalculateCRC(testSize, readHash);

                // Compare checksum
                CollectionAssert.AreEquivalent(sourceStream.CRC.GetCurrentHash(), readHash.GetCurrentHash());
            }
        }

        /// <summary>
        /// Background Synchronous writer method is blocked until the data is read
        /// and then it is released. Test hangs if this doesn't work properly.
        /// </summary>
        [TestMethod]
        public void Background_sync_Writer_blocked_until_read()
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

                buffer.StartBackgroundWrite((s, c) => sourceStream.CopyTo(s), default, CancellationToken.None);

                // Read destination stream
                // This releases the writer.
                buffer.ReadStream.ReadAndCalculateCRC(testSize, readHash);

                // Compare checksum
                CollectionAssert.AreEquivalent(sourceStream.CRC.GetCurrentHash(), readHash.GetCurrentHash());
            }
        }

        /// <summary>
        /// Async Background writer is blocked until the data is read
        /// and then it is released. Test hangs if this doesn't work properly.
        /// </summary>
        [TestMethod]
        void Background_Async_Writer_blocked_until_read()
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

                buffer.StartBackgroundWrite(async (s, c) => await sourceStream.CopyToAsync(s, c), default, CancellationToken.None);

                // Read destination stream
                // This releases the writer.
                buffer.ReadStream.ReadAndCalculateCRC(testSize, readHash);

                // Compare checksum
                CollectionAssert.AreEquivalent(sourceStream.CRC.GetCurrentHash(), readHash.GetCurrentHash());
            }
        }

        /// <summary>
        /// Async Background writer is blocked until the data is read
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

                await buffer.StartBackgroundWrite(async (s, c) => await sourceStream.CopyToAsync(s, c), default, CancellationToken.None);

                // Read destination stream
                // This releases the writer.
                // 
                await buffer.StartBackgroundRead((s, c) => s.ReadAndCalculateCRC(testSize, readHash));

                await buffer.WaitForBackgroundAsync();

                // Compare checksum
                CollectionAssert.AreEquivalent(sourceStream.CRC.GetCurrentHash(), readHash.GetCurrentHash());
            }
        }
    }
}