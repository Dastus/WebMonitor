using Monitor.Application.MonitoringChecks.Models;
using System.Threading.Tasks;
using System;
using System.Linq;
using Monitor.Application.Interfaces;
using System.Collections.Generic;
using Monitor.Application.MonitoringChecks.Extensions;
using Monitor.Application.MonitoringChecks.Helpers;
using System.Net.Http;

namespace Monitor.Application.MonitoringChecks
{
    public class CategoryMetatagsCheck
    {
        private readonly IHttpRequestService _httpService;

        public CategoryMetatagsCheck(IHttpRequestService httpService)
        {
            _httpService = httpService;
        }

        public async Task<Check> CheckMetaInfo(CheckSettings settings)
        {
            var result = new Check{ Settings = settings };
            result.State.LastCheckTime = DateTime.Now;
            var requestTimeout = TimeSpan.FromSeconds(45);

            string address = new EnvironmentHelper().GetEnvironmentUrl(settings.EnvironmentId) + "/category/shiny-id49-3";

            try
            {
                var warningThreshold = 3;
                var errors = new List<string>();

                var startTime = DateTime.Now;
                var htmlResult = await _httpService.GetHtmlStructureAsGoogleBotDesktop(address, requestTimeout);
                var endTime = DateTime.Now;

                var title = htmlResult.GetTitle();

                if (title != "Шины от autodoc.ua (оригинал, аналог) | autodoc.ua")
                {
                    errors.Add("title incorrect: " + title);
                }

                var robots = htmlResult.GetMetaTagContent("robots");

                if (settings.EnvironmentId == (int)EnvironmentsEnum.Prod && robots != "index, follow")
                {
                    errors.Add("'robots' tag content incorrect: " + robots);
                }

                var keywords = htmlResult.GetMetaTagContent("keywords");

                if (keywords != "шины")
                {
                    errors.Add("'keywords' tag content incorrect: " + keywords);
                }

                var description = htmlResult.GetMetaTagContent("description");

                if (string.IsNullOrWhiteSpace(description))
                {
                    errors.Add("'description' tag content incorrect: " + description);
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
                    result.State.Status = StatusesEnum.CRITICAL;
                    result.State.Description = "Обнаружены следующие проблемы: " + string.Join(",", errors);
                    return result;                }

                var execTime = endTime - startTime;
                if (execTime > TimeSpan.FromSeconds(warningThreshold))
                {
                    result.State.Status = StatusesEnum.WARNING;
                    result.State.Description = string.Format("Время ответа больше порога {0} сек: {1:0.00}", warningThreshold, execTime.TotalSeconds);
                    return result;
                }

                result.State.Status = StatusesEnum.OK;
                result.State.Description = "Проблем не обнаружено";
            }
            catch (TaskCanceledException)
            {
                result.State.Status = StatusesEnum.CRITICAL;
                result.State.Description = string.Format("Превышен интервал выполнения запроса: {0} сек", requestTimeout.Seconds);
            }
            catch (HttpRequestException ex)
            {
                result.State.Status = StatusesEnum.CRITICAL;
                result.State.Description = "Ошибка http-запроса: " + ex.Message;
            }
            catch
            {
                result.State.Status = StatusesEnum.CRITICAL;
                result.State.Description = "Ошибка при обработке HTML";
            }

            return result;
        }
    }
}
