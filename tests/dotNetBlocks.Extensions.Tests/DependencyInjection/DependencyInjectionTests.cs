using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.CodeCoverage.Core.Reports.Coverage;
using System.Runtime.CompilerServices;
using dotNetBlocks.Extensions.DependencyInjection;
using dotNetBlocks.Extensions.Tests.DependencyInjection.TestServices;

namespace dotNetBlocks.Extensions.Tests.DependencyInjection
{


    [TestClass]
    public class DependencyInjectionTests
    {


        [TestMethod]
        public void TestDILazyConstructorInjectionusingLazyServiceSupport()
        {
            var services = new ServiceCollection();

            services.AddLazyServiceSupport();
            services.AddTransient<ServiceA>();
            services.AddTransient<ServiceB>();


            using (var provider = services.BuildServiceProvider())
            {
                provider.GetService<ServiceA>();
                var serviceB = provider.GetService<ServiceB>();
                serviceB.ShouldNotBeNull();


                serviceB!.TrueMethod().ShouldBeTrue();
                serviceB.TrueProperty.ShouldBeTrue();
            }

        }


        [TestMethod]
        public void EnsureLazyFailureTestswithoutLazySupport()
        {
            var services = new ServiceCollection();

            services.AddTransient<ServiceA>(); // Need to resolve the service.
            services.AddTransient<ServiceB>(); // Need to resolve the service.

            using (var provider = services.BuildServiceProvider())
            {
                Action act = () => provider.GetRequiredService<ServiceA>();
                Should.NotThrow(act);

                act = () => provider.GetRequiredService<Lazy<ServiceA>>();
                Should.Throw< InvalidOperationException>(act);

                act = () => provider.GetRequiredService<ServiceB>();
                Should.Throw<InvalidOperationException>(act);


            }
        }


        [TestMethod]
        public void TestDILazyConstructorInjectionWithIndividualLazyRegistration()
        {
            var services = new ServiceCollection();

            services.AddTransient<ServiceA>().AsLazy(); // Need to resolve the service.
            services.AddTransient<ServiceB>();



            using (var provider = services.BuildServiceProvider())
            {
                provider.GetService<ServiceA>();
                var serviceB = provider.GetService<ServiceB>();
                serviceB.ShouldNotBeNull();


                serviceB!.TrueMethod().ShouldBeTrue();
                serviceB.TrueProperty.ShouldBeTrue();
            }

        }


        [TestMethod]
        public void TestDILazyLifetimes()
        {
            var services = new ServiceCollection();

            services.AddLazyServiceSupport();
            services.AddTransient<ServiceA>();
            services.AddTransient<ServiceB>();


            using (var provider = services.BuildServiceProvider())
            {
                provider.GetService<ServiceA>();
                var serviceB = provider.GetService<ServiceB>();
                serviceB.ShouldNotBeNull();


                serviceB!.TrueMethod().ShouldBeTrue();
                serviceB.TrueProperty.ShouldBeTrue();
            }

        }


        [TestMethod]
        public void SimpleLazyTransientTests()
        {
            var services = new ServiceCollection();

            //Add the implementation classes to inject into a lazy
            services.AddTransient<ServiceA>().AsLazy();
            using (var provider = services.BuildServiceProvider())
            {

                // Get the lazy and make sure its functional
                var lazyInstance = provider.GetRequiredService<Lazy<ServiceA>>();
                lazyInstance.ShouldNotBeNull();

                lazyInstance = provider.GetService<Lazy<ServiceA>>();
                lazyInstance.ShouldNotBeNull();
                lazyInstance!.Value.ShouldNotBeNull();

                // Ensure the lazy is transient
                provider.GetService<Lazy<ServiceA>>().ShouldNotBe(lazyInstance);
                // The service must be transient
                provider.GetService<Lazy<ServiceA>>()!.Value.ShouldNotBe(lazyInstance.Value);

            }
        }







    }
}