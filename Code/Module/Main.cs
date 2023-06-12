using Monocle;
using System;
using Microsoft.Xna.Framework;

using Celeste.Mod.Mia.UtilsClass;
using Celeste.Mod.Mia.Settings;
using CelesteBot_Everest_Interop;


using System.Diagnostics;
using static IronPython.Modules._ast;
using System.Windows.Forms;
using IronPython.Compiler.Ast;
using WindowsInput.Native;
using WindowsInput;
using Celeste.Mia.Test;
using System.Runtime.InteropServices;

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

        InputSimulator simulator = new InputSimulator();

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
            walkLR = !walkLR;
            if (walkLR) direction.X = 1;
            string text = walkLR ? "Starting" : "Stopping";
            Engine.Commands.Log(text + " right movement...", Color.GreenYellow);
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

        int j = 0;
        private void ModPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
        {
            Vector2 previousPosition = self.Position;
            float previousStamina = self.Stamina;
            //            Microsoft.Xna.Framework.Input.Buttons.A.

            orig(self);
            if (Engine.Scene is Level level)
            {


                if (direction.X != 1 && MInput.Keyboard.Pressed(Input.MoveX.Positive.Keyboard[0])) { direction.X = 1; }
                if (direction.X != -1 && MInput.Keyboard.Pressed(Input.MoveX.Negative.Keyboard[0])) { direction.X = -1; }

                if (Settings.GetTiles)
                {
                    PlayerManager.PlayerManager.ManagePlayer(stopwatch, self, level, previousPosition, onVoidLevel);
                    TileManager.TileManager.getEntityAroundPlayerAsTiles(level, self);
                    TileManager.TileManager.getTilesAroundPlayer(level, map, self);
                    GameWindow window = Engine.Instance.Window;
                    window.Title = "Celeste.exe/Mia enabled";
                }
                if (walkLR) self.MoveH(direction.X);
                if (walkUD) self.MoveV(direction.Y);

                if (dash && self.Dashes > 0)
                {
                    self.StateMachine.State = 2;
                    self.Dashes--;

                }




                if (j == 0)
                {
                    if(self.InControl) { 
                    dash = true;
                    self.Jump();
                    Utils.print("UWU");
                        j = 1;
                    }
                }


                //                VirtualButton.KeyboardKey keyboardKey = new VirtualButton.KeyboardKey(Input.Grab);
                //Input.Grab

                //Utils.print(Input.MoveY.Value);
                if (grabbing && self.ClimbCheck((int)direction.X) && Input.MoveY.Value == -1f)
                {
                    self.MoveV(Input.MoveY.Value);
                }
                if (grabbing && self.ClimbCheck((int)direction.X) && self.Stamina > 0)
                {
                    self.StateMachine.State = 1;
                    //    Utils.print("=====================================================================");
                    //grabbing = false;
                }

                // Simulating 'U' key press
                
                if (self.OnGround() && self.StateMachine.State == 2) dash = false;
                if (self.StateMachine.State != 2 && dash == true) dash = false;
                //Utils.print(grabbing,self.StateMachine.State);
                if (self.Stamina <= 0 && grabbing) grabbing = false;
                if (self.StateMachine.State != 1 && grabbing) grabbing = false;
                if (self.Dashes == self.MaxDashes) self.Hair.Color = Settings.GetTiles ? Calc.HexToColor("004d00") : Calc.HexToColor("AC3232");

            }
        }
    }
}