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
using static System.Collections.Specialized.BitVector32;

namespace System.IO
{


    /// <summary>
    /// Buffer class controlling flow of data between reader and writer stream, limiting the amount of data in the buffer.
    /// </summary>
    /// <remarks> This class uses the <see cref="_pipe"/> class as the underlying implementation and wraps helper methods for asynchronous functionality.</remarks>
    public partial class StreamBuffer: IAsyncDisposable, IDisposable
    {
        // ActionDelegateDefinitions.
        public delegate void bufferAction(Stream stream, CancellationToken cancellationToken);
        public delegate Task bufferActionAsync(Stream stream, CancellationToken cancellationToken);




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
        public Task StartBackgroundWriteAction(bufferAction writeAction,CancellationToken cancellationToken = default)
            => StartBackgroundWrite(writeAction:writeAction, options:default, cancellationToken: cancellationToken);

        [MemberNotNull(nameof(backgroundWriteTask))]
        public Task StartBackgroundWriteAsync(bufferActionAsync writeActionAsync ,CancellationToken cancellationToken = default)
            => StartBackgroundWrite(writeActionAsync:writeActionAsync, options: default, cancellationToken:cancellationToken);

        [MemberNotNull(nameof(backgroundWriteTask))]
        public Task StartBackgroundWrite(bufferActionAsync? writeActionAsync = default, bufferAction? writeAction = default, TaskCreationOptions? options = default, CancellationToken cancellationToken = default)
        {

            if (!(backgroundWriteTask?.IsCompleted ?? true)) throw new InvalidOperationException("Beackground write already started.");

            // Start the action.
            return backgroundWriteTask = RunAction(
                new RunActionParameters(WriteStream, CancelAllBackgroundTasksTokenSource.Token) 
                { AsyncAction = writeActionAsync
                , Action = writeAction 
                }
            , cancellationToken);
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


        public Task StartBackgroundReadAction(bufferAction readAction, CancellationToken cancellationToken = default) => StartBackgroundReadAction(readAction:readAction, cancellationToken: cancellationToken);

        public Task StartBackgroundReadAsync(bufferActionAsync readActionAsync, CancellationToken cancellationToken = default) => StartBackgroundReadAsync(readActionAsync:readActionAsync, cancellationToken:cancellationToken);

            [MemberNotNull(nameof(backgroundReadTask))]
        public Task StartBackgroundRead(bufferActionAsync? readActionAsync = default, bufferAction? readAction = default, TaskCreationOptions? options = default, CancellationToken cancellationToken = default)
        {

            if (!(backgroundReadTask?.IsCompleted ?? true)) throw new InvalidOperationException("Beackground reader already started.");

            //Run the read action
            return backgroundReadTask = RunAction(new RunActionParameters(ReadStream, CancelAllBackgroundTasksTokenSource.Token)
            {
                AsyncAction = readActionAsync,
                Action = readAction,
            }, CancelBackgroundReadTokenSource.Token, cancellationToken);
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

        public record RunActionParameters(Stream Stream, CancellationToken MasterCancel, TaskCreationOptions? TaskOptions = default)
        {
            public bufferActionAsync? AsyncAction = default;
            public bufferAction? Action = default;

            public void Validate()
            {
                // Validate parameters
                // We must have an action.
                if (AsyncAction == default && Action == default) throw new ArgumentException(message: $"{nameof(Action)} or {nameof(AsyncAction)} is required");
                //Did we get a unique action provided?
                if (Action != default && AsyncAction != default) throw new ArgumentException(message: $"{nameof(Action)} and {nameof(AsyncAction)} supplied.");
                {

                    if (AsyncAction == default && Action == default) throw new ArgumentException(message: $"{nameof(Action)} or {nameof(AsyncAction)} is required");
                    //Did we get a unique action provided?
                    //if (Action != default && AsyncAction != default) throw new ArgumentException(message: $"{nameof(Action)} and {nameof(AsyncAction)} supplied.");
                    //public const TaskCreationOptions DefaultTaskOptions = TaskCreationOptions.LongRunning;



                };
            }
        }

        /// <summary>
        /// Runs the synchronouse asyncAction asynchronous.
        /// </summary>
        /// <param name="stream"><see cref="Stream" /> to process using the action.</param>
        /// <param name="masterCancel">The master <see cref="CancellationToken" /> Master cancellation token that will cancel the entire process.</param>
        /// <param name="asyncAction"async <see cref="bufferActionAsync"/> delegate to process</param>
        /// <param name="action"><see cref="bufferAction"/> delegate to process</param>
        /// <param name="TaskOptions"><see cref=""/>></param>
        /// <param name="cancellationTokens">The cancellation tokens.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        protected static Task RunAction(RunActionParameters actionParameters, params CancellationToken[] cancellationTokens)
        {

            actionParameters.Validate();


            var asyncAction = actionParameters.AsyncAction!;

            // prepare an action as an async function.
            //asyncAction ??= async (stream, cancel) => { action!(stream, cancel); await Task.CompletedTask; };
                asyncAction ??= async (stream, cancel) => { actionParameters.Action!(stream, cancel); await Task.CompletedTask; };

            // Wrap the action and add a joined token.
            var runAction = async () =>
                {
                    using (CancellationTokenSource ts = actionParameters.MasterCancel.CreateLinkedTokenSource(cancellationTokens)) // Joined token source for the task.
                        await asyncAction(actionParameters.Stream, ts.Token);
                };

            // Run the action.
            if (actionParameters.TaskOptions == default) // There is no special task creation process required.
                return Task.Run(runAction,  actionParameters.MasterCancel.CreateLinkedToken(cancellationTokens));


            // See this reference on task factory for async methods. https://devblogs.microsoft.com/pfxteam/task-run-vs-task-factory-startnew/
            return Task.Factory.StartNew(runAction, actionParameters.MasterCancel.CreateLinkedToken(cancellationTokens), actionParameters.TaskOptions?? TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
        }

        #endregion


    }
}
