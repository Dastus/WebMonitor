using Monitor.Application.MonitoringChecks.Models;
using System;

namespace Monitor.Application.MonitoringChecks.Helpers
{
    public class EnvironmentHelper
    {
        public string GetEnvironmentUrl(int environmentId)
        {
            //TO DO: move Urls to config
            switch((EnvironmentsEnum)environmentId)
            {
                case EnvironmentsEnum.Prod:
                    return @"https://autodoc.ua";
                case EnvironmentsEnum.Beta:
                    return @"https://beta.autodoc.ua";
                default:
                    throw new ArgumentException($"Incorrect {nameof(environmentId)} specified");
            }
        }

        public string GetApiUrl(int environmentId)
        {
            //TO DO: move Urls to config
            switch ((EnvironmentsEnum)environmentId)
            {
                case EnvironmentsEnum.Prod:
                    return @"https://api-backend.autodoc.ua";
                case EnvironmentsEnum.Beta:
                    return @"https://beta.api-backend.autodoc.ua"; //@"https://beta.autodoc.ua";//
                default:
                    throw new ArgumentException($"Incorrect {nameof(environmentId)} specified");
            }
        }
    }
}
