using AutoMapper;
using Microsoft.Extensions.Options;
using Monitor.Application.Interfaces;
using Monitor.Application.MonitoringChecks.Models;
using Monitor.Infrastructure.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Monitor.Infrastructure.Logger
{
    public class LogStashService : ILogStashService
    {
        private IMapper _mapper;
        private IOptions<LoggerSettings> _settings;
        private HttpClient _httpClient = new HttpClient();

        public LogStashService(
            IMapper mapper, 
            IOptions<LoggerSettings> settings
        )
        {
            _settings = settings ?? throw new ArgumentNullException("logger settings are empty");
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task SaveLog(CommandResult result)
        {
            var record = _mapper.Map<Check, LogStashCheckRecord>(result.CheckModel);
            await PostLogTaskAsync(record);
        }

        async Task PostLogTaskAsync(LogStashCheckRecord entry)
        {
            var endpoint = GetEndpoint(entry);
            var content = ConvertToByteArray(entry);
            await _httpClient.PostAsync(endpoint, content);
        }

        private string GetEndpoint(LogStashCheckRecord entry) => 
            (entry.EnvironmentId == (int)EnvironmentsEnum.Prod) ? _settings.Value.LogStashEndpointBeta : _settings.Value.LogStashEndpointProd;

        private ByteArrayContent ConvertToByteArray(LogStashCheckRecord entry)
        {
            var jsonEntry = JsonConvert.SerializeObject(entry);
            var buffer = Encoding.UTF8.GetBytes(jsonEntry);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return byteContent;
        }

    }
}
