using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BJUTDUHelper.Model
{
    public class ScheduleItem
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int WeekDay { get; set; }
        public int BeginClass{get;set;}
        public int EndClass { get; set; }
        public int BeginWeek { get; set; }
        public int EndWeek { get; set; }
        public string Teacher { get; set; }
        public string Room { get; set; }
    }
    public class ScheduleModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }

        private int _currentWeek = 0;
        public int CurrentWeek
        {
            get { return _currentWeek; }
            set
            {
                _currentWeek = value;
                OnPropertyChanged();
            }
        }

        private int _selectedWeek = -1;
        public int SelectedWeek
        {
            get { return _selectedWeek; }
            set
            {
                _selectedWeek = value;
                OnPropertyChanged();
            }
        }

        private int _allWeek = 16;
        public int AllWeek
        {
            get { return _allWeek; }
            set
            {
                _allWeek = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ScheduleItem> _scheduleItemList;
        public ObservableCollection<ScheduleItem> ScheduleItemList
        {
            get { return _scheduleItemList; }
            set { _scheduleItemList = value; OnPropertyChanged(); }
        }
        
        public ObservableCollection<int> _weeks;
        public ObservableCollection<int> Weeks
        {
            get { return _weeks; }
            set { _weeks = value; OnPropertyChanged(); }
        }

        public void GetAllWeek()
        {
            var max= ScheduleItemList.Select(m => m.EndWeek).OrderByDescending(m => m).FirstOrDefault();
            AllWeek = max;
            var list = new ObservableCollection<int>();
            for(int i = 1; i <= max; i++)
            {
                list.Add(i);
            }
            Weeks = list;
        }

        //从html解析课表信息
        public static ObservableCollection<ScheduleItem> GetSchedule(string html)
        {
            ObservableCollection<ScheduleItem> list = new
                ObservableCollection<ScheduleItem>();
            int courseId = 1;
            var htmlParser = new HtmlParser();
            var doc=htmlParser.Parse(html);
            var table = doc.GetElementById("Table1");
            var trs = table.QuerySelectorAll("tr");
            for(int i = 2; i < trs.Count(); i++)
            {
                var tds = trs[i].QuerySelectorAll("td");
                var colCount = tds.Count();
                int  j= colCount;
                switch (colCount)
                {
                    case 9: j = 2; break;
                    case 8: j = 1; break;
                    default:break;

                }

                for(;j < tds.Count(); j++)
                {
                    var noprint = tds[j].GetAttribute("class");
                    if ("noprint" == noprint)
                    {
                        continue;
                    }
                    if (tds[j].InnerHtml!= "&nbsp;")
                    {
                        var str= tds[j].InnerHtml;
                        var items=ParseScheduleItem(str);

                        foreach(var item in items)
                        {
                            item.BeginClass = i - 1;
                            var rowspan = tds[j].GetAttribute("rowspan");
                            if (string.IsNullOrEmpty(rowspan))
                            {
                                item.EndClass = i - 1;
                            }
                            else
                            {
                                item.EndClass = item.BeginClass + int.Parse(rowspan) - 1;
                            }
                            if (tds.Count() == 8)
                            {
                                item.WeekDay = j;
                            }
                            if (tds.Count() == 9)
                                item.WeekDay = j - 1;


                            var reCourse=list.Where(m => m.CourseName == item.CourseName).FirstOrDefault();
                            if (reCourse == null)
                            {
                                item.CourseId = courseId++;
                            }
                            else
                            {
                                item.CourseId = reCourse.CourseId;
                            }
                            list.Add(item);
                            
                        }
                        
                    }
                }
            }
            return list;
        }
        
        //解析出课程名称，教师，教室等信息
        public static ScheduleItem[] ParseScheduleItem(string source)
        {
            if (!string.IsNullOrEmpty(source))
            {
                var strs = source.Split(new string[] { "<br>" }, StringSplitOptions.None);
                var count=strs.Count() / 4;
                ScheduleItem[] items = new ScheduleItem[count];
                for (int i = 0; i < count; i++)
                {
                    items[i] = new ScheduleItem();
                    items[i].CourseName = strs[i * 5 + 0];
                    items[i].Teacher = strs[i * 5 + 2];
                    items[i].Room = strs[i * 5 + 3];

                    var match = Regex.Match(strs[i * 5 + 1], "[0-9]+-[0-9]+");
                    var termStr = match.Value;
                    var newStrs = termStr.Split('-');
                    items[i].BeginWeek = int.Parse(newStrs[0]);
                    items[i].EndWeek = int.Parse(newStrs[1]);
                }
                return items;
            }
            return null;
            
        }
    }
}
