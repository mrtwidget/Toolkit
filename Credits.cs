using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Steamworks;

namespace NEXIS.Toolkit
{
    public class Credits
    {
        public void Load()
        {
            // create file if not exist
            if (!File.Exists(Toolkit.Instance.Configuration.Instance.DataDirectory + "Credits.json"))
            {
                File.Create(Toolkit.Instance.Configuration.Instance.DataDirectory + "Credits.json").Dispose();
                File.WriteAllText(Toolkit.Instance.Configuration.Instance.DataDirectory + "Credits.json", "{}");
            }

            // load credits to list
            string file = File.ReadAllText(Toolkit.Instance.Configuration.Instance.DataDirectory + "Credits.json");
            JObject json = JObject.Parse(file);
            Toolkit.Instance.Balances = json.ToObject<Dictionary<string, decimal>>();

            if (Toolkit.Instance.Configuration.Instance.Debug)
                Console.WriteLine("Credits Loaded: " + Toolkit.Instance.Balances.Count);
        }

        public void Update()
        {
            // save credits to file
            string json = JsonConvert.SerializeObject(Toolkit.Instance.Balances, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(Toolkit.Instance.Configuration.Instance.DataDirectory + "Credits.json", json);
        }
    }
}
