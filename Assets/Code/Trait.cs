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
        public static int CODE_MILCAP = 4;
        public static int CODE_MILDEF= 5;
        public static int CODE_SABOTAGE = 6;
        public static int CODE_POLITICS = 7;

        public string name;
        public string desc = "DEFAULT DESCRIPTION";
        public virtual double receivedLikingDelta() { return 0; }
        public virtual double suspicionMult() { return 1; }
        public virtual double likingChange() { return 0; }
        public virtual double milCapChange() { return 0; }
        public virtual double defChange() { return 0; }
        public virtual double superiorPrestigeChange() { return 0; }
        public virtual double desirabilityAsFollower() { return 0; }
        public virtual double getAwarenessMult() { return 1; }
        public int groupCode;

        public virtual double getMilitarism() { return 0; }
        public virtual void turnTick(Person p) { }

        //0 to 1
        public virtual double getSelfInterest() { return 0; }

    }
}
