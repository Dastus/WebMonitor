using Monitor.Application.MonitoringChecks.Models;
using System.Threading.Tasks;
using System;
using System.Linq;
using Monitor.Application.Interfaces;
using System.Collections.Generic;
using Monitor.Application.MonitoringChecks.Extensions;
using Monitor.Application.MonitoringChecks.Helpers;

namespace Monitor.Application.MonitoringChecks
{
    public class MetatagsCheck
    {
        private readonly IHttpRequestService _httpService;

        public MetatagsCheck(IHttpRequestService httpService)
        {
            _httpService = httpService;
        }

        public async Task<Check> CheckMetaInfo(EnvironmentsEnum environment)
        {
            var environmentId = (int)environment;
            var result = new Check
            {
                Priority = PrioritiesEnum.Medium,
                Service = "Meta tags check",
                Type = (environment == EnvironmentsEnum.Prod) ?
                    CheckTypeEnum.MetaTagsProd : CheckTypeEnum.MetaTagsBeta,
                LastCheckTime = DateTime.Now,
                EnvironmentId = environmentId
            };

            string address = new EnvironmentHelper().GetEnvironmentUrl(environmentId) + "/category/shiny-id49-3";

            try
            {
                var warningThreshold = 3;
                var errors = new List<string>();

                var startTime = DateTime.Now;
                var htmlResult = await _httpService.GetHtmlStructure(address, TimeSpan.FromSeconds(60));
                var endTime = DateTime.Now;

                var title = htmlResult.GetTitle();

                if (title != "Шины от autodoc.ua (оригинал, аналог) | autodoc.ua")
                {
                    errors.Add("title incorrect: " + title);
                }

                var robots = htmlResult.GetMetaTagContent("robots");

                if (robots != "index, follow")
                {
                    errors.Add("'robots' tag content incorrect: " + robots);
                }

                var keywords = htmlResult.GetMetaTagContent("keywords");

                if (keywords != "шины")
                {
                    errors.Add("'keywords' tag content incorrect: " + keywords);
                }

                var canonicalLink = htmlResult.GetLinkContent("canonical");

                if (canonicalLink != address)
                {
                    errors.Add("'link' for rel='canonical' is incorrect: " + canonicalLink);
                }

                var nextLink = htmlResult.GetLinkContent("next");

                if (nextLink != address + "?page=2")
                {
                    errors.Add("'link' for rel='next' is incorrect: " + nextLink);
                }

                var prevLink = htmlResult.GetLinkContent("prev");

                if (prevLink != null)
                {
                    errors.Add("'link' for rel='prev' is present on page 1:" + prevLink);
                }

                if (errors.Count() > 0)
                {
                    result.Status = StatusesEnum.CRITICAL;
                    result.Description = "Обнаружены следующие проблемы: " + string.Join(",", errors);
                    return result;                }

                var execTime = endTime - startTime;
                if (execTime > TimeSpan.FromSeconds(warningThreshold))
                {
                    result.Status = StatusesEnum.WARNING;
                    result.Description = "Время ответа больше порога " + warningThreshold + " сек: " + execTime.Seconds;
                    return result;
                }

                result.Status = StatusesEnum.OK;
                result.Description = "Проблем не обнаружено";
            }
            catch
            {
                result.Status = StatusesEnum.CRITICAL;
                result.Description = "Ошибка при обработке HTML";
            }

            return result;
        }
    }
}
