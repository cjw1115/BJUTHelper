﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PivotPagePortable
{
    public class CarouselView : ViewPanel
    {
        public CarouselView()
        {
            InnerPlay();
            this.SelectChanged += CarousalView_SelectChanged;
        }

        private void CarousalView_SelectChanged(object sender, SelectedPositionChangedEventArgs e)
        {
            if (realIndex == this.CurrentIndex)
                return;
            if ((int)e.SelectedPosition == this.Children.Count - 1)
            {
                Select(1, false);
                realIndex = 1;
                SelectedIndex = 0;
            }
            else if ((int)e.SelectedPosition == 0)
            {
                Select(this.Children.Count - 2, false);
                realIndex = this.Children.Count - 2;
                SelectedIndex = realIndex - 1;
            }
            else
            {
                realIndex = this.CurrentIndex;
            }
        }

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create("ItemTemplate", typeof(DataTemplate), typeof(CarouselView), defaultValue: default(DataTemplate));
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)this.GetValue(ItemTemplateProperty); }
            set { this.SetValue(ItemTemplateProperty, value); }
        }

        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create("ItemsSource", typeof(IList), typeof(CarouselView), propertyChanged: ItemsSourceChanged);
        public static void ItemsSourceChanged(BindableObject sender, object oldValue, object newValue)
        {
            var cv = sender as CarouselView;
            var items = newValue as IList;

            //cv.isPlaying = false;

            if (items == null || items.Count <= 0)
            {
                return;
            }
            if (cv.ItemTemplate == null)
            {
                throw new Exception("not bind value");
            }

            var views = new List<View>();
            var view = cv.ItemTemplate.CreateContent() as View;
            view.BindingContext = items[items.Count - 1];
            views.Add(view);
            for (int i = 0; i < items.Count; i++)
            {
                view = cv.ItemTemplate.CreateContent() as View;
                view.BindingContext = items[i];
                views.Add(view);
            }
            view = cv.ItemTemplate.CreateContent() as View;
            view.BindingContext = items[0];
            views.Add(view);

            if (cv.Children != null)
            {
                cv.Children.Clear();
            }
            cv.Children = views;


            cv.InvalidateLayout();
        }
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }
        public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create("SelectedIndex", typeof(int), typeof(CarouselView), defaultValue: 0);

        private int realIndex = 1;
        public async void Next()
        {
            if (this.ItemsSource == null || this.ItemsSource.Count <= 2)
                return;
            realIndex = (realIndex + 1) % (this.Children.Count);
            if (realIndex == this.Children.Count - 1)
            {
                Select(realIndex, true);
                await Task.Delay(500);
                Select(1, false);
                realIndex = 1;

                SelectedIndex = 0;
            }
            else
            {
                Select(realIndex, true);
                SelectedIndex = realIndex - 1;
            }
        }

        public async void Prev()
        {
            if (this.ItemsSource == null || this.ItemsSource.Count <= 2)
                return;

            realIndex = (realIndex - 1);
            if (realIndex < 0)
                realIndex = this.Children.Count - 1;

            if (realIndex == 0)
            {
                Select(realIndex, true);
                await Task.Delay(500);
                Select(this.Children.Count - 1, false);
                realIndex = this.Children.Count - 1;

                SelectedIndex = this.ItemsSource.Count - 1;
            }
            else
            {
                Select(realIndex, true);
                SelectedIndex = realIndex + 1;
            }
        }

        public bool AutoPlay
        {
            get { return (bool)GetValue(AutoPlayProperty); }
            set { SetValue(AutoPlayProperty, value); }
        }
        public static readonly BindableProperty AutoPlayProperty = BindableProperty.Create("AutoPlay", typeof(bool), typeof(CarouselView), defaultValue: true, propertyChanged: (o, oldValue, newValue) =>
        {
            var cv = o as CarouselView;
            if (cv.AutoPlay)
            {
                cv.InnerPlay();
            }
        });

        void InnerPlay()
        {
            if (!AutoPlay)
                return;
            Device.StartTimer(TimeSpan.FromMilliseconds(Duration), () =>
             {
                 bool re = false;
                 if (AutoPlay == false)
                 {
                     re= false;
                 }
                 else
                 {
                     re= true;
                 }
                 try
                 {
#if DEBUG
                     Debug.WriteLine($"定时线程：{Task.CurrentId}");
#endif

                     Next();
                 }
                 catch (Exception e)
                 {

                 }
                 return re;

             });
        }

        public int Duration
        {
            get { return (int)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }
        public static readonly BindableProperty DurationProperty = BindableProperty.Create("Duration", typeof(int), typeof(CarouselView), defaultValue: 3000);

        public object Item
        {
            get { return this.ItemsSource[SelectedIndex]; }
        }
    }
}
