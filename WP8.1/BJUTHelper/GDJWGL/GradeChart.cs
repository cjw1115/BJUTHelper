using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;

namespace BJUTHelper
{
    public class TRData : INotifyPropertyChanged
    {
        private string _schoolYear, _term, _subject, _credit, _subjectType, _score, _reScore, _reLearnScore;

        public string Credit
        {
            get
            {
                return _credit;
            }

            set
            {
                _credit = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Creadit"));
            }
        }

        public string ReLearnScore
        {
            get
            {
                return _reLearnScore;
            }

            set
            {
                _reLearnScore = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ReLearnScore"));
            }
        }

        public string ReScore
        {
            get
            {
                return _reScore;
            }

            set
            {
                _reScore = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ReScore"));
            }
        }

        public string SchoolYear
        {
            get { return _schoolYear; }
            set
            {
                _schoolYear = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SchoolYear"));
            }
        }

      
        public string Score
        {
            get
            {
                return _score;
            }

            set
            {
                _score = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Score"));
            }
        }

        public string Subject
        {
            get
            {
                return _subject;
            }

            set
            {
                _subject = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Subject"));
            }
        }

        public string Term
        {
            get
            {
                return _term;
            }

            set
            {
                _term = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Term"));
            }
        }

        public string SubjectType
        {
            get
            {
                return _subjectType;
            }

            set
            {
                _subjectType = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SubjectType"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)

        {

            if (this.PropertyChanged != null) this.PropertyChanged(this, e);

        }
    }

    public class GradeChart : ObservableCollection<TRData>
    {
        List<TRData> MainList;
        public List<string> schoolYearList { get; set; }
        public List<string> termList { get; set; }

        public GradeChart()
        {
            
            MainList = new List<TRData>();

            schoolYearList = new List<string>();
            schoolYearList.Add("历年成绩");

            termList = new List<string>();
        }
        public void AddRecord(string schoolYear, string term, string subject, string credit, string score, string reScore, string reLearnScore,string _subjectType)
        {
            var temp = new TRData() { SchoolYear = schoolYear, Term = term, Subject = subject, Credit = credit, Score = score, ReScore = reScore, ReLearnScore = reLearnScore, SubjectType=_subjectType };
            Add(temp);
            //向主表添加成绩
            MainList.Add(temp);
            if (!schoolYearList.Contains(schoolYear))
                schoolYearList.Add(schoolYear);
            //过滤出所有的学年和学期
            if (!termList.Contains(term))
                termList.Add(term);
        }
        public void GetGradeChart(string html)
        {
            MainList?.Clear();
            int start = html.IndexOf("<table class=\"datelist\"");
            int end = html.IndexOf("</table>", start);
            char[] charScr = new char[html.Length/3];
            html.CopyTo(start, charScr, 0, end - start + 8);
            //NSoup.Nodes.Document doc = NSoup.NSoupClient.Parse(html);
            //NSoup.Nodes.Element element = doc.GetElementById("Datagrid1");
            //NSoup.Select.Elements trs = element.Select("tr");
            StringBuilder sb = new StringBuilder();
            sb.Append(charScr);

            var document = new HtmlParser(sb.ToString());
            
            var emphasize = document.Parse().GetElementById("Datagrid1");
            var trs = emphasize.QuerySelectorAll("tr");
            string score, rescore, relearn;
            for (int i = 1; i < trs.Length; i++)
            {

                var tds = trs[i].QuerySelectorAll("td");
                //学校空格为“&nbsp”；
                //如果存在补考成绩或者重修成绩，就将成绩置为重修成绩
                score = tds[8].InnerHtml == "&nbsp;" ? "" : tds[8].InnerHtml;
                rescore = tds[10].InnerHtml == "&nbsp;" ? "" : tds[10].InnerHtml;
                relearn = tds[11].InnerHtml == "&nbsp;" ? "" : tds[11].InnerHtml;
                if (rescore.Length > 0)
                    score = rescore;
                if (relearn.Length > 0)
                    score = relearn;
                AddRecord(tds[0].InnerHtml == "&nbsp;" ? "" : tds[0].InnerHtml,
                    tds[1].InnerHtml == "&nbsp;" ? "" : tds[1].InnerHtml,
                    tds[3].InnerHtml == "&nbsp;" ? "" : tds[3].InnerHtml,
                    tds[6].InnerHtml == "&nbsp;" ? "" : tds[6].InnerHtml,
                    score, rescore, relearn, tds[5].InnerHtml == "&nbsp;" ? "" : tds[5].InnerHtml
                    );

            }
        }
        public string GetWeightAvarageScore()//计算加权成绩
        {   
            double sum = 0;
            double weights = 0;
            foreach (TRData item in this)
            {
                if (item.SubjectType == "新生研讨课"||item.SubjectType=="第二课堂")
                    continue;
                sum += Convert.ToDouble(item.Score) * Convert.ToDouble(item.Credit);
                weights += Convert.ToDouble(item.Credit);
            }
            return "加权成绩：" + (Convert.ToString((sum / weights).ToString("#.00")));
            
        }
        public void GetSpecificGradeChart(string schoolYear, string term)
        {
            //清空当前显示的成绩表
            this.Clear();
            
            foreach (var item in MainList)
            {
                if (schoolYear != null)
                {
                    if (schoolYear == "历年成绩")
                    {
                        this.Add(item);
                    }
                    else
                    {
                        if(item.SchoolYear == schoolYear)
                            this.Add(item);
                    }
                }
            }
            if (term != null)
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i].Term != term)
                    {
                        this.RemoveAt(i);
                        i--;
                    }
                }
            }
            
        }
    }
}
