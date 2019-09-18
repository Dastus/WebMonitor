using MediatR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;

namespace Monitor.Application.MonitoringChecks.Commands
{

    public class LoginAdminCheckProdCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Medium,
            Service = "Unit test execution: 'AutodocAutoTest.TestsCases.LoginAdminka'",
            Type = CheckTypeEnum.LoginAdminPanelUnitTestProd,
            EnvironmentId = (int)EnvironmentsEnum.Prod,
            CheckFullDescription = "Результаты запуска юнит теста 'AutodocAutoTest.TestsCases.LoginAdminka'"
        };
    }
}
