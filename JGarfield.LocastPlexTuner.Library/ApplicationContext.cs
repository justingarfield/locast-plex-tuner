using JGarfield.LocastPlexTuner.Domain;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace JGarfield.LocastPlexTuner.Library
{
    public class ApplicationContext
    {
        private readonly IServer _server;

        private readonly IConfiguration _configuration;

        public ApplicationContext(IServer server, IConfiguration configuration)
        {
            _server = server;
            _configuration = configuration;
        }

        public string FfmpegBinaryPath { get; set; }

        public DmaLocation CurrentDMA { get; set; }

        public Uri BaseUri {
            get
            {
                // TODO: Need to abstract this out another layer..what if library is used outside of a WebApi? (e.g. Console app)
                //       Although...capabilities that use this also can't function properly without an address to bind to.
                var firstAddress = _server.Features.Get<IServerAddressesFeature>()?.Addresses.First();
                return new Uri(firstAddress);
            }
        }

        public bool RunningInContainer {
            get
            {
                if (bool.TryParse(_configuration.GetSection("DOTNET_RUNNING_IN_CONTAINER").Value, out bool runningInContainer))
                {
                    return runningInContainer;
                }
                else
                {
                    return false;
                }
            } 
        }
    }
}
