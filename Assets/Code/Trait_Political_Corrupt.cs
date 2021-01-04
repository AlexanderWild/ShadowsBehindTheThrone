using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Trait_Political_Corrupt : Trait
    {
        public Trait_Political_Corrupt()
        {
            name = "Politics: Corrupt";

            desc = "This character is politically corruot selfish. In a time of crisis, they will be upset of they are not catered to.";
            groupCode = Trait.CODE_POLITICS;
        }

        public override double getSelfInterest()
        {
            return 1;
        }
    }
}
