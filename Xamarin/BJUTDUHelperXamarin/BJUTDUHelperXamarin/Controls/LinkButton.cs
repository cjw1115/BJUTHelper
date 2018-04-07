using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Controls
{
    public class LinkButton:Button
    {
        public LinkButton()
        {
            this.BorderColor =Color.Transparent;
            this.BorderRadius = 0;
            this.BorderWidth = 0;
            this.MinimumHeightRequest = 4;
            this.MinimumWidthRequest = 4;
            this.BackgroundColor = Color.Transparent;
        }

    }
}
