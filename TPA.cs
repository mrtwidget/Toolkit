using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Steamworks;

namespace NEXIS.Toolkit
{
    public class TPA
    {
        public CSteamID PlayerRequesting { get; set; }
        public CSteamID PlayerAccepting { get; set; }
        public DateTime TimeRequested { get; set; }
    }
}