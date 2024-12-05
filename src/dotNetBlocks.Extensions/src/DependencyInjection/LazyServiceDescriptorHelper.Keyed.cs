using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Reflection;


namespace dotNetBlocks.Extensions.DependencyInjection
{
    internal static partial class LazyServiceDescriptorHelper
    {


        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <typeparamref name="TService"/>, <typeparamref name="TImplementation"/>,
        /// and the <see cref="ServiceLifetime.Transient"/> lifetime.
        /// </summary>
        /// <typeparam name="TService">The type of the serviceType.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the serviceType.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeKeyedTransientLazy<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(object? serviceKey)
            where TService : class
            where TImplementation : class, TService
        {
            return DescribeKeyedLazy<TService, TImplementation>(serviceKey, ServiceLifetime.Transient);
        }

        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <paramref name="service"/> and <paramref name="implementationType"/>
        /// and the <see cref="ServiceLifetime.Transient"/> lifetime.
        /// </summary>
        /// <param name="service">The type of the serviceType.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the serviceType.</param>
        /// <param name="implementationType">The type of the implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        public static ServiceDescriptor DescribeKeyedTransientLazy(
            Type service,
            object? serviceKey,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType)
        {
            ArgumentNullException.ThrowIfNull(service);
            ArgumentNullException.ThrowIfNull(implementationType);

            return DescribeKeyedLazy(service, serviceKey, implementationType, ServiceLifetime.Transient);
        }


        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <typeparamref name="TService"/>, <typeparamref name="TImplementation"/>,
        /// <paramref name="implementationFactory"/>,
        /// and the <see cref="ServiceLifetime.Transient"/> lifetime.
        /// </summary>
        /// <typeparam name="TService">The type of the serviceType.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the serviceType.</param>
        /// <param name="implementationFactory">A factory to create new instances of the serviceType implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeKeyedTransientLazy<TService, TImplementation>(
                object? serviceKey,
                Func<IServiceProvider, object?, TImplementation> implementationFactory)
                where TService : class
                where TImplementation : class, TService
        {
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return DescribeKeyedLazy(typeof(TService), serviceKey, implementationFactory, ServiceLifetime.Transient);
        }

        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <typeparamref name="TService"/>, <paramref name="implementationFactory"/>,
        /// and the <see cref="ServiceLifetime.Transient"/> lifetime.
        /// </summary>
        /// <typeparam name="TService">The type of the serviceType.</typeparam>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the serviceType.</param>
        /// <param name="implementationFactory">A factory to create new instances of the serviceType implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeKeyedTransientLazy<TService>(object? serviceKey, Func<IServiceProvider, object?, TService> implementationFactory)
                where TService : class
        {
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return DescribeKeyedLazy(typeof(TService), serviceKey, implementationFactory, ServiceLifetime.Transient);
        }

        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <paramref name="service"/>, <paramref name="implementationFactory"/>,
        /// and the <see cref="ServiceLifetime.Transient"/> lifetime.
        /// </summary>
        /// <param name="service">The type of the serviceType.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the serviceType.</param>
        /// <param name="implementationFactory">A factory to create new instances of the serviceType implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeKeyedTransientLazy(Type service, object? serviceKey, Func<IServiceProvider, object?, object> implementationFactory)
        {
            ArgumentNullException.ThrowIfNull(service);
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return DescribeKeyedLazy(service, serviceKey, implementationFactory, ServiceLifetime.Transient);
        }

        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <typeparamref name="TService"/>, <typeparamref name="TImplementation"/>,
        /// and the <see cref="ServiceLifetime.Scoped"/> lifetime.
        /// </summary>
        /// <typeparam name="TService">The type of the serviceType.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the serviceType.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeKeyedScopedLazy<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(object? serviceKey)
                where TService : class
                where TImplementation : class, TService
        {
            return DescribeKeyedLazy<TService, TImplementation>(serviceKey, ServiceLifetime.Scoped);
        }

        internal static ServiceDescriptor DescribeKeyedScopedLazy(
                Type service,
                object? serviceKey,
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType)
        {
            return DescribeKeyedLazy(service, serviceKey, implementationType, ServiceLifetime.Scoped);
        }


        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <typeparamref name="TService"/>, <typeparamref name="TImplementation"/>,
        /// <paramref name="implementationFactory"/>,
        /// and the <see cref="ServiceLifetime.Scoped"/> lifetime.
        /// </summary>
        /// <typeparam name="TService">The type of the serviceType.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the serviceType.</param>
        /// <param name="implementationFactory">A factory to create new instances of the serviceType implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        public static ServiceDescriptor DescribeKeyedScopedLazy<TService, TImplementation>(
            object? serviceKey,
            Func<IServiceProvider, object?, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return DescribeKeyedLazy(typeof(TService), serviceKey, implementationFactory, ServiceLifetime.Scoped);
        }


        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <typeparamref name="TService"/>, <paramref name="implementationFactory"/>,
        /// and the <see cref="ServiceLifetime.Scoped"/> lifetime.
        /// </summary>
        /// <typeparam name="TService">The type of the serviceType.</typeparam>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the serviceType.</param>
        /// <param name="implementationFactory">A factory to create new instances of the serviceType implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeKeyedScopedLazy<TService>(object? serviceKey, Func<IServiceProvider, object?, TService> implementationFactory)
                where TService : class
        {
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return DescribeKeyedLazy(typeof(TService), serviceKey, implementationFactory, ServiceLifetime.Scoped);
        }


        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <paramref name="service"/>, <paramref name="implementationFactory"/>,
        /// and the <see cref="ServiceLifetime.Scoped"/> lifetime.
        /// </summary>
        /// <param name="service">The type of the serviceType.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the serviceType.</param>
        /// <param name="implementationFactory">A factory to create new instances of the serviceType implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeKeyedScopedLazy(Type service, object? serviceKey, Func<IServiceProvider, object?, object> implementationFactory)
        {
            ArgumentNullException.ThrowIfNull(service);
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return DescribeKeyedLazy(service, serviceKey, implementationFactory, ServiceLifetime.Scoped);
        }




        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <typeparamref name="TService"/>, <typeparamref name="TImplementation"/>,
        /// and the <see cref="ServiceLifetime.Singleton"/> lifetime.
        /// </summary>
        /// <typeparam name="TService">The type of the serviceType.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the serviceType.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeKeyedSingletonLazy<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
                object? serviceKey)
                where TService : class
                where TImplementation : class, TService
        {
            return DescribeKeyedLazy<TService, TImplementation>(serviceKey, ServiceLifetime.Singleton);
        }


        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <paramref name="service"/> and <paramref name="implementationType"/>
        /// and the <see cref="ServiceLifetime.Singleton"/> lifetime.
        /// </summary>
        /// <param name="service">The type of the serviceType.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the serviceType.</param>
        /// <param name="implementationType">The type of the implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeKeyedSingletonLazy(
                Type service,
                object? serviceKey,
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType)
        {
            ArgumentNullException.ThrowIfNull(service);
            ArgumentNullException.ThrowIfNull(implementationType);

            return DescribeKeyedLazy(service, serviceKey, implementationType, ServiceLifetime.Singleton);
        }


        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <typeparamref name="TService"/>, <typeparamref name="TImplementation"/>,
        /// <paramref name="implementationFactory"/>,
        /// and the <see cref="ServiceLifetime.Singleton"/> lifetime.
        /// </summary>
        /// <typeparam name="TService">The type of the serviceType.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the serviceType.</param>
        /// <param name="implementationFactory">A factory to create new instances of the serviceType implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeKeyedSingletonLazy<TService, TImplementation>(
                object? serviceKey,
                Func<IServiceProvider, object?, TImplementation> implementationFactory)
                where TService : class
                where TImplementation : class, TService
        {
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return DescribeKeyedLazy(typeof(TService), serviceKey, implementationFactory, ServiceLifetime.Singleton);
        }



        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <typeparamref name="TService"/>, <paramref name="implementationFactory"/>,
        /// and the <see cref="ServiceLifetime.Singleton"/> lifetime.
        /// </summary>
        /// <typeparam name="TService">The type of the serviceType.</typeparam>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the serviceType.</param>
        /// <param name="implementationFactory">A factory to create new instances of the serviceType implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeKeyedSingletonLazy<TService>(
                object? serviceKey,
                Func<IServiceProvider, object?, TService> implementationFactory)
                where TService : class
        {
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return DescribeKeyedLazy(typeof(TService), serviceKey, implementationFactory, ServiceLifetime.Singleton);
        }


        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <paramref name="serviceType"/>, <paramref name="implementationFactory"/>,
        /// and the <see cref="ServiceLifetime.Singleton"/> lifetime.
        /// </summary>
        /// <param name="serviceType">The type of the serviceType.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the serviceType.</param>
        /// <param name="implementationFactory">A factory to create new instances of the serviceType implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeKeyedSingletonLazy(
                Type serviceType,
                object? serviceKey,
                Func<IServiceProvider, object?, object> implementationFactory)
        {
            ArgumentNullException.ThrowIfNull(serviceType);
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return DescribeKeyedLazy(serviceType, serviceKey, implementationFactory, ServiceLifetime.Singleton);
        }



        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <typeparamref name="TService"/>, <paramref name="implementationInstance"/>,
        /// and the <see cref="ServiceLifetime.Singleton"/> lifetime.
        /// </summary>
        /// <typeparam name="TService">The type of the serviceType.</typeparam>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the serviceType.</param>
        /// <param name="implementationInstance">The valueInstance of the implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeKeyedSingletonLazy<TService>(
                object? serviceKey,
                TService implementationInstance)
                where TService : class
        {
            ArgumentNullException.ThrowIfNull(implementationInstance);

            return DescribeKeyedSingletonLazy(typeof(TService), serviceKey, implementationInstance);
        }


        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <paramref name="serviceType"/>, <paramref name="implementationInstance"/>,
        /// and the <see cref="ServiceLifetime.Singleton"/> lifetime.
        /// </summary>
        /// <param name="serviceType">The type of the serviceType.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the serviceType.</param>
        /// <param name="implementationInstance">The valueInstance of the implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        public static ServiceDescriptor DescribeKeyedSingletonLazy(
            Type serviceType,
            object? serviceKey,
            object implementationInstance)
        {
            ArgumentNullException.ThrowIfNull(serviceType);
            ArgumentNullException.ThrowIfNull(implementationInstance);

            var lazyType = serviceType.MakeLazyType();

            if (lazyType.IsAssignableFrom(implementationInstance.GetType()))
                // instance is a Lazy<ServiceType
                return ServiceDescriptor.KeyedSingleton(lazyType, serviceKey, implementationInstance);
            if (serviceType.IsAssignableFrom(implementationInstance.GetType()))
                // wrap instance as lazy.
                return ServiceDescriptor.KeyedSingleton(LazyType, serviceKey, implementationInstance.ObjectToLazy());




            var implementationFactory =
                CreateLazyFactorybyTypeKeyed(lazyType, valueInstance: implementationInstance);

            return ServiceDescriptor.KeyedSingleton(serviceType, serviceKey, implementationFactory);

        }

        internal static ServiceDescriptor DescribeKeyedLazy<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
                object? serviceKey,
                ServiceLifetime lifetime)
                where TService : class
                where TImplementation : class
        {
            return DescribeKeyedLazy(
                serviceType: typeof(TService),
                serviceKey: serviceKey,
                typeof(TImplementation),
                lifetime: lifetime);
        }


        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <paramref name="serviceType"/>, <paramref name="implementationType"/>,
        /// and <paramref name="lifetime"/>.
        /// </summary>
        /// <param name="serviceType">The type of the serviceType.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the serviceType.</param>
        /// <param name="implementationType">The type of the implementation.</param>
        /// <param name="lifetime">The lifetime of the serviceType.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeKeyedLazy(
                Type serviceType,
                object? serviceKey,
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType,
                ServiceLifetime lifetime)
        => DescribeKeyedLazy(
            serviceType: serviceType, 
            serviceKey: serviceKey,
            implementationFactory:default, 
            lifetime:lifetime);

        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <paramref name="serviceType"/>, <paramref name="implementationFactory"/>,
        /// and <paramref name="lifetime"/>.
        /// </summary>
        /// <param name="serviceType">The type of the serviceType.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the serviceType.</param>
        /// <param name="implementationFactory">A factory to create new instances of the serviceType implementation.</param>
        /// <param name="lifetime">The lifetime of the serviceType.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeKeyedLazy(Type serviceType, object? serviceKey, Func<IServiceProvider, object?, object>? implementationFactory, ServiceLifetime lifetime)
        {
            ArgumentNullException.ThrowIfNull(serviceType);

            var lazyService = serviceType.MakeLazyServiceType();
            implementationFactory =
                CreateLazyFactorybyTypeKeyed(lazyService, valuefactoryMethod: implementationFactory);
            return ServiceDescriptor.DescribeKeyed(serviceType, serviceKey, implementationFactory, lifetime);
        }




    }
}