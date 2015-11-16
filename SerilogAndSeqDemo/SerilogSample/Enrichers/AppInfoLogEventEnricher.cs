using Serilog.Core;
using Serilog.Events;

namespace SerilogSample.Enrichers
{
    public class AppInfoLogEventEnricher : ILogEventEnricher
    {
        private readonly string _username;

        public AppInfoLogEventEnricher(string username)
        {
            _username = username;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Username", "TestUsername"));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ComputerName", "DimitrisPC"));
        }
    }
}
