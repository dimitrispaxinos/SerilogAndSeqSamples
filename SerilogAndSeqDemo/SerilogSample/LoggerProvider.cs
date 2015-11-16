using Serilog;

namespace SerilogSample
{
    public class LoggerProvider
    {
        private readonly ILogger _logger;
        

        public LoggerProvider()
        {
            _logger = new LoggerConfiguration().CreateLogger();
        }



    }
}
