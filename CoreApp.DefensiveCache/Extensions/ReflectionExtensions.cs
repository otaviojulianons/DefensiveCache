using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CoreApp.DefensiveCache.Extensions
{
    public static class ReflectionExtensions
    {
        public static T CreateInstanceDecorated<T>(Type type, params object[] args) =>
            (T)Activator.CreateInstance(type, args);

        public static string GetDeclaration(this ParameterInfo parameterInfo) =>
            $"{parameterInfo.ParameterType.FullName} {parameterInfo.Name}";

        public static string GetDeclarationType(this Type type)
        {
            var genereciArguments = type.GetGenericArguments();
            if (!genereciArguments.Any())
                return type.FullName;
            var genericDeclarationTypes = genereciArguments.Select(x => x.GetDeclarationType());
            var originalName = type.FullName.Split('`').FirstOrDefault();
            return $"{originalName}<{string.Join(",", genericDeclarationTypes)}>"; 
        }

        public static IEnumerable<MethodInfo> GetAllMethods(this Type type)
        {
            foreach (var method in type.GetMethods())
                yield return method;
            foreach (var @interface in type.GetInterfaces())
                foreach (var method in @interface.GetAllMethods())
                    yield return method;
            if (type.BaseType != null)
                foreach (var method in type.BaseType.GetAllMethods())
                    yield return method;
        }
    }
}
