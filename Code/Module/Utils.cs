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