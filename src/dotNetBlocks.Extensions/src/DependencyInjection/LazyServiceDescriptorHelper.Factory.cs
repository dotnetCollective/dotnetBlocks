using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.Extensions.DependencyInjection
{
	internal static partial class LazyServiceDescriptorHelper

	{

		#region Types and constants

		private readonly static MethodInfo BuildlazyFactoryMethod = typeof(LazyServiceDescriptorHelper).GetMethod(nameof(BuildLazyFactory),SearchBindingFlags) ?? throw new InvalidOperationException();
        private readonly static MethodInfo BuildlazyFactoryMethodKeyed = typeof(LazyServiceDescriptorHelper).GetMethod(nameof(BuildLazyFactoryKeyed), SearchBindingFlags) ?? throw new InvalidOperationException();
        #endregion
        internal static Func<IServiceProvider, object> CreateLazyFactoryByType(Type serviceType, Type? implementationType = default, object? lazyInstance = default, object? valueInstance = default, Func<IServiceProvider, object?>? valuefactoryMethod = default)
        {
            // unless specified, implementation and service type are the same.
            if (implementationType == default) implementationType = serviceType;

            // call the generic factory constructor.
            return BuildlazyFactoryMethod.MakeGenericMethod(serviceType, implementationType).Invoke(null, new object?[] { lazyInstance, valueInstance, valuefactoryMethod }) as Func<IServiceProvider, object> ?? throw new InvalidOperationException();
        }

        internal static Func<IServiceProvider, object?, object> CreateLazyFactorybyTypeKeyed(Type serviceType, Type? implementationType = default, object? lazyInstance = default, object? valueInstance = default,
			Func<IServiceProvider, object?, object>? valuefactoryMethod = default
			)
		{
			// unless specified, implementation and service type are the same.
			if (implementationType == default) implementationType = serviceType;
			return BuildlazyFactoryMethodKeyed.MakeGenericMethod(serviceType, implementationType).Invoke(null, new object?[] { lazyInstance, valueInstance, valuefactoryMethod }) as Func<IServiceProvider, object?, object> ?? throw new InvalidOperationException();
		}



        internal static Func<IServiceProvider, object> BuildLazyFactory<TService, TImplementation>(Lazy<TService>? lazyInstance = default, TService? valueInstance = default, Func<IServiceProvider, object?>? valuefactoryMethod = default)
			where TService : class
			where TImplementation : class, TService
		{
			if (lazyInstance != default)
				// the factory always returns the provided lazy instance.
				return (IServiceProvider sp) => lazyInstance as object;

			if (valueInstance != default) // return Lazy(valueInstance);
				valuefactoryMethod = (IServiceProvider sp) => new Lazy<TService>(valueInstance);

			// set the service factory to resolve the internal value using different methods.
			// resolve the instance from the service provider.
			if (typeof(TImplementation) == typeof(TService))
				valuefactoryMethod = (IServiceProvider sp) => sp.GetRequiredService<TService>();
			else // Did we receive an implementation type different from our service?
				valuefactoryMethod = (IServiceProvider sp) => sp.GetRequiredService<TImplementation>();
			// Return the 
			return (IServiceProvider serviceProvider) =>
				new Lazy<TService>(() => (TService)valuefactoryMethod(serviceProvider.CreateScope().ServiceProvider)!);
		}


        internal static Func<IServiceProvider, object?, object> BuildLazyFactoryKeyed<TService, TImplementation>(Lazy<TService>? lazyInstance = default, TService? valueInstance = default, Func<IServiceProvider, object?, object?>? valuefactoryMethod = default)
			where TService : class
			where TImplementation : class, TService
		{
			if (lazyInstance != default)
				// the factory always returns the provided lazy instance.
				return (IServiceProvider sp, object? serviceKey) => lazyInstance as object;

			if (valueInstance != default) // return Lazy(valueInstance);
				valuefactoryMethod = (IServiceProvider sp, object? serviceKey) => new Lazy<TService>(valueInstance);

			// set the service factory to resolve the internal value using different methods.
			// resolve the instance from the service provider.
			if (typeof(TImplementation) == typeof(TService))
				valuefactoryMethod = (IServiceProvider sp, object? serviceKey) => sp.GetRequiredKeyedService<TService>(serviceKey);
			else // Did we receive an implementation type different from our service?
				valuefactoryMethod = (IServiceProvider sp, object? serviceKey) => sp.GetRequiredKeyedService<TImplementation>(serviceKey);
			// Return the 
			return (IServiceProvider serviceProvider, object? serviceKey) =>
				new Lazy<TService>(() => (TService)valuefactoryMethod(serviceProvider.CreateScope().ServiceProvider, serviceKey)!);
		}


    }
}
