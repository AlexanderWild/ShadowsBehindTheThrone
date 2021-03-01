using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class DipRel
    {
        public double a;
        public double b;
        public string nameA;
        public string nameB;

        public enum dipState { none, war, alliance };
        public dipState state = dipState.none;
        public War war;
        public Map map;
        public int lastTick = -1;

        public DipRel(Map map, SocialGroup a, SocialGroup b)
        {
            this.map = map;
            this.a = a.randID;
            this.b = b.randID;
            nameA = a.getName();
            nameB = b.getName();
        }
        public SocialGroup other(SocialGroup c)
        {
            double key = 0;
            if (c.randID == a) { key = b; }
            else { key = a; }
            foreach (SocialGroup sg in map.socialGroups)
            {
                if (sg.randID == key) { return sg; }
            }
            return null;
        }
        public SocialGroup getA()
        {
            foreach (SocialGroup sg in map.socialGroups)
            {
                if (sg.randID == a) { return sg; }
            }
            return null;
        }
        public SocialGroup getB()
        {
            foreach (SocialGroup sg in map.socialGroups)
            {
                if (sg.randID == a) { return sg; }
            }
            return null;
        }


        public void turnTick()
        {
            if (lastTick == map.turn) { return; }
            lastTick = map.turn;
        }
    }
}
