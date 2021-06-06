using System;

namespace JGarfield.LocastPlexTuner.Domain.AggregateRoots.ElectronicProgramGuide
{
    /// <summary>
    /// Represents an Electronic Program Guide (EPG).
    /// <br /><br />
    /// See: <see href="https://en.wikipedia.org/wiki/Electronic_program_guide"/>
    /// </summary>
    public class ElectronicProgramGuide
    {
        public string SourceInfoName { get; } = "Locast";

        public Uri SourceInfoUri { get; } = new Uri("https://www.locast.org");

        public string GeneratorInfoName { get; } = "LocastPlexTuner";

        public Uri GeneratorInfoUri { get; } = new Uri("https://github.com/justingarfield/LocastPlexTuner");
    }
}
