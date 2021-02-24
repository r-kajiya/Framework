using System;

namespace Framework
{
    public static class TimeUtil
    {
        public static readonly TimeSpan Tokyo = new TimeSpan(+09, 00, 00);
        
        public static long CurrentUnixTimeTokyo()
        {
            var dto = new DateTimeOffset(DateTime.Now.Ticks, Tokyo);
            return dto.ToUnixTimeSeconds();
        }
        
        public static long CurrentUnixTime(TimeSpan timeSpan)
        {
            var dto = new DateTimeOffset(DateTime.Now.Ticks, timeSpan);
            return dto.ToUnixTimeSeconds();
        }
    }
}