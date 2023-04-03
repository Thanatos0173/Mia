using Celeste.Mod.UI;
using FMOD.Studio;
using Monocle;
using Celeste;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMOD;
using Microsoft.Xna.Framework;
using On.Celeste;
using Celeste.Mod;
using MonoMod.Utils;

using Celeste.Mod.Mia.UtilsClass;
using Celeste.Mod.Mia.Settings;
using CelesteBot.Manager;


using System.Diagnostics;
using IL.Celeste;
using Microsoft.Xna.Framework.Graphics;


namespace Celeste.Mod.Module
{
    public class Module : EverestModule
    {

        // Only one alive module instance can exist at any given time.
        public static Module Instance;

        private Stopwatch stopwatch;
        public override Type SettingsType => typeof(ExampleModuleSettings);
        public static ExampleModuleSettings Settings => (ExampleModuleSettings)Instance._Settings;
        public Module()
        {
            Instance = this;
        }

        // Set up any hooks, event handlers and your mod in general here.
        // Load runs before Celeste itself has initialized properly.
        public override void Load()
        {
            Utils.print("TIPE Mod Loaded");
            On.Celeste.Player.Update += ModPlayerUpdate;
            Everest.Events.Player.OnSpawn += onSpawn;
            stopwatch = new Stopwatch();
        }

        // Optional, do anything requiring either the Celeste or mod content here.
        public override void LoadContent(bool firstLoad)
        {
        }

        // Unload the entirety of your mod's content. Free up any native resources.
        public override void Unload()
        {
            Utils.print("TIPE Mod unloaded");
            On.Celeste.Player.Update -= ModPlayerUpdate;
            Everest.Events.Player.OnSpawn -= onSpawn;
        }

        private void onSpawn(Player player)
        {
            stopwatch.Restart();
        }

        private void ModPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
        {
            float previousStamina = self.Stamina;
            Vector2 previousPosition = self.Position;
            int dashesLefts = self.Dashes;
            orig(self);



            if (self.Position != previousPosition)
            {
                stopwatch.Restart();
            }
            if (stopwatch.Elapsed >= TimeSpan.FromSeconds(Settings.IdleTime))
            {
                self.Die(self.Position);
            }
            if (Engine.Scene is Level level)
            {
                if (level.InCutscene)
                {
                    level.SkipCutscene();
                }
                if (level.Session.LevelData.Name.ToString() == "void")
                {
                    stopwatch.Stop();
                }
                if (level.Session.LevelData.Name.ToString() != "void" && !stopwatch.IsRunning && stopwatch != null)
                {
                    stopwatch.Restart();
                }

            }
            Manager.PutEntitiesToFile();
        }
    }
}
