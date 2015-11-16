using SerilogSample.UnityInterception.Interception;

namespace SerilogSample.UnityInterception
{
    public interface ISamRepository
    {
        [OperationTimer(2000,"Calling GetSample", "SamServices")]
        ISample GetSample(string sampleCode);

        [OperationTimer(1600, "Calling GetSampleWithDelay", "SamServices")]
        ISample GetSampleWithDelay(string sampleCode);

        [OperationTimer(1600, "Calling GetSampleThrowingAnException", "SamServices")]
        ISample GetSampleThrowingAnException(string sampleCode);
    }
}
