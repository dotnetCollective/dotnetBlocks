
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