using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class DipRel
    {
        public SocialGroup a;
        public SocialGroup b;
        public enum dipState { none, war, alliance };
        public dipState state = dipState.none;
        public War war;
        public Map map;
        public int lastTick = -1;

        public DipRel(Map map, SocialGroup a, SocialGroup b)
        {
            this.map = map;
            this.a = a;
            this.b = b;
        }
        public SocialGroup other(SocialGroup c)
        {
            if (c == a) { return b; }
            return a;
        }

        public void turnTick()
        {
            if (lastTick == map.turn) { return; }
            lastTick = map.turn;
        }
    }
}
