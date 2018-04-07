using AngleSharp.Parser.Html;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BJUTDUHelperXamarin.Models
{
    public class ScheduleItem
    {
        public int ID { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int WeekDay { get; set; }
        public int BeginClass { get; set; }
        public int EndClass { get; set; }
        public int BeginWeek { get; set; }
        public int EndWeek { get; set; }
        public string Teacher { get; set; }
        public string Room { get; set; }
        public int? Parity { get; set; }
    }
    public class ScheduleModel
    {
        public int ID { get; set; }
        public int CurrentWeekIndex { get; set; } = 0;
        public int SelectedWeekIndex { get; set; } = 0;
        public int AllWeek { get; set; } = 16;

        public string ShcoolYear { get; set; }
        public int? Term { get; set; }

        [Ignore]
        public ObservableCollection<ScheduleItem> ScheduleItemList { get; set; }
        [Ignore]
        public List<int> Weeks { get; set; }

        
        public void GetAllWeek()
        {
            var max = ScheduleItemList.Select(m => m.EndWeek).OrderByDescending(m => m).FirstOrDefault();
            AllWeek = max;
            var list = new List<int>();
            for (int i = 1; i <= max; i++)
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
            var doc = htmlParser.Parse(html);
            var table = doc.GetElementById("Table1");
            var trs = table.QuerySelectorAll("tr");
            for (int i = 2; i <trs.Count(); i++)
            {
                
                var tds = trs[i].QuerySelectorAll("td");
                var colCount = tds.Count();

                int col = 0;
                if (i == 2 || i == 6 || i == 10)
                {
                    col = 1;
                }
                var begainClass = int.Parse(tds[col].InnerHtml.Split(new char[] { '第', '节' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                
                int j = 1;
                switch (colCount)
                {
                    case 9: j = 2; break;
                    case 8: j = 1; break;
                    default: break;

                }

                for (; j < tds.Count(); j++)
                {
                    var noprint = tds[j].GetAttribute("class");
                    if ("noprint" == noprint)
                    {
                        continue;
                    }
                    if (tds[j].InnerHtml != "&nbsp;")
                    {
                        var str = tds[j].InnerHtml;
                        var items = ParseScheduleItem(str);
                        int span = 1;
                        int.TryParse(tds[j].GetAttribute("RowSpan"),out span);
                        foreach (var item in items)
                        {
                            item.BeginClass = begainClass;
                            item.EndClass = item.BeginClass + span-1;
                            if (item.WeekDay == -1)
                            {
                                item.WeekDay = j - col;
                            }
                            var reCourse = list.Where(m => m.CourseName == item.CourseName).FirstOrDefault();
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

        

        public static ScheduleItem[] ParseScheduleItem(string source)
        {
            List<ScheduleItem> scheduleItems = new List<ScheduleItem>();
            try
            {
                if (!string.IsNullOrEmpty(source))
                {
                    var strs = source.Split(new string[] { "<br><br>" }, StringSplitOptions.None);
                    for (int i = 0; i < strs.Length; i++)
                    {
                        var scheduleItem = new ScheduleItem();
                        var items = strs[i].Split(new string[] { "<br>" }, StringSplitOptions.None);
                        for (int j = 0; j < items.Length; j++)
                        {
                            switch (j)
                            {
                                case 0:
                                    scheduleItem.CourseName = items[j];
                                    break;
                                case 1:
                                    //起始周
                                    var match = Regex.Match(items[j], "[0-9]+-[0-9]+");
                                    var termStr = match.Value;
                                    var newStrs = termStr.Split('-');
                                    scheduleItem.BeginWeek = int.Parse(newStrs[0]);
                                    scheduleItem.EndWeek = int.Parse(newStrs[1]);

                                    //单双周
                                    if (items[j].Contains("单周"))
                                    {
                                        scheduleItem.Parity = 1;
                                    }
                                    if (items[j].Contains("双周"))
                                    {
                                        scheduleItem.Parity = 0;
                                    }

                                    try
                                    {
                                        var str = items[j].Split('节')[0];
                                        var array = str.Split('第', ',');
                                        scheduleItem.WeekDay = ParseWeekDay(array[0]);
                                    }
                                    catch
                                    {
                                        scheduleItem.WeekDay = -1;
                                    }
                                    

                                    break;
                                case 2:
                                    scheduleItem.Teacher = items[j];
                                    break;
                                case 3:
                                    scheduleItem.Room = items[j];
                                    break;
                                default: break;
                            }
                        }
                        scheduleItems.Add(scheduleItem);
                    }
                    
                }
                return scheduleItems.ToArray();
            }
            catch
            {
                return scheduleItems.ToArray();
            }
        }

        public static int ParseWeekDay(string weekday)
        {
            switch (weekday)
            {
                case "周一":
                    return 1;
                case "周二":
                    return 2;
                case "周三":
                    return 3;
                case "周四":
                    return 4;
                case "周五":
                    return 5;
                case "周六":
                    return 6;
                case "周日":
                    return 7;
                default:
                    throw new Exception();
            }
        }

    }
}
