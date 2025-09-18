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

        /// <summary>
        /// Creates a <see cref="Lazy{T}"/> <see cref="ServiceDescriptor"/> for the descriptor provided.
        /// </summary>
        /// <param name="sourceDescriptor">The source <see cref="ServiceDescriptor"/>  > for the new service descriptor.</param>
        /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/>> for the lazy class. Uses the source lifetime if none provided.</param>
        /// <returns>New <see cref=" ServiceDescriptor"/> defining a lazy version of the source descriptor.</returns>
        /// <exception cref="System.ArgumentException">instance object is not {lazyServiceType.FullName} - lazyInstance</exception>
        public static ServiceDescriptor AsLazy(this ServiceDescriptor sourceDescriptor, ServiceLifetime? serviceLifetime = default)
        {
            Type serviceType = sourceDescriptor.ServiceType;
            Type lazyServiceType = serviceType.MakeLazyType();
            return sourceDescriptor.IsKeyedService switch
            {
                true => new ServiceDescriptor(lazyServiceType, LazyServiceDescriptorHelper.BuildSimpleLazyFactoryByTypeKeyed(serviceType), serviceLifetime ?? sourceDescriptor.Lifetime),
                false => new ServiceDescriptor(lazyServiceType, LazyServiceDescriptorHelper.BuildSimpleLazyFactoryByType(serviceType), serviceLifetime ?? sourceDescriptor.Lifetime),
            };
        }

        /// <summary>
        /// Creates a <see cref="Lazy{T}"/> <see cref="ServiceDescriptor"/> for the descriptor provided.
        /// </summary>
        /// <param name="sourceDescriptor">The source <see cref="ServiceDescriptor"/>  > for the new service descriptor.</param>
        /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/>> for the lazy class. Uses the source lifetime if none provided.</param>
        /// <param name="valueInstance">Instance value to use in the lazy class instead of resolving the value. </param>
        /// <param name="lazyInstance">The lazy instance to register.</param>
        /// <returns>New <see cref=" ServiceDescriptor"/> defining a lazy version of the source descriptor.</returns>
        /// <exception cref="System.ArgumentException">instance object is not {lazyServiceType.FullName} - lazyInstance</exception>
        public static ServiceDescriptor AsLazyAdvanced(this ServiceDescriptor sourceDescriptor, ServiceLifetime? serviceLifetime = default, object? valueInstance = default, object? lazyInstance = default)
        {
            Type lazyServiceType = sourceDescriptor.ServiceType.MakeLazyType();

            // Check the instance object is the right type.
            if (lazyInstance != default && !lazyServiceType.IsAssignableFrom(lazyInstance.GetType())) throw new ArgumentException($"instance object is not {lazyServiceType.FullName}", nameof(lazyInstance));

            if (lazyInstance != default) // This is a pure singleton registration with a provided value, just register it.
                return sourceDescriptor.IsKeyedService switch {
                    true => new ServiceDescriptor(lazyInstance!.GetType(), sourceDescriptor.ServiceKey, lazyInstance), // KeyedService singleton.
                    _ => new ServiceDescriptor(lazyInstance!.GetType(), lazyInstance) // Unkeyed singleton.
                };

            // This is the standard implementation and depends on a factory method to register the class properly. This is either keyed on un-keyed.
            return sourceDescriptor.IsKeyedService switch
            {
                true => new ServiceDescriptor(
                        serviceType: lazyServiceType, 
                        serviceKey: sourceDescriptor.ServiceKey,
                        factory: LazyServiceDescriptorHelper.CreateLazyFactorybyTypeKeyed(
                            serviceType: sourceDescriptor.ServiceType,
                            implementationType: sourceDescriptor.KeyedImplementationType,
                            lazyInstance: lazyInstance,
                            valueInstance: valueInstance ?? sourceDescriptor.KeyedImplementationInstance, valuefactoryMethod: sourceDescriptor.KeyedImplementationFactory), serviceLifetime ?? sourceDescriptor.Lifetime),
                false => new ServiceDescriptor(
                        serviceType: lazyServiceType,
                        factory: LazyServiceDescriptorHelper.CreateLazyFactoryByType(
                            serviceType: sourceDescriptor.ServiceType,
                            implementationType: sourceDescriptor.ImplementationType,
                            lazyInstance: lazyInstance,
                            valueInstance: valueInstance ?? sourceDescriptor.ImplementationInstance, valuefactoryMethod: sourceDescriptor.ImplementationFactory), serviceLifetime ?? sourceDescriptor.Lifetime),
            };


        }
    }
}
