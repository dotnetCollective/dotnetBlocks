﻿<!---
Title: Design - Business Entities
NavigationTitle: Design - Business Entities
ShowInNavigation: false
ShowInSidebar: true
NoSidebar: false
Excerpt: describes all design of the base classes, interfaces and patterns defining the domain models used in the architecture.

--->

# Business entities

Business entities are the basis for all business models. They can also be used in method parameters and serialized for transmission. Descending from business entities helps with discovery and for the cration of generic type closures to create a better experience for developers using the architecture.

# Keyed Entities
Keyed entities define an identifier for a business entity to uniquely identify that instance. A unique ID is mostly required for CRUD purposes.
# Global Keyed Entities
Global entities using a GUID as their unique identifier, but a default implementation of a CombGuid (Combinational Guid) is provided to reduce the impact of random distributions. That topic is discussed at length elsewhere.
#Auditable Entities
Auditable entities use a composition model to persist information about the standard CRUD operations of entities.
#Persistence.
Business entities are not active, but their common structures mean tasks like persistence can be made generic reducing the amount of coding required.
