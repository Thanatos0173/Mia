using System;
using Monocle;
using Celeste.Mod.Mia.Plage;
using Celeste.Mod.Mia.EntityExtension;

namespace Celeste.Mod.Mia.TileManager
{
    public class TileManager
    {
        public static int[,] getEntityAroundPlayerAsTiles(Level level, Player player)
        {
            int[,] tilesAroundPlayer = new int[20, 20];
            for (int i = 0; i < level.Entities.Count; i++)
            {

                Entity entity = level.Entities[i];
                if (entity.IsAroundPosition(player.Position) && (entity.HaveComponent("Celeste.PlayerCollider") || entity.HaveComponent("Monocle.Image") || entity.HaveComponent("Celeste.LightOcclude")))
                {
                    int entityXTiled = 10 + (int)entity.X / 8 - (int)player.Position.X / 8;
                    int entityYTiled = 10 + (int)entity.Y / 8 - (int)player.Position.Y / 8;
                    int entityTileHeight = (int)entity.Height / 8;
                    int entityTileWidth = (int)entity.Width / 8;
                    int UUID = entity.UUID();
                    tilesAroundPlayer.FillPlage(entityXTiled, entityYTiled, entityTileWidth, entityTileHeight, UUID);
                    }
            }
            return tilesAroundPlayer;
        }
        public static int[,] getTilesAroundPlayer(Level level, char[,] array, Player player)
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
                        if (array[i, j] != '0') tilesAroundPlayer[incrI, incrJ] = 127; //Due to binary representation : Something is 0000000, nothing is 1111111, and there is 125 entities that can be stored. For now, I think it's more than enough.
                        else tilesAroundPlayer[incrI, incrJ] = 0;
                    }
                    catch (IndexOutOfRangeException) { tilesAroundPlayer[incrI, incrJ] = 0; }
                }
            }
            return tilesAroundPlayer;
        }
    }
}