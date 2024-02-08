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
using myform;

using System.Net.Sockets;
using System.Text;
using Celeste.Mod.Mia.NeuralNetwork;
using Celeste.Mod.Mia.KeystrokesEntity;
using System.Data.OleDb;
using Celeste.Mod.Mia.EntityExtension;
using Celeste.Mod.Mia.Actions;
using Celeste.Mod.StaminaMeter;
using System.Security.Policy;

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
        private bool[] old { get; set; }

        public static int UwuInt { get; set; } = 0;
        private bool[] oldInput;
        private static double lr = 0.001;
        private static bool doPlay = false;

        private static int roomCompleted;
        private static int currentRoom = 0;
        private static List<List<string>> mainRoomsPerChapter = new List<List<string>>
        {
          new List<string>{"0", "1", "2"},
          new List<string>{"1", "2", "3", "4", "3b", "5", "6", "6a", "6b", "6c", "7", "8", "8b", "9", "9b", "10", "11", "12", "12a", "end"}
        };
        public int score = 0;

        

        private List<bool> movements = Enumerable.Repeat(false, 7).ToList(); //{left,right,up,down,dash,grab,jump}

        [Command("mia", "Enable / Disable Mia")]
        public static void MiaCommand(string lrS)
        {
            try
            {
                lr = double.Parse(lrS);
            }
            catch (ArgumentNullException)
            {

            }
            Settings.GetTiles = !Settings.GetTiles;
            if(Settings.GetTiles) 
            {
                NeuralNetwork.NeuralNetwork.Open();
                if(Engine.Scene is Level level)
                {
                    map = level.SolidsData.ToArray();

                }
            }
            else
            {
                NeuralNetwork.NeuralNetwork.Save();
            }
            Engine.Commands.Log(" Mia is now " + (Settings.GetTiles ? "enable. " : "disable."), Color.GreenYellow);
            if (!doPlay)
            {
                Engine.Commands.Log("Mia is in mode Train. That mean that by playing normally the game, she will train by herself. To put it in play mode, you need to type the command \"switch\"");
            }
            else
            {
                Engine.Commands.Log("Mia is in mode Play. That mean that she will play by herself. To put it in train mode, you need to type the command \"switch\"");
            }

        }

        [Command("switch", "Switch from train to play / play to train")]
        public static void SwitchCommand(string lrS)
        {
            
            doPlay = !doPlay;
            if (!doPlay)
            {
                Engine.Commands.Log("Mia is in mode Train. That mean that by playing normally the game, she will train by herself. To put it in play mode, you need to type the command \"switch\"");
            }
            else
            {
                Engine.Commands.Log("Mia is in mode Play. That mean that she will play by herself. To put it in train mode, you need to type the command \"switch\"");
            }

        }








        [Command("uwu", "say UwU")]
        public static void UwUCommand()
        {
            Engine.Commands.Log("You should look at the console", Color.GreenYellow);
            Console.WriteLine("UwU");
        }

        [Command("create", "Create Neural Network Structure, Use carefully")]
        public static void CreateCommand()
        {
            //            int[] settings = new int[] { 400, 2048, 1024, 56 };
                        int[] settings = new int[] { 400, 128, 64, 56 };

            Engine.Commands.Log("Created the neural network with settings " + Utils.Convert2DArrayToString(settings));

            NeuralNetwork.NeuralNetwork.Create(settings, Engine.Commands);

        }



        public override void Load()
        {
            Utils.Print("Mod Loaded ==========================================================================================================================================================================================================================================");
            On.Celeste.Player.Update += ModPlayerUpdate;
            Everest.Events.Player.OnSpawn += OnSpawn;
            Everest.Events.Level.OnLoadLevel += LoadLevel;
            stopwatch = new Stopwatch();
        }


        private void LoadLevel(Level level, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            if (isFromLoader)
            {
                roomCompleted = 0;
            }
            if (isFromLoader && !Settings.GetTiles)
            {
                Utils.Print("Tiles won't be retreived. Change the option in the settings to make able to retrieve level, Mia will not work otherwise.");
            }
            if (isFromLoader && Settings.GetTiles)
            {
                map = level.SolidsData.ToArray();
            }
            if (level.Session.Area.ID < mainRoomsPerChapter.Count)
            {
                if (mainRoomsPerChapter[level.Session.Area.ID].IndexOf(level.Session.LevelData.Name) != -1)
                {
                    score++;
                }
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
            if (Engine.Scene is Level level)
            {
                Console.WriteLine(level.Session.Area.LevelSet);
            }

            if (Main.Settings.KillPlayer) stopwatch.Restart();
            if (Main.Settings.Debug && Main.Settings.KillPlayer) Utils.Print("Starting stopwatch");
            if (baseTimer == null)
            {
                baseTimer = player.AutoJumpTimer;
            }
        }

        private void Show(bool[] input)
        {
            string[] inputThing = { "Right", "Left", "Up", "Down", "Dash", "Jump", "Grab" };
            for (int i = 0; i < 7; i++)
            {
                Console.Write(inputThing[i] + ": " + input[i] + " \n");
            }
        }



        int i = 0;
        private void ModPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
        {
            if (Settings.SendRequests) Inputting.Move(movements);
            orig(self);
            GameWindow window = Engine.Instance.Window;
            if (Engine.Scene is Level level)
            {
                if (Settings.GetTiles && self.Position != self.PreviousPosition)
                {
                    if (self.Position != self.PreviousPosition)
                    {
                        if (!doPlay && (self.Position != self.PreviousPosition)) 
                        {
//                            Console.WriteLine("Player moved");
//?                            Utils.Print2dArray(map);
                            NeuralNetwork.NeuralNetwork.Train(lr, 
                                Utils.Convert2DArrayTo1DArray(TileManager.TileManager.FusedArrays(level, level.SolidsData.ToArray(), self))
                                ,Utils.AllArray());

                        }
                        else
                        {
                            //NeuralNetwork.NeuralNetwork.ForPropagation(Utils.Convert2DArrayTo1DArray(TileManager.TileManager.FusedArrays(level, map, self)));

                        }
                    }
                    
                }


                /*                if (GetIfInputHaveChanges)
                                {
                                    bool[] currentInput = Utils.GetInputs();

                                    // Compare each element of the arrays to detect changes
                                    bool inputChanged = oldInput == null || !Enumerable.SequenceEqual(currentInput, oldInput);

                                    if (inputChanged)
                                    {
                                        //                    NeuralNetwork.NeuralNetwork.Create(Utils.AllArray());
                                    }

                                    // Update oldInput to the current state
                                    oldInput = currentInput;

                                }*/

                if (Settings.GetTiles) window.Title = "Celeste.exe/Mia enabled";
                else window.Title = "Celeste.exe/Mia not enabled";

                level.Add(new KeyStokesEntity());
            }
        }
    }
}