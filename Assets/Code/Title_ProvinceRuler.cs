using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Code{
    public class Title_ProvinceRuler : Title
    {
        public Province province;
        public Title_ProvinceRuler(Society soc,Province province):base(soc)
        {
            nameM = "Duke";
            nameF = "Duchess";
            this.province = province;
        }

        public override void turnTick()
        {
            base.turnTick();
        }
        public override string getName()
        {
            return "Dukedom of " + province.name;
        }
        public override double getPrestige()
        {
            return society.map.param.society_dukePrestige;
        }

        public override List<Person> getEligibleHolders(Society soc)
        {
            List<Person> candidates = new List<Person>();
            //Nobles holding land in region are eligible
            foreach (Person p in society.people)
            {
                if (p == society.getSovreign()) { continue; }
                if (p.title_land != null && p.title_land.settlement.location.province == province)
                {
                    candidates.Add(p);
                }
            }
            return candidates;
        }
    }
}
