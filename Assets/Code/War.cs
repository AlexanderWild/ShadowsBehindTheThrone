using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class War
    {
        public SocialGroup att;
        public SocialGroup def;
        public int startTurn;
        public bool canTimeOut = true;

        public War(Map map,SocialGroup att, SocialGroup def)
        {
            startTurn = map.turn;
            this.att = att;
            this.def = def;
        }
    }
}
