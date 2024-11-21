using dotNetBlocks.Examples.LightManager.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.Examples.LightManager.Tests.Hardware
{

    /// <summary>
    /// Fake implementation of a light controller representing a light string controller by hardware.
    /// </summary>
    /// <seealso cref="dotNetBlocks.Examples.LightManager.Hardware.ILightStringController" />
    public class FakeLightStringController : ILightStringController
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        private List<Bulb> _lights = new List<Bulb>()
            {
                new Bulb() { Id = 1, IsActive = false },
                new Bulb() { Id = 2, IsActive = false },
                new Bulb() { Id = 3, IsActive = false },
                new Bulb() { Id = 4, IsActive = false }
            };

        public IEnumerable<Bulb> Lights => _lights;

        public Bulb GetLightById(int lightId) => _lights.Single(l => l.Id == lightId);

    }
}
