using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CoreApp.DefensiveCache.Services
{
    public class CompilerService
    {
        public static Assembly GenerateAssemblyFromCode(Assembly referenceAssembly, string className, params string[] classCode)
        {
            CSharpParseOptions parseOptions = new CSharpParseOptions()
                .WithDocumentationMode(DocumentationMode.Parse)
                .WithKind(SourceCodeKind.Regular)
                .WithLanguageVersion(LanguageVersion.Latest);

            var files = classCode.Select(@class => CSharpSyntaxTree.ParseText(@class, parseOptions)).ToList();
            var referencesBuild = new List<MetadataReference>();

            var coreDir = Directory.GetParent(typeof(Guid).Assembly.Location).FullName;
            var netstandardLocation = Assembly.Load("netstandard").Location;

            referencesBuild.Add(MetadataReference.CreateFromFile(referenceAssembly.Location));
            referencesBuild.Add(MetadataReference.CreateFromFile(GetPathAssemblyFromNamespace(coreDir, "System.Runtime")));
            referencesBuild.Add(MetadataReference.CreateFromFile(netstandardLocation));
            referencesBuild.Add(MetadataReference.CreateFromFile(typeof(IDistributedCache).Assembly.Location));
            referencesBuild.Add(MetadataReference.CreateFromFile(typeof(CompilerService).Assembly.Location));
            referencesBuild.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            referencesBuild.Add(MetadataReference.CreateFromFile(GetPathAssemblyFromType(coreDir, typeof(object))));
            referencesBuild.Add(MetadataReference.CreateFromFile(GetPathAssemblyFromType(coreDir, typeof(Enumerable))));
            referencesBuild.Add(MetadataReference.CreateFromFile(GetPathAssemblyFromType(coreDir, typeof(Task))));

            foreach (var reference in referenceAssembly.GetReferencedAssemblies())
            {
                var referencePath = GetPathAssemblyFromNamespace(coreDir, reference.Name);
                if (File.Exists(referencePath))
                    referencesBuild.Add(MetadataReference.CreateFromFile(referencePath));
            }

            var compilation = CSharpCompilation.Create("DynamicAssembly",
                              files,
                              referencesBuild,
                              new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var stream = new MemoryStream();
            var emitResult = compilation.Emit(stream);

            if (!emitResult.Success)
            {
                var error = emitResult.Diagnostics.FirstOrDefault(x => x.Severity == DiagnosticSeverity.Error);
                throw new Exception($"Failed to compile cache code {className}. {error}");
            }
            else
            {
                stream.Seek(0, SeekOrigin.Begin);
                return Assembly.Load(stream.GetBuffer());
            }

        }

        private static string GetPathAssemblyFromType(string coreLocation, Type type) =>
            GetPathAssemblyFromNamespace(coreLocation, type.Namespace);

        private static string GetPathAssemblyFromNamespace(string coreLocation, string @namespace) =>
            $"{coreLocation}{Path.DirectorySeparatorChar}{@namespace}.dll";
    }
}
