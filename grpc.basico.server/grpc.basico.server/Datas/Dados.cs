using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sensoriamento.server.Datas
{
    public static class Dados
    {
        public static List<Sensor> sensors => gerarsensores();

        private static List<Sensor> gerarsensores()
        {
            List<Sensor> sensores = new List<Sensor>();
            var rand = new Random();

            for(int i =0; i < 100; i++)
            {
                List<double> temps = new List<double>();

                for(int j = 0; j < 50; j++)
                {
                    var temp = rand.Next(10, 30) + rand.NextDouble();
                    temps.Add(temp);
                }
                sensores.Add(new Sensor(i, rand.NextDouble(), rand.NextDouble(), temps));
            }
            return sensores;
        }
    }

    public class Sensor
    {
        public Sensor(int id, double x, double y, List<double> temps)
        {
            this.sensor_id = id;
            this.position_x = x;
            this.position_y = y;
            this.temperatures = temps;
        }
        public int sensor_id { get; set; }
        public double position_x { get; set; }
        public double position_y { get; set; }
        public List<double> temperatures { get; set; }
    }
}
