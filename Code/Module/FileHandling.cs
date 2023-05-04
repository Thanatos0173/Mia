using Celeste.Mod.Mia.UtilsClass;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Celeste.Mod.mia.FileHandling
{
    public class FileHandling
    {
        public static List<string> LoadFile(string path) // Create files needed to the AI to work. 
        {
            string newPath = path;
            if (!(path.IndexOfAny(Path.GetInvalidPathChars()) == -1))
            {
                newPath = Environment.CurrentDirectory;
//                Utils.print("File Path",path,"invalid. Saving at", newPath);
            }
            List<string> result = new List<string>();
            string tilesFile = newPath + @"\Mia\Tiles";
            if(!Directory.Exists(tilesFile)) { Directory.CreateDirectory(tilesFile); }
            result.Add(tilesFile);  
            return result;
        } 

        public static void saveEntities(string path,string entities)
        {
            string tilesFile = LoadFile(path)[0];
            System.IO.File.WriteAllText(tilesFile + @"\tiles.txt", entities);
        }
        public static void saveTiles(string path,Level level,char[,] array)
        {
            string tilesFile = LoadFile(path)[0];
            string UUID = level.Session.LevelData.LoadSeed.ToString();
            string chapterName = AreaData.Get(level.Session.Area).Name.DialogClean();
            if (!Directory.Exists(tilesFile + @"/"+chapterName)) { Directory.CreateDirectory(tilesFile + @"/" + chapterName); }
            int x = Math.Abs(level.TileBounds.X - (level.Session.LevelData.TileBounds.X));
            int y = Math.Abs(level.TileBounds.Y - (level.Session.LevelData.TileBounds.Y));
            DirectoryInfo dirTilesFiles = new DirectoryInfo(tilesFile + @"/" + chapterName);
            FileInfo[] Files = dirTilesFiles.GetFiles("*.txt");
            bool fileExist = false;
            foreach (FileInfo file in Files)
            {
                if (file.Name == UUID+".txt") { fileExist = true; Utils.print(file.OpenText().ReadToEnd()); break; }
                Utils.print("ee");
            }
            if (!fileExist) {
                Utils.print("Writing level for the first time. Celeste might not respond for a few seconds, but that's normal.");
                String tiles = "";
                for (int j = y; j < y + level.Session.LevelData.TileBounds.Height; j++)
                {
                    for (int i = x; i < x + level.Session.LevelData.TileBounds.Width; i++)
                    {
                        if (array[i, j] != '0') tiles += "+";
                        else tiles += " ";
                    }
                    tiles += "\n";

                    string TenHash = new string('#', (int)(j * 100 / array.GetLength(1)));
                    string EmptyRest = new string(' ', 100 - (int)(j * 100 / array.GetLength(1)));

                    Console.Write("\r[" + TenHash + EmptyRest + "]" + (int)(j * 100 / array.GetLength(1)) + "%              ");
                }
                Console.Write("\r[####################################################################################################]100%\n");
                Utils.print(tiles);
                System.IO.File.WriteAllText(tilesFile + @"/" + chapterName + @"\"+UUID+".txt", tiles);
            }
        }
    }
}
