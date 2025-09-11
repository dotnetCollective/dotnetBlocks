<!---
Title: Business.Shared
BreadcrumbTitle: Business.Shared
ShowInNavigation: true
ShowInSidebar: true
NoSidebar: false
Description: Shared objects defining all business components in the architecture and the basis for creating domain models.
--->
# Overview


library contains all the contracts and models used or implemented by business classes libraries supporting the architecture.
The library is referenced by any system following the business logic pattern.
It forms the basis for the logic, operation business entity contracts.

Entities are business object models that can be serialized, transmitted or used in a persistance strategy (typically EF or blob storage);
Business entities are similar to data transfer objects in older n-tier paradigms.

For a detailed explanation about how entities work and their design and philosophy, see [Design and usage information](/Design/Business.Entities.html) 
That documentation explains the base models, id and globalID keying strategies and other usage information.


# Abstract base and Generic derived class problem
This article explains the design pattern for derived classes.
https://www.devgem.io/posts/resolving-generic-type-conversion-issues-in-c-inheritance


### Getting started
[ NuGet : dotnetBlocks.ServiceBus.Shared](https://www.nuget.org/packages/DotNetBlocks.ServiceBus.Shared)

[Licensing and other information](/)

### Functionality

# # Examples

```c#

```
