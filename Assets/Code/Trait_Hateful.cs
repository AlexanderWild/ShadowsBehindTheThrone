using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Trait_Hateful : Trait
    {
        public Trait_Hateful()
        {
            name = "Hateful";

            desc = "This character is filled with dislike for others, reducing relationships.";
            groupCode = Trait.CODE_LIKING;
        }

        public override double likingChange()
        {
            return World.staticMap.param.trait_hateful;
        }
    }
}
