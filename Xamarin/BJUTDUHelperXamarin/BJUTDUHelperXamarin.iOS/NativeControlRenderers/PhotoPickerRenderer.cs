using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using BJUTDUHelperXamarin.iOS.NativeControlRenderers;
using BJUTDUHelperXamarin.Controls;
using ClipImageView;
using CoreGraphics;
using BJUTDUHelperXamarin.iOS.NativeControls;
using System.Threading.Tasks;
using System.IO;

[assembly: ExportRenderer(typeof(BJUTDUHelperXamarin.Controls.PhotoPicker), typeof(PhotoPickerRenderer))]
namespace BJUTDUHelperXamarin.iOS.NativeControlRenderers
{

    public class PhotoPickerRenderer : PageRenderer
    {
        public static PhotoPicker PhotoPicker { get; set; }

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
            {
                return;
            }


            PhotoPicker = this.Element as PhotoPicker;

            var controller=UIApplication.SharedApplication.KeyWindow.RootViewController;

            var img=UIImage.FromFile(PhotoPicker.Path);
            var width = View.Frame.Width - 40;
            var cropperController = new ClipImageView.VPImageCropperViewController(img,new CGRect(20f,(View.Frame.Height - width)/2f,width,width),3);
            cropperController.Delegate = new VPImageCropperViewControllerDelegete();


            controller.PresentViewController(cropperController, true,null);
        }

    }
    public class VPImageCropperViewControllerDelegete : ClipImageView.VPImageCropperDelegate
    { 
        public async override void ImageCropper(VPImageCropperViewController cropperViewController, UIImage editedImage)
        {
            var re=await SaveBitmap(editedImage);
            PhotoPickerRenderer.PhotoPicker.SetClipImagePath(re);
            cropperViewController.DismissViewController(true,null);
            await PhotoPickerRenderer.PhotoPicker.Navigation.PopModalAsync();

        }

        public async override void ImageCropperDidCancel(VPImageCropperViewController cropperViewController)
        {
            cropperViewController.DismissViewController(true, null);
            await PhotoPickerRenderer.PhotoPicker.Navigation.PopModalAsync();
        }

        public async Task<string>  SaveBitmap(UIImage bitmap)
        {
            string path = null;
            await Task.Run(() =>
            {
                var folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                var filePath = System.IO.Path.Combine(folder, "bjuthelpertempavatar.jpeg");
                bitmap.AsJPEG(0.4f).Save(filePath,NSDataWritingOptions.FileProtectionNone,out NSError error);
#if DEBUG
                System.Diagnostics.Debug.WriteLine($"Save file error:{error}");
#endif
                path= filePath;
            });

            return path;
        }
    }
    //public class PhotoPickerRenderer:PageRenderer
    //{


    //    UIImageView imageView;
    //    CropperView cropperView;

    //    UIPanGestureRecognizer pan;

    //    UIPinchGestureRecognizer pinch;

    //    UITapGestureRecognizer doubleTap;


    //    PhotoPicker _formsControl;

    //    protected override void OnElementChanged(VisualElementChangedEventArgs e)
    //    {
    //        base.OnElementChanged(e);

    //        _formsControl = this.Element as PhotoPicker;
    //    }


    //    public void Center()
    //    {
    //    }
    //    public override void ViewDidLoad()

    //    {

    //        base.ViewDidLoad();


    //        cropperView = new CropperView(View.Frame);

    //        using (var image = UIImage.FromFile(_formsControl.Path))
    //        {

    //            imageView = new UIImageView(new CGRect(0, 0, image.Size.Width, image.Size.Height));

    //            imageView.Image = image;

    //        }

    //        if (imageView.Frame.Width > cropperView.CropRect.Width && imageView.Frame.Height > cropperView.CropRect.Height)
    //        {
    //            var rateX = View.Frame.Width / imageView.Frame.Width;

    //            var rateY = View.Frame.Height / imageView.Frame.Height;

    //            var rate = Math.Min(rateX, rateY);
    //            imageView.Transform.Scale((float)rate, (float)rate);
    //        }
    //        else if(imageView.Frame.Width > cropperView.CropRect.Width && imageView.Frame.Height <= cropperView.CropRect.Height)
    //        {
    //            var rate = cropperView.Frame.Height / imageView.Frame.Height;
    //            imageView.Transform.Scale((float)rate, (float)rate);
    //        }
    //        else if (imageView.Frame.Width <= cropperView.CropRect.Width && imageView.Frame.Height > cropperView.CropRect.Height)
    //        {
    //            var rate = cropperView.Frame.Width / imageView.Frame.Width;
    //            imageView.Transform.Scale((float)rate, (float)rate);
    //        }
    //        else
    //        {
    //            var rateX = cropperView.Frame.Width / imageView.Frame.Width;

    //            var rateY = cropperView.Frame.Height / imageView.Frame.Height;

    //            var rate = Math.Max(rateX, rateY);
    //            imageView.Transform.Scale((float)rate, (float)rate);
    //        }
    //        View.AddSubviews(imageView, cropperView);



    //        nfloat dx = 0;

    //        nfloat dy = 0;

    //        //pan = new UIPanGestureRecognizer(() => {

    //        //    if ((pan.State == UIGestureRecognizerState.Began || pan.State == UIGestureRecognizerState.Changed) && (pan.NumberOfTouches == 1))
    //        //    {



    //        //        var p0 = pan.LocationInView(View);



    //        //        if (dx == 0)

    //        //            dx = p0.X - cropperView.Origin.X;



    //        //        if (dy == 0)

    //        //            dy = p0.Y - cropperView.Origin.Y;



    //        //        var p1 = new CGPoint(p0.X - dx, p0.Y - dy);



    //        //        cropperView.Origin = p1;

    //        //    }
    //        //    else if (pan.State == UIGestureRecognizerState.Ended)
    //        //    {

    //        //        dx = 0;

    //        //        dy = 0;

    //        //    }

    //        //});



    //        //nfloat s0 = 1;



    //        //pinch = new UIPinchGestureRecognizer(() => {

    //        //    nfloat s = pinch.Scale;

    //        //    nfloat ds = (nfloat)Math.Abs(s - s0);

    //        //    nfloat sf = 0;

    //        //    const float rate = 0.5f;



    //        //    if (s >= s0)
    //        //    {

    //        //        sf = 1 + ds * rate;

    //        //    }
    //        //    else if (s < s0)
    //        //    {

    //        //        sf = 1 - ds * rate;

    //        //    }

    //        //    s0 = s;



    //        //    cropperView.CropSize = new CGSize(cropperView.CropSize.Width * sf, cropperView.CropSize.Height * sf);



    //        //    if (pinch.State == UIGestureRecognizerState.Ended)
    //        //    {

    //        //        s0 = 1;

    //        //    }

    //        //});



    //        //doubleTap = new UITapGestureRecognizer((gesture) => Crop())
    //        //{

    //        //    NumberOfTapsRequired = 2,
    //        //    NumberOfTouchesRequired = 1

    //        //};



    //        //cropperView.AddGestureRecognizer(pan);

    //        //cropperView.AddGestureRecognizer(pinch);

    //        //cropperView.AddGestureRecognizer(doubleTap);

    //    }



    //    void Crop()

    //    {

    //        var inputCGImage = UIImage.FromFile("monkey.png").CGImage;



    //        var image = inputCGImage.WithImageInRect(cropperView.CropRect);

    //        using (var croppedImage = UIImage.FromImage(image))
    //        {



    //            imageView.Image = croppedImage;

    //            imageView.Frame = cropperView.CropRect;

    //            imageView.Center = View.Center;



    //            cropperView.Origin = new CGPoint(imageView.Frame.Left, imageView.Frame.Top);

    //            cropperView.Hidden = true;

    //        }

    //    }
    //}


}