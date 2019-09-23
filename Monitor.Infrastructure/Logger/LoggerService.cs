using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Monitor.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Monitor.Infrastructure.Logger
{
    public class LoggerService : ILoggerService
    {
        private IOptions<LoggerSettings> _settings;
        private HashSet<string> _createdFiles = new HashSet<string>();
        private ConcurrentDictionary<string, ReaderWriterLockSlim> _locksMap = new ConcurrentDictionary<string, ReaderWriterLockSlim>();

        public LoggerService(IOptions<LoggerSettings> settings)
        {
            _settings = settings ?? throw new ArgumentNullException("logger settings are empty");
        }

        public async Task SaveLog(CommandResult result)
        {
            await Task.Run(() => {
                var filePath = GetFilePath(result);

                if (_locksMap.ContainsKey(filePath))
                {
                    WriteToFile(filePath, result);
                    return;
                } 

                var fileLock = new ReaderWriterLockSlim();
                _locksMap.TryAdd(filePath, fileLock);

                if (!File.Exists(filePath))
                {
                    PrepareLogFile(result);
                    WriteToFile(filePath, result);
                }

            }).ConfigureAwait(false);            
        }

        private void PrepareLogFile(CommandResult result)
        {    
            var dirPathYear = Path.Combine(_settings.Value.LogDirectoryPath, DateTime.Now.Year.ToString());
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
                var file = File.Create(filePath);
                file.Close();//
                var head = "DateTime | Check | Success | Description | Duration | DiagInfo";
                PerformSafeWrite(filePath, head);
            }
        }

        private void WriteToFile(string filePath, CommandResult result)
        {
            string dateTime = result.CheckModel?.State.LastCheckTime.ToString() ?? DateTime.Now.ToString();
            string check = result.CheckModel?.Settings.Service;
            string env = GetEnvironmentName(result);
            string success = ((result.Success) ? " Success" : " Fail");
            string errors = (result.Success) ? "" : String.Join(" ", result.Errors);
            string status = result.CheckModel?.State.Status.ToFriendlyString();
            string description = result.CheckModel?.State.Description;
            string duration = result.CheckModel?.State.ExecutionDuration.ToString();
            string diagnosticsInfo = result.CheckModel?.State.DiagnosticsInfo;

            var logRecord = String.Join(" | ", dateTime, check, success, status, errors, description, duration, diagnosticsInfo);

            PerformSafeWrite(filePath, logRecord);
        }
 
        private void PerformSafeWrite(string filePath, string logRecord)
        {
            var fileLock = _locksMap[filePath];

            try
            {                
                fileLock.EnterWriteLock();
                File.AppendAllLines(filePath, new string[] { logRecord });
            }
            finally
            {
                fileLock.ExitWriteLock();
            }
        }

        private string GetFilePath(CommandResult result)
        {
            return Path.Combine(
                 _settings.Value.LogDirectoryPath,
                 DateTime.Now.Year.ToString(),
                 DateTime.Now.Month.ToString(),
                 GetFileName(result)
                );
        }

        private string GetFileName(CommandResult result) => DateTime.Now.Day.ToString() + "_" + GetEnvironmentName(result) + ".txt";


        private string GetEnvironmentName(CommandResult result)
        {
            string env;
            if (result.CheckModel?.Settings.EnvironmentId != null)
            {
                env = (result.CheckModel.Settings.EnvironmentId == (int)EnvironmentsEnum.Prod) ? " prod" : "beta";
            }
            else
            {
                env = "unknown";
            }

            return env;
        }
    }
}
