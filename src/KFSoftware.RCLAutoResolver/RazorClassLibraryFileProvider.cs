using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Reflection;

namespace KFSoftware.RCLAutoResolver
{
    public class RazorClassLibraryFileProvider : IFileProvider
    {
        private readonly ILogger<RazorClassLibraryFileProvider> _logger = null;
        private readonly Assembly[] _assemblies = null;
        private readonly IFileProvider _baseProvider = null;

        public RazorClassLibraryFileProvider(ILogger<RazorClassLibraryFileProvider> logger, IFileProvider baseProvider, Assembly[] assemblies)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _baseProvider = baseProvider ?? throw new ArgumentNullException(nameof(baseProvider));
            _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            IDirectoryContents result = _baseProvider.GetDirectoryContents(subpath);
            string searchPath = subpath.StartsWith("~") ? subpath.Replace("~/", "") : subpath;

            if (result?.Exists == true)
            {
                return result;
            }

            // TODO: Prioritize assemblies
            foreach (var assembly in _assemblies)
            {
                searchPath = $"_content/{assembly.GetName().Name}{searchPath}";
                result = _baseProvider.GetDirectoryContents(searchPath);
                if (result?.Exists == true)
                {
                    return result;
                }
            }

            return new NotFoundDirectoryContents();
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            IFileInfo result = _baseProvider.GetFileInfo(subpath);
            string searchPath = subpath.StartsWith("~") ? subpath.Replace("~/", "") : subpath;

            if (result?.Exists == true)
            {
                return result;
            }

            // TODO: Prioritize assemblies
            foreach (var assembly in _assemblies)
            {
                searchPath = $"_content/{assembly.GetName().Name}{subpath}";
                result = _baseProvider.GetFileInfo(searchPath);
                if (result.Exists)
                {
                    return result;
                }
            }

            return new NotFoundFileInfo(Path.GetFileName(subpath));
        }

        public IChangeToken Watch(string filter) => _baseProvider.Watch(filter);
    }
}