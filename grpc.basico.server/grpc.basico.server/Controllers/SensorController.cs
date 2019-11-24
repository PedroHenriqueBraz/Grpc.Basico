using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using sensoriamento.server.Sensores;

namespace grpc.basico.server
{
    public class SensorController : SensorService.SensorServiceBase
    {
        private readonly ILogger<SensorController> _logger;
        
        public SensorController(ILogger<SensorController> logger)
        {
            _logger = logger;
        }

        public override Task<SensorResponse> GetSensorData(SensorRequest request, ServerCallContext context)
        {
            foreach (var entry in context.RequestHeaders)
            {
                Console.WriteLine(entry);
            }

            SensorResponse sensor = new SensorResponse();
            sensor.Sensor = Sensores.sensores.Find(s => s.Id == request.SensorId);
            
            return Task.FromResult(sensor);
        }

    }
}
