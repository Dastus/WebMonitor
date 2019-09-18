using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Monitor.Infrastructure.Http
{
    public class HttpRequestService : IHttpRequestService
    {
        private HttpClient _userHttpClient = new HttpClient();
        private HttpClient _googleBotHttpClient = new HttpClient {
            DefaultRequestHeaders = { { "User-Agent", "Googlebot/2.1 (+http://www.google.com/bot.html)" } }
        };

        public async Task<HtmlDocument> GetHtmlStructure(string url, TimeSpan timeout)
        {
            var result = await _userHttpClient.GetAsyncWithTimeout(url, timeout);
            var stream = await result.Content.ReadAsStreamAsync();

            var doc = new HtmlDocument();
            doc.Load(stream);

            return doc;
        }

        public async Task<HtmlDocument> GetHtmlStructureAsGoogleBot(string url, TimeSpan timeout)
        {
            var result = await _googleBotHttpClient.GetAsyncWithTimeout(url, timeout);
            var stream = await result.Content.ReadAsStreamAsync();

            var doc = new HtmlDocument();
            doc.Load(stream);

            return doc;
        }

        public async Task<WebPageLoadResult> GetPageLoadResultAsGoogleBot(string url, TimeSpan timeout)
        {
            var startTime = DateTime.Now;
            var resp = await _googleBotHttpClient.GetAsyncWithTimeout(url, timeout);
            var endTime = DateTime.Now;

            return new WebPageLoadResult
            {
                ResponseStatus = resp.StatusCode,
                LoadTime = (endTime - startTime)
            };
        }

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
    }

    static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> GetAsyncWithTimeout(this HttpClient httpClient, string url, TimeSpan timeout)
        {
            if (timeout == null)
            {
                return await httpClient.GetAsync(url);
            }

            var cts = new CancellationTokenSource();
            cts.CancelAfter(timeout);

            return await httpClient.GetAsync(url, cts.Token);
        }
    }
}
