using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;


namespace dotNetBlocks.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {

        /// <summary>
        /// Adds support for lazy types implementing using <see cref="LazyService{T}"/> approximation.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> service collection being extended.</param>
        /// <param name="lifetime">The default lifetime for a <see cref="Lazy{T}"/> class.</param>
        /// <remarks>
        /// This is not a true <see cref="Lazy{T}"/> class implementation but uses a descendant service class to provide identical functionality <see cref="LazyService{T}"/>
        /// </remarks>
        public static void AddLazyServiceSupport(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {   // Register Lazy<>, LazyService<>;
            services.Add(ServiceDescriptor.Describe(LazyServiceDescriptorHelper.LazyType, LazyServiceDescriptorHelper.LazyServiceType, lifetime));
        }

        /// <summary>
        /// /// Adds a <see cref="Lazy{T}"/> definition to lookup the last registration and add a lazy class to.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/>containing the registration to add a lazy functionality for.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">service collection is null.</exception>
        /// <remarks>Looks up the last class and adds a lazy registration.</remarks>
        public static IServiceCollection AsLazy(this IServiceCollection services, ServiceLifetime? serviceLifetime = default)
        {
            var lastService = services.LastOrDefault();
            ArgumentNullException.ThrowIfNull(lastService);
            services.Add(lastService.AsLazy(serviceLifetime));
            return services;
        }


        /// <summary>
        /// Creates a <see cref="Lazy{T}"/> <see cref="ServiceDescriptor"/> for the descriptor provided.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/>containing the registration to add a lazy functionality for.</param>
        /// <param name="serviceLifetime">The <see cref="ServiceLifetime"/>> for the lazy class. Uses the source lifetime if none provided.</param>
        /// <param name="valueInstance">Instance value to use in the lazy class instead of resolving the value. </param>
        /// <param name="lazyInstance">The lazy instance to register.</param>
        /// <returns>New <see cref=" ServiceDescriptor"/> defining a lazy version of the source descriptor.</returns>
        /// <exception cref="System.ArgumentException">instance object is not {lazyServiceType.FullName} - lazyInstance</exception>
        public static IServiceCollection AsLazyAdvanced(this IServiceCollection services, ServiceLifetime? serviceLifetime = default, object? valueInstance = default, object? lazyInstance = default)
        {
            var lastService = services.LastOrDefault();
            ArgumentNullException.ThrowIfNull(lastService);
            services.Add(lastService.AsLazyAdvanced(serviceLifetime, valueInstance, lazyInstance));
            return services;
        }

    }
}