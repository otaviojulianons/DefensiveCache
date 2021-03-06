﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace CoreApp.DefensiveCache.Templates
{
    public class CacheTemplate
    {
        public const string NameSuffix = "DynamicCache";
        private List<CacheMethodTemplate> _methods = new List<CacheMethodTemplate>();
        private List<CachePropertyTemplate> _properties = new List<CachePropertyTemplate>();

        public CacheTemplate(Type type)
        {
            Name = type.Name + NameSuffix;
            InterfaceName = type.FullName;
        }

        public string Name { get; private set; }

        public string InterfaceName { get; private set; }

        public IReadOnlyCollection<CacheMethodTemplate> Methods => _methods;

        public IReadOnlyCollection<CachePropertyTemplate> Properties => _properties;

        public void AddMethod(CacheMethodTemplate cacheMethodTemplate) => _methods.Add(cacheMethodTemplate);

        public void AddProperty(PropertyInfo propertyInfo) =>
            _properties.Add(new CachePropertyTemplate(propertyInfo));
    }
}
