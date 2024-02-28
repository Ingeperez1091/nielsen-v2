using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NielsenChannelsReporting.Application.Abstractions;
using NielsenChannelsReporting.Infrastructure.Persistence.Contexts;
using NielsenChannelsReporting.Infrastructure.Persistence.UnitOfWorks;
using System.Reflection;

namespace NielsenChannelsReporting.Infrastructure.Persistence
{
    public static class PersistenceDI
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, string connectionString)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddDbContext<UniversalContext>(options => options
                .UseSqlServer(connectionString)
                .EnableDetailedErrors());

            services.AddScoped<IUniversalUnitOfWork, UniversalUnitOfWork>();
            return services;
        }
    }
}
