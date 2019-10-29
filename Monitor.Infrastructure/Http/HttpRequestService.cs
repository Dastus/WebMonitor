using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Monitor.Infrastructure.Http
{
    public class HttpRequestService : IHttpRequestService
    {
        private HttpClient _userHttpClient = new HttpClient();
        private HttpClient _googleBotHttpClientDesktop = new HttpClient {
            DefaultRequestHeaders = { { "User-Agent", "Googlebot/2.1 (+http://www.google.com/bot.html)" } }
        };
        private HttpClient _googleBotHttpClientMobile = new HttpClient
        {
            DefaultRequestHeaders = { { "User-Agent", "Mozilla/5.0 (Linux; Android 6.0.1; Nexus 5X Build/MMB29P) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/41.0.2272.96 Mobile Safari/537.36 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)" } }
        };

        /// <summary>Get page HTML structure as Google Bot</summary>
        public async Task<HtmlDocument> GetHtmlStructure(string url, TimeSpan timeout)
        {
            var result = await _userHttpClient.GetAsyncWithTimeout(url, timeout);
            var stream = await result.Content.ReadAsStreamAsync();

            var doc = new HtmlDocument();
            doc.Load(stream);

            return doc;
        }

        /// <summary>Get page HTML structure</summary>
        public async Task<HtmlDocument> GetHtmlStructureAsGoogleBotDesktop(string url, TimeSpan timeout)
        {
            var result = await _googleBotHttpClientDesktop.GetAsyncWithTimeout(url, timeout);
            var stream = await result.Content.ReadAsStreamAsync();

            var doc = new HtmlDocument();
            doc.Load(stream);

            return doc;
        }

        public async Task<HtmlDocument> GetHtmlStructureAsGoogleBotMobile(string url, TimeSpan timeout)
        {
            var result = await _googleBotHttpClientMobile.GetAsyncWithTimeout(url, timeout);
            var stream = await result.Content.ReadAsStreamAsync();

            var doc = new HtmlDocument();
            doc.Load(stream);

            return doc;
        }

        /// <summary>Get page load time as Google Bot</summary>
        public async Task<WebPageLoadResult> GetPageLoadResultAsGoogleBot(string url, TimeSpan timeout)
        {
            var startTime = DateTime.Now;
            var resp = await _googleBotHttpClientDesktop.GetAsyncWithTimeout(url, timeout);
            var endTime = DateTime.Now;

            return new WebPageLoadResult
            {
                ResponseStatus = resp.StatusCode,
                LoadTime = (endTime - startTime)
            };
        }

        /// <summary>Get page load time</summary>
        public async Task<WebPageLoadResult> GetPageLoadResult(string url, TimeSpan timeout)
        {
            var startTime = DateTime.Now;

            var resp = await _userHttpClient.GetAsyncWithTimeout(url, timeout);
            var endTime = DateTime.Now;

            return new WebPageLoadResult
            {
                ResponseStatus = resp.StatusCode,
                LoadTime = (endTime - startTime)
            };
        }

        public async Task<HttpResponseMessage> PerformGetRequest(string url, TimeSpan timeout, IDictionary<string, string> queryParameters = null)
        {
            return await _userHttpClient.GetAsyncWithTimeout(url, timeout, queryParameters);
        }

        public async Task<HttpResponseMessage> PerformGetRequestAsGoogleBot(string url, TimeSpan timeout, IDictionary<string, string> queryParameters = null)
        {
            return await _googleBotHttpClientDesktop.GetAsyncWithTimeout(url, timeout, queryParameters);
        }

        public async Task<HttpResponseMessage> PerformPostRequest(string url, TimeSpan timeout, object requestModel = null)
        {
            return await _userHttpClient.PostAsyncWithTimeout(url, requestModel, timeout);
        }

        public async Task<HttpResponseMessage> PerformPostRequestAsGoogleBot(string url, TimeSpan timeout, object requestModel = null)
        {
            return await _googleBotHttpClientDesktop.PostAsyncWithTimeout(url, requestModel, timeout);
        }
    }

    static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> GetAsyncWithTimeout(this HttpClient httpClient, string url, TimeSpan timeout, IDictionary<string, string> queryParameters = null)
        {
            url = (queryParameters == null) ? url : QueryHelpers.AddQueryString(url, queryParameters);

            if (timeout == null)
            {
                return await httpClient.GetAsync(url);
            }

            var cts = new CancellationTokenSource();
            cts.CancelAfter(timeout);

            return await httpClient.GetAsync(url, cts.Token);
        }

        public static async Task<HttpResponseMessage> PostAsyncWithTimeout(this HttpClient httpClient, string url, object model, TimeSpan timeout)
        {
            var content = GetByteContent(model);

            if (timeout == null)
            {
                return await httpClient.PostAsync(url, content);
            }

            var cts = new CancellationTokenSource();
            cts.CancelAfter(timeout);

            return await httpClient.PostAsync(url, content);
        }

        public static ByteArrayContent GetByteContent(object obj)
        {
            string content = JsonConvert.SerializeObject(obj);
            var buffer = Encoding.UTF8.GetBytes(content);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return byteContent;
        }
    }
}
