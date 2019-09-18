using MediatR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;

namespace Monitor.Application.MonitoringChecks.Commands
{

    public class LoginCheckProdCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Medium,
            Service = "Unit test execution: 'AutodocAutoTest.TestsCases.Login'",
            Type = CheckTypeEnum.LoginUnitTestProd,
            EnvironmentId = (int)EnvironmentsEnum.Prod,
            CheckFullDescription = "Результаты запуска юнит теста 'AutodocAutoTest.TestsCases.Login'"
        };
    }
}
