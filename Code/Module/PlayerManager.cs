using Monocle;
using System;
using Microsoft.Xna.Framework;

using Celeste.Mod.Mia.UtilsClass;


using System.Diagnostics;

namespace Celeste.Mod.Mia.PlayerManager
{
    public class PlayerManager
    {
        public static void ManagePlayer(Stopwatch stopwatch, Player player, Level level, Vector2 previousPosition, bool onVoidLevel)
        {
            if (stopwatch.Elapsed >= TimeSpan.FromSeconds(Mia.Main.Settings.IdleTime))
            {
                if (Mia.Main.Settings.KillPlayer) player.Die(player.Position);
                if (Mia.Main.Settings.Debug && Mia.Main.Settings.KillPlayer) Utils.print("Player was idle too long : killed");
            }

            if (player.Position != previousPosition)
            {
                if (Mia.Main.Settings.KillPlayer) stopwatch.Restart(); //Utils.putToFile(level);
               
            }
            if (level.InCutscene)
            {
                level.SkipCutscene();
                if (Mia.Main.Settings.Debug) Utils.print("Skipping cutscene :", Engine.Scene?.GetType().FullName.ToString());

            }
            if (level.Session.LevelData.Name.ToString() == "void" && !onVoidLevel)
            {
                if (Mia.Main.Settings.KillPlayer) stopwatch.Stop();
                onVoidLevel = true;
                if (Mia.Main.Settings.Debug && Mia.Main.Settings.KillPlayer) Utils.print("Stopping stopwatch : currently in \"void\" level");
            }
            if (level.Session.LevelData.Name.ToString() != "void" && !stopwatch.IsRunning && stopwatch != null && onVoidLevel)
            {
                onVoidLevel = false;
                if (Mia.Main.Settings.KillPlayer) stopwatch.Restart();
                if (Mia.Main.Settings.Debug && Mia.Main.Settings.KillPlayer) Utils.print("Restarting stopwatch : exiting \"void\" level");

            }
        }
    }
}
