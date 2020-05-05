using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Over_EnthrallAgent: Ability
    {
        public override void castInner(Map map, Unit other)
        {
            other.person.state = Person.personState.enthralledAgent;

            map.world.prefabStore.popImgMsg(
                "You take " + other.getName() + " under your control",
                map.world.wordStore.lookup("ABILITY_ENTHRALL_AGENT"));

            Evidence ev = new Evidence(map.turn);
            ev.pointsTo = other;
            ev.weight = 0.34;
            other.location.evidence.Add(ev);

            other.task = null;
            other.movesTaken += 1;
        }
        public override bool castable(Map map, Person person)
        {
            if (person.unit == null) { return false; }
            if (person.state == Person.personState.enthralledAgent) { return false; }

            int nOwned = 0;
            foreach (Unit u in map.units)
            {
                if (u.isEnthralled()) { nOwned += 1; }
            }
            return nOwned < map.param.units_maxEnthralled;
        }

        public override bool castable(Map map, Unit unit)
        {
            if (unit.person == null) { return false; }
            return castable(map,unit.person);
        }

        public override int getCooldown()
        {
            return World.staticMap.param.ability_enthrallUnitCooldown;
        }

        public override bool castable(Map map, Hex hex)
        {
            return false;
        }
        public override int getCost()
        {
            return World.staticMap.param.ability_enthrallUnitCost;
        }
        public override string getDesc()
        {
            return "Enthrall an agent to turn it to your control. Places a piece of evidence pointing to the guilt of your new agent."
                + "\n[Requires a unit. Max " + World.staticMap.param.units_maxEnthralled + "]";
        }

        public override string getName()
        {
            return "Enthrall Unit";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_convert;
        }
    }
}