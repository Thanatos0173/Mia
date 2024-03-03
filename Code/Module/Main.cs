using Monocle;
using System;
using Microsoft.Xna.Framework;

using Celeste.Mod.Mia.UtilsClass;
using Celeste.Mod.Mia.Settings;


using Celeste.Mod.Mia.InputAdder;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using NumSharp;
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



        private static double lr = 0.001;
        private static bool doPlay = false;
        private static int savedNumber = 0;
        private static int planeIndex = 0;
        private static NDArray threedimarray = np.zeros(10_000,20, 20);
        private static NDArray twodimarray = np.zeros(10000, 7);
        private static bool record;
        int index = 0;
        bool[] movements = new bool[7];
        private static List<List<string>> mainRoomsPerChapter = new List<List<string>>
        {
          new List<string>{"0", "1", "2"},
          new List<string>{"1", "2", "3", "4", "3b", "5", "6", "6a", "6b", "6c", "7", "8", "8b", "9", "9b", "10", "11", "12", "12a", "end"}
        };
        public int score = 0;






        [Command("play", "Make Mia Play")]
        public static void PlayCommand()
        {
            if (!doPlay) NeuralNetwork.NeuralNetwork.Open();
            doPlay = !doPlay;
            if (doPlay) record = false;
            string a = doPlay ? "" : "not";
            Engine.Commands.Log($"Mia should now be {a} playing");

        }

        [Command("record", "Record Command")]
        public static void RecordCommand()
        {
            record = !record;
            if (record) doPlay = false;
            Engine.Commands.Log($"Recording set to {record}");
            if (!record)
            {
                if (!Directory.Exists("Mia")) Directory.CreateDirectory("Mia");
                if (!Directory.Exists("Mia/Saves")) Directory.CreateDirectory("Mia/Saves");
                np.Save((Array)threedimarray[new Slice(0, planeIndex), new Slice(0, 20), new Slice(0, 20)], $"Mia/Saves/ArraySaved_{savedNumber}.npy");
                np.Save((Array)twodimarray[new Slice(0, planeIndex), new Slice(0, 7)], $"Mia/Saves/InputSaved_{savedNumber}.npy");

            }
        }


        [Command("save", "Save Command")]
        public static void SaveCommand()
        {
            NeuralNetwork.NeuralNetwork.Open();
            NeuralNetwork.NeuralNetwork.Save();
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
            Console.WriteLine(new FileInfo("Mia/structure_settings.txt").Length);
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
            }
            if (!File.Exists("Mia/structure_settings.txt")) File.Create("Mia/structure_settings.txt");
            if (settings.Count == 0) settings = new List<int> { 400, 256, 128, 56 };
            Engine.Commands.Log("Created the neural network with settings " + Utils.Convert2DArrayToString(settings.ToArray()));
            Engine.Commands.Log("You can modify the structure settings without closing the game by modifying the file /Mia/structure_settings.txt, located in your Celeste SaveFile.");
            NeuralNetwork.NeuralNetwork.Create(settings, Engine.Commands);

        }

        public override void Load()
        {
            Utils.Print("Sacha V Mia Loaded");
            On.Celeste.Player.Update += ModPlayerUpdate;
            Everest.Events.Level.OnLoadLevel += LoadLevel;
            Everest.Events.Level.OnExit += ExitLevel;
        }

        public override void Unload()
        {
            Utils.Print("Mod unloaded");
            On.Celeste.Player.Update -= ModPlayerUpdate;
            Everest.Events.Level.OnLoadLevel -= LoadLevel;
            if (savedNumber != 0)
            {
                if (!Directory.Exists("Mia")) Directory.CreateDirectory("Mia");
                if (!Directory.Exists("Mia/Saves")) Directory.CreateDirectory("Mia/Saves");
                np.Save((Array)threedimarray[new Slice(0, planeIndex), new Slice(0, 20), new Slice(0, 20)], $"Mia/Saves/ArraySaved_{savedNumber}.npy");
                np.Save((Array)twodimarray[new Slice(0, planeIndex), new Slice(0, 7)], $"Mia/Saves/InputSaved_{savedNumber}.npy");
            }
            Everest.Events.Level.OnExit -= ExitLevel;

        }
        private void ExitLevel(Level level, LevelExit exit, LevelExit.Mode mode, Session session, HiresSnow snow)
        {
            if(record) 
            {
                record = false;
            }
            if(savedNumber != 0) 
            {
                if (!Directory.Exists("Mia")) Directory.CreateDirectory("Mia");
                if (!Directory.Exists("Mia/Saves")) Directory.CreateDirectory("Mia/Saves");

                np.Save((Array)threedimarray[new Slice(0, planeIndex), new Slice(0, 20), new Slice(0, 20)], $"Mia/Saves/ArraySaved_{savedNumber}.npy");
                np.Save((Array)twodimarray[new Slice(0, planeIndex), new Slice(0, 7)], $"Mia/Saves/InputSaved_{savedNumber}.npy");
            }
        }

        private void LoadLevel(Level level, Player.IntroTypes playerIntro, bool isFromLoader)
        {
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

        public static void PrintProgressBar(double percentage, int one, int two)
        {
            int totalWidth = 100; // Total width of the progress bar
            int numHashes = (int)(percentage * totalWidth);
            string loadingBar = "[" + new string('#', numHashes) + new string(' ', totalWidth - numHashes) + "]";
            Console.Write($"\r{loadingBar} {percentage * 100:0.00} % {one}/{two}");
        }

        public static void Load2DInto3D(NDArray array2D, NDArray array3D, int planeIndex)
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

        public static void Load1DInto2D(NDArray array1D, NDArray array2D, int planeIndex)
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

        private void ModPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
        {
            orig(self);
            GameWindow window = Engine.Instance.Window;
            if (Engine.Scene is Level level)
            {
                if (record && self.Position != self.PreviousPosition)
                {
                    Load2DInto3D(new NDArray(TileManager.TileManager.FusedArrays(level, level.SolidsData.ToArray(), self)), threedimarray, planeIndex);
                    Load1DInto2D(new NDArray(Utils.GetInputs()), twodimarray, planeIndex);
                    ++planeIndex;
                    if (planeIndex >= 10_000)
                    {
                        Console.WriteLine("Creating New File");
                        if (!Directory.Exists("Mia")) Directory.CreateDirectory("Mia");
                        if (!Directory.Exists("Mia/Saves")) Directory.CreateDirectory("Mia/Saves");
                        np.Save((Array)threedimarray, $"Mia/Saves/ArraySaved_{savedNumber}.npy");
                        np.Save((Array)twodimarray, $"Mia/Saves/InputSaved_{savedNumber}.npy");
                        savedNumber++;
                        planeIndex = 0;
                        threedimarray = np.zeros(10_000, 20, 20);
                        twodimarray = np.zeros(10_000, 7);
                    }
                }
                
                if (doPlay && !Input.ESC) { 
                if (index%20 == 0)
                {
                    movements = Utils.GetWhatThingToMove(NeuralNetwork.NeuralNetwork.ForPropagation(new NDArray(Utils.Convert2DArrayTo1DArray(TileManager.TileManager.FusedArrays(level, level.SolidsData.ToArray(), self))).reshape(1, 400)));
                }
                if (doPlay && Input.ESC) doPlay = false;
                Inputting.MoveAsync(movements);
                ++index;
                }
                if (Settings.GetTiles) window.Title = "Celeste.exe/Mia enabled";
                else window.Title = "Celeste.exe/Mia not enabled";
            }
        }
    }
}
