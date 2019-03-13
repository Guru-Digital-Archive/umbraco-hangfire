using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace UmbracoHangfire
{
    /// <summary>
    /// Convert Min/Hour/Day/Month <=> Cron
    /// </summary>
    public class HangfireNumMod
    {
        public int Num { get; set; }
        public HangfireMod Mod { get; set; }

        public HangfireNumMod(int Num, HangfireMod Mod)
        {
            this.Num = Num;
            this.Mod = Mod;
        }

        public HangfireNumMod(string expression)
        {
            string[] spl = expression.Split(' ');
            if (spl.Length != 5)
                throw new Exception("Parameter was not a valid cron expression, expression was longer than five items when split by ' '.");

            if (spl[0].Contains('/'))
            {
                Mod = HangfireMod.Minute;
                Num = ReadCronNum(spl[0]);
            }
            else if (spl[1].Contains('/'))
            {
                Mod = HangfireMod.Hour;
                Num = ReadCronNum(spl[1]);
            }
            else if (spl[2].Contains('/'))
            {
                Mod = HangfireMod.Day;
                Num = ReadCronNum(spl[2]);
            }
            else if (spl[3].Contains('/'))
            {
                Mod = HangfireMod.Month;
                Num = ReadCronNum(spl[3]);
            }
        }

        static int ReadCronNum(string num)
        {
            if (!int.TryParse(num.Replace("*/", ""), out int inum))
                throw new Exception("Parameter was not a valid cron expression, could not retrieve num.");
            return inum;
        }

        public string ToCron()
        {
            switch (Mod)
            {
                case HangfireMod.Minute: return String.Format("*/{0} * * * *", Num);
                case HangfireMod.Hour: return String.Format("0 */{0} * * *", Num);
                case HangfireMod.Day: return String.Format("0 0 */{0} * *", Num);
                case HangfireMod.Month: return String.Format("0 0 1 */{0} *", Num);
            }

            throw new Exception("Unhandled UmbracoHangfireMod value");
        }

        public string ModToString()
        {
            switch (Mod)
            {
                case HangfireMod.Minute: return "Minutes";
                case HangfireMod.Hour: return "Hours";
                case HangfireMod.Day: return "Days";
                case HangfireMod.Month: return "Months";
            }
            return "";
        }
        public void StringToMod(string mod)
        {
            if (mod == "Minutes") Mod = HangfireMod.Minute;
            else if (mod == "Hours") Mod = HangfireMod.Hour;
            else if (mod == "Days") Mod = HangfireMod.Day;
            else if (mod == "Months") Mod = HangfireMod.Month;
        }
    }
}