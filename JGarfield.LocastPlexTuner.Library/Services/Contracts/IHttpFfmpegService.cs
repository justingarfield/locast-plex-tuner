using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services.Contracts
{
    public interface IHttpFfmpegService
    {
        Task StreamToHttpResponseAsync(Uri streamUri, HttpContext httpContext);
    }
}
