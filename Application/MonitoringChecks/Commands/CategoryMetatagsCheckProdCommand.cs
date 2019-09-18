using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;

namespace Monitor.Application.MonitoringChecks
{
    public class CategoryMetatagsCheckProdCommand : IRequest<CommandResult>,  ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Medium,
            Service = "Meta tags check",
            Type = CheckTypeEnum.CategoryMetaTagsProd,
            EnvironmentId = (int)EnvironmentsEnum.Prod,
            CheckFullDescription = "Проверка тегов rel, next, canonical, index, follow"
        };
    }
}
