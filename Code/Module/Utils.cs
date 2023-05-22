using System;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Mod.Mia.UtilsClass
{
    public class Utils
    {
        public static void print(params object[] arguments)
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
            Logger.Log("Tipe Mod", text);
        }
        public static void printList(List<string> arguments)
        {
            string text = "";
            foreach (string arg in arguments)
            {
                text += arg + " ";
            }
            Utils.print(text);
        }
        public static void print2dArray(int[,] arguments)
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
            Utils.print(text);

        }

    }
}