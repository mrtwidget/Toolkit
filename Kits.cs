using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Steamworks;

namespace NEXIS.Toolkit
{
    public class Kits
    {
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public List<KitItem> Items { get; set; }

        public void Load()
        {
            // create file if not exist
            if (!File.Exists(Toolkit.Instance.Configuration.Instance.DataDirectory + "Kits.json"))
            {
                // create default kits if none exist
                Toolkit.Instance.KitList = new List<Kits>() {
                    new Kits() { Name = "test1", Items = new List<KitItem>() { new KitItem(245, 1, null), new KitItem(81, 2, null), new KitItem(16, 1, null) }},
                    new Kits() { Name = "test2", Cost = 30, Items = new List<KitItem>() { new KitItem(112, 1, null), new KitItem(113, 3, null), new KitItem(254, 3, null) }},
                    new Kits() { Name = "test3", Cost = 20, Items = new List<KitItem>() { new KitItem(109, 1, null), new KitItem(111, 3, null), new KitItem(236, 1, null) }}
                };
                string kits = JsonConvert.SerializeObject(Toolkit.Instance.KitList, Newtonsoft.Json.Formatting.Indented);

                File.Create(Toolkit.Instance.Configuration.Instance.DataDirectory + "Kits.json").Dispose();
                File.WriteAllText(Toolkit.Instance.Configuration.Instance.DataDirectory + "Kits.json", kits);
            }

            // load file to list
            string file = File.ReadAllText(Toolkit.Instance.Configuration.Instance.DataDirectory + "Kits.json");
            JArray json = JArray.Parse(file);
            Toolkit.Instance.KitList = json.ToObject<List<Kits>>();

            if (Toolkit.Instance.Configuration.Instance.Debug)
                Console.WriteLine("Kits Loaded: " + Toolkit.Instance.KitList.Count);
        }

        public void Update()
        {
            // save list to file
            string json = JsonConvert.SerializeObject(Toolkit.Instance.KitList, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(Toolkit.Instance.Configuration.Instance.DataDirectory + "Kits.json", json);
        }
    }

    public class KitItem
    {
        public KitItem() { }

        public KitItem(ushort itemId, byte amount, string metadata)
        {
            ItemId = itemId;
            Amount = amount;
            Metadata = metadata;
        }

        public ushort ItemId;
        public byte Amount;
        public string Metadata;
    }
}