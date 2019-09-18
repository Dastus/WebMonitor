using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Monitor.Infrastructure.CommandsRunner
{
    public class ConsoleCommandsRunner
    {
        public async Task<ProcessExecutionResult> RunProcessAsync(string fileName, string args)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = fileName,
                    Arguments = args,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                },
                EnableRaisingEvents = true
            };

            try
            {
                using (process)
                {
                    return await RunProcessAsync(process).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                return new ProcessExecutionResult { Success = false, ErrorMessage = ex.Message };
            }
        }

        private Task<ProcessExecutionResult> RunProcessAsync(Process process)
        {
            var tcs = new TaskCompletionSource<ProcessExecutionResult>();
            string errorMsg = "";
            string outputMsg = "";

            process.Exited += (s, ea) => tcs.SetResult(new ProcessExecutionResult
            {
                Success = (process.ExitCode == 0),
                ErrorMessage = errorMsg,
                OutputMessage = outputMsg
            });

            process.OutputDataReceived += (s, ea) => {
                if (ea.Data != null)
                {
                    outputMsg += (ea.Data + "\n");
                }
            };

            process.ErrorDataReceived += (s, ea) => {
                if (ea.Data != null)
                {
                    errorMsg += (ea.Data + "\n");
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return tcs.Task;
        }
    }

    public class ProcessExecutionResult
    {
        public bool Success;
        public string OutputMessage;
        public string ErrorMessage;
    }
}
