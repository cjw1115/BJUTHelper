using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Parser.Html;

namespace BJUTDUHelper.Model
{
    public class TRData : INotifyPropertyChanged
    {
        private string _schoolYear, _term, _subject, _credit, _subjectType, _score,_secondFlag, _reScore, _reLearnScore;

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
        public string SecondFlag
        {
            get
            {
                return _secondFlag;
            }

            set
            {
                _secondFlag = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SecondFlag"));
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
        public ObservableCollection<TRData> MainList { get; set; }
        public ObservableCollection<string> SchoolYearList { get; set; }
        public ObservableCollection<string> TermList { get; set; }

        public GradeChart()
        {

            MainList = new ObservableCollection<TRData>();

            SchoolYearList = new ObservableCollection<string>();
            SchoolYearList.Add("历年成绩");

            TermList = new ObservableCollection<string>();
        }
        public void InitList()
        {
            
            if (SchoolYearList == null)
            {
                SchoolYearList = new ObservableCollection<string>();
            }
            if (TermList == null)
            {
                TermList = new ObservableCollection<string>();
            }
            foreach (var item in this)
            {
                MainList.Add(item);

                if (!SchoolYearList.Contains(item.SchoolYear))
                    SchoolYearList.Add(item.SchoolYear);
                //过滤出所有的学年和学期
                if (!TermList.Contains(item.Term))
                    TermList.Add(item.Term);
            }
        }

        public void AddRecord(string schoolYear, string term, string subject, string credit, string score, string reScore,string secondFlag, string reLearnScore, string _subjectType)
        {
            var temp = new TRData() { SchoolYear = schoolYear, Term = term, Subject = subject, Credit = credit, Score = score,SecondFlag=secondFlag, ReScore = reScore, ReLearnScore = reLearnScore, SubjectType = _subjectType };
            Add(temp);
            //向主表添加成绩
            MainList.Add(temp);
            if (!SchoolYearList.Contains(schoolYear))
                SchoolYearList.Add(schoolYear);
            //过滤出所有的学年和学期
            if (!TermList.Contains(term))
                TermList.Add(term);
        }
        public void GetGradeChart(string html)
        {
            try
            {
                MainList.Clear();//清空主表信息
                this?.Clear();//清空全部成绩信息
                int start = html.IndexOf("<table class=\"datelist\"");
                int end = html.IndexOf("</table>", start);
                char[] charScr = new char[html.Length / 3];
                html.CopyTo(start, charScr, 0, end - start + 8);
                //NSoup.Nodes.Document doc = NSoup.NSoupClient.Parse(html);
                //NSoup.Nodes.Element element = doc.GetElementById("Datagrid1");
                //NSoup.Select.Elements trs = element.Select("tr");
                StringBuilder sb = new StringBuilder();
                sb.Append(charScr);

                var parser = new HtmlParser();
                var document = parser.Parse(sb.ToString());

                var emphasize = document.GetElementById("Datagrid1");
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
                        score, rescore,
                        tds[9].InnerHtml == "&nbsp;" ? "" : tds[9].InnerHtml,
                        relearn, tds[5].InnerHtml == "&nbsp;" ? "" : tds[5].InnerHtml
                        );
                }
            }
            catch
            {
                throw;
            }
            
        }
        public string GetWeightAvarageScore()//计算加权成绩
        {
            double sum = 0;
            double weights = 0;
            foreach (TRData item in MainList)
            {
                if (item.SubjectType == "新生研讨课" || item.SubjectType == "第二课堂"||item.SecondFlag=="1")
                    continue;
                sum += Convert.ToDouble(item.Score) * Convert.ToDouble(item.Credit);
                weights += Convert.ToDouble(item.Credit);
            }
            return "加权成绩：" + (Convert.ToString((sum / weights).ToString("#.000")));

        }
        public void GetSpecificGradeChart(string schoolYear, string term)
        {
            //清空当前显示的成绩表
            MainList.Clear();

            foreach (var item in this)
            {
                if (schoolYear != null)
                {
                    if (schoolYear == "历年成绩")
                    {
                        MainList.Add(item);
                    }
                    else
                    {
                        if (item.SchoolYear == schoolYear)
                            MainList.Add(item);
                    }
                }
            }
            if (term != null&&term!="-1")
            {
                for (int i = 0; i < MainList.Count; i++)
                {
                    if (MainList[i].Term != term)
                    {
                        MainList.RemoveAt(i);
                        i--;
                    }
                }
            }

        }

    }
}
