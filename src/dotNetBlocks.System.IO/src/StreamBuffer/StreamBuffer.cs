using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO.Pipelines;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
        private const double DefaultResumePercentBufferUsed = 0.75;
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
        public StreamBuffer(PipeOptions pipeOptions)
        {
            _pipe = new Pipe(pipeOptions);

            // Initialize the master background token.
            CancelAllBackgroundTasksTokenSource = new CancellationTokenSource();

            // Initialize Background Read Write tokens and join to the Background Master token;
            CancelBackgroundReadTokenSource = CancelAllBackgroundTasksTokenSource.Token.CreateLinkedTokenSource();
            CancelBackgroundWriteTokenSource = CancelAllBackgroundTasksTokenSource.Token.CreateLinkedTokenSource();

        }

        /// <remarks> The buffer stops writing and connot completely at EXACTLY buffer size bytes.
        /// </remarks>
        /// </summary>
        /// <param name="bufferSize">Size of the buffer.
        ///         /// Default buffer size is 64k

        /// </param>
        public StreamBuffer(long? bufferSize = DefaultBufferSize,  double? resumePercentBufferUsed = DefaultResumePercentBufferUsed) 
            : this( CalculatePipeOptions(bufferSize, resumePercentBufferUsed))
        {

        }
        // ToDo: Rework this with a buffer options class.
        private static PipeOptions CalculatePipeOptions(long? bufferSize = DefaultBufferSize, double? resumePercentBufferUsed = DefaultResumePercentBufferUsed)
        {
            // Configure the options based on the buffer.
            bufferSize ??= DefaultBufferSize;
            resumePercentBufferUsed ??= DefaultResumePercentBufferUsed;

            long pauseWriterThreshold = bufferSize.Value;
            long resumeWriteThreshold = (long)(pauseWriterThreshold * resumePercentBufferUsed);

            // Create the pipe for this buffer;
            return new PipeOptions(pauseWriterThreshold: pauseWriterThreshold, resumeWriterThreshold: resumeWriteThreshold);

        }



        #region  Background processes

        public readonly TimeSpan DefaultMaxTimeToWaitForTaskComplete = TimeSpan.FromMilliseconds(100);

        protected readonly CancellationTokenSource CancelAllBackgroundTasksTokenSource;

        public async Task CancelBackgroundAsync(TimeSpan? waitTime = default, CancellationToken cancellationToken = default)
        {
            // Already done?
            if ((backgroundWriteTask?.IsCompleted ?? true) && (backgroundWriteTask?.IsCompleted ?? true))
                return;
            // Request cancelation of all work.
            if (!CancelAllBackgroundTasksTokenSource.IsCancellationRequested)
                await CancelAllBackgroundTasksTokenSource.CancelAsync();


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

        protected readonly CancellationTokenSource CancelBackgroundWriteTokenSource;

        public Task? backgroundWriteTask { get; protected set; } = default;
        public bool IsBackgroundWriteTaskAssigned => backgroundWriteTask != default;


        [MemberNotNull(nameof(backgroundWriteTask))]
        public Task StartBackgroundWrite(bufferActionAsync writeActionAsync, CancellationToken cancellationToken = default)
            => StartBackgroundWrite(writeActionAsync, writeAction:default, cancellationToken);

        [MemberNotNull(nameof(backgroundWriteTask))]
        public Task StartBackgroundWrite(bufferAction writeAction, CancellationToken cancellationToken = default)
            => StartBackgroundWrite(writeActionAsync:default, writeAction, cancellationToken);

        [MemberNotNull(nameof(backgroundWriteTask))]
        public Task StartBackgroundWrite(bufferActionAsync? writeActionAsync = default, bufferAction? writeAction = default, CancellationToken cancellationToken = default)
        {

            if (!(backgroundWriteTask?.IsCompleted ?? true)) throw new InvalidOperationException("Beackground write already started.");

            // We need at least one asyncAction but not both.
            if (writeActionAsync == default && writeAction == default) ArgumentNullException.ThrowIfNull(writeAction);
            if (writeActionAsync != default && writeAction != default) throw new ArgumentException("cannot provide sync and async action");

            // Start the action.
            backgroundWriteTask = RunAction(WriteStream, CancelAllBackgroundTasksTokenSource.Token, writeActionAsync, writeAction, CancelBackgroundWriteTokenSource.Token, cancellationToken);
            return backgroundWriteTask;
        }

        public async Task CancelBackgroundWriteAsync(TimeSpan? timeout = default, CancellationToken cancelWait = default)
        {
            // Already done?
            if (backgroundWriteTask?.IsCompleted ?? true) return;

            // Request cancelation of the task.
            if (!CancelBackgroundWriteTokenSource.IsCancellationRequested)
                await CancelBackgroundWriteTokenSource.CancelAsync();

            await WaitForBackgroundWriteAsync(timeout, cancelWait);

        }
        public async Task WaitForBackgroundWriteAsync(TimeSpan? timeout = default, CancellationToken cancelWait = default)
        => await (backgroundWriteTask!??Task.CompletedTask).WaitAsync(timeout ?? DefaultMaxTimeToWaitForTaskComplete, cancelWait);



        #endregion Background processing

        #region Background Read
        protected readonly CancellationTokenSource CancelBackgroundReadTokenSource;

        public Task? backgroundReadTask { get; protected set; } = default;

        public bool IsBackgroundReadTaskAssigned => backgroundReadTask != default;


        [MemberNotNull(nameof(backgroundReadTask))]
        public Task StartBackgroundRead(bufferActionAsync? readActionAsync = default, bufferAction ? readAction = default, CancellationToken cancellationToken = default)
        {

            if (!(backgroundReadTask?.IsCompleted??true)) throw new InvalidOperationException("Beackground reader already started.");

            // We need at least one asyncAction but not both.
            if (readActionAsync != default && readAction != default) throw new ArgumentException("Provide a sync of async action, not both.");
            if (readActionAsync is null && readAction is null) ArgumentNullException.ThrowIfNull(readAction);

            //Run the aaction
            return backgroundReadTask = RunAction(ReadStream, CancelAllBackgroundTasksTokenSource.Token,  readActionAsync, readAction, cancellationToken, CancelBackgroundReadTokenSource.Token, cancellationToken);
        }

        public async Task CancelBackgroundReadAsync(TimeSpan? timeout = default, CancellationToken cancelWait = default)
        {
            // Already done?
            if (backgroundReadTask?.IsCompleted ?? true) return;

            // Request cancelation of the task.
            if (!CancelBackgroundReadTokenSource.IsCancellationRequested)
                await CancelBackgroundReadTokenSource.CancelAsync();

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
        /// Runs the synchronouse asyncAction asynchronous.
        /// </summary>
        /// <param name="action">asyncAction delegate to execute</param>
        /// <param name="stream"><see cref="Stream"/> process by the asyncAction.</param>
        /// <param name="masterCancel">The master <see cref="CancellationToken"/></param>
        /// <param name="cancellationToken">asyncAction <see cref="CancellationToken"/></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        private static Task RunAction( Stream stream, CancellationToken masterCancel, bufferActionAsync? asyncAction = default, bufferAction? action = default, params CancellationToken[] cancellationTokens)
        {
            // validate parameters.

            // We must have an action.
            if (asyncAction == default && action == default) 
            {
                throw new ArgumentException( message: $"{ nameof(action)} or { nameof(asyncAction)} is required");
            }

            //Did we get a unique action provided?
            if (action != default && asyncAction != default) throw new ArgumentException(message: $"{nameof(action)} and {nameof(asyncAction)} supplied.");


            // Are we cancelled?
            if (cancellationTokens.Any(t => t.IsCancellationRequested) || masterCancel.IsCancellationRequested) return Task.CompletedTask;

            // prepare an action as an async function.
            //asyncAction ??= async (stream, cancel) => { action!(stream, cancel); await Task.CompletedTask; };
            if(asyncAction == default)
                asyncAction = async (stream, cancel) => { action!(stream, cancel); await Task.CompletedTask; };

            // Wrap the action and add a joined token.
            var runAction = async () =>
                {
                    using (CancellationTokenSource ts = masterCancel.CreateLinkedTokenSource(cancellationTokens)) // Joined token source for the task.
                        await asyncAction(stream, ts.Token);
                };

            // Run the action.
            return Task.Run(runAction, masterCancel.CreateLinkedToken(cancellationTokens));
            //return Task.Factory.StartNew(runAction, TaskCreationOptions.LongRunning).Unwrap();
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
                    CancelAllBackgroundTasksTokenSource.Dispose();
                    CancelBackgroundReadTokenSource.Dispose();
                    CancelBackgroundWriteTokenSource.Dispose();
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
