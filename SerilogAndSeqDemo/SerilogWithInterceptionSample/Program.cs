using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Serilog;
using SerilogSample.Wpf;

namespace SerilogSample.UnityInterception
{
    class Program
    {
        private static IUnityContainer unityContainer;

        static void Main(string[] args)
        {
            // Create setup Unity
            SetupContainer();

            var samRepository = unityContainer.Resolve<ISamRepository>();

             //samRepository.GetSample("SampleCode");
           // samRepository.GetSampleWithDelay("SampleCode");
            samRepository.GetSampleThrowingAnException("SampleCode");

            // Console.ReadKey();
        }

        public static void SetupContainer()
        {
            // Create User
            var user = new CurrentUser() { Username = "GDP1", LabCode = "EUHAWE3" };

            // Create Logger
            var logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
                .MinimumLevel.Warning()                 // Set MinimumLevel of Warning
                .WriteTo.Seq("http://localhost:5341")
              .CreateLogger();

            unityContainer = new UnityContainer();

            // Register User
            unityContainer.RegisterInstance<ICurrentUser>(user);

            // Add Interception Extension
            unityContainer.AddNewExtension<Microsoft.Practices.Unity.InterceptionExtension.Interception>();

            //Register Logger Instance
            unityContainer.RegisterInstance<ILogger>(logger);

            unityContainer.RegisterType<ISamRepository, SamRepository>()
                .Configure<Microsoft.Practices.Unity.InterceptionExtension.Interception>()
                .SetInterceptorFor<ISamRepository>(
                    new InterfaceInterceptor());
        }
    }
}
