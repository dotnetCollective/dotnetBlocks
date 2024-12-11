using dotNetBlocks.Business.Shared;
using System.ComponentModel.DataAnnotations;

namespace dotNetBlocks.ServiceBus.Shared
{

    /// <summary>
    /// Wrapper for all activities transported on the service bus.
    /// </summary>
    /// <seealso cref="Message{TActivity}" />
    public record  Message
    {

    }

    public record Message<TActivity>
        where TActivity : BusinessActivity
    {

        public Message(TActivity? activity = default) 
            => activity = activity ?? throw new ArgumentNullException(nameof(activity));

        public TActivity? Activity { get; set; } = default;
    }
}
