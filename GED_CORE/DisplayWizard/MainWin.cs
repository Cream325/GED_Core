using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using GED.Core.Ctrl;
using GED.Core.SanityCheck;

namespace GED.Core.DisplayWizard
{
    public partial class MainWin : Window
    {
        #region Member Fields
        internal uint ZeroPosX, ZeroPosY;
        internal WriteableBitmap __DisplayBuffer;
        #endregion

        #region Properties
        public WriteableBitmap DisplayBuffer { get => __DisplayBuffer; }

        public Image? Buffer;
        #endregion

        unsafe public MainWin(out int err, int VisualWidth, int VisualHeight)
        {
            err = FuckedNumbers.OK;
            if (VisualWidth == 0 || VisualHeight == 0)
            {
                err = FuckedNumbers.WRONG_OPERATION;
                return;
            }

            __DisplayBuffer = new WriteableBitmap(new PixelSize(VisualWidth, VisualHeight), new Vector(96, 96), format: PixelFormats.Bgr24);

            try
            {
                PointerMoved += OnPointerMoved;
                SizeChanged += OnSizeChanged;
                AvaloniaXamlLoader.Load(this);
            }
            catch
            {
                err = FuckedNumbers.ALLOC_FAILED;
                return;
            }
        }

        #region Private Functions
        private unsafe void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            var pos = e.GetPosition(this);
            fMousePoint.X[0] = pos.X;
            fMousePoint.Y[0] = pos.Y;
        }
        
        private void OnSizeChanged(object? sender, SizeChangedEventArgs e)
        {
            if(Buffer != null) {
                Buffer.Source = __DisplayBuffer;
            }
        }
        #endregion
    }
}