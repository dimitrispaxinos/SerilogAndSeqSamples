using System;
using Serilog;

namespace SerilogSample.UnityInterception
{
    public class GetSamDataHandler
    {
        private readonly ILogger _logger;
        private readonly ISamRepository _samRepository;

        public GetSamDataHandler(ILogger logger, ISamRepository samRepository)
        {
            _logger = logger;
            _samRepository = samRepository;
        }

        public ISample Handle()
        {
            ISample sample = null;

            using (_logger.BeginTimedOperation("Fetching Sample Data from SAM", "ServiceCalls", warnIfExceeds: new TimeSpan(1300)))
            {
                try
                {
                    sample = _samRepository.GetSample("sampleCode");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error Calling SAM GetSample");
                    throw ex;
                }
            }

            return sample;
        }
    }
}
