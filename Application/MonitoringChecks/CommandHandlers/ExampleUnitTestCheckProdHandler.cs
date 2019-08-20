using MediatR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.ChecksLogic;
using Monitor.Application.MonitoringChecks.Commands;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks.CommandHandlers
{

    public class ExampleUnitTestCheckProdHandler : IRequestHandler<ExampleUnitTestCheckProdCommand, CommandResult>
    {
        private readonly IUnitTestsProcessorService _processor;

        public ExampleUnitTestCheckProdHandler(IUnitTestsProcessorService processor)
        {
            _processor = processor ?? throw new ArgumentNullException(nameof(IUnitTestsProcessorService));
        }

        public async Task<CommandResult> Handle(ExampleUnitTestCheckProdCommand request, CancellationToken cancellationToken)
        {
            var path = @"E:\Vodolazkiy\test\UnitTestsSolution\UnitTestsProject";
            var testName = "TestMethodPositive";

            var result = new CommandResult { Success = true };
            var check = new UnitTestCheck(_processor);
            result.CheckModel = await check.RunUnitTest(request.CheckSettings, testName, path);
            return result;
        }
    }
}
