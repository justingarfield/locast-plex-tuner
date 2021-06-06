using System;

namespace JGarfield.LocastPlexTuner.Domain.Exceptions
{
    public class LocastPlexTunerDomainException : Exception
    {
        public LocastPlexTunerDomainException()
        { }

        public LocastPlexTunerDomainException(string message)
            : base(message)
        { }

        public LocastPlexTunerDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
