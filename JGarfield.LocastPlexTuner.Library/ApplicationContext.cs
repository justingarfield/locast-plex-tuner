using JGarfield.LocastPlexTuner.Library.Domain;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System;
using System.Linq;

namespace JGarfield.LocastPlexTuner.Library
{
    public class ApplicationContext
    {
        private readonly IServer _server;

        public ApplicationContext(IServer server)
        {
            _server = server;
        }

        public DmaLocation CurrentDMA { get; set; }

        public Uri BaseUri {
            get {
                var firstAddress = _server.Features.Get<IServerAddressesFeature>()?.Addresses.First();
                return new Uri(firstAddress);
            }
        }
    }
}
