using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using AngleSharp.Parser.Html;
using AngleSharp.Dom;
using AngleSharp;

namespace BJUTHelper
{
    public class LoginInfo
    {
        public object ob;
        public string ergs;
    }
    
    public class GDJWGL
    {
        public string StudentID { get; set; }

        public string StudentName { get; set; }

        public LoginInfo loginInfo { get; set; }

        public GradeChart gc { get; set; }

        public int flag { get; set; }

        private GDJWGL()
        {
            StudentName = string.Empty;
            flag = 0;
            loginInfo = new LoginInfo();
            gc = new GradeChart();
        }
        //单例设计模式，保证只能有一个GDJWGL的实例
        public static GDJWGL GDJWGLInit()
        {
            return new GDJWGL();
        }

        //获取登录学生姓名
        public string GetName(string html)
        {
            
            string Name = "";
            string specificStr = "xhxm\">";
            int start = html.IndexOf(specificStr);
            if (start <= 0)
                return Name;
            int i = 0;
           
            while (html[start + specificStr.Length + i] != '同')
            {
                Name += html[start + specificStr.Length + i++];
            }
            return Name;
        }
        
        public string GetName(Stream htmlStream)
        {
            var document = new HtmlParser(htmlStream);
            var element = document.Parse().GetElementById("xhxm");

            if (element == null)
                return string.Empty;
            string info = element.TextContent;
            string[] strs = info.Split(new string[] { " ", "同学" }, StringSplitOptions.RemoveEmptyEntries);

            if (strs.Length >= 1)
                StudentName = strs[1];
            else
                StudentName = string.Empty;
            return StudentName;
        }

        public string GetVIEWSTATE(string html)
        {
            string specifcStr = "__VIEWSTATE\" value=\"";
            string goal = "";
            int start=html.IndexOf("__VIEWSTATE\" value=\"");
            if (start <= 0)
                return goal;
            int i = 0;
            while (html[start + specifcStr.Length + i] != '"')
            {
                goal += html[start + specifcStr.Length + i++];
            }
            return goal;
        }

        public async Task<string> GetVIEWSTATE(Stream htmlStream)
        {
            //NSoup.Nodes.Document doc = NSoup.NSoupClient.Parse(html);
            //NSoup.Nodes.Element element = doc.GetElementById("Form1");
            //NSoup.Select.Elements elements = element.Select("input");
            var document = new HtmlParser(htmlStream);
            var docParse =  await document.ParseAsync();

            var emphasize = docParse.GetElementsByName("__VIEWSTATE");
            string str = emphasize[0].GetAttribute("value");
            //NSoup.Nodes.Element[] elementArry = elements.ToArray();
            //string str = elementArry[2].Attr("value");
            //return str;
            return str.Length > 0 ? str : string.Empty;
        }
        
        
    }
}
