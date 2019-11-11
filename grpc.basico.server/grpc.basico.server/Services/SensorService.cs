using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using sensoriamento.server.Datas;

namespace grpc.basico.server
{
    public class SensorService : Sensor.SensorBase
    {
        private readonly ILogger<SensorService> _logger;
        
        public SensorService(ILogger<SensorService> logger)
        {
            _logger = logger;
        }

        public override Task<SensorResponse> GetLastSensorData(SensorRequest request, ServerCallContext context)
        {
            SensorResponse sensor = new SensorResponse();

           var dados =  Dados.sensors.SingleOrDefault(_=> _.sensor_id == request.SensorId);

            sensor.PositionX = dados.position_x;
            sensor.PositionY = dados.position_y;
            sensor.Temperature = dados.temperatures.Last();

            return Task.FromResult(sensor);

        }

        public override async Task GetSensorData(SensorRequest request, IServerStreamWriter<SensorResponse> responseStream, ServerCallContext context)
        {
            List<SensorResponse> sensor = new List<SensorResponse>();

            var dados = Dados.sensors.FirstOrDefault(_ => _.sensor_id == request.SensorId);

           // TODO: PASSAR TEMPERATURAS DE FORMA ASSINCRONA PARA O STREAM

           
        }
    }
}
