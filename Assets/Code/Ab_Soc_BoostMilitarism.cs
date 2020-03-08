using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Soc_BoostMilitarism: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Person other = hex.location.person();
            castInner(map, other);
        }
        public override void castInner(Map map, Person other)
        {


            other.getRelation(map.overmind.enthralled).addLiking(-World.staticMap.param.ability_changePoliticsLikingCost, "Argued about militarism", map.turn);

            double prev = other.politics_militarism;
            other.politics_militarism = Math.Min(1, other.politics_militarism + 0.25);
            double curr = other.politics_militarism;

            map.world.prefabStore.popImgMsg(
                other.getFullName() + "'s miliarism increases, from " + (int)(100*prev)  + "% to " + (int)(100*curr) + "%",
                map.world.wordStore.lookup("SOC_INCREASE_MILITARISM"));
        }

        public override bool castable(Map map, Person person)
        {
            if (map.overmind.enthralled == null) { return false; }
            if (person.getRelation(map.overmind.enthralled).getLiking() < World.staticMap.param.ability_changePoliticsLikingCost) { return false; }
            if (person.politics_militarism >= 1) { return false; }

            return true;
        }
        public override bool castable(Map map, Hex hex)
        {
            if (map.overmind.enthralled == null) { return false; }
            if (hex.location == null) { return false; }
            if (hex.location.person() == null) { return false; }
            if (hex.location.person() == map.overmind.enthralled) { return false; }
            if (hex.location.soc != map.overmind.enthralled.society) { return false; }
            Person p = hex.location.person();
            return castable(map, p);
        }

        public override string specialCost()
        {
            return "Cost: -" + World.staticMap.param.ability_changePoliticsLikingCost + " liking";
        }
        public override int getCost()
        {
            return 0;
        }

        public override string getDesc()
        {
            return "Argues in support of increased militarism. Makes the chosen noble more warlike, if they like you enough."
                + "\n[Requires a noble in your enthralled's society with a positive attitude towards you enthralled]";
        }

        public override string getName()
        {
            return "Change Politics: +Militarism";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_mask;
        }
    }
}