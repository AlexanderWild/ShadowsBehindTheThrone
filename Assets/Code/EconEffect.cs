using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class EconEffect
    {
        public EconTrait from;
        public EconTrait to;
        public int turnStarted;
        public int durationLeft;

        public EconEffect(Map map,EconTrait from,EconTrait to)
        {
            this.durationLeft = map.param.econ_buffDuration;
            this.turnStarted = map.turn;
            this.from = from;
            this.to = to;
        }
    }
}
