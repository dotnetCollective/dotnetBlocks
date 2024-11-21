using dotNetBlocks.Examples.LightManager.Business;
using dotNetBlocks.Examples.LightManager.BusinessShared.contracts;
using dotNetBlocks.Examples.LightManager.Hardware;
using dotNetBlocks.Examples.LightManager.Tests.Hardware;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using System.Linq;

namespace dotNetBlocks.Examples.LightManager.Tests
{
    [TestClass]
    public sealed class InProcess
    {
        [TestMethod]
        public async Task DirectCallTestsAsync()
        {
            // bootstrap container

            ServiceCollection services = new();


            // Use light manager system.
            services.UseLightManager();

            // register fake light string
            services.AddSingleton<ILightStringController, FakeLightStringController>();

            using (var container = services.BuildServiceProvider())
            {
                var logic = container.GetService<ILightManagerLogic>();
                logic.Should().NotBeNull();

                var operation = new BusinessShared.operations.DiscoverLightsOperation { };


                // Send operations directly.
                var discoverResult = await logic!.DiscoverLightsAsync(operation);

                discoverResult.Should().NotBeNull();

                // Check operation and transaction continuity.
                discoverResult.CorrelationId.Should().Be(operation.CorrelationId);
                discoverResult.RelatedToId.Should().Be(operation.Id);

                // Check the results against the fake controller.
                var actualLights = container.GetService<ILightStringController>();

                actualLights.Should().NotBeNull();

                // Same number of lights
                actualLights!.Lights.Should().HaveSameCount(discoverResult.Lights);

                // Same light numbers
                actualLights!.Lights.Select(l => l.Id).Should().Equal(discoverResult.Lights.Select(l => l.Id));


                // all lights should be off
                actualLights.Lights.Should().OnlyContain(li => !li.IsActive);
                discoverResult.Lights.Should().OnlyContain(li => !li.IsOn);

                // Count same number of lights on in both.
                actualLights.Lights.Select(li => li.IsActive).Should().HaveSameCount(discoverResult.Lights.Select(li => li.IsOn));


            }
        }
    }
}