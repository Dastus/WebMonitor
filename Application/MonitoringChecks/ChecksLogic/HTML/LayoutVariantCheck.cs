using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Extensions;
using Monitor.Application.MonitoringChecks.Helpers;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks.ChecksLogic.HTML
{
    class LayoutVariantCheck
    {
        private readonly IHttpRequestService _httpService;

        public LayoutVariantCheck(IHttpRequestService httpService)
        {
            _httpService = httpService;
        }

        public async Task<Check> PerformCheck(CheckSettings settings, bool isMobile = false)
        {
            var expectedMarker = isMobile ? "MOBILE" : "DESKTOP";
            var result = new Check { Settings = settings };
            result.State.LastCheckTime = DateTime.Now;
            var requestTimeout = TimeSpan.FromSeconds(30);

            string address = new EnvironmentHelper().GetEnvironmentUrl(settings.EnvironmentId);

            try
            {
                var warningThreshold = 3;
                var errors = new List<string>();

                var startTime = DateTime.Now;
                var htmlResult = (isMobile) 
                    ? await _httpService.GetHtmlStructureAsGoogleBotMobile(address, requestTimeout)
                    : await _httpService.GetHtmlStructureAsGoogleBotDesktop(address, requestTimeout);
                var endTime = DateTime.Now;

                var footerMarker = htmlResult.FindNodesByClassName("div","footer__holder").FirstOrDefault()?.FirstChild;
                if (footerMarker == null)
                {
                    errors.Add("footer MOBILE marker not found");
                }
                if (footerMarker.InnerText != expectedMarker)
                {
                    errors.Add($"Expected footer marker is '{expectedMarker}'. Received is '{footerMarker.InnerText}'");
                }

                var appMarker = htmlResult.FindNodesByClassName("div", "wrapper__holder").FirstOrDefault()?.FirstChild;
                if (appMarker == null)
                {
                    errors.Add("app MOBILE marker not found");
                }
                if (appMarker.InnerText != expectedMarker)
                {
                    errors.Add($"Expected app marker is '{expectedMarker}'. Received is '{footerMarker.InnerText}'");
                }

                if (errors.Count() > 0)
                {
                    result.State.Status = StatusesEnum.CRITICAL;
                    result.State.Description = "Обнаружены следующие проблемы: " + string.Join(",", errors);
                    return result;
                }

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
            catch (Exception ex)
            {
                Console.WriteLine(ex);//
                result.State.Status = StatusesEnum.CRITICAL;
                result.State.Description = "Ошибка при обработке HTML";
            }

            return result;
        }
    }
}
