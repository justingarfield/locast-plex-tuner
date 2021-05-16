using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace JGarfield.LocastPlexTuner.WebApi.Controllers
{
    [Route("[controller]")]
    public class MetricsController : ControllerBase
    {
        private JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
        {
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public MetricsController()
        {

        }

    }
}
