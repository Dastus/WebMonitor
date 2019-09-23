using System;
using System.Collections.Generic;
using System.Text;

namespace Monitor.Infrastructure.Settings
{
    public class LoggerSettings
    {
        public string LogDirectoryPath { get; set; }
        public string LogStashEndpointProd { get; set; }
        public string LogStashEndpointBeta { get; set; }
    }
}
