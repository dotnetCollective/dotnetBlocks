using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.Extensions.Tests.DependencyInjection.TestServices
{
    public class ServiceA
    {
        // Some methods to call from the using class.
        public bool TrueProperty { get; set; } = true;
        public bool TrueMethod() => true;
    }
}
