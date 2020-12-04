using Microsoft.Build.Execution;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CompilerTest
{
    public static class AnalyzerService
    {
        const string programText =
@"using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace HelloWorld
{
    public class MyAttribute : Attribute
    {
        public Type TypeMap { get; set; }
    }

    [MyAttribute(TypeMap = typeof(string))]
    public class MyClass {

        public void Initialize()
        {
            Map<int>();
        }

        public IEnumerable<Type> ListTypes()
        {
            yield return typeof(int);
            yield return typeof(string);
        }

        public void Map<T>()
        {
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(""Hello, World!"");
            Map(typeof(int));
        }

        public static void Map(Type type)
        {
        }
    }
}";
        public static bool GetCacheServicesFromSyntaxTree()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);

            var references = GetReferences(@"C:\Projetos\github\DefensiveCache\CoreApp.DefensiveCache.Example\CoreApp.DefensiveCache.Example.csproj");

            var compilation = CSharpCompilation.Create("DynamicAssembly")
                .AddSyntaxTrees(tree)
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(references.Select(path => MetadataReference.CreateFromFile(path)));

            var stream = new MemoryStream();
            var emitResult = compilation.Emit(stream);

            if (!emitResult.Success)
            {
                var error = emitResult.Diagnostics.FirstOrDefault(x => x.Severity == DiagnosticSeverity.Error);
            }
            else
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

            var members = tree.GetRoot().DescendantNodes().OfType<MemberDeclarationSyntax>();
            var methods = members.Where(m => m is MethodDeclarationSyntax)
                                 .Select(m => m as MethodDeclarationSyntax)
                                 .Select(m => m.Identifier);
            var method = methods.FirstOrDefault();


            foreach (var @class in tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                if (!@class.AttributeLists.Any())
                    continue;

                IEnumerable<MethodDeclarationSyntax> methodsClass = @class.DescendantNodes()
                  .OfType<MethodDeclarationSyntax>().ToList();

                foreach (var methodClass in methodsClass)
                {
                    Console.WriteLine(methodClass.Identifier);
                    foreach (var child in methodClass.ChildNodes().OfType<InvocationExpressionSyntax>())
                    {
                        Console.WriteLine(child.Expression);
                    }
                }

                var firstMethod = @class.Members.First();
                Console.WriteLine(firstMethod.ToFullString());

                var firstAttribute = @class.AttributeLists.First().Attributes.First();
                var attributeName = firstAttribute.Name.NormalizeWhitespace().ToFullString();

                
                var firstArgument = firstAttribute.ArgumentList.Arguments.First();
                var argumentName = firstArgument.NameEquals.Name.Identifier.ValueText;
                Console.WriteLine(argumentName);
                // prints --> Test

                var argumentExpression = firstArgument.Expression.NormalizeWhitespace().ToFullString();
                Console.WriteLine(argumentExpression);
                Console.WriteLine(attributeName);
            }

            //var myClass = members.FirstOrDefault( x => x.)
            //var firstNamedArg = firstAttribute.NamedArguments[0];
            //var key = firstNamedArg.Key; // "Test"
            //var value = firstNamedArg.Value.Value; // "Hello"

            return true;
        }


        private static IEnumerable<string> GetReferences(string projectFileName)
        {
            var projectInstance = new ProjectInstance(projectFileName);
            var result = BuildManager.DefaultBuildManager.Build(
                new BuildParameters(),
                new BuildRequestData(projectInstance, new[]
                {
                    "ResolveProjectReferences",
                    "ResolveAssemblyReferences"
                }));

            IEnumerable<string> GetResultItems(string targetName)
            {
                var buildResult = result.ResultsByTarget[targetName];
                var buildResultItems = buildResult.Items;

                return buildResultItems.Select(item => item.ItemSpec);
            }

            return GetResultItems("ResolveProjectReferences")
                .Concat(GetResultItems("ResolveAssemblyReferences"));
        }


    }
}
