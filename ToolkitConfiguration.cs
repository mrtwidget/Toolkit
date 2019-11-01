using Rocket.API;

namespace NEXIS.Toolkit
{
    public class ToolkitConfiguration : IRocketPluginConfiguration
    {
        public bool Debug;
        public string DataDirectory;
        public decimal InitialBalance;
        public bool PayZombieKills;
        public bool PayPlayerKills;
        public bool PayDistanceMultiplier;
        public double PayoutMinDistance;
        public decimal PayoutKillZombie;
        public decimal PayoutKillMegaZombie;
        public decimal PayoutKillPlayer;

        public void LoadDefaults()
        {
            Debug = true;
            DataDirectory = "Plugins/Toolkit/";
            InitialBalance = 500.0m;
            PayZombieKills = true;
            PayPlayerKills = true;
            PayDistanceMultiplier = true;
            PayoutMinDistance = 50;
            PayoutKillZombie = 5.0m;
            PayoutKillMegaZombie = 50.0m;
            PayoutKillPlayer = 100.0m;
        }
    }
}
