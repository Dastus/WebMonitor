//using MediatR;
//using Monitor.Application.Interfaces;
//using Monitor.Application.MonitoringChecks.ChecksLogic;
//using Monitor.Application.MonitoringChecks.Models;
//using System;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Monitor.Application.MonitoringChecks.Commands.Beta.IntegrationTests
//{
//    public class RunVsTestCheckCommand : IRequest<CommandResult>, ICommand<CommandResult>
//    {
//        public CheckSettings CheckSettings => new CheckSettings
//        {
//            Priority = PrioritiesEnum.Medium,
//            Service = "VsTest Example",
//            Type = CheckTypeEnum.VsTestExampleBeta,
//            EnvironmentId = (int) EnvironmentsEnum.Beta,
//            CheckFullDescription = "Запуск интеграционного теста"
//        };
//    }

//    public class RunVsTestCheckHandler : IRequestHandler<RunVsTestCheckCommand, CommandResult>
//    {
//        private readonly IUnitTestsProcessorService _processor;

//        public RunVsTestCheckHandler(IUnitTestsProcessorService processor)
//        {
//            _processor = processor ?? throw new ArgumentNullException(nameof(IUnitTestsProcessorService));
//        }

//        public async Task<CommandResult> Handle(RunVsTestCheckCommand request, CancellationToken cancellationToken)
//        {
//            var path = @"E:\Vodolazkiy\autodoc\XUnitIntegrationTests";
//            //var path = @"E:\Vodolazkiy\autodoc\XUnitIntegrationTests\bin\Debug\netcoreapp2.2\Autodoc.IntegrationTests.dll";
//            var testName = "Get_EndpointsReturnSuccessAndCorrectContentType";

//            var result = new CommandResult { Success = true };
//            var check = new UnitTestCheck(_processor);
//            result.CheckModel = await check.RunMsUnitTest(request.CheckSettings, testName, path);

//            //result.CheckModel = await check.RunVsUnitTest(request.CheckSettings, testName, path);
//            return result;
//        }
//    }
//}
