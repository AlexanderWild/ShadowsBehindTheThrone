using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Over_DisruptAgent: Ability
    {
        public override void castInner(Map map, Unit other)
        {
            map.world.prefabStore.popImgMsg(
                "You disrupt the work of " + other.getName() + ". They will require some time before they can act again.",
                map.world.wordStore.lookup("ABILITY_DISRUPT_AGENT"));
            

            other.task = new Task_Disrupted();
            other.movesTaken += 1;
        }
        public override bool castable(Map map, Person person)
        {
            return false;
        }

        public override bool castable(Map map, Unit unit)
        {
            return true;
        }

        public override int getCooldown()
        {
            return 0;
        }

        public override bool castable(Map map, Hex hex)
        {
            return false;
        }
        public override int getCost()
        {
            return World.staticMap.param.ability_disruptAgentCost;
        }
        public override string getDesc()
        {
            return "Disrupts an agent/unit, preventing them from taking action for " + World.staticMap.param.unit_disruptDuration+ " turns."
                + "\n[Requires a unit]";
        }

        public override string getName()
        {
            return "Disrupt Unit";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_convert;
        }
    }
}