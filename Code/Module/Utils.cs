using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Serialization.Formatters;
using Celeste.Mod.Mia.FileHandling;
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
        public static void printList(List<string> arguments)
        {
            string text = "";
            foreach (string arg in arguments)
            {
                text += arg + " ";
            }
            Utils.print(text);
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
            Mia.FileHandling.FileHandling.saveEntities(Mia.Main.Settings.Path, text);

        }
        public static List<Entity> entityList(Level level,Player player)
        {
            String text = level.Session.LevelData.Position.ToString() + "\n";
            EntityList entities = level.Entities;
            List<Entity> list = new List<Entity>();
            List<string> lis = new List<string>();
            for (int i = 0; i < entities.Count; i++)
            {
                int tag = level.Entities[i].Tag;
                string name = entities[i].ToString();
                Vector2 position = level.Entities[i].Position;

                int xDiffBetweenPlayerAndEntity = Math.Abs((int)position.X - (int)player.Position.X);
                int yDiffBetweenPlayerAndEntity = Math.Abs((int)position.Y - (int)player.Position.Y);
                if ((xDiffBetweenPlayerAndEntity < 200 && yDiffBetweenPlayerAndEntity < 200) &&(tag == 16 /*Player*/|| (tag == 0 && !name.Contains("Decal") && !name.Contains("Straberry") && !name.Contains("SlashFx") && !name.Contains("Monocle") && !name.Contains("WaterSurface") && !name.Contains("CameraTargetTrigger"))))
                { // Do not take useless informations. 
                    list.Add(entities[i]);
                    lis.Add(name + " " + position.ToString() + xDiffBetweenPlayerAndEntity.ToString());
                }
            }
            return list;

        }

        public static char[,] getTilesAroundPlayer(Level level, char[,] array, Player player)
        {

            char[,] tilesAroundPlayer = new char[20,20];

            int playerXTile = (int)player.Position.X / 8; //Coordinates on player in tiles
            int playerYTile = (int)player.Position.Y / 8;

            int playerX = Math.Abs(level.TileBounds.X - playerXTile);
            int playerY = Math.Abs(level.TileBounds.Y - playerYTile);

            for (int j = playerY - 10; j < playerY + 10; j++)
            {
                for (int i = playerX - 10; i < playerX + 10; i++)
                {
                    int incrI = i - (playerX - 10);
                    int incrJ = j - (playerY - 10);
                    try
                    {
                        if (array[i, j] != '0') tilesAroundPlayer[incrI,incrJ] = '1';
                        else tilesAroundPlayer[incrI, incrJ] = '0';
                    }
                    catch (IndexOutOfRangeException) { tilesAroundPlayer[incrI, incrJ] = '0'; }
                }

            }
            return tilesAroundPlayer;
        }

    
        public static string getTilesAroundPlayerAsString(Level level, char[,] array, Player player)
        {
            int playerXTile = (int)player.Position.X / 8; //Coordinates on player in tiles
            int playerYTile = (int)player.Position.Y / 8;
            int playerX = Math.Abs(level.TileBounds.X - playerXTile);
            int playerY = Math.Abs(level.TileBounds.Y - playerYTile);
            
            string tiles = "";
            for (int j = playerY - 10; j < playerY + 10; j++)
            {
                for (int i = playerX - 10; i < playerX + 10; i++)
                {
                    try
                    {
                        if (array[i, j] != '0') tiles += "+";
                        else tiles += " ";
                    }
                    catch (IndexOutOfRangeException) { tiles += "+"; }
                }
                tiles += "\n";
            }
            return tiles;
        }
    }
}