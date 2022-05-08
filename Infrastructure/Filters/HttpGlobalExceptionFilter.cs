using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Infrastructure.Filters
{
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<HttpGlobalExceptionFilter> _logger;
        /// <summary>Initializes a new instance of the <see cref="HttpGlobalExceptionFilter" /> class.</summary>
        /// <param name="env">The env.</param>
        /// <param name="logger">The logger.</param>
        public HttpGlobalExceptionFilter(IWebHostEnvironment env, ILogger<HttpGlobalExceptionFilter> logger)
        {
            _env = env;
            _logger = logger;
        }
        /// <summary>Called after an action has thrown an <see cref="T:System.Exception">Exception</see>.</summary>
        /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.ExceptionContext">ExceptionContext</see>.</param>
        public void OnException(ExceptionContext context)
        {
            _logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                "{Message}", context.Exception.Message);

            var jsonErrorResponse = new ErrorResponse
            {
                Messages = new[] { "An internal error has occurred" }
            };

            if (_env.IsDevelopment())
            {
                jsonErrorResponse.Exception = context.Exception.ToString();
            }

            context.Result = new ObjectResult(jsonErrorResponse) { StatusCode = StatusCodes.Status500InternalServerError };
            context.ExceptionHandled = true;
        }
    }
}
