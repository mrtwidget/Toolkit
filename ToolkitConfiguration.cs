using Rocket.API;

namespace NEXIS.Toolkit
{
    public class ToolkitConfiguration : IRocketPluginConfiguration
    {
        public bool Debug;

        public void LoadDefaults()
        {
            Debug = true;
        }
    }
}
