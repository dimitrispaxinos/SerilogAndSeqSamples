using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Practices.Unity.InterceptionExtension;

using Serilog.Context;

namespace SerilogSample.UnityInterception.Lib.Interception
{
    public class WatchServiceCallWatcherHanlder : ICallHandler
    {
        public IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext)
        {
            IMethodReturn result = null;

            result = getNext()(input, getNext);

            var returnValue = result.ReturnValue;

            return result;
        }

        public int Order { get; set; }
    }
}
