using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Set_Fort : SettlementHuman
    {
        public Set_Fort(Location loc) : base(loc)
        {
            title = new TitleLanded("Baron", "Baroness",this);
            int q = Eleven.random.Next(2);
            if (q == 0)
            {
                name = loc.shortName + " Castle";
            }else if (q == 1)
            {
                name = "Fort " + loc.shortName;
            }


            militaryCapAdd += 20;
            militaryRegenAdd = 3;
            this.defensiveStrengthMax = 25;
            isHuman = true;
        }

        public override void humanTurnTick()
        {
        }

        public override string getFlavour()
        {
            return location.map.world.wordStore.lookup("SET_FORT");
        }
        public override Sprite getSprite()
        {
            return location.map.world.textureStore.loc_minor_fort;
        }
    }
}
