using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TaleKitEditor.UI.Controls {
    public class NinePatchPanel : ContentControl {
        ImageSource[] patchs;

        [Category("Background")]
        [Description("Set nine patch background image")]
        public ImageSource BackgroundImage {
            get { return (ImageSource)GetValue(BackgroundImageProperty); }
            set { SetValue(BackgroundImageProperty, value); }
        }
        public static readonly DependencyProperty BackgroundImageProperty =
            DependencyProperty.Register("BackgroundImage", typeof(ImageSource), typeof(NinePatchPanel), new PropertyMetadata(null, SetImage));

        [Category("Background")]
        [Description("Set the center split area")]
        public Thickness SideAspect {
            get { return (Thickness)GetValue(CenterAreaProperty); }
            set { SetValue(CenterAreaProperty, value); }
        }
        public static readonly DependencyProperty CenterAreaProperty =
            DependencyProperty.Register("CenterArea", typeof(Thickness), typeof(NinePatchPanel), new PropertyMetadata(new Thickness(0.3d, 0.3d, 0.3d, 0.3d), SetArea));

        // [ Constructor ]
        public NinePatchPanel() : base() {
        }

        // [ Event ]
        protected override void OnRender(DrawingContext drawingContext) {
            if (patchs != null) {
                double x1 = patchs[0].Width, x2 = Math.Max(ActualWidth - patchs[2].Width, 0);
                double y1 = patchs[0].Height, y2 = Math.Max(ActualHeight - patchs[6].Height, 0);
                double w1 = patchs[0].Width, w2 = Math.Max(x2 - x1, 0), w3 = patchs[2].Width;
                double h1 = patchs[0].Height, h2 = Math.Max(y2 - y1, 0), h3 = patchs[6].Height;
                var rects = new[]
                {
                    new Rect(0, 0, w1, h1),
                    new Rect(x1, 0, w2, h1),
                    new Rect(x2, 0, w3, h1),
                    new Rect(0, y1, w1, h2),
                    new Rect(x1, y1, w2, h2),
                    new Rect(x2, y1, w3, h2),
                    new Rect(0, y2, w1, h3),
                    new Rect(x1, y2, w2, h3),
                    new Rect(x2, y2, w3, h3)
                };
                for (int i = 0; i < 9; i++) {
                    drawingContext.DrawImage(patchs[i], rects[i]);
                }
            }
            base.OnRender(drawingContext);
        }

        private static void SetArea(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e) {
            var np = dependencyObject as NinePatchPanel;
            if (np == null) return;
            var bm = np.BackgroundImage as BitmapSource;
            if (bm != null) SetPatchs(np, bm);
        }
        private static void SetImage(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e) {
            var nimePatch = dependencyObject as NinePatchPanel;
            if (nimePatch == null) return;
            var bitmap = nimePatch.BackgroundImage as BitmapSource;

            SetPatchs(nimePatch, bitmap);
        }

        private static void SetPatchs(NinePatchPanel ninePatch, BitmapSource bitmap) {
            int x1 = (int)(ninePatch.SideAspect.Left * bitmap.PixelWidth);
            int x2 = (int)((1d - ninePatch.SideAspect.Right) * bitmap.PixelWidth);

            int y1 = (int)(ninePatch.SideAspect.Top * bitmap.PixelHeight);
            int y2 = (int)((1d - ninePatch.SideAspect.Bottom) * bitmap.PixelHeight);

            int w1 = (int)x1;
            int w2 = (int)((1d - ninePatch.SideAspect.Right - ninePatch.SideAspect.Left) * bitmap.PixelWidth);
            int w3 = (int)(ninePatch.SideAspect.Right * bitmap.PixelWidth);
            
            int h1 = (int)y1;
            int h2 = (int)((1d - ninePatch.SideAspect.Top - ninePatch.SideAspect.Bottom) * bitmap.PixelHeight);
            int h3 = (int)(ninePatch.SideAspect.Bottom * bitmap.PixelHeight);

            ninePatch.patchs = new[] {
                new CroppedBitmap(bitmap, new Int32Rect(0, 0, w1, h1)),
                new CroppedBitmap(bitmap, new Int32Rect(x1, 0, w2, h1)),
                new CroppedBitmap(bitmap, new Int32Rect(x2, 0, w3, h1)),
                new CroppedBitmap(bitmap, new Int32Rect(0, y1, w1, h2)),
                new CroppedBitmap(bitmap, new Int32Rect(x1, y1, w2, h2)),
                new CroppedBitmap(bitmap, new Int32Rect(x2, y1, w3, h2)),
                new CroppedBitmap(bitmap, new Int32Rect(0, y2, w1, h3)),
                new CroppedBitmap(bitmap, new Int32Rect(x1, y2, w2, h3)),
                new CroppedBitmap(bitmap, new Int32Rect(x2, y2, w3, h3))
            };
        }
    }
}
