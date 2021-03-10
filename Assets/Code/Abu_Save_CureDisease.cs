using UnityEngine;
using System.Collections.Generic;

namespace Assets.Code
{
    public class Abu_Save_CureDisease: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            Society soc = (Society)u.location.soc;

            List<Location> locs = new List<Location>();
            locs.Add(u.location);
            foreach (Location l2 in u.location.getNeighbours())
            {
                locs.Add(l2);
            }
            int nBoosted = 0;
            foreach (Location loc in locs)
            {
                if (loc.person() == null) { continue; }
                bool cured = false;
                List<Property> rems = new List<Property>();
                foreach (Property pr in loc.properties)
                {
                    if (pr.proto.isDisease)
                    {
                        rems.Add(pr);
                    }
                }
                foreach (Property pr in rems)
                {
                    pr.proto.endProperty(loc, pr);
                    cured = true;
                }
                if (cured)
                {
                    double boost = loc.person().threat_plague.threat / 2;
                    loc.person().getRelation(u.person).addLiking(boost, "Gave Miracle Cure", map.turn, RelObj.STACK_REPLACE, true);
                    nBoosted += 1;
                }
            }

            Evidence e2 = new Evidence(map.turn);
            e2.pointsTo = u;
            e2.weight = u.location.map.param.unit_minorEvidence;
            u.location.evidence.Add(e2);

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " heals the peoples of " + u.location.getName() + " and neighbouring settlements, bringing an end to the terrible plague which was spreading across the nation."
                + "\n" + nBoosted + " noble(s) have had their settlements cured and gained liking, proportionally to their fear of disease.",
                u.location.map.world.wordStore.lookup("ABILITY_SAVIOUR_MIRACLE_CURE"), 7) ;

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.soc == null) { return false; }
            if (u.location.settlement == null) { return false; }
            if (u.location.person() == null) { return false; }
            if (u.location.soc is Society == false) { return false; }
            Society soc = (Society)u.location.soc;
            if (soc.voteSession != null) { return false; }
            foreach (Property pr in u.location.properties)
            {
                if (pr.proto.isDisease)
                {
                    return true;
                }
            }
            return false;
        }

        public override void cast(Map map, Hex hex) { }
        public override bool castable(Map map, Hex hex)
        {
            return false;
        }

        public override int getCost()
        {
            return 0;
        }

        public override int getCooldown()
        {
            return 0;
        }

        public override string specialCost()
        {
            return "Minor Evidence";
        }
        public override string getDesc()
        {
            return "Cure a disease at this location and neighbouring ones, to gain a liking boost from affected nobles, proportional to how much they feared the disease."
                + "\n[Requires a location with a noble and a disease]";
        }

        public override string getName()
        {
            return "Miracle Cure";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_saviour;
        }
    }
}