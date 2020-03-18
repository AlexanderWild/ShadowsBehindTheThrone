using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Trait_BadCommander : Trait
    {
        public Trait_BadCommander()
        {
            name = "Bad Commander";

            desc = "This character reduces the amount of military capacity produced by a settlement.";
            groupCode = Trait.CODE_MILCAP;
        }

        public override double milCapChange()
        {
            return World.staticMap.param.trait_badCommander;
        }
    }
}
