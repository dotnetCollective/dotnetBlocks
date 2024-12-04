using dotNetBlocks.ServiceBus.Consumers;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.ServiceBus.Handlers
{
    /// <summary>
    /// Implements consumer and routes to internal implementation class.
    /// </summary>
    /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <seealso cref="dotNetBlocks.ServiceBus.Consumers.ConsumerBase{TMessage}" />
    /// <remarks>
    ///     Implements consumer that routes calls to an implementation class using delegates.
    /// </remarks>
    internal class ConsumerFacade<TImplementation, TMessage> : ConsumerBase<TMessage> where TMessage : class where TImplementation : class
    {

        // delegates
        // 
        public HandleMessageAsync<TImplementation, TMessage>? HandleMessageAsync { get; set; } = default;


        public HandleConsumeAsync<TImplementation, TMessage>? HandleConsumeAsync { get; set; } = default;

        // Implementation properties

        private readonly TImplementation _implementation;
        protected TImplementation Implementation => _implementation;


        // Constructors
        public ConsumerFacade(TImplementation implementation,
            HandleMessageAsync<TImplementation, TMessage>? handleMessageAsync = default,
            HandleConsumeAsync<TImplementation, TMessage>? handleConsumeAsync = default,
            Lazy<ILoggerFactory>? loggerFactory = default)
            : base(loggerFactory) 
        {
            // Store the required implementation class.
            _implementation = implementation ?? throw new ArgumentNullException(nameof(implementation));

            // Setup the original routing if not provided.
            handleConsumeAsync ??= async (imp, ctx, cancel)=> await base.ConsumeInternalAsync(ctx, cancel);
            handleMessageAsync ??= async (imp, msg, cancel) => await base.ConsumeInternalAsync(msg, cancel);

            // Store the handlers and ensure they all have values.
            HandleConsumeAsync = handleConsumeAsync ?? throw new ArgumentNullException(nameof(handleConsumeAsync));
            HandleMessageAsync = handleMessageAsync ?? throw new ArgumentNullException(nameof(HandleMessageAsync));

        }

        /// <see cref="ConsumerBase{TMessage}.ConsumeInternalAsync(ConsumeContext{TMessage}, CancellationToken)"/>
        protected override async Task ConsumeInternalAsync(ConsumeContext<TMessage> context, CancellationToken cancellation = default)
        {
            if(HandleConsumeAsync == default) throw new ArgumentNullException(nameof(HandleConsumeAsync));
            await HandleConsumeAsync(Implementation, context, cancellation);
        }
        

        /// <see cref="ConsumerBase{TMessage}.ConsumeInternalAsync(TMessage, CancellationToken)"/>
        protected override async Task ConsumeInternalAsync(TMessage message, CancellationToken cancellation = default)
        {  if (HandleMessageAsync == default) throw new ArgumentNullException(nameof(HandleMessageAsync));
            await HandleMessageAsync(Implementation, message, cancellation);
        }

    }
}
