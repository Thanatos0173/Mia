using On.Celeste.Mod;
using IL.Celeste.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celeste.Mod.UI;

using Celeste.Mod.Mia.MiaOptions;
using System.Xml.Serialization;
using YamlDotNet.Serialization;

namespace Celeste.Mod.Mia.Settings
{
    public class ExampleModuleSettings : EverestModuleSettings
    {
        [SettingInGame(true)]
        public int IdleTime { get; set; } = 15;

        public void CreateIdleTimeEntry(TextMenu menu, bool inGame)
        {
          
                menu.Add(new TextMenu.Button("Idle Time")
                    .Pressed(() => OuiGenericMenu.Goto<OuiExampleSubmenu>(overworld => overworld.Goto<OuiModOptions>(), new object[0])));
            
        }
    }

}

