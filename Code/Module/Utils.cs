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
        public static int[] AllArray()
        {
            int[] y_true = new int[56];
            int i = 0;
            if ((Input.Grab.Check) && !(Input.DashPressed) && !(Input.Jump.Check))
            {
                i += 9;
            }
            else if (!(Input.Grab.Check) && (Input.DashPressed) && !(Input.Jump.Check))
            {
                i += 18;
            }
            else if (!(Input.Grab.Check) && !(Input.DashPressed) && (Input.Jump.Check))
            {
                i += 27;
            }
            else if ((Input.Grab.Check) && (Input.DashPressed) && !(Input.Jump.Check))
            {
                i += 36;
            }
            else if ((Input.Grab.Check) && !(Input.DashPressed) && (Input.Jump.Check))
            {
                i += 45;
            }
            else if (!(!(Input.Grab.Check) && !(Input.DashPressed) && !(Input.Jump.Check)))
            {
                return new int[56];
            }
            if (!(Input.MoveX.Value == 1) && !(Input.MoveX.Value == -1) && !(Input.MoveY.Value == -1) && (Input.MoveY.Value == 1))
            {
                i += 1;
            }
            else if (!(Input.MoveX.Value == 1) && (Input.MoveX.Value == -1) && !(Input.MoveY.Value == -1) && !(Input.MoveY.Value == 1))
            {
                i += 2;
            }
            else if (!(Input.MoveX.Value == 1) && !(Input.MoveX.Value == -1) && (Input.MoveY.Value == -1) && !(Input.MoveY.Value == 1))
            {
                i += 3;
            }
            else if ((Input.MoveX.Value == 1) && !(Input.MoveX.Value == -1) && !(Input.MoveY.Value == -1) && !(Input.MoveY.Value == 1))
            {
                i += 4;
            }
            else if (!(Input.MoveX.Value == 1) && (Input.MoveX.Value == -1) && !(Input.MoveY.Value == -1) && (Input.MoveY.Value == 1))
            {
                i += 5;
            }
            else if ((Input.MoveX.Value == 1) && !(Input.MoveX.Value == -1) && !(Input.MoveY.Value == -1) && (Input.MoveY.Value == 1))
            {
                i += 6;
            }
            else if (!(Input.MoveX.Value == 1) && (Input.MoveX.Value == -1) && (Input.MoveY.Value == -1) && !(Input.MoveY.Value == 1))
            {
                i += 7;
            }
            else if ((Input.MoveX.Value == 1) && !(Input.MoveX.Value == -1) && (Input.MoveY.Value == -1) && !(Input.MoveY.Value == 1))
            {
                i += 8;
            }
            else if (!(!(Input.MoveX.Value == 1) && !(Input.MoveX.Value == -1) && !(Input.MoveY.Value == -1) && !(Input.MoveY.Value == 1)))
            {
                return new int[56];
            }
            y_true[i] = 1;
            return y_true;
        }
        public static NDArray AllArrayFromOld(double[] array)
        {
            NDArray y_true = new int[56];
            int i = 0;

            if (array[6] == 1 && array[4] != 1 && array[5] != 1)
            {
                i += 9;
            }
            else if (array[6] != 1 && array[4] == 1 && array[5] != 1)
            {
                i += 18;
            }
            else if (array[6] != 1 && array[4] != 1 && array[5] == 1)
            {
                i += 27;
            }
            else if (array[6] == 1 && array[4] == 1 && array[5] != 1)
            {
                i += 36;
            }
            else if (array[6] == 1 && array[4] != 1 && array[5] == 1)
            {
                i += 45;
            }
            else if (!(array[6] == 1 || array[4] == 1 || array[5] == 1))
            {
                return new int[56];
            }

            if (array[0] != 1 && array[1] != 1 && array[2] != 1 && array[3] == 1)
            {
                i += 1;
            }
            else if (array[0] != 1 && array[1] == 1 && array[2] != 1 && array[3] != 1)
            {
                i += 2;
            }
            else if (array[0] != 1 && array[1] != 1 && array[2] == 1 && array[3] != 1)
            {
                i += 3;
            }
            else if (array[0] == 1 && array[1] != 1 && array[2] != 1 && array[3] != 1)
            {
                i += 4;
            }
            else if (array[0] != 1 && array[1] == 1 && array[2] != 1 && array[3] == 1)
            {
                i += 5;
            }
            else if (array[0] == 1 && array[1] != 1 && array[2] != 1 && array[3] == 1)
            {
                i += 6;
            }
            else if (array[0] != 1 && array[1] == 1 && array[2] == 1 && array[3] != 1)
            {
                i += 7;
            }
            else if (array[0] == 1 && array[1] != 1 && array[2] == 1 && array[3] != 1)
            {
                i += 8;
            }
            else if (!(array[0] != 1 || array[1] != 1 || array[2] != 1 || array[3] != 1))
            {
                return new int[56];
            }

            y_true[i] = 1;
            return y_true;
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

            if (Input.MoveX.Value == 1) inputs[0] = 1;
            if (Input.MoveX.Value == -1) inputs[1] = 1;
            if (Input.MoveY.Value == -1) inputs[2] = 1;
            if (Input.MoveY.Value == 1) inputs[3] = 1;
            if (Input.DashPressed) inputs[4] = 1;
            if (Input.Jump.Check) inputs[5] = 1;
            if (Input.Grab.Check) inputs[6] = 1;

            return inputs;

        }


        public static bool AreEquals(bool[] val1, bool[] val2) {
            for(int i = 0; i < Math.Max(val1.Length,val2.Length); i++)
            {
                if (val1[i] != val2[i]) return false;
            }
            return true;
        
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