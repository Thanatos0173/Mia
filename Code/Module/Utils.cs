using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Serialization.Formatters;
using Celeste.Mod.mia.FileHandling;
using IL.Celeste;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;

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

        public static void putToFile(Level level)
        {
            String text = level.Session.LevelData.Position.ToString() + "\n";
            EntityList entities = level.Entities;


            for (int i = 0; i < entities.Count; i++)
            {
                int tag = level.Entities[i].Tag;
                string name = entities[i].ToString();
                if (tag == 16 /*Player*/|| (tag == 0 && !name.Contains("Decal") && !name.Contains("Straberry") && !name.Contains("SlashFx") && !name.Contains("Monocle") && !name.Contains("WaterSurface") && !name.Contains("CameraTargetTrigger")))
                { // Do not take useless informations. 
                    text += name;
                    string position = level.Entities[i].Position.ToString();
                    string height = level.Entities[i].Height.ToString();
                    string width = level.Entities[i].Width.ToString();
                    text += "(" + position + "{H: " + height + " W: " + width + "},tag: " + tag + ")\n";
                }
            }
            FileHandling.saveEntities(Module.Module.Settings.Path, text);

        }


        public static void WriteCurrentLevel(char[,] array, Level level)
        {
            FileHandling.saveTiles(Module.Module.Settings.Path, level, array);
        }


    }
}