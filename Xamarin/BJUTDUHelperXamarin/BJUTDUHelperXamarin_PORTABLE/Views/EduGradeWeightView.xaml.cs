using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
namespace BJUTDUHelperXamarin.Views
{
    
    public partial class EduGradeWeightView : ContentView
    {
        public EduGradeWeightView()
        {
            InitializeComponent();

            
        }

        public static readonly BindableProperty ValueChangedCommandProperty = BindableProperty.CreateAttached("ValueChangedCommand", typeof(ICommand), typeof(Slider),null);
        public static ICommand GetValueChangedCommand(BindableObject o)
        {
            return (ICommand)o.GetValue(ValueChangedCommandProperty);
        }
        public static void SetValueChangedCommand(BindableObject o,ICommand value)
        {
            o.SetValue(ValueChangedCommandProperty, value);
        }


        public static readonly BindableProperty DeleteCommandProperty = BindableProperty.CreateAttached("DeleteCommand", typeof(ICommand), typeof(ListView), null);
        public static ICommand GetDeleteCommand(BindableObject o)
        {
            return (ICommand)o.GetValue(DeleteCommandProperty);
        }
        public static void SetDeleteCommand(BindableObject o, ICommand value)
        {
            o.SetValue(DeleteCommandProperty, value);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var command=GetDeleteCommand(this.gradeList);
            var button = sender as Button;
            command.Execute(button.CommandParameter);
        }
    }
}
