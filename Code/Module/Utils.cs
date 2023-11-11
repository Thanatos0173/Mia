using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using static Celeste.TrackSpinner;
using System.Runtime.InteropServices.ComTypes;

namespace Celeste.Mod.Mia.UtilsClass
{
    public class Utils
    {
        public static void Print(params object[] arguments)
        {
            if (!arguments.Any())
            {
                throw new System.Exception("You need to provide at least 1 argument");
            }
            String text = "";
            foreach (object arg in arguments)
            {
                text += arg.ToString();
                text += " ";
            }
            Logger.Log("Mia", text);
        }
        public static void PrintList(List<string> arguments)
        {
            string text = "";
            foreach (string arg in arguments)
            {
                text += arg + " ";
            }
            Utils.Print(text);
        }
        public static void Print2dArray(int[,] arguments)
        {
            string text = "\n";
            for(int j = 0;j <arguments.GetLength(1); j++)
            {
                for (int i = 0; i < arguments.GetLength(0); i++)
                {
                    text += arguments[i, j].ToString();
                }
                text += "\n";
            }
            Utils.Print(text);

        }


    }
}

/*    PlayerManager.PlayerManager.ManagePlayer(stopwatch, self, level);
                        int[,] array = TileManager.TileManager.FusedArrays(level,map,self);
                        string path = Environment.CurrentDirectory;
                        string[] substring = @path.Split('\\');
                        substring = substring.Take(substring.Length - 1).ToArray();
                        string newPath = string.Join("\\", substring);

                        if (!File.Exists(newPath + @"\pythonFiles\tiles.txt"))
                            File.Create(newPath + @"\pythonFiles\tiles.txt");

                        string toAdd  = string.Empty;
                       for(int i = 0; i < 20; i++)
                        {
                            for(int j = 0; j < 20; j++)
                            {
                                toAdd = String.Concat(toAdd," ",array[j,i].ToString());


                            }
                                                }
                        File.WriteAllText(newPath + @"\pythonFiles\tiles.txt", toAdd);

                                   Utils.Print2dArray(
                                       TileManager.TileManager.GetEntityAroundPlayerAsTiles(level, self));
                                   Utils.Print2dArray(
                                       TileManager.TileManager.GetTilesAroundPlayer(level, map, self));*/