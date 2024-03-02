using System;
using System.Collections.Generic;
using System.Linq;
using NumSharp;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using NumSharp.Generic;
//using Newtonsoft.Json;

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
        public static void PrintArray(bool[] array)
        {
            Console.Write("\r");
            foreach (bool element in array)
            {
                Console.Write((element ? 1 : 0).ToString() + " ");
            }

//            Console.WriteLine(); // Move to the next line after printing the array
        }
        public static void Print2dArray(char[,] arguments)
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
        public static int[] Convert2DArrayTo1DArray(int[,] twoDArray)
        {
            int rows = twoDArray.GetLength(0);
            int cols = twoDArray.GetLength(1);

            int[] oneDArray = new int[rows * cols];
            int index = 0;

            // Copy elements from 2D array to 1D array
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    oneDArray[index++] = twoDArray[i, j];
                }
            }

            return oneDArray;
        }
       

        public static bool[] GetWhatThingToMove(NDArray y_pred)
        {
            bool[] movements = new bool[7]; 
            int n = np.argmax(y_pred);
            int q = n / 9;
            int r = n % 9;
            if (q == 1)
            {
                movements[4] = true;
            }
            else if (q == 2)
            {
                movements[5] = true;
            }
            else if (q == 3)
            {
                movements[6] = true;
            }
            else if (q == 4)
            {
                movements[4] = true;
                movements[5] = true;
            }
            else if (q == 5)
            {
                movements[4] = true;
                movements[6] = true;
            }
            if (r == 1)
            {
                movements[2] = true;
            }
            else if (r == 2)
            {
                movements[0] = true;
            }
            else if (r == 3)
            {
                movements[3] = true;
            }
            else if (r == 4)
            {
                movements[1] = true;
            }
            else if (r == 5)
            {
                movements[2] = true;
                movements[0] = true;
            }
            else if (r == 6)
            {
                movements[2] = true;
                movements[1] = true;
            }
            else if (r == 7)
            {
                movements[3] = true;
                movements[0] = true;
            }
            else if (r == 8)
            {
                movements[3] = true;
                movements[1] = true;
            }
            return movements;
    }

        public static string Convert2DArrayToString(int[] array)
        {
            string s = "[";
            foreach(int item in array) { 
                s += item.ToString() + ",";
            }
            return s.Substring(0,s.Length-1) + "]";
        }

        public static int[] GetInputs()
        {
            int[] inputs = new int[7];

            if (Input.MoveX.Value == -1) inputs[0] = 1;
            if (Input.MoveX.Value == 1) inputs[1] = 1;
            if (Input.MoveY.Value == 1) inputs[2] = 1;
            if (Input.MoveY.Value == -1) inputs[3] = 1;
            if (Input.Grab.Check) inputs[4] = 1;
            if (Input.DashPressed) inputs[5] = 1;
            if (Input.Jump.Check) inputs[6] = 1;
            return inputs;

        }

        public static void CopyDirectory(string source, string target)
        {
            string folderName = source.Split('/').LastOrDefault();
            if(!Directory.Exists(target + "/" + folderName))Directory.CreateDirectory(target + "/" + folderName);
            foreach (var file in Directory.GetFiles(source))
            {
                string file_name = file.Split('\\').LastOrDefault();
                File.Copy(file, target + $"/{folderName}/" + file_name);

            }
        }


        public static void MoveDirectoryAndDeleteOld(string source, string target)
        {
            string folderName = source.Split('/').LastOrDefault();
            if (!Directory.Exists(target + "/" + folderName)) Directory.CreateDirectory(target + "/" + folderName);
            foreach (var file in Directory.GetFiles(source))
            {
                string file_name = file.Split('\\').LastOrDefault();
                File.Copy(file, target + $"/{folderName}/" + file_name);
                File.Delete(file);
            }
            Directory.Delete(source,true);
        }

        

    }

}