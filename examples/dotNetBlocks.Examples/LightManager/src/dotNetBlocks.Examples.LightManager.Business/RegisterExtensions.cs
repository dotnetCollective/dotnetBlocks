using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dotNetBlocks.Examples.LightManager.BusinessShared.contracts;
using Microsoft.Extensions.DependencyInjection;

namespace dotNetBlocks.Examples.LightManager.Business
{
    public static class RegisterExtensions
    {
        /// <summary>
        /// Uses the light manager system.
        /// </summary>
        /// <param name="services">The services.</param>
        public static void UseLightManager(this IServiceCollection services)
        {
            services.AddTransient<ILightManagerLogic, LightManagerLogic>();
        }
    }
}
