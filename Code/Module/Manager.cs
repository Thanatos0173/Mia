﻿using Celeste;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CelesteBot.Manager
{
    public class Manager
    {
        private static Vector2 FontScale = new Vector2(0.4f, 0.4f);
        private static Vector2 textPos = new Vector2(20f, 30f);
        private static KeyboardState kbState;
        public static String activeText = "Vision:\n"; // overriden by INTEROP

        // NEAT PARAMS:
        // For Genome:
        public static float RE_RANDOMIZE_WEIGHT_CHANCE = 0.1f;
        public static float WEIGHT_MUTATION_CHANCE = 0.7f;
        public static float ADD_CONNECTION_CHANCE = 0.1f;
        public static float ADD_NODE_CHANCE = 0.01f;
        // For Population:
        public static int EXTINCTION_SAVE_TOP = 5; // The number of Species to save when a mass extinction occurs




        private static bool IsKeyDown(Keys key)
        {
            return kbState.IsKeyDown(key);
        }
        public static MTexture GetTile(Vector2 tile)
        {
            Level celesteLevel = Celeste.Celeste.Scene as Level;
            SolidTiles tiles = celesteLevel.SolidTiles;
            MTexture[,] tileArray = tiles.Tiles.Tiles.ToArray();
            return tileArray[(int)tile.X, (int)tile.Y];
        }
        public static Vector2 GetXYCenterFromTile(Vector2 tPos)
        {
            int offsetX = -428;
            int offsetY = -244;
            int width = 8, height = 8;
            return new Vector2(offsetX + tPos.X * width, offsetY + tPos.Y * height);
        }
        public static Vector2 GetXYTopLeftFromTile(Vector2 tPos)
        {
            int offsetX = -432;
            int offsetY = -248;
            int width = 8, height = 8;
            return new Vector2(offsetX + tPos.X * width, offsetY + tPos.Y * height);
        }
        public static Vector2 GetTilePosFromXYCenter(Vector2 realPos)
        {
            int offsetX = -428;
            int offsetY = -244;
            int width = 8, height = 8;
            return new Vector2((int)((realPos.X - offsetX) / width), (int)((realPos.Y - offsetY) / height));
        }
        public static Vector2 GetTilePosFromXYTopLeft(Vector2 realPos)
        {
            int offsetX = -432;
            int offsetY = -248;
            int width = 8, height = 8;
            return new Vector2((int)((realPos.X - offsetX) / width), (int)((realPos.Y - offsetY) / height));
        }
        public static Entity GetEntityFromXYCenter(Vector2 realPos)
        {
            //System.Collections.Generic.List<Entity> entities = Celeste.Celeste.Scene.Tracker.GetEntities<Entity>();
            EntityList entities = Celeste.Celeste.Scene.Entities;/*Tracker.GetEntities<Entity>();*/
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].Collidable && entities[i].CollidePoint(realPos) && entities[i].Tag != 16) // 16=Tag of player
                {
                    // Collided with the point and is collidable
                    return entities[i];
                }
            }
            return null;
        }
        public static Vector2 GetPlayerPos()
        {
            Player player = Celeste.Celeste.Scene.Tracker.GetEntity<Player>();
            if (player != null)
                return player.Position; // This is the bottom center
            return new Vector2(0, 0);
        }
        public static Vector2 GetTileUnderPlayer()
        {
            Vector2 playerPos = GetPlayerPos();
            playerPos = new Vector2(playerPos.X, playerPos.Y + 4);

            return GetTilePosFromXYCenter(playerPos); // This is to account for offset
        }
        public static Vector2 GetTileInFrontOfPlayer()
        {
            Player player = Celeste.Celeste.Scene.Tracker.GetEntity<Player>();
            if (player == null) return new Vector2(0, 0);
            Vector2 playerPos = player.Position; // This is the bottom center

            if (player.Facing > 0)
            {
                playerPos = new Vector2(playerPos.X + 8, playerPos.Y - 4); // This is hacky but probably works
                // Facing Right
            }
            else
            {
                playerPos = new Vector2(playerPos.X - 8, playerPos.Y - 4); // This is hacky but probably works
                // Facing Left
            }

            return GetTilePosFromXYCenter(playerPos);
        }
        public static bool IsTile(Vector2 tile)
        {
            Level celesteLevel = Celeste.Celeste.Scene as Level;
            SolidTiles tiles = celesteLevel.SolidTiles;
            MTexture[,] tileArray = tiles.Tiles.Tiles.ToArray();
            return tileArray[(int)tile.X, (int)tile.Y] != null;
        }
        public static bool IsWall(Vector2 tile)
        {
            Level celesteLevel = Celeste.Celeste.Scene as Level;
            SolidTiles tiles = celesteLevel.SolidTiles;
            MTexture[,] tileArray = tiles.Tiles.Tiles.ToArray();
            if (tileArray[(int)tile.X, (int)tile.Y] != null)
            {
                if (!tileArray[(int)tile.X, (int)tile.Y].ToString().Equals("tilesets/scenery"))
                {
                    return true; // It isn't part of the scenery, so it must be something that i can hit
                }
            }
            return false; // it either is blank or is part of the scenery
        }
        public static void PutEntitiesToFile()
        {
            Level celesteLevel = Celeste.Celeste.Scene as Level;
            SolidTiles tiles = celesteLevel.SolidTiles;

            string readableTextures = "Center of all tiles (?): " + tiles.Center + "\n";
            try
            {
                readableTextures += "Tile(s?) are at Position: " + tiles.Tiles.Position + " with tile (w, h): (" + tiles.Tiles.TileWidth + ", " + tiles.Tiles.TileHeight + ")\n";
                System.Collections.Generic.IEnumerator<Component> enummer = tiles.GetEnumerator();
                enummer.Reset();
                for (int i = 0; i < 100000; i++)
                {
                    enummer.MoveNext();
                    readableTextures += "Texture (?) at Position: " + enummer.Current.Entity.Position + " and Center: " + enummer.Current.Entity.Center + " has tag: " + enummer.Current.Entity.Tag + "\n";
                }
            }
            catch (Exception)
            {
                readableTextures += "Enummer has completed!";
            }
            /*
            MTexture[,] textures = tiles.Tiles.Tiles.ToArray();
            string readableTextures = "";
            for (int i = 0; i < textures.Length; i++)
            {
                for (int j = 0; j < textures.Length; j++)
                {
                    try
                    {
                        readableTextures += "Texture at: " + textures[i, j].Center + " looks like (as a string): " + textures[i, j].ToString() + "\n";
                    } catch (IndexOutOfRangeException e)
                    {
                        readableTextures += "Texture at index: (" + i + ", " + j + ") is out of bounds!\n";
                    }
                }
                
            }
            */

            MTexture[,] tileArray = tiles.Tiles.Tiles.ToArray();

            //System.IO.File.WriteAllText(@"C:\Program Files (x86)\Steam\steamapps\common\Celeste\tiles.txt", text);
            System.IO.File.WriteAllText(@"C:\Users\elioo\OneDrive\Bureau\TIPE\CelesteSaving\readableTextures.txt", readableTextures);
            //System.IO.File.WriteAllText(@"C:\Program Files (x86)\Steam\steamapps\common\Celeste\textures.txt", arrayText);

            EntityList entities = Celeste.Celeste.Scene.Entities;/*Tracker.GetEntities<Entity>();*/

            Vector2 tileLocationUnderPlayer = GetTileUnderPlayer();
            Vector2 tileLocationInFront = GetTileInFrontOfPlayer();

            Player player = Celeste.Celeste.Scene.Tracker.GetEntity<Player>();
            Vector2 playerPos = player.Position;

            string toWrite = "Tile under player does not exist! err: " + tileLocationUnderPlayer + " with player pos: " + playerPos;
            if (IsTile(tileLocationUnderPlayer))
            {
                // This should be the tile X,Y under the player. Save to file.
                toWrite = "Tile under player has Center: " + GetXYCenterFromTile(tileLocationUnderPlayer) + " and Tile Loc: " + tileLocationUnderPlayer;
                toWrite += "\nWith Player pos: " + playerPos;
            }
            MTexture texture = null;
            try
            {
                texture = GetTile(tileLocationInFront);
            }
            catch (NullReferenceException e)
            {
                // Texture/Tile does not exist

            }
            if (IsTile(tileLocationInFront))
            {
                toWrite += "\nTile in front of player has Center: " + GetXYCenterFromTile(tileLocationInFront) + " and Tile Loc: " + tileLocationInFront;
                toWrite += "\nWith Player facing: " + (player.Facing == (Facings)1 ? "Right" : "Left");
                if (texture != null)
                {
                    toWrite += "\nWith tag: " + tileArray[(int)tileLocationInFront.X, (int)tileLocationInFront.Y].ToString();
                    toWrite += "\nWhich is " + (IsWall(tileLocationInFront) ? "" : "NOT ") + "a wall";
                }
            }
            else
            {
                toWrite += "\nTile in front of player does not exist! err: " + tileLocationInFront + " with player pos: " + playerPos + " facing: " + (player.Facing == (Facings)1 ? "Right" : "Left");
            }


            System.IO.File.WriteAllText(@"C:\Users\elioo\OneDrive\Bureau\TIPE\CelesteSaving\info.txt", toWrite);


            //string text2 = "";
            string readableText = "";
            for (int i = 0; i < entities.Count; i++)
            {
                //text += entities[i].ToString() + "\n";
                readableText += "Entity at Position: " + entities[i].Position + " and Center: " + entities[i].Center + " has tag: " + entities[i].Tag + " with collision: " + entities[i].Collidable + "\n";
            }
            //System.IO.File.WriteAllText(@"C:\Program Files (x86)\Steam\steamapps\common\Celeste\entities.txt", text2);
            System.IO.File.WriteAllText(@"C:\Users\elioo\OneDrive\Bureau\TIPE\CelesteSaving\readableEntities.txt", readableText);
        }
        public static MTexture[,] GetVision()
        {

            int visionX = 10;
            int visionY = 10;

            int underYIndex = visionY / 2 + 1;
            int underXIndex = visionX / 2;
            Vector2 tileUnder = GetTileUnderPlayer();
            /*
             * 0    1   2   3   4
             * 1    1   2   3   4
             * 2    1   2   3   4
             * 3    1   2   3   4
             * 4    1   2   3   4
             * 5    1   2   3   4
             */
            MTexture[,] outTextures = new MTexture[visionX, visionY];
            for (int i = 0; i < visionX; i++)
            {
                for (int j = 0; j < visionY; j++)
                {
                    outTextures[i, j] = GetTile(new Vector2(tileUnder.X - underXIndex + j, tileUnder.Y - underYIndex + i));
                }
            }
            string text = new Vector2(tileUnder.X - underXIndex, tileUnder.Y - underYIndex).ToString();
            text += "\n" + new Vector2(tileUnder.X - underXIndex + visionX - 1, tileUnder.Y - underYIndex + visionY - 1);
            System.IO.File.WriteAllText(@"C:\Users\elioo\OneDrive\Bureau\TIPE\CelesteSaving\est.txt", text);
            //outTextures[underXIndex, underYIndex] = GetTile(tileUnder); // fail safe?
            return outTextures;
        }
        public static int[,] GetVisionInt()
        {
            int visionX = 5;
            int visionY = 5;

            int underYIndex = visionY / 2 + 1;
            int underXIndex = visionX / 2;
            Vector2 tileUnder = GetTileUnderPlayer();
            int[,] outInts = new int[visionX, visionY];
            for (int i = 0; i < visionY; i++)
            {
                for (int j = 0; j < visionX; j++)
                {
                    int temp = IsWall(new Vector2(tileUnder.X - underXIndex + j, tileUnder.Y - underYIndex + i)) ? 1 : 0;
                    if (temp == 0)
                    {
                        temp = GetEntityFromXYCenter(GetXYCenterFromTile(new Vector2(tileUnder.X - underXIndex + j, tileUnder.Y - underYIndex + i))) != null ? 1 : 0;
                    }
                    outInts[i, j] = temp;
                }
            }
            string text = new Vector2(tileUnder.X - underXIndex, tileUnder.Y - underYIndex).ToString();
            text += "\n" + new Vector2(tileUnder.X - underXIndex + visionX - 1, tileUnder.Y - underYIndex + visionY - 1);
            System.IO.File.WriteAllText(@"C:\Users\elioo\OneDrive\Bureau\TIPE\CelesteSaving\test.txt", text);
            return outInts;
        }
        public static void WriteTexturesToFile(string file, MTexture[,] textures)
        {
            string text = "";
            for (int i = 0; i < textures.GetLength(0); i++)
            {
                for (int j = 0; j < textures.GetLength(1); j++)
                {
                    if (textures[i, j] != null)
                    {
                        text += textures[i, j].ToString() + "\t";
                    }
                    else
                    {
                        text += "tilesets/air\t";
                    }
                }
                text += "\n";
            }
            System.IO.File.WriteAllText(file, text);
        }
        public static void WriteIntsToFile(string file, int[,] ints)
        {
            string text = "";
            for (int i = 0; i < ints.GetLength(0); i++)
            {
                for (int j = 0; j < ints.GetLength(1); j++)
                {
                    text += ints[i, j] + "\t";
                }
                text += "\n";
            }
            System.IO.File.WriteAllText(file, text);
        }
        private static GamePadState GetGamePadState()
        {
            GamePadState currentState = MInput.GamePads[0].CurrentState;
            for (int i = 0; i < 4; i++)
            {
                currentState = GamePad.GetState((PlayerIndex)i);
                if (currentState.IsConnected)
                {
                    break;
                }
            }
            return currentState;
        }
        public static GamePadState CalculateInputs()
        {
            kbState = Keyboard.GetState();
            // THIS MUST BE FIXED!
            GamePadDPad pad = new GamePadDPad(
                ButtonState.Released,
                ButtonState.Released,
                IsKeyDown(Keys.OemQuotes) ? ButtonState.Pressed : ButtonState.Released,
                IsKeyDown(Keys.OemBackslash) ? ButtonState.Pressed : ButtonState.Released);
            GamePadThumbSticks sticks = new GamePadThumbSticks(new Vector2(0, 0), new Vector2(0, 0));
            GamePadState padState = new GamePadState(
                sticks,
                new GamePadTriggers(0, 0),
                new GamePadButtons(
                    IsWall(GetTileInFrontOfPlayer()) ? Buttons.A : (Buttons)0
                    //Buttons.A
                    | (Buttons)0
                    | (Buttons)0
                    | (Buttons)0
                    | (Buttons)0
                    | (Buttons)0
                    | (Buttons)0
                ),
                pad
            );
            return padState;
        }

        public static void ResetActiveText(string reset)
        {
            activeText = reset;
        }
    }
}