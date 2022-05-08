using API.Core.Settings;
using API.Extensions;
using API.Infrastructure.Filters;
using API.Infrastructure.Registrations;
using Microsoft.AspNetCore.Mvc;

namespace API
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
            services
               .AddHttpContextAccessor()
               .AddRouting(options => options.LowercaseUrls = true)
               .AddMvcCore(options =>
               {
                   options.Filters.Add<HttpGlobalExceptionFilter>();
                   options.Filters.Add<ValidateModelStateFilter>();
               })
               .AddApiExplorer()
               .AddDataAnnotations()
               .AddAuthorization();
            //Enable CORS
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod()
                 .AllowAnyHeader());
            });

            services.PostConfigure<ApiBehaviorOptions>(options =>
            {
                var builtInFactory = options.InvalidModelStateResponseFactory;

                options.InvalidModelStateResponseFactory = context =>
                {
                    // Get an instance of ILogger and log accordingly.

                    var loggerFactory = context.HttpContext.RequestServices
                                .GetRequiredService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger(context.ActionDescriptor.DisplayName);

                    var errorList = (from state in context.ModelState.Values
                                     from error in state.Errors
                                     select error.ErrorMessage)?.ToList();

                    logger.LogError("{ModelState}", errorList);

                    return builtInFactory(context);
                };
            });

            services.Configure<DbConnectionSettings>(Configuration.GetSection("DbConnection"));

            services.AddSwagger(Configuration);

            services.AddDatabase();
            services.AddRepositories();
            services.AddServices();

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OnlineService v1"));
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
