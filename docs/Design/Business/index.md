<!---
Title: Design - Business
NavigationTitle: Design - Business
BreadcrumbTitle: Design - Business
ShowInNavigation: True
ShowInSidebar: True
NoSidebar: false
Level: 1
Order: 0
Excerpt: Describes what the business area of the design is.
--->

>

# Summary

In this design, the business classes are the base uses to implement all the business logic and domain models.

Using these classes as the base allows the rest of the system to extend and wrap these classes to easily solve business problems.


## [Business Logic](Business.Logic.md)

Business logic classes support, contain and isolate business logic, preventing bleeding into the rest of the design.


### [Operations](../Operations.md)
Operations and events are the implementations of a command parameter pattern, isolating the events and commands from the business logic and transport. Operations are self-validating increasing the probability of success prior to presentation for processing.

### [Business Entities](Business.Entities.md) 
Business entities represent the domain model base building blocks. A business entity approach forces engineers to model the business using a domain model approach and allows the framework to expand generically, but extends the functionality gated to business entities or business entity based intefaces.

### [References](../References.md)

