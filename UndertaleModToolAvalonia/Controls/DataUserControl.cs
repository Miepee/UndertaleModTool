using Avalonia;
using Avalonia.Controls;

namespace UndertaleModToolAvalonia
{
    public partial class DataUserControl : UserControl
    {
        public DataUserControl()
        {
        }

        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> e)
        {
            // prevent Avalonia binding errors (and unnecessary "DataContextChanged" firing) when switching to incompatible data type
            //if (e.NewValue is null && e.Property == DataContextProperty)
            // e.newvalue is non-nullable

            base.OnPropertyChanged(e);
        }

    }
}
