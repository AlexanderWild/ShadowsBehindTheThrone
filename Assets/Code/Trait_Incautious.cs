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
            suspicionMult = World.staticMap.param.trait_incautious;

            desc = "This character does not pay enough attention to the evidence of darkness around others, and will gain suspicion from evidence at a rate of " + ((int)(100*suspicionMult)) +  "%.";
            groupCode = Trait.CODE_SUSPICION;
        }
        
    }
}
