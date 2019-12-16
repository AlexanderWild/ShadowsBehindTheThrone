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

            if (society.people.Count > World.staticMap.param.society_nPeopleForEmpire)
            {
                nameM = "Emperor";
                nameF = "Empress";
            }
            else if (society.people.Count > World.staticMap.param.society_nPeopleForKingdom)
            {
                nameM = "King";
                nameF = "Queen";
            }
            else
            {
                nameM = "Duke";
                nameF = "Duchess";
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
