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
            name = "Deep One Lair";

            militaryCapAdd += 25;


            Unit_Fishman army = new Unit_Fishman(location, location.soc);
            location.map.units.Add(army);
            army.maxHp = (int)this.getMilitaryCap();
            this.attachedUnit = army;
        }

    public override void checkUnitSpawning()
    {
        spawnCounter += 1;
        if (spawnCounter > 5)
        {
            spawnCounter = 0;

            if (this.attachedUnit != null) { throw new Exception(); }

            Unit_Fishman army = new Unit_Fishman(location, location.soc);
            location.map.units.Add(army);
            army.maxHp = (int)this.getMilitaryCap();
            this.attachedUnit = army;
        }
    }

    public override Sprite getSprite()
        {
            return location.map.world.textureStore.loc_minor_ritualCircle;
        }
    }
}
