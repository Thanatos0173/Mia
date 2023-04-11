using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Hosting;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Mod.Mia.UtilsClass
{
    public class Utils
    {
        public static void print(params string[] arguments)
        {
            if (!arguments.Any())
            {
                throw new System.Exception("You need to provide at least 1 argument");
            }
            String text = "";
            foreach (string arg in arguments)
            {
                text += arg;
                text += " ";
            }
            Logger.Log("Tipe Mod", text);
        }
    }
}
