using dotNetBlocks.Examples.LightManager.BusinessShared.contracts;
using dotNetBlocks.Examples.LightManager.BusinessShared.models;
using dotNetBlocks.Examples.LightManager.BusinessShared.operations;
using dotNetBlocks.Examples.LightManager.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.Examples.LightManager.Business
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="dotNetBlocks.Examples.LightManager.BusinessShared.contracts.ILightManagerLogic" />
    public class LightManagerLogic : ILightManagerLogic
    {

        private IDictionary<Guid, ILightStringController> _lightStrings { get; set; }

        public LightManagerLogic(IEnumerable< ILightStringController> lightStrings)
        { 
            // Catalog the light strings provided by id;
            _lightStrings = lightStrings.ToDictionary((ls) => ls.Id);
        }
        /// <see cref="ILightManagerLogic.DiscoverLightsAsync(DiscoverLightsOperation)"/>>
        public Task<DiscoverLightsResult> DiscoverLightsAsync(DiscoverLightsOperation operation)
        {
            var lights = (from ls in _lightStrings
                          from li in ls.Value.Lights
                          select new Light()
                          {
                              LightStringId = ls.Key,
                              IsOn = li.IsActive,
                              Id = li.Id
                          });

            var result = new DiscoverLightsResult(operation)
            {
                Lights = lights.ToList(),
            };
            return Task.FromResult(result);
        }
    }
}
