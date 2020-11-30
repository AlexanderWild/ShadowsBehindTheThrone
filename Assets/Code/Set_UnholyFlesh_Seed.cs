using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Set_UnholyFlesh_Seed : Settlement
    {
        public Set_UnholyFlesh_Seed(Location loc) : base(loc)
        {
            this.isHuman = false;
            name = "Unholy Seed";


        }

        public override Sprite getCustomTerrain(Hex hex)
        {
            int c = hex.graphicalIndexer % 2;
            if (c == 0)
            {
                return World.staticMap.world.textureStore.hex_special_flesh;
            }
            else
            {
                return World.staticMap.world.textureStore.hex_special_flesh2;
            }
        }

        public override void turnTick()
        {
            base.turnTick();

            foreach (Hex h in location.territory)
            {
                h.flora = null;
            }
        }

        public override Sprite getSprite()
        {
            return location.map.world.textureStore.loc_flesh;
        }
    }
}
