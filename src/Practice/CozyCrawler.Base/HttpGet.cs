﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CozyCrawler.Base
{
    public class HttpGet
    {
        public static HttpResponseMessage Get(string url, bool allowAutoRedirect = true)
        {
            Uri uri = new Uri(HttpCommon.Host);
            HttpClientHandler handler = new HttpClientHandler { UseCookies = true, AllowAutoRedirect = allowAutoRedirect, };
            CookieContainer CookieContainer = HttpCookie.GetUriCookieContainer(uri);
            if (CookieContainer != null)
            {
                handler.CookieContainer = CookieContainer;
            }
            else
            {
                handler.CookieContainer = new CookieContainer();
            }
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("User-Agent", HttpCommon.DefaultUA);

            HttpClient client = new HttpClient(handler);
            HttpResponseMessage response = client.SendAsync(request).Result;
            return response;
        }
    }
}
