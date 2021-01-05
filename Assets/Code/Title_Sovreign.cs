using UnityEngine;
using System.Collections;

namespace Assets.Code{
    public class Title_Sovreign : Title
    {
        public Title_Sovreign(Society soc):base(soc)
        {
            nameM = "King";
            nameF = "Queen";
        }

        public override void turnTick()
        {
            base.turnTick();

            if (society.getLevel() == 2)
            {
                nameM = "Emperor";
                nameF = "Empress";
            }
            else if (society.getLevel() == 1)
            {
                nameM = "King";
                nameF = "Queen";
            }
            else
            {
                nameM = "ArchDuke";
                nameF = "ArchDuchess";
            }
        }
        public override string getName()
        {
            return "Sovreignty of " + society.getName();
        }
        public override double getPrestige()
        {
            return society.map.param.society_sovreignPrestige;
        }
    }
}
