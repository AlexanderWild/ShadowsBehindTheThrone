using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Trait_Political_Honorable : Trait
    {
        public Trait_Political_Honorable()
        {
            name = "Politics: Honorable";

            desc = "This character is will set aside their own self-interest and vote as they think is best for their nation.";
            groupCode = Trait.CODE_POLITICS;
        }

        public override double getSelfInterest()
        {
            return 0;
        }
    }
}
