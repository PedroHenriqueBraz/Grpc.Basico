using grpc.basico.server;
using Grpc.Net.Client;
using System;
using System.Threading.Tasks;

namespace sensoriamento.client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");

            var sensorClient = new Sensor.SensorClient(channel);

            var sensorData = await sensorClient.GetLastSensorDataAsync(new SensorRequest() { SensorId = 1});

            Console.WriteLine($"x: {sensorData.PositionX} \n y: {sensorData.PositionY} \n temperatura: {sensorData.Temperature}");
        }
    }
}
