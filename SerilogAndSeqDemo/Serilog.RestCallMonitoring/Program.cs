using System;
using System.Net.Http;

namespace Serilog.RestCallMonitoring
{
    class Program
    {
        static void Main(string[] args)
        {
            //GetLoggerWithContext();
            //Log.Logger.Information("this is a test case {FirstThing}","test","whatelse","onemoreProperty");

            var httpClient = new HttpClient();
            //httpClient.BaseAddress = new Uri("http://jsonplaceholder.typicode.com/");

            SerilogHttpClientWrapper wrapper = new SerilogHttpClientWrapper(httpClient);

            //var response = wrapper.GetAsync(new Uri("http://jsonplaceholder.typicode.com/users")).Result;
            var response = wrapper.GetAsync(new Uri("http://www.234fdasdasdasdasdsa")).Result;

        }

        private static ILogger GetLoggerWithContext()
        {
            Log.Logger =
                new LoggerConfiguration().Enrich.FromLogContext().WriteTo.Seq("http://localhost:5341").CreateLogger();

            return Log.Logger;
        }
    }
}
