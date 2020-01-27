using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Market.CustomersAndStaff.GroboContainer.Core
{
    public static class AssembliesLoaderBase
    {
        public static IEnumerable<Assembly> Load(string[] assemblyPrefixes, string[] assemblyExtensions = null)
        {
            if(assemblyExtensions == null)
            {
                assemblyExtensions = new[] {".exe", ".dll"};
            }

            return EnumerateFiles()
                   .Where(x => IsOurAssembly(x, assemblyPrefixes, assemblyExtensions))
                   .Select(Load)
                   .ToArray();
        }

        private static Assembly Load(string path)
        {
            var loadFrom = Assembly.LoadFrom(path);
            try
            {
                var type = loadFrom.GetType("Costura.AssemblyLoader");
                if(type != null)
                {
                    // hack to load costured assemblies 
                    var attachMemeber = (MethodInfo)type.GetMembers().First(x => x.Name == "Attach");
                    attachMemeber.Invoke(null, new object[0]);
                }
            }
            catch
            {
                // ignored
            }

            return loadFrom;
        }

        private static IEnumerable<string> EnumerateFiles()
        {
            var result = new List<string>();
            if(!string.IsNullOrEmpty(AppDomain.CurrentDomain.RelativeSearchPath))
                result.AddRange(Directory.EnumerateFiles(AppDomain.CurrentDomain.RelativeSearchPath, "*", SearchOption.TopDirectoryOnly));
            if(!string.IsNullOrEmpty(AppDomain.CurrentDomain.BaseDirectory))
                result.AddRange(Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "*", SearchOption.TopDirectoryOnly));

            return result;
        }

        private static bool IsOurAssembly(string fullFileName, string[] assemblyPrefixes, string[] assemblyExtensions)
        {
            var fileName = Path.GetFileName(fullFileName);
            if(string.IsNullOrEmpty(fileName))
                return false;
            return assemblyExtensions.Any(ext => fileName.EndsWith(ext, StringComparison.InvariantCultureIgnoreCase)) &&
                   assemblyPrefixes.Any(pref => fileName.StartsWith(pref, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}