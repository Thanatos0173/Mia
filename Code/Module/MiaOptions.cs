using Celeste.Mod.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

using Celeste.Mod.Mia.Settings;
using Celeste.Mod.Mia;

namespace Celeste.Mod.Mia.MiaOptions
{
    class OuiExampleSubmenu : OuiGenericMenu, OuiModOptions.ISubmenu
    {
        public override string MenuName => "Mia settings";

        private string IntString(int value)
        {
            if (value % 60 == 0) return value / 60 + " minutes";
            else return value / 60 + " minutes  and " + value % 60 + " seconds";
        }
        protected override void addOptionsToMenu(TextMenu menu)
        {
            menu.Add(new TextMenu.Slider("Idle Time", IntString, 5, 300, Mia.Main.Settings.IdleTime)
                 .Change(newValue => Mia.Main.Settings.IdleTime = newValue)) ;

            menu.Add(new TextMenu.OnOff("Kill player", Mia.Main.Settings.KillPlayer)
                .Change(newValue => Mia.Main.Settings.KillPlayer = newValue));
        }
    }
}
