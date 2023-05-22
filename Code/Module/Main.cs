using Monocle;
using System;
using Microsoft.Xna.Framework;

using Celeste.Mod.Mia.UtilsClass;
using Celeste.Mod.Mia.Settings;

using System.Diagnostics;

namespace Celeste.Mod.Mia
{
    public class Main : EverestModule
    {
        public static Main Instance;

        private Stopwatch stopwatch;
        public override Type SettingsType => typeof(SettingsClass);
        public static SettingsClass Settings => (SettingsClass)Instance._Settings;
        public Main()
        {
            Instance = this;
        }
        public override void Load()
        {
            Utils.print("TIPE Mod Loaded");
            On.Celeste.Player.Update += ModPlayerUpdate;
            Everest.Events.Player.OnSpawn += onSpawn;
            stopwatch = new Stopwatch();
        }


        public override void Unload()
        {
            Utils.print("TIPE Mod unloaded");
            On.Celeste.Player.Update -= ModPlayerUpdate;
            Everest.Events.Player.OnSpawn -= onSpawn;
        }

        
        private void onSpawn(Player player)
        {
            if (Main.Settings.KillPlayer) stopwatch.Restart();
            if (Main.Settings.Debug && Main.Settings.KillPlayer) Utils.print("Starting stopwatch");
        }
        bool onVoidLevel = false;


        private void ModPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
        {
            float previousStamina = self.Stamina;
            Vector2 previousPosition = self.Position;
            int dashesLefts = self.Dashes;

            orig(self);


            if (Engine.Scene is Level level)
            {
                PlayerManager.PlayerManager.ManagePlayer(stopwatch, self, level, previousPosition, onVoidLevel);
                TileManager.TileManager.getEntityAroundPlayerAsTiles(level, self);
                TileManager.TileManager.getTilesAroundPlayer(level,level.SolidsData.ToArray() ,self);
                
            }
        }
    }
}
