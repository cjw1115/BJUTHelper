//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Input;
//using Xamarin.Forms;
//using static Xamarin.Forms.BindableProperty;

//namespace BJUTDUHelperXamarin.Behaviors
//{
//    public class TapedCommandBehavior : Behavior<CarouselView>
//    {
//        public static readonly BindableProperty TappedCommandProperty = BindableProperty.Create("TappedCommand",typeof(ICommand), typeof(TapedCommandBehavior),null,BindingMode.OneWay);
//        public ICommand TappedCommand
//        {
//            get { return (ICommand)this.GetValue(TappedCommandProperty); }
//            set
//            {
//                SetValue(TappedCommandProperty, value);
//                //OnPropertyChanged();
//            }
//        }
//        private TapGestureRecognizer tapGestureRecognizer;
//        private CarouselView _carouselView;
//        protected override void OnAttachedTo(BindableObject bindable)
//        {
//            base.OnAttachedTo(bindable);

//            var view = bindable as CarouselView;
//            _carouselView = view;
//            _carouselView.BindingContextChanged += _carouselView_BindingContextChanged;
            
//            tapGestureRecognizer = new TapGestureRecognizer()/*{ Command = TappedCommand, CommandParameter = view.Item }*/;
//            tapGestureRecognizer.Tapped += TapGestureRecognizer_Tapped;
//            view.GestureRecognizers.Add(tapGestureRecognizer);
//        }

//        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
//        {
//            TappedCommand?.Execute(_carouselView.Item);
//        }

       
//        private void _carouselView_BindingContextChanged(object sender, EventArgs e)
//        {
//            OnBindingContextChanged();
//        }

//        protected override void OnDetachingFrom(BindableObject bindable)
//        {
//            base.OnDetachingFrom(bindable);

//            var view = bindable as CarouselView;
//            view.GestureRecognizers.Remove(tapGestureRecognizer);
           
//        }
       

//        protected override void OnBindingContextChanged()
//        {
//            base.OnBindingContextChanged();
//            this.BindingContext = _carouselView.BindingContext;
//        }
//    }

//}
