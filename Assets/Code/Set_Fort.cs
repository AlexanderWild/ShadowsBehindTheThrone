using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Set_Fort : Settlement
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


            militaryCapAdd += 5;
            militaryRegenAdd = 0.07;
            this.defensiveStrengthMax = 15;
            isHuman = true;
        }

        public override Sprite getSprite()
        {
            return location.map.world.textureStore.loc_minor_fort;
        }
    }
}
