using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NEXIS.Toolkit
{
    public class Warps
    {
        public string Name { get; set; }
        public List<string> Position { get; set; }
        public float Rotation { get; set; }
        public decimal Price { get; set; }

        public void Load()
        {
            // create file if not exist
            if (!File.Exists(Toolkit.Instance.Configuration.Instance.DataDirectory + "Warps.json"))
            {
                File.Create(Toolkit.Instance.Configuration.Instance.DataDirectory + "Warps.json").Dispose();
                File.WriteAllText(Toolkit.Instance.Configuration.Instance.DataDirectory + "Warps.json", "[]");
            }

            // load warps file to list
            string file = File.ReadAllText(Toolkit.Instance.Configuration.Instance.DataDirectory + "Warps.json");
            JArray json = JArray.Parse(file);
            Toolkit.Instance.WarpList = json.ToObject<List<Warps>>();

            if (Toolkit.Instance.Configuration.Instance.Debug)
                Console.WriteLine("Warps Loaded: " + Toolkit.Instance.WarpList.Count);
        }

        public void Update()
        {
            // save warp list to file
            string json = JsonConvert.SerializeObject(Toolkit.Instance.WarpList, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(Toolkit.Instance.Configuration.Instance.DataDirectory + "Warps.json", json);
        }
    }
}
