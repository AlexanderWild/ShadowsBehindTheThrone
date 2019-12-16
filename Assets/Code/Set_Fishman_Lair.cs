using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Set_Fishman_Lair : Settlement
    {
        public Set_Fishman_Lair(Location loc) : base(loc)
        {
            this.isHuman = false;
            name = "Fishman Lair";


            militaryCapAdd += 10;
            militaryRegenAdd = 0;
        }

        public override Sprite getSprite()
        {
            return location.map.world.textureStore.loc_minor_ritualCircle;
        }
    }
}
