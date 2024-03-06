using System;
using Monocle;
using Celeste.Mod.Mia.Plage;
using Celeste.Mod.Mia.EntityExtension;
using Celeste.Mod.Mia.Actions;
using System.Security.Policy;

namespace Celeste.Mod.Mia.TileManager
{
    public class TileManager
    {
        public static int[,] GetEntityAroundPlayerAsTiles(Level level, Player player)
        {
            int[,] tilesAroundPlayer = new int[20, 20];
            for (int i = 0; i < level.Entities.Count; i++)
            {

                Entity entity = level.Entities[i];
                if (entity.IsAroundPosition(player.Position) && (entity.HaveComponent("Celeste.PlayerCollider") || entity.HaveComponent("Monocle.Image") || entity.HaveComponent("Celeste.LightOcclude") || entity is Solid))
                {
                    int entityXTiled = 10 + (int)entity.X / 8 - (int)player.Position.X / 8;
                    int entityYTiled = 10 + (int)entity.Y / 8 - (int)player.Position.Y / 8;
                    int entityTileHeight = (int)entity.Height / 8;
                    int entityTileWidth = (int)entity.Width / 8;
                    int UUID = EntitiesActions.Actions(level, entity);
                    tilesAroundPlayer.FillPlage(entityXTiled, entityYTiled, entityTileWidth, entityTileHeight, UUID);
                }
            }
            return tilesAroundPlayer;
        }
        public static int[,] GetTilesAroundPlayer(Level level, char[,] array, Player player)
        {
            int[,] tilesAroundPlayer = new int[20, 20];
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
                        if (array[i, j] != '0') tilesAroundPlayer[incrI, incrJ] = 1; 
                        else tilesAroundPlayer[incrI, incrJ] = 0;
                    }
                    catch (IndexOutOfRangeException) { tilesAroundPlayer[incrI, incrJ] = 1; }
                }
            }
            return tilesAroundPlayer;
        }
        public static bool isAboveVoid(Player entity,Level level)
        {
            LevelData room = level.Session.LevelData;
            int yMinPosition = room.Bounds.Bottom;
            for (int i = (int)entity.Position.Y; i != yMinPosition; i+= ((int)entity.Position.Y < yMinPosition) ? 1 : -1)
            {
                if (level.CollideCheck(new Microsoft.Xna.Framework.Vector2(entity.Position.X, i), 5)) { return false && entity.Position != entity.PreviousPosition;  } //If we are not moving, then either we are flying, nor we are not falling but the code say we are due to impretisions.
            }   
            return true;

        }
        public static int[,] FusedArrays(Level level, char[,] array, Player player)
        {
            int[,] entityArray = GetEntityAroundPlayerAsTiles(level, player);
            int[,] tilesArray = GetTilesAroundPlayer(level, array, player);

            int[,] globalTiles = new int[20, 20];
            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    
                    if (entityArray[j, i] == 0) //There is no entity in that tile
                    {
                        globalTiles[j, i] = tilesArray[i, j];
                    }
                    else globalTiles[j, i] = entityArray[i, j];
                }
            }
            return globalTiles;

        }
    }
}