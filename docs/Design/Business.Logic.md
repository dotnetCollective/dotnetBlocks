<!---
Title: Business Logic
NavigationTitle: Business Logic
ShowInNavigation: false
ShowInSidebar: true
NoSidebar: false
Excerpt: describes the roles of business logic classes and interfaces in the design

--->

# Design Overview


Engineers in large organizations and enterprises are focused on delivering value by solving the business problem, not playing with technology. The design patterns, practices and technologies they use should solve the technical problems without requiring deep thought so the focus is on the business problem.

To help engineers focus on the business problem, the architecture introduces:
1. Business Logic Classes
2. IBusiness Logic Interface
3. Operations and Events
4. Business Entitites

### IBusinessLogic Interface
This is a common inteface shared by business logic classes allowing architecture expension areound the promises made by the business logic interface, its inheritors and implementors.

### Business Logic Classes

These classes implement the solution to the business problem and can be hosted anywhere in the enterprise. They encapsulate the logic, similar to the "actor" pattern used by [.NetAspire](https://learn.microsoft.com/en-us/dotnet/aspire/get-started/aspire-overview). A business logic controller or implementor is similar to a grain used in the asire model.
Because the class hides behind a IBusinessLogic Interface, it can be moved around in the organizations, abstracted from transport (e.g. web services, Event handlers, grpc, direct instantiation), hosted anywhere in the enterprise (containers, functions, on-permise, direct instance, in the cloud etc) and not allow the logic to bleed into the host or take on host dependencies.
Its isolation also improves unit testing abilities.

All business code is contained in business logic classes that implement an IBusiness Logic interface with methods that accept a business logic operation or event as parameters. This encapsulation of the logic forces the logic to remain in the class and not bleed out into the hosting or transport classes e.g. api host, handler host or function host. The design means that business logic becomes host and transport agnostic, but remains isolated because of the "programming by strong contract design".

Business logic classes take events and operations as their parameters, similar to a command pattern but without the dependency to the implementation. Operations and events can also be transported via message and event platforms or web transports. The operations take advantage of serializers to reduce the coupling, especially when using JSON and other forgiving serializers.

### [Operations](/Design/operations.html)
Operations often contain business entities, so the models is passed around the enterprise reducing the amount of object mapping required by the architecture.
Operations and events are the implementations of a command parameter pattern, isolating the events and commands from the business logic and transport. Operations are self-validating increasing the probability of success prior to presentation for processing.

### [Business Entities](/design/Business.Entities.html) 
Business entities represent the domain model base building blocks. A business entity approach forces engineers to model the business using a domain model approach and allows the framework to expand generically, but extends the functionality gated to business entities or business entity based intefaces.

## More Content
<div>\@Html.Partial("_ChildPages",Document)</div>
