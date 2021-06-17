using JGarfield.LocastPlexTuner.Domain;
using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services.Contracts
{
    public interface IHttpFfmpegService
    {
        Task StreamToHttpResponseAsync(StreamDetails stationStreamDetails, HttpContext httpContext, CancellationToken cancellationToken);
    }
}
