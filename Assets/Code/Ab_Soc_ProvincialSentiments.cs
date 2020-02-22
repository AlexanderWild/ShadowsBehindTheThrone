using UnityEngine;

using System;
using System.Collections.Generic;

namespace Assets.Code
{
    public class Ab_Soc_ProvincialSentiments: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
                Person other = hex.location.person();
            castInner(map, other);
        }

        public override void castInner(Map map, Person other)
        {

            foreach (Person p in map.overmind.enthralled.society.people)
            {
                foreach (Person p2 in map.overmind.enthralled.society.people)
                {
                    if (p.getLocation().province == p2.getLocation().province)
                    {
                        p.getRelation(p2).addLiking(map.param.ability_provincialSentimentLikingChangePositive, "Provincial Sentiments", map.turn);
                    }
                    else
                    {
                        p.getRelation(p2).addLiking(map.param.ability_provincialSentimentLikingChangeNegative, "Provincial Sentiments", map.turn);
                    }
                }
            }

            map.world.prefabStore.popImgMsg(
                "Your enthralled stirs up the isolationist and xenophobic spirits of the people, both noble and common, rallying them around their province, against their fellow countrymen.",
                map.world.wordStore.lookup("ABILITY_PROVINCIAL_SENTIMENTS"));
        }
        public override bool castable(Map map, Person person)
        {
            if (map.overmind.enthralled == null) { return false; }

            HashSet<Province> provinces = new HashSet<Province>();
            foreach (Person p in map.overmind.enthralled.society.people)
            {
                provinces.Add(p.getLocation().province);
                if (provinces.Count > 1) { return true; }
            }
            return false;
        }

        public override bool castable(Map map, Hex hex)
        {
            if (map.overmind.enthralled == null) { return false; }
            return castable(map, map.overmind.enthralled);

        }

        public override string specialCost()
        {
            return "";
        }
        public override int getCost()
        {
            return 0;
        }

        public override int getCooldown()
        {
            return World.staticMap.param.ability_provincialSentimentLikingChangeCooldown;
        }
        public override string getDesc()
        {
            return "Causes the nobles in a nation to gain " + (int)(World.staticMap.param.ability_provincialSentimentLikingChangePositive) + " liking for those in their own province, and "
                + " lose " + (int)(World.staticMap.param.ability_provincialSentimentLikingChangeNegative) + " for all others."
                + "\n[Requires an enthralled in a nation with more than one province]";
        }

        public override string getName()
        {
            return "Provincial Sentiments";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_mask;
        }
    }
}