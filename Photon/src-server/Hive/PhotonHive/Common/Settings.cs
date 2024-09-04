using Photon.SocketServer;
using Photon.SocketServer.Annotations;

namespace Photon.Hive.Common
{
    [SettingsMarker("Photon:Hive")]
    public class Settings
    {
        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Settings()
        {
        }

        public static Settings Default { get; } = ApplicationBase.GetConfigSectionAndValidate<Settings>("Photon:Hive");


        public string PluginConfig { get; set; } = "plugin.config";
    }
}
