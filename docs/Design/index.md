<!---
Title: The Design
NavigationTitle: Design
BreadcrumbTitle: Design
ShowInNavigation: True
ShowInSidebar: True
NoSidebar: false
Level: 1
Order: 1
Excerpt: Describes the design and philosophy for the architecture.
--->

>
_"A Scalable Design approach" - 
Follow the patterns and deploy it any where, anyhow and it just works! Easy to upgrade and maintain without the code-splaining of micro-services."_

# Summary
.NetBlocks is an opinionated design pattern and complementary architecture


The objective of the .NetBlocks design is to create building blocks that form a pattern or recipe that is easy for any engineer to follow. This allows the engineer to focus on implementing the business logic to solve business problems in an architecture that "just works". The unique isolation of business logic in an actor pattern and the strong contract programming methodology decouples the business logic from the architecture, allowing unlimited flexibility for transport methodologies and enterprise hosting locations, with minimal additional engineering work."

# References
The design patterns in .NetBlocks rely heavily on existing an up and coming cloud and asynchronous processing technologies.

## [Business Logic](Business.Logic.html)
Engineers in large organizations and enterprises are focused on delivering value by solving the business problem, not playing with technology. The design patterns, practices and technologies they use should solve the technical problems without requiring deep thought so the focus is on the business problem.

All business code is contained in business logic classes that implement an IBusiness Logic interface with methods that accept a business logic operation or event as parameters. This encapsulation of the logic forces the logic to remain in the class and not bleed out into the hosting or transport classes e.g. api host, handler host or function host. The design means that business logic becomes host and transport agnostic, but remains isolated because of the "programming by strong contract design".

### [Operations](operations.html)
Operations and events are the implementations of a command parameter pattern, isolating the events and commands from the business logic and transport. Operations are self-validating increasing the probability of success prior to presentation for processing.

### [Business Entities](Business.Entities.html) 
Business entities represent the domain model base building blocks. A business entity approach forces engineers to model the business using a domain model approach and allows the framework to expand generically, but extends the functionality gated to business entities or business entity based intefaces.

### [References](references.html)

## More Content
<div>\@Html.Partial("_ChildPages",Document)</div>
