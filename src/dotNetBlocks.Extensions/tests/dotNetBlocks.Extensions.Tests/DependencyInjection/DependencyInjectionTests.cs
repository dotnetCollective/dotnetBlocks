using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions.Common;
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
                serviceB.Should().NotBeNull();


                serviceB!.TrueMethod().Should().BeTrue();
                serviceB.TrueProperty.Should().BeTrue();
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
                act.Should().NotThrow();

                act = () => provider.GetRequiredService<Lazy<ServiceA>>();
                act.Should().Throw< InvalidOperationException>();

                act = () => provider.GetRequiredService<ServiceB>();
                act.Should().Throw<InvalidOperationException>();


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
                serviceB.Should().NotBeNull();


                serviceB!.TrueMethod().Should().BeTrue();
                serviceB.TrueProperty.Should().BeTrue();
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
                serviceB.Should().NotBeNull();


                serviceB!.TrueMethod().Should().BeTrue();
                serviceB.TrueProperty.Should().BeTrue();
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
                lazyInstance.Should().NotBeNull();

                lazyInstance = provider.GetService<Lazy<ServiceA>>();
                lazyInstance.Should().NotBeNull();
                lazyInstance!.Value.Should().NotBeNull();

                // Ensure the lazy is transient
                provider.GetService<Lazy<ServiceA>>().Should().NotBe(lazyInstance);
                // The service must be transient
                provider.GetService<Lazy<ServiceA>>()!.Value.Should().NotBe(lazyInstance.Value);

            }
        }







    }
}