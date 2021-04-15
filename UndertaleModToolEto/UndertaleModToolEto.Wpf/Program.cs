using Eto.Forms;
using System;

namespace UndertaleModToolEto.Wpf
{

    class MainClass
    {
        [STAThread]
        public static void Main(string[] args)
        {
                new Application(Eto.Platforms.WinForms).Run(new MainWindow()); 
        }

    }
}
