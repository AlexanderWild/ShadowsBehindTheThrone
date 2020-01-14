using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Trait
    {
        public static int CODE_LIKABILITY = 1;
        public static int CODE_SUSPICION = 2;
        public static int CODE_LIKING = 3;

        public string name;
        public string desc = "DEFAULT DESCRIPTION";
        public double receivedLikingDelta = 0;
        public double suspicionMult = 1;
        public double likingChange = 0;
        public int groupCode;
    }
}
