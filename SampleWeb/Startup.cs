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
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                await StreamedAction.Go(context.Response, async () => {
                    await StartSection("Section");
                    await Announce("Hello world");
                    foreach (var continent in "Asia, Africa, North America, South America, Antarctica, Europe, Australia".Split(", ")) {
                        await Task.Delay(1000);
                        await StartSection($"Talking to {continent}");
                        await Task.Delay(1000);
                        await Announce($"Hello {continent}");
                        await EndSection();
                    }
                    await EndSection();
                });             
            });
        }
    }
}
