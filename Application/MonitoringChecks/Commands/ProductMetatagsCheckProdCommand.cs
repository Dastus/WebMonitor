using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using MediatR;

namespace Monitor.Application.MonitoringChecks
{
    public class ProductMetatagsCheckProdCommand : IRequest<CommandResult>,  ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Medium,
            Service = "Meta tags check for product",
            Type = CheckTypeEnum.ProductMetaTagsProd,
            EnvironmentId = (int)EnvironmentsEnum.Prod,
            CheckFullDescription = "Проверка тегов rel, next, canonical, index, follow на странице товара oc90"
        };
    }
}
