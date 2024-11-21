using dotNetBlocks.Business.Shared;
using dotNetBlocks.Examples.LightManager.BusinessShared.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.Examples.LightManager.BusinessShared.operations
{
    public class DiscoverLightsResult : Result
    {
        public DiscoverLightsResult()
        {
        }

        public DiscoverLightsResult(Event relatedTo) : base(relatedTo)
        {
        }

        public DiscoverLightsResult(BusinessActivity relatedTo) : base(relatedTo)
        {
        }

        public IEnumerable<Light> Lights {  get; set; } = new List<Light>();
    }
}
