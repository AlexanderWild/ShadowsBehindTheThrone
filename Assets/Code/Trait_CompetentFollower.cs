using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Trait_CompetentFollower : Trait
    {
        public Trait_CompetentFollower()
        {
            name = "Good Follower";

            desc = "This character increases  their direct superior's prestige, if they have one. (The ruler of their province, either duke or sovreign)";
            groupCode = Trait.CODE_SABOTAGE;
        }

        public override double superiorPrestigeChange()
        {
            return World.staticMap.param.trait_competent_gain;
        }
        public override double desirabilityAsFollower()
        {
            return World.staticMap.param.trait_competent_desirability;
        }
    }
}
