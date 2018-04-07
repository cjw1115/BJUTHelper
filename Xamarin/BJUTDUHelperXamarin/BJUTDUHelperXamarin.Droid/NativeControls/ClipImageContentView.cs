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
using Xamarin.Forms.Platform.Android;
using static Android.Views.ScaleGestureDetector;
using static Android.Views.View;
using Android.Graphics;
using Android.Util;
using Java.Lang;
using Android.Graphics.Drawables;
using static Android.Gestures.GestureOverlayView;
using Android.Gestures;
using static Android.Views.GestureDetector;
using Java.IO;
using System.Threading.Tasks;
using System.IO;

namespace BJUTDUHelperXamarin.Droid.NativeControls
{
    public class ClipImageContentView : ImageView, IOnScaleGestureListener, IOnTouchListener, ViewTreeObserver.IOnGlobalLayoutListener
    {
        public static readonly float SCALE_MAX = 4.0f;

        /**
         * 初始化时的缩放比例，如果图片宽或高大于屏幕，此值将小于0
         */
        private float initScale = 1.0f;
        private bool once = true;

        /**
         * 用于存放矩阵的9个值
         */
        private readonly float[] matrixValues = new float[9];

        /**
         * 缩放的手势检测
         */
        private ScaleGestureDetector mScaleGestureDetector = null;
        private readonly Matrix mScaleMatrix = new Matrix();

        /**
         * 用于双击检测
         */
        private GestureDetector mGestureDetector;

        private int mTouchSlop;

        private float mLastX;
        private float mLastY;

        private bool isCanDrag;
        private int lastPointerCount;

        private bool isCheckTopAndBottom = true;
        private bool isCheckLeftAndRight = true;

        private int mHorizontalPadding = 0;
        private int mVerticalPadding = 0;
        public void setHorizontalPadding(int value)
        {
            mHorizontalPadding = value;

        }
        public ClipImageContentView(Context context) : this(context, null)
        {
            this.LayoutChange += ImageViewExpand_LayoutChange;
        }

        private void ImageViewExpand_LayoutChange(object sender, LayoutChangeEventArgs e)
        {
            OnGlobalLayout();
        }
        public ClipImageContentView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            base.SetScaleType(ScaleType.Matrix);
            this.Background = new ColorDrawable(Color.White);
            mGestureDetector = new GestureDetector(context, new CustomSimpleOnGestureListener(this));

            mScaleGestureDetector = new ScaleGestureDetector(context, this);
            this.SetOnTouchListener(this);
        }
        public class CustomSimpleOnGestureListener : SimpleOnGestureListener
        {
            private ClipImageContentView _instance;
            public CustomSimpleOnGestureListener(ClipImageContentView instance)
            {
                _instance = instance;
            }
            public override bool OnDoubleTap(MotionEvent e)
            {
                float x = e.GetX();
                float y = e.GetY();
                float scaleFactor = 1f;
                //Log.e("DoubleTap", getScale() + " , " + initScale);
                float scale = _instance.getScale();
                if (scale < SCALE_MAX)
                {
                    scaleFactor = 2;
                }
                else
                {
                    scaleFactor = _instance.initScale / scale;
                }

                _instance.mScaleMatrix.PostScale(scaleFactor, scaleFactor, x, y);
                _instance.checkBorder();
                _instance.ImageMatrix = (_instance.mScaleMatrix);
                // 进行缩放

                return true;
            }
        }
        public float getScale()
        {
            mScaleMatrix.GetValues(matrixValues);
            return matrixValues[Matrix.MscaleX];
        }
        public bool OnScale(ScaleGestureDetector detector)
        {
            float scale = getScale();
            float scaleFactor = detector.ScaleFactor;

            if (Drawable == null)
                return true;

            /**
             * 缩放的范围控制
             */
            if ((scale < SCALE_MAX && scaleFactor > 1.0f)
                    || (scale > initScale && scaleFactor < 1.0f))
            {
                /**
                 * 最大值最小值判断
                 */
                if (scaleFactor * scale < initScale)
                {
                    scaleFactor = initScale / scale;
                }
                if (scaleFactor * scale > SCALE_MAX)
                {
                    scaleFactor = SCALE_MAX / scale;
                }
                /**
                 * 设置缩放比例
                 */
                mScaleMatrix.PostScale(scaleFactor, scaleFactor,
                        detector.FocusX, detector.FocusY);
                checkBorder();
                ImageMatrix = mScaleMatrix;
            }
            return true;
        }

        public bool OnScaleBegin(ScaleGestureDetector detector)
        {
            return true;
        }

        public void OnScaleEnd(ScaleGestureDetector detector)
        {

        }
        //private void checkBorderAndCenterWhenScale()
        //{

        //    RectF rect = getMatrixRectF();
        //    float deltaX = 0;
        //    float deltaY = 0;

        //    int width = Width-2*mHorizontalPadding;
        //    //int height = Height;

        //    // 如果宽或高大于屏幕，则控制范围
        //    if (rect.Width() >= width)
        //    {
        //        if (rect.Left > mHorizontalPadding)
        //        {
        //            deltaX = mHorizontalPadding - rect.Left;
        //        }
        //        if (rect.Right < width- mHorizontalPadding)
        //        {
        //            deltaX = rect.Right-(width - mHorizontalPadding);
        //        }
        //    }
        //    var mVerticalPadding = (Height - width) / 2;

        //    if (rect.Height() >= width)
        //    {
        //        if (rect.Top> mVerticalPadding)
        //        {
        //            deltaY = mVerticalPadding - rect.Top;
        //        }
        //        if (rect.Bottom < Height- mVerticalPadding)
        //        {
        //            deltaY = rect.Bottom-(Height -mVerticalPadding) ;
        //        }
        //    }

        //    mScaleMatrix.PostTranslate(deltaX, deltaY);


        //    //var scale = 1f;
        //    //// 如果宽或高小于屏幕，则让其居中
        //    //if (rect.Width() < width|| rect.Height() < width)
        //    //{
        //    //    scale= Java.Lang.Math.Max(width / rect.Width(), width / rect.Height());
        //    //}

        //    ////Log.e(TAG, "deltaX = " + deltaX + " , deltaY = " + deltaY);
        //    //mScaleMatrix.PostScale(scale,scale);
        //}
        private RectF getMatrixRectF()
        {
            Matrix matrix = mScaleMatrix;
            RectF rect = new RectF();
            Drawable d = Drawable;
            if (null != d)
            {
                rect.Set(0, 0, d.IntrinsicWidth, d.IntrinsicHeight);
                matrix.MapRect(rect);
            }
            return rect;
        }

        private bool IsCanDrag(float dx, float dy)
        {
            return System.Math.Sqrt((dx * dx) + (dy * dy)) >= mTouchSlop;
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            if (mGestureDetector.OnTouchEvent(e))
                return true;
            mScaleGestureDetector.OnTouchEvent(e);

            float x = 0, y = 0;
            // 拿到触摸点的个数
            int pointerCount = e.PointerCount;
            // 得到多个触摸点的x与y均值
            for (int i = 0; i < pointerCount; i++)
            {
                x += e.GetX(i);
                y += e.GetY(i);
            }
            x = x / pointerCount;
            y = y / pointerCount;

            /**
             * 每当触摸点发生变化时，重置mLasX , mLastY
             */
            if (pointerCount != lastPointerCount)
            {
                isCanDrag = false;
                mLastX = x;
                mLastY = y;
            }

            lastPointerCount = pointerCount;

            RectF rectF = getMatrixRectF();

            var width = Width - 2 * mHorizontalPadding;
            switch (e.Action)
            {
                //case MotionEventActions.Down:
                //    if (rectF.Width() > Width || rectF.Height() > Height)
                //    {
                //        Parent.RequestDisallowInterceptTouchEvent(true);
                //    }
                //    break;
                case MotionEventActions.Move:
                    //if (rectF.Width() > Width || rectF.Height() > Height)
                    //{
                    //    Parent.RequestDisallowInterceptTouchEvent(true);
                    //}
                    //Log.e(TAG, "ACTION_MOVE");
                    float dx = x - mLastX;
                    float dy = y - mLastY;

                    if (!isCanDrag)
                    {
                        isCanDrag = IsCanDrag(dx, dy);
                    }
                    if (isCanDrag)
                    {

                        if (Drawable != null)
                        {
                            // if (getMatrixRectF().left == 0 && dx > 0)
                            // {
                            // getParent().requestDisallowInterceptTouchEvent(false);
                            // }
                            //
                            // if (getMatrixRectF().right == Width && dx < 0)
                            // {
                            // getParent().requestDisallowInterceptTouchEvent(false);
                            // }
                            //isCheckLeftAndRight = isCheckTopAndBottom = true;
                            //// 如果宽度小于屏幕宽度，则禁止左右移动
                            //if (rectF.Width() < width)
                            //{
                            //    dx = 0;
                            //}
                            //// 如果高度小雨屏幕高度，则禁止上下移动
                            //if (rectF.Height() < width)
                            //{
                            //    dy = 0;
                            //    isCheckTopAndBottom = false;
                            //}


                            mScaleMatrix.PostTranslate(dx, dy);
                            checkMatrixBounds();
                            ImageMatrix = (mScaleMatrix);
                        }
                    }
                    mLastX = x;
                    mLastY = y;
                    break;
                case MotionEventActions.Up:
                case MotionEventActions.Cancel:
                    //Log.e(TAG, "ACTION_UP");
                    lastPointerCount = 0;
                    break;
            }
            return true;
        }
        private void checkMatrixBounds()
        {
            RectF rect = getMatrixRectF();

            float deltaX = 0, deltaY = 0;
            float viewWidth = Width;
            float viewHeight = Height;
            // 判断移动或缩放后，图片显示是否超出屏幕边界
            if (rect.Top > mVerticalPadding && isCheckTopAndBottom)
            {
                deltaY = mVerticalPadding - rect.Top;
            }
            if (rect.Bottom < viewHeight - mVerticalPadding && isCheckTopAndBottom)
            {
                deltaY = (viewHeight - mVerticalPadding) - rect.Bottom;
            }
            if (rect.Left > mHorizontalPadding && isCheckLeftAndRight)
            {
                deltaX = mHorizontalPadding - rect.Left;
            }
            if (rect.Right < viewWidth - mHorizontalPadding && isCheckLeftAndRight)
            {
                deltaX = (viewWidth - mHorizontalPadding) - rect.Right;
            }
            mScaleMatrix.PostTranslate(deltaX, deltaY);
        }


        public void OnGlobalLayout()
        {
            if (once)
            {
                Drawable d = Drawable;
                if (d == null)
                    return;
                //Log.e(TAG, d.getIntrinsicWidth() + " , " + d.getIntrinsicHeight());
                // 计算padding的px
                //mHorizontalPadding = (int)TypedValue.ApplyDimension(
                //         ComplexUnitType.Dip, mHorizontalPadding,
                //        Resources.DisplayMetrics);
                // 垂直方向的边距
                mVerticalPadding = (Height - (Width - 2 * mHorizontalPadding)) / 2;

                int width = Width;
                int height = Height;
                // 拿到图片的宽和高
                int dw = d.IntrinsicWidth;
                int dh = d.IntrinsicHeight;
                float scale = 1.0f;
                if (dw < Width - mHorizontalPadding * 2
                        && dh > Height - mVerticalPadding * 2)
                {
                    scale = (Width * 1.0f - mHorizontalPadding * 2) / dw;
                }

                else if (dh < Height - mVerticalPadding * 2
                        && dw > Width - mHorizontalPadding * 2)
                {
                    scale = (Height * 1.0f - mVerticalPadding * 2) / dh;
                }

                else if (dw < Width - mHorizontalPadding * 2
                        && dh < Height - mVerticalPadding * 2)
                {
                    float scaleW = (Width * 1.0f - mHorizontalPadding * 2)
                            / dw;
                    float scaleH = (Height * 1.0f - mVerticalPadding * 2) / dh;
                    scale = Java.Lang.Math.Max(scaleW, scaleH);
                }
                else
                {
                    float scaleW = (Width * 1.0f - mHorizontalPadding * 2)
                            / dw;
                    float scaleH = (Height * 1.0f - mVerticalPadding * 2) / dh;
                    scale = Java.Lang.Math.Max(scaleW, scaleH);
                }
                initScale = scale;
                //SCALE_MID = initScale * 2;
                //SCALE_MAX = initScale * 4;
                //Log.e(TAG, "initScale = " + initScale);
                mScaleMatrix.PostTranslate((width - dw) / 2, (height - dh) / 2);
                mScaleMatrix.PostScale(scale, scale, Width / 2,
                        height / 2);
                // 图片移动至屏幕中心
                ImageMatrix = (mScaleMatrix);
                once = false;
            }

        }

        /**
         * 剪切图片，返回剪切后的bitmap对象
         * 
         * @return
         */
        public Bitmap clip()
        {
            Bitmap bitmap = Bitmap.CreateBitmap(Width,Height,Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            Draw(canvas);
            bitmap= Bitmap.CreateBitmap(bitmap, mHorizontalPadding,
                    mVerticalPadding, Width - 2 * mHorizontalPadding,
                    Width - 2 * mHorizontalPadding);
            return scaleBitmap(bitmap, 100, 100);
        }
        private Bitmap scaleBitmap(Bitmap origin, int newWidth, int newHeight)
        {
            if (origin == null)
            {
                return null;
            }
            int height = origin.Width;
            int width = origin.Width;
            float scaleWidth = ((float)newWidth) / width;
            float scaleHeight = ((float)newHeight) / height;
            Matrix matrix = new Matrix();
            matrix.PostScale(scaleWidth, scaleHeight);// 使用后乘
            Bitmap newBM = Bitmap.CreateBitmap(origin, 0, 0, width, height, matrix, false);
            if (!origin.IsRecycled)
            {
                origin.Recycle();
            }
            return newBM;
        }



        /**
         * 边界检测
         */
        private void checkBorder()
        {

            RectF rect = getMatrixRectF();
            float deltaX = 0;
            float deltaY = 0;

            int width = Width;
            int height = Height;

            // 如果宽或高大于屏幕，则控制范围
            if (rect.Width() >= width - 2 * mHorizontalPadding)
            {
                if (rect.Left > mHorizontalPadding)
                {
                    deltaX = -rect.Left + mHorizontalPadding;
                }
                if (rect.Right < width - mHorizontalPadding)
                {
                    deltaX = width - mHorizontalPadding - rect.Right;
                }
            }
            if (rect.Height() >= height - 2 * mVerticalPadding)
            {
                if (rect.Top > mVerticalPadding)
                {
                    deltaY = -rect.Top + mVerticalPadding;
                }
                if (rect.Bottom < height - mVerticalPadding)
                {
                    deltaY = height - mVerticalPadding - rect.Bottom;
                }
            }
            mScaleMatrix.PostTranslate(deltaX, deltaY);

        }
        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            ViewTreeObserver.AddOnGlobalLayoutListener(this);
        }
    }
}