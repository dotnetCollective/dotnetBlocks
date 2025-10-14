using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using RT.Comb;

namespace System
{
    /// <summary>
    /// Extension classes extending functionality of existing types in the System namespace.
    /// </summary>
    public static partial class SystemExtensions
    {

        /// <summary>
        /// Wraps an instance  with a  Lazy(value)
        /// </summary>
        /// <remarks>fluent notation converting values into Lazy where its required e.g. parameters and testing.</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static  Lazy<T> AsLazy<T>(this T value) => new Lazy<T>(value);

    }
}
