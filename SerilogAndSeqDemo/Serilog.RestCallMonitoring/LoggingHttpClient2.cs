using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Serilog;

namespace Eurofins.Scp.Logging
{
    public class LoggingHttpClient : HttpClient
    {
        #region Private Properties

        private static string Environment = ConfigurationManager.AppSettings["CurrentEnvironment"];
        private static string ComputerName = System.Environment.MachineName;

        private static string _postPutTemplateString =
            "{methodName} Method failed when calling {@requestUri}.HttpStatus: {StatusCode} The request was {@content} and the response was {@response}";

        private static string _getDeleteTemplateString =
            "{methodName} Method failed when calling {@requestUri}.HttpStatus: {StatusCode} The response was {@response}";

        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        #endregion

        #region ctor

        protected LoggingHttpClient() { }

        /// <summary>
        /// Instantiate LoggingHttpClient
        /// </summary>
        /// <param name="httpClient">An HttpClient that might include your authentication details</param>
        /// <param name="seqUrl">Url for the Seq Instance</param>
        public LoggingHttpClient(HttpClient httpClient, string seqUrl)
        {
            if (httpClient == null)
                throw new ArgumentNullException("Please provide an HttpClient instance");
            if (seqUrl == null)
                throw new ArgumentNullException("Please provide an logger repositoryUrl instance");

            _httpClient = httpClient;
            _logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
                .MinimumLevel.Warning()                 // Set MinimumLevel of Logging to Warning
                .WriteTo.Seq(seqUrl)
              .CreateLogger();
        }

        #endregion

        #region Basic Hiding HttpClient Method Implementation

        public new async Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content)
        {
            var res = await _httpClient.PostAsync(requestUri, content);

            if (ShouldSercviceCallBeRecorded(res.StatusCode))
            {
                GetLoggerWithContext()
                    .Error(
                        _postPutTemplateString, "Post",
                        CombineAddress(requestUri),
                        res.StatusCode,
                        content,
                        res.Content.ReadAsStringAsync().Result);
            }
            return res;
        }

        public new Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content)
        {
            var res = _httpClient.PutAsync(requestUri, content);

            if (ShouldSercviceCallBeRecorded(res.Result.StatusCode))
            {
                GetLoggerWithContext()
                    .Error(
                        _postPutTemplateString, "Post",
                        CombineAddress(requestUri),
                        res.Result.StatusCode,
                        content,
                        res.Result.Content.ReadAsStringAsync().Result);
            }
            return res;
        }

        public new Task<HttpResponseMessage> GetAsync(Uri requestUri)
        {
            var res = _httpClient.GetAsync(requestUri);

            if (ShouldSercviceCallBeRecorded(res.Result.StatusCode))
            {
                GetLoggerWithContext()
                    .Error(
                        _getDeleteTemplateString, "Get",
                        CombineAddress(requestUri),
                        res.Result.StatusCode,
                        res.Result.Content.ReadAsStringAsync().Result);
            }
            return res;
        }

        public new Task<HttpResponseMessage> DeleteAsync(Uri requestUri)
        {
            var res = _httpClient.DeleteAsync(requestUri);

            if (ShouldSercviceCallBeRecorded(res.Result.StatusCode))
            {
                GetLoggerWithContext()
                    .Error(
                        _getDeleteTemplateString, "Delete",
                        CombineAddress(requestUri),
                        res.Result.StatusCode,
                        res.Result.Content.ReadAsStringAsync().Result);
            }
            return res;
        }

        #endregion

        #region Hiding HttpClient Method Implementation using string as Uri

        public new Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            return PostAsync(new Uri(requestUri, UriKind.Relative), content);
        }

        public new Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content)
        {
            return PutAsync(new Uri(requestUri, UriKind.Relative), content);
        }

        public new Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return GetAsync(new Uri(requestUri, UriKind.Relative));
        }

        public new Task<HttpResponseMessage> DeleteAsync(string requestUri)
        {
            return DeleteAsync(new Uri(requestUri, UriKind.Relative));
        }

        #endregion

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        #region Helper Classes

        /// <summary>
        /// Add Common Properties to every message
        /// </summary>
        /// <returns></returns>
        private ILogger GetLoggerWithContext()
        {
            return _logger.ForContext("Environment", Environment).ForContext("ComputerName", ComputerName);
        }

        private string CombineAddress(Uri uri)
        {
            if (uri.IsAbsoluteUri)
                return uri.AbsoluteUri;

            if (_httpClient == null || _httpClient.BaseAddress == null) return uri.AbsoluteUri;

            return String.Format("{0}{1}", _httpClient.BaseAddress.AbsoluteUri, uri.ToString());
        }

        /// <summary>
        /// Define the statuses in which the response should be recorded.
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        protected bool ShouldSercviceCallBeRecorded(HttpStatusCode statusCode)
        {
            return true;
            if (statusCode == HttpStatusCode.InternalServerError || statusCode == HttpStatusCode.BadRequest
                || statusCode == HttpStatusCode.GatewayTimeout || statusCode == HttpStatusCode.NotFound
                || statusCode == HttpStatusCode.NotImplemented) return true;
            return false;
        }

        #endregion
    }
}