using Microsoft.Xna.Framework;
using System;
using System.IO;
using Monocle;

namespace Celeste.Mod.Mia.EntityExtension
{
    public static class EntityExtensions
    {
        public static int UUID(this Entity entity) /// Assign UUID to entity if non-existing UUID and return it as a int between 0 and 126
        {

            string name = entity.ToString();
            string newPath = Environment.CurrentDirectory + @"\Mia";
            if (!Directory.Exists(newPath))
                Directory.CreateDirectory(newPath);
            string filePath = newPath + @"\UUIDs.txt";
            if (!File.Exists(filePath))
                using (File.Create(filePath)) { }
            var lines = File.ReadAllLines(filePath);
            for (int i = 0; i < lines.Length - 1; i += 2)
            {
                if (lines[i] == name)
                {
                    try { return int.Parse(lines[i + 1]); }
                    catch (FormatException) { Console.WriteLine($"{lines[i + 1]} could not be converted to an integer."); }
                }
            }
            int lastUUID = lines.Length > 0 ? int.Parse(lines[lines.Length - 1]) : 0;
            if (lastUUID + 1 > 126)
            {
                Console.WriteLine($"The maximum value of UUIDs that can be assigned (125) has been reached. The entity {name} could not be assigned a UUID and has been assigned -1. Issues might occur or have occurred.");
                return -1;
            }
            using (StreamWriter sw = File.AppendText(filePath))
            {
                sw.WriteLine(name);
                sw.WriteLine((lastUUID + 1).ToString());
            }
            return lastUUID + 1;
        }
        
        public static bool IsAroundPosition(this Entity entity, Vector2 position)
        {
            return (Math.Abs((int)entity.Position.X - position.X) <= 80 && Math.Abs((int)entity.Position.Y - position.Y) <= 80);
        }
        public static bool HaveComponent(this Entity entity, String component)
        {
            foreach (Component componentToCheck in entity.Components)
            {
                if(componentToCheck.ToString() == component) return true;
            }
            return false;
        }
    }
}
