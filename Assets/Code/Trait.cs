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
        public virtual double receivedLikingDelta() { return 0; }
        public virtual double suspicionMult() { return 1; }
        public virtual double likingChange() { return 0; }
        public int groupCode;
    }
}
