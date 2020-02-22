using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Over_SowDissent: Ability
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


            map.world.prefabStore.popImgMsg(
                other.getFullName() + " gains dislike for their sovreign, " + other.society.getSovreign().getFullName() + ".",
                map.world.wordStore.lookup("ABILITY_SOW_DISSENT"));
        }
        public override bool castable(Map map, Person person)
        {
            if (person.state == Person.personState.enthralled) { return false; }
            if (person.society.getSovreign() == null) { return false; }
            if (person == person.society.getSovreign()) { return false; }
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
            return World.staticMap.param.ability_sowDissentCost;
        }
        public override string getDesc()
        {
            return "Causes a noble to gain " + World.staticMap.param.ability_sowDissentLikingChange + " disliking for their sovreign."
                + "\n[Requires a noble with a sovreign]";
        }

        public override string getName()
        {
            return "Sow Dissent";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_convert;
        }
    }
}