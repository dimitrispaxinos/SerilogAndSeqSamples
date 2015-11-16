using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using System.Windows;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;
using SerilogSample.Enrichers;

namespace SerilogSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Contact Contact { get; set; }
        public User User { get; set; }

        public MainWindow()
        {
            Contact = new Contact() { Name = "TestName", Email = "dpaxinos@gmail.com" };
            User = new User() { Username = "GDP1", Lab = "DimitrisPC" };

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();
        }

        #region Basic Cases

        private void CreateInformationEntry(object sender, RoutedEventArgs e)
        {
            Log.Logger.Information("This first attempt actually worked");
        }

        private void CreateErrorEntry(object sender, RoutedEventArgs e)
        {
            Log.Logger.Error("This is an error");
        }

        private void CreateEntryWithObject(object sender, RoutedEventArgs e)
        {
            Log.Logger.Error(new Exception("Something went wrong"), "Oops the {@Contact} could not be saved", Contact);
        }

        private void CreateEntryEnrichedWithUserInfo(object sender, RoutedEventArgs e)
        {
            var log = new LoggerConfiguration()
                .Enrich.FromLogContext()
                  .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();

            using (LogContext.PushProperty("User", "GDP1"))
            using (LogContext.PushProperty("Lab", "EUHAWE3"))
            {
                log.Information("Info logged with user info");
            }
        }

        #endregion

        #region Contextual Examples

        private void AddOnePropertyToContext(object sender, RoutedEventArgs e)
        {
            Log.Logger = new LoggerConfiguration()
             .Enrich.FromLogContext()
               .WriteTo.Seq("http://localhost:5341")
             .CreateLogger();

            // Add log event with a contextual property
            using (LogContext.PushProperty("User", "Test User"))
            {
                Log.Logger.Information("This log entry includes a contextual property");
            }

            // Add log event without contextual properties
            Log.Logger.Information("This log entry does not include any contextual property");
        }

        // Declare Person class
        public class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        private void AddAStructuredObjectToContext(object sender, RoutedEventArgs e)
        {
            var log = new LoggerConfiguration()
              .Enrich.FromLogContext()
                .WriteTo.Seq("http://localhost:5341")
              .CreateLogger();

            // Instantiate a Person  
            var person = new Person()
            {
                FirstName = "Dimitris",
                LastName = "Paxinos"
            };

            using (LogContext.PushProperty("PersonDetails", person, destructureObjects: true))
            {
                log.Information("This log entry includes a structured object");
            }
        }

        private void AddADynamicObjectToContext(object sender, RoutedEventArgs e)
        {
            Log.Logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
                .WriteTo.Seq("http://localhost:5341")
              .CreateLogger();

            var dynamicObject = new
            {
                OperatingSystem = "Windows",
                Username = "TestUser",
                OneMoreRandomProperty = "Random Property Value "
            };

            using (LogContext.PushProperty("DynamicObject", dynamicObject, destructureObjects: true))
            {
                Log.Logger.Information("This log entry includes a dynamic object");
            }
        }


        private void AddMultiplePropertiesToContext(object sender, RoutedEventArgs e)
        {
            var log = new LoggerConfiguration()
              .Enrich.FromLogContext()
                .WriteTo.Seq("http://localhost:5341")
              .CreateLogger();

            using (LogContext.PushProperty("User", "GDP1"))
            using (LogContext.PushProperty("Lab", "EUHAWE3"))
            {
                log.Information("This log entry includes two attached properties");
            }
        }

        private void AddMultiplePropertiesToContextUsingCustomPropertyAttacher(object sender, RoutedEventArgs e)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();

            properties.Add("SampleId", "EUANNA-20150112-1254896215");
            properties.Add("User", "GDP1");
            properties.Add("Lab", "EUHAWE3");

            var log = new LoggerConfiguration()
                .Enrich.FromLogContext()
                  .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();

            using (new MultiplePropertyPusher(properties))
            {
                log.Information("This log entry includes two attached properties using a custom multiproperty attacher");
            }
        }

        private void AddClassNameSpaceToContext(object sender, RoutedEventArgs e)
        {
            // Explicitly including the class
            Log.Logger.ForContext<MainWindow>().Information("This log entry includes the class name as context");

            // More generic getting the type of the class we are in 
            Log.Logger.ForContext(this.GetType()).Information("This log entry includes the class name as context");
        }

        private void AddContextUsingEnrichers(object sender, RoutedEventArgs e)
        {
            //Log.ForContext(new ILogEventEnricher[] { new UserLogEventEnricher(User), })
            //    .Information("Properties were added to the context using the enricher");

            using (LogContext.PushProperties(new UserLogEventEnricher(User)))
            {
                Log.Logger.Information("Properties were added to the context using the enricher");
            }
        }

        #endregion

        #region Timing Operations - Calling WebServices

        private void LogSuccesfullServiceCall(object sender, RoutedEventArgs e)
        {
            var log = new LoggerConfiguration()
                .Enrich.FromLogContext()
                  .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();

            using (new MultiplePropertyPusher(GetProperties()))
            using (var op = log.BeginTimedOperation("ServiceCalls", "SamUpdateService"))
            {
                Thread.Sleep(1000);
            }
        }

        private void ProduceWarningForServiceCallThatTakesLong(object sender, RoutedEventArgs e)
        {
            var log = new LoggerConfiguration()
                .Enrich.FromLogContext()
                  .WriteTo.Seq("http://localhost:5341")
                .CreateLogger();

            //using (new MultiplePropertyPusher(GetProperties()))
            using (var op = log.BeginTimedOperation("SamUpdateService", "ServiceCalls", LogEventLevel.Warning, new TimeSpan(0, 0, 0, 0, 1300)))
            {
                Thread.Sleep(1400);
            }
        }

        #endregion

        public Dictionary<string, object> GetProperties()
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();

            properties.Add("SampleId", "EUANNA-20150112-1254896215");
            properties.Add("CallingUrl", "www.google.com");
            properties.Add("User", "GDP1");
            properties.Add("Lab", "EUHAWE3");

            return properties;
        }
    }

    #region Helper Classes

    public class Contact
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public interface IUser
    {
        string Username { get; set; }
        string Lab { get; set; }
    }

    public class User : IUser
    {
        public string Username { get; set; }
        public string Lab { get; set; }
    }

    #endregion
}
