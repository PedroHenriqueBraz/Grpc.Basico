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

            //GetSensorData(sensorClient).Wait();

            //  GetAllSensorDatas(sensorClient).Wait();

            // UploadPhoto(sensorClient).Wait();

            AddNewSensor(sensorClient).Wait();
        }

        private static async Task GetSensorData(SensorService.SensorServiceClient sensorClient)
        {
            Metadata md = new Metadata();
            md.Add("my-token","testeToken123");

            try
            {
                var sensor = await sensorClient.GetSensorDataAsync(new SensorRequest() { SensorId = 5 }, md);
                Console.WriteLine(sensor.Sensor);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        private static async Task GetAllSensorDatas(SensorService.SensorServiceClient sensorClient)
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
