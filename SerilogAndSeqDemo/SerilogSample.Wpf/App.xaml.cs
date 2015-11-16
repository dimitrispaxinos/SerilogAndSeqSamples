using System;
using System.Configuration;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.Unity;
using Serilog;
using Serilog.Context;

namespace SerilogSample.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IUnityContainer UnityContainer;
        public ILogger Logger;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create Logger
            Logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
                .WriteTo.Seq("http://localhost:5341")
              .CreateLogger();

            // Create Container
            UnityContainer = new UnityContainer();
            UnityContainer.RegisterInstance<ICurrentUser>(new CurrentUser() { Username = "GDP1", LabCode = "EUANNA" });
        }

        private void ApplicationDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            if (UnityContainer != null)
            {
                var computerName = Environment.MachineName;
                var currentUser = UnityContainer.Resolve<ICurrentUser>();
                var environment = ConfigurationManager.AppSettings["Environment"];
                var applicationName = ConfigurationManager.AppSettings["ApplicationName"];

                var randomNumber = (new Random()).Next(0, 1000).ToString("D4");
                var errorId = string.Format("{0}-{1}-{2}", currentUser.Username, currentUser.LabCode, randomNumber);


                using (LogContext.PushProperty("ErrorId", errorId))
                using (LogContext.PushProperty("Username", currentUser.Username))
                using (LogContext.PushProperty("LabCode", currentUser.LabCode))
                using (LogContext.PushProperty("ComputerName", computerName))
                using (LogContext.PushProperty("Environment", environment))
                using (LogContext.PushProperty("ApplicationName", applicationName))
                {
                    // Not very descriptive message 
                    //Logger.Fatal(e.Exception, "A fatal error occured.");

                    // More humanly readable message
                    Logger.Fatal(e.Exception, "A fatal error occured in the User interface of {ApplicationName} in {Environment} on the machine {ComputerName} where user {Username} was logged on.",
                      applicationName, environment, computerName, currentUser.Username);

                    MessageBox.Show(String.Format("A fatal error occured and the application has to shutdown.Please contanct help desk using the following Error Id: {0}", errorId));
                    Current.Shutdown();
                }
            }
        }
    }
}
