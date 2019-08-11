using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Infrastructure.Logger
{
    public class LoggerService : ILoggerService
    {
        //TODO: move to config
        const string LOG_PATH = @"E:\Vodolazkiy\test\Logs";

        public async Task SaveLog(CommandResult result)
        {
            var filePath = GetFilePath(result);
            if (File.Exists(filePath))
            {
                await WriteToFile(filePath, result);
            }
            await PrepareLogFile(result);
            await WriteToFile(filePath, result);
        }

        private async Task PrepareLogFile(CommandResult result)
        {
            var dirPathYear = Path.Combine(LOG_PATH, DateTime.Now.Year.ToString());
            var dirPathMonth = Path.Combine(dirPathYear, DateTime.Now.Month.ToString());
            var filePath = Path.Combine(dirPathMonth, GetFileName(result));

            if (!Directory.Exists(dirPathYear))
            {
                Directory.CreateDirectory(dirPathYear);
            }

            if (!Directory.Exists(dirPathMonth))
            {
                Directory.CreateDirectory(dirPathMonth);
            }

            if (!File.Exists(filePath))
            {
                File.Create(filePath);
                using (StreamWriter outputFile = File.AppendText(filePath))
                {
                    var head = "DateTime | Check | Success | Description | Duration";
                    await outputFile.WriteLineAsync(head);
                }
            }
        }

        private async Task WriteToFile(string filePath, CommandResult result)
        {
            using (StreamWriter outputFile = File.AppendText(filePath))
            {
                string dateTime = result.CheckModel.LastCheckTime.ToShortDateString();
                string check = result.CheckModel?.Service;
                string env = GetEnvironmentName(result);
                string success = ((result.Success) ? " Success" : " Fail");
                string errors = (result.Success) ? "" : String.Join(" ", result.Errors);
                string status = result.CheckModel?.Status.ToFriendlyString();
                string description = result.CheckModel?.Description;
                string duration = result.CheckModel?.ExecutionDuration.ToString();

                var logRecord = String.Join(" | ", dateTime, check, success, status, errors, description, duration);

                await outputFile.WriteLineAsync(logRecord);
            }
        }
        private string GetFilePath(CommandResult result)
        {
            return Path.Combine(
                LOG_PATH,
                 DateTime.Now.Year.ToString(),
                 DateTime.Now.Month.ToString(),
                 GetFileName(result)
                );
        }

        private string GetFileName(CommandResult result) => DateTime.Now.Day.ToString() + "_" + GetEnvironmentName(result) + ".txt";


        private string GetEnvironmentName(CommandResult result)
        {
            string env;
            if (result.CheckModel?.EnvironmentId != null)
            {
                env = (result.CheckModel.EnvironmentId == (int)EnvironmentsEnum.Prod) ? " prod" : "beta";
            }
            else
            {
                env = "unknown";
            }

            return env;
        }
    }
}
