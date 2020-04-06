using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Fash_PolariseByFear: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            if (hex.location.soc == null || (hex.location.soc is Society == false)) { return; }
            Society soc = (Society)hex.location.soc;

            int nAffected = 0;
            double totalV = 0;
            foreach (Person p in soc.people)
            {
                bool affected = false;
                ThreatItem t = p.getGreatestThreat();
                foreach (Person p2 in soc.people)
                {
                    if (p2 == p) { continue; }
                    if (p2.state == Person.personState.enthralled) { continue; }
                    foreach (ThreatItem t2 in p2.threatEvaluations)
                    {
                        if (t2.isSame(t))
                        {
                            double delta = t.threat - t2.threat;
                            if (delta > 0)
                            {
                                affected = true;
                                delta *= map.param.ability_polariseByFearMult;
                                p.getRelation(p2).addLiking(-delta, "Polarised by fear of " + t.getTitle(), map.turn);
                                totalV += delta;
                            }
                            break;
                        }
                    }
                }
                if (affected)
                {
                    nAffected += 1;
                }
            }

            double avrg = 0;
            if (nAffected > 0)
            {
                avrg = totalV / nAffected;
            }
            map.world.prefabStore.popImgMsg(
                "You turn the people of " + soc.getName() + " against each other, causing nobles to hate those who do not share their fears.\n" +
                nAffected + " nobles are affected, with an average liking change of of " + (int)(-avrg),
                map.world.wordStore.lookup("ABILITY_POLARISE_BY_FEAR"));
        }
        public override void castInner(Map map, Person person)
        {
            cast(map, person.getLocation().hex);
        }

        public override bool castable(Map map, Person person)
        {
            return true;
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.soc == null || (hex.location.soc is Society == false)) { return false; }
            return true;
        }

        public override int getCooldown()
        {
            return World.staticMap.param.ability_denounceLeaderCooldown;
        }
        public override string specialCost()
        {
            return "";
        }
        public override int getCost()
        {
            return 0;
        }

        public override string getDesc()
        {
            return "Causes all members of a society to dislike each other (except your enthralled) if they do not share the same degree of fear towards their greatest threat, proportional to their fear difference."
                + "\n[Requires a society]";
        }

        public override string getName()
        {
            return "Polarise by Fear";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_axes;
        }
    }
}