

using Monocle;
using System;
using Microsoft.Xna.Framework;

using Celeste.Mod.Mia.UtilsClass;
using Celeste.Mod.Mia.Settings;
using Celeste.Mod.Mia.PlayerManager;


using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using IL.MonoMod;
using Celeste.Mod.mia.FileHandling;

namespace Celeste.Mod.Module
{
    public class Module : EverestModule
    {
        public static Module Instance;

        private Stopwatch stopwatch;
        public override Type SettingsType => typeof(ExampleModuleSettings);
        public static ExampleModuleSettings Settings => (ExampleModuleSettings)Instance._Settings;
        public Module()
        {
            Instance = this;
        }
        public override void Load()
        {
            Utils.print("TIPE Mod Loaded");
            On.Celeste.Player.Update += ModPlayerUpdate;
            Everest.Events.Player.OnSpawn += onSpawn;
            Everest.Events.Level.OnLoadLevel += LoadLevel;
            stopwatch = new Stopwatch();
        }


        public override void Unload()
        {
            Utils.print("TIPE Mod unloaded");
            On.Celeste.Player.Update -= ModPlayerUpdate;
            Everest.Events.Player.OnSpawn -= onSpawn;
            Everest.Events.Level.OnLoadLevel -= LoadLevel;
        }

        private void LoadLevel(Level level, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            if (true)
            {
                Char[,] map = level.SolidsData.ToArray();
                Utils.print(level.TileBounds.ToString());
                Utils.WriteCurrentLevel(map,level);
            }
        }

        private void onSpawn(Player player)
        {
            if (Module.Settings.KillPlayer) stopwatch.Restart();
            if (Module.Settings.Debug && Module.Settings.KillPlayer) Utils.print("Starting stopwatch");
            if (Engine.Scene is Level level) Utils.putToFile(level);
        }
        bool onVoidLevel = false;


        private int j = 0;

        private void ModPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)

        {
            float previousStamina = self.Stamina;
            Vector2 previousPosition = self.Position;
            int dashesLefts = self.Dashes;

            orig(self);


            if (Engine.Scene is Level level)
            {

                PlayerManager.ManagePlayer(stopwatch, self, level, previousPosition, onVoidLevel);
                FileHandling.LoadFile(Settings.Path);

            }
        }
    }
}
