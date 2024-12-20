using StreamBufferTests;
using System;
using System.Collections.Generic;
using System.IO.Hashing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotNetBlocks.System.IO;

namespace dotNetBlocks.System.IO.Tests.StreamBuffer
{
    [TestClass]
    public class TestHelperTests
    {

        [TestMethod]
        public async Task RandomStreamCalculatesCRCProperly()
        {
            await Task.CompletedTask;

            var testSize = 256;
            var testPosition = testSize - 8;

            using (RandomStream testStream = new RandomStream(testSize))
            {
                Crc32 crc = new Crc32();

                testStream.Position = testPosition;

                // Chop out the used section and calculate the crc.
                var usedBytes = (int)testStream.Position;
                var usedSlice = testStream.ToArray();
                usedSlice = usedSlice.Take(usedBytes).ToArray();

                crc.Append(usedSlice);
                testStream.Position = testPosition;

                testStream.CRC.GetCurrentHash().Should().BeEquivalentTo(crc.GetCurrentHash());

            }
        }

        [TestMethod()]
        public async Task streamcopybytes_test()
        {
            await Task.CompletedTask;

            const int bufferSize = 1024;
            const int sliceSize = bufferSize / 2;
            using (var source = new RandomStream(bufferSize))
            {
                using (var target = new MemoryStream(sliceSize))
                {
                    source.CopyBytes(target, sliceSize);
                    source.Position = 0;
                    target.Position = 0;
                    source.ToArray().Take(sliceSize).ToArray().Should().BeEquivalentTo(target.ToArray());
                }

                var halfslizesize = sliceSize / 2;
                using (var target = new MemoryStream(halfslizesize))
                {
                    source.CopyBytes(target, halfslizesize);
                    source.Position = 0;
                    target.Position = 0;
                    source.ToArray().Take(halfslizesize).ToArray().Should().BeEquivalentTo(target.ToArray());
                }

            }
        }

        [TestMethod()]
        public async Task streamcopybytes_test_Async()
        {
            await Task.CompletedTask;

            const int testSize = 1024;
            const int sliceSize = testSize / 2;
            using (var source = new RandomStream(testSize))
            {
                using (var target = new MemoryStream(testSize))
                {
                    await source.CopyBytesAsync(target, sliceSize);
                    source.Position = 0;
                    target.Position = 0;
                    source.ToArray().Take(sliceSize).ToArray().Should().BeEquivalentTo(target.ToArray());
                }

                var halfslizesize = sliceSize / 2;
                using (var target = new MemoryStream(halfslizesize))
                {
                    await source.CopyBytesAsync(target, halfslizesize);
                    source.Position = 0;
                    target.Position = 0;
                    source.ToArray().Take(halfslizesize).ToArray().Should().BeEquivalentTo(target.ToArray());
                }

            }
        }
        [TestMethod()]
        public async Task streamCopyCalculateCRC()
        {
            const int bufferSize = 1024;
            const int sliceSize = bufferSize / 2;

            var testcrc = new Crc32();

            using (var source = new RandomStream(bufferSize))
            {

                testcrc.Reset();
                source.Position = 0;
                    await source.ReadAndCalculateCRCAsync(testcrc, sliceSize);
                testcrc.GetCurrentHash().ToArray().Should().BeEquivalentTo(source.CRC.GetCurrentHash());
            }
        }
        [TestMethod()]
        public async Task streamCopyCalculateCRCfullbuffer()
        {
            await Task.CompletedTask;

            const int bufferSize = 2049;

            var testcrc = new Crc32();

            using (var source = new RandomStream(bufferSize))
            {

                testcrc.Reset();
                source.Position = 0;
                await source.ReadAndCalculateCRCAsync(testcrc, bufferSize);
                testcrc.GetCurrentHash().ToArray().Should().BeEquivalentTo(source.CRC.GetCurrentHash());
            }
        }
    }
}