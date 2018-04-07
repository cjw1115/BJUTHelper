//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Input;
//using Xamarin.Forms;

//namespace BJUTDUHelperXamarin.Behaviors
//{
//    public class CarouselViewSelectedItemBehavior : Behavior<CarouselView>
//    {
//        public static readonly BindableProperty CommandProperty =
//   BindableProperty.Create("Command", typeof(ICommand), typeof(CarouselViewSelectedItemBehavior), null);
        
//        public ICommand Command
//        {
//            get { return (ICommand)GetValue(CommandProperty); }
//            set { SetValue(CommandProperty, value); }
//        }

//        public CarouselView AssociatedObject { get; private set; }

//        protected override void OnAttachedTo(CarouselView bindable)
//        {
//            base.OnAttachedTo(bindable);
//            AssociatedObject = bindable;
//            bindable.BindingContextChanged += OnBindingContextChanged;
//            bindable.ItemSelected += OnListViewItemSelected;
//        }

//        protected override void OnDetachingFrom(CarouselView bindable)
//        {
//            base.OnDetachingFrom(bindable);
//            bindable.BindingContextChanged -= OnBindingContextChanged;
//            bindable.ItemSelected -= OnListViewItemSelected;
//            AssociatedObject = null;
//        }
//        void OnBindingContextChanged(object sender, EventArgs e)
//        {
//            OnBindingContextChanged();
//        }

//        void OnListViewItemSelected(object sender, SelectedItemChangedEventArgs e)
//        {
//            if (Command == null)
//            {
//                return;
//            }
//            if (e.SelectedItem == null)
//                return;

          
//            if (Command.CanExecute(e.SelectedItem))
//            {
//                Command.Execute(e.SelectedItem);
//            }
//        }

//        protected override void OnBindingContextChanged()
//        {
//            base.OnBindingContextChanged();
//            BindingContext = AssociatedObject.BindingContext;
//        }
//    }
//}
