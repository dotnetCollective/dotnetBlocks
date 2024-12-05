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
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeLazyTransient<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            return DescribeKeyedLazy<TService, TImplementation>(null, ServiceLifetime.Transient);
        }


        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <paramref name="service"/> and <paramref name="implementationType"/>
        /// and the <see cref="ServiceLifetime.Transient"/> lifetime.
        /// </summary>
        /// <param name="service">The type of the serviceType.</param>
        /// <param name="implementationType">The type of the implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeTransientLazy(
                Type service,
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType)
        {
            ArgumentNullException.ThrowIfNull(service);
            ArgumentNullException.ThrowIfNull(implementationType);

            return DescribeLazy(service, implementationType, ServiceLifetime.Transient);
        }



        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <typeparamref name="TService"/>, <typeparamref name="TImplementation"/>,
        /// <paramref name="implementationFactory"/>,
        /// and the <see cref="ServiceLifetime.Transient"/> lifetime.
        /// </summary>
        /// <typeparam name="TService">The type of the serviceType.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="implementationFactory">A factory to create new instances of the serviceType implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeTransientLazy<TService, TImplementation>(
                Func<IServiceProvider, TImplementation>? implementationFactory)
                where TService : class
                where TImplementation : class, TService
        {
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return DescribeLazy(typeof(TService), implementationFactory, ServiceLifetime.Transient);
        }



        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <typeparamref name="TService"/>, <paramref name="implementationFactory"/>,
        /// and the <see cref="ServiceLifetime.Transient"/> lifetime.
        /// </summary>
        /// <typeparam name="TService">The type of the serviceType.</typeparam>
        /// <param name="implementationFactory">A factory to create new instances of the serviceType implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeTransientLazy<TService>(Func<IServiceProvider, TService> implementationFactory)
                where TService : class
        {
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return DescribeLazy(typeof(TService), implementationFactory, ServiceLifetime.Transient);
        }


        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <paramref name="service"/>, <paramref name="implementationFactory"/>,
        /// and the <see cref="ServiceLifetime.Transient"/> lifetime.
        /// </summary>
        /// <param name="service">The type of the serviceType.</param>
        /// <param name="implementationFactory">A factory to create new instances of the serviceType implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeTransientLazy(Type service, Func<IServiceProvider, object> implementationFactory)
        {
            ArgumentNullException.ThrowIfNull(service);
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return DescribeLazy(service, implementationFactory, ServiceLifetime.Transient);
        }


        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <typeparamref name="TService"/>, <typeparamref name="TImplementation"/>,
        /// and the <see cref="ServiceLifetime.Scoped"/> lifetime.
        /// </summary>
        /// <typeparam name="TService">The type of the serviceType.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeScopedLazy<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>()
                where TService : class
                where TImplementation : class, TService
        {
            return DescribeKeyedLazy<TService, TImplementation>(null, ServiceLifetime.Scoped);
        }



        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <paramref name="service"/> and <paramref name="implementationType"/>
        /// and the <see cref="ServiceLifetime.Scoped"/> lifetime.
        /// </summary>
        /// <param name="service">The type of the serviceType.</param>
        /// <param name="implementationType">The type of the implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeScopedLazy(
                Type service,
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType)
        {
            return DescribeLazy(service, implementationType, ServiceLifetime.Scoped);
        }



        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <typeparamref name="TService"/>, <typeparamref name="TImplementation"/>,
        /// <paramref name="implementationFactory"/>,
        /// and the <see cref="ServiceLifetime.Scoped"/> lifetime.
        /// </summary>
        /// <typeparam name="TService">The type of the serviceType.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="implementationFactory">A factory to create new instances of the serviceType implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeScopedLazy<TService, TImplementation>(
                Func<IServiceProvider, TImplementation> implementationFactory)
                where TService : class
                where TImplementation : class, TService
        {
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return DescribeLazy(typeof(TService), implementationFactory, ServiceLifetime.Scoped);
        }



        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <typeparamref name="TService"/>, <paramref name="implementationFactory"/>,
        /// and the <see cref="ServiceLifetime.Scoped"/> lifetime.
        /// </summary>
        /// <typeparam name="TService">The type of the serviceType.</typeparam>
        /// <param name="implementationFactory">A factory to create new instances of the serviceType implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeScopedLazy<TService>(Func<IServiceProvider, TService> implementationFactory)
                where TService : class
        {
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return DescribeLazy(typeof(TService), implementationFactory, ServiceLifetime.Scoped);
        }



        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <paramref name="service"/>, <paramref name="implementationFactory"/>,
        /// and the <see cref="ServiceLifetime.Scoped"/> lifetime.
        /// </summary>
        /// <param name="service">The type of the serviceType.</param>
        /// <param name="implementationFactory">A factory to create new instances of the serviceType implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeScopedLazy(Type service, Func<IServiceProvider, object> implementationFactory)
        {
            ArgumentNullException.ThrowIfNull(service);
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return DescribeLazy(service, implementationFactory, ServiceLifetime.Scoped);
        }





        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <paramref name="serviceType"/> and <paramref name="implementationType"/>
        /// and the <see cref="ServiceLifetime.Singleton"/> lifetime.
        /// </summary>
        /// <param name="serviceType">The type of the serviceType.</param>
        /// <param name="implementationType">The type of the implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeSingletonLazy(
                Type serviceType,
                [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType)
        {
            ArgumentNullException.ThrowIfNull(serviceType);
            ArgumentNullException.ThrowIfNull(implementationType);

            return DescribeLazy(serviceType, implementationType, ServiceLifetime.Singleton);
        }



        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <typeparamref name="TService"/>, <typeparamref name="TImplementation"/>,
        /// <paramref name="implementationFactory"/>,
        /// and the <see cref="ServiceLifetime.Singleton"/> lifetime.
        /// </summary>
        /// <typeparam name="TService">The type of the serviceType.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        /// <param name="implementationFactory">A factory to create new instances of the serviceType implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeSingletonLazy<TService, TImplementation>(
                Func<IServiceProvider, TImplementation> implementationFactory)
                where TService : class
                where TImplementation : class, TService
        {
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return DescribeLazy(typeof(TService), implementationFactory, ServiceLifetime.Singleton);
        }



        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <typeparamref name="TService"/>, <paramref name="implementationFactory"/>,
        /// and the <see cref="ServiceLifetime.Singleton"/> lifetime.
        /// </summary>
        /// <typeparam name="TService">The type of the serviceType.</typeparam>
        /// <param name="implementationFactory">A factory to create new instances of the serviceType implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        public static ServiceDescriptor DescribeSingletonLazy<TService>(Func<IServiceProvider, TService> implementationFactory)
            where TService : class
        {
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return DescribeLazy(typeof(TService), implementationFactory, ServiceLifetime.Singleton);
        }

        

        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <paramref name="serviceType"/>, <paramref name="implementationFactory"/>,
        /// and the <see cref="ServiceLifetime.Singleton"/> lifetime.
        /// </summary>
        /// <param name="serviceType">The type of the serviceType.</param>
        /// <param name="implementationFactory">A factory to create new instances of the serviceType implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        public static ServiceDescriptor DescribeSingletonLazy(
            Type serviceType,
            Func<IServiceProvider, object> implementationFactory)
        {
            ArgumentNullException.ThrowIfNull(serviceType);
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return DescribeLazy(serviceType, implementationFactory, ServiceLifetime.Singleton);
        }

        

        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <typeparamref name="TService"/>, <paramref name="implementationInstance"/>,
        /// and the <see cref="ServiceLifetime.Singleton"/> lifetime.
        /// </summary>
        /// <typeparam name="TService">The type of the serviceType.</typeparam>
        /// <param name="implementationInstance">The valueInstance of the implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeSingletonLazy<TService>(TService implementationInstance)
                where TService : class
        {
            ArgumentNullException.ThrowIfNull(implementationInstance);

            return DescribeSingletonLazy(serviceType: typeof(TService), implementationInstance: implementationInstance);
        }

        

        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <paramref name="serviceType"/>, <paramref name="implementationInstance"/>,
        /// and the <see cref="ServiceLifetime.Singleton"/> lifetime.
        /// </summary>
        /// <param name="serviceType">The type of the serviceType.</param>
        /// <param name="implementationInstance">The valueInstance of the implementation.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeSingletonLazy(
                Type serviceType,
                object implementationInstance)
        {
            ArgumentNullException.ThrowIfNull(serviceType);
            ArgumentNullException.ThrowIfNull(implementationInstance);

            return new ServiceDescriptor(serviceType, implementationInstance);
        }


        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <paramref name="serviceType"/>, <paramref name="implementationType"/>,
        /// and <paramref name="lifetime"/>.
        /// </summary>
        /// <param name="serviceType">The type of the serviceType.</param>
        /// <param name="implementationType">The type of the implementation.</param>
        /// <param name="lifetime">The lifetime of the serviceType.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        public static ServiceDescriptor DescribeLazy(
            Type serviceType,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType,
            ServiceLifetime lifetime)
        {
            var lazyServiceType = serviceType.MakeLazyType();
            var implementationFactory =
                CreateLazyFactoryByType(serviceType, implementationType);
            return ServiceDescriptor.Describe(lazyServiceType, implementationFactory, lifetime);
        }


        /// <summary>
        /// Creates an Instance of <see cref="ServiceDescriptor"/> with the specified
        /// <paramref name="serviceType"/>, <paramref name="implementationFactory"/>,
        /// and <paramref name="lifetime"/>.
        /// </summary>
        /// <param name="serviceType">The type of the serviceType.</param>
        /// <param name="implementationFactory">A factory to create new instances of the serviceType implementation.</param>
        /// <param name="lifetime">The lifetime of the serviceType.</param>
        /// <returns>A new valueInstance of <see cref="ServiceDescriptor"/>.</returns>
        internal static ServiceDescriptor DescribeLazy(Type serviceType, Func<IServiceProvider, object> implementationFactory, ServiceLifetime lifetime)
        {
            implementationFactory =
                CreateLazyFactoryByType(serviceType,valuefactoryMethod:implementationFactory);

            var lazyServiceType = serviceType.MakeLazyType();
            return ServiceDescriptor.Describe(lazyServiceType, implementationFactory, lifetime);
        }


    }
}