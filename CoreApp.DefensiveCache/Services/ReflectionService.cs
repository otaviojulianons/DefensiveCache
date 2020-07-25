using CoreApp.DefensiveCache.Configuration;
using CoreApp.DefensiveCache.Extensions;
using CoreApp.DefensiveCache.Templates;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApp.DefensiveCache.Services
{
    public class ReflectionService
    {
        public static CacheTemplate GetTemplateModel(Type type, InterfaceCacheConfiguration cacheConfiguration)
        {
            var model = new CacheTemplate();
            model.Name = type.Name + "DynamicCache";
            model.InterfaceName = type.FullName;

            foreach (var method in type.GetAllMethods())
            {
                var methodConfiguration = cacheConfiguration.Methods.FirstOrDefault(x => x.Name == method.Name);
                var cacheMethod = new CacheMethodTemplate(method, methodConfiguration);
                model.AddMethod(cacheMethod);
            };
            return model;
        }
    }
}
