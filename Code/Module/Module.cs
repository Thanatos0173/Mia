using Monocle;
using System;
using Microsoft.Xna.Framework;

using Celeste.Mod.Mia.UtilsClass;
using Celeste.Mod.Mia.Settings;
using CelesteBot.Manager;


using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

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
            if(Module.Settings.KillPlayer)stopwatch.Restart();
            if (Module.Settings.Debug && Module.Settings.KillPlayer) Utils.print("Starting stopwatch");
        }

        private void ModPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
        {
            float previousStamina = self.Stamina;
            Vector2 previousPosition = self.Position;
            int dashesLefts = self.Dashes;
            bool onVoidLevel = false;
            orig(self);

            if (self.Position != previousPosition)
            {
                if(Module.Settings.KillPlayer)stopwatch.Restart();
            }
            if (stopwatch.Elapsed >= TimeSpan.FromSeconds(Settings.IdleTime))
            {
                if(Module.Settings.KillPlayer)self.Die(self.Position);
                if (Module.Settings.Debug && Module.Settings.KillPlayer) Utils.print("Player was idle too long : killed");

            }
            if (Engine.Scene is Level level)
            {
                if (self.Position != previousPosition)
                {
                    String text = "";
                    EntityList entities = level.Entities;
                    for (int i = 0; i < entities.Count; i++)
                    {
                        if (entities[i].Visible && level.Entities[i].Tag == 0)
                        {
                            text += entities[i].ToString();
                            string position = level.Entities[i].Position.ToString();
                            string height = level.Entities[i].Height.ToString();
                            string width = level.Entities[i].Width.ToString();
                            text += "(" + position + "{H: " + height + " W: " + width + "},tag: " + level.Entities[i].Tag.ToString() + ")\n";

                        }
                    }
                    System.IO.File.WriteAllText(@"C:\Users\elioo\OneDrive\Bureau\TIPE\CelesteSaving\tiles.txt", text);
                    //Utils.print("Entities :",text);

                }
                if (level.InCutscene)
                {
                    level.SkipCutscene();
                    if (Module.Settings.Debug) Utils.print("Skipping cutscene :", Engine.Scene?.GetType().FullName.ToString());

                }
                if (level.Session.LevelData.Name.ToString() == "void" && ! onVoidLevel)
                {
                    if(Module.Settings.KillPlayer) stopwatch.Stop();
                    onVoidLevel = true;
                    if (Module.Settings.Debug && Module.Settings.KillPlayer) Utils.print("Stopping stopwatch : currently in \"void\" level");
                }
                if (level.Session.LevelData.Name.ToString() != "void" && !stopwatch.IsRunning && stopwatch != null && onVoidLevel)
                {
                    onVoidLevel = false;
                    if(Module.Settings.KillPlayer)stopwatch.Restart();
                    if (Module.Settings.Debug && Module.Settings.KillPlayer) Utils.print("Restarting stopwatch : exiting \"void\" level");

                }

            }
        }
    }
}
