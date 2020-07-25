using CoreApp.DefensiveCache.Templates;
using Stubble.Core;
using Stubble.Core.Builders;
using Stubble.Core.Settings;
using System.IO;
using System.Reflection;

namespace CoreApp.DefensiveCache.Services
{
    public class TemplateService
    {
        public static RenderSettings renderSettings;
        public static string Generate(CacheTemplate model)
        {
            if(renderSettings == null)
            {
                renderSettings = RenderSettings.GetDefaultRenderSettings();
                renderSettings.SkipHtmlEncoding = true;
            }
            StubbleVisitorRenderer _stubbleTemplate = new StubbleBuilder().Build();
            return _stubbleTemplate.Render(GetTemplate(), model, renderSettings);
        }

        private static string GetTemplate()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "CoreApp.DefensiveCache.Templates._CacheProxyTemplate.mustache";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
