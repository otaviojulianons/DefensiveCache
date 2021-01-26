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
        public const int ERROR = 1;
        public const int SUCCESS = 0;

        static int Main(string[] args)
        {
            WriteAppName();
            if (args.Length == 0)
                return SUCCESS;

            var assemblyPath = args[0];
            var outputPath = args[1];

            Console.WriteLine("Assembly: " + assemblyPath);
            Console.WriteLine("OutputPath: " + outputPath);

            var templates = TemplateService.GetCacheTemplatesFromAssembly(assemblyPath);
            if (!templates?.Any() ?? false)
            {
                Console.WriteLine($"Nothig template was found.");
                return ERROR;
            }
            else
                Console.WriteLine($"Was found {templates.Count()} templates.");
            
            var codeTemplate = TemplateService.GenerateCacheCode(templates.ToArray());
            return WriteCodeTemplate(codeTemplate, outputPath) ? SUCCESS : ERROR;
        }

        private static void WriteAppName()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Console.WriteLine(@$"");
            Console.WriteLine(@$" ______      __               _            _____            _           ");
            Console.WriteLine(@$" |  _  \    / _|             (_)          /  __ \          | |          ");
            Console.WriteLine(@$" | | | |___| |_ ___ _ __  ___ ___   _____ | /  \/ __ _  ___| |__   ___  ");
            Console.WriteLine(@$" | | | / _ \  _/ _ \ '_ \/ __| \ \ / / _ \| |    / _` |/ __| '_ \ / _ \ ");
            Console.WriteLine(@$" | |/ /  __/ ||  __/ | | \__ \ |\ V /  __/| \__/\ (_| | (__| | | |  __/ ");
            Console.WriteLine(@$" | _ / \___|_| \___|_| |_|___/_| \_/ \___| \___ /\__,_|\__ |_| |_|\___| V{version}");
            Console.WriteLine(@$"");
        }

        private static bool WriteCodeTemplate(string codeTemplate, string outputPath)
        {
            try
            {
                string contents = null;
                var target = new FileInfo(outputPath);

                if (target.Exists)
                {
                    contents = File.ReadAllText(target.FullName, Encoding.UTF8).Trim();
                    if (string.Equals(contents, codeTemplate, StringComparison.Ordinal))
                    {
                        Console.WriteLine($"File '{outputPath}' exists!");
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
                    FileStream file = null;
                    for (int retryCount = 3; retryCount >= 0; retryCount--)
                    {
                        try
                        {
                            file = File.Open(target.FullName, FileMode.Create, FileAccess.Write, FileShare.None);
                            break;
                        }
                        catch (Exception)
                        {
                            Thread.Sleep(500);
                            if (retryCount == 0)
                                throw;
                        }
                    }

                    using (var sw = new StreamWriter(file, Encoding.UTF8))
                        sw.WriteLine(codeTemplate);

                    Console.WriteLine($"'{outputPath}' file successfully generated.");
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Generate DefensiveCache fail '{e.Message}'.");
                return false;
            }
        }
    }
}
