# DotNetBuildingBlocks

 
 >>"Helping developers focus on solving the business problem instead of fighting with technology and design." 
 >> We solve the hardest problems so you don't have to.
 
 
 ![DotNetBuildingBlocksMainImage](Images/DotNetBuildingBlocks.jpg "DotNetBuildingBlocks")
 
# DotNet Building Blocks

 Fundamental building blocks for developing and supporting large scalable applications and platforms. with minimal code and minimal custom technical design or problem solving. Focus on solving the "business problem".
 

- .Net Building blocks provides design, patterns and fundamental blocks used in creating distributed scalable applications.
- Building blocks allow developers and organizations to build applications that rapidly scale quickly with strong repeatable design patterns.


## License

MS-PL. This software is governed by the Microsoft Public  license. (MS-PL)

https://fossa.com/blog/open-source-licenses-101-microsoft-public-license-ms-pl/

## The Team

This project is brought to you by "The DotNetCollective".




# The fundamentals


Creating a scalable platform requires:
1. re-usable building blocks or core components or providers abstracting underlying technologies for portability. 
1. 2 subsystems : Re-usable subsystems solving a variety of problems in a re-usable macro functional independently deployable system. Larger than micro-services, but similar in concept.
1. 3. Recipes standard re-usable patterns and solutions ensuring scalability e.g. LinQ projection.

# Future track

# Core components (Providers)
1. Storage Provider - Abstraction of storage providing pluggable storage technologies for different storage technologies in  all our components e.g.  - AWS, Azure, Google Cloud, Disk storage.
1. Messaging Provider - cornerstone of distributed processing - sending receiving and processing of asynchronous messages. extending the Mass Transit project, simplifying the implementation and management, solving for additional messaging problems e.g. routing, recovery, error,management, large messages. and plugging in other messaging technologies. e.g Rabbit MQ, Azure Service Bus, Event Grid, Azure Queues.	
	1.


# Point solutions
1.  Pre - solved problems
	1.  Problem 1
# Problems to solve
1. Versioning strategy, schema and dev ops implementation.

# Micro subsystems

1. Batch orchestration and tracking - lightweight subsystem for tracking distributed process steps and signaling other platforms on process completion.
1. Notification - notifying communication targets of events, regardless of channel. Target Registration, preference management, rendering / composition , delivery, confirmation, suppression,  with pluggable communication channels.
1. Rendering - take data and creates formatted output. takes input, applies a template based transformation and formats the output based on the output adapter applied. System uses a storage provider as a data source, applies custom formatters and uses output providers in an adapter pattern to format the file output in a consumable standard format.

