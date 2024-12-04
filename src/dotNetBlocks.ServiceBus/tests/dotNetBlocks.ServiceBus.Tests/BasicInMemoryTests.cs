using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace dotNetBlocks.ServiceBus.Tests
{
    [TestClass]
    public sealed class BasicInMemoryTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var services = new ServiceCollection();

            services.AddMassTransit( cfg =>
                {
                    cfg.UsingInMemory(
                                        (ctx, cfg) =>
                                            {
                                                cfg.ConfigureEndpoints(ctx);
                                            }
                                    );
                }
            );
        }
    }
}
