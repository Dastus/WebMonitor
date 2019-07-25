using Monitor.Application.MonitoringChecks.Commands;
using Monitor.Application.MonitoringChecks.Models;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Helpers;

namespace Monitor.Application.MonitoringChecks
{
    public class WebUISearchCheck
    {
        private IWebDriversFactory _driversFactory;

        public WebUISearchCheck(IWebDriversFactory driversFactory)
        {
            _driversFactory = driversFactory;
        }

        public async Task<Check> CheckWebUISearch(EnvironmentsEnum environment)
        {
            var errorMessages = new List<string>();
            var environmentId = (int)environment;
            var result = new Check
            {
                Priority = PrioritiesEnum.Critical,
                Service = "Web UI search",
                Type = (environment == EnvironmentsEnum.Prod) ?
                    CheckTypeEnum.WebUISearchProd : CheckTypeEnum.WebUISearchBeta,
                LastCheckTime = DateTime.Now,
                EnvironmentId = environmentId
            };

            var baseUrl = new EnvironmentHelper().GetEnvironmentUrl(environmentId);

            using (var driver = _driversFactory.GetChromeDriver())
            {
                driver.Navigate().GoToUrl(baseUrl);
                var searchInput = driver.FindElementById("search-autocomplete");
                searchInput.SendKeys("oc90");
                var searchBtn = driver.FindElementByClassName("search-form__submit");
                searchBtn.Click();

                await Task.Delay(1000);
                var currentUrl = driver.Url;

                if (currentUrl != baseUrl + "/search-result?searchPhrase=oc90")
                {
                    errorMessages.Add("URL отличается от ожидаемого. Результат: " + currentUrl);
                }
            }

            if (errorMessages.Count > 0)
            {
                result.Status = StatusesEnum.CRITICAL;
                result.Description = "Обнаружены следующие проблемы: " + string.Join(", ", errorMessages);
                return result;
            }

            result.Status = StatusesEnum.OK;
            result.Description = "Проблем не обнаружено";

            return result;
        }
    }
}
