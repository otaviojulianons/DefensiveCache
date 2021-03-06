﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
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
        public const string DynamicAssemblyName = "DynamicAssembly";

        public static Assembly GenerateAssemblyFromCode(Assembly referenceAssembly, string className, params string[] classCode)
        {
            CSharpParseOptions parseOptions = new CSharpParseOptions()
                .WithDocumentationMode(DocumentationMode.Parse)
                .WithKind(SourceCodeKind.Regular)
                .WithLanguageVersion(LanguageVersion.Latest);

            var files = classCode.Select(@class => CSharpSyntaxTree.ParseText(@class, parseOptions)).ToList();
            var referencesBuild = GetMetadataReferencesForTemplate(referenceAssembly);

            var compilation = CSharpCompilation.Create(DynamicAssemblyName,
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

        private static IEnumerable<MetadataReference> GetMetadataReferencesForTemplate(Assembly referenceAssembly)
        {
            var coreDir = Directory.GetParent(typeof(Guid).Assembly.Location).FullName;
            var netstandardLocation = Assembly.Load("netstandard").Location;

            yield return MetadataReference.CreateFromFile(referenceAssembly.Location);
            yield return MetadataReference.CreateFromFile(GetPathAssemblyFromNamespace(coreDir, "System.Runtime"));
            yield return MetadataReference.CreateFromFile(netstandardLocation);
            yield return MetadataReference.CreateFromFile(typeof(IDistributedCache).Assembly.Location);
            yield return MetadataReference.CreateFromFile(typeof(IConfiguration).Assembly.Location);
            yield return MetadataReference.CreateFromFile(typeof(ConfigurationBinder).Assembly.Location);
            yield return MetadataReference.CreateFromFile(typeof(CompilerService).Assembly.Location);
            yield return MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            yield return MetadataReference.CreateFromFile(GetPathAssemblyFromType(coreDir, typeof(object)));
            yield return MetadataReference.CreateFromFile(GetPathAssemblyFromType(coreDir, typeof(Enumerable)));
            yield return MetadataReference.CreateFromFile(GetPathAssemblyFromType(coreDir, typeof(Task)));

            foreach (var reference in referenceAssembly.GetReferencedAssemblies())
            {
                var referencePath = GetPathAssemblyFromNamespace(coreDir, reference.Name);
                if (File.Exists(referencePath))
                    yield return MetadataReference.CreateFromFile(referencePath);
            }
        }

        private static string GetPathAssemblyFromType(string coreLocation, Type type) =>
            GetPathAssemblyFromNamespace(coreLocation, type.Namespace);

        private static string GetPathAssemblyFromNamespace(string coreLocation, string @namespace) =>
            $"{coreLocation}{Path.DirectorySeparatorChar}{@namespace}.dll";

    }
}
