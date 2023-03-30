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
                if (level.Session.LevelData.Name.ToString() != "void" && !stopwatch.IsRunning)
                {
                    stopwatch.Restart();
                }

            }

            Utils.print(GetEntityFromXYCenter(GetTileUnderPlayer()).ToString());
            

        }
        public static Entity GetEntityFromXYCenter(Vector2 realPos)
        {
            //System.Collections.Generic.List<Entity> entities = Celeste.Celeste.Scene.Tracker.GetEntities<Entity>();
            EntityList entities = Celeste.Scene.Entities;/*Tracker.GetEntities<Entity>();*/
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].Collidable && entities[i].CollidePoint(realPos) && entities[i].Tag != 16) // 16=Tag of player
                {
                    // Collided with the point and is collidable
                    return entities[i];
                }
            }
            return null;
        }

        public static Vector2 GetTilePosFromXYCenter(Vector2 realPos)
        {
            int offsetX = -428;
            int offsetY = -244;
            int width = 8, height = 8;
            return new Vector2((int)((realPos.X - offsetX) / width), (int)((realPos.Y - offsetY) / height));
        }

        public static Vector2 GetTileUnderPlayer()
        {
            Vector2 playerPos = GetPlayerPos();
            playerPos = new Vector2(playerPos.X, playerPos.Y + 4);

            return GetTilePosFromXYCenter(playerPos); // This is to account for offset
        }

        public static Vector2 GetPlayerPos()
        {
            Player player = Celeste.Scene.Tracker.GetEntity<Player>();
            if (player != null)
                return player.Position; // This is the bottom center
            return new Vector2(0, 0);
        }

    }
}
