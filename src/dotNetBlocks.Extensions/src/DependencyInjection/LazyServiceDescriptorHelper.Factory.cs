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
		private const BindingFlags SearchBindingFlags = BindingFlags.Default | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic|BindingFlags.Static;
		private readonly static Type LazyType = typeof(Lazy<>);
		private readonly static Type LazyServiceType = typeof(LazyService<>);		

		private readonly static MethodInfo createlazyFactoryMethod = typeof(LazyServiceDescriptorHelper).GetMethod(nameof(CreateLazyFactory),SearchBindingFlags) ?? throw new InvalidOperationException();
		private readonly static MethodInfo createlazyFactoryMethodKeyed = typeof(LazyServiceDescriptorHelper).GetMethod(nameof(CreateLazyFactoryKeyed), SearchBindingFlags) ?? throw new InvalidOperationException();
		#endregion

		private static Func<IServiceProvider, object> CreateLazyFactoryByType(Type serviceType, Type? implementationType = default, object? lazyInstance = default, object? valueInstance = default, Func<IServiceProvider, object?>? valuefactoryMethod = default)
		{
			// unless specified, implementation and service type are the same.
			if (implementationType == default) implementationType = serviceType;

			// call the generic factory constructor.
			return createlazyFactoryMethod.MakeGenericMethod(serviceType, implementationType).Invoke(null, new object?[] { lazyInstance, valueInstance, valuefactoryMethod }) as Func<IServiceProvider, object> ?? throw new InvalidOperationException();
		}
		private static Func<IServiceProvider, object?, object> CreateLazyFactorybyTypeKeyed(Type serviceType, Type? implementationType = default, object? lazyInstance = default, object? valueInstance = default,
			Func<IServiceProvider, object?, object>? valuefactoryMethod = default
			)
		{
			// unless specified, implementation and service type are the same.
			if (implementationType == default) implementationType = serviceType;
			return createlazyFactoryMethodKeyed.MakeGenericMethod(serviceType, implementationType).Invoke(null, new object?[] { lazyInstance, valueInstance, valuefactoryMethod }) as Func<IServiceProvider, object?, object> ?? throw new InvalidOperationException();
		}
		private static Func<IServiceProvider, object> CreateLazyFactory<TService, TImplementation>(Lazy<TService>? lazyInstance = default, TService? valueInstance = default, Func<IServiceProvider, object?>? valuefactoryMethod = default)
			where TService : class
			where TImplementation : class
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

		private static Func<IServiceProvider, object?, object> CreateLazyFactoryKeyed<TService, TImplementation>(Lazy<TService>? lazyInstance = default, TService? valueInstance = default, Func<IServiceProvider, object?, object?>? valuefactoryMethod = default)
			where TService : class
			where TImplementation : class
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

        // Lazy object helpers."
        private static Type MakeLazyType<T>()
                    where T : class
                    => typeof(T).MakeLazyType();
        private static Type MakeLazyType(this Type type) 
			=> LazyType.MakeGenericType(type);
        private static Type MakeLazyServiceType(this Type type) 
			=> LazyServiceType.MakeGenericType(type);
		private static Type MakeLazyServiceType<T>() where T : class 
			=> typeof(T).MakeLazyServiceType();

        
        private static object ObjectToLazy(this object value) => ToLazy((dynamic)value);
        private static Lazy<T> ToLazy<T>(this T instance) => new Lazy<T>(instance);
    }
}
