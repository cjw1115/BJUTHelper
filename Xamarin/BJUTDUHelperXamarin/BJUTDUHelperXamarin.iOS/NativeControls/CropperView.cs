
using UIKit;
using CoreGraphics;

namespace BJUTDUHelperXamarin.iOS.NativeControls
{
    public class CropperView : UIView
    {
        CGPoint origin;
        CGSize cropSize;

        public float OffsetX = 20;
        public CropperView(CGRect rect)
        {
            this.Frame = rect;

            var width=rect.Size.Width - OffsetX * 2;
            var offsetY = (rect.Size.Height - width)/2;
            
            origin = new CGPoint(OffsetX, offsetY);

            cropSize = new CGSize(width, width);

            BackgroundColor = UIColor.Clear;
            Opaque = false;

            Alpha = 0.4f;
        }

        public CGPoint Origin
        {
            get
            {
                return origin;
            }

            set
            {
                origin = value;
                //SetNeedsDisplay();
            }
        }

        public CGSize CropSize
        {
            get
            {
                return cropSize;
            }
            set
            {
                cropSize = value;
                //SetNeedsDisplay();
            }
        }

        public CGRect CropRect
        {
            get
            {
                return new CGRect(Origin, CropSize);
            }
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);

            using (var g = UIGraphics.GetCurrentContext())
            {

                g.SetFillColor(UIColor.Gray.CGColor);
                g.FillRect(rect);

                g.SetBlendMode(CGBlendMode.Clear);
                UIColor.Clear.SetColor();

                var path = new CGPath();
                path.AddRect(new CGRect(origin, cropSize));

                g.AddPath(path);
                g.DrawPath(CGPathDrawingMode.Fill);
            }
        }
    }
}
