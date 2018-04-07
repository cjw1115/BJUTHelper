//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Xamarin.Forms;

//namespace BJUTDUHelperXamarin.Controls
//{
//    public class MyCarouselView : CarouselView
//    {
//        #region CarouselView轮播

//        public static readonly BindableProperty AutoPlayProperty = BindableProperty.Create("AutoPlay", typeof(bool?), typeof(MyCarouselView), null, propertyChanged: AutoPlayBindingPropertyChanged);
//        public bool? AutoPlay
//        {
//            get { return (bool?)this.GetValue(AutoPlayProperty); }
//            set
//            {
//                SetValue(AutoPlayProperty, value);
//            }
//        }
//        //public static void SetAutoPlay(BindableObject o, bool? value)
//        //{
//        //    o.SetValue(AutoPlayProperty, value);
//        //}
//        //public static bool? GetAutoPlay(BindableObject o)
//        //{
//        //    return (bool?)o.GetValue(AutoPlayProperty);
//        //}


//        private static int _itemsCount = 0;
//        private static CarouselView _cv;
//        public static void AutoPlayBindingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
//        {
//            if ((bool)newValue == true)
//            {
//                CarouselView cv = bindable as CarouselView;
//                _cv = cv;

//                double period = 3000;
//                Device.StartTimer(System.TimeSpan.FromMilliseconds(period), () =>
//                {
//                    IList list = _cv.ItemsSource as IList;
//                    _itemsCount = list.Count;
//                    if (_itemsCount == 0)
//                        return false;
//                    _cv.Position = (_cv.Position + 1) % _itemsCount;

//                    var re = (bool)_cv.GetValue(AutoPlayProperty);
//                    return re;
//                });
//            }
//        }

//        #endregion
//    }
//}
