using System;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog.Context;

namespace Serilog.RestCallMonitoring
{
    public class SerilogHttpClientWrapper : HttpClient
    {
        private static string _templateString =
            "Rest Call: {RestAction} called {RequestUri}. HttpStatus: {StatusCode}.";

        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public SerilogHttpClientWrapper()
        {
            _httpClient = new HttpClient();
            _logger = new LoggerConfiguration().WriteTo.Seq("http://localhost:5341").CreateLogger();
        }


        public SerilogHttpClientWrapper(HttpClient httpClient)//, ILogger logger)
        {
            _httpClient = httpClient;
            //_logger = logger;
            _logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.Seq("http://localhost:5341").CreateLogger();
        }

        #region Overrides

        public async new Task<HttpResponseMessage> GetAsync(Uri requestUri)
        {
            var res = await _httpClient.GetAsync(requestUri);
            LogResponseMessage(requestUri, res, "GET");
            return res;
        }

        public new async Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content)
        {
            var res = await _httpClient.PostAsync(requestUri, content);
            LogResponseMessage(requestUri, res, "POST", content);
            return res;
        }

        public async new Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content)
        {
            var res = await _httpClient.PutAsync(requestUri, content);
            LogResponseMessage(requestUri, res, "PUT", content);
            return res;
        }

        public async new Task<HttpResponseMessage> DeleteAsync(Uri requestUri)
        {
            var res = await _httpClient.DeleteAsync(requestUri);
            LogResponseMessage(requestUri, res, "DELETE");
            return res;
        }

        #endregion

        protected virtual void LogResponseMessage(Uri requestUri, HttpResponseMessage res, string restAction, HttpContent content = null)
        {
            using (LogContext.PushProperty("Request", content))
            using (LogContext.PushProperty("Response", res.Content.ReadAsStringAsync().Result))
            {
                if (res.IsSuccessStatusCode)
                {
                    _logger.Information(
                            _templateString, restAction,
                            CombineAddress(requestUri),
                            res.StatusCode);
                }
                else
                    _logger.Error(
                            _templateString, restAction,
                            CombineAddress(requestUri),
                            res.StatusCode);
            }
        }
        private string CombineAddress(Uri uri)
        {
            if (uri.IsAbsoluteUri)
                return uri.AbsoluteUri;

            if (_httpClient == null || _httpClient.BaseAddress == null) return uri.AbsoluteUri;

            return String.Format("{0}{1}", _httpClient.BaseAddress.AbsoluteUri, uri.ToString());
        }
    }
}
