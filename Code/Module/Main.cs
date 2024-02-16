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

using NumSharp;
using NumSharp.Extensions;
using Microsoft.Xna.Framework.Input;
using YamlDotNet.Serialization;
using NumSharp.Generic;

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


        private static float baseTimer;

        private static double lr = 0.001;
        private static bool doPlay = false;

        private static int roomCompleted;
        private static int currentRoom = 0;
        private static int savedNumber = 0;
        private static int planeIndex = 0;
        private static NDArray threedimarray = np.zeros(10_000,20, 20);
        private static NDArray twodimarray = np.zeros(10000, 7);




        private static List<List<string>> mainRoomsPerChapter = new List<List<string>>
        {
          new List<string>{"0", "1", "2"},
          new List<string>{"1", "2", "3", "4", "3b", "5", "6", "6a", "6b", "6c", "7", "8", "8b", "9", "9b", "10", "11", "12", "12a", "end"}
        };
        public int score = 0;

        


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
            try
            {
                if (Settings.GetTiles)
                {
                    NeuralNetwork.NeuralNetwork.Open();
                }
                else NeuralNetwork.NeuralNetwork.Save();
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
            catch(Exception ex) 
            {
                Engine.Commands.Log("An error ocured.");
                Engine.Commands.Log("The problem might be that you haven't created the neural network");
                Engine.Commands.Log("If you have created it, you might want to contact Thanatos_0173 on GitHub.");
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
        public static void CreateCommand() {
            List<int> settings = new List<int> {};

            if (File.Exists("Mia/structure_settings.txt") && new FileInfo("Mia/structure_settings.txt").Length != 0)
            {
                string[] toParse = File.ReadLines("Mia/structure_settings.txt").FirstOrDefault().Split(' ');

                foreach (string substring in toParse)
                {
                    int result;
                    // Try parsing the substring as a string
                    if (!int.TryParse(substring, out result))
                    {
                        Console.WriteLine("File Issue: Creating Neural Network with hardcoded settings");
                        settings = new List<int> { 400, 256, 128, 56 };
                        break;
                    }
                    else
                    {
                        settings.Add(result);
                    }
                }
                if (settings[settings.Count - 1] != 56) settings.Add(56);   
                if(settings.Count == 0) settings = new List<int> { 400, 256, 128, 56 };
            }
            if (!File.Exists("Mia/structure_settings.txt")) File.Create("Mia/structure_settings.txt");
            Engine.Commands.Log("Created the neural network with settings " + Utils.Convert2DArrayToString(settings.ToArray()));
            Engine.Commands.Log("You can modify the structure settings without closing the game by modifying the file /Mia/structure_settings.txt, located in your Celeste SaveFile.");
            NeuralNetwork.NeuralNetwork.Create(settings, Engine.Commands);

        }

        private static void PrintProgressBar(double percentage, int one, int two)
        {
            int totalWidth = 100; // Total width of the progress bar
            int numHashes = (int)(percentage * totalWidth);
            string loadingBar = "[" + new string('#', numHashes) + new string(' ', totalWidth - numHashes) + "]";
            Console.Write($"\r{loadingBar} {percentage * 100:0.00} % {one}/{two}");
        }

        [Command("train","Train the neural network")]
        public static void TrainCommand()
        {
            Engine.Commands.Log("During this period of time, you won't be able to move");
            Engine.Commands.Log("Mia will train by herself. A message will be sent when the training will be over.");

            if (!Directory.Exists("Mia/Saves"))
            {
                Engine.Commands.Log("It seems like you have not record you plays. Mia can't train");
                return;
            }
            NeuralNetwork.NeuralNetwork.Open();

            for (int i = 0; i < Directory.GetFiles("Mia/Saves","*",SearchOption.AllDirectories).Length/2; i++)
            {
                var tdimarray = np.load($"Mia/Saves/ArraySaved_{i}.npy");
                var tdiminput = np.load($"Mia/Saves/InputSaved_{i}.npy");
                for(int j = 0; j < 10_000; j++)
                {
                    Engine.Commands.Log(tdimarray[j].Shape);
                    double[] input = (double[])(Array)tdiminput[j];
                    NeuralNetwork.NeuralNetwork.Train(lr, tdimarray[j], Utils.AllArrayFromOld(input ));
                    double progress = (double)(i * 10_000 + j + 1) / (Directory.GetFiles("Mia/Saves", "*", SearchOption.AllDirectories).Length/2 * 10_000);

                    // Update progress bar
                    PrintProgressBar(progress, i * 10_000 + j, Directory.GetFiles("Mia/Saves", "*", SearchOption.AllDirectories).Length/2 * 10_000);
                }
            }
            Engine.Commands.Log("Mia have been trained");

        }
        
        public override void Load()
        {
            Utils.Print("Sacha V Mia Loaded");
            On.Celeste.Player.Update += ModPlayerUpdate;
            Everest.Events.Level.OnLoadLevel += LoadLevel;
            On.Celeste.Level.LoadLevel += AddEntity;
            Everest.Events.Level.OnExit += ExitLevel;
        }

        private void ExitLevel(Level level, LevelExit exit, LevelExit.Mode mode, Session session, HiresSnow snow)
        {
            if (!Directory.Exists("Mia")) Directory.CreateDirectory("Mia");
            if (!Directory.Exists("Mia/Saves")) Directory.CreateDirectory("Mia/Saves");
            np.Save((Array)threedimarray, $"Mia/Saves/ArraySaved_{savedNumber}.npy");
            np.Save((Array)twodimarray, $"Mia/Saves/InputSaved_{savedNumber}.npy");
        }

        private void AddEntity(On.Celeste.Level.orig_LoadLevel orig, Level self, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            orig(self, playerIntro, isFromLoader);
            /*if (self.Tracker.GetEntity<Player>() != null)
            {
                self.Add(new KeyStokesEntity());
            }*/
            
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
            Everest.Events.Level.OnLoadLevel -= LoadLevel;
            On.Celeste.Level.LoadLevel -= AddEntity;
            if (!Directory.Exists("Mia")) Directory.CreateDirectory("Mia");
            if (!Directory.Exists("Mia/Saves")) Directory.CreateDirectory("Mia/Saves");
            np.Save((Array)threedimarray, $"Mia/Saves/ArraySaved_{savedNumber}.npy");
            np.Save((Array)twodimarray, $"Mia/Saves/InputSaved_{savedNumber}.npy");
            Everest.Events.Level.OnExit -= ExitLevel;

        }


        private static void Load2DInto3D(NDArray array2D, NDArray array3D, int planeIndex)
        {
            if (array2D.shape.Length != 2 || array3D.shape.Length != 3)
            {
                Console.WriteLine(array2D.shape.Length);
                Console.WriteLine(array3D.shape.Length);
                throw new ArgumentException("Array dimensions are not valid.");
            }

            if (array2D.shape[0] != array3D.shape[1] || array2D.shape[1] != array3D.shape[2])
            {
                throw new ArgumentException("Array dimensions are incompatible.");
            }

            if (planeIndex < 0 || planeIndex >= array3D.shape[0])
            {
                throw new ArgumentException("Plane index is out of bounds.");
            }

            array3D[planeIndex] = array2D;
        }

        private static void Load1DInto2D(NDArray array1D, NDArray array2D, int planeIndex)
        {
            if (array1D.shape.Length != 1 || array2D.shape.Length != 2)
            {
                throw new ArgumentException("Array dimensions are not valid.");
            }

            if (array1D.shape[0] != array2D.shape[1])
            {
                throw new ArgumentException("Array dimensions are incompatible.");
            }

            if (planeIndex < 0 || planeIndex >= array2D.shape[0])
            {
                throw new ArgumentException("Plane index is out of bounds.");
            }

            array2D[planeIndex] = array1D;
        }

        int i = 0;
        private void ModPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
        {
            orig(self);
            GameWindow window = Engine.Instance.Window;
            if (Engine.Scene is Level level)
            {
                Load2DInto3D(new NDArray(TileManager.TileManager.FusedArrays(level, level.SolidsData.ToArray(), self)), threedimarray, planeIndex);
                Load1DInto2D(new NDArray(Utils.GetInputs()), twodimarray, planeIndex);
                ++planeIndex;
                if(planeIndex >= 10_000)
                {
                    Console.WriteLine("Creating New File");
                    if (!Directory.Exists("Mia")) Directory.CreateDirectory("Mia");
                    if (!Directory.Exists("Mia/Saves")) Directory.CreateDirectory("Mia/Saves");
                    np.Save((Array)threedimarray, $"Mia/Saves/ArraySaved_{savedNumber}.npy");
                    np.Save((Array)twodimarray, $"Mia/Saves/InputSaved_{savedNumber}.npy");
                    savedNumber++;
                    planeIndex = 0;
                    threedimarray = np.zeros(10_000,20, 20);
                    twodimarray = np.zeros(10_000, 7);
                }
                


                /*     if (Settings.GetTiles)
            {
                if (!doPlay)
                {
                    if (self.Position != self.PreviousPosition)
                    {
                      NeuralNetwork.NeuralNetwork.Train(lr, 
                            Utils.Convert2DArrayTo1DArray(TileManager.TileManager.FusedArrays(level, level.SolidsData.ToArray(), self))
                           ,Utils.AllArray());

                    }
                }
                else
                    {
                        bool[] movements = Utils.GetWhatThingToMove(NeuralNetwork.NeuralNetwork.ForPropagation(new NDArray(Utils.Convert2DArrayTo1DArray(TileManager.TileManager.FusedArrays(level, level.SolidsData.ToArray(), self))).reshape(1, 400)));

                        Inputting.Move(movements);
                    }   
            }
         */   
                if (Settings.GetTiles) window.Title = "Celeste.exe/Mia enabled";
                else window.Title = "Celeste.exe/Mia not enabled";
            }
        }
    }
}