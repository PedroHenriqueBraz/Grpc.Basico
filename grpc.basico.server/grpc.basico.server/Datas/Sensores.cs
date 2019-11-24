using grpc.basico.server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sensoriamento.server.Sensores
{
    public static class Sensores
    {
        public static List<Sensor> sensores = new List<Sensor>()
        {
            new Sensor
            {
                Id = 1,
                PositionX = 1.988,
                PositionY = 2.455,
                Temperature = 78.12
            },
            new Sensor
            {
                Id = 2,
                PositionX = 4.001,
                PositionY = 2.455,
                Temperature = 75.20
            },
            new Sensor
            {
                Id = 3,
                PositionX = 1.988,
                PositionY = 3.000,
                Temperature = 69.00
            },
            new Sensor
            {
                Id = 4,
                PositionX = 5.00,
                PositionY = 5.00,
                Temperature = 70.00
            },
            new Sensor
            {
                Id = 5,
                PositionX = 2.345,
                PositionY = 2.004,
                Temperature = 66.66
            },
            new Sensor
            {
                Id = 6,
                PositionX = 6.098,
                PositionY = 3.123,
                Temperature = 78.89
            },
            new Sensor
            {
                Id = 7,
                PositionX = 1.010,
                PositionY = 2.577,
                Temperature = 71.99
            },
            new Sensor
            {
                Id = 8,
                PositionX = 0.778,
                PositionY = 5.900,
                Temperature = 78.12
            }

        };
    }
}
