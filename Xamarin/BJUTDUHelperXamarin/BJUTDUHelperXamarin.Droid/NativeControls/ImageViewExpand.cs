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
using static Android.Views.ScaleGestureDetector;
using static Android.Views.View;
using Android.Graphics;
using Android.Util;
using static Android.Views.GestureDetector;
using Android.Graphics.Drawables;

namespace BJUTDUHelperXamarin.Droid.NativeControls
{
    public class ImageViewExpand : ImageView, IOnScaleGestureListener, IOnTouchListener, ViewTreeObserver.IOnGlobalLayoutListener
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
        public ImageViewExpand(Context context) : this(context, null)
        {
            this.LayoutChange += ImageViewExpand_LayoutChange;
        }

        private void ImageViewExpand_LayoutChange(object sender, LayoutChangeEventArgs e)
        {
            OnGlobalLayout();
        }
        public ImageViewExpand(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            base.SetScaleType(ScaleType.Matrix);

            mGestureDetector = new GestureDetector(context, new CustomSimpleOnGestureListener(this));

            mScaleGestureDetector = new ScaleGestureDetector(context, this);
            this.SetOnTouchListener(this);
        }
        public class CustomSimpleOnGestureListener : SimpleOnGestureListener
        {
            private ImageViewExpand _instance;
            public CustomSimpleOnGestureListener(ImageViewExpand instance)
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
                _instance.checkBorderAndCenterWhenScale();
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
                checkBorderAndCenterWhenScale();
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
        private void checkBorderAndCenterWhenScale()
        {

            RectF rect = getMatrixRectF();
            float deltaX = 0;
            float deltaY = 0;

            int width = Width;
            int height = Height;

            // 如果宽或高大于屏幕，则控制范围
            if (rect.Width() >= width)
            {
                if (rect.Left > 0)
                {
                    deltaX = -rect.Left;
                }
                if (rect.Right < width)
                {
                    deltaX = width - rect.Right;
                }
            }
            if (rect.Height() >= height)
            {
                if (rect.Top > 0)
                {
                    deltaY = -rect.Top;
                }
                if (rect.Bottom < height)
                {
                    deltaY = height - rect.Bottom;
                }
            }
            // 如果宽或高小于屏幕，则让其居中
            if (rect.Width() < width)
            {
                deltaX = width * 0.5f - rect.Right + 0.5f * rect.Width();
            }
            if (rect.Height() < height)
            {
                deltaY = height * 0.5f - rect.Bottom + 0.5f * rect.Height();
            }
            //Log.e(TAG, "deltaX = " + deltaX + " , deltaY = " + deltaY);

            mScaleMatrix.PostTranslate(deltaX, deltaY);

        }
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

            switch (e.Action)
            {
                case MotionEventActions.Down:
                    if (rectF.Width() > Width || rectF.Height() > Height)
                    {
                        Parent.RequestDisallowInterceptTouchEvent(true);
                    }
                    break;
                case MotionEventActions.Move:
                    if (rectF.Width() > Width || rectF.Height() > Height)
                    {
                        Parent.RequestDisallowInterceptTouchEvent(true);
                    }
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
                            isCheckLeftAndRight = isCheckTopAndBottom = true;
                            // 如果宽度小于屏幕宽度，则禁止左右移动
                            if (rectF.Width() < Width)
                            {
                                dx = 0;
                                isCheckLeftAndRight = false;
                            }
                            // 如果高度小雨屏幕高度，则禁止上下移动
                            if (rectF.Height() < Height)
                            {
                                dy = 0;
                                isCheckTopAndBottom = false;
                            }


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
            if (rect.Top > 0 && isCheckTopAndBottom)
            {
                deltaY = -rect.Top;
            }
            if (rect.Bottom < viewHeight && isCheckTopAndBottom)
            {
                deltaY = viewHeight - rect.Bottom;
            }
            if (rect.Left > 0 && isCheckLeftAndRight)
            {
                deltaX = -rect.Left;
            }
            if (rect.Right < viewWidth && isCheckLeftAndRight)
            {
                deltaX = viewWidth - rect.Right;
            }
            mScaleMatrix.PostTranslate(deltaX, deltaY);
        }

        public void OnGlobalLayout()
        {
            Drawable d = Drawable;
            if (d == null)
                return;
            //Log.e(TAG, d.IntrinsicWidth + " , " + d.IntrinsicHeight);
            int width = Width;
            int height = Height;
            // 拿到图片的宽和高
            int dw = d.IntrinsicWidth;
            int dh = d.IntrinsicHeight;
            if (dw == -1 || dh == -1)
                return;
            float scale = 1.0f;
            // 如果图片的宽或者高大于屏幕，则缩放至屏幕的宽或者高
            if (dw > width && dh <= height)
            {
                scale = width * 1.0f / dw;
            }
            if (dh > height && dw <= width)
            {
                scale = height * 1.0f / dh;
            }
            // 如果宽和高都大于屏幕，则让其按按比例适应屏幕大小
            if (dw > width && dh > height)
            {
                scale = System.Math.Min(width * 1.0f / dw, height * 1.0f / dh);
            }
            initScale = scale;

            //Log.e(TAG, "initScale = " + initScale);
            mScaleMatrix.PostTranslate((width - dw) / 2, (height - dh) / 2);
            mScaleMatrix.PostScale(scale, scale, Width / 2,
                    Height / 2);
            // 图片移动至屏幕中心
            ImageMatrix = (mScaleMatrix);
            once = false;
        }
    }
}