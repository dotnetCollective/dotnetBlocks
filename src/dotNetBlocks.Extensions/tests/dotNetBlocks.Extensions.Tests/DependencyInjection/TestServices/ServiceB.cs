using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNetBlocks.Extensions.Tests.DependencyInjection.TestServices
{

    public class ServiceB
    {

        public bool TrueProperty => ServiceA.TrueProperty;

        public bool TrueMethod() => ServiceA.TrueMethod();

        private Lazy<ServiceA> _serviceA;
        public ServiceA ServiceA => _serviceA.Value;

        // _lazy constructor.
        public ServiceB(Lazy<ServiceA> a)
        {
            _serviceA = a;
        }
    }
}