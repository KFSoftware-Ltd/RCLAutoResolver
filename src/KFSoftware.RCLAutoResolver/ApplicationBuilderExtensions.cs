using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;

namespace KFSoftware.RCLAutoResolver
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds an <see cref="IFileProvider">FileProvider</see> to search Razor Class Libraries for static files.
        /// </summary>
        /// <param name="assemblies">A list of assemblies to search for the requested static file.</param>
        public static IApplicationBuilder UseRCLAutoResolver(this IApplicationBuilder app, params Assembly[] assemblies)
        {
            if (assemblies is null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            using (var scope = app.ApplicationServices.GetRequiredService<IServiceProvider>().CreateScope())
            {
                IWebHostEnvironment env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
                ILoggerFactory loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                IFileProvider provider = new CompositeFileProvider(new RazorClassLibraryFileProvider(loggerFactory.CreateLogger<RazorClassLibraryFileProvider>(), env.WebRootFileProvider, assemblies), env.WebRootFileProvider);
                env.WebRootFileProvider = provider;
            }
            return app;
        }
    }
}