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
            _handler.UseProxy = false;
            _handler.AllowAutoRedirect = allowRedirect;
            
            
            _client = new HttpClient(_handler);
        }
        public async Task<string> SendRequst(string uri, HttpMethod method, IDictionary<string, string> dic = null, string referUri = "", string useragnet="",CancellationToken cancellation = new CancellationToken(),Encoding postEncoding=null,string Authorization = null)
        {
            HttpContent content=null;
            if (method == HttpMethod.Post)
            {
                content = new FormUrlEncodedContent(dic);
            }
            return await SendRequst(uri, method, content, referUri, useragnet, cancellation, postEncoding, Authorization);
        }

        public async Task<string> SendRequst(string uri, HttpMethod method, HttpContent content, string referUri = "", string useragnet = "", CancellationToken cancellation = new CancellationToken(), Encoding postEncoding = null, string Authorization = null)
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


                HttpRequestMessage requestMessage = new HttpRequestMessage();
                if (!string.IsNullOrEmpty(Authorization))
                {
                    requestMessage.Headers.Add("Authorization", Authorization);
                }

                if (method == HttpMethod.Get || method == HttpMethod.Head)
                {
                    requestMessage.Method = HttpMethod.Get;
                    requestMessage.RequestUri = new Uri(uri);

                    response = await _client.SendAsync(requestMessage);
                    //response = await _client.GetAsync(uri, cancellation);
                    requestMessage.Dispose();
                }
                else if (method == HttpMethod.Delete)
                {
                    requestMessage.Method = HttpMethod.Delete;
                    requestMessage.RequestUri = new Uri(uri);
                    response = await _client.SendAsync(requestMessage);
                    requestMessage.Dispose();
                }
                else
                {
                    requestMessage.Method = HttpMethod.Post;
                    requestMessage.RequestUri = new Uri(uri);
                    requestMessage.Content = content;
                    if(content is MultipartFormDataContent)
                    {
                        requestMessage.Headers.Add("ContentType", "multipart/form-data");
                    }
                    else
                    {
                        requestMessage.Headers.Add("ContentType", "application/x-www-form-urlencoded");
                    }
                    
                    response = await _client.SendAsync(requestMessage, cancellation);

                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new InvalidUserInfoException("请重新登录");
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
                    var str = encoding.GetString(buffer, 0, buffer.Length);

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
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

                    response = await _client.SendAsync(requestMessage, cancellation);
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

        public async Task<bool> GetCanResponse(string uri)
        {
            _client.Timeout=TimeSpan.FromSeconds(3);
            HttpResponseMessage response = null;
            HttpRequestMessage request = null;
            try
            {
                request = new HttpRequestMessage(HttpMethod.Head, uri);
                response = await _client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            finally
            {
                response?.Dispose();
                request?.Dispose();

                _client.Timeout = TimeSpan.FromSeconds(3);
            }
        }
    }
}
