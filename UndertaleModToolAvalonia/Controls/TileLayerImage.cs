using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using static UndertaleModLib.Models.UndertaleRoom;

namespace UndertaleModToolAvalonia
{
    // source - https://stackoverflow.com/a/4801434/12136394
    public class TileLayerImage : Image
    {
        private static readonly AvaloniaProperty LayerTilesDataProperty =
            AvaloniaProperty.Register<>("LayerTilesData", typeof(Layer.LayerTilesData),
                typeof(TileLayerImage),
                new FrameworkPropertyMetadata(null));
        private static readonly AvaloniaProperty CheckTransparencyProperty =
            AvaloniaProperty.Register<>("CheckTransparency", typeof(bool),
                typeof(TileLayerImage),
                new FrameworkPropertyMetadata(false));

        public Layer.LayerTilesData LayerTilesData
        {
            get => (Layer.LayerTilesData)GetValue(LayerTilesDataProperty);
            set => SetValue(LayerTilesDataProperty, value);
        }
        public bool CheckTransparency
        {
            get => (bool)GetValue(CheckTransparencyProperty);
            set => SetValue(CheckTransparencyProperty, value);
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            Bitmap source = (Bitmap)Source;

            // Get the pixel of the source that was hit
            int x = (int)(hitTestParameters.HitPoint.X / Width * source.PixelSize.Width);
            int y = (int)(hitTestParameters.HitPoint.Y / Height * source.PixelSize.Height);

            if (CheckTransparency)
            {
                // Copy the single pixel into a new byte array representing RGBA
                byte[] pixel = new byte[4];
                source.CopyPixels(new Int32Rect(x, y, 1, 1), pixel, 4, 0);

                // Check the alpha (transparency) of the pixel
                if (pixel[3] == 0)
                    return null;
            }
            else
            {
                int x1 = x / (int)LayerTilesData.Background.GMS2TileWidth;
                int y1 = y / (int)LayerTilesData.Background.GMS2TileHeight;

                if (x1 < 0 || x1 > LayerTilesData.TilesX - 1 ||
                    y1 < 0 || y1 > LayerTilesData.TilesY - 1 ||
                    LayerTilesData.TileData[y1][x1] == 0)
                    return null;
            }

            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }
    }
}
