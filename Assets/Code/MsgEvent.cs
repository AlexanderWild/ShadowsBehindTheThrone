using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class MsgEvent : IComparable<MsgEvent>
    {
        public static int LEVEL_BLUE   = -1;
        public static int LEVEL_RED = 0;
        public static int LEVEL_ORANGE = 1;
        public static int LEVEL_YELLOW = 2;
        public static int LEVEL_GRAY = 3;

        public static int LEVEL_GREEN = 0;
        public static int LEVEL_DARK_GREEN = 1;
        public static int LEVEL_DARK_GREEN2 = 2;
        /*
         * Basically an int, but implemented as doubles to allow fine-grain control.
         * -1: Blue. Action available
         * 0: Red: Critical (vote to kill enthralled, war declared on enthralled civ...)
         * 1: Orange: Major events (city taken, vote passes in enthralled soc...)
         * 2: Yellow: Irrelevant but bad (unknown, ideally rarely used)
         * 3: Grey other stuffs
         *
         * We then loop. Positive stuff gets the same priority values, to allow it to sort based on usefulness (red items are more exciting than boring greens)
         */
        public double priority;
        public bool beneficial;
        public string msg;

        public MsgEvent(string v, double u,bool positive)
        {
            this.msg = v;
            if (u < 0)
            {
                this.priority = u + (Eleven.random.NextDouble() * -0.001); // Skew for stable sort
            }
            else {
                this.priority = u + (Eleven.random.NextDouble() * 0.001); // Skew for stable sort
            }
            this.beneficial = positive;
        }

        public int CompareTo(MsgEvent other)
        {
            return Math.Sign(priority - other.priority);
        }
    }
}
