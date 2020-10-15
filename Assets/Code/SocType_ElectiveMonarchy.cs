using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class SocType_ElectiveMonarchy : SocType
    {
        public override string getName() { return "Elective Monarchy"; }

        public override string getDesc()
        {
            return "A human society, consisting of large numbers of serfs, and the nobles who rule over them."
            + "\nThis society is an elective monarchy, that is to say that the nobles vote on their rulers, selecting a single noble to act as sovreign."
            + "\nIf the nation owns enough territory outside the sovreign's province, they will elect dukes to rule over provinces. If they do, the sovreign can only be elected from the dukes (or be re-elected)"
            + " Only nobles residing in a province are eligible to become that province's duke."
            + "\n\nAll the actions (wars, territory allocation, criminal trials...) the society takes are voted on by the nobles, with weight a noble's vote carries equal to their prestige.";

        }

        public override bool periodicElection()
        {
            return true;
        }

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
