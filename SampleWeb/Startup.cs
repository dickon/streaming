using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Lib;
using static Lib.StructuredLogger;

namespace SampleWeb
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();

            app.Run(async (context) =>
            {
                if (context.Request.Path == "/") {
                    await StreamedAction.Go(context, async (sl) => {
                        await sl.StartSection("Example");
                        await sl.Announce("Hello world");
                        foreach (var continent in "Asia, Africa, North America, South America, Antarctica, Europe, Australia".Split(", ")) {
                            await Task.Delay(1000);
                            await sl.StartSection($"Calling {continent}");
                            await Task.Delay(1000);
                            await sl.Announce($"Hello from {continent}");
                            await sl.EndSection();
                        }
                        await sl.EndSection();
                    });             
                }
            });
        }
    }
}
