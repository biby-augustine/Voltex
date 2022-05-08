using Microsoft.OpenApi.Models;
namespace API.Infrastructure.Registrations
{
    public static class SwaggerRegistration
    {

        /// <summary>Adds the swagger.</summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        public static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddSwaggerGen(swaggerOptions =>
            {
                swaggerOptions.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Voltex Interface Api",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Name = "Biby Augustine",
                        Url = new Uri("https://www.voltex.com/"),
                    }
                });

                swaggerOptions.OrderActionsBy(x => x.RelativePath);
            });
        }
    }
}
