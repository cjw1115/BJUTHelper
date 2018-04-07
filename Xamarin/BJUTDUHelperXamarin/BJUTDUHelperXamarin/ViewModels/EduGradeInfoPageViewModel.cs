﻿

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Prism.Commands;
using Prism.Mvvm;


namespace BJUTDUHelperXamarin.ViewModels
{
    public class PivotItemModel
    {
        public string Title { get; set; }
        public Lazy<View> View { get; set; }
    }
    public class EduGradeInfoPageViewModel
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private ObservableCollection<PivotItemModel> _pivotItems;
        public ObservableCollection<PivotItemModel> PivotItems
        {
            get { return _pivotItems; }
            set { _pivotItems = value; OnPropertyChanged(); }
        }

        private ObservableCollection<PivotItemModel> _headers;
        public ObservableCollection<PivotItemModel> Headers
        {
            get { return _headers; }
            set { _headers = value; OnPropertyChanged(); }
        }
        private ObservableCollection<View> _views;
        public ObservableCollection<View> Views
        {
            get { return _views; }
            set { _views = value; OnPropertyChanged(); }
        }

        private static EduGradeInfoPageViewModel _eduGradeInfoVM;
        public static EduGradeInfoPageViewModel EduGradeInfoVM
        {
            get
            {
                if (_eduGradeInfoVM==null)
                {
                    _eduGradeInfoVM = new EduGradeInfoPageViewModel();
                    
                }
                return _eduGradeInfoVM;
            }
        }

        public EduGradeInfoPageViewModel()
        {
            Views = new ObservableCollection<View>();
            Headers = new ObservableCollection<PivotItemModel>();

            LoadData();
        }
        public  void LoadData()
        {
            Headers.Add(new PivotItemModel { Title = "加权" });
            Views.Add(new Views.EduGradeWeightView());

            Headers.Add(new PivotItemModel { Title = "GPA" });
            Views.Add(new Views.EduGradeGpaView());

        }
    }
}
