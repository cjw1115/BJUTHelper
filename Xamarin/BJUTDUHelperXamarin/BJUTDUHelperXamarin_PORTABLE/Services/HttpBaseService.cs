using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text;

namespace BJUTDUHelperXamarin.Services
{
    public class HttpBaseService
    {
        private HttpClientHandler _handler;
        private HttpClient _client;
        public HttpBaseService(bool allowRedirect = false)
        {
            //_handler = new ModernHttpClient.NativeMessageHandler();
            _handler = new HttpClientHandler();
            _handler.AllowAutoRedirect = allowRedirect;
            
            _client = new HttpClient(_handler);
        }
        public async Task<string> SendRequst(string uri, HttpMethod method, IDictionary<string, string> dic = null, string referUri = "", string useragnet="",CancellationToken cancellation = new CancellationToken(),Encoding postEncoding=null)
        {
            HttpResponseMessage response = null;
            Encoding encoding = Encoding.UTF8;
            try
            {
                if (!string.IsNullOrEmpty(referUri))
                {
                    _client.DefaultRequestHeaders.Referrer = new Uri(referUri);
                }
                if (!string.IsNullOrWhiteSpace(useragnet))
                {
                    _client.DefaultRequestHeaders.UserAgent.Clear();
                        _client.DefaultRequestHeaders.UserAgent.ParseAdd(useragnet);
                }

                
                if (method == HttpMethod.Get|| method == HttpMethod.Head)
                {
                    response = await _client.GetAsync(uri, cancellation);
                }
                else if (method == HttpMethod.Delete)
                {
                    var request = new HttpRequestMessage(HttpMethod.Delete, uri);
                    response = await _client.SendAsync(request);
                    request.Dispose();
                }
                else
                {
                    HttpContent content;
                    content = new FormUrlEncodedContent(dic);
                    response = await _client.PostAsync(uri, content, cancellation);
                }
                var mediaTypeHeaderValue = response.Content.Headers.ContentType;
                if (mediaTypeHeaderValue != null && mediaTypeHeaderValue.CharSet != null)
                {
                    if (mediaTypeHeaderValue.CharSet.Contains("gb2312"))
                    {
                        encoding = Encoding.GetEncoding("gb2312");
                    }
                }
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    byte[] buffer = new byte[stream.Length];
                    await stream.ReadAsync(buffer, 0, buffer.Length);
                    var str = encoding.GetString(buffer,0,buffer.Length);

                    return str;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                response?.Dispose();
            }
        }


        public async Task<Stream> SendRequstForStream(string uri, HttpMethod method, IDictionary<string, string> dic = null, CancellationToken cancellation = new CancellationToken())
        {
            HttpResponseMessage response = null;
            try
            {
                if (method == HttpMethod.Get)
                {
                    response = await _client.GetAsync(uri, cancellation);
                    return await response.Content.ReadAsStreamAsync();
                }
                else
                {
                    FormUrlEncodedContent content = new FormUrlEncodedContent(dic);

                    response = await _client.PostAsync(uri, content, cancellation);
                    MemoryStream ras = new MemoryStream();
                    var stream = await response.Content.ReadAsStreamAsync();
                    await stream.CopyToAsync(ras);
                    return ras;
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                response?.Dispose();
            }
        }

        public async Task<HttpStatusCode> GetResponseCode(string uri, CancellationToken cancellation = new CancellationToken())
        {
            var tempStatus = _handler.AllowAutoRedirect;
            HttpResponseMessage response = null;
            try
            {
                response = await _client.GetAsync(uri, cancellation);

                if (response.RequestMessage.RequestUri.AbsoluteUri != uri)
                {
                    return HttpStatusCode.Redirect;
                }
                return response.StatusCode;
            }
            catch
            {
                throw;
            }
            finally
            {
                //_handler.AllowAutoRedirect = tempStatus;
                response?.Dispose();
            }
        }
    }
}
