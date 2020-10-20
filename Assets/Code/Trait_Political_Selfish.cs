using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Trait_Political_Selfish : Trait
    {
        public Trait_Political_Selfish()
        {
            name = "Politics: Selfish";

            desc = "This character is politically selfish. In a time of crisis, they will be upset of they are not catered to.";
            groupCode = Trait.CODE_POLITICS;
        }
    }
}
