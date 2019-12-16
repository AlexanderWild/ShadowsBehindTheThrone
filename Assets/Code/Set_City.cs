using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Set_City : Settlement
    {
        public Set_City(Location loc) : base(loc)
        {
            title = new TitleLanded("Count", "Countess",this);
            name = "City of " + loc.shortName;
            basePrestige = 25;
            militaryCapAdd += 10;
            defensiveStrengthMax = 5;
            militaryRegenAdd = 0.15;
            isHuman = true;
        }

        public override Sprite getSprite()
        {
            return location.map.world.textureStore.loc_city_roman;
        }
    }
}
