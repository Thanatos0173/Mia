using Monocle;
using System;
using Microsoft.Xna.Framework;

using Celeste.Mod.Mia.UtilsClass;
using Celeste.Mod.Mia.Settings;


using System.Diagnostics;
using WindowsInput;
using System.Runtime.InteropServices;

using Celeste.Mod.Mia.InputAdder;
using System.Collections.Generic;
using System.Linq;
using System.IO.Pipes;
using System.IO;
using GridWindow;

using System.Net.Sockets;
using System.Text;
using IronPython.Runtime.Operations;
using Celeste.Mod.Mia.NeuralNetwork;

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

        private Stopwatch stopwatch;
        private static Vector2 direction = new Vector2(1, 0);

        private static float baseTimer;
        private static char[,] map;

        private List<bool> movements = Enumerable.Repeat(false, 7).ToList(); //{left,right,up,down,dash,grab,jump}

        [Command("mia", "Enable / Disable Mia")]
        public static void MiaCommand()
        {
            Settings.GetTiles = !Settings.GetTiles;
            Engine.Commands.Log(" Mia is now " + (Settings.GetTiles ? "enable." : "disable."), Color.GreenYellow);

        }




        public override void Load()
        {
            Utils.Print("Mod Loaded ========================== JEU GIY GYD(UB FEYT* FVY* BDVY*)EUI GV%^*CT&II UYUOPEB YG*OUIBFUJI(OV T&G*EPI.");
            On.Celeste.Player.Update += ModPlayerUpdate;
            Everest.Events.Player.OnSpawn += OnSpawn;
            Everest.Events.Level.OnLoadLevel += LoadLevel;

            stopwatch = new Stopwatch();
        }


        private void LoadLevel(Level level, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            if (isFromLoader && !Settings.GetTiles)
            {
                Utils.Print("Tiles won't be retreived. Change the option in the settings to make able to retrieve level, Mia will not work otherwise.");
            }
            if(isFromLoader && Settings.GetTiles)
            {
                map = level.SolidsData.ToArray();
            }


        }

        public override void Unload()
        {
            Utils.Print("Mod unloaded");
            On.Celeste.Player.Update -= ModPlayerUpdate;
            Everest.Events.Player.OnSpawn -= OnSpawn;
            Everest.Events.Level.OnLoadLevel -= LoadLevel;

        }


        private void OnSpawn(Player player)
        {
            if (Main.Settings.KillPlayer) stopwatch.Restart();
            if (Main.Settings.Debug && Main.Settings.KillPlayer) Utils.Print("Starting stopwatch");
            if (baseTimer == null)
            { 
                baseTimer = player.AutoJumpTimer;
            }
        }


        private void ModPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
        {

            if (Settings.SendRequests) Inputting.Move(movements);
                orig(self);
                GameWindow window = Engine.Instance.Window;
                if (Engine.Scene is Level level)
                {
                NeuralNetwork.NeuralNetwork.isWorking();
                
                    if (direction.X != 1 && MInput.Keyboard.Pressed(Input.MoveX.Positive.Keyboard[0])) { direction.X = 1; }
                    if (direction.X != -1 && MInput.Keyboard.Pressed(Input.MoveX.Negative.Keyboard[0])) { direction.X = -1; }

                    if (Settings.GetTiles) window.Title = "Celeste.exe/Mia enabled";
                    else window.Title = "Celeste.exe/Mia not enabled";

                }
        }
    }
}