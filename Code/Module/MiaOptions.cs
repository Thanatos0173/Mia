using Celeste.Mod.UI;

namespace Celeste.Mod.Mia.MiaOptions
{
    class OuiExampleSubmenu : OuiGenericMenu, OuiModOptions.ISubmenu
    {
        public override string MenuName => "Mia settings";

        private string IntString(int value)
        {
            if (value % 60 == 0) return value / 60 + " minutes";
            else return value / 60 + " minutes  and " + value % 60 + " seconds";
        }
        protected override void addOptionsToMenu(TextMenu menu)
        {
            menu.Add(new TextMenu.Slider("Idle Time", IntString, 5, 300, Mia.Main.Settings.IdleTime)
                 .Change(newValue => Mia.Main.Settings.IdleTime = newValue)) ;

            menu.Add(new TextMenu.OnOff("Kill player", Mia.Main.Settings.KillPlayer)
                .Change(newValue => Mia.Main.Settings.KillPlayer = newValue));
        }
    }
}


//switch (action.ToLower())
//{
//    case "jump":
//        Console.WriteLine("Jumpingn...");
//        player.Jump();
//        break;
//    case "dash":
//        Console.WriteLine("Dashing...");
//        player.StartDash();
//        break;
//    case "grab":
//        Console.WriteLine("Grabbing...");
//        player.Holding.Pickup(player);
//        break;
//    case "left":
//        Console.WriteLine("Moving left...");
//        player.MoveH(-1);
//        break;
//    case "right":
//        Console.WriteLine("Moving right...");
//        player.MoveH(1);
//        break;
//    case "up":
//        Console.WriteLine("Moving up...");
//        player.MoveV(-1);
//        break;
//    case "down":
//        Console.WriteLine("Moving down...");
//        player.MoveV(1);
//        break;
//    default:
//        Console.WriteLine("Unknown action.");
//        break;
//}