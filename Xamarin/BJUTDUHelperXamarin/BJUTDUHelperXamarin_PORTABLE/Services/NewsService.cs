using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BJUTDUHelperXamarin.Services
{
    public class NewsService
    {
        private static readonly string newsHeadersUri = "https://cjw1115.com/news/getnewsheader";
        private static readonly string newsUri = "https://cjw1115.com/news/getnews";
        public static async Task<IList<Models.NewsHeaderModel>> GetHeaders(Services.HttpBaseService httpService)
        {
            try
            {
                var re = await httpService.SendRequst(newsHeadersUri, HttpMethod.Get);
                var headers=Newtonsoft.Json.JsonConvert.DeserializeObject<Models.NewsHeaderModel[]>(re);
                return headers;
            }
            catch(HttpRequestException)
            {
                return null;
            }
            catch(Exception e)
            {
                return null;
            }
        }
        public static async Task<Models.NewsModel> GetNews(Services.HttpBaseService httpService,int id)
        {
            try
            {
                var re = await httpService.SendRequst($"{newsUri}/?={id}", HttpMethod.Get);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<Models.NewsModel>(re);
            }
            catch (HttpRequestException)
            {
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }
            }
}
