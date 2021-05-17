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
                // TODO: Need to abstract this out another layer..what if library is used outside of a WebApi? (e.g. Console app)
                //       Although...capabilities that use this also can't function properly without an address to bind to.
                var firstAddress = _server.Features.Get<IServerAddressesFeature>()?.Addresses.First();
                return new Uri(firstAddress);
            }
        }
    }
}
