using System;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.Threading;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media;
using Toast;

namespace ACFUN
{
    public partial class ImageView : PhoneApplicationPage
    {
        string url;
        byte[] bytes;

        private const string CacheDirectory = "CacheHtmlFolder";

        public ImageView()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New && NavigationContext.QueryString.ContainsKey("url"))
            {
                try
                {
                    url = NavigationContext.QueryString["url"];
                    var stream = await new HttpHelp().Get(url);

                    bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);

                    ImgZoom.Source = new DownLoadedImage(bytes);
                }
                catch (Exception)
                {
                    new ToastPrompt().Show("图片出了点问题~");
                }
            }
        }

        // these two fields fully define the zoom state:
        private double TotalImageScale = 1d;
        private Point ImagePosition = new Point(0, 0);


        private const double MAX_IMAGE_ZOOM = 5;
        private Point _oldFinger1;
        private Point _oldFinger2;
        private double _oldScaleFactor;


        #region Event handlers

        /// <summary>
        /// Initializes the zooming operation
        /// </summary>
        private void OnPinchStarted(object sender, PinchStartedGestureEventArgs e)
        {
            _oldFinger1 = e.GetPosition(ImgZoom, 0);
            _oldFinger2 = e.GetPosition(ImgZoom, 1);
            _oldScaleFactor = 1;
        }

        /// <summary>
        /// Computes the scaling and translation to correctly zoom around your fingers.
        /// </summary>
        private void OnPinchDelta(object sender, PinchGestureEventArgs e)
        {
            var scaleFactor = e.DistanceRatio / _oldScaleFactor;
            if (!IsScaleValid(scaleFactor))
                return;

            var currentFinger1 = e.GetPosition(ImgZoom, 0);
            var currentFinger2 = e.GetPosition(ImgZoom, 1);

            var translationDelta = GetTranslationDelta(
                currentFinger1,
                currentFinger2,
                _oldFinger1,
                _oldFinger2,
                ImagePosition,
                scaleFactor);

            _oldFinger1 = currentFinger1;
            _oldFinger2 = currentFinger2;
            _oldScaleFactor = e.DistanceRatio;

            UpdateImageScale(scaleFactor);
            UpdateImagePosition(translationDelta);
        }

        /// <summary>
        /// Moves the image around following your finger.
        /// </summary>
        private void OnDragDelta(object sender, DragDeltaGestureEventArgs e)
        {
            var translationDelta = new Point(e.HorizontalChange, e.VerticalChange);

            if (IsDragValid(1, translationDelta))
                UpdateImagePosition(translationDelta);
        }

        /// <summary>
        /// Resets the image scaling and position
        /// </summary>
        private void OnDoubleTap(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        {
            ResetImagePosition();
        }

        #endregion

        #region Utils

        /// <summary>
        /// Computes the translation needed to keep the image centered between your fingers.
        /// </summary>
        private Point GetTranslationDelta(
            Point currentFinger1, Point currentFinger2,
            Point oldFinger1, Point oldFinger2,
            Point currentPosition, double scaleFactor)
        {
            var newPos1 = new Point(
             currentFinger1.X + (currentPosition.X - oldFinger1.X) * scaleFactor,
             currentFinger1.Y + (currentPosition.Y - oldFinger1.Y) * scaleFactor);

            var newPos2 = new Point(
             currentFinger2.X + (currentPosition.X - oldFinger2.X) * scaleFactor,
             currentFinger2.Y + (currentPosition.Y - oldFinger2.Y) * scaleFactor);

            var newPos = new Point(
                (newPos1.X + newPos2.X) / 2,
                (newPos1.Y + newPos2.Y) / 2);

            return new Point(
                newPos.X - currentPosition.X,
                newPos.Y - currentPosition.Y);
        }

        /// <summary>
        /// Updates the scaling factor by multiplying the delta.
        /// </summary>
        private void UpdateImageScale(double scaleFactor)
        {
            TotalImageScale *= scaleFactor;
            ApplyScale();
        }

        /// <summary>
        /// Applies the computed scale to the image control.
        /// </summary>
        private void ApplyScale()
        {
            ((CompositeTransform)ImgZoom.RenderTransform).ScaleX = TotalImageScale;
            ((CompositeTransform)ImgZoom.RenderTransform).ScaleY = TotalImageScale;
        }

        /// <summary>
        /// Updates the image position by applying the delta.
        /// Checks that the image does not leave empty space around its edges.
        /// </summary>
        private void UpdateImagePosition(Point delta)
        {
            var newPosition = new Point(ImagePosition.X + delta.X, ImagePosition.Y + delta.Y);

            if (newPosition.X > 0) newPosition.X = 0;
            if (newPosition.Y > 0) newPosition.Y = 0;

            if ((ImgZoom.ActualWidth * TotalImageScale) + newPosition.X < ImgZoom.ActualWidth)
                newPosition.X = ImgZoom.ActualWidth - (ImgZoom.ActualWidth * TotalImageScale);

            if ((ImgZoom.ActualHeight * TotalImageScale) + newPosition.Y < ImgZoom.ActualHeight)
                newPosition.Y = ImgZoom.ActualHeight - (ImgZoom.ActualHeight * TotalImageScale);

            ImagePosition = newPosition;

            ApplyPosition();
        }

        /// <summary>
        /// Applies the computed position to the image control.
        /// </summary>
        private void ApplyPosition()
        {
            ((CompositeTransform)ImgZoom.RenderTransform).TranslateX = ImagePosition.X;
            ((CompositeTransform)ImgZoom.RenderTransform).TranslateY = ImagePosition.Y;
        }

        /// <summary>
        /// Resets the zoom to its original scale and position
        /// </summary>
        private void ResetImagePosition()
        {
            TotalImageScale = 1;
            ImagePosition = new Point(0, 0);
            ApplyScale();
            ApplyPosition();
        }

        /// <summary>
        /// Checks that dragging by the given amount won't result in empty space around the image
        /// </summary>
        private bool IsDragValid(double scaleDelta, Point translateDelta)
        {
            if (ImagePosition.X + translateDelta.X > 0 || ImagePosition.Y + translateDelta.Y > 0)
                return false;

            if ((ImgZoom.ActualWidth * TotalImageScale * scaleDelta) + (ImagePosition.X + translateDelta.X) < ImgZoom.ActualWidth)
                return false;

            if ((ImgZoom.ActualHeight * TotalImageScale * scaleDelta) + (ImagePosition.Y + translateDelta.Y) < ImgZoom.ActualHeight)
                return false;

            return true;
        }

        /// <summary>
        /// Tells if the scaling is inside the desired range
        /// </summary>
        private bool IsScaleValid(double scaleDelta)
        {
            return (TotalImageScale * scaleDelta >= 1) && (TotalImageScale * scaleDelta <= MAX_IMAGE_ZOOM);
        }

        #endregion

        private void saveToLib()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    using (var stream = new MemoryStream())
                    {
                        var bmp = new WriteableBitmap(new DownLoadedImage(bytes));
                        var lib = new MediaLibrary();
                        var filePath = string.Format(Guid.NewGuid() + ".jpg");

                        bmp.SaveJpeg(stream, bmp.PixelWidth, bmp.PixelHeight, 0, 100);
                        stream.Seek(0, SeekOrigin.Begin);
                        lib.SavePicture(filePath, stream);

                        var toast = new ToastPrompt();
                        toast.Show("图片存储成功~");
                    }
                }
                catch
                {
                    MessageBox.Show(
                        "请断开手机与电脑的连接。",
                        "无法保存",
                        MessageBoxButton.OK);
                }
            });
        }

        private void insert_save_Click(object sender, EventArgs e)
        {
            var t = new Thread(saveToLib);
            t.Start();
        }
    }

    public sealed class DownLoadedImage : BitmapSource
    {
        public DownLoadedImage(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            SetSource(stream);
        }
    }
}