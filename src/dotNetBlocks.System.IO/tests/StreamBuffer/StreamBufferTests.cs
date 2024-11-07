using System.Diagnostics;
using System.IO.Hashing;


namespace StreamBufferTests
{
    [TestClass]
    public partial class StreamBufferTests
    {
        /// <summary>
        /// Test data written into the buffer is read from it correctly.
        /// </summary>
        [TestMethod]
        public void BasicPassThroughTest()
        {
            const int testSize = 4096;
            StreamBuffer buffer = new();

            using (RandomStream sourceStream = new RandomStream(testSize))
            {
                var readHash = new Crc32();

                // Write source stream into pipe
                sourceStream.CopyTo(buffer.WriteStream);
                // Read destination stream
                buffer.ReadStream.ReadAndCalculateCRC(testSize, readHash);

                // Compare checksum
                CollectionAssert.AreEquivalent(sourceStream.CRC.GetCurrentHash(), readHash.GetCurrentHash());
            }


        }


        /// <summary>
        /// Full buffer blocks write until read.
        /// </summary>
        [TestMethod]
        public async Task FullBufferBlocksWriteUntilRead()
        {
            const int testSize = 4096;
            StreamBuffer buffer = new(testSize/2);

            using (RandomStream sourceStream = new RandomStream(testSize))
            {
                var readHash = new Crc32();

                // Write source stream into pipe
                Assert.IsTrue(
                    await StreamBufferTestHelper.ExecuteWithTimeoutAsync(
                        () =>
                            { 
                                sourceStream.CopyTo(buffer.WriteStream);}
                        ,TimeSpan.FromMilliseconds(250)
                        ));
                // Read destination stream
                buffer.ReadStream.ReadAndCalculateCRC(testSize, readHash);

                // Compare checksum
                CollectionAssert.AreEquivalent(sourceStream.CRC.GetCurrentHash(), readHash.GetCurrentHash());
            }
        }

    }

}