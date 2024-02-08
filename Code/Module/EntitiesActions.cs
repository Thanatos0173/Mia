using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Monocle;
using Celeste.Mod.Mia.EntityExtension;
using IL.MonoMod;

namespace Celeste.Mod.Mia.Actions
{
    public class EntitiesActions
    {
        public static int Actions(Level level, Entity entity)
        {
            string newPath = Environment.CurrentDirectory + @"\Mia";
            if (!Directory.Exists(newPath)) Directory.CreateDirectory(newPath);
            string filePath = newPath + @"\EntitiesID.txt";
            if (!File.Exists(filePath)) using (File.Create(filePath)) { }
            var lines = File.ReadAllLines(filePath);
            int j=0;
            if (entity is Solid || entity.HaveComponent("Celeste.PlayerCollider"))
            {
                while(j < lines.Length) 
                {
//                    Console.WriteLine("j " + j + " " + lines.Length);

                    if (lines[j][0] == '#')
                    {

                        j += 1;
                        continue;
                    }
                    if (lines[j] == entity.ToString())
                    {
                        try { return int.Parse(lines[j + 1]); }
                        catch (FormatException) { Console.WriteLine($"{lines[j + 1]} could not be converted to an integer."); }
                    }
                    j += 2;
                }
//                Console.WriteLine(entity.ToString() + " have no ID. Please insert it manually in" + filePath);
                using (StreamWriter sw = File.AppendText(filePath))
                {
                    sw.WriteLine(entity.ToString());
                    sw.WriteLine("0");
                }
            }
            return 0;
        }
    }
}
