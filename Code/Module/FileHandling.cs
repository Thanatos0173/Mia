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
        public static void saveTiles(string path,string tiles)
        {
            string tilesFile = LoadFile(path)[0];
            System.IO.File.WriteAllText(Environment.CurrentDirectory + tilesFile + @"\current.npy", tiles);
 
            Utils.print(Environment.CurrentDirectory + tilesFile);
        }
    }
    
}
