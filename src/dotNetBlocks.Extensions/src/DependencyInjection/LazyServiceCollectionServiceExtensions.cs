using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace dotNetBlocks.Extensions.DependencyInjection
{
    internal class LazyServiceCollectionServiceExtensions
    {
    }
}
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


namespace dotNetBlocks.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for adding services to an <see cref="IServiceCollection" />.
    /// </summary>
    public static partial class ServiceCollectionServiceExtensions
    {
        /// <summary>
        /// Adds a transient service of the type specified in <paramref name="serviceType"/> with an
        /// implementation of the type specified in <paramref name="implementationType"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Transient"/>
        public static IServiceCollection AddKeyedTransientLazy(
            this IServiceCollection services,
            Type serviceType,
            object? serviceKey,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(serviceType);
            ArgumentNullException.ThrowIfNull(implementationType);

            return AddKeyedLazy(services, serviceType, serviceKey, implementationType, ServiceLifetime.Transient);
        }

        /// <summary>
        /// Adds a transient service of the type specified in <paramref name="serviceType"/> with a
        /// factory specified in <paramref name="implementationFactory"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Transient"/>
        public static IServiceCollection AddKeyedTransientLazy(
            this IServiceCollection services,
            Type serviceType,
            object? serviceKey,
            Func<IServiceProvider, object?, object> implementationFactory)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(serviceType);
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return AddKeyedLazy(services, serviceType, serviceKey, implementationFactory, ServiceLifetime.Transient);
        }

        /// <summary>
        /// Adds a transient service of the type specified in <typeparamref name="TService"/> with an
        /// implementation type specified in <typeparamref name="TImplementation"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Transient"/>
        public static IServiceCollection AddKeyedTransientLazy<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
            this IServiceCollection services,
            object? serviceKey)
            where TService : class
            where TImplementation : class, TService
        {
            ArgumentNullException.ThrowIfNull(services);

            return services.AddKeyedTransientLazy(typeof(TService), serviceKey, typeof(TImplementation));
        }

        /// <summary>
        /// Adds a transient service of the type specified in <paramref name="serviceType"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceType">The type of the service to register and the implementation to use.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Transient"/>
        public static IServiceCollection AddKeyedTransientLazy(
            this IServiceCollection services,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type serviceType,
            object? serviceKey)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(serviceType);

            return services.AddKeyedTransientLazy(serviceType, serviceKey, serviceType);
        }

        /// <summary>
        /// Adds a transient service of the type specified in <typeparamref name="TService"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Transient"/>
        public static IServiceCollection AddKeyedTransientLazy<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TService>(
            this IServiceCollection services,
            object? serviceKey)
            where TService : class
        {
            ArgumentNullException.ThrowIfNull(services);

            return services.AddKeyedTransientLazy(typeof(TService), serviceKey);
        }

        /// <summary>
        /// Adds a transient service of the type specified in <typeparamref name="TService"/> with a
        /// factory specified in <paramref name="implementationFactory"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Transient"/>
        public static IServiceCollection AddKeyedTransientLazy<TService>(
            this IServiceCollection services,
            object? serviceKey,
            Func<IServiceProvider, object?, TService> implementationFactory)
            where TService : class
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return services.AddKeyedTransientLazy(typeof(TService), serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a transient service of the type specified in <typeparamref name="TService"/> with an
        /// implementation type specified in <typeparamref name="TImplementation" /> using the
        /// factory specified in <paramref name="implementationFactory"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Transient"/>
        public static IServiceCollection AddKeyedTransientLazy<TService, TImplementation>(
            this IServiceCollection services,
            object? serviceKey,
            Func<IServiceProvider, object?, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return services.AddKeyedTransientLazy(typeof(TService), serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a scoped service of the type specified in <paramref name="serviceType"/> with an
        /// implementation of the type specified in <paramref name="implementationType"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Scoped"/>
        public static IServiceCollection AddKeyedScopedLazy(
            this IServiceCollection services,
            Type serviceType,
            object? serviceKey,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(serviceType);
            ArgumentNullException.ThrowIfNull(implementationType);

            return AddKeyedLazy(services, serviceType, serviceKey, implementationType, ServiceLifetime.Scoped);
        }

        /// <summary>
        /// Adds a scoped service of the type specified in <paramref name="serviceType"/> with a
        /// factory specified in <paramref name="implementationFactory"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Scoped"/>
        public static IServiceCollection AddKeyedScopedLazy(
            this IServiceCollection services,
            Type serviceType,
            object? serviceKey,
            Func<IServiceProvider, object?, object> implementationFactory)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(serviceType);
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return AddKeyedLazy(services, serviceType, serviceKey, implementationFactory, ServiceLifetime.Scoped);
        }

        /// <summary>
        /// Adds a scoped service of the type specified in <typeparamref name="TService"/> with an
        /// implementation type specified in <typeparamref name="TImplementation"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Scoped"/>
        public static IServiceCollection AddKeyedScopedLazy<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
            this IServiceCollection services,
            object? serviceKey)
            where TService : class
            where TImplementation : class, TService
        {
            ArgumentNullException.ThrowIfNull(services);

            return services.AddKeyedScopedLazy(typeof(TService), serviceKey, typeof(TImplementation));
        }

        /// <summary>
        /// Adds a scoped service of the type specified in <paramref name="serviceType"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceType">The type of the service to register and the implementation to use.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Scoped"/>
        public static IServiceCollection AddKeyedScopedLazy(
            this IServiceCollection services,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type serviceType,
            object? serviceKey)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(serviceType);

            return services.AddKeyedScopedLazy(serviceType, serviceKey, serviceType);
        }

        /// <summary>
        /// Adds a scoped service of the type specified in <typeparamref name="TService"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Scoped"/>
        public static IServiceCollection AddKeyedScopedLazy<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TService>(
            this IServiceCollection services,
            object? serviceKey)
            where TService : class
        {
            ArgumentNullException.ThrowIfNull(services);

            return services.AddKeyedScopedLazy(typeof(TService), serviceKey);
        }

        /// <summary>
        /// Adds a scoped service of the type specified in <typeparamref name="TService"/> with a
        /// factory specified in <paramref name="implementationFactory"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Scoped"/>
        public static IServiceCollection AddKeyedScopedLazy<TService>(
            this IServiceCollection services,
            object? serviceKey,
            Func<IServiceProvider, object?, TService> implementationFactory)
            where TService : class
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return services.AddKeyedScopedLazy(typeof(TService), serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a scoped service of the type specified in <typeparamref name="TService"/> with an
        /// implementation type specified in <typeparamref name="TImplementation" /> using the
        /// factory specified in <paramref name="implementationFactory"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Scoped"/>
        public static IServiceCollection AddKeyedScoped<TService, TImplementation>(
            this IServiceCollection services,
            object? serviceKey,
            Func<IServiceProvider, object?, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return services.AddKeyedScopedLazy(typeof(TService), serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a singleton service of the type specified in <paramref name="serviceType"/> with an
        /// implementation of the type specified in <paramref name="implementationType"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
        /// <param name="implementationType">The implementation type of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Singleton"/>
        public static IServiceCollection AddKeyedSingletonLazy(
            this IServiceCollection services,
            Type serviceType,
            object? serviceKey,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(serviceType);
            ArgumentNullException.ThrowIfNull(implementationType);

            return AddKeyedLazy(services, serviceType, serviceKey, implementationType, ServiceLifetime.Singleton);
        }

        /// <summary>
        /// Adds a singleton service of the type specified in <paramref name="serviceType"/> with a
        /// factory specified in <paramref name="implementationFactory"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Singleton"/>
        public static IServiceCollection AddKeyedSingletonLazy(
            this IServiceCollection services,
            Type serviceType,
            object? serviceKey,
            Func<IServiceProvider, object?, object> implementationFactory)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(serviceType);
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return AddKeyedLazy(services, serviceType, serviceKey, implementationFactory, ServiceLifetime.Singleton);
        }

        /// <summary>
        /// Adds a singleton service of the type specified in <typeparamref name="TService"/> with an
        /// implementation type specified in <typeparamref name="TImplementation"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Singleton"/>
        public static IServiceCollection AddKeyedSingletonLazy<TService, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(
            this IServiceCollection services,
            object? serviceKey)
            where TService : class
            where TImplementation : class, TService
        {
            ArgumentNullException.ThrowIfNull(services);

            return services.AddKeyedSingletonLazy(typeof(TService), serviceKey, typeof(TImplementation));
        }

        /// <summary>
        /// Adds a singleton service of the type specified in <paramref name="serviceType"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceType">The type of the service to register and the implementation to use.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Singleton"/>
        public static IServiceCollection AddKeyedSingletonLazy(
            this IServiceCollection services,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type serviceType,
             object? serviceKey)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(serviceType);

            return services.AddKeyedSingletonLazy(serviceType, serviceKey, serviceType);
        }

        /// <summary>
        /// Adds a singleton service of the type specified in <typeparamref name="TService"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Singleton"/>
        public static IServiceCollection AddKeyedSingletonLazy<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TService>(
            this IServiceCollection services,
            object? serviceKey)
            where TService : class
        {
            ArgumentNullException.ThrowIfNull(services);

            return services.AddKeyedSingletonLazy(typeof(TService), serviceKey, typeof(TService));
        }

        /// <summary>
        /// Adds a singleton service of the type specified in <typeparamref name="TService"/> with a
        /// factory specified in <paramref name="implementationFactory"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Singleton"/>
        public static IServiceCollection AddKeyedSingletonLazy<TService>(
            this IServiceCollection services,
            object? serviceKey,
            Func<IServiceProvider, object?, TService> implementationFactory)
            where TService : class
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return services.AddKeyedSingletonLazy(typeof(TService), serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a singleton service of the type specified in <typeparamref name="TService"/> with an
        /// implementation type specified in <typeparamref name="TImplementation" /> using the
        /// factory specified in <paramref name="implementationFactory"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
        /// <param name="implementationFactory">The factory that creates the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Singleton"/>
        public static IServiceCollection AddKeyedSingletonLazy<TService, TImplementation>(
            this IServiceCollection services,
            object? serviceKey,
            Func<IServiceProvider, object?, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(implementationFactory);

            return services.AddKeyedSingletonLazy(typeof(TService), serviceKey, implementationFactory);
        }

        /// <summary>
        /// Adds a singleton service of the type specified in <paramref name="serviceType"/> with an
        /// instance specified in <paramref name="implementationInstance"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceType">The type of the service to register.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
        /// <param name="implementationInstance">The instance of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Singleton"/>
        public static IServiceCollection AddKeyedSingletonLazy(
            this IServiceCollection services,
            Type serviceType,
            object? serviceKey,
            object implementationInstance)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(serviceType);
            ArgumentNullException.ThrowIfNull(implementationInstance);

            var serviceDescriptor = LazyServiceDescriptorHelper.DescribeKeyedSingletonLazy(serviceType, serviceKey, implementationInstance);
            services.Add(serviceDescriptor);
            return services;
        }

        /// <summary>
        /// Adds a singleton service of the type specified in <typeparamref name="TService" /> with an
        /// instance specified in <paramref name="implementationInstance"/> to the
        /// specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
        /// <param name="serviceKey">The <see cref="ServiceDescriptor.ServiceKey"/> of the service.</param>
        /// <param name="implementationInstance">The instance of the service.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="ServiceLifetime.Singleton"/>
        public static IServiceCollection AddKeyedSingletonLazy<TService>(
            this IServiceCollection services,
            object? serviceKey,
            TService implementationInstance)
            where TService : class
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(implementationInstance);

            return services.AddKeyedSingletonLazy(typeof(TService), serviceKey, implementationInstance);
        }

        private static IServiceCollection AddKeyedLazy(
            IServiceCollection collection,
            Type serviceType,
            object? serviceKey,
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type implementationType,
            ServiceLifetime lifetime)
        {
            var descriptor = LazyServiceDescriptorHelper.DescribeKeyedLazy(serviceType, serviceKey, implementationType, lifetime);
            collection.Add(descriptor);
            return collection;
        }

        private static IServiceCollection AddKeyedLazy(
            IServiceCollection collection,
            Type serviceType,
            object? serviceKey,
            Func<IServiceProvider, object?, object> implementationFactory,
            ServiceLifetime lifetime)
        {
            var descriptor = LazyServiceDescriptorHelper.DescribeKeyedLazy(serviceType, serviceKey, implementationFactory, lifetime);
            collection.Add(descriptor);
            return collection;
        }
    }
}
