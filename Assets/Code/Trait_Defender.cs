using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Trait_Defender : Trait
    {
        public Trait_Defender()
        {
            name = "Defender";

            desc = "This character adds a bonus to the defense of their settlement.";
            groupCode = Trait.CODE_MILDEF;
        }

        public override double defChange()
        {
            return World.staticMap.param.trait_defender;
        }
    }
}
