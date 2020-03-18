using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Trait_Warmaster : Trait
    {
        public Trait_Warmaster()
        {
            name = "Warmaster";

            desc = "This character adds a bonus to the military capacity of their settlement.";
            groupCode = Trait.CODE_MILCAP;
        }

        public override double milCapChange()
        {
            return World.staticMap.param.trait_warmaster;
        }
    }
}
