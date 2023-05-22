using Celeste.Mod.Mia.UtilsClass;
using System;
using System.Collections.Generic;
using System.IO;

namespace Celeste.Mod.Mia.FileHandling
{
    public class FileHandling
    {
        public static List<string> LoadFile(string path) // Create files needed to the AI to work. 
        {
            string newPath = path;
            if (!(path.IndexOfAny(Path.GetInvalidPathChars()) == -1))
            {
                newPath = Environment.CurrentDirectory;
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
        public static void saveTiles(string path,string tiles)
        {
            string tilesFile = LoadFile(path)[0];
            System.IO.File.WriteAllText(Environment.CurrentDirectory + tilesFile + @"\current.npy", tiles);
 
            Utils.print(Environment.CurrentDirectory + tilesFile);
        }
    }
    
}
