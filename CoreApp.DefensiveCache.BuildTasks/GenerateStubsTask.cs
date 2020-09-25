using System;
using System.IO;
using System.Text;
using System.Threading;
using CoreApp.DefensiveCache.Configuration.Core;
using CoreApp.DefensiveCache.Templates.Core;
using CoreApp.DefensiveCache.Tests.Contracts;
using Microsoft.Build.Framework;
using CoreApp.DefensiveCache.Services.Core;

namespace CoreApp.DefensiveCache.BuildTasks
{
    public class GenerateStubsTask : Microsoft.Build.Utilities.Task
    {
        [Required]
        public string BaseDirectory { get; set; }

        [Required]
        public string OutputFile { get; set; }

        public override bool Execute()
        {
            Log.LogWarning($"GenerateStubsTask '{OutputFile}' on path '{BaseDirectory}'");
            
            var cacheCode = typeof(IProductRepository).GenerateCacheCodeFromType(new InterfaceCacheConfiguration());
            string template = cacheCode;
            //var data = DateTime.Now.ToString();
            //string template = "public static class Compilacao { public const string Data = \"" + data + "\"; }";
            string contents = null;
            try
            {
                var target = new FileInfo(OutputFile);

                if (target.Exists)
                {
                    // Only try writing if the contents are different. Don't cause a rebuild
                    contents = File.ReadAllText(target.FullName, Encoding.UTF8).Trim();
                    if (string.Equals(contents, template, StringComparison.Ordinal))
                    {
                        Log.LogWarning($"'{OutputFile}' exists on '{BaseDirectory}'");
                        return true;
                    }
                }

                // If the file is read-only, we might be on a build server. Check the file to see if 
                // the contents match what we expect
                if (target.Exists && target.IsReadOnly)
                {
                    if (!string.Equals(contents, template, StringComparison.Ordinal))
                    {
                        Log.LogError($"File '{target}' is ReadOnly and cannot be written");
                        return false;
                    }
                }
                else
                {
                    var retryCount = 3;
                    retry:
                    FileStream file;

                    // NB: Parallel build weirdness means that we might get >1 person 
                    // trying to party on this file at the same time.
                    try
                    {
                        file = File.Open(target.FullName, FileMode.Create, FileAccess.Write, FileShare.None);
                    }
                    catch (Exception)
                    {
                        if (retryCount < 0)
                        {
                            throw;
                        }

                        retryCount--;
                        Thread.Sleep(500);
                        goto retry;
                    }

                    using (var sw = new StreamWriter(file, Encoding.UTF8))
                    {
                        sw.WriteLine(template);
                    }
                }
            }
            catch (Exception e)
            {
                Log.LogWarning($"GenerateStubsTask fail '{e.Message}'.");
                Log.LogErrorFromException(e);
                return false;
            }

            return true;
        }


        private CacheTemplate GetCacheTemplate()
        {

            var type = typeof(IProductRepository);
            var model = new CacheTemplate(type);

            var getProduct = type.GetMethod(nameof(IProductRepository.GetProduct));
            var getProductAsync = type.GetMethod(nameof(IProductRepository.GetProductAsync));

            var getProductConfig = new MethodCacheConfiguration()
            {
                Name = getProduct.Name,
                ExpirationSeconds = 60,
                KeyTemplate = "product_{id}"
            };
            var getProductAsyncConfig = new MethodCacheConfiguration()
            {
                Name = getProduct.Name,
                ExpirationSeconds = 60,
                KeyTemplate = "product_async_{id}"
            };

            model.AddMethod(new CacheMethodTemplate(getProduct, getProductConfig));
            model.AddMethod(new CacheMethodTemplate(getProductAsync, getProductAsyncConfig));

            return model;
        }
    }
}
