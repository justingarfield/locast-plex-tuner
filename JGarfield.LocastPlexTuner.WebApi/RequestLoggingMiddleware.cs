using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.WebApi
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        
        private readonly ILogger _logger;
        
        public RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            finally
            {
                var feature = context.Features.Get<IHttpConnectionFeature>();
                var divider = "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!";
                var logLine = $"[{DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz")}] Request {context.Request?.Method} {context.Request?.Path.Value} => {context.Response?.StatusCode}";
                _logger.LogWarning(divider);
                _logger.LogWarning(logLine);
                _logger.LogWarning($"{feature.RemoteIpAddress}");
                _logger.LogWarning(divider);
            }
        }
    }
}
