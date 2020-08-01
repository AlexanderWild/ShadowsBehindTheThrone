using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Set_Corpseroot : Settlement
    {
        public Set_Corpseroot(Location loc,SocialGroup sg) : base(loc)
        {
            this.isHuman = false;
            name = "Corpseroot";
            this.militaryCapAdd = World.staticMap.param.unit_armyOfDeadMaxHP;
            this.embeddedUnit = new Unit_ArmyOfTheDead(loc, sg);
           
        }

        public override string getFlavour()
        {
            return "Corpseroot prevent all life, thus preventing the dead from decaying. The bodies lie in wait, ready to rise again.";
        }
        public override void turnTick()
        {
            base.turnTick();

            this.location.hex.purity = 0.0f;
        }


        public override Sprite getSprite()
        {
            return location.map.world.textureStore.loc_corpseroot;
        }
    }
}
