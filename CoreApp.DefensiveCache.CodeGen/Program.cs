using CoreApp.DefensiveCache.Services;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace ConsoleGenerator
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("Hello World Console Generator!");
            if (args.Length == 0)
                return 0;

            var assemblyPath = args[0];
            var outputPath = args[1];

            Console.WriteLine("ASSEMBLY __> " + assemblyPath);
            Console.WriteLine("OUTPUTPATH __> " + outputPath);

            var templates = TemplateService.GetCacheTemplatesFromAssembly(assemblyPath);
            if (!templates?.Any() ?? false)
            {
                Console.WriteLine($"TB: Nothig template was found. -> '{assemblyPath}'");
                return 0;
            }
            else
                Console.WriteLine($"TB: Was found {templates.Count()} templates.");
            
            var codeTemplate = TemplateService.GenerateCacheCode(templates.ToArray());
            return WriteCodeTemplate(codeTemplate, outputPath) ? 0 : 1;
        }

        private static bool WriteCodeTemplate(string codeTemplate, string outputPath)
        {
            try
            {
                string contents = null;
                var target = new FileInfo(outputPath);

                if (target.Exists)
                {
                    // Only try writing if the contents are different. Don't cause a rebuild
                    contents = File.ReadAllText(target.FullName, Encoding.UTF8).Trim();
                    if (string.Equals(contents, codeTemplate, StringComparison.Ordinal))
                    {
                        Console.WriteLine($"'{outputPath}' exists!");
                        return true;
                    }
                }

                if (target.Exists && target.IsReadOnly)
                {
                    if (!string.Equals(contents, codeTemplate, StringComparison.Ordinal))
                    {
                        Console.WriteLine($"File '{target}' is ReadOnly and cannot be written");
                        return false;
                    }
                }
                else
                {
                    var retryCount = 3;
                    retry:
                    FileStream file;

                    try
                    {
                        file = File.Open(target.FullName, FileMode.Create, FileAccess.Write, FileShare.None);
                    }
                    catch (Exception)
                    {
                        if (retryCount < 0)
                            throw;

                        retryCount--;
                        Thread.Sleep(500);
                        goto retry;
                    }

                    using (var sw = new StreamWriter(file, Encoding.UTF8))
                    {
                        sw.WriteLine(codeTemplate);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"GenerateStubsTask fail '{e.Message}'.");
                return false;
            }
        }
    }
}
