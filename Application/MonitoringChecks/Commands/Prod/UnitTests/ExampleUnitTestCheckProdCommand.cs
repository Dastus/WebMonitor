using MediatR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.ChecksLogic;
using Monitor.Application.MonitoringChecks.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Monitor.Application.MonitoringChecks.Commands
{
    public class ExampleUnitTestCheckProdCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Low,
            Service = "Unit test execution: 'TestMethodPositive'",
            Type = CheckTypeEnum.ExampleUnitTestCheckProd,
            EnvironmentId = (int)EnvironmentsEnum.Prod,
            CheckFullDescription = "Результаты запуска юнит теста 'TestMethodPositive'"
        };
    }

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
            result.CheckModel = await check.RunMsUnitTest(request.CheckSettings, testName, path);
            //result.CheckModel.State.Status = StatusesEnum.CRITICAL;//test
            return result;
        }
    }
}
