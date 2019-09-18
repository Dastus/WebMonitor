using MediatR;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;

namespace Monitor.Application.MonitoringChecks.Commands
{
    public class ProductHtmlCheckProdCommand : IRequest<CommandResult>, ICommand<CommandResult>
    {
        public CheckSettings CheckSettings => new CheckSettings
        {
            Priority = PrioritiesEnum.Critical,
            Service = "Product HTML structure check",
            Type = CheckTypeEnum.ProductHtmlCheckProd,
            EnvironmentId = (int)EnvironmentsEnum.Prod,
            CheckFullDescription = "Проверка HTML структуры для product/filtr-maslyanyj-dvigatelya-lanos-aveo-lacetti-nubira-nexia-pr-vo-knecht-mahle-id-84669-0-170"
        };
    }
}
