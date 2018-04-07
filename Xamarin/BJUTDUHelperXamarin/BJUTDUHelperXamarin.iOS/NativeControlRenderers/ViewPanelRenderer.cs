﻿using CoreGraphics;
using PivotPagePortable;
using PivotView;
using PivotView.iOS;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ViewPanel), typeof(ViewPanelRenderer))]
namespace PivotView.iOS
{
    public class ViewPanelRenderer : ScrollViewRenderer
    {
        private ViewPanel _viewPanel;
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            this.PagingEnabled = true;
            _viewPanel = this.Element as ViewPanel;
            _viewPanel.Select = Select;
            ((ScrollViewRenderer)this.NativeView).ShowsHorizontalScrollIndicator = false;
            ((ScrollViewRenderer)this.NativeView).DecelerationEnded += ScrollView_DecelerationEnded;
        }

        public void Select(int index, bool animate = true)
        {
            var perWidth = _viewPanel.Width;
            _viewPanel.CurrentIndex = index;
            _viewPanel.ScrollToAsync(index * perWidth, _viewPanel.ScrollY, animate);
        }

        private void ScrollView_DecelerationEnded(object sender, EventArgs e)
        {
            var index = (int)(_viewPanel.ScrollX / _viewPanel.Width);

            if (_viewPanel.Width / 2 < (_viewPanel.ScrollX % _viewPanel.Width))
            {
                index++;
            }

            _viewPanel.CurrentIndex = index;
            _viewPanel.OnSelectChanged();
        }

        private void ScrollView_DraggingStarted(object sender, EventArgs e)
        {

        }
    }
}
