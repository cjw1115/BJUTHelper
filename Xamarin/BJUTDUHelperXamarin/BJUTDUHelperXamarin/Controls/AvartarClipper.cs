using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Controls
{
    public interface IClip
    {
        Task<string> ClipImage();
        
    }
    public class AvartarClipper:View
    {
        public IClip Clipper { get; set; }
        public static readonly BindableProperty PathProperty = BindableProperty.Create("Path", typeof(string), typeof(AvartarClipper), null);
        public string Path
        {
            get => GetValue(PathProperty) as string;
            set => SetValue(PathProperty, value);
        }

        public async Task<string> ClipImage()
        {
            if (Clipper != null)
            {
                var re=await Clipper.ClipImage();
                return re;
            }
            return null;
        }
    }
}
