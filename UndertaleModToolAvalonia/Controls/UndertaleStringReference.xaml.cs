using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using UndertaleModLib.Models;
using UndertaleModToolAvalonia;

namespace UndertaleModToolAvalonia
{
    /// <summary>
    /// Logika interakcji dla klasy UndertaleStringReference.xaml
    /// </summary>
    public partial class UndertaleStringReference : UserControl
    {
        public static AvaloniaProperty ObjectReferenceProperty =
            AvaloniaProperty.Register<>("ObjectReference", typeof(UndertaleString),
                typeof(UndertaleStringReference),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public UndertaleString ObjectReference
        {
            get { return (UndertaleString)GetValue(ObjectReferenceProperty); }
            set { SetValue(ObjectReferenceProperty, value); }
        }

        public UndertaleStringReference()
        {
            InitializeIfNeeded();
            //InitializeComponent();
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            (((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow as MainWindow).ChangeSelection(ObjectReference);
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            ObjectReference = null;
        }

        private void TextBox_DragOver(object sender, DragEventArgs e)
        {
            UndertaleString sourceItem = e.Data.GetData(e.Data.GetFormats()[0]) as UndertaleString;

            e.Effects = e.AllowedEffects.HasFlag(DragDropEffects.Link) && sourceItem != null ? DragDropEffects.Link : DragDropEffects.None;
            e.Handled = true;
        }

        private void TextBox_Drop(object sender, DragEventArgs e)
        {
            UndertaleString sourceItem = e.Data.GetData(e.Data.GetFormats()[0]) as UndertaleString;

            e.Effects = e.AllowedEffects.HasFlag(DragDropEffects.Link) && sourceItem != null ? DragDropEffects.Link : DragDropEffects.None;
            if (e.Effects == DragDropEffects.Link)
            {
                ObjectReference = sourceItem;
            }
            e.Handled = true;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            var binding = BindingOperations.GetBindingExpression(tb, TextBox.TextProperty);
            if (binding.IsDirty)
            {
                if (ObjectReference != null)
                {
                    StringUpdateWindow dialog = new StringUpdateWindow();
                    dialog.Owner = Window.GetWindow(this);
                    dialog.ShowDialog();
                    switch (dialog.Result)
                    {
                        case StringUpdateWindow.ResultType.ChangeOneValue:
                            ObjectReference = (Application.Current.MainWindow as MainWindow).Data.Strings.MakeString(tb.Text);
                            break;
                        case StringUpdateWindow.ResultType.ChangeReferencedValue:
                            binding.UpdateSource();
                            break;
                        case StringUpdateWindow.ResultType.Cancel:
                            binding.UpdateTarget();
                            break;
                    }
                }
                else
                {
                    ObjectReference = (((IClassicDesktopStyleApplicationLifetime)Application.Current.ApplicationLifetime).MainWindow as MainWindow).Data.Strings.MakeString(tb.Text);
                }
            }
        }
    }
}
