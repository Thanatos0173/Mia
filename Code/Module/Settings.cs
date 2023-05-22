using Celeste.Mod.UI;

using Celeste.Mod.Mia.MiaOptions;

namespace Celeste.Mod.Mia.Settings
{
    public class SettingsClass : EverestModuleSettings
    {
        [SettingInGame(true)]
        public int IdleTime { get; set; } = 15;
        public bool KillPlayer { get; set; } = true;

        public bool Debug { get; set; } = false;

        public void CreateIdleTimeEntry(TextMenu menu, bool inGame)
        {
            menu.Add(new TextMenu.Button("Idle Time Manager")
                .Pressed(() => OuiGenericMenu.Goto<OuiExampleSubmenu>(overworld => overworld.Goto<OuiModOptions>(), new object[0])));
        }
        public void CreateDebugEntry(TextMenu menu, bool inGame)
        {
            menu.Add(new TextMenu.OnOff("Debug", Mia.Main.Settings.Debug)
                .Change(newValue => Mia.Main.Settings.Debug = newValue));
        }

        
    }
}

