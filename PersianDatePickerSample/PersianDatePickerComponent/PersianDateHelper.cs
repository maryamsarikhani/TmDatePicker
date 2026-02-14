using System.Globalization;

namespace TMPersianDatePicker.PersianDatePickerComponent;

public static class PersianDateHelper
{
    private static PersianCalendar pc = new();
    public static string[] WeekNames => new[] { "ش", "ی", "د", "س", "چ", "پ", "ج" }; // Fixed syntax for array initialization.

    public static int GetWeekSpan(this DayOfWeek week)
    {
        return week switch
        {
            DayOfWeek.Saturday => 0,
            DayOfWeek.Sunday => 1,
            DayOfWeek.Monday => 2,
            DayOfWeek.Tuesday => 3,
            DayOfWeek.Wednesday => 4,
            DayOfWeek.Thursday => 5,
            DayOfWeek.Friday => 6,
            _ => 0 // Default value remains the same.
        };
    }

    public static string GetMonthName(this int month) =>
        month switch
        {
            1 => "فروردین",
            2 => "اردیبهشت",
            3 => "خرداد",
            4 => "تیر",
            5 => "مرداد",
            6 => "شهریور",
            7 => "مهر",
            8 => "آبان",
            9 => "آذر",
            10 => "دی",
            11 => "بهمن",
            12 => "اسفند",
            _ => "نامشخص", // Default value for invalid month numbers.
        };

    public static (int MonthNumber, string MonthName)[] GetMonths()
    {
        return new[] // Corrected syntax for array initialization.
        {
            (1, "فروردین"),
            (2, "اردیبهشت"),
            (3, "خرداد"),
            (4, "تیر"),
            (5, "مرداد"),
            (6, "شهریور"),
            (7, "مهر"),
            (8, "آبان"),
            (9, "آذر"),
            (10, "دی"),
            (11, "بهمن"),
            (12, "اسفند")
        };
    }

    public static string ToPersianDate(this DateTime date)
    {
        var year = pc.GetYear(date);
        var month = pc.GetMonth(date);
        var day = pc.GetDayOfMonth(date);
        return $"{year}/{month:D2}/{day:D2}"; // Simplified formatting.
    }

    public static int GetPersianDayOfWeek(DateTime date)
    {
        var pc = new PersianCalendar();
        return (int)pc.GetDayOfWeek(date); // 5 == جمعه
    }

    public static class HolidayHelper
    {
        // 📅 تعطیلات شمسی - ثابت
        private static readonly HashSet<string> Holidays = new()
        {
        // 📅 تعطیلات شمسی (ثابت)
        "01/01",  // نوروز - 1 فروردین
        "01/02",  // نوروز - 2 فروردین
        "01/03",  // نوروز - 3 فروردین
        "01/04",  // نوروز - 4 فروردین
        "01/12",  // روز جمهوری اسلامی
        "01/13",  // روز طبیعت
        "03/14",  // رحلت امام خمینی
        "03/15",  // قیام 15 خرداد
        "11/22",  // پیروزی انقلاب اسلامی
        "12/29",  // ملی شدن صنعت نفت 29
        };

        // 🕌 تعطیلات مذهبی - قمری
        private static readonly HashSet<string> HijriHolidays = new()
        {
        "1404/01/11",
        "1404/02/04",
        "1404/03/24",
        "1404/04/14",
        "1404/04/15",
        "1404/05/23",
        "1404/06/02",
        "1404/06/10",
        "1404/06/19",
        "1404/09/03",
        "1404/10/13",
        "1404/10/27",
        "1404/11/15",
        "1404/12/20",

        "1405/01/25", //شهادت امام جعفر صادق
        "1405/03/06", //عید سعید قربان
        "1405/04/03", //تاسوعای حسینی
        "1405/04/04", //عاشورای حسینی
        "1405/05/13", //اربعین حسینی
        "1405/05/21", //رحلت رسول اکرم؛شهادت امام حسن مجتبی
        "1405/05/22", //شهادت امام رضا
        "1405/05/30", //شهادت امام حسن عسکری
        "1405/06/08", //میلاد رسول اکرم و امام جعفر صادق
        "1405/10/02", //ولادت امام علی (ع) و روز پدر
        "1405/10/16", //مبعث رسول اکرم
        "1405/11/04", //ولادت حضرت قائم عجل الله تعالی فرجه و جشن نیمه شعبان
        "1405/12/09", //شهادت حضرت علی
        "1405/12/19", //عید سعید فطر
        "1405/12/20", //تعطیل به مناسبت عید سعید فطر
        };

        public static int GetPersianDayOfWeek(DateTime date)
        {
            var pc = new PersianCalendar();
            return (int)pc.GetDayOfWeek(date); // 5 == جمعه
        }

        public static List<DateTime> GetAllHolidays(int persianYear)
        {
            var holidays = new List<DateTime>();
            var persian = new PersianCalendar();
            var hijri = new HijriCalendar();

            // 📌 تعطیلات قمری
            foreach (var entry in HijriHolidays)
            {
                var parts = entry.Split('/');
                int year = int.Parse(parts[0]);
                int month = int.Parse(parts[1]);
                int day = int.Parse(parts[2]);

                try
                {
                    DateTime holiday = persian.ToDateTime(year, month, day, 0, 0, 0, 0);
                    holidays.Add(holiday);
                }
                catch (ArgumentOutOfRangeException)
                {
                    // در صورتی که تاریخ معتبر نباشد مثلاً برای سال های کبیسه
                }
            }

            // 📌 تعطیلات شمسی
            foreach (var entry in Holidays)
            {
                var parts = entry.Split('/');
                int month = int.Parse(parts[0]);
                int day = int.Parse(parts[1]);

                try
                {
                    DateTime holiday = persian.ToDateTime(persianYear, month, day, 0, 0, 0, 0);
                    holidays.Add(holiday);
                }
                catch (ArgumentOutOfRangeException)
                {
                    // در صورتی که تاریخ معتبر نباشد مثلاً برای سال های کبیسه
                }
            }

            return holidays.Distinct().OrderBy(d => d).ToList();
        }

        public static bool IsHoliday(DateTime date, int persianYear)
        {
            var holidays = GetAllHolidays(persianYear);
            return holidays.Any(d => d.Date == date.Date);
        }
    }
}