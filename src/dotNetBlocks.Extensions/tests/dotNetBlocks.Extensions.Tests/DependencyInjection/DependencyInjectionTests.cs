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






    }
}