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
using Microsoft.Extensions.Hosting;

namespace Monitor.Infrastructure.Logger
{
    public class TextLoggerService : ILoggerService
    {
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private IOptions<LoggerSettings> _settings;
        private TimeSpan _interval = TimeSpan.FromSeconds(5);
        private ConcurrentQueue<LogRecord> concurrentQueue = new ConcurrentQueue<LogRecord>();
        private int _startedFlag = 0;

        public TextLoggerService(IOptions<LoggerSettings> settings)
        {
            _settings = settings ?? throw new ArgumentNullException("logger settings are empty");
        }

        public async Task SaveLog(CommandResult result)
        {
            var record = GetLogRecord(result);
            concurrentQueue.Enqueue(record);

            if (_startedFlag > 0)
            {
                return;
            }

            Interlocked.Increment(ref _startedFlag);
            await StartSchedule(_cts.Token);
        }

        private async Task StartSchedule(CancellationToken token)
        {
            await ProcessQueue();
            if (token.IsCancellationRequested)
            {
                return;
            }
            await Task.Delay(_interval);
            await StartSchedule(token);
        }

        private async Task ProcessQueue()
        {
            if (concurrentQueue.IsEmpty)
            {
                return;
            }

            var logs = new List<LogRecord>();
            while (!concurrentQueue.IsEmpty)
            {
                var dequed = concurrentQueue.TryDequeue(out var record);
                if (dequed)
                {
                    logs.Add(record);
                }                    
            }

            await WriteLogsAsync(logs);           
        }

        private async Task WriteLogsAsync(IEnumerable<LogRecord> logs)
        {
            var logGroups = logs.GroupBy(x => x.FilePath);

            foreach(var logsList in logGroups)
            {
                var filePath = logsList.FirstOrDefault().FilePath;
                var messages = logsList.Select(x => x.Message);

                if (!File.Exists(filePath))
                {
                    PrepareLogFile(filePath);
                } 

                await File.AppendAllLinesAsync(filePath, messages);
            }            
        }

        private void PrepareLogFile(string filePath)
        {    
            var dirPathYear = Path.Combine(_settings.Value.LogDirectoryPath, DateTime.Now.Year.ToString());
            var dirPathMonth = Path.Combine(dirPathYear, DateTime.Now.Month.ToString());              

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
                var head = "DateTime | Check | Success | Description | Duration | DiagInfo" + Environment.NewLine;
                File.AppendAllText(filePath, head);
            }
        }

        private LogRecord GetLogRecord(CommandResult result)
        {
            var filePath = GetFilePath(result);
            
            string dateTime = result.CheckModel?.State.LastCheckTime.ToString() ?? DateTime.Now.ToString();
            string check = result.CheckModel?.Settings.Service;
            string env = GetEnvironmentName(result);
            string success = (result.Success) ? " Success" : " Fail";
            string errors = (result.Success) ? "" : String.Join(" ", result.Errors);
            string status = result.CheckModel?.State.Status.ToFriendlyString();
            string description = result.CheckModel?.State.Description;
            string duration = result.CheckModel?.State.ExecutionDuration.ToString();
            string diagnosticsInfo = result.CheckModel?.State.DiagnosticsInfo;

            var message = String.Join(" | ", dateTime, check, success, status, errors, description, duration, diagnosticsInfo);

            return new LogRecord { Message = message, FilePath = filePath };
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
                env = (result.CheckModel.Settings.EnvironmentId == (int)EnvironmentsEnum.Prod) ? "prod" : "beta";
            }
            else
            {
                env = "unknown";
            }

            return env;
        }
    }

    internal struct LogRecord
    {
        public string Message {get; set; }
        public string FilePath {get; set; }
    }
}
