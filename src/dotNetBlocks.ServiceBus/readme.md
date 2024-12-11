# Messages, Envelopes, the Design approach and architectural decisions.

# Message design approach.

Mass transit messages a class and MT uses an envelope design pattern, wrapping every transmitted message in an envelope.
In our service bus implementation, we also use an envelope design pattern, but we will hide it from the end consumer using extensions and generic types.
. When a message is transmitted in mass transit, it is wrapped in an envelope - a standard pattern for service bus and messaging technologies.


# Handlers - handlers function as consumers in the MT world.

We are implementing the base handler so it case be used as is in straight inheritance or can be injected with a class and a delegate.