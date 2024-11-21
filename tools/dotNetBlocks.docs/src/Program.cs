using System;
using System.Globalization;
using System.Threading.Tasks;
using Devlead.Statiq.Themes;
using Statiq.App;
using Statiq.Web;

namespace dotNetBlocks.docs
{

    /// <remarks>
    /// https://www.statiq.dev/
    /// 
    /// </remarks>
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            // Ensure consistent date handling
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");

            return await Bootstrapper
                .Factory
                .CreateDefault(args)
                // This additional functionality pulls in the latest version of the themes site once running.
                //.AddThemeFromUri(new Uri("https://github.com/statiqdev/CleanBlog/archive/refs/heads/main.zip"))

                .AddWeb()
                .RunAsync();
        }
    }
}