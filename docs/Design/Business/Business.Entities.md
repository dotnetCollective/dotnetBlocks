<!---
Title: Business Entities
NavigationTitle: Business Entities
ShowInNavigation: false
ShowInSidebar: true
NoSidebar: false
Excerpt: describes design of all the base classes, interfaces and patterns defining the domain models used in the architecture.

--->

# Design Overview


Entities are business object models that can be serialized, transmitted or used in a persistance strategy (typically EF or blob storage);
Business entities are similar to data transfer objects in older n-tier paradigms.

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


# Abstract base and Generic derived class problem
This article explains the design pattern for derived classes.
https://www.devgem.io/posts/resolving-generic-type-conversion-issues-in-c-inheritance


#Keying strategies

storing, retrieving and uniquely identifying objects requires an identification system. The identifiers are called keys, with a subset of keys being called ids.

The framework makes a keyed business entity available for use.

1. Keyed entities - generic class allowing a key type during closure.

## Key or ID types

For performance reasons, data bases and other repository types prefer sequential increased identifiers. numeric id types are preferred.

Disadvantage of numeric types is the ID cannot be allocated in advance, so it can only be generated during persistence. Numeric keys are not portable and are typically re-assigned when data is moved between repositories.

Global Identifiers (GUIDS) allow any system to generate an ID. Guids are a random number and the uniform distribution causes performance issues in most repository technologies.

GUIDS have the advantage of being portable, so data can be moved from system to system or correlated across multiple systems.

## Generating GUIDs
Alternate GUID generation algorithmns create semi-unique ever increasing numbers. This helps but doesn't mitigate the poor performance of GUIDS.
The algorithm recommended for the framework is the CombGuid algorithmn first created by Jimmmy Nilsson. The referenced nuget library for GUID generation gives framework users lots of options.

## Dual keying

The framework provides a dual keyed object using an ID field and an additional GlobalID to store a guid. This is a best of both worlds blend allowing portability between rpositories but performance within a single repository.

The entities are the BusinessEntity (ID) and the GlobalKeyedEntity (ID and Global ID);



## Business Entities

1. BusinessEntity - base class for business entities using for type closures
1. 2. KeyedEntity - Business Entity with a generic ID property
	1. GlobalKeyedEntity - Dual keyed entity with Generic ID property and a GUID Global ID (Recommended.)
	1. IAuditable Entities - versions of the entity types implementing the IAuditable interface and properties, so store data about the CRUD operations for auditing purposes.
