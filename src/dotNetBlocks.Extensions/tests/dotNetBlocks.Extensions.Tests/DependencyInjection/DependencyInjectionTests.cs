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
        public void LazyFactoryConversionTests()
        {
        }

        [TestMethod]
        public void TestDILazyConstructorInjection()
        {
            var services = new ServiceCollection();

            services.AddLazyService();
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
        public void TestDILazyConstructorInjectionWithIndividualLazyRegistration()
        {
            var services = new ServiceCollection();

            services.AddTransient<ServiceA>(); // Need to resolve the service.
            services.AddTransientLazy<ServiceA>(); // This lazyA is injected intoB.
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


        public void TestDILazyLifetimes()
        {
            var services = new ServiceCollection();

            services.AddLazyService();
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
        public void LazyDescriptorTest()
        {
            var serviceDescriptor = ServiceDescriptor.Describe(typeof(ServiceA), typeof(ServiceA),ServiceLifetime.Singleton);
            //serviceDescriptor.AsLazy();
        }

        [TestMethod]
        public void SimpleLazyTransientTests()
        {
            var services = new ServiceCollection();

            //Add the implementation classes to inject into a lazy
            services.AddTransient<ServiceA>();
            services.AddTransientLazy<ServiceA>();
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