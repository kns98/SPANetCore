// Filename: Program.cs

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using RazorLight;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddHttpClient();
        services.AddSingleton<IRazorLightEngine>(new RazorLightEngineBuilder()
            .UseMemoryCachingProvider()
            .Build());
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapGet("/", async context =>
            {
                await ExecuteRazorAsync(context);
            });
        });
    }

    private async Task ExecuteRazorAsync(HttpContext context)
    {
        var engine = context.RequestServices.GetRequiredService<IRazorLightEngine>();

        var model = new
        {
            Message = "Hello, Spa.NET Core App!",
            Items = new List<string> { "Item 1", "Item 2", "Item 3" }
        };

        var template = @"
            <!DOCTYPE html>
            <html>
            <head>
                <title>Spa.NET Core App</title>
            </head>
            <body>
                <h1>@Model.Message</h1>
                <ul>
                    @foreach (var item in Model.Items)
                    {
                        <li>@item</li>
                    }
                </ul>
            </body>
            </html>
        ";

        var result = await engine.CompileRenderStringAsync("templateKey", template, model);
        await context.Response.WriteAsync(result);
    }
}
