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

            if (location.soc is SG_UnholyFlesh)
            {
                SG_UnholyFlesh parent = (SG_UnholyFlesh)location.soc;
                if (parent.warState == SG_UnholyFlesh.warStates.DEFEND)
                {
                    defensiveStrengthMax = location.map.param.ability_fleshWithdrawBonus;
                }
            }
        }

        public override void checkUnitSpawning()
        {
            spawnCounter += 1;
            if (spawnCounter > 5)
            {
                spawnCounter = 0;

                if (this.attachedUnit != null) { throw new Exception(); }

                if (location.soc is SG_UnholyFlesh == false) { return; }
                Unit_Flesh army = new Unit_Flesh(location, (SG_UnholyFlesh)location.soc);
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
