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

            desc = "This character has a chance every turn to harm their superior's prestige. (sovreign or provicial duke)";
            groupCode = Trait.CODE_SABOTAGE;
        }

        public override void turnTick(Person p)
        {
            if (Eleven.random.NextDouble() > p.map.param.trait_incompetent_probability) { return; }
            Person sup = p.getSuperiorIfAny();
            if (sup == null) { return; }

            double prev = sup.prestige;
            sup.prestige -= p.map.param.trait_incompetent_loss;
            if (sup.prestige < 0) { sup.prestige = 0; }
            sup.getRelation(p).addLiking(p.map.param.trait_incompetent_dislike,"Incompetence harmed prestige",p.map.turn);

            if (sup.state == Person.personState.enthralled)
            {
                p.map.world.prefabStore.popMsg(p.getFullName() + " has harmed your enthralled's prestige through incompetence. Prestige goes from " + (int)(prev) + " to " + (sup.prestige));
            }
            else if (p.state == Person.personState.enthralled)
            {
                p.map.world.prefabStore.popMsg("Your enthralled has harmed " + p.getFullName() + "'s prestige through incompetence. Prestige goes from " + (int)(prev) + " to " + (sup.prestige) +
                    ".\nThey will not be pleased.");

            }

            if (sup.society.hasEnthralled())
            {
                bool positive = sup.state != Person.personState.broken;
                p.map.addMessage(p.getFullName() + " has accidentally harmed the prestige of " + sup.getFullName());
            }
        }
    }
}
