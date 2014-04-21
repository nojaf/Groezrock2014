using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Groezrock2014.Models
{
    public class GroezrockConstants
    {
        public readonly static DateTime DayOne = new DateTime(2014, 5, 2);
        public readonly static DateTime DayTwo = new DateTime(2014, 5, 3);

        public const string ScheduleUrl = "http://www.groezrock.be/timetable/";
        public const string BandUrl = "http://www.groezrock.be/bands/";
        public const string GroezrockUrl = "http://www.groezrock.be/";
        public const string CacheFile = "data.json";
        public const string InfoUrl = "http://www.groezrock.be/info";
        public const string InfoFile = "info.json";
    }
}
