<!---
Title: DotNetBlocks.Extensions
SubTitle: .Extensions

ShowInSidebar: true
Excerpt: Extends Microsoft.Extensions. Adds Lazy<T> support to Microsoft.Extensions.DependencyInjection
Level: 0
Order: 0
--->


# DotNetBlocks.Extensions.DependencyInjection

Adds functionality to the Microsoft.Extensions namespace to solve Lazy<T> DI and other missing functionality problems.

### Getting started
[ NuGet : DotNetBlocks.Extensions.DependencyInjection](https://www.nuget.org/packages/dotNetBlocks.Extensions.DependencyInjection)

[Licensing and other information](/)

### Functionality

## DependencyInjection

To improve performance, design patterns use the injection of Lazy<TService> in their constructors to delay the cost of initiation to JustInTime.
Microsoft DI does not support LAZY Service types. Options are to use a different DI provider or use these extensions.

### Problems Solved

Lack of support for lazy types in Microsoft.Extensions.DependencyInjection

#Examples


# Dependency Injection Problem Solved.
Microsoft DI is the simplest DI system available and does not support inferring Lazy<T> using already registered service types.
The most effective solution to wasting resources when injecting Services via class constructor parameters is using a Lazy<Service pattern.
This delays the construction of the resource until first use.

If you use MS DI, you have to registee the Lazy<T> for every class that may be lazy, and we want the DI container to generically support Lazy<T> for our constructors.

We need to add the functionality to create any Lazy<T> class if the service is already registered.
# Alternatives
A better solution is to use AutoFac or other open source DI systems and leverage the more advanced functionality.

## The Solution

## References

[Microsoft DI](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)
[Default Service Container Replacement](https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-guidelines#default-service-container-replacement)

# Implementation notes
A first attempt was to use a lighter facade factory pattern using implicit type conventions, but issues with Di type conversion testing on implicit classes prevented its use.
The final lazy implementation had to exactly match the lazy<T> signature, so extending the class with DI support became the easiest solution.
You also can't use a generic factory method and a dynamic keyword because there is no way to add a parameter that can be used for dynamic type discovery.

Solution inspiration is this discussion aboard
[Solution ideas discussion - for credit](https://stackoverflow.com/questions/44934511/does-net-core-dependency-injection-support-lazyt)

## details

This library implements two solutions internally. One uses a Lazy Implementation class that inherits from Lazy but with a constructor with DI support. This is a simple but effective solution to the problem
and is the soution implementation when you "add Lazy Support" to the service collection. It registers this LazyService type for use in all lazy<T> requests.

The second implementation is a "true lazy" implementation using a delegate constructor to create an true Lazy class with a dynamic costructor. This is the solution used when you use a Lazy registration for a type or the AsLazy registration extension method.
The "true" implementation is modeled on the standard microsoft libraries, using copied and modified versions of the microsoft DI service registration and service descriptor static methods.


The internal pattern is to create ServiceDescriptors using the "describe" pattern and add those to the service collection. The describe method creates the appropriate factories and lifetimes supporting the requeted lazy usage registration type.

## Factory methods

The factory methods use methodInfo to close the types and invoke the factory method builder, but its important to note this does not have a performance impact, because the builder method is only called during registration to create an appropriate delegate that is passed to the dependency registration. This technique is faster than building the lambdas using code and more maintainable.