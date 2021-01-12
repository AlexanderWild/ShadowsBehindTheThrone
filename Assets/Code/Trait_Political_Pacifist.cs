using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Trait_Political_Pacifist : Trait
    {
        public Trait_Political_Pacifist()
        {
            name = "Politics: Pacifist";

            desc = "This character dislikes war, and will vote against military options unless all other options are closed.";
            groupCode = Trait.CODE_POLITICS;
        }
        public override double getMilitarism()
        {
            return -1;
        }
        public override double getSelfInterest()
        {
            return 0.25;
        }
    }
}
