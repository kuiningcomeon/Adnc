using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Autofac;
using Adnc.Infra.Consul;
using Adnc.Infra.EventBus;
using Adnc.Whse.Application.EventSubscribers;
using Adnc.WebApi.Shared;
using Microsoft.AspNetCore.Authorization;

namespace Adnc.Whse.WebApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly ServiceInfo _serviceInfo;

        public Startup(IConfiguration configuration
            , IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
            _serviceInfo = ServiceInfo.Create(Assembly.GetExecutingAssembly());
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAdncServices<PermissionHandlerRemote>(_configuration, _environment, _serviceInfo, (registion) =>
            {
                registion.AddEventBusSubscribers(EbConsts.CapTableNamePrefix, EbConsts.CapDefaultGroup, (srv) =>
                {
                    srv.AddScoped<OrderCreatedEventSubscirber>();
                });
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterAdncModules(_configuration,_serviceInfo);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAdncMiddlewares();

            if (_environment.IsProduction() || _environment.IsStaging())
            {
                app.RegisterToConsul();
            }
        }
    }
}