using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Reflection;

namespace Monitor.Infrastructure.Selenium
{
    public class SeleniumDriversFactory : IWebDriversFactory
    {
        public SeleniumDriversFactory()
        {

        }

        public ChromeDriver GetChromeDriver()
        {
            var assemblyPath = Assembly.GetAssembly(typeof(Check)).Location;

            //TODO: move to configuration
            var path = Path.Combine(Environment.CurrentDirectory, "StaticFiles");

            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");

            return new ChromeDriver(path, chromeOptions);
        }
    }
}
