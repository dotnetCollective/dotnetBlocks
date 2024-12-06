using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using dotNetBlocks.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace dotNetBlocks.Extensions
{
    public static class ServiceDescriptorExtensions
    {
        public static ServiceDescriptor AsLazy(this ServiceDescriptor sourceService, ServiceLifetime? serviceLifetime = default, object? valueInstance = default, object? lazyInstance = default)
        {
            Type lazyServiceType = sourceService.ServiceType.MakeLazyType();

            // Check the instance object is the right type.
            if (lazyInstance != default && !lazyServiceType.IsAssignableFrom(lazyInstance.GetType())) throw new ArgumentException($"instance object is not {lazyServiceType.FullName}", nameof(lazyInstance));

            if (lazyInstance != default) // This is a pure singleton registration with a provided value, just register it.
                return sourceService.IsKeyedService switch {
                    true => new ServiceDescriptor(lazyInstance!.GetType(), sourceService.ServiceKey, lazyInstance), // KeyedService singleton.
                    _ => new ServiceDescriptor(lazyInstance!.GetType(), lazyInstance) // Unkeyed singleton.
                };

            // This is the standard implementation and depends on a factory method to register the class properly. This is either keyed on un-keyed.
            return sourceService.IsKeyedService switch
            {
                true => new ServiceDescriptor(
                        serviceType: lazyServiceType, 
                        serviceKey: sourceService.ServiceKey,
                        factory: LazyServiceDescriptorHelper.CreateLazyFactorybyTypeKeyed(
                            serviceType: sourceService.ServiceType,
                            implementationType: sourceService.KeyedImplementationType,
                            lazyInstance: lazyInstance,
                            valueInstance: valueInstance ?? sourceService.KeyedImplementationInstance, valuefactoryMethod: sourceService.KeyedImplementationFactory), serviceLifetime ?? sourceService.Lifetime),
                false => new ServiceDescriptor(
                        serviceType: lazyServiceType,
                        factory: LazyServiceDescriptorHelper.CreateLazyFactoryByType(
                            serviceType: sourceService.ServiceType,
                            implementationType: sourceService.ImplementationType,
                            lazyInstance: lazyInstance,
                            valueInstance: valueInstance ?? sourceService.ImplementationInstance, valuefactoryMethod: sourceService.ImplementationFactory), serviceLifetime ?? sourceService.Lifetime),
            };


        }
    }
}
