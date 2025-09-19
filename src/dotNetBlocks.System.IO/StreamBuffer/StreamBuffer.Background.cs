using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.IO.Pipelines;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using static System.IO.StreamBuffer;

namespace System.IO
{


    /// <summary>
    /// Buffer class controlling flow of data between reader and writer stream, limiting the amount of data in the buffer.
    /// </summary>
    /// <remarks> This class uses the <see cref="_pipe"/> class as the underlying implementation and wraps helper methods for asynchronous functionality.</remarks>
    public partial class StreamBuffer : IAsyncDisposable, IDisposable
    {


        #region  Background processes

        // Buffer Functions
        /// <summary>
        /// Buffer function signature for Background work <see cref="Func{Stream, CancellationToken, Task}<"/>
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/>> to process</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/>></param>
        /// <returns></returns>
        public delegate Task BufferFunction(Stream stream, CancellationToken cancellationToken);

        // Buffer Actions
        /// <summary>
        /// Buffer action signature for Background work of void (Stream).
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/>> to process </param>
        public delegate void BufferAction(Stream stream);

        /// <summary>
        ///  Buffer action signature for Background work that is cancellable void(Stream, Cancellation Token);
        /// </summary>
        /// <param name="stream">The <see cref="Stream>"/> </param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> </param>
        public delegate void BufferActionWithCancel(Stream stream, CancellationToken cancellationToken);



        /// <summary>
        /// Parameter object for different background methods.
        /// </summary>
        public record BackgroundAction(BufferFunction? BufferFunction = default, BufferAction? BufferAction = default, BufferActionWithCancel? BufferActionWithCancel = default)
        {

            // async task(stream, cancel) 
            public static explicit operator BackgroundAction(BufferFunction function) => new BackgroundAction { BufferFunction = function };
            public static explicit operator BackgroundAction(Func<Stream, CancellationToken, Task> function) => new BackgroundAction { BufferFunction = function.AsBufferFunction() };

            // action(stream)
            public static explicit operator BackgroundAction(BufferAction action) => new BackgroundAction { BufferAction = action };
            public static explicit operator BackgroundAction(Action<Stream> action) => new BackgroundAction { BufferAction = action.AsBufferAction() };

            // action(stream, cancel).
            public static explicit operator BackgroundAction(BufferActionWithCancel action) => new BackgroundAction { BufferActionWithCancel = action };
            public static explicit operator BackgroundAction(Action<Stream, CancellationToken> action) => new BackgroundAction { BufferActionWithCancel = action.AsBufferAction() };

            /// <summary>
            /// Validates the parameters provided for the buffer action.
            /// </summary>
            /// <exception cref="System.ArgumentNullException">
            /// Function or action is required
            /// or
            /// Too many actions supplied.
            /// </exception>
            /// <exception cref="ArgumentNullException> all the actions are null."
            /// <exception cref="System.ArgumentException"> if there is an issue with the combination of aparameters. or async void method was provided. </exception>
            public void Validate()
            {
                if (BufferFunction is null && BufferAction is null && BufferActionWithCancel is null) throw new ArgumentNullException("Function or action is required");
                if (BufferFunction is not null && BufferAction is not null && BufferActionWithCancel is not null) throw new ArgumentNullException("Too many actions supplied.");
                if (BufferAction is not null && BufferAction!.Method.GetCustomAttribute<AsyncStateMachineAttribute>() != null && BufferAction.Method.ReturnType == typeof(void)) throw new ArgumentException($"{BufferAction} async void action causes thread management issues and not allowed.");
                if (BufferActionWithCancel is not null && BufferActionWithCancel!.Method.GetCustomAttribute<AsyncStateMachineAttribute>() != null && BufferActionWithCancel.Method.ReturnType == typeof(void)) throw new ArgumentException($"{BufferAction} async void action causes thread management issues and not allowed.");

            }
        }

        #region Background Process Control

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


        #endregion  Background Process Control


        #region Background Write

        protected readonly CancellationTokenSource CancelBackgroundWriteTokenSource;

        public Task? backgroundWriteTask { get; protected set; } = default;
        public bool IsBackgroundWriteTaskAssigned => backgroundWriteTask != default;

        [MemberNotNull(nameof(backgroundWriteTask))]
        public Task StartBackgroundWrite(Action<Stream, CancellationToken> action, CancellationToken cancellationToken = default)
            => StartBackgroundWrite((BackgroundAction)action, default, cancellationToken);


        [MemberNotNull(nameof(backgroundWriteTask))]
        public Task StartBackgroundWrite(Action<Stream> action, CancellationToken cancellationToken = default)
            => StartBackgroundWrite((BackgroundAction)action, default, cancellationToken);


        #region Background Write Functions

        public Task StartBackgroundWrite(Func<Stream, CancellationToken, Task> func, CancellationToken cancellationToken = default)
            => StartBackgroundWrite((BackgroundAction)func, default, cancellationToken);

        #endregion Background Write Functions

        // MAIN BACKGROUND WRITE METHOD

        /// <summary>
        /// Starts the writing process in a background task.
        /// </summary>
        /// <param name="writeActionAsync">The write action asynchronous.</param>
        /// <param name="writeAction">The write action.</param>
        /// <param name="options"><see cref="TaskCreationOptions"/> for finer control of the background task creation.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/></param>
        /// <returns><see cref="Task"/> proxy to the process and stored in the <see cref="StreamBuffer.backgroundWriteTask"/></returns>
        /// <exception cref="System.InvalidOperationException">Beackground write already started.</exception>
        /// <returns> <see cref="Task"/> <see cref="StreamBuffer.backgroundWriteTask"/> for the process </returns>
        /// <exception cref="System.InvalidOperationException">Beackground write already started.</exception>
        [MemberNotNull(nameof(backgroundWriteTask))]
        public Task StartBackgroundWrite(BackgroundAction action, TaskCreationOptions? options = default, CancellationToken cancellationToken = default)
        {

            if (!(backgroundWriteTask?.IsCompleted ?? true)) throw new InvalidOperationException("Beackground write already started.");

            // Start the action.
            return backgroundWriteTask = RunAction(
                actionParameters:
                    new RunActionParameters { Action = action, Stream = WriteStream, MasterCancel = CancelAllBackgroundTasksTokenSource.Token, TaskOptions = options }
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
        => await (backgroundWriteTask! ?? Task.CompletedTask).WaitAsync(timeout ?? DefaultMaxTimeToWaitForTaskComplete, cancelWait);


        #endregion Background Write


        #region Background Read
        protected readonly CancellationTokenSource CancelBackgroundReadTokenSource;

        public Task? backgroundReadTask { get; protected set; } = default;

        public bool IsBackgroundReadTaskAssigned => backgroundReadTask != default;


        [MemberNotNull(nameof(backgroundReadTask))]
        public Task StartBackgroundRead(Action<Stream, CancellationToken> action, CancellationToken cancellationToken = default)
            => StartBackgroundRead((BackgroundAction)action, default, cancellationToken);

        [MemberNotNull(nameof(backgroundReadTask))]
        public Task StartBackgroundRead(Action<Stream> action, CancellationToken cancellationToken = default)
            => StartBackgroundRead((BackgroundAction)action, default, cancellationToken);


        #region Background Read Functions

        [MemberNotNull(nameof(backgroundReadTask))]
        public Task StartBackgroundRead(Func<Stream, CancellationToken, Task> func, CancellationToken cancellationToken = default)
            => StartBackgroundRead((BackgroundAction)func, default, cancellationToken);

        #endregion Background Read Functions

        // MAIN BACKGROUND READ METHOD

        // Main background read method.

        [MemberNotNull(nameof(backgroundReadTask))]
        public Task StartBackgroundRead(BackgroundAction action, TaskCreationOptions? options = default, CancellationToken cancellationToken = default)
        {

            if (!(backgroundReadTask?.IsCompleted ?? true)) throw new InvalidOperationException("Beackground reader already started.");

            //Run the read action
            return backgroundReadTask = RunAction(new RunActionParameters
            {
                Action = action,
                MasterCancel = CancelAllBackgroundTasksTokenSource.Token,
                Stream = ReadStream,
                TaskOptions = options
            }, cancellationToken);
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

        #region Run Action

        public record RunActionParameters(TaskCreationOptions? TaskOptions = default)
        {
            public required BackgroundAction Action { get; init; }
            public required Stream Stream { get; init; }
            public required CancellationToken MasterCancel { get; init; }
            public void Validate() => Action.Validate();
        }

        /// <summary>
        ///         /// Internal background task factory method.
        /// </summary>
        /// <param name="actionParameters"><see cref="RunActionParameters"/>The Parameters controlling the background task creation.</param>
        /// <param name="cancellationTokens">The cancellation tokens.</param>
        /// <returns><see cref="Task"/> representing the action running in the background.</returns>
        protected static Task RunAction(RunActionParameters actionParameters, params CancellationToken[] cancellationTokens)
        {
            // Validation ensures we have only one action provided.

            ArgumentNullException.ThrowIfNull(actionParameters);
            actionParameters.Validate();


            // running the action requires creating a linked cancellation token using a apture and wrapping the method. We do this for sync and async cases.
            // Notes: I forced everything into an async action, but had undesired consequences, so now we deal with each one independently instead of shared code.
            // How we launch this process depends on if its an action or async action and if there are task creation options used or not.

            // Build and store the wrapped actions.
            Action? runAction = default;
            Func<Task>? runActionAsync = default;

            if (actionParameters.Action.BufferAction != default)
            {
                // Simple Synchronous non cancelable buffer action.
                runAction = () => actionParameters.Action.BufferAction(actionParameters.Stream);
            }
            else if (actionParameters.Action.BufferActionWithCancel != default)
            {
                // Synchronous cancelable action, so wrap with joined tokens
                runAction = () =>
                {
                    using (CancellationTokenSource ts = actionParameters.MasterCancel.CreateLinkedTokenSource(cancellationTokens)) // Joined captured token source for the task.
                        actionParameters.Action.BufferActionWithCancel(actionParameters.Stream, ts.Token); // Call the sync method with a cancelation token.
                };
            }
            else // We are running an asynchronous action, so wrap with joined tokens
            {
                runActionAsync = async () =>
                {
                    using (CancellationTokenSource ts = actionParameters.MasterCancel.CreateLinkedTokenSource(cancellationTokens)) // Joined captured token source for the task.
                        await actionParameters.Action.BufferFunction!(actionParameters.Stream, ts.Token); // await the async method call.
                };
            };

            // For more information on launching tasks, See this reference on task factory for async methods. https://devblogs.microsoft.com/pfxteam/task-run-vs-task-factory-startnew/

            // Run the action.
            return actionParameters.TaskOptions switch
            {
                // No Special Options
                // sync no special options.
                null when runAction is not null => Task.Run(runAction, actionParameters.MasterCancel.CreateLinkedToken(cancellationTokens)), // sync action with no special options, default factory.
                // async no special options.
                null when runAction is null => Task.Run(runActionAsync!, actionParameters.MasterCancel.CreateLinkedToken(cancellationTokens)), // Async action with no special options., default factory.
                // Special Options
                // sync with special options, uses task factory class. see ->  https://devblogs.microsoft.com/pfxteam/task-run-vs-task-factory-startnew/
                null when runAction is not null => // sync action with special options
                    Task.Factory.StartNew(runAction!, actionParameters.MasterCancel.CreateLinkedToken(cancellationTokens), actionParameters.TaskOptions ?? TaskCreationOptions.LongRunning, TaskScheduler.Default),

                null when runActionAsync is not null => // async action with special options, unwrapped for monitoring.
                    Task.Factory.StartNew(runActionAsync, actionParameters.MasterCancel.CreateLinkedToken(cancellationTokens), actionParameters.TaskOptions ?? TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap(),
                _ => throw new InvalidOperationException("Illegal parameter combination passed validation process. "),
            };

        }



        #endregion Helper methods

        #endregion Run Action
    }

    internal static class BufferActionExtensions
    {
        public static BufferFunction AsBufferFunction(this Func<Stream, CancellationToken, Task> func)
            => async Task (Stream stream, CancellationToken cancellationToken) => await func(stream, cancellationToken);
        public static BufferAction AsBufferAction(this Action<Stream> action) => (Stream stream) => action(stream);
        public static BufferActionWithCancel AsBufferAction(this Action<Stream, CancellationToken> action) => (Stream stream, CancellationToken cancellation) => action(stream, cancellation);
    }


}

#endregion
