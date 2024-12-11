using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using System.Security.Cryptography.X509Certificates;
using MassTransit.SagaStateMachine;
using FluentAssertions.Common;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.CodeCoverage.Core.Reports.Coverage;
using System.Runtime.CompilerServices;
using MassTransit.Configuration;

namespace dotNetBlocks.ServiceBus.Tests
{

    [TestClass]
    public class DependencyInjectionTests
    {
        [TestMethod]
        public void TestDILazyConstructorInjection()
        {
            var services = new ServiceCollection();

            services.AddTransient(typeof(Lazy<>), typeof(LazyFactory<>));
            //services.AddTransient(typeof(Lazy<>), typeof(LazyService<>));
            services.AddTransient<ServiceA>();
            services.AddTransient<ServiceB>();



            using (var provider = services.BuildServiceProvider())
            {
                Lazy<ServiceA> serviceA = new LazyFactory<ServiceA>(provider);

                provider.GetService<ServiceA>();
                var serviceB = provider.GetService<ServiceB>();
                //serviceB.Should().NotBeNull();


                serviceB!.TrueMethod().Should().BeTrue();
                serviceB.TrueProperty.Should().BeTrue();
            }

        }


        public class LazyFactory<T>
            where T : class
        {
            public Lazy<T>Lazy;

            public static implicit operator Lazy<T>(LazyFactory<T> factory) => factory.Lazy;

            public LazyFactory(IServiceProvider provider)
            {
                Lazy = new Lazy<T>(() => provider.GetRequiredService<T>());
            }
        }

        public class LazyService<T> : Lazy<T> where T : class
        {
            public LazyService(IServiceScopeFactory scopeFactory)
                : base(() =>
                {
                    var scope = scopeFactory.CreateScope();
                    return scope.ServiceProvider.GetRequiredService<T>();
                })
            {
            }
        }



        public class ServiceA
        {
            // Some methods to call from the using class.
            public bool TrueProperty { get; set; } = true;
            public bool TrueMethod() => true;
        };

        public class ServiceB
        {

            public bool TrueProperty => ServiceA.TrueProperty;

            public bool TrueMethod() => ServiceA.TrueMethod();

            private Lazy<ServiceA> _serviceA;
            public ServiceA ServiceA => _serviceA.Value;

            // _lazy constructor.
            public ServiceB(Lazy<ServiceA> a)
            {
                _serviceA = a;
            }
        }

    }
}