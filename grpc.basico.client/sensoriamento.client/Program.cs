using Google.Protobuf;
using grpc.basico.server;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace sensoriamento.client
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");

            var sensorClient = new SensorService.SensorServiceClient(channel);

            bool menu = true;
            while (menu)
            {
                menu = await Menu(sensorClient);
            }

        }

        private static async Task<bool> Menu(SensorService.SensorServiceClient sensorClient)
        {
            Console.WriteLine("\n 1 - Get sensor data \n " +
                               "2 - Get all sensors data \n " +
                               "3 - Upload file \n " +
                               "4 - Add new sensors \n " + 
                               "5 - Sair");

            var op = Convert.ToInt32(Console.ReadLine());

            switch (op)
            {
                case 1:
                    await GetSensorData(sensorClient);
                    return true;
                case 2:
                    await GetAllSensorData(sensorClient);
                    return true;
                case 3:
                    await UploadPhoto(sensorClient);
                    return true;
                case 4:
                    await AddNewSensor(sensorClient);
                    return true;
                case 5:
                    return false;
            }
            return false;
        }
        private static async Task GetSensorData(SensorService.SensorServiceClient sensorClient)
        {
            Metadata md = new Metadata();
            md.Add("my-token","testeToken123");

            Console.WriteLine("id: ");
            var sensorId = Convert.ToInt32(Console.ReadLine());

            try
            {
                var sensor = await sensorClient.GetSensorDataAsync(new SensorRequest() { SensorId = sensorId}, md);
                Console.WriteLine(sensor.Sensor);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        private static async Task GetAllSensorData(SensorService.SensorServiceClient sensorClient)
        {
            using (var call = sensorClient.GetAllData(new Google.Protobuf.WellKnownTypes.Empty()))
            {
                var responseStream = call.ResponseStream;
                while (await responseStream.MoveNext())
                {
                    Console.WriteLine(responseStream.Current.Sensor);
                }
            }
        }

        public static async Task UploadPhoto(SensorService.SensorServiceClient sensorClient)
        {
            FileStream fs = File.OpenRead(@"C:\Users\pedro\Pictures\sensor2.png");

            using (var call = sensorClient.AddPhoto())
            {
                var stream = call.RequestStream;
                while (true)
                {
                    byte[] buffer = new byte[64 * 1024];
                    int numRead = await fs.ReadAsync(buffer, 0, buffer.Length);
                    if (numRead == 0)
                    {
                        break;
                    }
                    if (numRead < buffer.Length)
                    {
                        Array.Resize(ref buffer, numRead);
                    }

                    await stream.WriteAsync(new AddPhotoRequest()
                    {
                        Data = ByteString.CopyFrom(buffer)
                    });
                }
                await stream.CompleteAsync();

                var res = await call.ResponseAsync;

                Console.WriteLine(res.IsOk);
            }
        }

        private static async Task AddNewSensor(SensorService.SensorServiceClient sensorClient)
        {
            var sensors = new List<Sensor>()
            {
                new Sensor {
                   Id = 101,
                   PositionX = 11.001,
                   PositionY = 9.00,
                   Temperature = 0
                },
                 new Sensor {
                   Id = 102,
                   PositionX = 10.00,
                   PositionY = 8.00,
                   Temperature = 0
                },
            };
            using (var call = sensorClient.AddNewSensor())
            {
                var requestStream = call.RequestStream;
                var responseStream = call.ResponseStream;

                var responseTask = Task.Run(async () =>
                {
                    while (await responseStream.MoveNext())
                    {
                        Console.WriteLine("Saved: " + responseStream.Current.Sensor);
                    }
                });

                foreach (var sensor in sensors)
                {
                    await requestStream.WriteAsync(new SensorAddRequest()
                    {
                        Sensor = sensor
                    });
                }
                await call.RequestStream.CompleteAsync();
                await responseTask;
            }
        }

    }
}
