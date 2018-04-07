﻿using PivotPagePortable;
using PivotView;
using PivotView.iOS;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
[assembly: ExportRenderer(typeof(ScrollViewExpand), typeof(ScrollViewExpandRenderer))]
namespace PivotView.iOS
{
    public class ScrollViewExpandRenderer : ScrollViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            var scrollView = this.NativeView as UIScrollView;

            scrollView.PagingEnabled = true;

            scrollView.DraggingStarted += ScrollView_DraggingStarted;

            scrollView.DecelerationEnded += ScrollView_DecelerationEnded; ;

        }
        private void ScrollView_DecelerationEnded(object sender, EventArgs e)
        {
            var scrollView = this.Element as ScrollViewExpand;
            scrollView.OnEndScroll();
        }
        private void ScrollView_DraggingStarted(object sender, EventArgs e)
        { 
            var scrollView = this.Element as ScrollViewExpand;
            scrollView.OnBeginScroll();
        }

    }

}

