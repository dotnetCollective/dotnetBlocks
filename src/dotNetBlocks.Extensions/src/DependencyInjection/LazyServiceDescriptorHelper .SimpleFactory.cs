using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.Extensions.DependencyInjection
{
    internal static partial class LazyServiceDescriptorHelper

    {
        #region Types and constants

        private readonly static MethodInfo BuildSimpleLazyFactoryMethod = typeof(LazyServiceDescriptorHelper).GetMethod(nameof(BuildSimpleLazyFactory), SearchBindingFlags) ?? throw new InvalidOperationException();
        private readonly static MethodInfo BuildSimpleLazyFactoryKeyedMethod = typeof(LazyServiceDescriptorHelper).GetMethod(nameof(BuildSimpleLazyFactoryKeyed), SearchBindingFlags) ?? throw new InvalidOperationException();
        #endregion

        internal static Func<IServiceProvider, object> BuildSimpleLazyFactoryByType(Type serviceType)
        {
            ArgumentNullException.ThrowIfNull(serviceType);
            // call the generic factory constructor.
            return BuildSimpleLazyFactoryMethod.MakeGenericMethod(serviceType).Invoke(default, default) as Func<IServiceProvider, object> ?? throw new InvalidOperationException();
        }
        internal static Func<IServiceProvider, object> BuildSimpleLazyFactory<TService>()
            where TService : class
            => (IServiceProvider sp)
                => new Lazy<TService>(sp.CreateScope().ServiceProvider.GetRequiredService<TService>);



        internal static Func<IServiceProvider, object?, object> BuildSimpleLazyFactoryByTypeKeyed(Type serviceType)
        {
            ArgumentNullException.ThrowIfNull(serviceType);

            // call the generic factory constructor.
            return BuildSimpleLazyFactoryKeyedMethod.MakeGenericMethod(serviceType).Invoke(default, default) as Func<IServiceProvider, object?, object> ?? throw new InvalidOperationException();
        }

        internal static Func<IServiceProvider, object?, object> BuildSimpleLazyFactoryKeyed<TService>()
    where TService : class
    => (IServiceProvider sp, object? serviceKey)
        => new Lazy<TService>(() => sp.CreateScope().ServiceProvider.GetRequiredKeyedService<TService>(serviceKey));


    }
}
