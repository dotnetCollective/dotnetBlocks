Title: dotNetBlocks.System.Threading
---
# dotNetBlocks.System.Threading

## problem description
to have multiple tasks wait on the same token requires creation of a shared token source. classes and methods may not have access to that shared token source to create new linked tokens. These extensions and wait methods give access to token sources given only the token.

New threading functionality for advanced waiting on token sources of existing cancellation tokens.

* Task extensions for new token wait types that don't require a merged token source.