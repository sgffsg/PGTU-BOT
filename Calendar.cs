using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TestBOTForPGTU
{
    class CalendarView
    {

            public List<Calendar> calen;
            public static string PrintCalend()
            {
                string tmpStr = "";

                return tmpStr;
            }

            private static Calendar instance;
            public static Calendar Instance()
            {
                if (instance == null)
                {
                    instance = new Calendar();
                }
                return instance;
            }

            public string Date { get; set; }
            public class Calendar
            {
                public string building { get; set; }
                public string category { get; set; }
                public DateTime date { get; set; }
                public string description { get; set; }
                public string fio { get; set; }
                public string fullDescription { get; set; }
                public string subGroup { get; set; }
                public int personId { get; set; }
                public string room { get; set; }
                public List<TimeBegin> timeBegin { get; set; }
                public List<TimeEnd> timeEnd { get; set; }
                public class TimeBegin
                {
                    public int ticks { get; set; }
                    public int days { get; set; }
                    public int hours { get; set; }
                    public int milliseconds { get; set; }
                    public int minutes { get; set; }
                    public int seconds { get; set; }
                    public double totalDays { get; set; }
                    public double totalHours { get; set; }
                    public double totalMilliseconds { get; set; }
                    public double totalMinutes { get; set; }
                    public double totalSeconds { get; set; }
                }

                public class TimeEnd
                {
                    public int ticks { get; set; }
                    public int Days { get; set; }
                    public int hours { get; set; }
                    public int milliseconds { get; set; }
                    public int minutes { get; set; }
                    public int seconds { get; set; }
                    public double totalDays { get; set; }
                    public double totalHours { get; set; }
                    public double totalMilliseconds { get; set; }
                    public double totalMinutes { get; set; }
                    public double totalSeconds { get; set; }
                }

                public string typeWorkName { get; set; }
                public int weekNumber { get; set; }
                public string weekTypeName { get; set; }
                public int htmlColorCode { get; set; }
                public string url { get; set; }

            }

            public class UnivuzApiService : IDisposable
            {
                private Autorisation.UnivuzApiClient client;
                public UnivuzApiService()
                {
                    client = new Autorisation.UnivuzApiClient();
                }

                public async Task<Calendar> GetCalendar(int studentId)
                {
                    Calendar stInf = null;
                    var tmp = await client.Get($"Calendar/{studentId}/{"2021 - 21 - 05T07:00:30.579Z"}/{"2021 - 21 - 05T22:00:30.579Z"}", typeof(Calendar));
                    if (tmp is Calendar calen)
                    {
                        stInf = calen;
                    }
                    else
                        throw new Exception((tmp as Autorisation.ErrorMsg)?.Message);
                    return stInf;
                }
                public void Dispose()
                {
                    client?.Dispose();
                }
            }
        }


    
}
