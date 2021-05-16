using JGarfield.LocastPlexTuner.Library.Services.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace JGarfield.LocastPlexTuner.Library.Services
{
    public class HttpFfmpegService : IHttpFfmpegService
    {
        private readonly ILogger<HttpFfmpegService> _logger;

        public HttpFfmpegService(ILogger<HttpFfmpegService> logger)
        {
            _logger = logger;
        }

        public async Task StreamToHttpResponseAsync(Uri streamUri, HttpContext httpContext)
        {
            Process process = null;
            try
            {
                var processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = Constants.FFMPEG_BINARY;

                // Notes for ffmpeg arguments:
                //  -c copy             = Copy all streams as-is (we don't need to re-encode, just copy and pass it through)
                //  -f mpegts           = Force output file format (this is the format that the Locast M3U playlists point to)
                //  -nostats            = Don't output the encoding statistics (it'll mess up the StandardOutput stream data)
                //  -hide_banner        = Don't output the standard banner, copyrights, etc. (it'll mess up the StandardOutput stream data)
                //  -loglevel warning   = Show all warnings and errors. Any message related to possibly incorrect or unexpected events will be shown.
                //  pipe:1              = Use Pipes for output
                processStartInfo.Arguments = $"-i {streamUri} -c copy -f mpegts -nostats -hide_banner -loglevel warning pipe:1";

                // We want all of the output being piped from ffmpeg to go directly to stdout since we're feeding that raw into the HttpResponse Body's stream.
                processStartInfo.RedirectStandardOutput = true;

                process = Process.Start(processStartInfo);

                var videoData = new byte[65536];
                var bytesRead = await process.StandardOutput.BaseStream.ReadAsync(videoData);

                while (!httpContext.RequestAborted.IsCancellationRequested)
                {
                    if (bytesRead == 0)
                    {
                        break;
                    }
                    else
                    {
                        await httpContext.Response.Body.WriteAsync(videoData, 0, bytesRead, httpContext.RequestAborted);
                        bytesRead = await process.StandardOutput.BaseStream.ReadAsync(videoData, httpContext.RequestAborted);
                    }
                }

            }
            // TODO: Catch more explicit exceptions here, shouldn't be catching Exception
            catch (Exception ex)
            {
                httpContext.Abort();
                _logger.LogError(ex.Message);
            }
            finally
            {
                // Try everything we can to make sure the ffmpeg process wrapper closes down
                // and releases its resources properly, since transcoding 1080p and/or 4k streams
                // can use quite a bit of system resources over time.
                if (process != null)
                {
                    await process.WaitForExitAsync(httpContext.RequestAborted);
                    process.Close();
                    process.Dispose();
                }

                // Flush any remaining bytes out of the Response Body that may be sitting in 
                // the underlying buffers. (not 100% sure if this is needed, but just to be safe)
                await httpContext.Response.Body.FlushAsync(httpContext.RequestAborted);

                // Trying to make really damn sure the Response Stream gets closed.
                httpContext.Response.Body.Close();
            }
        }
    }
}
