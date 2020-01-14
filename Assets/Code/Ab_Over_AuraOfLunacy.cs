using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Over_AuraOfLunacy : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Person other = hex.location.person();
            castInner(map, other);
        }

        public override void castInner(Map map, Person other)
        {
            other.getRelation(other.society.getSovreign()).addLiking(map.param.ability_sowDissentLikingChange, "Dissent sown", map.turn);

            int nHit = 0;
            string strHit = "(";
            foreach (Location loc in map.overmind.enthralled.getLocation().getNeighbours()){
                if (loc.person() != null)
                {
                    loc.person().sanity -= map.param.ability_auraOfLunacyHit;
                    if (loc.person().sanity < 0) { loc.person().sanity = 0; }
                    if (nHit != 0) { strHit += "; "; }
                    strHit += loc.person().getFullName();
                    nHit += 1;
                }
            }
            map.overmind.enthralled.evidence += (map.param.ability_auraOfLunacyEvidence / 100d);
            if (map.overmind.enthralled.evidence > 1) { map.overmind.enthralled.evidence = 1; }

            string msg = "Madness seeps out from " + map.overmind.enthralled.getFullName() + ", draining up to " + map.param.ability_auraOfLunacyHit + " sanity from "
                + nHit + " nobles";
            if (nHit != 0) { msg += " " + strHit + ")."; }
            else
            {
                msg += ".";
            }
            map.world.prefabStore.popImgMsg(msg,map.world.wordStore.lookup("ABILITY_AURA_OF_LUNACY"));
        }
        public override bool castable(Map map, Person person)
        {
            if (map.overmind.enthralled == null) { return false; }
            if (map.overmind.enthralled.getLocation() == null) { return false; }
            return true;
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.person() == null) { return false; }
            return castable(map,hex.location.person());
        }
        public override int getCost()
        {
            return World.staticMap.param.ability_auraOfLunacyCost;
        }
        public override string getDesc()
        {
            return "Causes all nobles in locations neighbouring your enthralled's location to lose " + World.staticMap.param.ability_auraOfLunacyHit + " sanity, and adds " +
                    World.staticMap.param.ability_auraOfLunacyEvidence + "% evidence to your enthralled."
                + "\n[Requires an enthralled noble]";
        }

        public override string getName()
        {
            return "Aura of Lunacy";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_moon;
        }
    }
}