using Monocle;
using System;
using Microsoft.Xna.Framework;

using Celeste.Mod.Mia.UtilsClass;
using Celeste.Mod.Mia.Settings;



using System.Diagnostics;

namespace Celeste.Mod.Mia
{
    
    public class Main : EverestModule
    {
        public static Main Instance;



        public override Type SettingsType => typeof(SettingsClass);
        public static SettingsClass Settings => (SettingsClass)Instance._Settings;
        public Main()
        {
            Instance = this;
        }

        private static int index = 0;
        
        private Stopwatch stopwatch;
        private static bool walkLR = false;
        private static bool walkUD = false;
        private static Vector2 direction = new Vector2(1, 0);
        private static bool grabbing { get; set; } = false;
        private static bool dash { get; set; }  = false;

        private static float baseTimer;


        
        [Command("left", "Move left")]
        public static void LeftCommand()
        {
            walkLR = !walkLR;
            if (walkLR) direction.X = -1;
            else direction.X = 0;
            string text = walkLR ? "Starting" : "Stopping"; 
            Engine.Commands.Log(text + " left movement...",   Color.GreenYellow);
        }

        [Command("right", "Move right")]
        public static void RightCommand()
        {
            Player player = Engine.Scene.Tracker.GetEntity<Player>();
            walkLR = !walkLR;
            if (walkLR) direction.X = 1;
            else direction.X = 0;
            string text = walkLR ? "Starting" : "Stopping";
            Engine.Commands.Log(text + " right movement...", Color.GreenYellow);
        }
        [Command("state", "Move right")]
        public static void StateMachineCommand()
        {
            index++;
            Engine.Commands.Log("Change to index "+index.ToString(), Color.GreenYellow);
        }
        [Command("up", "Move up")]
        public static void UpCommand()
        {
            walkUD = !walkUD;
            if (walkUD) direction.Y = -1;
            else direction.Y = 0;
            string text = walkLR ? "Starting" : "Stopping";
            Engine.Commands.Log(text + " up movement...", Color.GreenYellow);
        }

        [Command("down", "Move down")]
        public static void DownCommand()
        {
            walkUD = !walkUD;
            if (walkUD) direction.Y = 1;
            else direction.Y = 0;
            string text = walkLR ? "Starting" : "Stopping";
            Engine.Commands.Log(text + " down movement...", Color.GreenYellow);

        }
        [Command("dash", "Dash")]
        public static void DashCommand()
        {
            Engine.Commands.Log("Dashing...", Color.GreenYellow);
            dash = true;
            
        }
        [Command("grab", "Start/Stop grabbing. Optionnal argument : if true then the player start grabbing, if false the player end grabbing. If nothing, the player shift between start and stop")]
        public static void GrabCommand(string force)
        {
            try
            {
                grabbing = bool.Parse(force);
            }
            catch(ArgumentNullException)
            {
                grabbing = !grabbing ;
            }
            Engine.Commands.Log((grabbing ? "Started" : "Stopping" )+ " Grabbing...", Color.GreenYellow);
        }

        public override void Load()
        {
            Utils.print("TIPE Mod Loaded");
            On.Celeste.Player.Update += ModPlayerUpdate;
            Everest.Events.Player.OnSpawn += onSpawn;
            Everest.Events.Level.OnLoadLevel += loadLevel;
            stopwatch = new Stopwatch();
        }


        

   
        private void loadLevel(Level level, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            if (isFromLoader && !Settings.GetTiles) Utils.print("Tiles won't be retrived. Change the option in the settings to make able to retrieve level, Mia will not work otherwise.");
            
        }

        public override void Unload()
        {
            Utils.print("TIPE Mod unloaded");
            On.Celeste.Player.Update -= ModPlayerUpdate;
            Everest.Events.Player.OnSpawn -= onSpawn;
            Everest.Events.Level.OnLoadLevel -= loadLevel;
            
        }

        
        private void onSpawn(Player player)
        {
            if (Main.Settings.KillPlayer) stopwatch.Restart();
            if (Main.Settings.Debug && Main.Settings.KillPlayer) Utils.print("Starting stopwatch");
            if(baseTimer == null)
            {
                Utils.print("BOUH LE NULL");
                baseTimer = player.AutoJumpTimer;
            }
        }
        bool onVoidLevel = false;



        private bool tileAtPosition(Level level, Vector2 position)
        {
            Vector2 tilePosition = new Vector2((int)position.X / 8, (int)position.Y / 8 - 1);
            Utils.print(Math.Abs(level.Session.LevelData.TileBounds.X - (int)tilePosition.X),Math.Abs(level.Session.LevelData.TileBounds.Y - (int)tilePosition.Y));
            char[,] map = level.SolidsData.ToArray();
            int positionXInArray = Math.Abs(level.TileBounds.X - (int)tilePosition.X);
            int positionYInArray = Math.Abs(level.TileBounds.Y - (int)tilePosition.Y);
            return map[positionXInArray,positionYInArray] != '0';
            
        }

        /*private void touchWall(Player player,Level level)
        {

            char[,] map = level.SolidsData.ToArray();
            Vector2 tilePositionDown = new Vector2((int)player.Position.X/8, (int)player.Position.Y/8);
            Vector2 tilePositionUp = new Vector2((int)player.Position.X / 8, ((int)player.Position.Y - (int)player.Height)/8);
            int playerX = Math.Abs(level.TileBounds.X - (int)tilePositionDown.X);
            int playerY = Math.Abs(level.TileBounds.Y - (int)tilePositionDown.X);

            for (int j = playerY - 10; j < playerY + 10; j++)
            {
                for (int i = playerX - 10; i < playerX + 10; i++)
                {
                    int incrI = i - (playerX - 10);
                    int incrJ = j - (playerY - 10);
                    try
                    {
                        if (array[i, j] != '0') tilesAroundPlayer[incrI, incrJ] = 128; //Due to binary representation : Something is 00000000, nothing is 11111111, and there is 126 entities that can be stored. For now, I think it's more than enough.
                        else tilesAroundPlayer[incrI, incrJ] = 0;
                    }
                    catch (IndexOutOfRangeException) { tilesAroundPlayer[incrI, incrJ] = 0; }
                }
            }
            return tilesAroundPlayer;

            Utils.print(tilePositionDown, tilePositionUp);
        }*/
        int y = 0;
        

        public bool playerBetweenEntityPositionLeft(Vector2 positionToCheck,  Vector2 position1,Vector2 position2) { //position1.X should be equal to position2.X
            return ((int)positionToCheck.X - 4 == (int)position1.X && (int)positionToCheck.Y > (int)position1.Y && (int)positionToCheck.Y < (int)position2.Y);
        }

        public bool playerBetweenEntityPositionRight(Vector2 positionToCheck, Vector2 position1, Vector2 position2)
        { //position1.X should be equal to position2.X
            //Utils.print((int)positionToCheck.X + 4 == (int)position1.X);
            return ((int)positionToCheck.X + 4 == (int)position1.X && (int)positionToCheck.Y > (int)position1.Y && (int)positionToCheck.Y < (int)position2.Y);
        }

        public bool canPlayerGrab(Player player,Level level)
        {
            for (int i = 0; i < level.Entities.Count; i++)
            {
                Entity entity = level.Entities[i];
                float positionX = entity.Position.X;
                float positionY = entity.Position.Y;

                if (level.Entities[i].HaveComponent("Celeste.LightOcclude")) return
                        playerBetweenEntityPositionLeft(player.Position, new Vector2(positionX + entity.Width, positionY), new Vector2(positionX + entity.Width, positionY + entity.Height)) ||
                        playerBetweenEntityPositionLeft(new Vector2(player.Position.X,player.Position.Y - (player.Height/2)), new Vector2(positionX + entity.Width, positionY), new Vector2(positionX + entity.Width, positionY + entity.Height)) ||
                        playerBetweenEntityPositionLeft(new Vector2(player.Position.X, player.Position.Y - player.Height), new Vector2(positionX + entity.Width, positionY), new Vector2(positionX + entity.Width, positionY + entity.Height)) ||
                        playerBetweenEntityPositionRight(player.Position, new Vector2(positionX , positionY), new Vector2(positionX + entity.Width, positionY + entity.Height)) ||
                        playerBetweenEntityPositionRight(new Vector2(player.Position.X, player.Position.Y - (player.Height / 2)), new Vector2(positionX , positionY), new Vector2(positionX + entity.Width, positionY + entity.Height)) ||
                        playerBetweenEntityPositionRight(new Vector2(player.Position.X, player.Position.Y - player.Height), new Vector2(positionX, positionY), new Vector2(positionX + entity.Width, positionY + entity.Height))






                        ;
            }
            return false;
        }
        private void ModPlayerUpdate(On.Celeste.Player.orig_Update orig, Player self)
        {
            float previousStamina = self.Stamina;
            Vector2 previousPosition = self.Position;

            //self.StateMachine.State = index;
            
            orig(self);
            if (Engine.Scene is Level level) 
            {
                for (int i = 0; i < level.Entities.Count; i++)
                {
                    Entity entity = level.Entities[i];
                    float positionX = entity.Position.X;
                    float positionY = entity.Position.Y;

                    if (level.Entities[i].HaveComponent("Celeste.LightOcclude")) Utils.print(canPlayerGrab(self,level), new Vector2(level.Entities[i].Position.X, level.Entities[i].Position.Y), new Vector2(level.Entities[i].Position.X, level.Entities[i].Position.Y + level.Entities[i].Height), self.Position, new Vector2(self.Position.X, self.Position.Y - (self.Height / 2)), new Vector2(self.Position.X, self.Position.Y - self.Height));
                }

                //Utils.print(new Vector2(self.Position.X - 4, self.Position.Y - (int)(self.Height/2)),self.Position,tileAtPosition(level,new Vector2(self.Position.X - 8, self.Position.Y)));



                if (Settings.GetTiles)
                {
                    PlayerManager.PlayerManager.ManagePlayer(stopwatch, self, level, previousPosition, onVoidLevel);
                    TileManager.TileManager.getEntityAroundPlayerAsTiles(level, self);
                    TileManager.TileManager.getTilesAroundPlayer(level, level.SolidsData.ToArray(), self);
                }
                if (walkLR) self.MoveH(direction.X);
                if (walkUD) self.MoveV(direction.Y);
               
                if (dash && self.Dashes > 0)
                {
                    self.StateMachine.State = 2;
                    self.Dashes--;

                }
                if (self.OnGround() && self.StateMachine.State == 2) dash = false;
                if(self.StateMachine.State != 2 && dash == true) dash = false;

                if (self.Stamina <= 0 && grabbing) grabbing = false; 
                if (self.StateMachine.State != 1 && grabbing) grabbing = false;
                
            }
        }
    }
}