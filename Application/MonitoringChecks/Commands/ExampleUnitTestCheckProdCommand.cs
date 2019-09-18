using MediatR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;


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
}
