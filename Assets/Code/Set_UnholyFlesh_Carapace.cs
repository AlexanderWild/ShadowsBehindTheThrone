using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Set_UnholyFlesh_Carapace : Settlement
    {
        public Set_UnholyFlesh_Carapace(Location loc) : base(loc)
        {
            this.isHuman = false;
            name = "Unholy Carapace";


            militaryCapAdd += 2;
            militaryRegenAdd = 0;
            defensiveStrengthMax = 10;
        }

        public override Sprite getSprite()
        {
            return location.map.world.textureStore.loc_flesh;
        }
    }
}
