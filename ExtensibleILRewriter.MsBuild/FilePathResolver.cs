using System;
using System.Collections.Generic;
using System.IO;

namespace ExtensibleILRewriter.MsBuild
{
    internal class FilePathResolver
    {
        private readonly List<string> searchPaths = new List<string>();

        public void AddSearchPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new InvalidOperationException($"Search path must be nonnull and nonempty.");
            }

            if (!Directory.Exists(path))
            {
                throw new InvalidOperationException($"Directory with path '{path}' does not exist.");
            }

            searchPaths.Add(path);
        }

        public string ResolveFilePath(string fileName)
        {
            string resolvedPath;
            if (TryResolveFilePath(fileName, out resolvedPath))
            {
                return resolvedPath;
            }

            throw new FileNotFoundException($"Unable to find file '{fileName}' in any of search directories.");
        }

        public bool TryResolveFilePath(string fileName, out string resolvedPath)
        {
            resolvedPath = null;

            if (Path.IsPathRooted(fileName))
            {
                if (File.Exists(fileName))
                {
                    resolvedPath = fileName;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            foreach (var searchPath in searchPaths)
            {
                var path = Path.Combine(searchPath, fileName);

                if (File.Exists(path))
                {
                    resolvedPath = path;
                    return true;
                }
            }

            return false;
        }
    }
}
