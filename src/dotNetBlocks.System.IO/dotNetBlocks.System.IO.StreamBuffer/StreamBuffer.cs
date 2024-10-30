using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace System.IO
{


    /// <summary>
    /// Buffer class controlling flow of data between reader and writer stream, limiting the amount of data in the buffer.
    /// </summary>
    /// <remarks> This class uses the <see cref="_pipe"/> class as the underlying implementation and wraps helper methods for asynchronous functionality.</remarks>
    public class StreamBuffer: IAsyncDisposable, IDisposable
    {
        // ActionDelegateDefinitions.
        public delegate void bufferAction(Stream stream, CancellationToken cancellationToken);
        public delegate Task bufferActionAsync(Stream stream, CancellationToken cancellationToken);


        #region Pipe
        private const long DefaultBufferSize = 65536; // 64k
        private Pipe? _pipe;
        // Streams
        public Stream ReadStream => _disposed ? throw new ObjectDisposedException(nameof(StreamBuffer)) : _pipe!.Reader.AsStream();
        public Stream WriteStream => _disposed ? throw new ObjectDisposedException(nameof(StreamBuffer)) : _pipe!.Writer.AsStream();
        #endregion


        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamBuffer"/> class.
        /// </summary>
        public StreamBuffer():this(default)
        {


        }

        /// <summary>
        /// Default buffer size is 64k
        /// </summary>
        public StreamBuffer(long? bufferSize = DefaultBufferSize)
        {

            // Initialize the master background token.
            CancelAllBackgroundTasksToken = new CancellationTokenSource();

            // Initialize Background Read Write tokens and join to the Background Master token;
            CancelBackgroundReadToken = CancelAllBackgroundTasksToken.Token.CreateLinkedTokenSource();
            CancelBackgroundWriteToken = CancelAllBackgroundTasksToken.Token.CreateLinkedTokenSource();

            // Configure the options based on the buffer.
            long pauseWriterThreshold = bufferSize ?? DefaultBufferSize;
            long resumeWriterThreshold = pauseWriterThreshold / 2;

            // Default resume writing at half-buffer size.

            // Create the pipe for this buffer;
            PipeOptions options = new PipeOptions(pauseWriterThreshold: pauseWriterThreshold, resumeWriterThreshold: resumeWriterThreshold);
            _pipe = new Pipe(options);
        }



        #region  Background processes

        public readonly TimeSpan DefaultMaxTimeToWaitForTaskComplete = TimeSpan.FromMilliseconds(100);

        protected readonly CancellationTokenSource CancelAllBackgroundTasksToken;

        public async Task CancelBackgroundAsync(TimeSpan? waitTime = default, CancellationToken cancellationToken = default)
        {
            // Already done?
            if ((backgroundWriteTask?.IsCompleted ?? true) && (backgroundWriteTask?.IsCompleted ?? true))
                return;
            // Request cancelation of all work.
            if (!CancelAllBackgroundTasksToken.IsCancellationRequested)
                await CancelAllBackgroundTasksToken.CancelAsync();


            // Wait for the background tasks to exit gracefully.
            await WaitForBackgroundAsync(waitTime, cancellationToken);
        }

        /// <summary>
        /// Waits aynchronously for the background tasks to complete.
        /// </summary>
        /// <param name="waitTime"><see cref="TimeSpan"/> to wait for completion.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to cancel the wait.</param>
        public async Task WaitForBackgroundAsync(TimeSpan? waitTime = default, CancellationToken cancellationToken = default)
        {
            var tasks = new Task[] { backgroundReadTask ?? Task.CompletedTask, backgroundWriteTask ?? Task.CompletedTask };
            await tasks.WaitAllAsync(waitTime ?? DefaultMaxTimeToWaitForTaskComplete, cancellationToken);
        }


        #endregion


        #region Background Write

        protected readonly CancellationTokenSource CancelBackgroundWriteToken;

        public Task? backgroundWriteTask { get; protected set; } = default;
        public bool IsBackgroundWriteTaskAssigned => backgroundWriteTask != default;

        public Task StartBackgroundWrite(bufferAction? writeAction = default, bufferActionAsync? writeActionAsync = default, CancellationToken cancellationToken = default)
        {
            if (!(backgroundWriteTask?.IsCompleted??true)) throw new InvalidOperationException("Beackground write already started.");

            // We need at least one action.
            if (writeActionAsync is null && writeAction is null) ArgumentNullException.ThrowIfNull(writeAction);

            // Start the async action.
            if (writeActionAsync is not null)
            {
                return backgroundWriteTask = RunAsyncAction(writeActionAsync, WriteStream, CancelBackgroundWriteToken.Token);
            }
            // Start the sync action

            // can't supply two actions.
            if (writeAction is null) throw new ArgumentException(nameof(writeAction));
            return backgroundWriteTask = RunActionAsync(writeAction!, WriteStream, cancellationToken, CancelBackgroundWriteToken.Token);
        }

        public async Task CancelBackgroundWriteAsync(TimeSpan? timeout = default, CancellationToken cancelWait = default)
        {
            // Already done?
            if (backgroundWriteTask?.IsCompleted ?? true) return;

            // Request cancelation of the task.
            if (!CancelBackgroundWriteToken.IsCancellationRequested)
                await CancelBackgroundWriteToken.CancelAsync();

            await WaitForBackgroundWriteAsync(timeout, cancelWait);

        }
        public async Task WaitForBackgroundWriteAsync(TimeSpan? timeout = default, CancellationToken cancelWait = default)
        => await (backgroundWriteTask??Task.CompletedTask).WaitAsync(timeout ?? DefaultMaxTimeToWaitForTaskComplete, cancelWait);



        #endregion

        #region Background Read
        protected readonly CancellationTokenSource CancelBackgroundReadToken;

        public Task? backgroundReadTask { get; protected set; } = default;

        public bool IsBackgroundReadTaskAssigned => backgroundReadTask != default;


        public Task StartBackgroundRead(bufferAction? readAction = default, bufferActionAsync? readActionAsync = default, CancellationToken cancellationToken = default)
        {
            if (!(backgroundReadTask?.IsCompleted??true)) throw new InvalidOperationException("Beackground reader already started.");

            // We need at least one action.
            if (readActionAsync is null && readAction is null) ArgumentNullException.ThrowIfNull(readAction);

            //async action supplied.
            if (readActionAsync is not null)
            {
                return backgroundReadTask = RunAsyncAction(readActionAsync, ReadStream, cancellationToken, CancelBackgroundReadToken.Token);
            }

            ArgumentNullException.ThrowIfNull(readAction);
            return backgroundReadTask = RunActionAsync(readAction!, ReadStream, cancellationToken, CancelBackgroundReadToken.Token);
        }

        public async Task CancelBackgroundReadAsync(TimeSpan? timeout = default, CancellationToken cancelWait = default)
        {
            // Already done?
            if (backgroundReadTask?.IsCompleted ?? true) return;

            // Request cancelation of the task.
            if (!CancelBackgroundReadToken.IsCancellationRequested)
                await CancelBackgroundReadToken.CancelAsync();

            await WaitForBackgroundReadAsync(timeout, cancelWait).ConfigureAwait(false);

        }
        /// <summary>
        /// asynchronously wait for background read to complete.
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/>> to wait to complete.</param>
        /// <param name="cancelWait"><see cref="CancellationToken"/> to cancel the wait.</param>
        public async Task WaitForBackgroundReadAsync(TimeSpan? timeout = default, CancellationToken cancelWait = default)
        => await (backgroundReadTask ?? Task.CompletedTask).WaitAsync(timeout ?? DefaultMaxTimeToWaitForTaskComplete, cancelWait);


        #endregion

        #region helper methods

        /// <summary>
        /// Runs the synchronouse action asynchronous.
        /// </summary>
        /// <param name="action">action delegate to execute</param>
        /// <param name="stream"><see cref="Stream"/> process by the action.</param>
        /// <param name="masterCancel">The master <see cref="CancellationToken"/></param>
        /// <param name="cancellationToken">action <see cref="CancellationToken"/></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        private static Task RunActionAsync(bufferAction action, Stream stream, CancellationToken masterCancel, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(action); ArgumentNullException.ThrowIfNull(stream);

            // Are we cancelled?
            if (cancellationToken.IsCancellationRequested || masterCancel.IsCancellationRequested) return Task.CompletedTask;

            // Start the task
            var runAction = () =>
                {
                    using (CancellationTokenSource ts = masterCancel.CreateLinkedTokenSource(cancellationToken)) // Joined token source for the task.
                        action(stream, ts.Token);
                };

            // Run the action asynchronously.
            return Task.Run(runAction, masterCancel.CreateLinkedToken(cancellationToken));
        }

        /// <summary>
        /// Runs the a synchronouse action.
        /// </summary>
        /// <param name="action">action delegate to execute</param>
        /// <param name="stream"><see cref="Stream"/> process by the action.</param>
        /// <param name="masterCancel">The master <see cref="CancellationToken"/></param>
        /// <param name="cancellationToken">action <see cref="CancellationToken"/></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        private async static Task RunAsyncAction(bufferActionAsync action, Stream stream, CancellationToken masterCancel = default, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(action); ArgumentNullException.ThrowIfNull(stream);

            // Are we canceled already?
            if (cancellationToken.IsCancellationRequested || masterCancel.IsCancellationRequested) { await Task.CompletedTask; return; }

            // Run the action.
            using (var ts = masterCancel.CreateLinkedTokenSource(cancellationToken)) // Disposed after action completes.
                await action(stream, ts.Token);
        }
        #endregion

        #region IDisposable

        public async ValueTask DisposeAsync()
        {
            // Perform async cleanup.
            await DisposeAsyncCore().ConfigureAwait(false);

            // Dispose of unmanaged resources.
            Dispose(false);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Over-ridable implemention for async dispose by implementors.
        /// </summary>
        /// <returns></returns>
        protected virtual async ValueTask DisposeAsyncCore()
        {
            // Cancal and wait for all background tasks.
            await CancelBackgroundAsync();
                await ValueTask.CompletedTask;
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)

                    // We Assume that DisposeAsync is already called.
                    // We assume that background threads
                    // // had a chance to finish.

                    // Clean up the pipe
                    // Dispose the pipe streams to removing blocking.
                    ReadStream.Dispose();
                    WriteStream.Dispose();

                    // Release a reference to the pipe
                    _pipe = null;

                    // Dispose all token sources.
                    CancelAllBackgroundTasksToken.Dispose();
                    CancelBackgroundReadToken.Dispose();
                    CancelBackgroundWriteToken.Dispose();
                    // Release Tasks.
                    backgroundReadTask?.Dispose();
                    backgroundReadTask  = null;

                    backgroundWriteTask?.Dispose();
                    backgroundWriteTask = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposed = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~StreamBuffer()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion


    }
}
