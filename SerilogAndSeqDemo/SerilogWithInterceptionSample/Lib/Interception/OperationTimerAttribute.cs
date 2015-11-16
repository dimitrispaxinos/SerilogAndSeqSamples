using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Serilog;
using SerilogSample.Wpf;

namespace SerilogSample.UnityInterception.Interception
{
    public class OperationTimerAttribute : HandlerAttribute
    {
        private readonly int _acceptableTime;
        private readonly string _identifier;
        private readonly string _description;

        public OperationTimerAttribute(int acceptableTime, string description, string identifier)
        {
            _acceptableTime = acceptableTime;
            _identifier = identifier;
            _description = description;
        }

        public override ICallHandler CreateHandler(IUnityContainer container)
        {
            return new OperationTimerCallHandler(_acceptableTime, _description, _identifier, container.Resolve<ICurrentUser>(), container.Resolve<ILogger>());
        }
    }
}
