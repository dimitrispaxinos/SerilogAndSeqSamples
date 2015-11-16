using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Context;

namespace SerilogSample
{
    public class MultiplePropertyPusher :IDisposable
    {
        private readonly List<IDisposable> _propertyPushers = new List<IDisposable>();

        public MultiplePropertyPusher(Dictionary<string, object> properties)
        {
            foreach (var property in properties)
            {
                _propertyPushers.Add(LogContext.PushProperty(property.Key,property.Value));
            }
        }

        public void Dispose()
        {
            if(_propertyPushers.Any())
                foreach (var propertyPusher in _propertyPushers)
                {
                    propertyPusher.Dispose();
                }
        }
    }
}
