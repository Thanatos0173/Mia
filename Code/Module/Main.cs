using Monocle;
using System;
using System.Threading;
using Microsoft.Xna.Framework;

using Celeste.Mod.Mia.UtilsClass;
using Celeste.Mod.Mia.Settings;



using System.Diagnostics;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using YamlDotNet.Core.Tokens;
using IL.Celeste;
using Microsoft.Xna.Framework.Input;
using IL.FMOD.Studio;
using MonoMod.Utils;
using On.Celeste;
using static System.Windows.Forms.AxHost;
using static IronPython.Modules._ast;
using WindowsInput.Native;
using WindowsInput;
using System.Windows.Forms;


namespace Celeste.Mod.Mia
{
    
    public class Main : EverestModule
    {
        public static Main Instance;



        public override Type SettingsType => typeof(SettingsClass);
        public static SettingsClass Settings => (SettingsClass)Instance._Settings;
        public Main()
        {
            Instance = this;
        }

        private static int index = 0;
        
        private Stopwatch stopwatch;
        private static bool walkLR = false;
        private static bool walkUD = false;
        private static Vector2 direction = new Vector2(1, 0);
        private static bool grabbing = false;
        private static bool dash = false;

        private static float baseTimer;
        private static float timer = 0;
        private static bool jump = false;

        private static bool fly = false;
        public bool grab { get; set; } = false;


        
        [Command("left", "Move left")]
        public static void LeftCommand()
        {
            walkLR = !walkLR;
            if (walkLR) direction.X = -1;
            else direction.X = 0;
            string text = walkLR ? "Starting" : "Stopping"; 
            Engine.Commands.Log(text + " left movement...",   Color.GreenYellow);
        }

        [Command("right", "Move right")]
        public static void RightCommand()
        {
            Player player = Engine.Scene.Tracker.GetEntity<Player>();
            walkLR = !walkLR;
            if (walkLR) direction.X = 1;
            else direction.X = 0;
            string text = walkLR ? "Starting" : "Stopping";
            Engine.Commands.Log(text + " right movement...", Color.GreenYellow);
        }
        [Command("state", "Move right")]
        public static void StateMachineCommand()
        {
            index++;
            Engine.Commands.Log("Change to index "+index.ToString(), Color.GreenYellow);
        }
        [Command("up", "Move up")]
        public static void UpCommand()
        {
            walkUD = !walkUD;
            if (walkUD) direction.Y = -1;
            else direction.Y = 0;
            string text = walkLR ? "Starting" : "Stopping";
            Engine.Commands.Log(text + " up movement...", Color.GreenYellow);
        }

        [Command("down", "Move down")]
        public static void DownCommand()
        {
            walkUD = !walkUD;
            if (walkUD) direction.Y = 1;
            else direction.Y = 0;
            string text = walkLR ? "Starting" : "Stopping";
            Engine.Commands.Log(text + " down movement...", Color.GreenYellow);

        }
        [Command("dash", "Dash")]
        public static void DashCommand()
        {
            Engine.Commands.Log("Dashing...", Color.GreenYellow);
            dash = true;
            
        }

        public override void Load()
        {
            Utils.print("TIPE Mod Loaded");
            On.Celeste.Player.Update += ModPlayerUpdate;
            Everest.Events.Player.OnSpawn += onSpawn;
            Everest.Events.Level.OnLoadLevel += loadLevel;
            stopwatch = new Stopwatch();
        }


        

   
        private void loadLevel(Level level, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            if (isFromLoader && !Settings.GetTiles) Utils.print("Tiles won't be retrived. Change the option in the settings to make able to retrieve level, Mia will not work otherwise.");
            
        }

        public override void Unload()
        {
            Utils.print("TIPE Mod unloaded");
            On.Celeste.Player.Update -= ModPlayerUpdate;
            Everest.Events.Player.OnSpawn -= onSpawn;
            Everest.Events.Level.OnLoadLevel -= loadLevel;
            
        }

        
        private void onSpawn(Player player)
        {
            if (Main.Settings.KillPlayer) stopwatch.Restart();
            if (Main.Settings.Debug && Main.Settings.KillPlayer) Utils.print("Starting stopwatch");
            if(baseTimer == null)
            {
                Utils.print("BOUH LE NULL");
                baseTimer = player.AutoJumpTimer;
            }
        }
        bool onVoidLevel = false;


        
        private void ModPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
        {
            float previousStamina = self.Stamina;
            Vector2 previousPosition = self.Position;
            
            orig(self);
            Utils.print(self.StateMachine.State,self.Dashes > 0);


            if (Engine.Scene is Level level) 
            {
                if (Settings.GetTiles)
                {
                    PlayerManager.PlayerManager.ManagePlayer(stopwatch, self, level, previousPosition, onVoidLevel);
                    TileManager.TileManager.getEntityAroundPlayerAsTiles(level, self);
                    TileManager.TileManager.getTilesAroundPlayer(level, level.SolidsData.ToArray(), self);
                }
                if (walkLR) self.MoveH(direction.X);
                if (walkUD) self.MoveV(direction.Y);
               
                if (dash && self.Dashes > 0)
                {
                    self.StateMachine.State = 2;
                    self.Dashes--;
                    Utils.print("MODIFIED");

                }
                if (self.OnGround() && self.StateMachine.State == 2) dash = false;
                if(self.StateMachine.State != 2 && dash == true) dash = false;
            }
        }
    }
}