using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace BJUTDUHelper.Control
{
    public sealed partial class ScheduleBlock : UserControl
    {
        public ScheduleBlock()
        {
            this.InitializeComponent();

            
        }
        public Model.ScheduleItem Course
        {
            get { return (Model.ScheduleItem)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Course", typeof(Model.ScheduleItem), typeof(ScheduleBlock), new PropertyMetadata(null,(o,e)=> 
        {
            var block = o as ScheduleBlock;
            block.mainGrid.DataContext = block.Course;
        }));
        public Brush BlockBrush
        {
            get { return (Brush)GetValue(BlockBrushProperty); }
            set { SetValue(BlockBrushProperty, value); }
        }
        public static readonly DependencyProperty BlockBrushProperty = DependencyProperty.Register("BlockBrush", typeof(Brush), typeof(ScheduleBlock), new PropertyMetadata(null));

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }
    }
}
