using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Trait_Political_Warlike : Trait
    {
        public Trait_Political_Warlike()
        {
            name = "Politics: Warlike";

            desc = "This character is very focused on military solutions to problems and will be favourable to war declarations.";
            groupCode = Trait.CODE_POLITICS;
        }

        public override double getMilitarism()
        {
            return 1;
        }
        public override double getSelfInterest()
        {
            return 0.25;
        }
    }
}
