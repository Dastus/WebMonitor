using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Extensions;
using Monitor.Application.MonitoringChecks.Helpers;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks.ChecksLogic
{
    public class ProductHtmlCheck
    {
        private readonly IHttpRequestService _httpService;

        public ProductHtmlCheck(IHttpRequestService httpService)
        {
            _httpService = httpService;
        }

        public async Task<Check> CheckCategoryInfo(CheckSettings settings)
        {
            var result = new Check { Settings = settings };
            result.State.LastCheckTime = DateTime.Now;
            var requestTimeout = TimeSpan.FromSeconds(30);

            string address = new EnvironmentHelper().GetEnvironmentUrl(settings.EnvironmentId) + "/product/filtr-maslyanyj-dvigatelya-lanos-aveo-lacetti-nubira-nexia-pr-vo-knecht-mahle-id-84669-0-170";

            try
            {
                var warningThreshold = 3;
                var errors = new List<string>();

                var startTime = DateTime.Now;
                var htmlResult = await _httpService.GetHtmlStructureAsGoogleBot(address, requestTimeout);
                var endTime = DateTime.Now;

                var breadcrumbs = htmlResult.FindNodesByName("autodoc-breadcrumbs").FirstOrDefault();
                if (breadcrumbs == null)
                {
                    errors.Add("breadcrumbs element not found");
                }

                var expectedCrumbs = new List<string> {
                    "href=\"/\"", //home
                    "href=\"/category/dvigatel-sistemy-i-komponenty-id1-3\"", //Двигатель, системы и компоненты
                    "href=\"/category/sistema-smazki-id57-3\"", //Система смазки      
                    "href=\"/category/maslyanyj-filtr-id170-3\"", //Масляный фильтр
                };

                var missingCrumbs = new List<string>() as IEnumerable<string>;
                if (breadcrumbs !=null && !breadcrumbs.InnerHtml.ContainsAll(expectedCrumbs, out missingCrumbs))
                {
                    errors.Add("Following breadcrumbs aren't found: " + string.Join(", ", missingCrumbs));
                }

                //check h1
                var h1 = htmlResult.FindNodesByName("h1").FirstOrDefault();
                if (h1 != null && h1.InnerHtml != "Фильтр масляный двигателя LANOS, AVEO, LACETTI, NUBIRA, NEXIA (пр-во KNECHT-MAHLE). OC90")
                {
                    errors.Add("h1 differs from expected: " + h1.InnerHtml);
                }
                
                //check price                
                var price = htmlResult.FindNodesByClassName("span", "product-section__new-price").FirstOrDefault();
                var isNumber = float.TryParse(price?.InnerHtml, NumberStyles.Any, CultureInfo.InvariantCulture, out var num);
                if (!isNumber)
                {
                    errors.Add("price not found");
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
