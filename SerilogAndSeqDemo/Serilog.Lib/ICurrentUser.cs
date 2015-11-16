namespace SerilogSample.Wpf
{
    public interface ICurrentUser
    {
        string Username { get; }

        string LabCode { get; }

        bool FatalErrorOccurred { get; set; }

        string FatalErrorMessage { get; set; }
    }
}
