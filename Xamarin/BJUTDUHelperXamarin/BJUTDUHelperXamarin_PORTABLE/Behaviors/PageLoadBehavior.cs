using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Behaviors
{
    public class PageLoadBehavior:Behavior<Page>
    {
        public static readonly BindableProperty CommandProperty =
   BindableProperty.Create("Command", typeof(ICommand), typeof(PageLoadBehavior));
      
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public Page AssociatedObject { get; private set; }

        protected override void OnAttachedTo(Page bindable)
        {
            base.OnAttachedTo(bindable);
            AssociatedObject = bindable;
            bindable.BindingContextChanged += OnBindingContextChanged;
            bindable.Appearing += Bindable_Appearing;
        }

        private void Bindable_Appearing(object sender, EventArgs e)
        {
            if (Command == null)
            {
                return;
            }
            if (Command.CanExecute(null))
            {
                Command.Execute(null);
            }
        }

        protected override void OnDetachingFrom(Page bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.BindingContextChanged -= OnBindingContextChanged;
            //bindable.Appearing -= Bindable_Appearing;
            AssociatedObject = null;
        }
        void OnBindingContextChanged(object sender, EventArgs e)
        {
            OnBindingContextChanged();
        }
        
        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            BindingContext = AssociatedObject.BindingContext;
        }
    }
}
