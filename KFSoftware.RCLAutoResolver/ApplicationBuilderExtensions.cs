﻿using Microsoft.AspNetCore.Builder;
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
        public static IApplicationBuilder UseRazorLibraryResolver(this IApplicationBuilder app, params Assembly[] assemblies)
        {
            if (assemblies is null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            using (var scope = app.ApplicationServices.GetRequiredService<IServiceProvider>().CreateScope())
            {
                IWebHostEnvironment env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
                ILoggerFactory loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
                IFileProvider provider = new CompositeFileProvider(new RazorLibraryFileProvider(loggerFactory.CreateLogger<RazorLibraryFileProvider>(), env.WebRootFileProvider, assemblies), env.WebRootFileProvider);
                env.WebRootFileProvider = provider;
                app.UseStaticFiles();
            }
            return app;
        }
    }
}