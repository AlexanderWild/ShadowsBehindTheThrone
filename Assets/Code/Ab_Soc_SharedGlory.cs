using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Soc_SharedGlory : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Person other = hex.location.person();
            castInner(map, other);
        }

        public override void castInner(Map map, Person other)
        {
            map.overmind.enthralled.prestige -= map.param.ability_sharedGloryAmount;
            other.prestige += map.param.ability_sharedGloryAmount;
            other.getRelation(map.overmind.enthralled).addLiking(map.param.ability_sharedGloryLikingGain,"Shared their prestige",map.turn);

            map.world.prefabStore.popImgMsg(
                "Your enthralled transfers " + map.param.ability_sharedGloryAmount + " pretige to " + other.getFullName() +
                            ", increasing their voting capabilities and earning " + map.param.ability_sharedGloryLikingGain + " liking.",
                map.world.wordStore.lookup("ABILITY_SHARED_GLORY"));
        }
        public override bool castable(Map map, Person person)
        {
            if (person.state == Person.personState.enthralled) { return false; }
            if (map.overmind.enthralled == null) { return false; }
            if (map.overmind.enthralled.prestige < map.param.ability_sharedGloryAmount) { return false; }
            return true;
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.person() == null) { return false; }
            if (hex.location.person().state == Person.personState.enthralled) { return false; }
            if (map.overmind.enthralled == null) { return false; }
            if (map.overmind.enthralled.prestige < map.param.ability_sharedGloryAmount) { return false; }
            return true;
        }

        public override string specialCost()
        {
            return "" + World.staticMap.param.ability_sharedGloryAmount + " prestige";
        }
        public override int getCost()
        {
            return 0;
        }

        public override int getCooldown()
        {
            return 0;
        }
        public override string getDesc()
        {
            return "Gives " + World.staticMap.param.ability_sharedGloryAmount + " of your enthralled's prestige to a target noble, advantaging their voting and gaining "
                    + World.staticMap.param.ability_sharedGloryLikingGain + " liking."
                + "\n[Requires your enthralled to have more than " + World.staticMap.param.ability_sharedGloryAmount + " prestige]";
        }

        public override string getName()
        {
            return "Shared Glory";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_mask;
        }
    }
}