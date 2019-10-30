using Rocket.API;

namespace NEXIS.Toolkit
{
    public class ToolkitConfiguration : IRocketPluginConfiguration
    {
        public bool Debug;
        public decimal InitialBalance;
        public bool PayZombieKills;
        public decimal PayoutZombie;
        public decimal PayoutMegaZombie;

        public void LoadDefaults()
        {
            Debug = true;
            InitialBalance = 500.0m;
            PayZombieKills = true;
            PayoutZombie = 5.0m;
            PayoutMegaZombie = 50.0m;
        }
    }
}
