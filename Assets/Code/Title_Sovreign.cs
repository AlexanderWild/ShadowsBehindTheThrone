using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

        public override List<Person> getEligibleHolders(Society soc)
        {
            List<Person> candidates = new List<Person>();
            if (society.titles.Count == 1)
            {
                //Everyone is eligible
                foreach (Person p in society.people)
                {
                    candidates.Add(p);
                }
            }
            else
            {
                //Only provincial rulers are elligible
                foreach (Person p in society.people)
                {
                    bool hasElevatedTitle = false;
                    foreach (Title t in p.titles)
                    {
                        if (t is Title_ProvinceRuler || t is Title_Sovreign)
                        {
                            hasElevatedTitle = true;
                        }
                    }
                    if (hasElevatedTitle)
                    {
                        candidates.Add(p);
                    }
                }
            }
            return candidates;
        }
    }
}
