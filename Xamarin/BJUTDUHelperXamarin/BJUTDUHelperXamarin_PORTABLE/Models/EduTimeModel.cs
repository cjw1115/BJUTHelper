
using System;

namespace BJUTDUHelperXamarin.Models
{
    public class EduTimeModel
    {
        public DateTime CreateTime { get; set; }
        public string SchoolYear { get; set; }
        public int Term { get; set; }
        public int Week { get; set; }

        /// <summary>
        /// 根据选定的日期和当前日期差值，计算选定日期是低级教学周O要求给定日期大于等于当前日期)
        /// </summary>
        /// <param name="goalDate"></param>
        /// <returns></returns>
        public EduTimeModel ToEduTime(DateTime goalDate)
        {
            if (goalDate < DateTime.Now)
            {
                return this;
            }
            var finalWeek = Week;

            var allDays = (goalDate.Date - DateTime.Today).Days;
            if (allDays >= 7)
            {
                finalWeek = Week + allDays / 7;
            }
            else
            {
                if (DateTime.Today.DayOfWeek < goalDate.Date.DayOfWeek && goalDate.Date.DayOfWeek != DayOfWeek.Sunday)
                {
                    finalWeek= Week+ 1;
                }
                else
                {
                    finalWeek = Week;
                }
            }
            return new EduTimeModel() { CreateTime = CreateTime, SchoolYear = SchoolYear, Week = finalWeek, Term = Term };
        }
    }
}
