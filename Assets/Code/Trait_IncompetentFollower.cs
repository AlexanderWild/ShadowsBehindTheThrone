using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Trait_IncompetentFollower : Trait
    {
        public Trait_IncompetentFollower()
        {
            name = "Incompetent Follower";

            desc = "This character reduces their direct superior's prestige, if they have one. (The ruler of their province, either duke or sovreign)";
            groupCode = Trait.CODE_SABOTAGE;
        }

        public override double desirabilityAsFollower()
        {
            return World.staticMap.param.trait_incompetent_desirability;
        }
        public override double superiorPrestigeChange()
        {
            return -World.staticMap.param.trait_incompetent_loss;
        }
    }
}
