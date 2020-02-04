using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Trait_Charismatic : Trait
    {
        public Trait_Charismatic()
        {
            name = "Charismatic";
            desc = "This character has a talent for convincing others to side with them, others will like them more than usual.";
            groupCode = Trait.CODE_LIKABILITY;
        }

        public override double receivedLikingDelta()
        {
            return World.staticMap.param.trait_charismatic;
        }
    }
}
