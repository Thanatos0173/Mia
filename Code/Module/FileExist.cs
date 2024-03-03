using System;
using System.Windows.Forms;

namespace Celeste.Mod.Mia.Code.Exceptions
{
    public class FileExist : Exception
    {
        public FileExist() { }

        public FileExist(string message) : base(message) {  }
        public FileExist(string message, Exception inner) : base(message, inner) { }

    }
}
