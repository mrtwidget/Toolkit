using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Steamworks;

namespace NEXIS.Toolkit
{
    public class Items
    {
        public ushort ID { get; set; }
        public string Name { get; set; }
        public decimal BuyPrice { get; set; }
        public decimal SellPrice { get; set; }

        public void Load()
        {
            // create file if not exist
            if (!File.Exists(Toolkit.Instance.Configuration.Instance.DataDirectory + "Items.json"))
            {
                File.Create(Toolkit.Instance.Configuration.Instance.DataDirectory + "Items.json").Dispose();
                File.WriteAllText(Toolkit.Instance.Configuration.Instance.DataDirectory + "Items.json", "[]");
            }

            // load shop to list
            string file = File.ReadAllText(Toolkit.Instance.Configuration.Instance.DataDirectory + "Items.json");
            JArray json = JArray.Parse(file);
            Toolkit.Instance.ItemList = json.ToObject<List<Items>>();

            if (Toolkit.Instance.Configuration.Instance.Debug)
                Console.WriteLine("Shop Items Loaded: " + Toolkit.Instance.ItemList.Count);
        }

        public void Update()
        {
            // save shop to file
            string json = JsonConvert.SerializeObject(Toolkit.Instance.ItemList, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(Toolkit.Instance.Configuration.Instance.DataDirectory + "Items.json", json);
        }
    }
}