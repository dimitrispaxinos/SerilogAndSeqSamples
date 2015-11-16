using Serilog.Core;
using Serilog.Events;

namespace SerilogSample.Enrichers
{
    public class UserLogEventEnricher : ILogEventEnricher
    {
        private readonly IUser _user;

        public UserLogEventEnricher(IUser user)
        {
            _user = user;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("Username", _user.Username));
            logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ComputerName", _user.Lab));
        }
    }
}
