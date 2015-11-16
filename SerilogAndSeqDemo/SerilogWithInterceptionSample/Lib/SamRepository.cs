using System;
using System.Threading;

namespace SerilogSample.UnityInterception
{
    public class SamRepository : ISamRepository
    {
        public ISample GetSample(string sampleCode)
        {
            return null;
        }

        public ISample GetSampleWithDelay(string sampleCode)
        {
            Thread.Sleep(2000);
            return null;
        }

        public ISample GetSampleThrowingAnException(string sampleCode)
        {
            throw new NotImplementedException();
        }
    }
}
