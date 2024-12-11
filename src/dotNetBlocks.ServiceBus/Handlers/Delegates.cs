using MassTransit;
using MassTransit.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.ServiceBus.Handlers
{
    // Consumer delegates

    public delegate Task HandleMessageAsync<in TImplementation, in TMessage>(TImplementation implementation, TMessage message, CancellationToken cancellation = default)
        where TMessage : class where TImplementation : class;
    public delegate Task HandleConsumeAsync<in TImplementation, in TMessage>(TImplementation implementation, ConsumeContext<TMessage> context, CancellationToken cancellation = default)
        where TMessage : class where TImplementation : class;

}
