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
            this.likingChange = World.staticMap.param.trait_hateful;

            desc = "This character is filled with dislike for others, reducing relationships by " + ((int)(likingChange)) +  ".";
            groupCode = Trait.CODE_LIKING;
        }
        
    }
}
