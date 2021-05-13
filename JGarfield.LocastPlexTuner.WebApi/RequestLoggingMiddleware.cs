using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.WebApi
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        
        private readonly ILogger _logger;
        
        public static readonly string APPLICATION_DATA_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LocastPlexTuner");

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
                var divider = "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!";
                var logLine = $"[{DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz")}] Request {context.Request?.Method} {context.Request?.Path.Value} => {context.Response?.StatusCode}";
                _logger.LogWarning(divider);
                _logger.LogWarning(logLine);
                _logger.LogWarning(divider);
                var filename = Path.Combine(APPLICATION_DATA_PATH, "debug-controller-calls");
                using (var sw = new StreamWriter(filename)) {
                    await sw.WriteAsync(logLine);
                }
            }
        }
    }
}
