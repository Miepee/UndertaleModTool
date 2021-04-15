using Eto.Forms;
using System;

namespace UndertaleModToolEto.Mac
{
    class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
            new Application(Eto.Platforms.Mac64).Run(new MainWindow());
        }
    }
}
