using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Over_UncannyGlamour: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Person other = hex.location.person();
            castInner(map, other);
        }

        public override void castInner(Map map, Person other)
        {
            other.evidence += (map.param.ability_uncannyGlamourEvidence / 100.0);
            other.prestige += map.param.ability_uncannyGlamourGain;
            if (other.evidence > 1) { other.evidence = 1; }

            map.world.prefabStore.popImgMsg(
                other.getFullName() + " gains " + map.param.ability_uncannyGlamourGain + " prestige, and are now at " + (int)other.prestige + ". Their baseline is " + (int)other.targetPrestige
                + " and they will slowly return to that point.",
                map.world.wordStore.lookup("ABILITY_UNCANNY_GLAMOUR"));
        }

        public override bool castable(Map map, Person person)
        {
            if (person.evidence > 0.5) { return false; }
            return true;
        }
        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.person() == null) { return false; }
            return castable(map, hex.location.person());
        }
        public override int getCooldown()
        {
            return World.staticMap.param.ability_uncannyGlamourCooldown;
        }
        public override int getCost()
        {
            return World.staticMap.param.ability_sowDissentCost;
        }
        public override string getDesc()
        {
            return "Causes a noble to gain " + World.staticMap.param.ability_uncannyGlamourGain + " prestige, temporarily, aiding them in upcoming votes. Adds "
                + World.staticMap.param.ability_uncannyGlamourEvidence + "% evidence to them"
                + "\n[Requires a noble with evidence < 50%]";
        }

        public override string getName()
        {
            return "Uncanny Glamour";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_convert;
        }
    }
}