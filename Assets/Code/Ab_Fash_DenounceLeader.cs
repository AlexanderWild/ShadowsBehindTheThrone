using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Fash_DenounceLeader: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            if (hex.location.soc == null || (hex.location.soc is Society == false)) { return; }
            ThreatItem item = hex.location.person().getGreatestThreat();
            if (item == null) { return; }

            Society soc = (Society)hex.location.soc;
            Person sov = soc.getSovreign();
            if (sov == null) { return; }
            int nAffected = 0;
            double totalV = 0;
            double sovFear = 0;
            foreach (ThreatItem ti in sov.threatEvaluations)
            {
                if (item.isSame(ti))
                {
                    sovFear = ti.threat;
                    break;
                }
            }
            foreach (Person p in soc.people)
            {
                double myFear = 0;
                foreach (ThreatItem ti in p.threatEvaluations)
                {
                    if (item.isSame(ti))
                    {
                        myFear = ti.threat;
                        break;
                    }
                }

                double deltaFear = myFear - sovFear;
                if (deltaFear <= 0) { continue; }

                double deltaLiking = deltaFear * map.param.ability_denounceLeaderLikingMult;
                deltaLiking = Math.Min(deltaLiking, map.param.ability_denounceLeaderMax);

                p.getRelation(sov).addLiking(-deltaLiking, "Doesn't take threat of " + item.getTitle() + " seriously", map.turn);

                nAffected += 1;
                totalV += deltaFear;
            }

            double avrg = 0;
            if (nAffected > 0)
            {
                avrg = totalV / nAffected;
            }
            map.world.prefabStore.popImgMsg(
                "You rally the people against " + sov.getFullName() + ", denouncing them as unable to defend the people against the threat of " + item.getTitle() + ".\n" +
                nAffected + " nobles agree, with an average liking change of of " + (int)(-avrg),
                map.world.wordStore.lookup("ABILITY_DENOUNCE_LEADER"));
        }
        public override void castInner(Map map, Person person)
        {
            cast(map, person.getLocation().hex);
        }

        public override bool castable(Map map, Person person)
        {
            
            if (person.society.getSovreign() == null) { return false; }
            if (person == person.society.getSovreign()) { return false; }
            if (person.state == Person.personState.enthralled) { return false; }

            return true;
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.soc == null || (hex.location.soc is Society == false)) { return false; }
            if (hex.location.person() == null) { return false; }
            return castable(map, hex.location.person());
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
            return "Select a person. All people in this society grow dislike of their sovreign if the sovreign fears this person's fear less than they do."
                + "\n[Requires a person who is neither sovreign nor enthralled]";
        }

        public override string getName()
        {
            return "Denounce Leader";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_axes;
        }
    }
}