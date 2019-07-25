using Monitor.Application.MonitoringChecks.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Monitor.Application.Interfaces
{
    public interface IWebDriversFactory
    {
        ChromeDriver GetChromeDriver();
    }
}
