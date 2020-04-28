using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Over_DisruptAction: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            castInner(map, hex.location.person());
        }

        public override void castInner(Map map, Person person)
        {

            person.action = new Act_Disrupted();

            map.world.prefabStore.popImgMsg(
                "You disrupt the efforts of " + person.getFullName() + ". They will need time to re-establish themselves and undo the chaos you've sown before they can act again.",
                map.world.wordStore.lookup("ABILITY_DISRUPT_ACTION"));
        }

        public override bool castable(Map map, Person person)
        {
            return person.action != null;
        }
        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.person() == null) { return false; }
            return castable(map,hex.location.person());
        }
        
        public override int getCost()
        {
            return World.staticMap.param.ability_disruptActionCost;
        }

        public override string getDesc()
        {
            return "Causes a noble to cancel their current action, then adds " + World.staticMap.param.ability_disruptActionDuration + " turns of disruption before they can act again."
                + "\n[Requires a noble performing an action]";
        }

        public override string getName()
        {
            return "Disrupt Action";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_convert;
        }
    }
}