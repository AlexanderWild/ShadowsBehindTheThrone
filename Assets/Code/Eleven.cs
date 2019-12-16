using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Eleven
    {
        public static Random random = new Random();


        public static string putCommas(double d)
        {
            return putCommas((int)d);
        }
        public static string putCommas(int d)
        {
            string s = "" + (int)d;
            char[] array = s.ToCharArray();

            string reply = "";
            for (int i = 0; i < s.Length; i++)
            {
                int remain = (i - s.Length) + 1;
                reply += array[i];
                if (remain != 0 && remain % 3 == 0)
                {
                    reply += ",";
                }
            }

           
            return reply;
        }

        public static string toFixedLen(double d, int len)
        {
            if (Math.Abs(d) < 0.0001) { d = 0; }

            string s = "" + d;
            if (s.Length > len)
            {
                s = s.Substring(0, len);
            }

            while (s.Length < len)
            {
                s = " " + s;
            }
            return s;
        }
        public static string toFixedLen(string s,int len)
        {
            if (s.Length > len)
            {
                s = s.Substring(0, len);
            }

            while ( s.Length < len)
            {
                s = " " + s;
            }
            return s;
        }
    }
}
