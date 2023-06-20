using Monocle;
using System;
using Microsoft.Xna.Framework;

using Celeste.Mod.Mia.UtilsClass;
using Celeste.Mod.Mia.Settings;


using System.Diagnostics;
using WindowsInput;
using System.Runtime.InteropServices;

using Celeste.Mod.Mia.InputAdder;

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
        private static bool grabbing { get; set; } = false;
        private static bool dash { get; set; } = false;

        private static float baseTimer;
        private static char[,] map;
        private static bool loadMap = false;

    

        [Command("left", "Move left")]
        public static void LeftCommand()
        {
            walkLR = !walkLR;
            if (walkLR) direction.X = -1;
            string text = walkLR ? "Starting" : "Stopping";
            Engine.Commands.Log(text + " left movement...", Color.GreenYellow);
        }

        [Command("right", "Move right")]
        public static void RightCommand()
        {
            bool fileHaveBeenReadSuccesfully = InputAdder.Inputting.moveToRight();
            if(fileHaveBeenReadSuccesfully) Engine.Commands.Log("Moving Right", Color.GreenYellow);
            else Engine.Commands.Log("File haven't been succesfully read.", Color.Red);
        }
        [Command("state", "Move right")]
        public static void StateMachineCommand()
        {
            index++;
            Engine.Commands.Log("Change to index " + index.ToString(), Color.GreenYellow);
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
        [Command("grab", "Start/Stop grabbing. Optionnal argument : if true then the player start grabbing, if false the player end grabbing. If nothing, the player shift between start and stop")]
        public static void GrabCommand(string force)
        {
            try
            {
                grabbing = bool.Parse(force);
            }
            catch (ArgumentNullException)
            {
                grabbing = !grabbing;
            }
            Engine.Commands.Log((grabbing ? "Started" : "Stopping") + " Grabbing...", Color.GreenYellow);
        }

        [Command("mia", "Enable / Disable Mia")]
        public static void MiaCommand()
        {
            Settings.GetTiles = !Settings.GetTiles;
            Engine.Commands.Log(" Mia is now " + (Settings.GetTiles ? "enaable." : "diasble."), Color.GreenYellow);

        }




        public override void Load()
        {
            Utils.print("Mod Loaded KEUYIUO YEUI");
            On.Celeste.Player.Update += ModPlayerUpdate;
            Everest.Events.Player.OnSpawn += onSpawn;
            Everest.Events.Level.OnLoadLevel += loadLevel;

            stopwatch = new Stopwatch();
        }


        private void loadLevel(Level level, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            if (isFromLoader && !Settings.GetTiles)
            {
                map = level.SolidsData.ToArray();
                Utils.print("Tiles won't be retreived. Change the option in the settings to make able to retrieve level, Mia will not work otherwise.");
            }
        }

        public override void Unload()
        {
            Utils.print("Mod unloaded");
            On.Celeste.Player.Update -= ModPlayerUpdate;
            Everest.Events.Player.OnSpawn -= onSpawn;
            Everest.Events.Level.OnLoadLevel -= loadLevel;

        }


        private void onSpawn(Player player)
        {
            if (Main.Settings.KillPlayer) stopwatch.Restart();
            if (Main.Settings.Debug && Main.Settings.KillPlayer) Utils.print("Starting stopwatch");
            if (baseTimer == null)
            {
                Utils.print("BOUH LE NULL");
                baseTimer = player.AutoJumpTimer;
            }
        }
        bool onVoidLevel = false;


 
        private void ModPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
        {
            try
            {
                orig(self);
                if (Engine.Scene is Level level)
                {

                    if (direction.X != 1 && MInput.Keyboard.Pressed(Input.MoveX.Positive.Keyboard[0])) { direction.X = 1; }
                    if (direction.X != -1 && MInput.Keyboard.Pressed(Input.MoveX.Negative.Keyboard[0])) { direction.X = -1; }

                    if (Settings.GetTiles)
                    {
                        PlayerManager.PlayerManager.ManagePlayer(stopwatch, self, level, self.Position, onVoidLevel);
                        TileManager.TileManager.getEntityAroundPlayerAsTiles(level, self);
                        TileManager.TileManager.getTilesAroundPlayer(level, map, self);
                        GameWindow window = Engine.Instance.Window;
                        window.Title = "Celeste.exe/Mia enabled";
                    }

                }
            }
            catch (ArgumentOutOfRangeException e) {
                Console.WriteLine("Exception happened");
            }
        }
    }
}