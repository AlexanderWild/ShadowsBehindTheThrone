using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Pr_EmptyGraves : Property_Prototype
    {
        
        public Pr_EmptyGraves(Map map,string name) : base(map,name)
        {
            this.informationAvailabilityMult = 0.5;
            this.baseCharge = map.param.unit_doctor_emptyGravesDuration;
            this.decaysOverTime = true;
            this.stackStyle = stackStyleEnum.ADD_CHARGE;
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.property_noDead;
        }

        internal override string getDescription()
        {
            return "Empty Graves: The graveyard has been looted, the bodies are gone. No more bodies can be obtained for a while.";
        }
    }
}
