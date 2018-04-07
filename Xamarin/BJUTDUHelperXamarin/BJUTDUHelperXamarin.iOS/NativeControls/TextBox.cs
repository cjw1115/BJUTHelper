using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreGraphics;
using CoreAnimation;

namespace BJUTDUHelperXamarin.iOS.NativeControls
{
    public class TextBox : UITextField
    {
        public bool banimationLine { get; set; }
        UILabel placeHolderLabel;

        //声明成员变量
        //嵌入
        float inset;
        //图层
        CAShapeLayer leftLayer;
        //嵌入点
        CGPoint textFieldInset, placeholderInset;

        float placeholderHeight;

        public override void DrawRect(CGRect area, UIViewPrintFormatter formatter)
        {
            base.DrawRect(area, formatter);

            this.Add(placeHolderLabel);
            //创建路径
            UIBezierPath leftPath = new UIBezierPath();

            //起点
            leftPath.MoveTo(new CGPoint(0, this.Bounds.Size.Height));

            //画第一条线
            leftPath.AddLineTo(new CGPoint(0, this.Bounds.Size.Height));

            //画第二条线
            leftPath.AddLineTo(new CGPoint(this.Bounds.Size.Width, this.Bounds.Size.Height));

            //设置图层的属性
            leftLayer.Path = leftPath.CGPath; //动画路径
            leftLayer.StrokeColor = new CGColor(1f, 1f, 1f); ;  //外边框颜色
            leftLayer.FillColor = null;   //不设置路径颜色填充

            leftLayer.BorderWidth = 3.0f; //图层边框
            leftLayer.LineCap = new NSString("kCALineCapRound");    //线头样式为圆形
            leftLayer.LineJoin = new NSString("kCALineJoinRound");  //拐角样式为圆角
                                                                    //    leftLayer.frame=CGRectMake(0, 50, 200, 1);
            leftLayer.BorderColor = UIColor.Black.CGColor; //边框颜色


            this.Layer.AddSublayer(leftLayer);
        }

    }
}