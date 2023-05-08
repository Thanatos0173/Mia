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
using Celeste.Mod.Mia.FileHandling;
using System.IO;
using Microsoft.Scripting.Hosting;
using IronPython.Hosting;
using System.Reflection;

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
         //   if(isFromLoader) Mia.FileHandling.FileHandling.saveTiles(Settings.Path, Eee(level, level.SolidsData.ToArray(), level.Entities.FirstOrDefault(entity => entity is Player) as Player));
        }

        private void onSpawn(Player player)
        {
            if (Main.Settings.KillPlayer) stopwatch.Restart();
            if (Main.Settings.Debug && Main.Settings.KillPlayer) Utils.print("Starting stopwatch");
            if (Engine.Scene is Level level)
            {
                Utils.putToFile(level);
                Utils.entityList(level, player);
            }
        }
        bool onVoidLevel = false;


        private int j = 0;
        

        private void ModPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)

        {
            float previousStamina = self.Stamina;
            Vector2 previousPosition = self.Position;
            int dashesLefts = self.Dashes;

            orig(self);

            j = 0;
            if (Engine.Scene is Level level)
            {
                Mia.PlayerManager.PlayerManager.ManagePlayer(stopwatch, self, level, previousPosition, onVoidLevel);
                Utils.entityList(level, self);
                if(previousPosition != self.Position && !self.IsIntroState)
                {

                                        Console.Write('\r');
                                        Console.Write(Utils.getTilesAroundPlayerAsString(level,level.SolidsData.ToArray(),self));
                    //Utils.getTilesAroundPlayerAsString(level, level.SolidsData.ToArray(), self);
                }
            }
        }
    }
}
