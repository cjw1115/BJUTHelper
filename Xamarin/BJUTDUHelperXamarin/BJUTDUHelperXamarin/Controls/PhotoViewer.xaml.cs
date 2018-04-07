using PivotPagePortable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BJUTDUHelperXamarin.Controls
{
    public partial class PhotoViewer : ContentPage
    {
        private ViewPanel _viewPanel;
        public PhotoViewer()
        {
            InitializeComponent();
            this.btnBack.Clicked += BtnBack_Clicked;
            this.Appearing += PhotoViewer_Appearing;
            _viewPanel = this.photoViewer;
            _viewPanel.SelectChanged += _viewPanel_SelectChanged;
        }

        private async void BtnBack_Clicked(object sender, EventArgs e)
        {
            await this.Navigation.PopModalAsync(true);
        }

        public IList<String> Photos { get; set; }
        public int SelectedIndex { get; set; }
        private void PhotoViewer_Appearing(object sender, EventArgs e)
        {

            var images = new List<View>();
            if(Photos==null)
            {
                return;
            }
            foreach (var item in Photos)
            {
                images.Add(CreateImage(item));
            }
            Views = images;
            if (SelectedIndex  >= images.Count)
            {
                SelectedIndex = 0;
            }
            this.labelCount.Text = images.Count.ToString();
            this.labelIndex.Text = (SelectedIndex + 1).ToString() ;
            _viewPanel.Select(SelectedIndex,false);
        }
        private Image CreateImage(string path)
        {
            Controls.ImageViewExpand image = new Controls.ImageViewExpand();
            image.Source = UriImageSource.FromUri(new Uri(path));
            return image;
        }
        private void _viewPanel_SelectChanged(object sender, SelectedPositionChangedEventArgs e)
        {
            var index = (int)e.SelectedPosition;
            this.SelectedIndex = index;
            this.labelIndex.Text = (SelectedIndex + 1).ToString();
        }


        /// <summary>
        /// PivotPage第二组成部分Views
        /// </summary>
        public static readonly BindableProperty ViewsProperty = BindableProperty.Create("Views", typeof(IEnumerable), typeof(PhotoViewer), null, propertyChanged: OnViewsPropertyChnaged);
        public IEnumerable Views
        {
            get { return (IEnumerable)this.GetValue(ViewsProperty); }
            set { SetValue(ViewsProperty, value); }
        }

        static void OnViewsPropertyChnaged(BindableObject sender, object oldValue, object newValue)
        {
            var pivot = sender as PhotoViewer;
            pivot._viewPanel.Children = (IList)newValue;
        }
    }
}