using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
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

        public override async Task GetAllData(Empty request, IServerStreamWriter<SensorResponse> responseStream, 
            ServerCallContext context)
        {
            foreach (var sensor in Sensores.sensores)
            {
                await Task.Delay(5000);
                await responseStream.WriteAsync(new SensorResponse()
                {
                    Sensor = sensor
                });
            }
        }

        public override async Task<AddPhotoResponse> AddPhoto(IAsyncStreamReader<AddPhotoRequest> requestStream, 
            ServerCallContext context)
        {
            var data = new List<byte>();
            
            while (await requestStream.MoveNext())
            {
                Console.WriteLine("Received " +
                    requestStream.Current.Data.Length + " bytes");
                data.AddRange(requestStream.Current.Data);
            }
            Console.WriteLine("Received file with " + data.Count + " bytes");

            return new AddPhotoResponse()
            {
                IsOk = true
            };
        }

        public override async Task AddNewSensor(IAsyncStreamReader<SensorAddRequest> requestStream, 
            IServerStreamWriter<SensorResponse> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var newSensor = requestStream.Current.Sensor;

                lock (this)
                {
                    Sensores.sensores.Add(newSensor);
                }

                await responseStream.WriteAsync(new SensorResponse()
                {
                    Sensor = newSensor
                });
            }

            foreach (var sensor in Sensores.sensores)
            {
                Console.WriteLine(sensor);
            }
        }
    }
}
