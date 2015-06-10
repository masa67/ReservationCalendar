using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ReservationCalendar.Helpers
{
    public class TimeHelper
    {
        public static long GetUTCTimeStamp()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        public static long GetUTCTimeStampWithOffset(long offset)
        {
            return (((long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds) + offset);
        }

        public static DateTime GetUTCDateTime()
        {
            return DateTime.UtcNow;
        }

        public static long DateTimeToUTCTimeStamp(DateTime date, bool isUtc)
        {
            if (!isUtc)
            {
                date = date.ToUniversalTime();
            }
            return (long)(date - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        public static DateTime UTCTimeStampToUTCDateTime(long timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp);
        }

        public static DateTime UTCTimeStampToLocalDateTime(long timestamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp).ToLocalTime();
        }

        public int DateTimeToUnixTimestamp(DateTime dt) {
            return (int) dt.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}