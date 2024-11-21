using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.Examples.LightManager.Hardware
{
    public interface ILightStringController
    {

        public Guid Id { get; set; }

        public IEnumerable<Bulb> Lights { get; }
        public Bulb GetLightById(int lightId);
    }
}
