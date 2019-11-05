using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Steamworks;

namespace NEXIS.Toolkit
{
    public class Vehicles
    {
        public ushort ID { get; set; }
        public string Name { get; set; }
        public decimal BuyPrice { get; set; }

        public void Load()
        {
            // create file if not exist
            if (!File.Exists(Toolkit.Instance.Configuration.Instance.DataDirectory + "Vehicles.json"))
            {
                File.Create(Toolkit.Instance.Configuration.Instance.DataDirectory + "Vehicles.json").Dispose();
                File.WriteAllText(Toolkit.Instance.Configuration.Instance.DataDirectory + "Vehicles.json", "[]");
            }

            // load shop to list
            string file = File.ReadAllText(Toolkit.Instance.Configuration.Instance.DataDirectory + "Vehicles.json");
            JArray json = JArray.Parse(file);
            Toolkit.Instance.VehicleList = json.ToObject<List<Vehicles>>();

            if (Toolkit.Instance.Configuration.Instance.Debug)
                Console.WriteLine("Shop Vehicles Loaded: " + Toolkit.Instance.VehicleList.Count);
        }

        public void Update()
        {
            // save shop to file
            string json = JsonConvert.SerializeObject(Toolkit.Instance.VehicleList, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(Toolkit.Instance.Configuration.Instance.DataDirectory + "Vehicles.json", json);
        }
    }
}