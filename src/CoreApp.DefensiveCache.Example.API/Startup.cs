using CoreApp.DefensiveCache.Extensions;
using CoreApp.DefensiveCache.Tests.Contracts;
using CoreApp.DefensiveCache.Serializers;
using CoreApp.DefensiveCache.Tests.Implementations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CoreApp.DefensiveCache.Example.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();

            //services.AddDistributedMemoryCache();
            services.AddDistributedRedisCache( options => {
                options.Configuration = Configuration.GetConnectionString("RedisConnection");
                options.InstanceName = "example";
            });

            services.AddScoped<ICacheSerializer, JsonNetCacheSerializer>();
            services.DecorateWithCacheServiceMapping(new CacheServiceMapping());
            services.DecorateWithCacheDynamicServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
