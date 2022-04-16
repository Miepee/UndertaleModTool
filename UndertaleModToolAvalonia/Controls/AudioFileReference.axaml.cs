using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using UndertaleModLib;
using UndertaleModLib.Models;

namespace UndertaleModToolAvalonia
{
    /// <summary>
    /// Logika interakcji dla klasy UndertaleObjectReference.xaml
    /// </summary>
    public partial class AudioFileReference : UserControl
    {
        public static readonly AvaloniaProperty AudioReferenceProperty =
            AvaloniaProperty.Register<>("AudioReference", typeof(UndertaleEmbeddedAudio),
                typeof(AudioFileReference),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly AvaloniaProperty GroupReferenceProperty =
            AvaloniaProperty.Register<>("GroupReference", typeof(UndertaleAudioGroup),
                typeof(AudioFileReference),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public static readonly AvaloniaProperty AudioIDReference =
            AvaloniaProperty.Register<>("AudioID", typeof(int),
                typeof(AudioFileReference));

        public static readonly AvaloniaProperty GroupIDProperty =
            AvaloniaProperty.Register<>("GroupID", typeof(int),
                typeof(AudioFileReference));

        public UndertaleEmbeddedAudio AudioReference
        {
            get { return (UndertaleEmbeddedAudio)GetValue(AudioReferenceProperty); }
            set { SetValue(AudioReferenceProperty, value); }
        }

        public UndertaleAudioGroup GroupReference
        {
            get { return (UndertaleAudioGroup)GetValue(GroupReferenceProperty); }
            set { SetValue(GroupReferenceProperty, value); }
        }

        public int AudioID
        {
            get { return (int)GetValue(AudioIDReference); }
            set { SetValue(AudioIDReference, value); }
        }

        public int GroupID
        {
            get { return (int)GetValue(GroupIDProperty); }
            set { SetValue(GroupIDProperty, value); }
        }

        public AudioFileReference()
        {
            InitializeIfNeeded();
            //InitializeComponent();
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            OpenReference();
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            AudioReference = null;
        }

        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenReference();
        }

        private void OpenReference()
        {
            if (GroupID != 0 && AudioID != -1)
            {
                (Application.Current.MainWindow as MainWindow).OpenChildFile("audiogroup" + GroupID + ".dat", "AUDO", AudioID);
                return;
            }

            if (AudioReference == null)
                return;

            (Application.Current.MainWindow as MainWindow).ChangeSelection(AudioReference);
        }

        private void TextBox_DragOver(object sender, DragEventArgs e)
        {
            UndertaleObject sourceItem = e.Data.GetData(e.Data.GetFormats()[0]) as UndertaleObject;

            e.Effects = GroupID == 0 && e.AllowedEffects.HasFlag(DragDropEffects.Link) && sourceItem != null && sourceItem.GetType() == typeof(UndertaleEmbeddedAudio) ? DragDropEffects.Link : DragDropEffects.None;
            e.Handled = true;
        }

        private void TextBox_Drop(object sender, DragEventArgs e)
        {
            UndertaleObject sourceItem = e.Data.GetData(e.Data.GetFormats()[0]) as UndertaleObject;

            e.Effects = GroupID == 0 && e.AllowedEffects.HasFlag(DragDropEffects.Link) && sourceItem != null && sourceItem.GetType() == typeof(UndertaleEmbeddedAudio) ? DragDropEffects.Link : DragDropEffects.None;
            if (e.Effects == DragDropEffects.Link)
            {
                AudioReference = (UndertaleEmbeddedAudio)sourceItem;
            }
            e.Handled = true;
        }
    }
}
