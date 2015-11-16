namespace SerilogSample.Wpf
{
    public class CurrentUser : ICurrentUser
    {

        public string Username { get; set; }

        public string LabCode { get; set; }


        public bool FatalErrorOccurred { get; set; }

        public string FatalErrorMessage { get; set; }
    }
}
