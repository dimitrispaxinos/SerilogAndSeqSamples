using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Serilog;

namespace SerilogSample.UnityInterception
{
    public class LoggingHttpClient : ILoggingHttpClient, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly IToken _token;

        public LoggingHttpClient(ILogger logger, IToken token)
        {
            _logger = logger;
            _token = token;

            if (_token != null)
            {
                _httpClient = new HttpClient();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("IPW", "Token");
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            }

            // TODO: This is a temporary solution, until we set up IIS with application pool running under this client
            //  At the moment, hard coded username and password

            var credentials = new NetworkCredential("es-area1\\DE40_SVC_SCP_USR", "NG_elims_HH!");
            var handler = new HttpClientHandler { Credentials = credentials };

            _httpClient = new HttpClient(handler);
            // client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            var res = _httpClient.PostAsync(requestUri, content);

            if (ShouldSercviceCallBeRecorded(res.Result.StatusCode))
            {
                _logger.Error("Post Method failed when calling {requestUri}. The request was {@content} and the response was {@response}", requestUri, content, res.Result);
            }
            return res;
        }

        public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content)
        {
            var res = _httpClient.PostAsync(requestUri, content);

            if (ShouldSercviceCallBeRecorded(res.Result.StatusCode))
            {
                _logger.Error("Put Method failed when calling {requestUri}. The request was {@content} and the response was {@response}", requestUri, content, res.Result);
            }
            return res;
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            var res = _httpClient.GetAsync(requestUri);

            if (ShouldSercviceCallBeRecorded(res.Result.StatusCode))
            {
                _logger.Error("Get Method failed when calling {requestUri}. The response was {@response}", requestUri);
            }
            return res;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        private bool ShouldSercviceCallBeRecorded(HttpStatusCode statusCode)
        {
            return true;

            if (statusCode == HttpStatusCode.InternalServerError || statusCode == HttpStatusCode.BadRequest
                || statusCode == HttpStatusCode.GatewayTimeout || statusCode == HttpStatusCode.NotFound
                || statusCode == HttpStatusCode.NotImplemented) return true;
            return false;
        }
    }

    public interface IToken
    {
        string Username { get; }
        string Labsite { get; }
        string Fullname { get; }
        DateTimeOffset ExpirationDate { get; }
    }
}

public interface ILoggingHttpClient
{
    Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);

    Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content);

    Task<HttpResponseMessage> GetAsync(string requestUri);
}