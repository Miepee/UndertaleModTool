using System.ComponentModel;
using UndertaleModLib.Scripting;
using Avalonia.Controls;

namespace UndertaleModToolAvalonia
{
    // Test code here
    public partial class MainWindow : Window, INotifyPropertyChanged, IScriptInterface
    {
        public bool DummyBool()
        {
            return true;
        }

        public void DummyVoid()
        {
        }
        public string DummyString()
        {
            return "";
        }
    }
}
