using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dotNetBlocks.Business.Shared;

namespace dotNetBlocks.Examples.LightManager.BusinessShared.models
{
    /// <summary>
    /// Defines a light
    /// </summary>
    /// <remarks>
    /// each light has a hard coded integer identity value
    /// typically integer ids are used for database storage performance
    /// but we are keeping this example simple and stateless.
    /// </remarks>
    public class Light : GlobalKeyedEntity<int>
    {
        public Guid LightStringId { get; set; }
        public bool IsOn { get; set; }
    }
}
