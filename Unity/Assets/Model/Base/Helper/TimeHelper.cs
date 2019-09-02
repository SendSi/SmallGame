using System;

namespace ETModel
{
    public static class TimeHelper
    {
        private static readonly long epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
        private static string[] mStrFormats;
        private static void GetStrFormats()
        {
            if (mStrFormats == null || mStrFormats.Length == 0)
                mStrFormats = LanguageMgr.mInstance.GetLanguageForKeyMoreValues("MethodTime001");
        }

        /// <summary>
        /// 客户端时间
        /// </summary>
        /// <returns></returns>
        public static long ClientNow()
        {
            return (DateTime.UtcNow.Ticks - epoch) / 10000;
        }

        public static long ClientNowSeconds()
        {
            return (DateTime.UtcNow.Ticks - epoch) / 10000000;
        }

        /// <summary>
        /// 登陆前是客户端时间,登陆后是同步过的服务器时间
        /// </summary>
        public static long Now()
        {
            return ClientNow();
        }

        /// <summary>         验证码 统一的 秒数         </summary>
        public static int GetCodeTime()
        {
            return 10;
        }

        /// <summary> 传毫秒 </summary>
	    public static DateTime GetDateTimer(long pNum)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            DateTime dt = startTime.AddMilliseconds(pNum);
            return dt;
        }

        /// <summary>传毫秒    得月日 </summary>
        public static string TimerDateStr(long pNum)
        {
            GetStrFormats();
            var t = GetDateTimer(pNum).ToString(string.Format("MM{0}dd{1}", mStrFormats[1], mStrFormats[2]));//月 日
            return t;
        }
        /// <summary> 时分 </summary>
	    public static string TimerDateMinStr(long pNum)
        {
            var t = GetDateTimer(pNum).ToString("HH:mm");
            return t;
        }
        /// <summary>
        /// 获取当前系统时间戳
        /// </summary>
        public static long GetTimestamp()
        {
            long timestamp = (long)DateTime.Now.Subtract(DateTime.Parse("1970-1-1")).TotalMilliseconds;
            return timestamp;
        }
        /// <summary> 毫秒         格式:余下----19天22时30分50秒  pSec是否要秒</summary>
	    public static string ShowRemainingTimer(long pNum, bool pSec = true)
        {//1小时3600秒      1天86400秒
            pNum = pNum / 1000;
            var vTimeStr = LanguageMgr.mInstance.GetLanguageForKeyMoreValues("UIMatch_itemTime");
            if (pNum >= 86400)//>1天
            {
                var tDay = pNum / 86400;
                var tHour = pNum % 86400 / 3600;
                var tMinutes = pNum % 86400 % 3600 / 60;
                var tseconds = pNum % 86400 % 3600 % 60;
                if (pSec)
                    return tDay.ToString() + vTimeStr[0] + tHour.ToString() + vTimeStr[1] + tMinutes.ToString() + vTimeStr[2] + tseconds.ToString() + vTimeStr[3];
                else
                    return tDay.ToString() + vTimeStr[0] + tHour.ToString() + vTimeStr[1] + tMinutes.ToString() + vTimeStr[2];
            }
            else if (pNum >= 3600)//>1小时
            {
                var tHour = pNum / 3600;
                var tMinutes = pNum % 3600 / 60;
                var tseconds = pNum % 3600 % 60;
                if (pSec)
                    return tHour.ToString() + vTimeStr[1] + tMinutes.ToString() + vTimeStr[2] + tseconds.ToString() + vTimeStr[3];
                else
                    return tHour.ToString() + vTimeStr[1] + tMinutes.ToString() + vTimeStr[2];
            }
            else if (pNum >= 60)//>1分钟
            {
                var tMinutes = pNum / 60;
                var tseconds = pNum % 60;
                if (pSec)
                    return tMinutes.ToString() + vTimeStr[2] + tseconds.ToString() + vTimeStr[3];
                else
                    return tMinutes.ToString() + vTimeStr[2];
            }
            else if (pNum < 60)
            {
                return pNum.ToString() + vTimeStr[3];
            }
            return "";
        }

        /// <summary> pNum秒单位         格式:余下----5:22:10  小时:分钟:秒</summary>
        public static string ShowRemainingSemicolon(int pNum)
        {//1小时3600秒      1天86400秒
            if (pNum >= 3600)//>1小时
            {
                var tHour = pNum / 3600;
                var tMinutes = pNum % 3600 / 60;
                var tseconds = pNum % 3600 % 60;
                return tHour.ToString().PadLeft(2, '0') + ":" + tMinutes.ToString().PadLeft(2, '0') + ":" + tseconds.ToString().PadLeft(2, '0');
            }
            else if (pNum >= 60)//>1分钟
            {
                var tMinutes = pNum / 60;
                var tseconds = pNum % 60;
                return "00:" + tMinutes.ToString().PadLeft(2, '0') + ":" + tseconds.ToString().PadLeft(2, '0');

            }
            else if (pNum < 60)
            {
                return "00:00:" + pNum.ToString();
            }
            return "";
        }


    }
}