using dotNetBlocks.ServiceBus.Handlers;
using MassTransit;
using MassTransit.Futures.Contracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.ServiceBus.Consumers
{
    /// <summary>
    /// Base class for all service bus implementations.
    /// </summary>
    /// <remarks>
    /// Wires in base functionality like logging and other consumer message handling
    /// </remarks>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <seealso cref="MassTransit.IConsumer{TMessage}" />
    internal abstract class ConsumerBase<TMessage> : IConsumer<TMessage>
        where TMessage : class
    {
        // Logging
        private ILogger? _logger = default;
        protected ILogger Logger => _logger ??= LoggerFactory.Value.CreateLogger(this.GetType());

        private Lazy<ILoggerFactory> _loggerFactory = new Lazy<ILoggerFactory>(new NullLoggerFactory()); // Initialize with a null factory.
        protected Lazy<ILoggerFactory> LoggerFactory 
        { get => _loggerFactory; 
            set { 
                _loggerFactory = value; // Store the new value
                _logger = default; // Reset the logger.
            } 
        }


        // constructors

        public ConsumerBase(Lazy<ILoggerFactory>? loggerFactory = default)
        {
            // Assign logging or use existing default value;
            loggerFactory = loggerFactory ?? LoggerFactory;

        }


        /// <see cref="IConsumer{TMessage}.Consume(ConsumeContext{TMessage})"/>
        async Task IConsumer<TMessage>.Consume(ConsumeContext<TMessage> context)
            => await ConsumeInternalAsync(context, context.CancellationToken);

        /// <summary>
        /// Internal implementation to consume the message with context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="cancellation">The cancellation.</param>
        protected virtual async Task ConsumeInternalAsync(ConsumeContext<TMessage> context, CancellationToken cancellation = default)
        {
            if (cancellation.IsCancellationRequested) return; 
            await ConsumeInternalAsync(context.Message, cancellation);
        }

        /// <summary>
        /// Internal implementation to consumer the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="cancellation">The cancellation.</param>
        protected virtual async Task ConsumeInternalAsync(TMessage message, CancellationToken cancellation = default) => await Task.CompletedTask;

    }
}
