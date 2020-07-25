using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Set_UnholyFlesh_Ganglion : Settlement
    {
        public Set_UnholyFlesh_Ganglion(Location loc) : base(loc)
        {
            this.isHuman = false;
            name = "Unholy Ganglion";


            militaryCapAdd += 15;
            militaryRegenAdd = 3;
        }

        public override Sprite getCustomTerrain()
        {
            return World.staticMap.world.textureStore.hex_special_flesh;
        }

        public override void turnTick()
        {
            base.turnTick();

            foreach (Hex h in location.territory)
            {
                h.flora = null;
            }
        }

        public override void checkUnitSpawning()
        {
            spawnCounter += 1;
            if (spawnCounter > 5)
            {
                spawnCounter = 0;

                if (this.attachedUnit != null) { throw new Exception(); }

                Unit_Flesh army = new Unit_Flesh(location, location.soc);
                location.map.units.Add(army);
                army.maxHp = (int)this.getMilitaryCap();
                this.attachedUnit = army;
                World.log("Created new flesh");
            }
        }
        public override Sprite getSprite()
        {
            return location.map.world.textureStore.loc_flesh;
        }
    }
}
