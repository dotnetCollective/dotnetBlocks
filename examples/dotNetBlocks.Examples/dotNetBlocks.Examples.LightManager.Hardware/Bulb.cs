using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.Examples.LightManager.Hardware
{
    public class Bulb
    {
        public Guid stringId { get;set; }
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}
