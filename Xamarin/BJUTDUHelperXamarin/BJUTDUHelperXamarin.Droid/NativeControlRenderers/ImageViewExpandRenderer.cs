using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using BJUTDUHelperXamarin.Droid.NativeControlRenderers;
using Xamarin.Forms.Platform.Android;
using FormsImageViewExpand = BJUTDUHelperXamarin.Controls.ImageViewExpand;
using BJUTDUHelperXamarin.Droid.NativeControls;
using System.Threading.Tasks;
using Android.Graphics;
using System.ComponentModel;
using Xamarin.Forms.Internals;

[assembly: ExportRenderer(typeof(FormsImageViewExpand), typeof(ImageViewExpandRenderer))]
namespace BJUTDUHelperXamarin.Droid.NativeControlRenderers
{

    internal interface IImageRendererController
    {
        void SkipInvalidate();
    }

    public class ImageViewExpandRenderer : ViewRenderer<FormsImageViewExpand, ImageViewExpand>
    {
        bool _isDisposed;

        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            base.Dispose(disposing);
        }

        protected override ImageViewExpand CreateNativeControl()
        {
            return new ImageViewExpand(Context);
        }

        protected override async void OnElementChanged(ElementChangedEventArgs<FormsImageViewExpand> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                var view = CreateNativeControl();
                SetNativeControl(view);
            }
            await TryUpdateBitmap(e.OldElement);
        }

        protected async override  void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Image.SourceProperty.PropertyName)
                await TryUpdateBitmap();
        }

        protected virtual async Task TryUpdateBitmap(Image previous = null)
        {
            // By default we'll just catch and log any exceptions thrown by UpdateBitmap so they don't bring down
            // the application; a custom renderer can override this method and handle exceptions from
            // UpdateBitmap differently if it wants to

            try
            {
                await UpdateBitmap(previous);
            }
            catch (Exception ex)
            {
                Log.Warning(nameof(ImageRenderer), "Error loading image: {0}", ex);
            }
            finally
            {
                ((IImageController)Element)?.SetIsLoading(false);
            }
        }

        protected async Task UpdateBitmap(Image previous = null)
        {
            if (Element == null || Control == null || Control.IsDisposed())
            {
                return;
            }

            await Control.UpdateBitmap(Element, previous);
        }
    }
    internal static class ImageViewExtensions
    {
        // TODO hartez 2017/04/07 09:33:03 Review this again, not sure it's handling the transition from previousImage to 'null' newImage correctly
        public static async Task UpdateBitmap(this ImageView imageView, Image newImage, Image previousImage = null)
        {
            if (imageView == null)
                return;

            if (Device.IsInvokeRequired)
                throw new InvalidOperationException("Image Bitmap must not be updated from background thread");

            if (previousImage != null && Equals(previousImage.Source, newImage.Source))
                return;

            var imageController = newImage as IImageController;

            imageController?.SetIsLoading(true);

            (imageView as IImageRendererController)?.SkipInvalidate();

            //imageView.SetImageResource(global::Android.Resource.Color.Transparent);

            ImageSource source = newImage?.Source;
            Bitmap bitmap = null;
            IImageSourceHandler handler;

            if (source != null && (handler = Xamarin.Forms.Internals.Registrar.Registered.GetHandler<IImageSourceHandler>(source.GetType())) != null)
            {
                try
                {
                    bitmap = await handler.LoadImageAsync(source, imageView.Context);
                }
                catch (TaskCanceledException)
                {
                    imageController?.SetIsLoading(false);
                }
            }

            if (newImage == null || !Equals(newImage.Source, source))
            {
                bitmap?.Dispose();
                return;
            }

            if (!imageView.IsDisposed())
            {
                if (bitmap == null && source is FileImageSource)
                    imageView.SetImageResource(ResourceManager.GetDrawableByName(((FileImageSource)source).File));
                else
                {
                    imageView.SetImageBitmap(bitmap);
                }
            }

            bitmap?.Dispose();

            imageController?.SetIsLoading(false);
            ((IVisualElementController)newImage).NativeSizeChanged();
        }
    }
    internal static class JavaObjectExtensions
    {
        public static bool IsDisposed(this Java.Lang.Object obj)
        {
            return obj.Handle == IntPtr.Zero;
        }

    }
}