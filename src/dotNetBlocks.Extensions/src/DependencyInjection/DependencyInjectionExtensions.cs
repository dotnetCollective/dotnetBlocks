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
        /// Adds unkeyed implicit support for lazy types with <see cref="LazyService{T}"/> Support
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> service collection being extended.</param>
        /// <param name="lifetime">The lifetime.</param>
        /// <remarks>
        /// This is not a true <see cref="Lazy{T}"/> class implementation but uses a descendant service class to provide identical functionality <see cref="LazyService{T}"/>
        /// </remarks>
        public static void AddLazyService(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {   // Register Lazy<>, LazyService<>;
            services.Add(ServiceDescriptor.Describe(LazyServiceDescriptorHelper.LazyType, LazyServiceDescriptorHelper.LazyServiceType, lifetime));
        }

        /// <summary>
        /// /// Adds a <see cref="Lazy{T}"/> definition for the last <see cref="ServiceDescriptor"/> Registered in the collection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static IServiceCollection AsLazy(this IServiceCollection services)
        {
            var lastService = services.LastOrDefault();
            ArgumentNullException.ThrowIfNull(lastService);
            services.Add(lastService.AsLazy());
            return services;
        }

    }
}