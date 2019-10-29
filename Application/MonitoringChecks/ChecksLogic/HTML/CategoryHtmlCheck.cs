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

namespace Monitor.Application.MonitoringChecks.ChecksLogic
{
    public class CategoryHtmlCheck
    {
        private readonly IHttpRequestService _httpService;

        public CategoryHtmlCheck(IHttpRequestService httpService)
        {
            _httpService = httpService;
        }

        public async Task<Check> CheckCategoryInfo(CheckSettings settings)
        {
            var result = new Check { Settings = settings };
            result.State.LastCheckTime = DateTime.Now;
            var requestTimeout = TimeSpan.FromSeconds(45);

            string address = new EnvironmentHelper().GetEnvironmentUrl(settings.EnvironmentId) + "/category/kolenval-i-komplektuyushhie-id53-3";

            try
            {
                var warningThreshold = 3;
                var errors = new List<string>();

                var startTime = DateTime.Now;
                var htmlResult = await _httpService.GetHtmlStructureAsGoogleBotDesktop(address, requestTimeout);
                var endTime = DateTime.Now;

                var breadcrumbs = htmlResult.FindNodesByName("autodoc-breadcrumbs").FirstOrDefault();
                if (breadcrumbs == null)
                {
                    errors.Add("breadcrumbs element not found");
                }

                var breadcumbStrings = new List<string> {
                    "href=\"/\"", //home
                    "href=\"/category/dvigatel-sistemy-i-komponenty-id1-3\"", //Двигатель, системы и компоненты
                    "href=\"/category/blok-porshnevaya-gruppa-id8-3\"", //Блок, поршневая группа
                };

                var missingCrumbs = new List<string>() as IEnumerable<string>;
                if (breadcrumbs !=null && !breadcrumbs.InnerHtml.ContainsAll(breadcumbStrings, out missingCrumbs))
                {
                    errors.Add("Following breadcrumbs aren't found: " + string.Join(", ", missingCrumbs));
                }

                var subcategoriesMenu = htmlResult.FindNodesByName("catalog-subs").FirstOrDefault();
                if (subcategoriesMenu == null)
                {
                    errors.Add("catalog subcategories menu element not found");
                }

                var subcategoryItems = new List<string>
                {
                    "href=\"/category/kolenval-id126-3\"",
                    "href=\"/category/podshipnik-kolenvala-id127-3\""
                };

                var missingSubcategories = new List<string>() as IEnumerable<string>;
                if (subcategoriesMenu != null && !subcategoriesMenu.InnerHtml.ContainsAll(subcategoryItems, out missingSubcategories))
                {
                    errors.Add("Following subcategories aren't found: " + string.Join(", ", missingSubcategories));
                }

                var paging = htmlResult.FindNodesByName("seo-paging").FirstOrDefault();
                if (paging == null)
                {
                    errors.Add("paging element not found");
                }

                var pageTwoElementCode = "href=\"/category/kolenval-i-komplektuyushhie-id53-3?page=2";
                if (paging != null && !paging.InnerHtml.Contains(pageTwoElementCode))
                {
                    errors.Add("paging doesn't contain page 2");
                }
                
                var carFiltersPanel = htmlResult.FindNodesByName("car-filter-panel").FirstOrDefault();
                if (carFiltersPanel == null)
                {
                    errors.Add("car filters panel not found");
                }

                var filters = htmlResult.FindNodesByName("generic-filter");
                if (filters == null || filters.Count() == 0)
                {
                    errors.Add("generic filters not found");
                }

                var combobox = htmlResult.FindNodesByName("filter-combobox");
                if (combobox == null || combobox.Count() == 0)
                {
                    errors.Add("brands not found");
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
