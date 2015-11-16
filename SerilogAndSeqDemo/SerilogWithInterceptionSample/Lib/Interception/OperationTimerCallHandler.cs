using System;
using System.Configuration;
using Microsoft.Practices.Unity.InterceptionExtension;
using Serilog;
using Serilog.Context;
using SerilogSample.Wpf;

namespace SerilogSample.UnityInterception.Interception
{
    public class OperationTimerCallHandler : ICallHandler
    {
        private readonly int _acceptableTime;
        private readonly string _identifier;
        private readonly string _description;
        private readonly ICurrentUser _currentUser;
        private readonly ILogger _logger;

        public OperationTimerCallHandler(int acceptableTime, string identifier, string description, ICurrentUser currentUser, ILogger logger)
        {
            _acceptableTime = acceptableTime;
            _currentUser = currentUser;
            _logger = logger;
            _identifier = identifier;
            _description = description;
        }

        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            IMethodReturn result = null;

            using (LogContext.PushProperty("Username", _currentUser.Username))
            using (LogContext.PushProperty("LabCode", _currentUser.LabCode))
            using (LogContext.PushProperty("ServerName", Environment.MachineName))
            using (LogContext.PushProperty("MethodName", input.MethodBase.Name))
            using (LogContext.PushProperty("Environment", ConfigurationManager.AppSettings["Environment"]))
            using (var op = _logger.BeginTimedOperation(_description ?? String.Empty, _identifier ?? String.Empty, warnIfExceeds: new TimeSpan(0, 0, 0, 0, _acceptableTime)))
            {
                //.ForContext(input.Target.GetType())
                result = getNext()(input, getNext);

                if (result.Exception != null)
                    _logger.Error(result.Exception, "{_identifier} failed.", _identifier);
            }

            return result;

        }

        public int Order { get; set; }
    }
}
