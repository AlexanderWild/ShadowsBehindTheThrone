using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class SocType_Monarchy : SocType
    {
        public override string getName() { return "Monarchy"; }

        public override string getDesc()
        {
            return "A human society, consisting of large numbers of serfs, and the nobles who rule over them."
            + "\nThis society is a monarchy. The nobles will vote to appoint a sovreign and their dukes, and those will then rule until death."
            + "\nAppointing rulers by lifetime election makes the society more brittle, as they cannot replace a corrupt or hated monarch, so civil war is more likely than in a society which could vote out its rulers."
            + "\n\nAll the actions (wars, territory allocation, criminal trials...) the society takes are voted on by the nobles, with weight a noble's vote carries equal to their prestige.";
        }
        public override bool periodicElection()
        {
            return false;
        }

        /**
         * We can have an arbitrary number of dukes, since we don't have elections, it won't get all shrivelly with constant elections
         */
        public override int getNDukesMax()
        {
            return World.staticMap.param.society_maxDukes;
        }

        public override bool usesHouses()
        {
            return true;
        }
    }
}
