using dotNetBlocks.Examples.LightManager.BusinessShared.operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.Examples.LightManager.BusinessShared.contracts
{
    public interface ILightManagerLogic
    {
        public Task<DiscoverLightsResult> DiscoverLightsAsync (DiscoverLightsOperation operation);
    }
}
