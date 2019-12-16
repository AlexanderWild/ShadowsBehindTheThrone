using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Set_DarkGeneric : Settlement
    {
        public Set_DarkGeneric(Location loc) : base(loc)
        {
            this.isHuman = false;
            name = "Dark Circle";


            militaryCapAdd += 5;
            militaryRegenAdd = 0.25;
            this.defensiveStrengthMax = 15;
        }

        public override Sprite getSprite()
        {
            return location.map.world.textureStore.loc_minor_ritualCircle;
        }
    }
}
