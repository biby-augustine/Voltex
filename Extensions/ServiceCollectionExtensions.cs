using API.Core.Repository;
using API.Services;

namespace API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            services.AddScoped<DBFactory>();
            return services;
        }
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
            .AddScoped(typeof(IEmployeeRepository), typeof(EmployeeRepository))
            .AddScoped(typeof(IDesignationRepository), typeof(DesignationRepository))
            .AddScoped(typeof(ICountyRepository), typeof(CountyRepository));
        }
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                .AddScoped<EmployeeService>();
        }
    }
}
