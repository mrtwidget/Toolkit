using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace NEXIS.Toolkit
{
    public class Messages
    {
        public int ID { get; set; }
        public string Message { get; set; }
        public string Color { get; set; }

        public void Load()
        {
            // create file if not exist
            if (!File.Exists(Toolkit.Instance.Configuration.Instance.DataDirectory + "Messages.json"))
            {
                File.Create(Toolkit.Instance.Configuration.Instance.DataDirectory + "Messages.json").Dispose();
                File.WriteAllText(Toolkit.Instance.Configuration.Instance.DataDirectory + "Messages.json", "[]");
            }

            // load file to memory
            string file = File.ReadAllText(Toolkit.Instance.Configuration.Instance.DataDirectory + "Messages.json");
            JArray json = JArray.Parse(file);
            Toolkit.Instance.MessageList = json.ToObject<List<Messages>>();

            if (Toolkit.Instance.Configuration.Instance.Debug)
                Console.WriteLine("Messages Loaded: " + Toolkit.Instance.MessageList.Count);
        }

        public void Update()
        {
            // save list to file
            string json = JsonConvert.SerializeObject(Toolkit.Instance.MessageList, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(Toolkit.Instance.Configuration.Instance.DataDirectory + "Messages.json", json);
        }
    }
}
