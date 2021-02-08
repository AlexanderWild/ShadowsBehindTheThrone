using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Set_Abbey : SettlementHuman
    {
        public Set_Abbey(Location loc) : base(loc)
        {
            title = new TitleLanded("Abbot", "Abbess",this);
            int q = Eleven.random.Next(3);
            if (q == 0)
            {
                name = loc.shortName + " Abbey";
            }else if (q == 1)
            {
                name = loc.shortName + " Cathedral";
            }
            else if (q == 2)
            {
                name = "Church of " + loc.shortName;
            }


            militaryCapAdd += 15 * location.map.param.unit_armyHPMult;
            militaryRegenAdd = 7;
            isHuman = true;
        }
        public override void humanTurnTick()
        {
        }

        public override string getFlavour()
        {
            return location.map.world.wordStore.lookup("SET_CHURCH");
        }
        public override Sprite getSprite()
        {
            if (title != null && title.heldBy != null && title.heldBy.shadow > 0.5)
            {
                return location.map.world.textureStore.loc_minor_church_dark;
            }
            return location.map.world.textureStore.loc_minor_church;
        }
    }
}
