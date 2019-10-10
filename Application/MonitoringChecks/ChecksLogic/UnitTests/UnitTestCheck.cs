using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks.ChecksLogic
{
    public class UnitTestCheck
    {
        private readonly IUnitTestsProcessorService _processor;

        public UnitTestCheck(IUnitTestsProcessorService processor)
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(IUnitTestsProcessorService));
        }

        public async Task<Check> RunMsUnitTest(CheckSettings checkSettings, string testName, string path)
        {
            var checkState = await _processor.ExecuteMsTest(testName, path);
            return new Check { Settings = checkSettings, State = checkState };
        }

        public async Task<Check> RunNUnitTest(CheckSettings checkSettings, string testName, string path)
        {
            var checkState = await _processor.ExecuteNUnitTest(testName, path);
            return new Check { Settings = checkSettings, State = checkState };
        }

        public async Task<Check> RunVsUnitTest(CheckSettings checkSettings, string testName, string path)
        {
            var checkState = await _processor.ExecuteVsTest(testName, path);
            return new Check { Settings = checkSettings, State = checkState };
        }
    }
}
