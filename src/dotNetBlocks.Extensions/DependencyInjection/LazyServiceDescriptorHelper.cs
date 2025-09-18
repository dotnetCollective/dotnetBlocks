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
        #region Types and constants
        private const BindingFlags SearchBindingFlags = BindingFlags.Default | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        internal readonly static Type LazyType = typeof(Lazy<>);
        internal readonly static Type LazyServiceType = typeof(LazyService<>);
        #endregion

        #region// Lazy object helpers."
        internal static Type MakeLazyType<T>()
                    where T : class
                    => typeof(T).MakeLazyType();
        internal static Type MakeLazyType(this Type type)
            => LazyType.MakeGenericType(type);
        internal static Type MakeLazyServiceType(this Type type)
            => LazyServiceType.MakeGenericType(type);
        internal static Type MakeLazyServiceType<T>() where T : class
            => typeof(T).MakeLazyServiceType();


        internal static object ObjectToLazy(this object value) => ToLazy((dynamic)value);
        internal static Lazy<T> ToLazy<T>(this T instance) => new Lazy<T>(instance);

        #endregion // Lazy object helpers."

    }
}