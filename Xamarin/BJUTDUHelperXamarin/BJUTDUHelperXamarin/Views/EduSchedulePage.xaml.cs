using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace BJUTDUHelperXamarin.Views
{
    public partial class EduSchedulePage : ContentPage
    {
        public EduSchedulePage()
        {
            InitializeComponent();
            pickerWeek.SelectedIndexChanged += PickerWeek_SelectedIndexChanged;
        }

       

        public static void SetSchedule(BindableObject o, Models.ScheduleModel value)
        {
            o.SetValue(ScheduleProperty, value);
        }
        public static Models.ScheduleModel GetSchedule(BindableObject o)
        {
            return (Models.ScheduleModel)o.GetValue(ScheduleProperty);
        }
        public static readonly BindableProperty ScheduleProperty = BindableProperty.CreateAttached("Schedule", typeof(Models.ScheduleModel), typeof(Views.EduSchedulePage), null,propertyChanged:(o,oldValue,newValue ) =>
        {
            var page = o as Views.EduSchedulePage;
            var value = newValue as Models.ScheduleModel;
            if (value != null)
            {
                //page.ArrangeSchedule();
            }
        });

        public List<Color> BlockColors { get; set; } = new List<Color>
        {   new Color (190/256d,60/256d ,253/256d,180/256d),
            new Color (29/256d, 145/256d,255/256d,180/256d),
            new Color (50/256d, 180/256d,52 /256d,180/256d),
            new Color (255/256d,153/256d,35 /256d, 180/256d),
            new Color (250/256d,97 /256d,152/256d, 180/256d),
                                      
            new Color (66 /256d,218/256d,240/256d ,180/256d),
            new Color (74 /256d,165/256d,85 /256d ,180/256d),
            new Color (125/256d,83 /256d,90 /256d ,180/256d),
            new Color (125/256d,40 /256d,200/256d ,180/256d),
                            
            new Color (185/256d,35 /256d,85 /256d ,180 /256d),
            new Color (74 /256d,83 /256d,90 /256d  ,180/256d),
            new Color (20 /256d,40 /256d,62 /256d  ,180/256d),
            new Color (210/256d,153/256d,35 /256d,180/256d),
            new Color (24 /256d,153/256d,35 /256d,180/256d)

        };
        Random random = new Random();
        /// <summary>
        /// re paint the schedule
        /// </summary>
        private void ArrangeSchedule()
        {
            var schedule = GetSchedule(this);

            if (schedule == null || schedule.ScheduleItemList == null)
                return;

            var list = gridSchedule.Children.Where(m => m.GetType() == typeof(StackLayout)).ToList();
            foreach (var item in list)
            {
                gridSchedule.Children.Remove(item);
            }

            for (int i = 0; i < schedule.ScheduleItemList.Count; i++)
            {

                var item = schedule.ScheduleItemList[i];
                if (item.BeginWeek <= schedule.SelectedWeekIndex+1 && item.EndWeek >= schedule.SelectedWeekIndex+1)
                {
                    if (item.Parity != null)
                    {
                        if ((schedule.SelectedWeekIndex + 1) % 2 != item.Parity)
                        {
                            continue;
                        }
                    }
                    StackLayout layout = new StackLayout();
                    layout.Orientation = StackOrientation.Vertical;

                    layout.BackgroundColor = BlockColors[item.CourseId % BlockColors.Count];

                    layout.SetValue(Grid.RowProperty, item.BeginClass);
                    layout.SetValue(Grid.ColumnProperty, item.WeekDay);
                    layout.SetValue(Grid.RowSpanProperty, item.EndClass - item.BeginClass + 1);


                    layout.HorizontalOptions = LayoutOptions.FillAndExpand;
                    layout.BindingContext = item;

                    Label title = new Label { Text = item.CourseName , FontSize=10,TextColor=new Color(1,1,1),HorizontalOptions=LayoutOptions.CenterAndExpand,HorizontalTextAlignment= TextAlignment.Center };
                    Label room = new Label { Text = item.Room, FontSize = 8, TextColor = new Color(1, 1, 1), HorizontalOptions = LayoutOptions.CenterAndExpand, HorizontalTextAlignment = TextAlignment.Center } ;

                    layout.Children.Add(title);
                    layout.Children.Add(room);
                    

                    var binding = new Binding();
                    binding.Path = "ItemClickedCommand";
                    binding.Mode = BindingMode.OneWay;

                    layout.BindingContext = this.BindingContext;
                    layout.SetBinding(ClickedCommandProperty, binding);

                    
                    layout.GestureRecognizers.Add(new TapGestureRecognizer { Command = (ICommand)layout.GetValue(ClickedCommandProperty), CommandParameter = item });

                    gridSchedule.Children.Add(layout);
                }

            }
        }
        private void PickerWeek_SelectedIndexChanged(object sender, EventArgs e)
        {
            var list = gridSchedule.Children.Where(m => (int)m.GetValue(Grid.RowProperty) >= 1 && (int)m.GetValue(Grid.ColumnProperty) >= 1 && m.GetType() == typeof(StackLayout)).ToList();
            foreach (var block in list)
            {
                gridSchedule.Children.Remove(block);
            }

            var index = pickerWeek.SelectedIndex;
            if (index == -1)
                return;
            ArrangeSchedule();
              
        }
       
       
        public ICommand ClickedCommand
        {
            get { return (ICommand)GetValue(ClickedCommandProperty); }
            set { SetValue(ClickedCommandProperty, value); }
        }
        public static readonly BindableProperty ClickedCommandProperty = BindableProperty.Create("ClickedCommand", typeof(ICommand), typeof(StackLayout),null);

    }
}
