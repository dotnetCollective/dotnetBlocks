using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

/*
 * Solution ideas and credit references "Michael Petito" and others on this thread
 * https://stackoverflow.com/questions/44934511/does-net-core-dependency-injection-support-lazyt
 * */

namespace dotNetBlocks.Extensions.DependencyInjection
{



    /// <summary>
    /// Extends the <see cref="Lazy{T}"/> class with SI constructor support;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Lazy&lt;T&gt;" />
    /// <remarks>Is the implementation class type registered for open lazy type.</remarks>
    public class LazyService<T> : Lazy<T> where T : class
    {

        private readonly IServiceScope _scope;
        private LazyService(IServiceScope scope) : base(() => scope.ServiceProvider.GetRequiredService<T>())

        { 
            // capture scope lifetime for delayed construction of lazy value.
            _scope = scope; }
        public LazyService(IServiceScopeFactory scopeFactory) : this(scopeFactory.CreateScope())
        { }

    }



    /* For reference purposes - I did try create a lighter lazy factory, but issues with type compatibility testing and transformation prevented its use.
     * 
    public class LazyFactory<TService>
        where TService : class
    {
        public Lazy<TService> Lazy;

        public static implicit operator Lazy<TService>(LazyFactory<TService> factory) => factory.Lazy;
        public static implicit operator LazyFactory<TService>(Lazy<TService> lazy) => new LazyFactory<TService>(lazy);

        public LazyFactory(Lazy<TService> lazy) => Lazy = lazy;

        public LazyFactory(IServiceProvider provider)
        {
            Lazy = new Lazy<TService>(() => provider.GetRequiredService<TService>());
        }
    }
    */



}
