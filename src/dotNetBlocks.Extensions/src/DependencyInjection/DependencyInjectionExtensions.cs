using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;


namespace dotNetBlocks.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        private readonly static Type LazyType = typeof(Lazy<>);
        private readonly static Type LazyServiceType = typeof(LazyService<>);

        /// <summary>
        /// Adds unkeyed implicit support for lazy types with the lazy service class.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection"/> service collection being extended.</param>
        /// <param name="lifetime">The lifetime.</param>
        public static void AddLazyService(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {   // Register Lazy<>, LazyService<>;
            services.Add(ServiceDescriptor.Describe(LazyType, LazyServiceType, lifetime));
        }

    }
}