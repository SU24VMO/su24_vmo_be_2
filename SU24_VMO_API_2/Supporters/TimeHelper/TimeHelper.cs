using System;

namespace SU24_VMO_API.Supporters.TimeHelper
{
    public class TimeHelper
    {
        public static string GetTimezone()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            return config["Timezone:SystemTimeZoneId"]!;
        }
        public static DateTime GetTime(DateTime time)
        {
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(GetTimezone()));
        }
    }
}
