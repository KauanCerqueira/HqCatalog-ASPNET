using HqCatalog.Business.Interfaces;
using HqCatalog.Business.Service;
using HqCatalog.Business.Services;
using HqCatalog.Data.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace HqCatalog.Api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddScoped<IHqRepository, HqRepository>(); // Repositório de HQs
            services.AddScoped<IHqService, HqService>(); // Serviço que precisa ser injetado no HqController
            return services;
        }
    }
}
