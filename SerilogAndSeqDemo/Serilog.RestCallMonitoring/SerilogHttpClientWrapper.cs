using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Serilog.RestCallMonitoring
{
    public abstract class SerilogHttpClientWrapper : HttpClient
    {
        private static string Environment = ConfigurationManager.AppSettings["CurrentEnvironment"];
        private static string ComputerName = System.Environment.MachineName;

        private static string _postPutTemplateString =
            "{methodName} Method calling {@requestUri}.HttpStatus: {StatusCode} The request was {@content} and the response was {@response}";

        private static string _postPutFailedTemplateString =
            "{methodName} Method failed when calling {@requestUri}.HttpStatus: {StatusCode} The request was {@content} and the response was {@response}";

        private static string _getDeleteTemplateString =
            "{methodName} Method failed when calling {@requestUri}.HttpStatus: {StatusCode} The response was {@response}";

        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        protected SerilogHttpClientWrapper(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public new async Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content)
        {
            var res = await _httpClient.PostAsync(requestUri, content);

            if (!res.IsSuccessStatusCode)
            {
                _logger.Error(
                        _postPutFailedTemplateString, "Post",
                        CombineAddress(requestUri),
                        res.StatusCode,
                        content,
                        res.Content.ReadAsStringAsync().Result);
            }



            //if (ShouldSercviceCallBeRecorded(res.StatusCode))
            //{
            //    GetLoggerWithContext()
            //        .Error(
            //            _postPutFailedTemplateString, "Post",
            //            CombineAddress(requestUri),
            //            res.StatusCode,
            //            content,
            //            res.Content.ReadAsStringAsync().Result);
            //}
            return res;
        }

        public new Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content)
        {
            var res = _httpClient.PutAsync(requestUri, content);

            //if (ShouldSercviceCallBeRecorded(res.Result.StatusCode))
            //{
            //    GetLoggerWithContext()
            //        .Error(
            //            _postPutFailedTemplateString, "Post",
            //            CombineAddress(requestUri),
            //            res.Result.StatusCode,
            //            content,
            //            res.Result.Content.ReadAsStringAsync().Result);
            //}
            return res;
        }

        public async new Task<HttpResponseMessage> GetAsync(Uri requestUri)
        {
            var res =await _httpClient.GetAsync(requestUri);
            

            


            if (!res.IsSuccessStatusCode)
            {
                _logger
                    .Error(
                        _getDeleteTemplateString, "Get",
                        CombineAddress(requestUri),
                        res.StatusCode,
                        res.Content.ReadAsStringAsync().Result);
            }
            else
            {
                _logger.Information();
            }


            //if (ShouldSercviceCallBeRecorded(res.Result.StatusCode))
            //{
            //    GetLoggerWithContext()
            //        .Error(
            //            _getDeleteTemplateString, "Get",
            //            CombineAddress(requestUri),
            //            res.Result.StatusCode,
            //            res.Result.Content.ReadAsStringAsync().Result);
            //}
            return res;
        }

        public new Task<HttpResponseMessage> DeleteAsync(Uri requestUri)
        {
            var res = _httpClient.DeleteAsync(requestUri);

            //if (ShouldSercviceCallBeRecorded(res.Result.StatusCode))
            //{
            //    GetLoggerWithContext()
            //        .Error(
            //            _getDeleteTemplateString, "Delete",
            //            CombineAddress(requestUri),
            //            res.Result.StatusCode,
            //            res.Result.Content.ReadAsStringAsync().Result);
            //}
            return res;
        }

        /// <summary>
        /// Define the statuses in which the response should be recorded.
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        protected abstract bool ShouldSercviceCallBeRecorded(HttpStatusCode statusCode);

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

    }
}
