using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace BJUTDUHelper.View
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BJUTEduScheduleView : Page
    {
        public ViewModel.BJUTEduScheduleVM BJUTEduScheduleVM { get; set; }
        private object navigationParam;
        public BJUTEduScheduleView()
        {
            this.InitializeComponent();
            this.Loaded += BJUTEduScheduleView_Loaded;
            var locator = Application.Current.Resources["Locator"] as ViewModel.ViewModelLocator;
            BJUTEduScheduleVM = locator.BJUTEduScheduleVM;
            
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationParam = e.Parameter;
        }
        private void BJUTEduScheduleView_Loaded(object sender, RoutedEventArgs e)
        {
            BJUTEduScheduleVM.Loaded(navigationParam);
        }

        
        public static void SetSchedule(DependencyObject o, Model.ScheduleModel value)
        {
            o.SetValue(ScheduleProperty, value);
        }
        public static Model.ScheduleModel GetSchedule(DependencyObject o)
        {
            return (Model.ScheduleModel)o.GetValue(ScheduleProperty);
        }
        public static readonly DependencyProperty ScheduleProperty = DependencyProperty.Register("Schedule", typeof(Model.ScheduleModel), typeof(BJUTEduScheduleView), new PropertyMetadata(null, (o, e) =>
         {
             var page = o as BJUTEduScheduleView;
             if (e.NewValue != null)
             {
                 page.ArrangeSchedule();
             }
         }));


        public List<Windows.UI.Color> BlockColors { get; set; } = new List<Windows.UI.Color>
        {   new Windows.UI.Color {A=180,R=190,G=60,B=253 },
            new Windows.UI.Color {A=180,R=29,G=145,B=255 },
            new Windows.UI.Color {A=180,R=50,G=180,B=52  },
            new Windows.UI.Color {A=180,R=255,G=153,B=35  },
            new Windows.UI.Color {A=180,R=250,G=97,B=152 },
 
            new Windows.UI.Color {A=180, R=66,G=218,B=240  },
            new Windows.UI.Color {A=180, R=74,G=165,B=85  },
            new Windows.UI.Color {A=180, R=125,G=83,B=90  },
            new Windows.UI.Color {A=180, R=125,G=40,B=200  },
                  
            new Windows.UI.Color {A=180, R=185,G=35,B=85  },
            new Windows.UI.Color {A=180, R=74,G=83,B=90  },
            new Windows.UI.Color {A=180, R=20,G=40,B=62  },
            new Windows.UI.Color {A=180, R=210,G=153,B=35  },
            new Windows.UI.Color {A=180, R=24,G=153,B=35  },

        };
        Random random = new Random();
        /// <summary>
        /// re paint the schedule
        /// </summary>
        private void ArrangeSchedule()
        {
            var schedule = GetSchedule(this);

            if (schedule == null|| schedule.ScheduleItemList==null)
                return;

            var list = gridSchedule.Children.Where(m => m.GetType() == typeof(Control.ScheduleBlock)).ToList();
            foreach (var item in list)
            {
                gridSchedule.Children.Remove(item);
            }

            for (int i = 0; i < schedule.ScheduleItemList.Count; i++)
            {

                var item = schedule.ScheduleItemList[i];
                if (item.BeginWeek <= schedule.SelectedWeek && item.EndWeek >= schedule.SelectedWeek)
                {
                    Control.ScheduleBlock block = new Control.ScheduleBlock();
                    //SolidColorBrush brush;
                    //var reBlock=gridSchedule.Children.Where(m => m.GetType() == typeof(Control.ScheduleBlock)).Where(m => ((Control.ScheduleBlock)m).Course.CourseId == item.CourseId).Select(m=>(Control.ScheduleBlock)m).FirstOrDefault();
                    //if (reBlock == null)
                    //{
                    //    brush = new SolidColorBrush() { Color = new Windows.UI.Color { G = (byte)random.Next(100,200), R = (byte)random.Next(0, 100), B = (byte)random.Next(100, 200), A = (byte)random.Next(100, 200) } };
                    //}
                    //else
                    //{
                    //    brush = reBlock.BlockBrush as SolidColorBrush;
                    //}
                    var brush = new SolidColorBrush(BlockColors[item.CourseId % BlockColors.Count]);
                    block.SetValue(Control.ScheduleBlock.BlockBrushProperty, brush);
                    block.SetValue(Grid.RowProperty, item.BeginClass);
                    block.SetValue(Grid.ColumnProperty, item.WeekDay);
                    block.SetValue(Grid.RowSpanProperty, item.EndClass - item.BeginClass + 1);

                    
                    block.HorizontalAlignment = HorizontalAlignment.Stretch;
                    block.Course = item;
                    gridSchedule.Children.Add(block);
                }
                
            }
        }
        private void lvWeek_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                
                var item = e.AddedItems[0];
                int itemValue = int.Parse(item.ToString());
                //if (itemValue== BJUTEduScheduleVM.Schedule.SelectedWeek)
                //{
                //    return;
                //}
                BJUTEduScheduleVM.Schedule.SelectedWeek = itemValue;

                var list = gridSchedule.Children.Where(m => (int)m.GetValue(Grid.RowProperty) >= 1 && (int)m.GetValue(Grid.ColumnProperty) >= 1 && m.GetType() == typeof(Control.ScheduleBlock)).ToList();
                foreach (var block in list)
                {
                    gridSchedule.Children.Remove(block);
                }
                
                ArrangeSchedule();
            }

        }

        #region 课表分享
        private RenderTargetBitmap scheduleBitpmap;
        private async void btnShare_Click(object sender, RoutedEventArgs e)
        {
            

            DataTransferManager transferMgr = DataTransferManager.GetForCurrentView();
            transferMgr.DataRequested += TransferMgr_DataRequested;

            DataTransferManager.ShowShareUI();


        }

        private async void TransferMgr_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var deferral = args.Request.GetDeferral();
            DataPackage package = args.Request.Data;
            package.Properties.Title = "晒课表";

            scheduleBitpmap = new RenderTargetBitmap();
            await scheduleBitpmap.RenderAsync(scheduleGrid);
            var buffer = await scheduleBitpmap.GetPixelsAsync();

            InMemoryRandomAccessStream ras = new InMemoryRandomAccessStream();
            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, ras);

            using (Stream stream = buffer.AsStream())
            {
                byte[] pixels = new byte[stream.Length];
                await stream.ReadAsync(pixels, 0, pixels.Length);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                          (uint)scheduleBitpmap.PixelWidth,
                          (uint)scheduleBitpmap.PixelHeight,
                          96.0,
                          96.0,
                          pixels);
            }
            await encoder.FlushAsync();


            var streamRef = RandomAccessStreamReference.CreateFromStream(ras);
            package.SetBitmap(streamRef);
            deferral.Complete();
        }
        #endregion

        
    }
    public class IntSelectedItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (int)value;
        }
    }
}
