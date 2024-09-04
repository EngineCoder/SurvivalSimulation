using Photon.Hive.Plugin;

namespace TestPlugins
{
    class SettingSlotsPlugin : TestPluginBase
    {
        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            info.Request.GameProperties[(byte)255] = 2;
            info.Request.GameProperties[(byte)247] = new string[] { "Player2" };
            base.OnCreateGame(info);
        }
    }
}
