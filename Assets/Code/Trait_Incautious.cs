using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Trait_Incautious : Trait
    {
        public Trait_Incautious()
        {
            name = "Incautious";

            desc = "This character does not pay enough attention to the evidence of darkness around others," +
                " and will become suspicion from seeing evidence at a slower rate.";
            groupCode = Trait.CODE_SUSPICION;
        }
        public override double suspicionMult()
        {
            return World.staticMap.param.trait_incautious;
        }

    }
}
