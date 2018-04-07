using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BJUTDUHelperXamarin.Models
{
    public class RoomModel
    {
        public Region RoomRegion { get; set; }
        public Building Building { get; set; }

        public string RoomNumber { get; set; }
        public RoomType RoomType { get; set; }
    }
    public class RoomQueryModel
    {
        public Region Region { get; set; } =  Region.本部;
        public Building Building { get; set; }

        public DateTime Date { get; set; }
        public ClassTime Time { get; set; }
        public RoomType RoomType { get; set; } = RoomType.多媒体教室;
        public string SchoolYear { get; set; }
        public int Term { get; set; }
        public int Week { get; set; }

        public IDictionary<string,string> GetPparameters()
        {
            var dic = new Dictionary<string, string>();
            dic.Add("xiaoq", Convert.ToInt32(Region) +"");//本部1 通州2
            dic.Add("jslb", RoomType.ToString());
            dic.Add("min_zws", "0");
            dic.Add("max_zws", null);

            dic.Add("xqj", Date.DayOfWeek == 0 ? "7" : Convert.ToInt32(Date.DayOfWeek)+"");
            dic.Add("ddlDsz", Week/2==0?"双":"单");
            dic.Add("sjd", GetFormatClassTime());
            dic.Add("Button2", "空教室查询");
           
            dic.Add("xn", SchoolYear);
            dic.Add("xq", Term+"");
            dic.Add("ddlSyXn", SchoolYear);
            dic.Add("ddlSyxq", SchoolYear);

            dic.Add("kssj", Convert.ToInt32(Date.DayOfWeek).ToString()+ Week.ToString());
            dic.Add("jssj", Convert.ToInt32(Date.DayOfWeek).ToString() + Week.ToString());

            return dic;
        }

        private string GetFormatClassTime()
        {
            switch (Time)
            {
                case ClassTime.第1_2节:
                    return "'1'|'1','0','0','0','0','0','0','0','0'";
                case ClassTime.第3_4节:
                    return "'2'|'0','3','0','0','0','0','0','0','0'";
                case ClassTime.第5_6节:
                    return "'3'|'0','0','5','0','0','0','0','0','0'";
                case ClassTime.第7_8节:
                    return "'4'|'0','0','0','7','0','0','0','0','0'";
                case ClassTime.第9_10节:
                    return "'5'|'0','0','0','0','9','0','0','0','0'";
                case ClassTime.第11_12节:
                    return "'6'|'0','0','0','0','0','11','0','0','0'";;
                case ClassTime.上午:
                    return "'7'|'1','3','0','0','0','0','0','0','0'";;
                case ClassTime.下午:
                    return "'8'|'0','0','5','7','0','0','0','0','0'";;
                case ClassTime.晚上:
                    return "'9'|'0','0','0','0','9','11','0','0','0'";;
                case ClassTime.白天:
                    return "'10'|'1','3','5','7','0','0','0','0','0'";;
                case ClassTime.全天:
                    return "'11'|'1','3','5','7','9','11','0','0','0'";
                default:return null;
            }
        }
    }
    public enum ClassTime
    {
        第1_2节, 第3_4节,第5_6节,第7_8节,第9_10节,第11_12节,上午,下午,晚上,白天,全天
    }
    public enum Region
    {
        本部=1,通州校区=2
    }
    public enum Building
    {
        一教=0,三教,四教
    }
    public enum RoomType
    {
        多媒体教室=1,
        机房,
        普通教室,
        数字语音室,
        体育场地,
        同传室,
        艺设专教,
        语音室,
        制图教室,
        专用教室
    }
}
