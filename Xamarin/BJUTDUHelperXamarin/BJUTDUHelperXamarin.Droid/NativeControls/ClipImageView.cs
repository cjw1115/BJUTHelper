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
using Android.Util;
using Android.Graphics;
using Android.Graphics.Drawables;
using System.IO;
using BJUTDUHelperXamarin.Controls;
using System.Threading.Tasks;

namespace BJUTDUHelperXamarin.Droid.NativeControls
{
    public class ClipImageView : RelativeLayout, IClip
    {
        private ClipImageContentView _contentView;
        private ClipImageBorderView _borderView;

        private int mHorizontalPadding = 40;

        private Drawable _drawable;
        public void SetBitmap(string path)
        {
           _drawable= BitmapDrawable.CreateFromPath(path);
            _contentView.SetImageDrawable(_drawable);
        }

        public ClipImageView(Context context, IAttributeSet attrs) : base(context, attrs)
        {

            this.Background = new ColorDrawable(Color.White);

            _contentView = new ClipImageContentView(context);
            _borderView = new ClipImageBorderView(context);

            var lp = new LayoutParams(
                Android.Views.ViewGroup.LayoutParams.MatchParent,
                Android.Views.ViewGroup.LayoutParams.MatchParent);

            this.AddView(_contentView, lp);
            this.AddView(_borderView, lp);

            //lp=new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.WrapContent, RelativeLayout.LayoutParams.WrapContent);
            //lp.AddRule(LayoutRules.AlignParentRight);

            //Button clipButton = new Button(this.Context);
            //clipButton.Click += ClipButton_Click;
            // 计算padding的px
            mHorizontalPadding = (int)TypedValue.ApplyDimension(
                 ComplexUnitType.Dip, mHorizontalPadding, Resources.DisplayMetrics);
            _contentView.setHorizontalPadding(mHorizontalPadding);
            _borderView.setHorizontalPadding(mHorizontalPadding);
        }
        /**
         * 对外公布设置边距的方法,单位为dp
         * 
         * @param mHorizontalPadding
         */
        public void setHorizontalPadding(int mHorizontalPadding)
        {
            this.mHorizontalPadding = mHorizontalPadding;
        }

        /**
         * 裁切图片
         * 
         * @return
         */
       
        public async Task<string> saveBitmap(Bitmap bitmap)
        {
            FileStream fileStream = null;
            try
            {
                
                var folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                var filePath = System.IO.Path.Combine(folder, "bjuthelpertempavatar.png");
                fileStream = System.IO.File.Create(filePath);
                await bitmap.CompressAsync(Bitmap.CompressFormat.Png,40, fileStream);
                await fileStream.FlushAsync();
                fileStream.Close();
                return filePath;
            }
            catch(Exception e)
            {
                fileStream?.Close();
            }
            return null;
        }


        public async Task<string> ClipImage()
        {
            var bitmap = _contentView.clip();
            return await saveBitmap(bitmap);
        }
    }
}

