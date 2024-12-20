using System.Buffers;
using System.IO.Hashing;

namespace dotNetBlocks.System.IO
{
    public static class StreamExtensions
    {
        private const int DefaultBufferSize = 81920;

        /// <summary>
        /// Reads the source and calculates the CRC32 hash.
        /// </summary>
        /// <param name="source">The source to read from.</param>
        /// <param name="crc">The CRC32 instance to calculate the hash.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async ValueTask ReadAndCalculateCRCAsync(this Stream source, Crc32 crc, long byteCount, CancellationToken cancellation = default)
        {
            using (var buffer = MemoryPool<byte>.Shared.Rent(DefaultBufferSize)) // Allocate a buffer for the copy process.
            {
                while (byteCount > 0)
                {
                    var readSlice = buffer.Memory.Slice(0, (int)Math.Min(byteCount, buffer.Memory.Length));
                    var bytesRead = await source.ReadAsync(readSlice);

                    if (bytesRead == 0) return; // Source is complete.

                    crc.Append(readSlice.Span[..bytesRead]);
                    byteCount -= bytesRead; //
                }
            }
        }


        /// <summary>
        /// Reads the source and calculates the CRC32 hash.
        /// </summary>
        /// <param name="source">The source to read from.</param>
        /// <param name="crc">The CRC32 instance to calculate the hash.</param>
        public static void ReadAndCalculateCRC(this Stream source, Crc32 crc, long byteCount)
        {
            using (var buffer = MemoryPool<byte>.Shared.Rent(DefaultBufferSize)) // Allocate a buffer for the copy process.
            {
                while (byteCount > 0)
                {
                    var readSlice = buffer.Memory.Slice(0, (int)Math.Min(byteCount, buffer.Memory.Length));
                    var bytesRead = source.Read(readSlice.Span);
                    if (bytesRead == 0) return; // Source is complete.
                    crc.Append(buffer.Memory.Span.Slice(0, bytesRead));
                    byteCount -= bytesRead; //
                }
            }
        }

        /// <summary>
        /// Copies the content of the source source to the target source synchronously.
        /// </summary>
        /// <param name="source">The source source.</param>
        /// <param name="target">The target source.</param>
        /// <param name="count">The number of bytes to copy.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The number of bytes copied.</returns>
        public static async ValueTask<int> CopyBytesAsync(this Stream source, Stream target, long count, CancellationToken cancellationToken = default)
        {
            // Check for a cancellation.
            cancellationToken.ThrowIfCancellationRequested();

            using (var buffer = MemoryPool<byte>.Shared.Rent(DefaultBufferSize)) // Allocate the buffer.
            {
                var bytesRemaining = count;
                var bytesCopied = 0;
                while (bytesRemaining > 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    // Fill the buffer. Read the remaining bytes or a full buffer. 
                    var readSlice = buffer.Memory.Slice(0, (int)Math.Min(bytesRemaining, buffer.Memory.Length));
                    var bytesRead = await source.ReadAsync(readSlice, cancellationToken);

                    // Is the source done?
                    if (bytesRead == 0) return bytesCopied;

                    // Write the bytes to the target.
                    await target.WriteAsync(readSlice.Slice(0, bytesRead), cancellationToken);

                    // Adjust the counters.
                    bytesCopied += bytesRead;
                    bytesRemaining -= bytesRead;
                }
                cancellationToken.ThrowIfCancellationRequested();
                return bytesCopied;
            }
        }

        /// <summary>
        /// Copies the content of the source source to the target source synchronously.
        /// </summary>
        /// <param name="source">The source source.</param>
        /// <param name="target">The target source.</param>
        /// <param name="count">The number of bytes to copy.</param>
        /// <param name="bufferSize">The size of the buffer to use for copying. Default is 81920 bytes.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>The number of bytes copied.</returns>
        public static int CopyBytes(this Stream source, Stream target, long count, CancellationToken cancellationToken = default)
        {
            // Check for a cancellation.
            cancellationToken.ThrowIfCancellationRequested();


            using (var buffer = MemoryPool<byte>.Shared.Rent(DefaultBufferSize)) // Allocate the buffer.
            {
                var bytesRemaining = count;
                var bytesCopied = 0;

                while (bytesRemaining > 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    // Fill the buffer. Read the remaining bytes or a full buffer.
                    var readSlice = buffer.Memory.Slice(0, (int)Math.Min(bytesRemaining, buffer.Memory.Length));
                    var bytesRead = source.Read(readSlice.Span);

                    // Is the source done?
                    if (bytesRead == 0) return bytesCopied;

                    // Write the bytes to the target.
                    target.Write(readSlice.Slice(0, bytesRead).Span);

                    // Adjust the counters.
                    bytesCopied += bytesRead;
                    bytesRemaining -= bytesRead;
                }
                cancellationToken.ThrowIfCancellationRequested();
                return bytesCopied;
            }
        }



        /// <summary>
        /// Reads the source and calculates the CRC32 hash.
        /// </summary>
        /// <param name="source">The source to read from.</param>
        /// <param name="crc">The CRC32 instance to calculate the hash.</param>
        /// <param name="bufferSize">The size of the buffer to use for reading. Default is 81920 bytes.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static void ReadAndCalculateCRC(this Stream source, Crc32 crc, long byteCount, int? bufferSize = default)
        {
            bufferSize ??= DefaultBufferSize;

            using (var buffer = MemoryPool<byte>.Shared.Rent(bufferSize.Value)) // Allocate a buffer for the copy process.
            {
                while (byteCount > 0)
                {
                    var readSlice = buffer.Memory.Slice(0, (int)Math.Min(byteCount, buffer.Memory.Length));
                    var bytesRead = source.Read(readSlice.Span);
                    if (bytesRead == 0) return; // Source is complete.
                    crc.Append(buffer.Memory.Span.Slice(0, bytesRead));
                    byteCount -= bytesRead; //
                }
            }
        }




    }
}
