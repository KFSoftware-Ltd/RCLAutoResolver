using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Reflection;

namespace KFSoftware.RCLAutoResolver
{
    public class RazorLibraryFileProvider : IFileProvider
    {
        private readonly ILogger<RazorLibraryFileProvider> _logger = null;
        private readonly Assembly[] _assemblies = null;
        private readonly IFileProvider _baseProvider = null;

        public RazorLibraryFileProvider(ILogger<RazorLibraryFileProvider> logger, IFileProvider baseProvider, Assembly[] assemblies)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _baseProvider = baseProvider ?? throw new ArgumentNullException(nameof(baseProvider));
            _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            foreach (var assembly in _assemblies)
            {
                var searchPath = $"_content/{assembly.GetName().Name}{subpath}";
                var result = _baseProvider.GetDirectoryContents(searchPath);
                if (result.Exists)
                {
                    return result;
                }
            }

            return _baseProvider.GetDirectoryContents(subpath);
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            foreach (var assembly in _assemblies)
            {
                var searchPath = $"_content/{assembly.GetName().Name}{subpath}";
                var result = _baseProvider.GetFileInfo(searchPath);
                if (result.Exists)
                {
                    return result;
                }
            }

            return _baseProvider.GetFileInfo(subpath);
        }

        public IChangeToken Watch(string filter) => _baseProvider.Watch(filter);
    }
}