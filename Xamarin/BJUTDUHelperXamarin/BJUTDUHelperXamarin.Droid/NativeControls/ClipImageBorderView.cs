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
using Android.Graphics;
using Android.Util;
using Android.Graphics.Drawables;

namespace BJUTDUHelperXamarin.Droid.NativeControls
{
    public class ClipImageBorderView : View
    {
        /**
        * 水平方向与View的边距
        */
        private int mHorizontalPadding = 0;
        public void setHorizontalPadding(int value)
        {
            mHorizontalPadding = value;
        }
        /**
         * 垂直方向与View的边距
         */
        private int mVerticalPadding;
        /**
         * 绘制的矩形的宽度
         */
        private int mWidth;
        /**
         * 边框的颜色，默认为白色
         */
        private Color mBorderColor = Color.ParseColor("#FFFFFF");
        /**
         * 边框的宽度 单位dp
         */
        private int mBorderWidth = 1;

        private Paint mPaint;

        public ClipImageBorderView(Context context) : this(context, null)
        {
            //this.Background = new ColorDrawable(Color.Transparent);
        }

        public ClipImageBorderView(Context context, IAttributeSet attrs) : this(context, attrs, 0)
        {
        }

        public ClipImageBorderView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {

            // 计算padding的px
            mHorizontalPadding = (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, mHorizontalPadding, Resources.DisplayMetrics);
            mBorderWidth = (int)TypedValue.ApplyDimension(
                     ComplexUnitType.Dip, mBorderWidth, Resources.DisplayMetrics);
            mPaint = new Paint();
            mPaint.AntiAlias = (true);
        }


        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            //计算矩形区域的宽度
            mWidth = Width - 2 * mHorizontalPadding;
            //计算距离屏幕垂直边界 的边距
            mVerticalPadding = (Height - mWidth) / 2;
            mPaint.Color = (Color.ParseColor("#aa000000"));
            mPaint.SetStyle(Paint.Style.Fill);
            // 绘制左边1
            canvas.DrawRect(0, 0, mHorizontalPadding, Height, mPaint);
            // 绘制右边2
            canvas.DrawRect(Width - mHorizontalPadding, 0, Width,
                    Height, mPaint);
            // 绘制上边3
            canvas.DrawRect(mHorizontalPadding, 0, Width - mHorizontalPadding,
                    mVerticalPadding, mPaint);
            // 绘制下边4
            canvas.DrawRect(mHorizontalPadding, Height - mVerticalPadding,
                    Width - mHorizontalPadding, Height, mPaint);
            // 绘制外边框

            mPaint.Color = Color.Transparent;
            mPaint.StrokeWidth = (mBorderWidth);
            mPaint.SetStyle(Paint.Style.Fill);
            canvas.DrawRect(mHorizontalPadding, mVerticalPadding, Width
                    - mHorizontalPadding, Height - mVerticalPadding, mPaint);

        }
    }
}